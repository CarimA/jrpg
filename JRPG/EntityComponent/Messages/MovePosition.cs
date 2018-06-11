﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent.Messages
{
    public struct MovePosition : IMessage
    {
        public Vector2 DeltaPosition;

        public MovePosition(Vector2 deltaPosition)
        {
            DeltaPosition = deltaPosition;
        }
    }
}
