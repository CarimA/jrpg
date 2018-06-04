using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent
{
    public class Entity
    {
        private EntityManager _entityManager;

        private readonly List<Type> _componentTypes;
        private readonly List<Component> _components;

        public bool Active;

        public Entity(EntityManager entityManager)
        {
            _entityManager = entityManager;
            _componentTypes = new List<Type>();
            _components = new List<Component>();
            Active = true;
        }

        public bool HasComponent<T>() where T : Component => _componentTypes.Exists(c => c == typeof(T));
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

        public void Clear() => _components.ForEach((c) => RemoveComponent(c));

        public void Destroy()
        {
            Clear();
            _entityManager.RemoveEntity(this);
        }

        public void Send(string key, Message message)
        {
            _entityManager.Send(key, this, message);
        }

        public void Send(Message message)
        {
            Receive(this, message);
        }

        public void Receive(Entity entity, Message message)
        {
            _components.ForEach((c) =>
            {
                c.Receive(entity, message);
            });
        }
    }
}
