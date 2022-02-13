using MaterialDesignThemes.Wpf;
using MyTaskV1.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyTaskV1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string date;
        ListsAndGridsViewModel list;
        public MainWindow()
        {
            initStart();
            //InitializeComponent();
        }

         private void initStart()
        {
            list = new ListsAndGridsViewModel();
            list.initData();
            this.DataContext = list;
            date = DateTime.Today.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Fonction de gestion du date picker
        /// change la date et l'affichage de la date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dpick_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            date = date_select.Text;

            var test = list.CreateDataTask(date);

            listView.ItemsSource = test;
            list.Items1 = test;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewTaskTextbox.Text = "";
        }

        /// <summary>
        ///  Ajoute ou modifie grace à son id une tache
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void Sample5_DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            Debug.WriteLine($"SAMPLE 5: Closing dialog with parameter: {eventArgs.Parameter ?? string.Empty}");

            if (!Equals(eventArgs.Parameter, true))
                return;
            if (DateTime.Now > DateTime.Parse(date + " 23:59:00"))
            {
                MessageBox.Show("Impossible d'ajouter une tache à une journée antérieur à la date du jour !");
                return;
            }

            if (NewTaskTextbox.Text != "" && NewTaskTextbox.Tag.ToString() != "")
            {
                Task task = list.Items1.Where(c => c.Id == Int32.Parse(NewTaskTextbox.Tag.ToString())).First();

                if (task.Statut == "terminé")
                {
                    MessageBox.Show("Vous ne pouvez pas modifier une tache terminée");
                    return;
                }
                task.Label = NewTaskTextbox.Text;

                list.UpdateLabel(task);
            }
            else
            {
                if (NewTaskTextbox.Text.Length == 0) { return; }

                Dictionary<string, string> addTask = new Dictionary<string, string>();

                addTask.Add("Label", NewTaskTextbox.Text);
                addTask.Add("Date", date);
                addTask.Add("Ordre", (list.Items1.Count + 1).ToString());
                addTask.Add("Statut", "à faire");

                var test = list.AddTask(addTask);

                listView.ItemsSource = test;
                list.Items1 = test;
            }

            NewTaskTextbox.Tag = "";
            NewTaskTextbox.Text = "";
        }

        /// <summary>
        /// Efface une tache grace à son id
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_delete_task(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            var result = MessageBox.Show("Voulez-vous vraiment supprimer la tache : " + btn.Tag.ToString(), "Supprimer une tache", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result.ToString() == "Yes")
            {
                var data = list.deleteTaskWithId(btn.Tag.ToString(), date);
                list.Items1 = data;
                listView.ItemsSource = data;
            }
        }

        /// <summary>
        /// mis a jour du statut 
        /// Statut = en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateStartTask(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            Task task = list.Items1.Where(c => c.Id == Int32.Parse(btn.Tag.ToString())).First();

            if (task.Statut != "à faire")
            {
                MessageBox.Show("Merci de vérfier votre action, car une erreur a été constaté");
                return;
            }

            task.Statut = "en cours";
            btn.Visibility = Visibility.Hidden;

            list.UpdateStatutTask(task);

            MessageBox.Show("Le Statut a bien été mis à jour", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Mis à jour du statut
        /// Statut = terminé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateEndTask(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            Task task = list.Items1.Where(c => c.Id == Int32.Parse(btn.Tag.ToString())).First();

            if (task.Statut != "en cours")
            {
                MessageBox.Show("Merci de vérfier votre action, car une erreur a été constaté");
                return;
            }

            task.Statut = "terminé";
            task.Ordre = 0;
            btn.Visibility = Visibility.Hidden;

            list.UpdateStatutTask(task);

            MessageBox.Show("Le Statut a bien été mis à jour", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Configure le text box d'ajout d'une nouvelle tache pour effectuer sa mis à jour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            Task task = list.Items1.Where(c => c.Id == Int32.Parse(btn.Tag.ToString())).First();
            NewTaskTextbox.Text = task.Label;
            NewTaskTextbox.Tag = task.Id;

        }

        /// <summary>
        /// Modifie l'ordre d'une tache, up la tache
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_up_ordre(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Task taskUp = list.Items1.Where(c => c.Id == Int32.Parse(btn.Tag.ToString())).First();

            if (taskUp.Ordre == 1)
            {
                MessageBox.Show("Opération impossible !");
                return;
            }

            Task taskDown = list.Items1.Where(c => c.Ordre == (taskUp.Ordre - 1)).First();

            if ((taskDown.Ordre + 1) == list.Items1.Count())
            {
                taskUp.Last = 0;
                taskDown.Last = 1;
            }

            taskUp.Ordre = taskUp.Ordre - 1;
            taskDown.Ordre = taskDown.Ordre + 1;

            list.UpOrDownOrderTask(taskUp, taskDown);

            listView.ItemsSource = list.Items1 = new ObservableCollection<Task>(list.Items1.OrderBy(c => c.Ordre));

        }

        /// <summary>
        /// Modifie l'ordre d'une tache, down la tache
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_down_ordre(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Task taskUp = list.Items1.Where(c => c.Id == Int32.Parse(btn.Tag.ToString())).First();

            if (taskUp.Ordre == list.Items1.Count())
            {
                MessageBox.Show("Opération impossible !");
                return;
            }

            Task taskDown = list.Items1.Where(c => c.Ordre == (taskUp.Ordre + 1)).First();

            if (taskUp.Ordre == (list.Items1.Count() - 1))
            {
                taskUp.Last = 1;
                taskDown.Last = 0;
            }

            taskUp.Ordre = taskUp.Ordre + 1;
            taskDown.Ordre = taskDown.Ordre - 1;

            list.UpOrDownOrderTask(taskDown, taskUp);

            listView.ItemsSource = list.Items1 = new ObservableCollection<Task>(list.Items1.OrderBy(c => c.Ordre));
        }


    }
}
