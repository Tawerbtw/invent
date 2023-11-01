using Inventory.Models;
using Inventory.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Inventory.Pages
{
    /// <summary>
    /// Логика взаимодействия для Developers.xaml
    /// </summary>
    public partial class Developers : Page
    {
        public ObservableCollection<DeveloperModel> DeveloperList { get; set; }
        private readonly DataBaseConnection connection;
        private readonly MainWindow mainWindow;
        public Developers(MainWindow mainWindow, DataBaseConnection connection)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.connection = connection;
            DeveloperList = connection.LoadDeveloperData();
            DeveloperListView.DataContext = this;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DeveloperNameTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            DeveloperModel Developer = new DeveloperModel
            {
                Name = DeveloperNameTextBox.Text
            };

            connection.AddDeveloper(Developer);

            DeveloperList.Add(Developer);

            ClearFields();

            UpdateTable();
        }

        private void DeveloperListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeveloperListView.SelectedItem != null)
            {
                DeveloperModel Developer = (DeveloperModel)DeveloperListView.SelectedItem;
                DeveloperNameTextBox.Text = Developer.Name;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeveloperListView.SelectedItem != null)
            {
                DeveloperModel Developer = (DeveloperModel)DeveloperListView.SelectedItem;

                Developer.Name = DeveloperNameTextBox.Text;

                connection.UpdateDeveloper(Developer);

                UpdateTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для редактирования.");
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeveloperListView.SelectedItem != null)
            {
                DeveloperModel selectedDeveloper = (DeveloperModel)DeveloperListView.SelectedItem;
                DeveloperList.Remove(selectedDeveloper);
                connection.DeleteDeveloper(selectedDeveloper.Id);
                ClearFields();
                UpdateTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для удаления.");
            }
        }

        private void ClearFields()
        {
            DeveloperNameTextBox.Clear();
        }

        private void UpdateTable()
        {
            DeveloperListView.ItemsSource = null;
            DeveloperList.Clear();
            DeveloperList = connection.LoadDeveloperData();
            DeveloperListView.ItemsSource = DeveloperList;
        }

        private void searc_TextChanged(object sender, TextChangedEventArgs e)
        {
            DeveloperModel developer = new DeveloperModel();
            SearchAndSort.SearchListView(DeveloperListView, developer, searc.Text);
        }
    }
}
