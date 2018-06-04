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
        private readonly Dictionary<string, Entity> _registeredEntities;

        public EntityManager(Game game) : base(game)
        {
            game.Components.Add(this);
            _entities = new List<Entity>();
            _registeredEntities = new Dictionary<string, Entity>();
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
        }

        public void RemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
            string key = (from kv in _registeredEntities
                         where kv.Value.Equals(entity)
                         select kv.Key).FirstOrDefault();
            if (key != null)
            {
                Unregister(key);
            }
        }

        public void Register(string key, Entity entity)
        {
            _registeredEntities[key] = entity;
        }

        public void Unregister(string key)
        {
            _registeredEntities.Remove(key);
        }

        public void Send(string key, Entity entity, Message message)
        {
            Entity e = _registeredEntities[key];
            if (e != null)
            {
                e.Receive(entity, message);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
