using Inventory.Models;
using Inventory.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для Users.xaml
    /// </summary>
    public partial class Users : Page
    {
        public ObservableCollection<UserModel> UsersList { get; set; }
        private readonly MainWindow mainWindow;
        private readonly DataBaseConnection connection;
        public Users(MainWindow mainWindow, DataBaseConnection connection)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.connection = connection;
            UsersList = connection.LoadUserData();
            UsersListView.DataContext = this;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(LoginTextBox.Text) || string.IsNullOrWhiteSpace(PasswordTextBox.Text) || RoleComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(EmailTextBox.Text)
                 || string.IsNullOrWhiteSpace(SecondNameTextBox.Text) || string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(LastNameTextBox.Text)
                 || string.IsNullOrWhiteSpace(PhoneTextBox.Text) || string.IsNullOrWhiteSpace(AddressTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }
            if (!IsValidEmail(EmailTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите корректный email.");
                return;
            }
            if (!IsValidPhoneNumber(PhoneTextBox.Text) )
            {
                MessageBox.Show("Пожалуйста, введите корректный номер телефона.");
                return;
            }
            if (PhoneTextBox.Text.Length != 11)
            {
                MessageBox.Show("Пожалуйста, введите корректный номер телефона.");
                return;
            }
            if (!IsValidAddress(AddressTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите корректный адрес.");
                return;
            }
            UserModel Programs = new UserModel
            {
                Login = LoginTextBox.Text,
                Password = PasswordTextBox.Text,
                Role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Email = EmailTextBox.Text,
                SecondName = LastNameTextBox.Text,
                Name = NameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Phone = PhoneTextBox.Text,
                Address = AddressTextBox.Text
            };

            connection.AddUser(Programs);

            UsersList.Add(Programs);

            ClearFields();

            UpdateTable();
        }
        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b");
        }
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^\+?\d+$");
        }

        private bool IsValidAddress(string address)
        {
            return !string.IsNullOrWhiteSpace(address);
        }
        private void UsersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersListView.SelectedItem != null)
            {
                UserModel Programs = (UserModel)UsersListView.SelectedItem;
                LoginTextBox.Text = Programs.Login;
                PasswordTextBox.Text = Programs.Password;
                RoleComboBox.SelectedItem = Programs.Role;
                EmailTextBox.Text = Programs.Email;
                LastNameTextBox.Text = Programs.LastName;
                NameTextBox.Text = Programs.Name;
                LastNameTextBox.Text = Programs.LastName;
                PhoneTextBox.Text = Programs.Phone;
                AddressTextBox.Text = Programs.Address;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidEmail(EmailTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите корректный email.");
                return;
            }
            if (!IsValidPhoneNumber(PhoneTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите корректный номер телефона.");
                return;
            }
            if(PhoneTextBox.Text.Length != 11)
            {
                MessageBox.Show("Пожалуйста, введите корректный номер телефона.");
                return;
            }
            if (!IsValidAddress(AddressTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите корректный адрес.");
                return;
            }
            if (UsersListView.SelectedItem != null)
            {
                UserModel Programs = (UserModel)UsersListView.SelectedItem;

                Programs.Login = LoginTextBox.Text;
                Programs.Password = PasswordTextBox.Text;
                Programs.Role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                Programs.Email = EmailTextBox.Text;
                Programs.SecondName = LastNameTextBox.Text;
                Programs.Name = NameTextBox.Text;
                Programs.LastName = LastNameTextBox.Text;
                Programs.Phone = PhoneTextBox.Text;
                Programs.Address = AddressTextBox.Text;

                connection.UpdateUser(Programs);

                UpdateTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для редактирования.");
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListView.SelectedItem != null)
            {
                UserModel selectedPrograms = (UserModel)UsersListView.SelectedItem;
                UsersList.Remove(selectedPrograms);
                connection.DeleteUser(selectedPrograms.Id);
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
        }

        private void UpdateTable()
        {
            UsersListView.ItemsSource = null;
            UsersList.Clear();
            UsersList = connection.LoadUserData();
            UsersListView.ItemsSource = UsersList;
        }

        private void searc_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserModel user = new UserModel();
            SearchAndSort.SearchListView(UsersListView, user, searc.Text);
        }
    }
}