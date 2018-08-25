using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JRPG.EntityComponent.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace JRPG.Scripting.Commands
{
    public class ShowTextCommand : Command
    {
        public override string Name => "show_text";

        bool held = true;

        int totalLines = 2;

        float renderSpeed = 35f;

        string fullText;     // full text

        bool onlyOneLine;

        float renderTime = 0;   // time passed since last char set
        int curChar = 0;    // current char being rendered

        bool pause = false;
        bool finished = false;

        Texture2D portrait;
        Texture2D atlas;
        BitmapFont font;
        
        public ShowTextCommand() : base()
        {
            atlas = Game.Assets.Get<Texture2D>(AssetManager.Asset.InterfacesAtlas);
            font = Game.Assets.Get<BitmapFont>(AssetManager.Asset.Font);
            portrait = Game.Assets.Get<Texture2D>(AssetManager.Asset.PortraitDebug);
        }

        public override object Action(params object[] args)
        {
            SetCommand();
            PlayerData.InControl = false;

            renderTime = 0;
            curChar = 0;
            newLinePos.Clear();
            newLinePos.Add(0);
            finished = false;
            pause = false;

            held = true;

            if (font.MeasureString((string)args[0]).Width >= 290)
            {


                fullText = "";

                string txt = args[0].ToString();
                string[] tokens = txt.Split(' ');

                string curLine = "";
                foreach (string token in tokens)
                {
                    if (token.Contains("\n"))
                    {
                        curLine += token;
                        fullText += curLine;
                        curLine = "";
                        continue;
                    }

                    if (font.MeasureString(curLine + token).Width >= 290)
                    {
                        curLine += "\n";
                        fullText += curLine;
                        curLine = "";
                    }

                    curLine += token;
                    curLine += " ";
                }

                fullText += curLine;
                onlyOneLine = false;
            }
            else
            {
                fullText = (string)args[0];
                onlyOneLine = true;
            }

            while (held) ;

            ClearCommand();
            PlayerData.InControl = true;
            return null;
        }

        List<int> newLinePos = new List<int>() { 0 };

        public override void Update(GameTime gameTime)
        {
            if (fullText == null || fullText == "")
                return;

            InputComponent pin = Player.GetComponent<InputComponent>();
            string pol = pin.PollPress();

            if (finished)
            {
                bool cont = pol == ("action");
                if (cont)
                {
                    held = false;
                }
            }
            else
            {
                if (pause)
                {
                    bool cont = pol == ("action");
                    if (cont)
                    {
                        pause = false;
                    }
                }

                if (!pause)
                {
                    renderTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (renderTime <= 0f && !pause)
                {
                    renderTime += 1f / renderSpeed;
                    curChar++;

                    if (fullText.Length == curChar)
                    {
                        finished = true;
                        return;
                    }

                    if (char.IsLetterOrDigit(fullText[curChar]))
                    {
                        // play sound
                    }

                    if (fullText[curChar] == '\n')
                    {
                        renderTime += (1f / renderSpeed) * 6f;
                        newLinePos.Add(curChar + 1);
                        if (newLinePos.Count() > totalLines)
                        {
                            pause = true;
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
            if (fullText == null || fullText == "")
                return;

            Game.SpriteBatch.DrawNineSlice(atlas, Color.White, new Rectangle(10, 148, 364, 54), new Rectangle(0, 0, 18, 18));

            Game.SpriteBatch.Draw(atlas, new Rectangle(61, 153, 4, 4), new Rectangle(18, 0, 4, 4), Color.White);
            Game.SpriteBatch.Draw(atlas, new Rectangle(61, 157, 4, 36), new Rectangle(18, 4, 4, 4), Color.White);
            Game.SpriteBatch.Draw(atlas, new Rectangle(61, 193, 4, 4), new Rectangle(18, 8, 4, 4), Color.White);

            Game.SpriteBatch.Draw(portrait, new Rectangle(15, 153, 44, 44), new Rectangle(0, 0, 44, 44), Color.White);
            Game.SpriteBatch.Draw(portrait, new Rectangle(15, 153, 44, 44), new Rectangle(44 * 2, 44, 44, 44), Color.White);

            // Game.SpriteBatch.Draw(textbox, new Rectangle(384 - 300 - 10, 216 - 62 - 10, 300, 62), new Rectangle(0, 0, 300, 62), Color.White);

            int p = pause ? 1 : 0;
            int y = MainGame.GAME_HEIGHT - (MainGame.GAME_HEIGHT - 206) - 46;
            if (onlyOneLine)
            {
                y += 46 / 4 - 3;
            }

            string text = fullText.Substring(
                    newLinePos[Math.Max(0, newLinePos.Count() - totalLines - p)],
                    curChar - newLinePos[Math.Max(0, newLinePos.Count() - totalLines - p)]);

            Game.SpriteBatch.DrawString(font, text, new Vector2(384 - 300 + 5 - 20 - 1, y), Color.Black);
            Game.SpriteBatch.DrawString(font, text, new Vector2(384 - 300 + 5 - 20 + 1, y), Color.Black);
            Game.SpriteBatch.DrawString(font, text, new Vector2(384 - 300 + 5 - 20, y - 1), Color.Black);
            Game.SpriteBatch.DrawString(font, text, new Vector2(384 - 300 + 5 - 20, y + 1), Color.Black);

            Game.SpriteBatch.DrawString(font, text, new Vector2(384 - 300 + 5 - 20, y), Color.White);

            if (pause)
            {
                Game.SpriteBatch.Draw(atlas, new Rectangle(384 - 23, 216 - 27 - (int)(Math.Sin(gameTime.TotalGameTime.TotalSeconds * 10) * 3), 6, 6), new Rectangle(0, 18, 6, 6), Color.White);
            }
            if (finished)
            {
                Game.SpriteBatch.Draw(atlas, new Rectangle(384 - 23 - (int)(Math.Sin(gameTime.TotalGameTime.TotalSeconds * 10) * 3), 216 - 27, 6, 6), new Rectangle(6, 18, 6, 6), Color.White);

            }
        }
    }
}
