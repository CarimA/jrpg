using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.GameComponents
{
    public class TransitionManager : DrawableGameComponent
    {
        public new MainGame Game => (MainGame)base.Game;
        private Texture2D _pixel;

        public TransitionManager(MainGame game) : base(game)
        {
            game.Components.Add(this);
            this.DrawOrder = 999;
        }

        public void CreateFade(Action<Action> action)
        {
            action(() =>
            {
                // set complete here and unfade
            });
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData<Color>(new Color[1] { Color.White });
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Game.SpriteBatch.Begin();
            Game.SpriteBatch.Draw(_pixel, new Rectangle(0, 0, MainGame.GAME_WIDTH, MainGame.GAME_HEIGHT), Color.DarkSlateGray);// * opac);
            Game.SpriteBatch.End();
        }
    }
}
