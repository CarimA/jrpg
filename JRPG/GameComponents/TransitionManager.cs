using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.GameComponents
{
    public enum TransitionState
    {
        On,
        Off,
        Waiting,
        Complete
    }

    public class TransitionManager : DrawableGameComponent
    {
        public new MainGame Game => (MainGame)base.Game;
        private Texture2D _pixel;

        public TimeSpan Timing = TimeSpan.Zero;
        public float Interpolation = 1f;
        public TransitionState State = TransitionState.Complete;
        public bool IsComplete = false;

        private float _timing;

        BitmapFont font;

        public TransitionManager(MainGame game) : base(game)
        {
            game.Components.Add(this);
            this.DrawOrder = 999;
        }

        public void CreateFade(TimeSpan timing, Action<Action> action)
        {
            if (State != TransitionState.Complete)
            {
                throw new Exception("Cannot have multiple fades");
            }

            Timing = timing;
            State = TransitionState.On;
            IsComplete = false;
            Interpolation = 0f;
            Task.Factory.StartNew(() =>
            {
                action(() =>
                {
                    // set complete here and unfade
                    IsComplete = true;
                });
            });
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData<Color>(new Color[1] { Color.White });
            font = Game.Assets.Get<BitmapFont>(AssetManager.Asset.Font);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (State == TransitionState.On)
            {
                if (UpdateTransition(gameTime, Timing, 1))
                {
                    State = TransitionState.Waiting;
                }
            }
            else if (State == TransitionState.Off)
            {
                if (UpdateTransition(gameTime, Timing, -1))
                {
                    State = TransitionState.Complete;
                }
            }
            else if (State == TransitionState.Waiting)
            {
                if (IsComplete)
                {
                    State = TransitionState.Off;
                }
            }
        }

        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            float deltaTransition;
            if (time == TimeSpan.Zero)
                deltaTransition = 1;
            else
                deltaTransition = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

            // todo: check if this is actually keeping to the time.
            Interpolation = MathHelper.SmoothStep(Interpolation, Interpolation + (deltaTransition * direction), (float)gameTime.ElapsedGameTime.TotalMilliseconds);

            //TransitionPosition += deltaTransition * direction;
            if (((direction < 0) && (Interpolation <= 0)) ||
                ((direction > 0) && (Interpolation >= 1)))
            {
                Interpolation = MathHelper.Clamp(Interpolation, 0, 1);
                return true;
            }

            return false;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (State == TransitionState.Complete)
            {
                return;
            }

            Game.SpriteBatch.Begin();
            Game.SpriteBatch.Draw(_pixel, new Rectangle(0, 0, MainGame.GAME_WIDTH, MainGame.GAME_HEIGHT), Color.Black * Interpolation);

            if (State == TransitionState.Waiting)
            {
                Game.SpriteBatch.DrawString(font, "Loading...", new Vector2(60, 60), Color.White);
            }
            Game.SpriteBatch.End();
        }
    }
}
