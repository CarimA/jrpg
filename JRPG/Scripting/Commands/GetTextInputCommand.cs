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

        Texture2D textbox;

        public GetTextInputCommand(ScriptingManager manager, Engine engine) : base(manager, engine)
        {
            textbox = Game.Content.Load<Texture2D>("interfaces/textui");
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
                        if (input.Length > 0)
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

        public override void DrawUI(GameTime gameTime)
        {

            //Game.SpriteBatch.Draw(Game.Assets.Textures["black"], new Rectangle(0, 0, 400, 240), Color.White * 0.5f);

            BitmapFont font = MainGame.Font;

            Game.SpriteBatch.Draw(textbox, Vector2.Zero, Color.White);
            Game.SpriteBatch.Draw(textbox, Vector2.Zero, new Rectangle(24*5, 0, 24*5, 24), Color.White);


            int row = 0;
            int column = 0;

            int left = 37+6;
            int top = 55+17;
            int gutter = 24;

            int highlight_width = y == 5 ? 24 * 4 : 24;
            int highlight_x = y == 5 ? 0 : 96;

            Game.SpriteBatch.Draw(textbox, new Rectangle(left - 7 + (x * 24), top - 6 + (y * 24), highlight_width, 24), new Rectangle(highlight_x, 0, highlight_width, 24), Color.White);

            Game.SpriteBatch.Draw(textbox, new Rectangle(left, top, 300, 132), new Rectangle(0, 216, 300, 132), Color.White);

            /*
            Color highlight = Color.White;

            foreach (char k in keys)
            {
                highlight = Color.Black;
                if (row == y && column == x)
                {
                    highlight = Color.Yellow;
                }

                Vector2 charSize = font.MeasureString(k.ToString());

                Game.SpriteBatch.DrawString(font, k.ToString(), new Vector2((int)(left + column * gutter + (charSize.X / 2)), (int)(top + row * gutter + (charSize.Y / 2))), highlight);

                column++;
                if (column >= 13)
                {
                    row++;
                    column = 0;
                }
            }

            highlight = Color.White;
            if (y == 5 && (x >= 1 && x <= 4))
            {
                highlight = Color.Yellow;
            }
            //Game.SpriteBatch.Draw(Game.Assets.Textures["inputbigbutton"], new Vector2(122 + 8, 150), Color.White);
            Vector2 spaceSize = font.MeasureString("SPACE");
            Game.SpriteBatch.DrawString(font, "SPACE", new Vector2((int)(left + gutter + (spaceSize.X / 2)), (int)(top + (gutter * 5) + (spaceSize.Y / 2))), highlight);

            highlight = Color.White;
            if (y == 5 && (x >= 5 && x <= 8))
            {
                highlight = Color.Yellow;
            }
            //Game.SpriteBatch.Draw(Game.Assets.Textures["inputbigbutton"], new Vector2(122 + (12 * 4) + 8, 150), Color.White);
            Vector2 deleteSize = font.MeasureString("DELETE");
            Game.SpriteBatch.DrawString(font, "DELETE", new Vector2((int)(left + (gutter * 4) + (deleteSize.X / 2)), (int)(top + (gutter * 5) + (deleteSize.Y / 2))), highlight);

            highlight = Color.White;
            if (y == 5 && (x >= 9 && x <= 12))
            {
                highlight = Color.Yellow;
            }
            //Game.SpriteBatch.Draw(Game.Assets.Textures["inputbigbutton"], new Vector2(122 + (12 * 8) + 8, 150), Color.White);
            Vector2 enterSize = font.MeasureString("ENTER");
            Game.SpriteBatch.DrawString(font, "ENTER", new Vector2((int)(left + (gutter * 8) + (enterSize.X / 2)), (int)(top + (gutter * 5) + (enterSize.Y / 2))), highlight);
            //spriteBatch.DrawString(font, "@", new Vector2(122 + x * 12, 90 + y * 12), Color.HotPink);*/

            Game.SpriteBatch.DrawString(font, title, new Vector2((int)((MainGame.GAME_WIDTH / 2) - (font.MeasureString(title).Width / 2)), top - 62), Color.Yellow);

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
