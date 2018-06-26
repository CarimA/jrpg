using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent.Components
{
    public partial class Position : Component
    {
        public override void Draw(GameTime gameTime)
        {

        }

        public override void Receive(Entity entity, IMessage message)
        {
            if (message is MovePosition msgMP)
            {
                Pos += msgMP.DeltaPosition;
            }

            if (message is SetPosition msgSP)
            {
                Pos = msgSP.NewPosition;
            }
        }

        public override void Update(GameTime gameTime)
        {
            //Game.Window.Title = Pos.ToString();
        }
    }
}
