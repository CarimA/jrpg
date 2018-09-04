using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace JRPG.Scripting.Commands
{
    public class EmptyCommand : Command
    {
        public override string Name => "null";

        bool held;

        public override object Action(params object[] args)
        {
            SetCommand();
            held = true;

            while (held)
            {
                System.Threading.Thread.Sleep(200);
            }

            ClearCommand();
            return null;
        }
        
        public override void Update(GameTime gameTime)
        {
            held = false;
        }

        public override void DrawMask(GameTime gameTime)
        {

        }

        public override void DrawFringe(GameTime gameTime)
        {

        }

        public override void DrawUI(GameTime gameTime)
        {

        }
    }
}
