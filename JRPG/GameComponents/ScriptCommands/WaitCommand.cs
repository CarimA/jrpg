using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;
using Microsoft.Xna.Framework;

namespace JRPG.GameComponents.ScriptCommands
{
    public class WaitCommand : Command
    {
        public override string Name => "wait";

        private float time;

        public WaitCommand(ScriptingManager manager, Engine engine) : base(manager, engine)
        {

        }

        public override object Action(params object[] args)
        {
            time = float.Parse(args[0].ToString());

            Wait();
            return null;
        }

        public override void Update(GameTime gameTime)
        {
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time <= 0f)
            {
                Resume();
            }
        }
    }
}
