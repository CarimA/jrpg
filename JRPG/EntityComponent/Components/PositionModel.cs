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
        public Vector2 Pos;
    }

    public struct MovePosition : IMessage
    {
        public Vector2 DeltaPosition;

        public MovePosition(Vector2 deltaPosition)
        {
            DeltaPosition = deltaPosition;
        }
    }

    public struct SetPosition : IMessage
    {
        public Vector2 NewPosition;

        public SetPosition(Vector2 newPosition)
        {
            NewPosition = newPosition;
        }
    }
}
