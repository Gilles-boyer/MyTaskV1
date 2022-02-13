using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTaskV1.Service
{
    class TableBDD
    {
        string _name;
        List<ChampsTable> _champs = new List<ChampsTable>();


        public string Name
        {
            get
            {
                return _name;
            }
        }

        public List<ChampsTable> Champs
        {
            get
            {
                return _champs;
            }
        }

        public TableBDD(string name)
        {
            _name = name;
        }

        public void addChamps(ChampsTable champ)
        {
            _champs.Add(champ);
        }
    }
}
