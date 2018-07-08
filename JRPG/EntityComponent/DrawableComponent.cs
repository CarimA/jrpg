using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent
{
    public abstract class DrawableComponent : Component
    {
        public abstract void DrawMask(GameTime gameTime);
        public abstract void DrawFringe(GameTime gameTime);
        public abstract void DrawUI(GameTime gameTime);   
    }
}
