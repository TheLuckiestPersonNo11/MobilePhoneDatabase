using ExcelDataReader;
using MainProject27.Model;
using MainProject27.MVVM;
using Microsoft.Win32;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.ComponentModel;
using System.IO;

namespace MainProject27.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private BindingList<Item> Items { get; set; }
        public BindingList<Item> FilteredItems { get; set; }


        private string filterText = "";
        public string FilterText 
        {
            get { return filterText; }
            set
            {
                filterText = value;
                FilterItems();
            }
        }

      
        

        public RelayCommand AddCommand => new RelayCommand(execute => AddItem());
        public RelayCommand DeleteCommand => new RelayCommand(execute => DeleteItem(), canExecute => SelectedItem != null);
        public RelayCommand OpenFileCommand => new RelayCommand(execute => OpenFileExcel());
        public RelayCommand SaveFileCommand => new RelayCommand(execute => SaveFileExcel());



        public MainWindowViewModel()
        {
            Items = new BindingList<Item>();
            FilteredItems = new BindingList<Item>();
            Items.ListChanged += ItemsOnListChanged;

        }

        private void ItemsOnListChanged(object sender, ListChangedEventArgs e)
        {
            FilterItems();
        }

        private Item selectedItem;

        public Item SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }
        public void FilterItems()
        {
            if(filterText == string.Empty)
            {
                FilteredItems.Clear();

                foreach (Item i in Items)
                {
                    FilteredItems.Add(i);
                }
            }
            else
            {
                string Model = "";
                
                FilteredItems.Clear();
                foreach (Item item in Items)
                {
                    Model = item.Model.ToString();
                    if (Model.StartsWith(filterText))
                    {
                        FilteredItems.Add(item);
                    }
                      
                }
                
            }
          
        }
        private void AddItem()
        {
            Items.Add(new Item
            {
                Identifier=3020000000000000,
                Department="(введіть відділ поліції)",
                Day=1,
                Month=1,
                Year=2000,
                Model="(оберіть модель)",
                IMEI=1000000000000000,
                SIM=1000
            });
          
        }

        private void DeleteItem()
        {
            Items.Remove(SelectedItem); 
        }

        private void OpenFileExcel()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files (*.xls;*.xlsx;*.xlsm)|*.xls;*.xlsx;*.xlsm";
            openFileDialog.InitialDirectory = System.Environment.CurrentDirectory;
            bool? dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == true)
            {
                string path = openFileDialog.FileName;
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = File.Open(path,FileMode.Open,FileAccess.ReadWrite))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var data = reader.AsDataSet();
                        Items.Clear();
                        for (int i = 1; i < data.Tables[0].Rows.Count; i++)
                        {
                            
                            Items.Add(new Item
                            {
                                Identifier = long.Parse(data.Tables[0].Rows[i][0].ToString()),
                                Department = data.Tables[0].Rows[i][1].ToString(),
                                Day = int.Parse(data.Tables[0].Rows[i][2].ToString()),
                                Month = int.Parse(data.Tables[0].Rows[i][3].ToString()),
                                Year = int.Parse(data.Tables[0].Rows[i][4].ToString()),
                                Model = data.Tables[0].Rows[i][5].ToString(),
                                IMEI = long.Parse(data.Tables[0].Rows[i][6].ToString()),
                                SIM = int.Parse(data.Tables[0].Rows[i][7].ToString())
                            });
                        }
                    }
                }
            }
        }

        private void SaveFileExcel()
        {
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files (*.xls;*.xlsx;*.xlsm)|*.xls;*.xlsx;*.xlsm";
            saveFileDialog.InitialDirectory = System.Environment.CurrentDirectory;
            saveFileDialog.FileName = $"Dataset_{DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")}.xlsx";

            bool? dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult == true)
            {
                string path = saveFileDialog.FileName;
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet1 = workbook.CreateSheet("Sheet1");
                
                IFont font = workbook.CreateFont();
                font.FontName = "Calibri"; 
                font.IsBold = true;
                font.FontHeightInPoints = 11;
                font.Color = HSSFColor.Black.Index;
              
                XSSFCellStyle cellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                cellStyle.SetFont(font);

                string[] headers = { "Ідентифікатор", "Відділ поліції", "День загублення", "Місяць загублення", "Рік загублення", "Модель", "IMEI", "Номер SIM-картки" };
                
                IRow row = sheet1.CreateRow(0);
                row.RowStyle = cellStyle;
                for (int i = 0; i < headers.Length; i++)
                {
                    row.CreateCell(i).SetCellValue(headers[i]); 
                }

                int index = 1;
                foreach(Item item in Items)
                {
                    row = sheet1.CreateRow(index);
                    row.CreateCell(0).SetCellValue(item.Identifier.ToString());
                    row.CreateCell(1).SetCellValue(item.Department.ToString());
                    row.CreateCell(2).SetCellValue(item.Day.ToString());
                    row.CreateCell(3).SetCellValue(item.Month.ToString());
                    row.CreateCell(4).SetCellValue(item.Year.ToString());
                    row.CreateCell(5).SetCellValue(item.Model.ToString());
                    row.CreateCell(6).SetCellValue(item.IMEI.ToString());
                    row.CreateCell(7).SetCellValue(item.SIM.ToString());
                    index++;
                }

                for(int i = 0; i < headers.Length; i++)
                {
                    sheet1.AutoSizeColumn(i);
                }

                using (FileStream sw = File.Create(path))
                {
                    workbook.Write(sw);
                    FileInfo fileInfo = new FileInfo(saveFileDialog.FileName);
                    
                    System.Windows.MessageBox.Show($"Файл \"{fileInfo.Name}\" збережено успішно!");
                }
            }
        }
    }
}
