using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.ServiceLocator.Services
{
    public interface IGraphics : IService
    {
        void Update(GameTime gameTime);

        void SetWindowTitle(string title);
    }
}
