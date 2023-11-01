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
    /// Логика взаимодействия для Auth.xaml
    /// </summary>
    public partial class Auth : Page
    {
        private readonly MainWindow mainWindow;
        private ObservableCollection<UserModel> users;
        DataBaseConnection user=new DataBaseConnection();

        public Auth(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            users = user.LoadUserData();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenPage(MainWindow.pages.reg);
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password; // Замените PasswordTextBox.ToString() на PasswordTextBox.Text

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните поля логина и пароля.");
                return;
            }
            

            UserModel user = users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user == null)
            {
                MessageBox.Show("Неправильный логин или пароль");
                return;
            }
            else
            {
                
            }

            // Вход выполнен успешно
            if (mainWindow != null)
            {
                mainWindow.OpenPage(MainWindow.pages.equip);
            }
        }
    }
}
