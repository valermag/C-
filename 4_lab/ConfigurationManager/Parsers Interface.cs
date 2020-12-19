using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationMeneger
{
    interface IParser
    {
        public T GetOptions<T>();
    }
}
