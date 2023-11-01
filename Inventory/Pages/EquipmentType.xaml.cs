using Inventory.Models;
using Inventory.Utilities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Inventory.Pages
{
    /// <summary>
    /// Логика взаимодействия для EquipmentType.xaml
    /// </summary>
    public partial class EquipmentType : Page
    {
        public ObservableCollection<EquipmentTypeModel> EquipmentTypeList { get; set; }
        private readonly DataBaseConnection connection;
        private readonly MainWindow mainWindow;
        public EquipmentType(MainWindow mainWindow, DataBaseConnection connection)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.connection = connection;
            EquipmentTypeList = connection.LoadEquipmentTypesData();
            EquipmentTypeListView.DataContext = this;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EquipmentTypeNameTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            EquipmentTypeModel EquipmentType = new EquipmentTypeModel
            {
                Name = EquipmentTypeNameTextBox.Text
            };
            connection.AddEquipmentType(EquipmentType);
            EquipmentTypeList.Add(EquipmentType);
            ClearFields();
            UpdateTable();
        }

        private void EquipmentTypeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EquipmentTypeListView.SelectedItem != null)
            {
                EquipmentTypeModel EquipmentType = (EquipmentTypeModel)EquipmentTypeListView.SelectedItem;
                EquipmentTypeNameTextBox.Text = EquipmentType.Name;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentTypeListView.SelectedItem != null)
            {
                EquipmentTypeModel EquipmentType = (EquipmentTypeModel)EquipmentTypeListView.SelectedItem;

                EquipmentType.Name = EquipmentTypeNameTextBox.Text;

                connection.UpdateEquipmentType(EquipmentType);

                UpdateTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для редактирования.");
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentTypeListView.SelectedItem != null)
            {
                EquipmentTypeModel selectedEquipmentType = (EquipmentTypeModel)EquipmentTypeListView.SelectedItem;
                EquipmentTypeList.Remove(selectedEquipmentType);
                connection.DeleteEquipmentType(selectedEquipmentType.Id);
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
            EquipmentTypeNameTextBox.Clear();
        }

        private void UpdateTable()
        {
            EquipmentTypeListView.ItemsSource = null;
            EquipmentTypeList.Clear();
            EquipmentTypeList = connection.LoadEquipmentTypesData();
            EquipmentTypeListView.ItemsSource = EquipmentTypeList;
        }

        private void searc_TextChanged(object sender, TextChangedEventArgs e)
        {
            EquipmentTypeModel equipment = new EquipmentTypeModel();
            SearchAndSort.SearchListView(EquipmentTypeListView, equipment, searc.Text);
        }
    }
}
