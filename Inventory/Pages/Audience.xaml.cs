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
    /// Логика взаимодействия для Audience.xaml
    /// </summary>
    public partial class Audience : Page
    {
        public ObservableCollection<AuditoriumModel> AuditoriumList { get; set; }
        private readonly MainWindow mainWindow;
        private readonly DataBaseConnection connection;
        public Audience(MainWindow mainWindow, DataBaseConnection connection)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.connection = connection;
            AuditoriumList = connection.LoadAuditoriumData();
            AuditoriumListView.DataContext = this;
            LoadRespUserComboBox();
            LoadTimeRespUserComboBox();
        }

        private void LoadRespUserComboBox()
        {
            var list = connection.LoadUserData();
            ResponsibleUserComboBox.ItemsSource = list;
            ResponsibleUserComboBox.DisplayMemberPath = "Name";
        }

        private void LoadTimeRespUserComboBox()
        {
            var list = connection.LoadUserData();
            TemporaryUserComboBox.ItemsSource = list;
            TemporaryUserComboBox.DisplayMemberPath = "Name";
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AudienceNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(AudienceShortNameTextBox.Text) ||
                ResponsibleUserComboBox.SelectedItem == null ||
                TemporaryUserComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            AuditoriumModel auditorium = new AuditoriumModel
            {
                Name = AudienceNameTextBox.Text,
                ShortName = AudienceShortNameTextBox.Text,
                ResponsibleUser = (ResponsibleUserComboBox.SelectedItem as UserModel)?.Id.ToString(),
                TemporaryUser = (TemporaryUserComboBox.SelectedItem as UserModel)?.Name,
            };

            connection.AddAuditorium(auditorium);

            AuditoriumList.Add(auditorium);

            ClearFields();

            UpdateTable();
        }

        private void AuditoriumListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AuditoriumListView.SelectedItem != null)
            {
                AuditoriumModel selectedAuditorium = (AuditoriumModel)AuditoriumListView.SelectedItem;
                AudienceNameTextBox.Text = selectedAuditorium.Name;
                AudienceShortNameTextBox.Text = selectedAuditorium.ShortName;
                ResponsibleUserComboBox.SelectedValue = selectedAuditorium.Id;
                TemporaryUserComboBox.SelectedValue = selectedAuditorium.Id;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (AuditoriumListView.SelectedItem != null)
            {
                AuditoriumModel selectedAuditorium = (AuditoriumModel)AuditoriumListView.SelectedItem;

                selectedAuditorium.Name = AudienceNameTextBox.Text;
                selectedAuditorium.ShortName = AudienceShortNameTextBox.Text;
                selectedAuditorium.ResponsibleUser = (ResponsibleUserComboBox.SelectedItem as UserModel)?.Id.ToString();
                selectedAuditorium.TemporaryUser = (TemporaryUserComboBox.SelectedItem as UserModel)?.Name;

                connection.UpdateAuditorium(selectedAuditorium);

                UpdateTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для редактирования.");
            }
        }

        public void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (AuditoriumListView.SelectedItem != null)
            {
                AuditoriumModel selectedAuditorium = (AuditoriumModel)AuditoriumListView.SelectedItem;
                AuditoriumList.Remove(selectedAuditorium);
                connection.DeleteAuditorium(selectedAuditorium.Id);
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
            AudienceNameTextBox.Clear();
            AudienceShortNameTextBox.Clear();
            ResponsibleUserComboBox.SelectedIndex = -1;
            TemporaryUserComboBox.SelectedIndex = -1;
        }

        private void UpdateTable()
        {
            AuditoriumListView.ItemsSource = null;
            AuditoriumList.Clear();
            AuditoriumList = connection.LoadAuditoriumData();
            AuditoriumListView.ItemsSource = AuditoriumList;
        }

        private void SearchOnAuditoriumListView(string searchText)
        {
            AuditoriumModel auditorium = new AuditoriumModel();
            SearchAndSort.SearchListView(AuditoriumListView, auditorium, searchText);
        }

        private void searc_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchOnAuditoriumListView(searc.Text);
        }
    }
}
