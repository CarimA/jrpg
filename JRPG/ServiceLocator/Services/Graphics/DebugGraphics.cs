using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.ServiceLocator.Services.Graphics
{
    public class DebugGraphics : IGraphics
    {
        public void Initialise()
        {

        }

        public void SetWindowTitle(string title)
        {
            Locator.GameInstance.Window.Title = title;
        }
    }
}
