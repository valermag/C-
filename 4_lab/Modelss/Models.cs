using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelss.MModels
{
    public class PersonalInfo
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public string MaritalStatus { get; set; }

        public byte PayCounter { get; set; }

        public decimal PaySize { get; set; }

        public string PhoneNumber { get; set; }


        public string EmailAddress { get; set; }

        public string JobTitle { get; set; }

        public int Vacation { get; set; }

        public string ShiftTime { get; set; }

        public string ProffSphere { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Street { get; set; }

        public string Country { get; set; }

        public string Province { get; set; }

        public string Continent { get; set; }
    }
    
}
