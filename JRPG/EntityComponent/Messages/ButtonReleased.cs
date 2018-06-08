using JRPG.EntityComponent.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent.Messages
{
    public struct ButtonReleased : IMessage
    {
        public Input.Actions Action;

        public ButtonReleased(Input.Actions action)
        {
            Action = action;
        }
    }
}
