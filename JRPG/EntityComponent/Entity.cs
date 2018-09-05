using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent
{
    public partial class Entity
    {
        public MainGame Game => _entityManager.Game;
        private EntityManager _entityManager;

        private readonly List<Type> _componentTypes;
        private readonly List<Component> _components;

        public readonly string Name;
        public readonly List<string> Tags;

        public bool Active;
        public int Priority;

        private readonly Dictionary<string, List<Action<Entity, Component>>> _subscriptions;

        public Entity(EntityManager entityManager, string name, int priority = 0)
        {
            _entityManager = entityManager;
            _entityManager.AddEntity(this);
            _componentTypes = new List<Type>();
            _components = new List<Component>();
            Tags = new List<string>();
            Name = name;
            Active = true;
            Priority = priority;
            _subscriptions = new Dictionary<string, List<Action<Entity, Component>>>();
        }

        public EntityManager GetManager => _entityManager;
        public EntityList Siblings => GetManager.Entities;

        public bool HasComponent<T>() where T : Component => _componentTypes.Exists(c => c == typeof(T));
        public bool HasComponent(Type t) => _componentTypes.Exists(c => c == t);
        public T GetComponent<T>() where T : Component => _components.Find(c => c.GetType() == typeof(T)) as T;
        public Component GetComponent(Type t) => _components.Find(c => c.GetType() == t);

        public List<Component> GetComponents() => _components;

        public void AddComponent(Component component)
        {
            _componentTypes.Add(component.GetType());
            _components.Add(component);
            component.Assign(this);
            component.Initialize();
        }
        public void AddComponents(List<Component> components) => components.ForEach(c => AddComponent(c));

        public void RemoveComponent(Component component)
        {
            _componentTypes.Remove(component.GetType());
            _components.Remove(component);
        }

        public void AddTag(string tag) => Tags.Add(tag);
        public void RemoveTag(string tag) => Tags.Remove(tag);

        public void Subscribe(string message, Action<Entity, Component> action)
        {
            if (!_subscriptions.ContainsKey(message))
            {
                _subscriptions.Add(message, new List<Action<Entity, Component>>());
            }
            _subscriptions[message].Add(action);
        }

        public void Send(string message, Entity entity, Component sender)
        {
            if (_subscriptions.ContainsKey(message))
            {
                _subscriptions[message].ForEach((a) =>
                {
                    a(entity, sender);
                });
            }
        }

        public void Clear() => _components.ForEach((c) => RemoveComponent(c));

        public void Destroy()
        {
            Clear();
            _entityManager.RemoveEntity(this);
        }
       
        public void Update(GameTime gameTime)
        {
            _components.ForEach(c => c.Update(gameTime));
        }

        public void DrawMask(GameTime gameTime)
        {
            if (!this.Active)
                return;

            _components.ForEach(c =>
            {
                if (c is DrawableComponent)
                {
                    (c as DrawableComponent).DrawMask(gameTime);
                }
            });
        }
        public void DrawFringe(GameTime gameTime)
        {
            if (!this.Active)
                return;

            _components.ForEach(c => 
            {
                if (c is DrawableComponent)
                {
                    (c as DrawableComponent).DrawFringe(gameTime);
                }
            });
        }
        public void DrawUI(GameTime gameTime)
        {
            if (!this.Active)
                return;

            _components.ForEach(c =>
            {
                if (c is DrawableComponent)
                {
                    (c as DrawableComponent).DrawUI(gameTime);
                }
            });
        }

        /*public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
        }

        public static Entity Deserialize(string text)
        {
            Entity e = JsonConvert.DeserializeObject<Entity>(text);
            e._entityManager = Locator.EC;
            return e;
        }*/
    }
}
