using Inventory.Models;
using Inventory.Utilities;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using Word = Microsoft.Office.Interop.Word;

namespace Inventory.Pages
{
    /// <summary>
    /// Логика взаимодействия для Materials.xaml
    /// </summary>
    public partial class Materials : System.Windows.Controls.Page
    {
        public ObservableCollection<MaterialsModel> EquipmentList { get; set; }
        private readonly MainWindow mainWindow;
        private readonly DataBaseConnection connection;

        public Materials(MainWindow mainWindow, DataBaseConnection connection)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.connection = connection;
            EquipmentList = connection.LoadMaterialsData();
            EquipmentListView.DataContext = this;
            LoadRespUserComboBox();
            LoadTimeRespUserComboBox();
            LoadMaterialTypeComboBox();
        }

        private void LoadRespUserComboBox()
        {
            var list = connection.LoadUserData();
            ResponsibleUserComboBox.ItemsSource = list;
            ResponsibleUserComboBox.DisplayMemberPath = "Name";
        }

        private void LoadTimeRespUserComboBox()
        {
            var list = connection.LoadUserData();
            TemporaryUserComboBox.ItemsSource = list;
            TemporaryUserComboBox.DisplayMemberPath = "Name";
        }

        private void LoadMaterialTypeComboBox()
        {
            var list = connection.LoadMaterialTypesData();
            TypeComboBox.ItemsSource = list;
            TypeComboBox.DisplayMemberPath = "Name";
        }

        private void UploadPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files |*.jpg;*.png;*.bmp;*.gif";

            if (openFileDialog.ShowDialog() == true)
            {
                MaterialThumbnail.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                MaterialThumbnail == null || string.IsNullOrWhiteSpace(DescriptionTextBox.Text)
                || string.IsNullOrWhiteSpace(CountTextBox.Text) || ResponsibleUserComboBox.SelectedItem == null ||
                TemporaryUserComboBox.SelectedItem == null || TypeComboBox.SelectedItem == null
                || DateAdd.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля и выберите изображение.");
                return;
            }

            byte[] imageBytes = ConvertImageToBytes((BitmapImage)MaterialThumbnail.Source);

            MaterialsModel equipment = new MaterialsModel
            {
                Name = NameTextBox.Text,
                Description = DescriptionTextBox.Text,
                DateAdd =DateAdd.SelectedDate.Value,
                Image = imageBytes,
                Count = Convert.ToInt32(CountTextBox.Text),
                ResponsibleUser = (ResponsibleUserComboBox.SelectedItem as UserModel)?.Id.ToString(),
                TemporaryUser = (TemporaryUserComboBox.SelectedItem as UserModel)?.Id.ToString(),
                Type = (TypeComboBox.SelectedItem as TypeMaterialModel)?.Id.ToString()
            };

            connection.AddMaterial(equipment);
            
            EquipmentList.Add(equipment);

            ClearFields();

