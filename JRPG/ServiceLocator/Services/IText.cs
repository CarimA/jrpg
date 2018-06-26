using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.ServiceLocator.Services
{
    public interface IText : IService
    {
        List<string> Maps { get; }
        List<string> Items { get; }
    }
}
