using Inventory.Models;
using Inventory.Utilities;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Word = Microsoft.Office.Interop.Word;

namespace Inventory.Pages
{
    /// <summary>
    /// Логика взаимодействия для Equipment.xaml
    /// </summary>
    public partial class Equipment : System.Windows.Controls.Page
    {
        public ObservableCollection<EquipmentTypeModel> EquipmentTypeList { get; set; }
        public ObservableCollection<EquipmentModel> EquipmentList { get; set; }
        public ObservableCollection<HistoryAud> EquipmentListHistAud { get; set; }
        public ObservableCollection<HistoryUser> EquipmentListHistUsr { get; set; }
        public Audience audience1 { get; set; }


        private readonly DataBaseConnection connection;
        private readonly MainWindow mainWindow;
        public EquipmentModel equipment = new EquipmentModel();

        public Equipment(MainWindow mainWindow, DataBaseConnection connection)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.connection = connection;
            EquipmentList = connection.LoadEquipmentData();
            EquipmentListView.ItemsSource = EquipmentList;
            LoadAuditoriumComboBox();
            LoadRespUserComboBox();
            LoadTimeRespUserComboBox();
            LoadDirectionComboBox();
            LoadStatusComboBox();
            LoadModelTypeComboBox();

            // Инициализация коллекций для истории
            EquipmentListHistAud = new ObservableCollection<HistoryAud>();
            EquipmentListHistUsr = new ObservableCollection<HistoryUser>();
        }

