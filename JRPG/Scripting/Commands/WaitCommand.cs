using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;
using Microsoft.Xna.Framework;

namespace JRPG.Scripting.Commands
{
    public class WaitCommand : Command
    {
        public override string Name => "wait";

        private float time;
        private bool held = true;
        
        public WaitCommand(ScriptingManager manager, Engine engine) : base(manager, engine)
        {

        }

        public override object Action(params object[] args)
        {
            SetCommand();

            time = float.Parse(args[0].ToString());
            held = true;

            while (held) { }

            ClearCommand();
            return "what";
        }

        public override void Update(GameTime gameTime)
        {
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time <= 0f)
            {
                held = false;
            }
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
