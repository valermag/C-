using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelss.MModels;

namespace DataAccess_Layer
{
    public interface IFiller<T>
    {
        public SearchRes<PersonalInfo> GetPersons();


    }
}