            UpdateTable();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentListView.SelectedItem != null)
            {
                MaterialsModel selectedEquipment = (MaterialsModel)EquipmentListView.SelectedItem;

                selectedEquipment.Name = NameTextBox.Text;
                selectedEquipment.Description = DescriptionTextBox.Text;
                selectedEquipment.DateAdd = DateAdd.SelectedDate.Value;
                selectedEquipment.Count = Convert.ToInt32(CountTextBox.Text);
                selectedEquipment.ResponsibleUser = (ResponsibleUserComboBox.SelectedItem as UserModel)?.Id.ToString();
                selectedEquipment.TemporaryUser = (TemporaryUserComboBox.SelectedItem as UserModel)?.Id.ToString();
                selectedEquipment.Type = (TypeComboBox.SelectedItem as TypeMaterialModel)?.Id.ToString();

                if (MaterialThumbnail.Source is BitmapImage bitmapImage)
                {
                    byte[] imageBytes = ConvertImageToBytes(bitmapImage);
                    selectedEquipment.Image = imageBytes;
                }

                connection.UpdateMaterial(selectedEquipment);

                UpdateTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для редактирования.");
            }
        }

        private void EquipmentListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EquipmentListView.SelectedItem != null)
            {
                MaterialsModel selectedEquipment = (MaterialsModel)EquipmentListView.SelectedItem;

                NameTextBox.Text = selectedEquipment.Name;
                DescriptionTextBox.Text = selectedEquipment.Description;
                DateAdd.SelectedDate = selectedEquipment.DateAdd;
                CountTextBox.Text = Convert.ToString(selectedEquipment.Count);
                ResponsibleUserComboBox.SelectedValue = selectedEquipment.Id;
                TemporaryUserComboBox.SelectedValue = selectedEquipment.Id;
                TypeComboBox.SelectedValue = selectedEquipment.Id;
                if (selectedEquipment.Image != null)
                {
                    MaterialThumbnail.Source = LoadImageFromBytes(selectedEquipment.Image);
                }
                else
                {
                    MaterialThumbnail.Source = null;
                }
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentListView.SelectedItem != null)
            {
                MaterialsModel selectedEquipment = (MaterialsModel)EquipmentListView.SelectedItem;
                connection.DeleteMaterial(selectedEquipment.Id);
                EquipmentList.Remove(selectedEquipment);
                ClearFields();
                UpdateTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите элемент для удаления.");
            }
        }

        private BitmapImage LoadImageFromBytes(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return null;
            }

            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream(imageBytes))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        private void ClearFields()
        {
            NameTextBox.Clear();
            DescriptionTextBox.Clear();
            CountTextBox.Clear();
            ResponsibleUserComboBox.SelectedIndex = -1;
            TemporaryUserComboBox.SelectedIndex = -1;
            TypeComboBox.SelectedIndex = -1;
            MaterialThumbnail.Source = null;
        }

        private void UpdateTable()
        {
            EquipmentListView.ItemsSource = null;
            EquipmentList.Clear();
            EquipmentList = connection.LoadMaterialsData();
            EquipmentListView.ItemsSource = EquipmentList;
        }

        private byte[] ConvertImageToBytes(ImageSource imageSource)
        {
            if (imageSource != null && imageSource is BitmapImage bitmapImage)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                    encoder.Save(stream);
                    return stream.ToArray();
                }
            }
            return null;
        }

        private void searc_TextChanged(object sender, TextChangedEventArgs e)
        {
            MaterialsModel materials = new MaterialsModel();
            SearchAndSort.SearchListView(EquipmentListView, materials, searc.Text);
        }

        private void MaterialsNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true; // Отменить ввод символа, если это не число
                MessageBox.Show("Инвентарный номер должен содержать только цифры.", "Ошибка ввода");
            }
        }
        public void GenerateEquipmentTransferAct(string city, string date, string organization, string employeeName, string equipmentDescription, string equipmentNumber, string equipmentCost, string returnDate)
        {
            Word.Application wordApp = new Word.Application();
            Word.Document doc = wordApp.Documents.Add();

            // Создаем заголовок документа
            Word.Paragraph title = doc.Paragraphs.Add();
            title.Range.Text = "АКТ приема-передачи оборудования на временное пользование";
            title.Range.Font.Bold = 1;
            title.Range.Font.Size = 14;
            title.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            title.Range.InsertParagraphAfter();

            // Заполняем информацию в документе
            Word.Paragraph actInfo = doc.Paragraphs.Add();
            actInfo.Range.Text = $"г. {city} {date}";
            actInfo.Range.InsertParagraphAfter();
            actInfo.Range.Text = organization;
            actInfo.Range.InsertParagraphAfter();
            actInfo.Range.Text = $"в целях обеспечением необходимым оборудованием для исполнения должностных обязанностей передаёт сотруднику {employeeName}, а сотрудник принимает от учебного учреждения следующее оборудование:";
            actInfo.Range.InsertParagraphAfter();
            actInfo.Range.Text = $"{equipmentDescription},Серийный номер {equipmentNumber}, стоимостью {equipmentCost} руб.";
            actInfo.Range.InsertParagraphAfter();
            actInfo.Range.Text = $"{employeeName}  Подпись    {date}";
            actInfo.Range.InsertParagraphAfter();
            string filePath = System.IO.Path.Combine(@"C:\Users\leim\Desktop\invent\Inventory\Inventory\", "приема-передачи расходных материалов.docx");
            doc.SaveAs2(filePath);
            doc.Close();
            wordApp.Quit();

            // Очищаем ресурсы
            System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
            MessageBox.Show("Успешно!");

            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
                if (ResponsibleUserComboBox.SelectedValue != null)
                {
                    GenerateEquipmentTransferAct("Пермь", "02.11.2023", "КГАПОУ Пермский Авиационный техникум им. А.Д. Швецова", ResponsibleUserComboBox.Text.ToString(), NameTextBox.Text, CountTextBox.Text,TypeComboBox.Text, "05.11.2023");
                }
                else
                {
                    MessageBox.Show("Выберите ответ.пользователя в ComboBox.");
                }
        }
    }
}
