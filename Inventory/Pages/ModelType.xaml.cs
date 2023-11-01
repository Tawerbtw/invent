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
    /// Логика взаимодействия для ModelType.xaml
    /// </summary>
    public partial class ModelType : Page
    {
        public ObservableCollection<ModelTypeModel> ModelTypeList { get; set; }
        private readonly DataBaseConnection connection;
        private readonly MainWindow mainWindow;
        public ModelType(MainWindow mainWindow, DataBaseConnection connection)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.connection = connection;
            ModelTypeList = connection.LoadModelTypeData();
            ModelTypeListView.DataContext = this;
            LoadCodeTypeComboBox();
        }

        private void LoadCodeTypeComboBox()
        {
            var list = connection.LoadEquipmentTypesData();
            EquipmentTypeCodeComboBox.ItemsSource = list;
            EquipmentTypeCodeComboBox.DisplayMemberPath = "Id";
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ModelTypeNameTextBox.Text) || EquipmentTypeCodeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            ModelTypeModel modelType = new ModelTypeModel
            {
                Name = ModelTypeNameTextBox.Text,
                EquipmentTypeCode = Convert.ToString((EquipmentTypeCodeComboBox.SelectedItem as EquipmentTypeModel)?.Id)
            };

            connection.AddModelType(modelType);

            ModelTypeList.Add(modelType);

            ClearFields();

            UpdateTable();
        }


        private void ModelTypeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModelTypeListView.SelectedItem != null)
            {
                ModelTypeModel ModelType = (ModelTypeModel)ModelTypeListView.SelectedItem;
                ModelTypeNameTextBox.Text = ModelType.Name;
                EquipmentTypeCodeComboBox.SelectedValue = ModelType.EquipmentTypeCode;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModelTypeListView.SelectedItem != null)
            {
                ModelTypeModel modelType = (ModelTypeModel)ModelTypeListView.SelectedItem;

                modelType.Name = ModelTypeNameTextBox.Text;
                modelType.EquipmentTypeCode = Convert.ToString((EquipmentTypeCodeComboBox.SelectedItem as EquipmentTypeModel)?.Id);

                connection.UpdateModelType(modelType);

                UpdateTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для редактирования.");
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModelTypeListView.SelectedItem != null)
            {
                ModelTypeModel selectedModelType = (ModelTypeModel)ModelTypeListView.SelectedItem;
                ModelTypeList.Remove(selectedModelType);
                connection.DeleteModelType(selectedModelType.Id);
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
            ModelTypeNameTextBox.Clear();
        }

        private void UpdateTable()
        {
            ModelTypeListView.ItemsSource = null;
            ModelTypeList.Clear();
            ModelTypeList = connection.LoadModelTypeData();
            ModelTypeListView.ItemsSource = ModelTypeList;
        }

        private void searc_TextChanged(object sender, TextChangedEventArgs e)
        {
            ModelTypeModel model = new ModelTypeModel();
            SearchAndSort.SearchListView(ModelTypeListView, model, searc.Text);
        }
    }
}

