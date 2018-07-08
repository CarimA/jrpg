using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.ServiceLocator.Services.Utility
{
    public class DebugUtility : IUtility
    {
        public void Initialise()
        {

        }

        public async Task Wait(float time)
        {
            System.Threading.Thread.Sleep((int)(time * 1000));
        }
    }
}
