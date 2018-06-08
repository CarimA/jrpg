using JRPG.EntityComponent.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent.Messages
{
    public struct ButtonPressed : IMessage
    {
        public Input.Actions Action;

        public ButtonPressed(Input.Actions action)
        {
            Action = action;
        }
    }
}
