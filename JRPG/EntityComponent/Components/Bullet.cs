using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent.Components
{
    public class Bullet : Component
    {
        public Entity Owner;

        public override void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Receive(Entity entity, IMessage message)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
