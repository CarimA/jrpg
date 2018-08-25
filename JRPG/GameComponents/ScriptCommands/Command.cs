using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;
using Microsoft.Xna.Framework;

namespace JRPG.GameComponents.ScriptCommands
{
    public abstract class Command
    {
        public abstract string Name { get; }

        private ScriptingManager _manager;
        private bool CommandHeld;

        public Command(ScriptingManager manager, Engine engine)
        {
            _manager = manager;
            engine.SetValue(Name, new Func<object[], object>(Action));
        }

        private void Subscribe()
        {
            Console.WriteLine("Subscribed: " + this.GetType());
            _manager.Subscribe(this);
            //Player.GetComponent<InputComponent>().FlushPresses();
        }

        private void Unsubscribe()
        {
            Console.WriteLine("Unsubscribed: " + this.GetType());
            _manager.Unsubscribe(this);
        }

        public void Wait()
        {
            CommandHeld = true;
            Subscribe();
            while (CommandHeld) ;
        }

        public void Resume()
        {
            CommandHeld = false;
            Unsubscribe();
        }

        public abstract object Action(params object[] args);
        public abstract void Update(GameTime gameTime);
    }
}
