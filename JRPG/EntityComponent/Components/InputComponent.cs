using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent.Components
{
    public class InputComponent : Component
    {
        public List<Tuple<string, Buttons?, Keys?>> Actions = new List<Tuple<string, Buttons?, Keys?>>()
        {
            new Tuple<string, Buttons?, Keys?>("up", Buttons.LeftThumbstickUp, Keys.Up),
            new Tuple<string, Buttons?, Keys?>("down", Buttons.LeftThumbstickDown, Keys.Down),
            new Tuple<string, Buttons?, Keys?>("left", Buttons.LeftThumbstickLeft, Keys.Left),
            new Tuple<string, Buttons?, Keys?>("right", Buttons.LeftThumbstickRight, Keys.Right),
            new Tuple<string, Buttons?, Keys?>("up-alt", Buttons.DPadUp, Keys.W),
            new Tuple<string, Buttons?, Keys?>("down-alt", Buttons.DPadDown, Keys.S),
            new Tuple<string, Buttons?, Keys?>("left-alt", Buttons.DPadLeft, Keys.A),
            new Tuple<string, Buttons?, Keys?>("right-alt", Buttons.DPadRight, Keys.D),
            new Tuple<string, Buttons?, Keys?>("action", Buttons.A, Keys.Z),
            new Tuple<string, Buttons?, Keys?>("cancel", Buttons.B, Keys.X),
            new Tuple<string, Buttons?, Keys?>("run", Buttons.B, Keys.X),
            new Tuple<string, Buttons?, Keys?>("fullscreen", null, Keys.F1)
        };

        public Dictionary<string, bool> lastPressed;
        public Dictionary<string, bool> pressed;
        
        public InputComponent() : base()
        {
            lastPressed = new Dictionary<string, bool>();
            pressed = new Dictionary<string, bool>();

            foreach (var a in Actions)
            {
                lastPressed.Add(a.Item1, false);
                pressed.Add(a.Item1, false);
            }
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);

            foreach (var a in Actions)
            {
                // set last presses
                lastPressed[a.Item1] = pressed[a.Item1];

                // todo: handle gamepad/keyboard priority
                if (gamePad.IsConnected)
                {
                    if (a.Item2.HasValue)
                    {
                        pressed[a.Item1] = gamePad.IsButtonDown(a.Item2.Value);
                        continue;
                    }
                }

                if (a.Item3.HasValue)
                {
                    pressed[a.Item1] = keyboard.IsKeyDown(a.Item3.Value);
                }
            }
        }

        public bool ButtonPressed(string key) => pressed[key] && !lastPressed[key];
        public bool ButtonReleased(string key) => !pressed[key] && lastPressed[key];
        public bool ButtonDown(string key) => pressed[key];
        public bool ButtonUp(string key) => !pressed[key];
    }
}
