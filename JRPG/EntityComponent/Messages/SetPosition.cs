using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent.Messages
{
    public struct SetPosition : IMessage
    {
        public Vector2 NewPosition;

        public SetPosition(Vector2 newPosition)
        {
            NewPosition = newPosition;
        }
    }
}
