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
    /// Логика взаимодействия для StatusAndDirection.xaml
    /// </summary>
    public partial class StatusAndDirection : Page
    {
        public ObservableCollection<StatusModel> StatusList { get; set; }
        public ObservableCollection<DirectionModel> DirectionList { get; set; }
        private readonly MainWindow mainWindow;
        private readonly DataBaseConnection connection;
        public StatusAndDirection(MainWindow mainWindow, DataBaseConnection connection)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.connection = connection;
            StatusList = connection.LoadStatusData();
            StatusListView.DataContext = this;
            DirectionList = connection.LoadDirectionData();
            DirectionListView.DataContext = this;
        }

        private void AddStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StatusNameTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            StatusModel status = new StatusModel
            {
                Name = StatusNameTextBox.Text
            };

            connection.AddStatus(status);

            StatusList.Add(status);

            ClearStatusFields();

            UpdateStatusTable();
        }

        private void StatusListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusListView.SelectedItem != null)
            {
                StatusModel selectedStatus = (StatusModel)StatusListView.SelectedItem;
                StatusNameTextBox.Text = selectedStatus.Name;
            }
        }

        private void EditStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (StatusListView.SelectedItem != null)
            {
                StatusModel selectedStatus = (StatusModel)StatusListView.SelectedItem;

                selectedStatus.Name = StatusNameTextBox.Text;

                connection.UpdateStatus(selectedStatus);

                UpdateStatusTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для редактирования.");
            }
        }

        private void RemoveStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (StatusListView.SelectedItem != null)
            {
                StatusModel selectedStatus = (StatusModel)StatusListView.SelectedItem;
                connection.DeleteStatus(selectedStatus.Id);
                StatusList.Remove(selectedStatus);
                ClearStatusFields();
                UpdateStatusTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для удаления.");
            }
        }

        private void ClearStatusFields()
        {
            StatusNameTextBox.Clear();
        }

        private void UpdateStatusTable()
        {
            StatusListView.ItemsSource = null;
            StatusList.Clear();
            StatusList = connection.LoadStatusData();
            StatusListView.ItemsSource = StatusList;
        }


        private void AddDirectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DirectionNameTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            DirectionModel Direction = new DirectionModel
            {
                Name = DirectionNameTextBox.Text
            };

            connection.AddDirection(Direction);

            DirectionList.Add(Direction);

            ClearDirectionFields();

            UpdateDirectionTable();
        }

        private void DirectionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DirectionListView.SelectedItem != null)
            {
                DirectionModel selectedDirection = (DirectionModel)DirectionListView.SelectedItem;
                DirectionNameTextBox.Text = selectedDirection.Name;
            }
        }

        private void EditDirectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (DirectionListView.SelectedItem != null)
            {
                DirectionModel selectedDirection = (DirectionModel)DirectionListView.SelectedItem;

                selectedDirection.Name = DirectionNameTextBox.Text;

                connection.UpdateDirection(selectedDirection);

                UpdateDirectionTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для редактирования.");
            }
        }

        private void RemoveDirectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (DirectionListView.SelectedItem != null)
            {
                DirectionModel selectedDirection = (DirectionModel)DirectionListView.SelectedItem;
                connection.DeleteDirection(selectedDirection.Id);
                DirectionList.Remove(selectedDirection);
                ClearDirectionFields();
                UpdateDirectionTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для удаления.");
            }
        }

        private void ClearDirectionFields()
        {
            DirectionNameTextBox.Clear();
        }

        private void UpdateDirectionTable()
        {
            DirectionListView.ItemsSource = null;
            DirectionList.Clear();
            DirectionList = connection.LoadDirectionData();
            DirectionListView.ItemsSource = DirectionList;
        }
        
            

        private void searcnap_TextChanged(object sender, TextChangedEventArgs e)
        {
            DirectionModel direction = new DirectionModel();
            SearchAndSort.SearchListView(DirectionListView, direction, searcnap.Text);
        }

        private void searc_TextChanged(object sender, TextChangedEventArgs e)
        {
            StatusModel stat = new StatusModel();
            SearchAndSort.SearchListView(StatusListView, stat, searc.Text);
        }
    }
}
