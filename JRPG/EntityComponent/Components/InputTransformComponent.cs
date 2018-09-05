using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace JRPG.EntityComponent.Components
{
    public class InputTransformComponent : Component
    {
        public Vector2 LeftThumbstick;

        public override void Initialize()
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (Game.Input.ButtonPressed("fullscreen"))
                Send("fullscreen-pressed");

            if (Game.Input.ButtonPressed("debug"))
                Send("debug-pressed");

            if (Game.Input.ButtonPressed("screenshot"))
                Send("screenshot-pressed");

            if (Game.Input.ButtonPressed("pgup"))
                Send("pgup-pressed");

            if (Game.Input.ButtonPressed("pgdown"))
                Send("pgdown-pressed");
            

            if (Game.Input.ButtonPressed("up"))
                Send("up-pressed");

            if (Game.Input.ButtonPressed("down"))
                Send("down-pressed");

            if (Game.Input.ButtonPressed("left"))
                Send("left-pressed");

            if (Game.Input.ButtonPressed("right"))
                Send("right-pressed");

            LeftThumbstick = Vector2.Zero;

            if (Game.Input.ButtonDown("up"))
                LeftThumbstick.Y--;
            if (Game.Input.ButtonDown("down"))
                LeftThumbstick.Y++;
            if (Game.Input.ButtonDown("left"))
                LeftThumbstick.X--;
            if (Game.Input.ButtonDown("right"))
                LeftThumbstick.X++;

            if (LeftThumbstick != Vector2.Zero)
                LeftThumbstick.Normalize();

            Send("input-state");

            if (Game.Input.ButtonPressed("action"))
                Send("action-pressed");

            if (Game.Input.ButtonPressed("run"))
                Send("run-pressed");
        }
    }
}
