using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent
{
    public class EntityList
    {
        private EntityManager _entityManager;
        List<Entity> _entities;

        public EntityList(EntityManager entityManager)
        {
            _entityManager = entityManager;
            _entities = new List<Entity>();
        }

        public Entity this[int index]
        {
            get { return _entities[index]; }
            set { _entities.Insert(index, value); }
        }

        public void Add(Entity entity) => _entities.Add(entity);
        public bool Remove(Entity entity) => _entities.Remove(entity);
        public void ForEach(Action<Entity> action) => _entities.ForEach(action);
        public void Sort(Comparison<Entity> comparison) => _entities.Sort(comparison);

        public EntityList FindWithTag(string tag) => (EntityList)_entities.Where(e => e.Tags.Contains(tag));
        // return entities where _any_ of the types match with the given list
        public EntityList Any(params Type[] Types) => (EntityList)_entities.Where((c) => Types.ToList().Any(ce => c.HasComponent(ce))).Distinct();
        // return entities where _none_ of the types are present
        public EntityList Except(params Type[] Types) => (EntityList)_entities.Where((c) => Types.ToList().All(ce => !c.HasComponent(ce))).Distinct();
        // return entities where _all_ of the types are present
        public EntityList All(params Type[] Types) => (EntityList)_entities.Where((c) => Types.ToList().All(ce => c.HasComponent(ce))).Distinct();
        
        public void Send(Entity sender, IMessage message)
        {
            _entities.ForEach((e) =>
            {
                e.Receive(sender, message);
            });
        }
    }
}
