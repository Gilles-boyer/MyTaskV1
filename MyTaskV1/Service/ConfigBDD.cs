using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTaskV1.Service
{
    class ConfigBDD
    {
        string _patch = "bdd.sqlite";
        List<TableBDD> _listTable = new List<TableBDD>();
        TableBDD _tasks = new TableBDD("tasks");

        public ConfigBDD()
        {
            if (!File.Exists(_patch)) SQLiteConnection.CreateFile(_patch);
            ConfigTask();
        }

        public string Patch
        {
            get { return _patch; }
        }

        public List<TableBDD> ListTable
        {
            get { return _listTable; }
        }

        /// <summary>
        /// ajoute les champs et la configuration d'une task et ajoute à la liste des tables, la table tasks
        /// </summary>
        void ConfigTask()
        {
            _tasks.addChamps(new ChampsTable("id", "INTEGER PRIMARY KEY AUTOINCREMENT"));
            _tasks.addChamps(new ChampsTable("label", "varchar(255) not null"));
            _tasks.addChamps(new ChampsTable("statut", "varchar(255) not null"));
            _tasks.addChamps(new ChampsTable("ordre", "int not null"));
            _tasks.addChamps(new ChampsTable("date", "date not null"));

            _listTable.Add(_tasks);
        }
    }
}
