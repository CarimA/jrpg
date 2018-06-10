using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent
{
    public class EntityList : List<Entity>
    {
        private EntityManager _entityManager;

        public EntityList(EntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public EntityList FindWithTag(string tag)
        {
            return (EntityList)(this.Where(e => e.Tags.Contains(tag)));
        }

        public EntityList Any(params Type[] Types)
        {
            // return entities where _any_ of the types match with the given list
            return (EntityList)this.Where((c) => Types.ToList().Any(ce => c.HasComponent(ce))).Distinct();
        }

        public EntityList Not(params Type[] Types)
        {
            // return entities where _none_ of the types are present
            return (EntityList)this.Where((c) => Types.ToList().All(ce => !c.HasComponent(ce))).Distinct();
        }

        public EntityList All(params Type[] Types)
        {
            // return entities where _all_ of the types are present
            return (EntityList)this.Where((c) => Types.ToList().All(ce => c.HasComponent(ce))).Distinct();
        }

        public void Send(Entity sender, IMessage message)
        {
            this.ForEach((e) =>
            {
                e.Receive(sender, message);
            });
        }
    }
}
