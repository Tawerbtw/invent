using Inventory.Models;
using Inventory.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;

namespace Inventory.Pages
{
    /// <summary>
    /// Логика взаимодействия для Network.xaml
    /// </summary>
    public partial class Network : Page
    {
        public ObservableCollection<NetworkModel> NetworkList { get; set; }
        private readonly MainWindow mainWindow;
        private readonly DataBaseConnection connection;
        public Network(MainWindow mainWindow, DataBaseConnection connection)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.connection = connection;
            NetworkList = connection.LoadNetworkData();
            NetworkListView.DataContext = this;
        }
        private void LimitSpaceInput(TextBox textBox, KeyEventArgs e)
        {
            // Запрет ввода пробела
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        private void LimitInputForIPAddress(TextBox textBox, TextCompositionEventArgs e)
        {
            // Получаем текст из TextBox
            string text = textBox.Text;

            // Разбиваем текст на секции
            string[] sections = text.Split('.');

            // Проверяем, что у нас уже есть четыре секции
            if (text.Length == 15)
            {
                e.Handled = true; // Запрещаем дополнительный ввод
                return;
            }

            // Проверяем, что вводимый символ - это цифра
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }

            // Добавляем точки в нужные позиции
            if (text.Length == 3 || text.Length == 7 || text.Length == 11)
            {
                text += ".";
            }

            textBox.Text = text;

            // Устанавливаем курсор в конец текста
            textBox.CaretIndex = text.Length;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string[] sections = MaskTextBox.Text.Split('.');
            foreach (string section in sections)
            {
                if (!int.TryParse(section, out int value) || value < 0 || value > 255)
                {
                    MessageBox.Show("Маска находится вне диапазона");
                    return;
                }
            }
            string[] sections1 = GetwayTextBox.Text.Split('.');
            foreach (string section in sections1)
            {
                if (!int.TryParse(section, out int value) || value < 0 || value > 255)
                {
                    MessageBox.Show("Шлюз находится вне диапазона");
                    return;
                }
            }
            string[] sections2 = DnsTextBox.Text.Split('.');

            foreach (string section in sections2)
            {
                if (!int.TryParse(section, out int value) || value < 0 || value > 255)
                {
                    MessageBox.Show("Днс находится вне диапазона");
                    return;
                }
            }
            string[] sections3 = ipAddressTextBox.Text.Split('.');
            foreach (string section in sections3)
            {
                if (!int.TryParse(section, out int value) || value < 0 || value > 255)
                {
                    MessageBox.Show("Ip находится вне диапазона");
                    return;
                }
            }
            if (string.IsNullOrWhiteSpace(ipAddressTextBox.Text) || string.IsNullOrWhiteSpace(MaskTextBox.Text) || string.IsNullOrWhiteSpace(GetwayTextBox.Text) || string.IsNullOrWhiteSpace(DnsTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }
            if (DnsTextBox.Text.Length != 15 || MaskTextBox.Text.Length != 15 || ipAddressTextBox.Text.Length != 15 || GetwayTextBox.Text.Length != 15)
            {
                MessageBox.Show("Заполните все поля.(Пример:XXX.XXX.XXX.XXX)");
                return;
            }
            foreach (NetworkModel networkModel in NetworkListView.Items)
            {
                if (networkModel.Ip == ipAddressTextBox.Text)
                {
                    MessageBox.Show("Этот IP уже существует.");
                    return;
                }
            }
            List<string> selectedEquipment = new List<string>();
            foreach (string equipment in EquipmentListBox.Items)
            {
                selectedEquipment.Add(equipment);
            }
            string equipmentString = string.Join(", ", selectedEquipment);
            NetworkModel Network = new NetworkModel
            {
                Ip = ipAddressTextBox.Text,
                Mask = MaskTextBox.Text,
                Gateway = GetwayTextBox.Text,
                Dns = equipmentString
            };
            connection.AddNetwork(Network);

            NetworkList.Add(Network);

            ClearFields();

            UpdateTable();
        }

        private void NetworkListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NetworkListView.SelectedItem != null)
            {

                NetworkModel network = (NetworkModel)NetworkListView.SelectedItem;
                string[] equipmentItems = network.Dns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                EquipmentListBox.Items.Clear();
                foreach (string equipment in equipmentItems)
                {
                    EquipmentListBox.Items.Add(equipment.Trim());
                }
                NetworkModel Network = (NetworkModel)NetworkListView.SelectedItem;
                ipAddressTextBox.Text = Network.Ip;
                MaskTextBox.Text = Network.Mask;
                GetwayTextBox.Text = Network.Gateway;
                if(EquipmentListBox.Items.Count == 1)
                {
                DnsTextBox.Text = Network.Dns;
                }
                else
                {
                    DnsTextBox.Text = null;
                }
                
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            string[] sections = MaskTextBox.Text.Split('.');
            foreach (string section in sections)
            {
                if (!int.TryParse(section, out int value) || value < 0 || value > 255)
                {
                    MessageBox.Show("Маска находится вне диапазона");
                    return;
                }
            }
            string[] sections1 = GetwayTextBox.Text.Split('.');
            foreach (string section in sections1)
            {
                if (!int.TryParse(section, out int value) || value < 0 || value > 255)
                {
                    MessageBox.Show("Шлюз находится вне диапазона");
                    return;
                }
            }
            string[] sections2 = DnsTextBox.Text.Split('.');
            foreach (string section in sections2)
            {
                if (!int.TryParse(section, out int value) || value < 0 || value > 255)
                {
                    MessageBox.Show("Днс находится вне диапазона");
                    return;
                }
            }
            string[] sections3 = ipAddressTextBox.Text.Split('.');
            foreach (string section in sections3)
            {
                if (!int.TryParse(section, out int value) || value < 0 || value > 255)
                {
                    MessageBox.Show("Ip находится вне диапазона");
                    return;
                }
            }
            if (string.IsNullOrWhiteSpace(ipAddressTextBox.Text) || string.IsNullOrWhiteSpace(MaskTextBox.Text) || string.IsNullOrWhiteSpace(GetwayTextBox.Text) || string.IsNullOrWhiteSpace(DnsTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }
            if (DnsTextBox.Text.Length != 15 || MaskTextBox.Text.Length != 15 || ipAddressTextBox.Text.Length != 15 || GetwayTextBox.Text.Length != 15)
            {
                MessageBox.Show("Заполните все поля.(Пример:XXX.XXX.XXX.XXX)");
                return;
            }
            foreach (NetworkModel networkModel in NetworkListView.Items)
            {
                if (networkModel.Ip == ipAddressTextBox.Text)
                {
                    MessageBox.Show("Этот IP уже существует.");
                    return;
                }
            }
            if (NetworkListView.SelectedItem != null)
            {
                NetworkModel Network = (NetworkModel)NetworkListView.SelectedItem;
                Network.Ip = ipAddressTextBox.Text;
                Network.Mask = MaskTextBox.Text;
                Network.Gateway = GetwayTextBox.Text;
                Network.Dns = DnsTextBox.Text;

                connection.UpdateNetwork(Network);

                UpdateTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для редактирования.");
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkListView.SelectedItem != null)
            {
                NetworkModel Network = (NetworkModel)NetworkListView.SelectedItem;
                connection.DeleteNetwork(Network.Id);
                NetworkList.Remove(Network);
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
            ipAddressTextBox.Clear();
            MaskTextBox.Clear();
            GetwayTextBox.Clear();
            DnsTextBox.Clear();
        }

        private void UpdateTable()
        {
            NetworkListView.ItemsSource = null;
            NetworkList.Clear();
            NetworkList = connection.LoadNetworkData();
            NetworkListView.ItemsSource = NetworkList;
        }

        private void searc_TextChanged(object sender, TextChangedEventArgs e)
        {
            NetworkModel network = new NetworkModel();
            SearchAndSort.SearchListView(NetworkListView, network, searc.Text);
        }

        private void MaskTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
            LimitSpaceInput((TextBox)sender, e);
        }

        private void MaskTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            LimitInputForIPAddress((TextBox)sender, e);  
        }

        private void GetwayTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            LimitInputForIPAddress((TextBox)sender, e);
            
        }

        private void GetwayTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            LimitSpaceInput((TextBox)sender, e);
        }

        private void DnsTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            LimitInputForIPAddress((TextBox)sender, e);
        }

        private void DnsTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            LimitSpaceInput((TextBox)sender, e);
        }

        private void ipAddressTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            LimitInputForIPAddress((TextBox)sender, e);
        }

        private void ipAddressTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            LimitSpaceInput((TextBox)sender, e);
        }

        private void AddEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            EquipmentListBox.Items.Add(DnsTextBox.Text);
        }

        private void RemoveEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedEquipment = (string)EquipmentListBox.SelectedItem;

            if (selectedEquipment != null)
            {
                // Удалите выбранное оборудование из ListBox.
                EquipmentListBox.Items.Remove(selectedEquipment);
            }
        }
    }
}
