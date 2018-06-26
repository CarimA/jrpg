using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.ServiceLocator.Services.Text
{
    public class EnglishText : IText
    {
        public List<string> Maps { get; private set; }
        public List<string> Items { get; private set; }

        public void Initialise()
        {

        }
    }
}
