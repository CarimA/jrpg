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
        public List<Tuple<string, List<Buttons>, List<Keys>>> Actions = new List<Tuple<string, List<Buttons>, List<Keys>>>()
        {
            new Tuple<string, List<Buttons>, List<Keys>>("up", new List<Buttons>() { Buttons.LeftThumbstickUp, Buttons.DPadUp }, new List<Keys>() { Keys.Up, Keys.W }),
            new Tuple<string, List<Buttons>, List<Keys>>("down", new List<Buttons>() { Buttons.LeftThumbstickDown, Buttons.DPadDown }, new List<Keys>() { Keys.Down, Keys.S }),
            new Tuple<string, List<Buttons>, List<Keys>>("left", new List<Buttons>() { Buttons.LeftThumbstickLeft, Buttons.DPadLeft }, new List<Keys>() { Keys.Left, Keys.A }),
            new Tuple<string, List<Buttons>, List<Keys>>("right", new List<Buttons>() { Buttons.LeftThumbstickRight, Buttons.DPadRight }, new List<Keys>() { Keys.Right, Keys.D }),

            new Tuple<string, List<Buttons>, List<Keys>>("action", new List<Buttons>() { Buttons.A }, new List<Keys>() { Keys.Z }),
            new Tuple<string, List<Buttons>, List<Keys>>("cancel", new List<Buttons>() { Buttons.B }, new List<Keys>() { Keys.X }),
            new Tuple<string, List<Buttons>, List<Keys>>("run", new List<Buttons>() { Buttons.B }, new List<Keys>() { Keys.X }),
            new Tuple<string, List<Buttons>, List<Keys>>("fullscreen", new List<Buttons>() { }, new List<Keys>() { Keys.F1 }),
            new Tuple<string, List<Buttons>, List<Keys>>("debug", new List<Buttons>() { }, new List<Keys>() { Keys.F12 }),
            new Tuple<string, List<Buttons>, List<Keys>>("screenshot", new List<Buttons>() { Buttons.Back }, new List<Keys>() { Keys.F2 }),
        };

        public Dictionary<string, bool> lastPressed;
        public Dictionary<string, bool> pressed;
        private Queue<string> presses;

        public InputComponent() : base()
        {
            lastPressed = new Dictionary<string, bool>();
            pressed = new Dictionary<string, bool>();
            presses = new Queue<string>();

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
                /*if (gamePad.IsConnected)
                {
                    if (a.Item2.HasValue)
                    {
                        pressed[a.Item1] = gamePad.IsButtonDown(a.Item2.Value);
                        continue;
                    }
                }*/


                pressed[a.Item1] = a.Item2.Where((b) => gamePad.IsButtonDown(b)).Count() > 0 || a.Item3.Where((k) => keyboard.IsKeyDown(k)).Count() > 0;
                

                if (pressed[a.Item1] && !lastPressed[a.Item1])
                {
                    presses.Enqueue(a.Item1);

                    if (presses.Count > 20)
                    {
                        presses.Dequeue();
                    }
                }
            }
        }

        public string PollPress()
        {
            if (presses.Count > 0)
            {
                return presses.Dequeue();
            }
            return "";
        }

        public void FlushPresses()
        {
            presses.Clear();
        }

        public bool ButtonPressed(string key) => pressed[key] && !lastPressed[key];
        public bool ButtonReleased(string key) => !pressed[key] && lastPressed[key];
        public bool ButtonDown(string key) => pressed[key];
        public bool ButtonUp(string key) => !pressed[key];
    }
}
