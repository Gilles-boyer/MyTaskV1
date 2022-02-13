using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTaskV1.ViewModel
{
    class ListsAndGridsViewModel
    {
        private Bdd _bddAction;

        /// <summary>
        /// Affecte le Compteur Objet et appel la fonction Bdd Select
        /// </summary>
        /// <param name="DateString"></param>
        /// <returns>List Task</returns>
        public ObservableCollection<Task> CreateDataTask(string DateString)
        {
            var dr = _bddAction.SelectDataTask("tasks", DateString);

            CountItems = dr.Count();

            return dr;
        }

        public void initData()
        {
            _bddAction = new Bdd();
            Items1 = _bddAction.InitProg(DateTime.Today.ToString("dd/MM/yyyy"));

            CountItems = Items1.Count();
        }

        /// <summary>
        ///  Ajoute une nouvelle tache en appellant la fonction Bdd
        /// </summary>
        /// <param name="task"></param>
        /// <returns>Nouvelle liste de Task mis a jour</returns>
        public ObservableCollection<Task> AddTask(Dictionary<string, string> task)
        {
            return _bddAction.addNewTask(task);
        }

        /// <summary>
        /// Efface une tache dans la bdd
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <returns>Nouvelle liste de Task mis a jour</returns>
        public ObservableCollection<Task> deleteTaskWithId(string id, string date)
        {
            return _bddAction.deleteTask(id, date);
        }

        /// <summary>
        /// update le statut d'une Tache
        /// </summary>
        /// <param name="task"></param>
        public void UpdateStatutTask(Task task)
        {
            _bddAction.UpdateStatut(task);
        }

        /// <summary>
        /// update le label d'une Tache
        /// </summary>
        /// <param name="task"></param>
        public void UpdateLabel(Task task)
        {
            _bddAction.UpdateLabel(task);
        }

        /// <summary>
        /// update l'ordre de deux taches
        /// </summary>
        /// <param name="up"></param>
        /// <param name="down"></param>
        public void UpOrDownOrderTask(Task up, Task down)
        {
            _bddAction.UpdateOrderList(up, down);
        }

        public ObservableCollection<Task> Items1 { get; set; }

        public int CountItems { get; set; }
    }
}
