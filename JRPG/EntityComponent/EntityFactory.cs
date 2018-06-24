using JRPG.EntityComponent.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent
{
    public partial class Entity
    {
        public static Entity Player(EntityManager entityManager, PlayerIndex controller)
        {
            Entity e = new Entity(entityManager, "player");
            e.AddComponents(new List<Component>()
            {
                new Player(),
                new Input(controller),
                new Position(),
                new Rotation()
            });
            return e;
        }
    }
}
