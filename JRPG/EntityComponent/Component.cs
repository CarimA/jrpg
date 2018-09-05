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

        public abstract void Initialize();
        public abstract void Update(GameTime gameTime);
        
        public void Subscribe(string message, Action<Entity, Component> action) => GetOwner.Subscribe(message, action);
        public void Send(string message) => GetOwner.Send(message, GetOwner, this);

        // todo: global send and global send all
    }
}
