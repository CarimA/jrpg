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
    public class ShowTextCommand : Command
    {
        public override string Name => "show_text";

        bool held = true;

        float renderSpeed = 15f;

        string fullText;     // full text

        float renderTime = 0;   // time passed since last char set
        int curChar = 0;    // current char being rendered

        bool pause = false;
        bool finished = false;

        Texture2D textbox;
        
        public ShowTextCommand(ScriptingManager manager, Engine engine) : base(manager, engine)
        {
            textbox = Game.Content.Load<Texture2D>("Interfaces/textbox");
        }

        public override object Action(params object[] args)
        {
            SetCommand();

            renderTime = 0;
            curChar = 0;
            newLinePos.Clear();
            newLinePos.Add(0);
            finished = false;
            pause = false;

            held = true;
            fullText = args[0].ToString();

            while (held) ;

            ClearCommand();
            return null;
        }

        List<int> newLinePos = new List<int>() { 0 };

        public override void Update(GameTime gameTime)
        {
            if (finished)
            {
                bool cont = Player.GetComponent<InputComponent>().ButtonDown("action");
                if (cont)
                {
                    held = false;
                }
            }
            else
            {
                if (pause)
                {
                    bool cont = Player.GetComponent<InputComponent>().ButtonDown("action");
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
                        if (newLinePos.Count() > 3)
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
            Game.SpriteBatch.Draw(textbox, new Rectangle(384 - 300 - 10, 216 - 62 - 10, 300, 62), new Rectangle(0, 0, 300, 62), Color.White);

            int p = pause ? 1 : 0;
            Game.SpriteBatch.DrawString(MainGame.Font, 
                fullText.Substring(
                    newLinePos[Math.Max(0, newLinePos.Count() - 3 - p)], 
                    curChar - newLinePos[Math.Max(0, newLinePos.Count() - 3 - p)]), 
                new Vector2(384 - 300 + 5, 216 - 62 - 1), new Color(13, 32, 48));

            if (pause)
            {
                Game.SpriteBatch.Draw(textbox, new Rectangle(384 - 23, 216 - 23 - (int)(Math.Sin(gameTime.TotalGameTime.TotalSeconds * 10) * 3), 6, 6), new Rectangle(300, 0, 6, 6), Color.White);
            }
            if (finished)
            {
                Game.SpriteBatch.Draw(textbox, new Rectangle(384 - 23 - (int)(Math.Sin(gameTime.TotalGameTime.TotalSeconds * 10) * 3), 216 - 23, 6, 6), new Rectangle(300, 6, 6, 6), Color.White);

            }
        }
    }
}
