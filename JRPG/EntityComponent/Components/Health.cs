﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace JRPG.EntityComponent.Components
{
    public class Health : Component
    {
        // receives IncreaseHealth/DecreaseHealth messages
        // sends HealthChanged message


        public override void Draw(GameTime gameTime)
        {

        }

        public override void Receive(Entity entity, IMessage message)
        {

        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}
