using Inventory.Models;
using Inventory.Pages;
using Inventory.Utilities;
using System;
using System.Collections.Generic;
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

namespace Inventory
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataBaseConnection connection;
        UserModel user;
        public MainWindow()
        {
            InitializeComponent();
            connection = new DataBaseConnection();
            user =new UserModel();

            OpenPage(pages.auth);
        }

        public enum pages
        {
            auth,
            reg,
            equip,
            audience,
            statusanddirection,
            equipmenttype,
            modeltype,
            programs,
            developers,
            inventory,
            users,
            network,
            materials,
            charconsumables,
            typematerials
        }

        public void OpenPage(pages page)
        {
            switch(page)
            {
                case pages.auth:
                    Header.Visibility = Visibility.Hidden;
                    frame.Navigate(new Auth(this));
                    break;
                case pages.reg:
                    Header.Visibility = Visibility.Hidden;
                    frame.Navigate(new Reg(this));
                    break;
                case pages.equip:
                    Header.Visibility = Visibility.Visible;
                    frame.Navigate(new Equipment(this, connection));
                    break;
                case pages.audience:
                    Header.Visibility = Visibility.Visible;
                    frame.Navigate(new Audience(this, connection));
                    break;
                case pages.statusanddirection:
                    Header.Visibility = Visibility.Visible;
                    frame.Navigate(new StatusAndDirection(this, connection));
                    break;
                case pages.equipmenttype:
                    Header.Visibility = Visibility.Visible;
                    frame.Navigate(new EquipmentType(this, connection));
                    break;
                case pages.modeltype:
                    Header.Visibility = Visibility.Visible;
                    frame.Navigate(new ModelType(this, connection));
                    break;
                case pages.programs:
                    Header.Visibility = Visibility.Visible;
                    frame.Navigate(new Programs(this, connection));
                    break;
                case pages.developers:
                    Header.Visibility = Visibility.Visible;
                    frame.Navigate(new Developers(this, connection));
                    break;
                case pages.inventory:
                    Header.Visibility = Visibility.Visible;
                    frame.Navigate(new Pages.Inventory(this, connection));
                    break;
                case pages.users:
                    Header.Visibility = Visibility.Visible;
                    frame.Navigate(new Pages.Users(this, connection));
                    break;
                case pages.network:
                    Header.Visibility = Visibility.Visible;
                    frame.Navigate(new Pages.Network(this, connection));
                    break;
                case pages.materials:
                    Header.Visibility = Visibility.Visible;
                    frame.Navigate(new Pages.Materials(this, connection));
                    break;
                case pages.charconsumables:
                    Header.Visibility = Visibility.Visible;
                    frame.Navigate(new Pages.CharConsumables(this, connection));
                    break;
                case pages.typematerials:
                    Header.Visibility = Visibility.Visible;
                    frame.Navigate(new Pages.MaterialsType(this, connection));
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenPage(pages.equip);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenPage(pages.audience);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenPage(pages.statusanddirection);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            OpenPage(pages.equipmenttype);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            OpenPage(pages.modeltype);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            OpenPage(pages.programs);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            OpenPage(pages.developers);
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            OpenPage(pages.inventory);
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            OpenPage(pages.users);
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            OpenPage(pages.network);
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            OpenPage(pages.materials);
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            OpenPage(pages.charconsumables);
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            OpenPage(pages.typematerials);
        }
    }
}
