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
        public MainGame Game;
        public Entity Player;
        public PlayerComponent PlayerData;

        public abstract string Name { get; }
       
        public void SetCommand()
        {

            Console.WriteLine("Started command: " + this.GetType());
        }

        public void ClearCommand()
        {

            Console.WriteLine("Stopped command: " + this.GetType());
        }

        public abstract object Action(params object[] args);
        public abstract void Update(GameTime gameTime);
        public abstract void DrawMask(GameTime gameTime);
        public abstract void DrawFringe(GameTime gameTime);
        public abstract void DrawUI(GameTime gameTime);
    }
}
