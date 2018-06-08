using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent
{
    public class EntityManager : DrawableGameComponent
    {
        private readonly List<Entity> _entities;
        private readonly Dictionary<string, List<Entity>> _registeredEntities;

        public EntityManager(Game game) : base(game)
        {
            game.Components.Add(this);
            _entities = new List<Entity>();
            _registeredEntities = new Dictionary<string, List<Entity>>();
        }

        public Entity CreateEntity()
        {
            Entity entity = new Entity(this);
            AddEntity(entity);
            return entity;
        }

        public void AddEntity(Entity entity)
        {
            _entities.Add(entity);
            _entities.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public void RemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
            List<string> keys = _registeredEntities.Where(l => l.Value.Contains(entity)).Select(x => x.Key).ToList();
            if (keys != null)
            {
                keys.ForEach(k =>
                {
                    Unregister(k, entity);
                });
            }
        }

        public void Register(string key, Entity entity)
        {
            if (!_registeredEntities.ContainsKey(key))
            {
                _registeredEntities[key] = new List<Entity>();
            }
            _registeredEntities[key].Add(entity);
        }

        public void Unregister(string key, Entity entity)
        {
            if (!_registeredEntities.ContainsKey(key))
            {
                _registeredEntities[key] = new List<Entity>();
            }
            _registeredEntities[key].Remove(entity);
        }

        public void Send(string key, Entity entity, IMessage message)
        {
            List<Entity> e = _registeredEntities[key];
            if (e != null)
            {
                e.ForEach((x) => x.Receive(entity, message));
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _entities.ForEach((e) =>
            {
                if (e.Active)
                {
                    e.Update(gameTime);
                }
            });
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _entities.ForEach((e) =>
            {
                if (e.Active)
                {
                    e.Draw(gameTime);
                }
            });
        }
    }
}
