using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JRPG.ServiceLocator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JRPG.EntityComponent.Components
{
    public class TextureComponent : Component
    {
        private string _texturePath;
        public Texture2D Texture; 

        public TextureComponent(string texturePath)
        {
            _texturePath = texturePath;
        }
        
        public override void Update(GameTime gameTime)
        {
            if (Texture == null)
            {
                Texture = Game.Content.Load<Texture2D>(_texturePath);
            }
        }
    }
}
