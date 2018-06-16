using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent.Components
{
    public class Input : Component
    {
        // todo: move into shared classes with data, logic, and messages
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

        public Input(PlayerIndex playerIndex) : base()
        {
            _playerIndex = playerIndex;
        }

        public override void Draw(GameTime gameTime)
        {

        }

        public override void Receive(Entity entity, IMessage message)
        {

        }

        public override void Update(GameTime gameTime)
        {
            Vector2 leftStick = Vector2.Zero;
            Vector2 rightStick = Vector2.Zero;
            bool action = false;
            bool cancel = false;

            // first check for keyboard input (if applicable)
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.W))
                leftStick.Y--;
            if (keyboard.IsKeyDown(Keys.S))
                leftStick.Y++;
            if (keyboard.IsKeyDown(Keys.A))
                leftStick.X--;
            if (keyboard.IsKeyDown(Keys.D))
                leftStick.X++;

            if (keyboard.IsKeyDown(ACTION_KEY))
                action = true;
            if (keyboard.IsKeyDown(CANCEL_KEY))
                cancel = true;
            
            // todo
            // now check for touch screen (if applicable)

            // now check for mouse input (if applicable)

            // now check for gamepad input (if applicable)
            GamePadState gamePad = GamePad.GetState(_playerIndex);

            leftStick += new Vector2(gamePad.ThumbSticks.Left.X, gamePad.ThumbSticks.Left.Y * -1);
            rightStick += new Vector2(gamePad.ThumbSticks.Right.X, gamePad.ThumbSticks.Right.Y * -1);

            if (gamePad.IsButtonDown(Buttons.DPadUp))
                leftStick.Y--;
            if (gamePad.IsButtonDown(Buttons.DPadDown))
                leftStick.Y++;
            if (gamePad.IsButtonDown(Buttons.DPadLeft))
                leftStick.X--;
            if (gamePad.IsButtonDown(Buttons.DPadRight))
                leftStick.X++;

            if (gamePad.IsButtonDown(ACTION_BUTTON))
                action = true;
            if (gamePad.IsButtonDown(CANCEL_BUTTON))
                cancel = true;

            // normalise sticks
            if (leftStick != Vector2.Zero)
                leftStick.Normalize();

            if (rightStick != Vector2.Zero)
                rightStick.Normalize();

            _lastLeftStick = _leftStick;
            _lastRightStick = _rightStick;
            _lastAction = _action;
            _lastCancel = _cancel;

            _leftStick = leftStick;
            _rightStick = rightStick;
            _action = action;
            _cancel = cancel;
            
            Send(new Messages.Thumbstick(_leftStick, _leftStick - _lastLeftStick, Buttons.LeftStick));
            Send(new Messages.Thumbstick(_rightStick, _rightStick - _lastRightStick, Buttons.RightStick));

            if (_action && !_lastAction)
                Send(new Messages.ButtonPressed(Actions.Action));

            if (!_action && _lastAction)
                Send(new Messages.ButtonReleased(Actions.Action));

            if (_cancel && !_lastCancel)
                Send(new Messages.ButtonPressed(Actions.Cancel));

            if (!_cancel && _lastCancel)
                Send(new Messages.ButtonReleased(Actions.Cancel));
        }
    }
}
