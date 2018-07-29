using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;
using JRPG.EntityComponent.Components;
using Microsoft.Xna.Framework;
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

        public GetTextInputCommand(ScriptingManager manager, Engine engine) : base(manager, engine)
        {

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

            Game.SpriteBatch.DrawString(font, title, new Vector2((int)(200 - (font.MeasureString(title).Width / 2)), 40), Color.White);

            int row = 0;
            int column = 0;
            Color highlight = Color.White;

            foreach (char k in keys)
            {
                highlight = Color.White;
                if (row == y && column == x)
                {
                    highlight = Color.Yellow;
                }

                Game.SpriteBatch.DrawString(font, k.ToString(), new Vector2(122 + column * 12, 90 + row * 12), highlight);

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
            Game.SpriteBatch.DrawString(font, "SPACE", new Vector2(122 + 8 + 24 - (font.MeasureString("SPACE").Width / 2), 150), highlight);

            highlight = Color.White;
            if (y == 5 && (x >= 5 && x <= 8))
            {
                highlight = Color.Yellow;
            }
            //Game.SpriteBatch.Draw(Game.Assets.Textures["inputbigbutton"], new Vector2(122 + (12 * 4) + 8, 150), Color.White);
            Game.SpriteBatch.DrawString(font, "DELETE", new Vector2(122 + (12 * 4) + 8 + 24 - (font.MeasureString("DELETE").Width / 2), 150), highlight);

            highlight = Color.White;
            if (y == 5 && (x >= 9 && x <= 12))
            {
                highlight = Color.Yellow;
            }
            //Game.SpriteBatch.Draw(Game.Assets.Textures["inputbigbutton"], new Vector2(122 + (12 * 8) + 8, 150), Color.White);
            Game.SpriteBatch.DrawString(font, "ENTER", new Vector2(122 + (12 * 8) + 8 + 24 - (font.MeasureString("ENTER").Width / 2), 150), highlight);
            //spriteBatch.DrawString(font, "@", new Vector2(122 + x * 12, 90 + y * 12), Color.HotPink);

            int calcX = (400 / 2) - ((length * 12) / 2);
            for (int i = 0; i < length; i++)
            {
                if (i >= input.Length)
                {
                    Game.SpriteBatch.DrawString(font, "_", new Vector2(calcX + (12 * i), 55), Color.White);
                }
                else
                {
                    Game.SpriteBatch.DrawString(font, input[i].ToString(), new Vector2(calcX + (12 * i), 55), Color.White);
                }
            }
        }
    }
}
