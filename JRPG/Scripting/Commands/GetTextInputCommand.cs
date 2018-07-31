using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;
using JRPG.EntityComponent.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace JRPG.Scripting.Commands
{
    public class GetTextInputCommand : Command
    {
        public override string Name => "get_text_input";

        bool held;

        char[] keys = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-!?".ToCharArray();
        string title;
        string input;
        int length;

        int x = 0;
        int y = 0;

        BitmapFont font;
        Texture2D atlas;

        public GetTextInputCommand(ScriptingManager manager, Engine engine) : base(manager, engine)
        {
            atlas = Game.Assets.Get<Texture2D>(AssetManager.Asset.InterfacesAtlas);
            font = Game.Assets.Get<BitmapFont>(AssetManager.Asset.Font);
        }

        public override object Action(params object[] args)
        {
            SetCommand();
            PlayerData.InControl = false;
            held = true;

            input = "";
            title = "";
            length = 8;

            x = 0;
            y = 0;

            if (args.Count() > 0)
            {
                title = (string)args[0];
            }
            if (args.Count() > 1)
            {
                length = int.Parse((string)args[1]);
            }
            if (args.Count() > 2)
            {
                input = (string)args[2];
            }

            while (held) ;

            ClearCommand();
            PlayerData.InControl = true;
            return input;
        }

        public override void Update(GameTime gameTime)
        {
            InputComponent pin = Player.GetComponent<InputComponent>();
            string pol = pin.PollPress();

            if (pol == "right")
            {
                if (y == 5)
                    x += 4;
                else
                    x++;
                if (y == 5)
                {
                    if (x > 12)
                        x = 1;
                }
                else
                {
                    if (x > 12)
                        x = 0;
                }
            }
            if (pol == "left")
            {
                if (y == 5)
                    x -= 4;
                else
                    x--;
                if (y == 5)
                {
                    if (x <= 0)
                        x = 12;
                }
                else
                {
                    if (x < 0)
                        x = 12;
                }
            }
            if (pol == "down")
            {
                y++;
                if (x == 0)
                {
                    if (y > 4)
                        y = 0;
                }
                else
                {
                    if (y > 5)
                        y = 0;
                }
            }

            if (pol == "up")
            {
                y--;
                if (x == 0)
                {
                    if (y < 0)
                        y = 4;
                }
                else if (y < 0)
                {
                    y = 5;
                }
            }

            if (y == 5)
            {
                if (x >= 1 && x <= 4)
                {
                    x = 1;
                }
                if (x >= 5 && x <= 8)
                {
                    x = 5;
                }
                if (x >= 9 && x <= 12)
                {
                    x = 9;
                }
            }

            if (pol == "action")
            {
                // pos = (y * 13) + x
                if (y < 5)
                {
                    if (input.Length < length)
                    {
                        input += keys[(y * 13) + x];
                    }
                }
                else
                {
                    if (x >= 1 && x <= 4)
                    {
                        input += " ";
                    }
                    if (x >= 5 && x <= 8)
                    {
                        if (input.Length > 0)
                        {
                            input = input.Substring(0, input.Length - 1);
                        }
                    }
                    if (x >= 9 && x <= 12)
                    {
                        if (input.Length > 0 && !string.IsNullOrWhiteSpace(input))
                        {
                            held = false;
                        }
                    }
                }
            }
        }

        public override void DrawMask(GameTime gameTime)
        {

        }

        public override void DrawFringe(GameTime gameTime)
        {

        }

        private void DrawTextCentered(string text, int left, int top, int width, int height, Color color)
        {
            Vector2 textSize = font.MeasureString(text);
            Game.SpriteBatch.DrawString(font, text, new Vector2((int)(left + (width / 2) - (textSize.X / 2)), (int)(top + (height / 2) - (textSize.Y / 2))), color);
        }

        public override void DrawUI(GameTime gameTime)
        { 
            // reconstruct whole thing
            Game.SpriteBatch.Draw(atlas, new Rectangle(0, 0, MainGame.GAME_WIDTH, MainGame.GAME_HEIGHT), new Rectangle(22, 0, 3, 3), Color.White);
            Game.SpriteBatch.Draw(atlas, new Rectangle(0, 50, MainGame.GAME_WIDTH, 19), new Rectangle(46, 0, 1, 19), Color.White);

            Game.SpriteBatch.Draw(atlas, new Rectangle(35, 65, 314, 1), new Rectangle(22, 3, 3, 1), Color.White);
            Game.SpriteBatch.Draw(atlas, new Rectangle(33, 68, 3, 1), new Rectangle(22, 4, 3, 1), Color.White);
            Game.SpriteBatch.Draw(atlas, new Rectangle(MainGame.GAME_WIDTH - 36, 68, 3, 1), new Rectangle(22, 5, 3, 1), Color.White);

            for (int i = 0; i < 5; i++)
            {
                Game.SpriteBatch.Draw(atlas, new Rectangle(6 * i, 71, 6, 11), new Rectangle(34, 0, 6, 11), Color.White);
                Game.SpriteBatch.Draw(atlas, new Rectangle(MainGame.GAME_WIDTH - 6 - (6 * i), 71, 6, 11), new Rectangle(35, 0, 6, 11), Color.White);
            }
            Game.SpriteBatch.Draw(atlas, new Rectangle(30, 71, 6, 11), new Rectangle(40, 0, 6, 11), Color.White);
            Game.SpriteBatch.Draw(atlas, new Rectangle(MainGame.GAME_WIDTH - 36, 71, 6, 11), new Rectangle(47, 0, 6, 11), Color.White);

            Game.SpriteBatch.DrawNineSlice(atlas, Color.White, new Rectangle(30, 83, 324, 109), new Rectangle(34, 11, 9, 9));

            Game.SpriteBatch.Draw(atlas, new Rectangle(33, 83, 3, 1), new Rectangle(22, 4, 3, 1), Color.White);
            Game.SpriteBatch.Draw(atlas, new Rectangle(57, 191, 3, 1), new Rectangle(22, 4, 3, 1), Color.White);
            Game.SpriteBatch.Draw(atlas, new Rectangle(MainGame.GAME_WIDTH - 36, 83, 3, 1), new Rectangle(22, 5, 3, 1), Color.White);
            Game.SpriteBatch.Draw(atlas, new Rectangle(MainGame.GAME_WIDTH - 36, 191, 3, 1), new Rectangle(22, 5, 3, 1), Color.White);

            int top = 55 + 17;
            int gutter = 24;
            for (int lx = 0; lx < 13; lx++)
            {
                for (int ly = 0; ly < 5; ly++)
                {
                    if (x == lx && y == ly)
                    {
                        Game.SpriteBatch.DrawNineSlice(atlas, Color.White, new Rectangle(36 + (lx * gutter), 66 + (ly * gutter), gutter, gutter), new Rectangle(25, 0, 9, 9));
                    } 
                    else
                    {
                        Game.SpriteBatch.DrawNineSlice(atlas, Color.White, new Rectangle(36 + (lx * gutter), 66 + (ly * gutter), gutter, gutter), new Rectangle(25, 9, 9, 9));
                    }
                }
            };

            if (y == 5 && (x >= 1 && x <= 4))
            {
                Game.SpriteBatch.DrawNineSlice(atlas, Color.White, new Rectangle(60, 186, gutter * 4, gutter), new Rectangle(25, 0, 9, 9));
            }
            else
            {
                Game.SpriteBatch.DrawNineSlice(atlas, Color.White, new Rectangle(60, 186, gutter * 4, gutter), new Rectangle(25, 9, 9, 9));
            }

            if (y == 5 && (x >= 4 && x <= 8))
            {
                Game.SpriteBatch.DrawNineSlice(atlas, Color.White, new Rectangle(60 + (gutter * 4), 186, gutter * 4, gutter), new Rectangle(25, 0, 9, 9));
            }
            else
            {
                Game.SpriteBatch.DrawNineSlice(atlas, Color.White, new Rectangle(60 + (gutter * 4), 186, gutter * 4, gutter), new Rectangle(25, 9, 9, 9));
            }

            if (y == 5 && (x >= 8 && x <= 12))
            {
                Game.SpriteBatch.DrawNineSlice(atlas, Color.White, new Rectangle(60 + (gutter * 8), 186, gutter * 4, gutter), new Rectangle(25, 0, 9, 9));
            }
            else
            {
                Game.SpriteBatch.DrawNineSlice(atlas, Color.White, new Rectangle(60 + (gutter * 8), 186, gutter * 4, gutter), new Rectangle(25, 9, 9, 9));
            }

            int row = 0;
            int column = 0;

            foreach (char k in keys)
            {                                
                DrawTextCentered(k.ToString(), 36 + column * gutter, 67 + row * gutter, gutter, gutter, Color.White);

                column++;
                if (column >= 13)
                {
                    row++;
                    column = 0;
                }
            }

            DrawTextCentered("SPACE", 60, 187, gutter * 4, gutter, Color.White);
            DrawTextCentered("DELETE", 60 + (gutter * 4), 187, gutter * 4, gutter, Color.White);
            DrawTextCentered("CONFIRM", 60 + (gutter * 8), 187, gutter * 4, gutter, Color.White);

            Game.SpriteBatch.DrawString(font, title, new Vector2((int)((MainGame.GAME_WIDTH / 2) - (font.MeasureString(title).Width / 2)), top - 62), Color.Lerp(Color.Yellow, Color.Red, (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 2f)));
            int calcX = (MainGame.GAME_WIDTH / 2) - ((length * 12) / 2);
            for (int i = 0; i < length; i++)
            {
                if (i >= input.Length)
                {
                    Game.SpriteBatch.DrawString(font, "_", new Vector2(calcX + (12 * i), top - 45), Color.White);
                }
                else
                {
                    Game.SpriteBatch.DrawString(font, input[i].ToString(), new Vector2(calcX + (12 * i), top - 45), Color.White);
                }
            }
        }
    }
}
