using Inventory.Models;
using Inventory.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Inventory.Pages
{
    /// <summary>
    /// Логика взаимодействия для Inventory.xaml
    /// </summary>
    public partial class Inventory : Page
    {
        public ObservableCollection<InventoryModel> InventoryList { get; set; }
        private readonly MainWindow mainWindow;
        private readonly DataBaseConnection connection;

        public Inventory(MainWindow mainWindow, DataBaseConnection connection)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.connection = connection;
            InventoryList = connection.LoadInventoryData();
            DataContext = this;
            LoadEquipmentComboBoxData();
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InventoryNameTextBox.Text) || StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null || EquipmentListBox.Items.Count == 0)
            {
                MessageBox.Show("Пожалуйста, заполните все поля и выберите оборудование.");
                return;
            }

            // Получите данные из полей и добавьте их в InventoryList
            DateTime startDate = (DateTime)StartDatePicker.SelectedDate;
            DateTime endDate = (DateTime)EndDatePicker.SelectedDate;

            // Получите выбранные элементы из ListBox.
            List<string> selectedEquipment = new List<string>();
            foreach (string equipment in EquipmentListBox.Items)
            {
                selectedEquipment.Add(equipment);
            }

            // Преобразуйте выбранные элементы в строку, объединив их названия через запятую.
            string equipmentString = string.Join(", ", selectedEquipment);
            InventoryModel inventory = new InventoryModel
            {
                Name = InventoryNameTextBox.Text,
                StartDate = startDate,
                EndDate = endDate,
                InventoriedEquipment = equipmentString,
                InventoryComment = InventoryCommentTextBox.Text,
            };

            connection.AddInventory(inventory);

            InventoryList.Add(inventory);

            ClearFields();

            UpdateTable();
        }
        private void InventoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InventoryListView.SelectedItem != null)
            {
                InventoryModel inventory = (InventoryModel)InventoryListView.SelectedItem;
                InventoryNameTextBox.Text = inventory.Name;

                // Разбиваем строку с оборудованием на отдельные элементы
                string[] equipmentItems = inventory.InventoriedEquipment.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                EquipmentListBox.Items.Clear();

                foreach (string equipment in equipmentItems)
                {
                    EquipmentListBox.Items.Add(equipment.Trim());
                }

                // Устанавливаем даты
                StartDatePicker.SelectedDate = inventory.StartDate;
                EndDatePicker.SelectedDate = inventory.EndDate;

                // Устанавливаем комментарий
                InventoryCommentTextBox.Text = inventory.InventoryComment;
            }
        }


        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryListView.SelectedItem != null)
            {
                if (string.IsNullOrWhiteSpace(InventoryNameTextBox.Text) || StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null || EquipmentListBox.Items.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля и выберите оборудование.");
                    return;
                }

                InventoryModel inventory = (InventoryModel)InventoryListView.SelectedItem;

                DateTime startDate = (DateTime)StartDatePicker.SelectedDate;
                DateTime endDate = (DateTime)EndDatePicker.SelectedDate;

                List<string> selectedEquipment = new List<string>();
                foreach (string equipment in EquipmentListBox.Items)
                {
                    selectedEquipment.Add(equipment);
                }

                string equipmentString = string.Join(", ", selectedEquipment);

                inventory.Name = InventoryNameTextBox.Text;
                inventory.StartDate = startDate;
                inventory.EndDate = endDate;
                inventory.InventoriedEquipment =inventory.Id.ToString();
                inventory.InventoryComment = InventoryCommentTextBox.Text;

                connection.UpdateInventory(inventory);

                UpdateTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для редактирования.");
            }
        }



        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryListView.SelectedItem != null)
            {
                InventoryModel selectedDeveloper = (InventoryModel)InventoryListView.SelectedItem;
                InventoryList.Remove(selectedDeveloper);
                connection.DeleteInventory(selectedDeveloper.Id);
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
            InventoryNameTextBox.Clear();
        }

        private void UpdateTable()
        {
            InventoryListView.ItemsSource = null;
            InventoryList.Clear();
            InventoryList = connection.LoadInventoryData();
            InventoryListView.ItemsSource = InventoryList;
        }

        private void LoadEquipmentComboBoxData()
        {
            var list = connection.LoadEquipmentData();
            EquipmentComboBox.ItemsSource = list;
            EquipmentComboBox.DisplayMemberPath = "EquipmentName";
        }

        private void EquipmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            // Получите выбранный элемент EquipmentModel из ComboBox.
            EquipmentModel selectedEquipment = EquipmentComboBox.SelectedItem as EquipmentModel;

            if (selectedEquipment != null)
            {
                // Добавьте выбранное оборудование в ListBox.
                EquipmentListBox.Items.Add(selectedEquipment.EquipmentName);

                // Очистите выбор, чтобы позволить выбрать другой элемент.
                EquipmentComboBox.SelectedItem = null;
            }
        }

        private void RemoveEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            // Получите выбранный элемент ListBox для удаления.
            string selectedEquipment = (string)EquipmentListBox.SelectedItem;

            if (selectedEquipment != null)
            {
                // Удалите выбранное оборудование из ListBox.
                EquipmentListBox.Items.Remove(selectedEquipment);
            }
        }

        private void searc_TextChanged(object sender, TextChangedEventArgs e)
        {
            InventoryModel invent = new InventoryModel();
            SearchAndSort.SearchListView(InventoryListView, invent, searc.Text);
        }
    }
}
