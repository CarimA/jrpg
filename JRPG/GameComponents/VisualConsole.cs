using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.GameComponents
{
    public struct ConsoleLine
    {
        public string Text;
        public Color ForegroundColor;
        public Color BackgroundColor;

        public void Draw(SpriteBatch spriteBatch, Texture2D bg, BitmapFont font, int index)
        {
            spriteBatch.Draw(bg, new Rectangle(23, 23 + (16 * index), 604, 16), BackgroundColor * 0.5f);
            spriteBatch.DrawString(font, Text, new Vector2(25, 23 + 16 * index), ForegroundColor);
        }

        public ConsoleLine(string text)
        {
            Text = text;
            ForegroundColor = Color.White;
            BackgroundColor = Color.Black;
        }

        public ConsoleLine(string text, Color foregroundColor, Color backgroundColor)
        {
            Text = text;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
        }
    }

    public class VisualConsole : TextWriter
    {
        // todo: change to array with colour info
        public readonly IndexedQueue<ConsoleLine> Text;

        public readonly MainGame Game;
        public override Encoding Encoding => Encoding.ASCII;

        public bool Visible = true;

        int maxLines = 100;
        int displayLineTotal = 16;
        int cursor = 0;

        string input = "";

        BitmapFont ds;
        // todo: change to monospaced font
        Texture2D _pixel;

        Keys[] keys;
        bool[] IskeyUp;
        string[] SC = { ")", "!", "\"", "£", "$", "%", "^", "&", "*", "(" };//special characters


        public VisualConsole(MainGame game)
        {
            Game = game;
            Text = new IndexedQueue<ConsoleLine>(maxLines);
            Console.SetOut(this);



            // todo: figure out a good input handler
            keys = new Keys[42];
            Keys[] tempkeys;
            tempkeys = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToArray<Keys>();
            int j = 0;
            for (int i = 0; i < tempkeys.Length; i++)
            {
                if (i == 1 || i == 11 || (i > 26 && i < 63))//get the keys listed above as well as A-Z
                {
                    keys[j] = tempkeys[i];//fill our key array
                    j++;
                }
            }
            keys[38] = Keys.OemPeriod;
            keys[39] = Keys.OemMinus;
            keys[40] = Keys.OemPlus;
            keys[41] = Keys.OemComma;
            IskeyUp = new bool[keys.Length]; //boolean for each key to make the user have to release the key before adding to the string
            for (int i = 0; i < keys.Length; i++)
                IskeyUp[i] = true;
        }

        public void Initialise()
        {
            ds = Game.Assets.Get<BitmapFont>(AssetManager.Asset.FontDebug);
            _pixel = new Texture2D(Game.GraphicsDevice, 1, 1);
            _pixel.SetData<Color>(new Color[1] { Color.White });
        }

        public override void Write(string value)
        {
            // todo: figure out an actual write implementation
            WriteLine(value);
        }

        public void AddLine(string value, Color fg, Color bg)
        {
            Text.Enqueue(new ConsoleLine(value, fg, bg));
            cursor = Math.Max(0, Text.Count - displayLineTotal);

            if (Text.Count > maxLines)
            {
                Text.Dequeue();
            }
        }

        public override void WriteLine(string value)
        {
            AddLine(value, Color.White, Color.Black);
        }

        public void WriteWarning(string value)
        {
            AddLine(value, Color.Yellow, Color.Black);
        }

        public void WriteError(string value)
        {
            AddLine(value, Color.Red, Color.Black);
        }

        public void WriteSuccess(string value)
        {
            AddLine(value, Color.LimeGreen, Color.Black);
        }

        public void ToggleVisible()
        {
            Visible = !Visible;
        }

        private void MoveCursor(int amount)
        {
            if (Visible)
            {
                cursor += amount;

                if (cursor < 0)
                {
                    cursor = 0;
                }

                if (cursor > Text.Count - displayLineTotal)
                {
                    cursor = Text.Count - displayLineTotal;
                }
            }
        }

        // todo: change to use pgup/pgdown and/or scrollwheel
        // todo: home to top, end to bottom
        public void MoveCursorUp() => MoveCursor(-1);
        public void MoveCursorDown() => MoveCursor(1);

        public void Clear()
        {
            
        }

        public void Update()
        {
            if (!Visible)
                return;

            KeyboardState state = Keyboard.GetState();
            int i = 0;

            foreach (Keys key in keys)
            {
                if (state.IsKeyDown(key))
                {
                    if (IskeyUp[i])
                    {
                        if (key == Keys.Back && input != "") input = input.Remove(input.Length - 1);
                        if (key == Keys.Space) input += " ";
                        if (i > 1 && i < 12)
                        {
                            if (state.IsKeyDown(Keys.RightShift) || state.IsKeyDown(Keys.LeftShift))
                                input += SC[i - 2];//if shift is down, and a number is pressed, using the special key
                            else input += key.ToString()[0];
                        }
                        if (i > 11 && i < 38)
                        {
                            if (state.IsKeyDown(Keys.RightShift) || state.IsKeyDown(Keys.LeftShift))
                                input += key.ToString();
                            else input += key.ToString().ToLower(); //return the lowercase char is shift is up.
                        }
                        if (i == 38)
                        {
                            input += ".";
                        }
                        if (i == 39)
                        {
                            if (state.IsKeyDown(Keys.RightShift) || state.IsKeyDown(Keys.LeftShift))
                                input += "_";
                            else
                                input += "-";
                        }
                        if (i == 40)
                        {
                            if (state.IsKeyDown(Keys.RightShift) || state.IsKeyDown(Keys.LeftShift))
                                input += "+";
                            else
                                input += "=";
                        }
                        if (i == 41)
                        {
                            input += ",";
                        }
                    }
                    IskeyUp[i] = false; //make sure we know the key is pressed
                }
                else if (state.IsKeyUp(key)) IskeyUp[i] = true;
                i++;
            }

            if (state.IsKeyDown(Keys.Enter))
            {
                if (input != "")
                {
                    WriteSuccess(" >> " + input);
                    try
                    {
                        // todo: display input in console
                        Game.ScriptingManager.Execute(input);
                    }
                    catch (Exception e)
                    {
                        WriteError(e.Message);
                    }
                    input = "";
                }
            }
        }

        public void Draw()
        {
            if (!Visible)
                return;


            //float scrollbar = Math.Min(224, 224 * ((displayLineTotal * 14) / (float)(Text.Count * 14)));
            // todo: fix when I care
            //float scrollbarY = (cursor * scrollbar) / (float)maxLines;
            //Game.SpriteBatch.Draw(_pixel, new Rectangle(625, 25 + (int)scrollbarY, 25, (int)scrollbar), Color.Black * 0.5f);

            int i = 0;
            for (int ic = Math.Max(0, cursor); ic < Math.Min(Text.Count, cursor + displayLineTotal); ic++)
            {
                Text[ic].Draw(Game.SpriteBatch, _pixel, ds, i);
                i++;
            }
            Game.SpriteBatch.Draw(_pixel, new Rectangle(23, 29 + (16 * displayLineTotal), 604, 16), Color.Black * 0.5f);
            Game.SpriteBatch.DrawString(ds, " >> " + input, new Vector2(25, 29 + 16 * displayLineTotal), Color.LimeGreen);
        }
    }
}
