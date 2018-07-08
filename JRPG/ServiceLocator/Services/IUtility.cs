using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.ServiceLocator.Services
{
    public interface IUtility : IService
    {
        Task Wait(float time);

    }
}
