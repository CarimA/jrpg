using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent.Components
{
    public partial class Input : Component
    {
        public enum Actions : byte
        {
            Action = 0,
            Cancel
        }

        private PlayerIndex _playerIndex;

        public const Buttons ACTION_BUTTON = Buttons.A;
        public const Buttons CANCEL_BUTTON = Buttons.B;

        public const Keys ACTION_KEY = Keys.O;
        public const Keys CANCEL_KEY = Keys.P;

        private bool _lastAction;
        private bool _lastCancel;
        private bool _action;
        private bool _cancel;

        private Vector2 _lastLeftStick;
        private Vector2 _lastRightStick;
        private Vector2 _leftStick;
        private Vector2 _rightStick;
    }

    public struct ButtonPressed : IMessage
    {
        public Input.Actions Action;

        public ButtonPressed(Input.Actions action)
        {
            Action = action;
        }
    }

    public struct ButtonReleased : IMessage
    {
        public Input.Actions Action;

        public ButtonReleased(Input.Actions action)
        {
            Action = action;
        }
    }

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
