using JRPG.EntityComponent;
using JRPG.EntityComponent.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG
{
    public class Camera : GameComponent
    {
        public Matrix ViewProjectionTransform;

        private bool isDirty = true;
        public Matrix ProjectionTransform { get; private set; } 

        private Vector2 virtualResolution { get => new Vector2(384, 216); }
        private float aspectRatio { get => virtualResolution.X / virtualResolution.Y; }
        private float scale;

        public Matrix ViewTransform { get; private set; }
        public float Rotation { get; set; } = 0f;
        public float Zoom { get; set; } = 1f;
        public Vector2 Translate { get; set; } = Vector2.Zero;

        private Entity target;

        public Rectangle VisibleArea
        {
            get
            {
                var inverseViewMatrix = Matrix.Invert(ViewTransform);
                var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
                var tr = Vector2.Transform(new Vector2(virtualResolution.X, 0), inverseViewMatrix);
                var bl = Vector2.Transform(new Vector2(0, virtualResolution.Y), inverseViewMatrix);
                var br = Vector2.Transform(virtualResolution, inverseViewMatrix);
                var min = new Vector2(
                    MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                    MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
                var max = new Vector2(
                    MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                    MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
                return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
            }
        }

        public Camera(Game game) : base(game)
        {
            game.Components.Add(this);
            game.Window.ClientSizeChanged += (object sender, EventArgs e) =>
            {
                UpdateProjection();
            };
            UpdateProjection();
        }

        private void UpdateProjection()
        {
            float width = Game.GraphicsDevice.Viewport.Width / virtualResolution.X;
            float height = Game.GraphicsDevice.Viewport.Height / virtualResolution.Y;
            scale = Math.Min(width, height);

            ProjectionTransform = Matrix.CreateScale(new Vector3(scale, scale, 1)) *
                Matrix.CreateTranslation(new Vector3(Game.GraphicsDevice.Viewport.Width * 0.5f, Game.GraphicsDevice.Viewport.Height * 0.5f, 0));
        }

        public void UpdateView()
        {
            ViewTransform = Matrix.CreateTranslation(new Vector3(-Translate.X, -Translate.Y, 0)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 1));
        }

        public void SetTarget(Entity entity)
        {
            if (entity.HasComponent<PositionComponent>())
                target = entity;
        }

        public override void Update(GameTime gameTime)
        { 
            if (target != null)
            {
                Translate = target.GetComponent<PositionComponent>().GetPosition();
            }

            UpdateView();

            ViewProjectionTransform = ViewTransform * ProjectionTransform;

            base.Update(gameTime);
        }

        public Vector2 WorldToScreen(Vector2 position) => Vector2.Transform(position, ViewProjectionTransform);
        public Vector2 ScreenToWorld(Vector2 position) => Vector2.Transform(position, Matrix.Invert(ViewProjectionTransform));

        public bool IsInBounds(Rectangle rect) => VisibleArea.Contains(rect) || VisibleArea.Intersects(rect);
    }
}
