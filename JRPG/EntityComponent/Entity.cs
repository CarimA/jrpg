﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent
{
    public partial class Entity
    {
        private EntityManager _entityManager;

        private readonly List<Type> _componentTypes;
        private readonly List<Component> _components;

        public readonly List<string> Tags;

        public bool Active;
        public int Priority;

        public Entity(EntityManager entityManager, int priority = 0)
        {
            _entityManager = entityManager;
            _componentTypes = new List<Type>();
            _components = new List<Component>();
            Tags = new List<string>();
            Active = true;
            Priority = priority;
        }

        public EntityManager GetManager => _entityManager;
        public EntityList Siblings => GetManager.Entities;

        public bool HasComponent<T>() where T : Component => _componentTypes.Exists(c => c == typeof(T));
        public bool HasComponent(Type t) => _componentTypes.Exists(c => c == t);
        public T GetComponent<T>() where T : Component => _components.Find(c => c.GetType() == typeof(T)) as T;
        
        public void AddComponent(Component component)
        {
            _componentTypes.Add(component.GetType());
            _components.Add(component);
            component.Assign(this);
        }
        public void AddComponents(List<Component> components) => components.ForEach(c => AddComponent(c));

        public void RemoveComponent(Component component)
        {
            _componentTypes.Remove(component.GetType());
            _components.Remove(component);
        }

        public void AddTag(string tag) => Tags.Add(tag);
        public void RemoveTag(string tag) => Tags.Remove(tag);

        public void Clear() => _components.ForEach((c) => RemoveComponent(c));

        public void Destroy()
        {
            Clear();
            _entityManager.RemoveEntity(this);
        }

        public void Send(IMessage message)
        {
            Receive(this, message);
        }

        public void Receive(Entity entity, IMessage message)
        {
            _components.ForEach((c) =>
            {
                c.Receive(entity, message);
            });
        }

        public void Update(GameTime gameTime)
        {
            _components.ForEach(c => c.Update(gameTime));
        }

        public void Draw(GameTime gameTime)
        {
            _components.ForEach(c => c.Draw(gameTime));
        }
    }
}
