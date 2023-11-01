using Inventory.Models;
using Inventory.Utilities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Inventory.Pages
{
    /// <summary>
    /// Логика взаимодействия для Programs.xaml
    /// </summary>
    public partial class Programs : Page
    {
        public ObservableCollection<ProgramModel> ProgramsList { get; set; }
        private readonly DataBaseConnection connection;
        private readonly MainWindow mainWindow;
        public Programs(MainWindow mainWindow, DataBaseConnection connection)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.connection = connection;
            ProgramsList = connection.LoadProgramData();
            ProgramsListView.DataContext = this;
            LoadDevelopersComboBox();
        }

        private void LoadDevelopersComboBox()
        {
            var list = connection.LoadDeveloperData();
            DeveloperComboBox.ItemsSource = list;
            DeveloperComboBox.DisplayMemberPath = "Name";
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProgramsNameTextBox.Text) || string.IsNullOrWhiteSpace(VersionTextBox.Text) || DeveloperComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }


            ProgramModel program = new ProgramModel
            {
                Name = ProgramsNameTextBox.Text,
                Developer = (DeveloperComboBox.SelectedItem as DeveloperModel)?.Id.ToString(),
                Version = VersionTextBox.Text
            };

            connection.AddProgram(program);

            ProgramsList.Add(program);

            ClearFields();

            UpdateTable();
        }

        private void ProgramsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProgramsListView.SelectedItem != null)
            {
                ProgramModel Programs = (ProgramModel)ProgramsListView.SelectedItem;
                ProgramsNameTextBox.Text = Programs.Name;
                DeveloperComboBox.SelectedValue = Programs.Developer;
                VersionTextBox.Text = Programs.Version;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProgramsListView.SelectedItem != null)
            {
                ProgramModel program = (ProgramModel)ProgramsListView.SelectedItem;

                program.Name = ProgramsNameTextBox.Text;
                program.Developer = (DeveloperComboBox.SelectedItem as DeveloperModel)?.Id.ToString();

                connection.UpdateProgram(program);

                UpdateTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для редактирования.");
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProgramsListView.SelectedItem != null)
            {
                ProgramModel selectedPrograms = (ProgramModel)ProgramsListView.SelectedItem;
                ProgramsList.Remove(selectedPrograms);
                connection.DeleteProgram(selectedPrograms.Id);
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
            ProgramsNameTextBox.Clear();
        }

        private void UpdateTable()
        {
            ProgramsListView.ItemsSource = null;
            ProgramsList.Clear();
            ProgramsList = connection.LoadProgramData();
            ProgramsListView.ItemsSource = ProgramsList;
        }

        private void searc_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProgramModel programModel = new ProgramModel();
            SearchAndSort.SearchListView(ProgramsListView, programModel, searc.Text);
        }
    }
}