        private void LoadAuditoriumComboBox()
        {
            var list = connection.LoadAuditoriumData();
            AuditoriumComboBox.ItemsSource = list;
            AuditoriumComboBox.DisplayMemberPath = "Name";
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

        private void LoadDirectionComboBox()
        {
            var list = connection.LoadDirectionData();
            EquipmentDirectionComboBox.ItemsSource = list;
            EquipmentDirectionComboBox.DisplayMemberPath = "Name";
        }

        private void LoadStatusComboBox()
        {
            var list = connection.LoadStatusData();
            EquipmentStatusComboBox.ItemsSource = list;
            EquipmentStatusComboBox.DisplayMemberPath = "Name";
        }

        private void LoadModelTypeComboBox()
        {
            var list = connection.LoadModelTypeData();
            EquipmentModelComboBox.ItemsSource = list;
            EquipmentModelComboBox.DisplayMemberPath = "Name";
        }

        private void UploadPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files |*.jpg;*.png;*.bmp;*.gif";

            if (openFileDialog.ShowDialog() == true)
            {
                EquipmentThumbnail.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EquipmentNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(InventoryNumberTextBox.Text) ||
                AuditoriumComboBox.SelectedItem == null ||
                ResponsibleUserComboBox.SelectedItem == null ||
                TemporaryUserComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(EquipmentCostTextBox.Text) ||
                EquipmentDirectionComboBox.SelectedItem == null ||
                EquipmentStatusComboBox.SelectedItem == null ||
                EquipmentModelComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(EquipmentCommentTextBox.Text) ||
                EquipmentThumbnail == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля и выберите изображение.");
                return;
            }

            byte[] imageBytes = ConvertImageToBytes((BitmapImage)EquipmentThumbnail.Source);

            EquipmentModel equipment = new EquipmentModel
            {
                EquipmentName = EquipmentNameTextBox.Text,
                EquipmentImage = imageBytes,
                InventoryNumber = InventoryNumberTextBox.Text,
                Auditorium = (AuditoriumComboBox.SelectedItem as AuditoriumModel)?.Id.ToString(),
                ResponsibleUser = (ResponsibleUserComboBox.SelectedItem as UserModel)?.Id.ToString(),
                TemporaryUser = (TemporaryUserComboBox.SelectedItem as UserModel)?.Id.ToString(),
                EquipmentCost = decimal.Parse(EquipmentCostTextBox.Text),
                EquipmentDirection = (EquipmentDirectionComboBox.SelectedItem as DirectionModel)?.Id.ToString(),
                EquipmentStatus = (EquipmentStatusComboBox.SelectedItem as StatusModel)?.Id.ToString(),
                EquipmentType = (EquipmentModelComboBox.SelectedItem as EquipmentTypeModel)?.Id.ToString(),
                EquipmentComment = EquipmentCommentTextBox.Text
            };

            EquipmentList.Add(equipment);

            connection.AddEquipment(equipment);

            ClearFields();

            UpdateTable();
            UpdateTableHistUsr();
            UpdateTableHistAud();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentListView.SelectedItem != null)
            {
                EquipmentModel selectedEquipment = (EquipmentModel)EquipmentListView.SelectedItem;

                // Создайте историческую запись и добавьте ее в соответствующую коллекцию
                HistoryAud historyAudm = new HistoryAud
                {
                    EquipmentName = selectedEquipment.EquipmentName,
                    InventoryNumber = selectedEquipment.InventoryNumber,
                    Auditorium = selectedEquipment.Auditorium
                };
                historyAud.ItemsSource=(historyAudm.ToString());

                HistoryUser userhist = new HistoryUser
                {
                    InventoryNumber = selectedEquipment.InventoryNumber,
                    ResponsibleUser = selectedEquipment.ResponsibleUser,
                    EquipmentComment = selectedEquipment.EquipmentComment
                };
                historyUser.ItemsSource =(userhist.ToString());

                // Далее обновите исходную коллекцию данных (EquipmentList) и элемент управления (EquipmentListView) снова.
                selectedEquipment.EquipmentName = EquipmentNameTextBox.Text;
                selectedEquipment.InventoryNumber = InventoryNumberTextBox.Text;
                selectedEquipment.Auditorium = (AuditoriumComboBox.SelectedItem as AuditoriumModel)?.Id.ToString();
                selectedEquipment.ResponsibleUser = (ResponsibleUserComboBox.SelectedItem as UserModel)?.Id.ToString();
                selectedEquipment.TemporaryUser = (TemporaryUserComboBox.SelectedItem as UserModel)?.Id.ToString();

                historyAud.Items.Refresh();
                historyUser.Items.Refresh();

                decimal equipmentCost;
                if (decimal.TryParse(EquipmentCostTextBox.Text, out equipmentCost))
                {
                    selectedEquipment.EquipmentCost = equipmentCost;
                }
                else
                {
                    MessageBox.Show("Введите корректное значение стоимости оборудования.");
                    return;
                }

                selectedEquipment.EquipmentDirection = (EquipmentDirectionComboBox.SelectedItem as DirectionModel)?.Id.ToString();
                selectedEquipment.EquipmentStatus = (EquipmentStatusComboBox.SelectedItem as StatusModel)?.Id.ToString();
                selectedEquipment.EquipmentType = (EquipmentModelComboBox.SelectedItem as EquipmentTypeModel)?.Id.ToString();
                selectedEquipment.EquipmentComment = EquipmentCommentTextBox.Text;

                if (EquipmentThumbnail.Source is BitmapImage bitmapImage)
                {
                    byte[] imageBytes = ConvertImageToBytes(bitmapImage);
                    selectedEquipment.EquipmentImage = imageBytes;
                }

                connection.UpdateEquipment(selectedEquipment);

                // Обновите элемент управления
                UpdateTable();
                UpdateTableHistUsr();
                UpdateTableHistAud();
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
                EquipmentModel selectedEquipment = (EquipmentModel)EquipmentListView.SelectedItem;
                EquipmentNameTextBox.Text = selectedEquipment.EquipmentName;
                InventoryNumberTextBox.Text = selectedEquipment.InventoryNumber;
                AuditoriumComboBox.SelectedValue = selectedEquipment.Id;
                ResponsibleUserComboBox.SelectedValue = selectedEquipment.Id;
                TemporaryUserComboBox.SelectedValue = selectedEquipment.Id;
                EquipmentCostTextBox.Text = selectedEquipment.EquipmentCost.ToString();
                EquipmentDirectionComboBox.SelectedValue = selectedEquipment.Id;
                EquipmentStatusComboBox.SelectedValue = selectedEquipment.Id;
                EquipmentModelComboBox.SelectedValue = selectedEquipment.Id;
                EquipmentCommentTextBox.Text = selectedEquipment.EquipmentComment;

                if (selectedEquipment.EquipmentImage != null)
                {
                    EquipmentThumbnail.Source = LoadImageFromBytes(selectedEquipment.EquipmentImage);
                }
                else
                {
                    EquipmentThumbnail.Source = null;
                }
            }
        }


        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentListView.SelectedItem != null)
            {
                EquipmentModel selectedEquipment = (EquipmentModel)EquipmentListView.SelectedItem;
                Audience audience = new Audience(mainWindow, connection);
                HistoryAud historyAudm = new HistoryAud
                {
                    EquipmentName = selectedEquipment.EquipmentName,
                    InventoryNumber = selectedEquipment.InventoryNumber,
                    Auditorium = selectedEquipment.Auditorium
                };
                EquipmentListHistAud.Add(historyAudm);

                HistoryUser userhist = new HistoryUser
                {
                    InventoryNumber = selectedEquipment.InventoryNumber,
                    ResponsibleUser = selectedEquipment.ResponsibleUser,
                    EquipmentComment = selectedEquipment.EquipmentComment
                };
                EquipmentListHistUsr.Add(userhist);
                EquipmentListView.Items.Refresh();
                EquipmentList.Remove(selectedEquipment);
                connection.DeleteEquipment(selectedEquipment.Id);
                ClearFields();
                UpdateTable();
                UpdateTableHistUsr();
                UpdateTableHistAud();
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
            EquipmentNameTextBox.Clear();
            InventoryNumberTextBox.Clear();
            AuditoriumComboBox.SelectedIndex = -1;
            ResponsibleUserComboBox.SelectedIndex = -1;
            TemporaryUserComboBox.SelectedIndex = -1;
            EquipmentCostTextBox.Clear();
            EquipmentDirectionComboBox.SelectedIndex = -1;
            EquipmentStatusComboBox.SelectedIndex = -1;
            EquipmentModelComboBox.SelectedIndex = -1;
            EquipmentCommentTextBox.Clear();
            EquipmentThumbnail.Source = null; // Сбрасываем изображение
        }

        public void UpdateTable()
        {
            EquipmentListView.ItemsSource = null;
            EquipmentList.Clear();
            EquipmentList = connection.LoadEquipmentData();
            EquipmentListView.ItemsSource = EquipmentList;
        }
        public void UpdateTableHistUsr()
        {
            historyUser.ItemsSource = null;
            EquipmentListHistUsr.Clear();
            EquipmentListHistUsr = connection.LoadHistoryUser();
            historyUser.ItemsSource = EquipmentListHistUsr;
        }
        public void UpdateTableHistAud()
        {
            historyAud.ItemsSource = null;
            EquipmentListHistAud.Clear();
            EquipmentListHistAud = connection.LoadhistoryAuds();
            historyAud.ItemsSource = EquipmentListHistAud;
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

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EquipmentModel em = new EquipmentModel();
            string searchText = SearchTextBox.Text;
            SearchAndSort.SearchListView(EquipmentListView, em, searchText);
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)SortComboBox.SelectedItem;
            string selectedOption = selectedItem.Content.ToString();

            if (selectedOption == "По возрастанию")
            {
                SearchAndSort.SortListView(EquipmentListView, true);
            }
            else if (selectedOption == "По убыванию")
            {
                SearchAndSort.SortListView(EquipmentListView, false);
            }
        }

        private void InventoryNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true; // Отменить ввод символа, если это не число
                MessageBox.Show("Инвентарный номер должен содержать только цифры.", "Ошибка ввода");
            }
        }

        private void EquipmentCostTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!decimal.TryParse(EquipmentCostTextBox.Text + e.Text, out _))
            {
                e.Handled = true; // Отменить ввод символа, если это не числовое значение
                MessageBox.Show("Стоимость должна содержать только числовые значения.", "Ошибка ввода");
            }
        }

        private void InventoryNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
                OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Выберите excel файл для импорта";
                openFileDialog.ShowDialog();
                if (!string.IsNullOrWhiteSpace(openFileDialog.FileName))
                {
                    ImportExcelData(openFileDialog.FileName);
                }
        }
        public void ImportExcelData(string filePath)
        {
            DataBaseConnection db = new DataBaseConnection();
            List<EquipmentModel> equipmentModels = new List<EquipmentModel>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                // Первая страница с входящими письмами
                ExcelWorksheet equipWorksheet = package.Workbook.Worksheets[0];
                int equipeRowCount = equipWorksheet.Dimension.Rows;
                int equipColCount = equipWorksheet.Dimension.Columns;

                Dictionary<string, int> equipColumnMap = new Dictionary<string, int>();

                // Определение соответствия заголовков столбцов и их индексов (входящие)
                for (int col = 1; col <= equipColCount; col++)
                {
                    var columnHeader = equipWorksheet.Cells[1, col].Value?.ToString();
                    if (!string.IsNullOrWhiteSpace(columnHeader))
                        equipColumnMap[columnHeader] = col;
                }

                // Проходим по каждой строке, начиная со второй строки (первая строка - заголовки)
                for (int row = 2; row <= equipeRowCount; row++)
                {
                    var equip = new EquipmentModel();
                    bool hasData = false;  // Флаг, указывающий наличие данных в текущей строке

                    foreach (var entry in equipColumnMap)
                    {
                        string columnHeader = entry.Key;
                        int col = entry.Value;
                        var cellValue = equipWorksheet.Cells[row, col].Value?.ToString();

                        //// Проверяем, что значение не пустое
                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            hasData = true;

                            switch (columnHeader.ToLower().Trim())
                            {
                                case "название оборудования":
                                    equip.EquipmentName = cellValue;
                                    break;
                                case "фотография оборудования":
                                    equip.EquipmentImage = System.Text.Encoding.UTF8.GetBytes(cellValue);
                                    break;
                                case "инвентарный номер оборудования":
                                    equip.InventoryNumber = cellValue;
                                    break;
                                case "аудитория":
                                    equip.Auditorium = cellValue;
                                    break;
                                case "ответственный пользователь ":
                                    equip.ResponsibleUser = cellValue;
                                    break;
                                case "временно-ответственный пользователь":
                                    equip.TemporaryUser = cellValue;
                                    break;
                                case "стоимость оборудования":
                                    equip.EquipmentCost = Convert.ToDecimal(cellValue);
                                    break;
                                case "направление оборудования;":
                                    equip.EquipmentDirection = cellValue;
                                    break;
                                case "статус оборудования":
                                    equip.EquipmentStatus = cellValue;
                                    break;
                                case "модель оборудования":
                                    equip.EquipmentType = cellValue;
                                    break;
                                case "комментарий":
                                    equip.EquipmentComment = cellValue;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    //// Добавляем shortLetter в список только если есть хотя бы одно непустое поле
                    if (hasData)
                    {
                        equipmentModels.Add(equip);
                        db.AddEquipment(equip);
                        UpdateTable();
                        UpdateTableHistUsr();
                        UpdateTableHistAud();
                    }
                }
            }
        }
        public void GenerateEquipmentTransferAct(string city, string date, string organization, string employeeName, string equipmentDescription, string equipmentNumber, decimal equipmentCost, string returnDate)
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
            actInfo.Range.InsertAlignmentTab(0);
            actInfo.Range.InsertParagraphAfter();
            actInfo.Range.Text = organization;
            actInfo.Range.InsertParagraphAfter();
            actInfo.Range.Text = $"в целях обеспечением необходимым оборудованием для исполнения должностных обязанностей nпередаёт сотруднику {employeeName}, а сотрудник принимает от учебного учреждения следующее оборудование:";
            actInfo.Range.InsertParagraphAfter();
            actInfo.Range.Text = $"{equipmentDescription},Серийный номер {equipmentNumber}, стоимостью {equipmentCost} руб.";
            actInfo.Range.InsertParagraphAfter();
            actInfo.Range.Text = $"По окончанию должностных работ {returnDate}, работник обязуется вернуть полученное оборудование.";
            actInfo.Range.InsertParagraphAfter();
            actInfo.Range.Text = $"{employeeName}  Подпись    {date}";
            actInfo.Range.InsertParagraphAfter();

            string filePath = Path.Combine(@"C:\Users\leim\Desktop\invent\Inventory\Inventory\", "приема-передачи оборудования на временное пользование.docx");
            doc.SaveAs2(filePath);
            doc.Close();
            wordApp.Quit();

            // Очищаем ресурсы
            System.Runtime.InteropServices.Marshal.ReleaseComObject(doc);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);

            Word.Application wordApp1 = new Word.Application();
            Word.Document doc1 = wordApp1.Documents.Add();

            // Создаем заголовок документа
            Word.Paragraph title1 = doc1.Paragraphs.Add();
            title1.Range.Text = "АКТ приема-передачи оборудования на временное пользование";
            title1.Range.Font.Bold = 1;
            title1.Range.Font.Size = 14;
            title1.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            title1.Range.InsertParagraphAfter();

            // Заполняем информацию в документе
            Word.Paragraph actInfo1 = doc1.Paragraphs.Add();
            actInfo1.Range.Text = $"г. {city} {date}";
            actInfo1.Range.InsertParagraphAfter();
            actInfo1.Range.Text = organization;
            actInfo1.Range.InsertParagraphAfter();
            actInfo1.Range.Text = $"в целях обеспечением необходимым оборудованием для исполнения должностных обязанностей передаёт сотруднику {employeeName}, а сотрудник принимает от учебного учреждения следующее оборудование:";
            actInfo1.Range.InsertParagraphAfter();
            actInfo1.Range.Text = $"{equipmentDescription},Серийный номер {equipmentNumber}, стоимостью {equipmentCost} руб.";
            actInfo1.Range.InsertParagraphAfter();
            actInfo1.Range.Text = $"{employeeName}  Подпись    {date}";
            actInfo1.Range.InsertParagraphAfter();

            string filePath2 = System.IO.Path.Combine(@"C:\Users\leim\Desktop\invent\Inventory\Inventory\", "приема-передачи оборудования");
            doc1.SaveAs2(filePath2);
            doc1.Close();
            wordApp1.Quit();

            // Очищаем ресурсы
            System.Runtime.InteropServices.Marshal.ReleaseComObject(doc1);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp1);
            MessageBox.Show("Успешно!");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentListView.SelectedItem != null)
            {
                EquipmentModel selectedEquipment = (EquipmentModel)EquipmentListView.SelectedItem;
                EquipmentNameTextBox.Text = selectedEquipment.EquipmentName;
                InventoryNumberTextBox.Text = selectedEquipment.InventoryNumber;

                if (ResponsibleUserComboBox.SelectedValue != null)
                {
                    GenerateEquipmentTransferAct("Пермь", "02.11.2023", "КГАПОУ Пермский Авиационный техникум им. А.Д. Швецова", ResponsibleUserComboBox.Text.ToString(), EquipmentNameTextBox.Text, InventoryNumberTextBox.Text, Convert.ToDecimal(EquipmentCostTextBox.Text), "05.11.2023");
                }
                else
                {
                    MessageBox.Show("Выберите ответ.пользователя в ComboBox.");
                }
            }
        }

        private void ResponsibleUserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
