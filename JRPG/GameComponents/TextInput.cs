using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.GameComponents
{
    public class TextInput : GameComponent
    {
        private string Input = "";

        public TextInput(Game game) : base(game)
        {
            game.Components.Add(this);

            // always perform first or you'll have 2 frames of delay in your input
            UpdateOrder = int.MinValue;

            Game.Window.TextInput += Window_TextInput;
        }

        private void Window_TextInput(object sender, TextInputEventArgs e)
        {
            //Console.Write((int)e.Character);

            if ((int)e.Character == 8)
            {
                if (Input.Length > 0)
                {
                    Input = Input.Substring(0, Input.Length - 1);
                }
            }
            else
            {
                Input += e.Character;
            }
        }

        public string Flush() => Input = "";
        public string GetInput() => Input;
    }
}
