using MyTaskV1.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTaskV1
{
    class Bdd
    {
        /// <summary>
        /// instance de la ConfigBDD
        /// </summary>
        ConfigBDD _config; 

        /// <summary>
        /// instance SQLiteConnection pour génerer les connexions à la Bdd
        /// </summary>
        SQLiteConnection _con;

        /// <summary>
        /// utilise la configuration dans Config Bdd pour créer et mettre en place la Bdd
        /// </summary>
         void CreateBDD()
        {
           
            _con.Open();

            foreach (TableBDD table in _config.ListTable)
            {
                string req = $"CREATE TABLE IF NOT EXISTS {table.Name} (";
                int count = 0;
                foreach (ChampsTable champ in table.Champs)
                {
                    count++;
                    if (table.Champs.Count() == count)
                    {
                        req += champ.Name + " " + champ.Designation + ")";
                        count = 0;
                    }
                    else
                    {
                        req += champ.Name + " " + champ.Designation + ", ";
                    }
                }
                SQLiteCommand command = new SQLiteCommand(req, _con);
                command.ExecuteNonQuery();
            }
            _con.Close();
        }

        /// <summary>
        /// Permet de générer une requête à la Bdd pour récupérer les informations pour intancier une Collection de Task grace à une date
        /// </summary>
        /// <param name="table"></param>
        /// <param name="date"></param>
        /// <returns>Collection de Task</returns>
        public ObservableCollection<Task> SelectDataTask(string table, string date)
        {
            _con.Open();

            string req = $"SELECT * FROM {table} WHERE date='{date + " 00:00:00"}' ORDER BY ordre";

            SQLiteCommand command = new SQLiteCommand(req, _con);
            SQLiteDataReader dr = command.ExecuteReader();
            SQLiteCommand commandCount = new SQLiteCommand(req, _con);
            SQLiteDataReader count = commandCount.ExecuteReader();

            var listTask = new ObservableCollection<Task>();
            int last = 0;
            int counter = 0;
            int taskEnd = 0;

            while (count.Read())
            {
                if (count.GetInt32(3) == 0)
                {
                    taskEnd++;
                }
                counter++;
            }
            Debug.WriteLine(counter);

            while (dr.Read())
            {
                if (dr.GetInt32(3) == (counter - taskEnd)) last = 1;

                listTask.Add(
                    new Task()
                    {
                        Id = dr.GetInt32(0),
                        Label = dr.GetString(1),
                        Statut = dr.GetString(2),
                        Ordre = dr.GetInt32(3),
                        Date = dr.GetString(4),
                        Last = last
                    }
                );
            }

            _con.Close();

            return listTask;
        }

        /// <summary>
        /// Permet de générer une requête à la Bdd pour récupérer les informations pour intancier une Collection de Task pour le démarage de l'application et effectuer une recherche
        /// pour récuperer les taches non terminé anterieur à la date du jour, de les affectés un ordre et de les modifier à la date donnée
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Collection de Task</returns>
        public ObservableCollection<Task> InitProg(string date)
        {
            _config = new ConfigBDD();
            _con = new SQLiteConnection($"Data Source={_config.Patch};Version=3;");

            CreateBDD();

            _con.Open();

            string req = $"SELECT * FROM tasks WHERE date <='{date + " 00:00:00"}' AND statut != 'terminé' OR date ='{date + " 00:00:00"}' AND statut = 'terminé' ;";
            string req2 = $"SELECT COUNT(*) FROM tasks WHERE date <='{date + " 00:00:00"}' AND statut != 'terminé' OR date ='{date + " 00:00:00"}' AND statut = 'terminé' ;";
            SQLiteCommand commandCountArchive = new SQLiteCommand(req2, _con);
            SQLiteDataReader countArchive = commandCountArchive.ExecuteReader();
            int counterArchive = 0;
            while (countArchive.Read())
            {
                counterArchive = countArchive.GetInt32(0);
                Debug.WriteLine("le nombre est de :" + countArchive.GetInt32(0));
            }
            if (counterArchive == 0)
            {
                _con.Close();
                return SelectDataTask("tasks", date);
            }

            SQLiteCommand commandArchive = new SQLiteCommand(req, _con);
            SQLiteDataReader drArchive = commandArchive.ExecuteReader();

            int ordre = 1;

            while (drArchive.Read())
            {
                if (drArchive.GetInt32(3) != 0)
                {
                    string reqUp = $"UPDATE tasks SET ordre='{ordre}', date='{date + " 00:00:00"}' WHERE id = {drArchive.GetInt32(0)};";
                    SQLiteCommand commandUp = new SQLiteCommand(reqUp, _con);
                    commandUp.ExecuteNonQuery();
                    ordre++;
                }
            }

            _con.Close();

            return SelectDataTask("tasks", date);
        }

        /// <summary>
        /// Grace au information d'un dictionary, la fonction va effectuer une requete à la bdd pour inserer une nouvelle Tache et retourné la collection de tache mis a jour
        /// </summary>
        /// <param name="newTask"></param>
        /// <returns> Collection de Task</returns>
        public ObservableCollection<Task> addNewTask(Dictionary<string, string> newTask)
        {
            _con.Open();
            string req = $"INSERT INTO tasks (label, date, ordre, statut) VALUES('{newTask["Label"]}', '{DateTime.Parse(newTask["Date"])}', '{Int32.Parse(newTask["Ordre"])}', '{newTask["Statut"]}')";
            SQLiteCommand command = new SQLiteCommand(req, _con);
            command.ExecuteNonQuery();
            _con.Close();
            var data = SelectDataTask("tasks", newTask["Date"]);
            return data;
        }

        /// <summary>
        /// Grace a l'id d'une Tache et la date du jour, il va effecer la tache dans la bdd + mis à jour de la collection de tache
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <returns>Collection de Task</returns>
        public ObservableCollection<Task> deleteTask(string id, string date)
        {
            _con.Open();
            string req = $"DELETE FROM tasks WHERE id = {id};";
            SQLiteCommand command = new SQLiteCommand(req, _con);
            command.ExecuteNonQuery();
            _con.Close();
            var data = SelectDataTask("tasks", date);
            return data;
        }


        /// <summary>
        /// Mise à jour du statut d'une tache grace à un object modifié en parametre
        /// </summary>
        /// <param name="task"></param>
        public void UpdateStatut(Task task)
        {
            _con.Open();

            string req;

            if (task.Ordre == 0)
            {
                req = $"UPDATE tasks SET statut='{task.Statut}', ordre='{task.Ordre}' WHERE id = {task.Id};";
            }
            else
            {
                req = $"UPDATE tasks SET statut='{task.Statut}' WHERE id = {task.Id};";
            }

            SQLiteCommand command = new SQLiteCommand(req, _con);
            command.ExecuteNonQuery();
            _con.Close();
        }

        /// <summary>
        /// Mise à jour d'un label d'une tache grace à un object modifié en parametre
        /// </summary>
        /// <param name="task"></param>
        public void UpdateLabel(Task task)
        {
            _con.Open();
            string req = $"UPDATE tasks SET label='{task.Label}' WHERE id = {task.Id};";
            SQLiteCommand command = new SQLiteCommand(req, _con);
            command.ExecuteNonQuery();
            _con.Close();
        }

        /// <summary>
        /// Modifie l'ordre de deux taches, grace à deux taches modifiées en parametre
        /// </summary>
        /// <param name="up"></param>
        /// <param name="down"></param>
        public void UpdateOrderList(Task up, Task down)
        {
            _con.Open();
            string reqUp = $"UPDATE tasks SET ordre='{up.Ordre}' WHERE id = {up.Id};";
            SQLiteCommand commandUp = new SQLiteCommand(reqUp, _con);
            commandUp.ExecuteNonQuery();

            string reqDown = $"UPDATE tasks SET ordre='{down.Ordre}' WHERE id = {down.Id};";
            SQLiteCommand commandDown = new SQLiteCommand(reqDown, _con);
            commandDown.ExecuteNonQuery();
            _con.Close();
        }

        /// <summary>
        /// modifie l'ordre d'une tache, grace à son id et son nouvel ordre
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ordre"></param>
        public void ChangeOrder(int id, int ordre)
        {
            _con.Open();
            string reqUp = $"UPDATE tasks SET ordre='{ordre}' WHERE id = {id};";
            SQLiteCommand commandUp = new SQLiteCommand(reqUp, _con);
            commandUp.ExecuteNonQuery();
            _con.Close();
        }
    }
}
