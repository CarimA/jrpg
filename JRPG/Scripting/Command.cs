using Jint;
using JRPG.EntityComponent;
using JRPG.EntityComponent.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.Scripting
{
    public abstract class Command
    {
        private ScriptingManager scriptingManager;
        public Entity Player => scriptingManager.Game.Player;
        public PlayerComponent PlayerData;
        public MainGame Game => scriptingManager.Game;

        public abstract string Name { get; }

        public Command(ScriptingManager manager, Engine engine)
        {
            scriptingManager = manager;
            PlayerData = Player.GetComponent<PlayerComponent>();
            engine.SetValue(Name, new Func<object[], object>(Action));
        }

        public void SetCommand()
        {
            scriptingManager.AddActiveCommand(this);
            Player.GetComponent<InputComponent>().FlushPresses();

            Console.WriteLine("Started command: " + this.GetType());
        }

        public void ClearCommand()
        {
            scriptingManager.RemoveActiveCommand(this);

            Console.WriteLine("Stopped command: " + this.GetType());
        }

        public abstract object Action(params object[] args);
        public abstract void Update(GameTime gameTime);
        public abstract void DrawMask(GameTime gameTime);
        public abstract void DrawFringe(GameTime gameTime);
        public abstract void DrawUI(GameTime gameTime);
    }
}
