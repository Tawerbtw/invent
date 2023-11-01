using Inventory.Models;
using Inventory.Utilities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Inventory.Pages
{
    public partial class MaterialsType : Page
    {
        public ObservableCollection<TypeMaterialModel> MaterialTypeList { get; set; }
        private readonly DataBaseConnection connection;
        private readonly MainWindow mainWindow;
        public MaterialsType(MainWindow mainWindow, DataBaseConnection connection)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.connection = connection;
            MaterialTypeList = connection.LoadMaterialTypesData();
            MaterialTypeListView.DataContext = this;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MaterialTypeNameTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            TypeMaterialModel MaterialType = new TypeMaterialModel
            {
                Name = MaterialTypeNameTextBox.Text
            };

            connection.AddMaterialType(MaterialType);

            MaterialTypeList.Add(MaterialType);

            ClearFields();

            UpdateTable();
        }

        private void MaterialTypeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MaterialTypeListView.SelectedItem != null)
            {
                TypeMaterialModel MaterialType = (TypeMaterialModel)MaterialTypeListView.SelectedItem;
                MaterialTypeNameTextBox.Text = MaterialType.Name;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialTypeListView.SelectedItem != null)
            {
                TypeMaterialModel MaterialType = (TypeMaterialModel)MaterialTypeListView.SelectedItem;

                MaterialType.Name = MaterialTypeNameTextBox.Text;

                connection.UpdateMaterialType(MaterialType);

                UpdateTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для редактирования.");
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialTypeListView.SelectedItem != null)
            {
                TypeMaterialModel selectedMaterialType = (TypeMaterialModel)MaterialTypeListView.SelectedItem;
                MaterialTypeList.Remove(selectedMaterialType);
                connection.DeleteMaterialType(selectedMaterialType.Id);
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
            MaterialTypeNameTextBox.Clear();
        }

        private void UpdateTable()
        {
            MaterialTypeListView.ItemsSource = null;
            MaterialTypeList.Clear();
            MaterialTypeList = connection.LoadMaterialTypesData();
            MaterialTypeListView.ItemsSource = MaterialTypeList;
        }

        private void searc_TextChanged(object sender, TextChangedEventArgs e)
        {
            TypeMaterialModel model = new TypeMaterialModel();
            SearchAndSort.SearchListView(MaterialTypeListView, model, searc.Text);
        }
    }
}
