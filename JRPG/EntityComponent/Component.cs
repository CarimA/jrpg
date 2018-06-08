using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent
{
    public abstract class Component
    {
        private Entity _entity;

        public void Assign(Entity entity) => _entity = entity;
        public Entity GetOwner() => _entity;

        public T GetComponent<T>() where T : Component => GetOwner()?.GetComponent<T>();

        public void Send(string key, IMessage message)
        {
            GetOwner().Send(key, message);
        }

        public void Send(IMessage message)
        {
            GetOwner().Send(message);
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
        public abstract void Receive(Entity entity, IMessage message);        
    }
}
