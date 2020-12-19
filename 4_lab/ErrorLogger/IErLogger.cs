using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorLogger
{
    public interface IErLogger
    {
        public void WriteError(Exception ex);
    }
}
