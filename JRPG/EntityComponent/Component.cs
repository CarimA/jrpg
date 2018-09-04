using Microsoft.Xna.Framework;
using Newtonsoft.Json;
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
        public Entity GetOwner => _entity;
        public MainGame Game => GetOwner.Game;

        public void Assign(Entity entity) => _entity = entity;

        public T GetComponent<T>() where T : Component => GetOwner?.GetComponent<T>();
        public bool HasComponent<T>() where T : Component => GetOwner.HasComponent<T>();
        public Component GetComponent(Type component) => GetOwner?.GetComponent(component);
        public bool HasComponent(Type component) => GetOwner.HasComponent(component);

        public abstract void Update(GameTime gameTime);

        public abstract void Receive(MessageType message, Entity entity, Component sender);

        public void Send(MessageType message, params Component[] components)
        {
            foreach (var component in components)
            {
                if (HasComponent(component.GetType())) {
                    GetComponent(component.GetType()).Receive(message, GetOwner, this);
                }
            }
        }

        public void SendAll(MessageType message)
        {
            foreach (var component in GetOwner.GetComponents())
            {
                if (component != this)
                {
                    component.Receive(message, GetOwner, this);
                }
            }
        }

        // todo: global send and global send all
    }
}
