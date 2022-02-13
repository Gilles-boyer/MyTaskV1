using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTaskV1.Service
{
    class ChampsTable
    {
        string _name;
        string _designation;

        public ChampsTable(string name, string designation)
        {
            _name = name;
            _designation = designation;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Designation
        {
            get
            {
                return _designation;
            }
        }
    }
}
