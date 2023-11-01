using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для Reg.xaml
    /// </summary>
    public partial class Reg : Page
    {
        private readonly MainWindow mainWindow;
        public Reg(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenPage(MainWindow.pages.auth);
        }

        public bool IsValidFullName(string fullName)
        {
            string pattern = @"^[А-Яа-яЁё\s\-]+$";
            return Regex.IsMatch(fullName, pattern);
        }

        public bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        public bool IsValidPhoneNumber(string phoneNumber)
        {
            string pattern = @"^\+?\d+$";
            return Regex.IsMatch(phoneNumber, pattern);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Получаем значения из текстовых полей
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Text;
            string email = EmailTextBox.Text;
            string fullName = FullNameTextBox.Text;
            string phoneNumber = PhoneNumberTextBox.Text;
            string address = AddressTextBox.Text;

            // Проверяем все поля на пустоту
            if (string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(phoneNumber) ||
                string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Заполните все поля.");
                return;
            } else
            {
                // Создаем регулярные выражения
                string fullNamePattern = @"^[А-Яа-яЁё\s\-]+$";
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                string phoneNumberPattern = @"^\+7\d{10}$";

                // Проверяем значения полей через регулярные выражения
                if (!Regex.IsMatch(fullName, fullNamePattern))
                {
                    MessageBox.Show("Некорректное ФИО.");
                }
                else if (!Regex.IsMatch(email, emailPattern))
                {
                    MessageBox.Show("Некорректный Email.");
                }
                else if (!Regex.IsMatch(phoneNumber, phoneNumberPattern))
                {
                    MessageBox.Show("Некорректный номер телефона.");
                }
                else
                {
                    mainWindow.OpenPage(MainWindow.pages.equip);
                }
            }
        }
    }
}
