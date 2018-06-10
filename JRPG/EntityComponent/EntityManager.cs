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
        private readonly EntityList _entities;

        public EntityManager(Game game) : base(game)
        {
            game.Components.Add(this);
            _entities = new EntityList(this);
        }

        public EntityList Entities => _entities;

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
