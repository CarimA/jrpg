using JRPG.EntityComponent;
using JRPG.EntityComponent.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.ServiceLocator
{
    public class EntityService
    {
        public EntityManager Service { get; set; }

        public Entity Player(int controller)
        {
            Entity e = new Entity(Service, "player");
            e.AddComponents(new List<Component>()
            {
                new InputComponent(),
                new PositionComponent(),
                new TextureComponent("Debug/player"),
                new PlayerComponent()
            });
            return e;
        }
    }
}
