using JRPG.EntityComponent.Messages;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent.Components
{
    public class Player : Component
    {
        public override void Draw(GameTime gameTime)
        {

        }

        public override void Receive(Entity entity, IMessage message)
        {
            if (message is ButtonPressed msgBP)
            {
                if (msgBP.Action == Input.Actions.Action)
                {

                }
            }

            if (message is Thumbstick msgT)
            {
                if (msgT.Stick == Microsoft.Xna.Framework.Input.Buttons.LeftStick)
                {
                    Send(new MovePosition(msgT.Position));
                }
            }
        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}
