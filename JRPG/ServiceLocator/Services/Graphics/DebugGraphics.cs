using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JRPG.ServiceLocator.Services.Graphics
{
    public class DebugGraphics : IGraphics
    {
        string _title;

        public void Initialise()
        {

        }

        public void Update(GameTime gameTime)
        {
            if (_title != null)
            {
                Locator.GameInstance.Window.Title = _title;
                _title = null;
            }
        }

        public void SetWindowTitle(string title)
        {
            _title = title;   
        }

    }
}
