using Inventory.Models;
using Inventory.Utilities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Inventory.Pages
{
    /// <summary>
    /// Логика взаимодействия для CharConsumables.xaml
    /// </summary>
    public partial class CharConsumables : Page
    {
        public ObservableCollection<CharConsumbalesModel> ModelTypeList { get; set; }
        private readonly MainWindow mainWindow;
        private readonly DataBaseConnection connection;
        public CharConsumables(MainWindow mainWindow, DataBaseConnection connection)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.connection = connection;
            ModelTypeList = connection.LoadCharConsumablesData();
            listView.DataContext = this;
            LoadMaterialTypeComboBox();
        }

        private void LoadMaterialTypeComboBox()
        {
            var list = connection.LoadMaterialTypesData();
            TypeCombobox.ItemsSource = list;
            TypeCombobox.DisplayMemberPath = "Name";
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || TypeCombobox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            CharConsumbalesModel ModelType = new CharConsumbalesModel
            {
                CharName = NameTextBox.Text,
                Consumbale = (TypeCombobox.SelectedItem as TypeMaterialModel)?.Id.ToString()
            };

            connection.AddCharConsumable(ModelType);

            ModelTypeList.Add(ModelType);

            ClearFields();

            UpdateTable();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView.SelectedItem != null)
            {
                CharConsumbalesModel ModelType = (CharConsumbalesModel)listView.SelectedItem;
                NameTextBox.Text = ModelType.CharName;
                TypeCombobox.SelectedValue = ModelType.Id;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem != null)
            {
                CharConsumbalesModel ModelType = (CharConsumbalesModel)listView.SelectedItem;

                ModelType.CharName = NameTextBox.Text;
                ModelType.Consumbale = (TypeCombobox.SelectedItem as TypeMaterialModel)?.Id.ToString();

                connection.UpdateCharConsumable(ModelType);

                UpdateTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для редактирования.");
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem != null)
            {
                CharConsumbalesModel selectedModelType = (CharConsumbalesModel)listView.SelectedItem;
                connection.DeleteCharConsumable(selectedModelType.Id);
                ModelTypeList.Remove(selectedModelType);
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
            NameTextBox.Clear();
            TypeCombobox.SelectedIndex = -1;
        }

        private void UpdateTable()
        {
            listView.ItemsSource = null;
            ModelTypeList.Clear();
            ModelTypeList = connection.LoadCharConsumablesData();
            listView.ItemsSource = ModelTypeList;
        }

        private void searc_TextChanged(object sender, TextChangedEventArgs e)
        {
            CharConsumbalesModel charConsumables = new CharConsumbalesModel();
            SearchAndSort.SearchListView(listView, charConsumables, searc.Text);
        }
    }
}
