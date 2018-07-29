using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;
using Microsoft.Xna.Framework;

namespace JRPG.Scripting.Commands
{
    public class EmptyCommand : Command
    {
        public override string Name => "null";

        bool held;

        public EmptyCommand(ScriptingManager manager, Engine engine) : base(manager, engine)
        {

        }

        public override object Action(params object[] args)
        {
            SetCommand();
            held = true;



            while (held) ;

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
