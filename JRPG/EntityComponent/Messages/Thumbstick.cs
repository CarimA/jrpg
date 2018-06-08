using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent.Messages
{
    public struct Thumbstick : IMessage
    {
        public Vector2 Position;
        public Vector2 DeltaPosition;
        public Buttons Stick;

        public Thumbstick(Vector2 position, Vector2 deltaPosition, Buttons stick)
        {
            Position = position;
            DeltaPosition = deltaPosition;

            if (!(stick == Buttons.LeftStick || stick == Buttons.RightStick))
            {
                throw new InvalidOperationException();
            }
            Stick = stick;
        }
    }
}
