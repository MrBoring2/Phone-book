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
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Data.Entity;

using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Data.OleDb;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace DataBase
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private Application application;
        private Workbook workBook;
        private Worksheet worksheet;
        SpravochnikModel.DataEntities db;
        List<SpravochnikModel.Spravochnik> selectedList;
        List<SpravochnikModel.Spravochnik> filter= new List<SpravochnikModel.Spravochnik>();

        int page = 0;
        int selectLimitRows=10;
        int totalRows;
        public MainWindow()
        {
            InitializeComponent();

            db = new SpravochnikModel.DataEntities(); 
            selectedList = db.Spravochnik.ToList();

            db.Spravochnik.Load();
            totalRows = db.Spravochnik.Count();
            PageSort(db.Spravochnik.ToList());
            updateCountOfRows();
            //connectionString = ConfigurationManager.ConnectionStrings["HomeConnection"].ConnectionString;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            page = 0;

           

            if(filter.Count > 0)
            {
                selectedList = filter;
            }
            else
            {
                selectedList = db.Spravochnik.ToList();
            }

            if (listView.SelectedIndex != listView.Items.Count - 1)
            {
                selectLimitRows = Convert.ToInt32((listView.SelectedItem as TextBlock).Text);
                PageSort(selectedList);

            }
            else
            {
                page = 0;
                selectLimitRows = totalRows;
                PageSort(selectedList);                         
            }
            searchText.Text = "";
            totalRows = selectedList.Count;
        }
      
        private void nextPage_Click(object sender, RoutedEventArgs e)
        {
            if (filter.Count > 0)
            {
                selectedList = filter;
            }


            if (data.Items.Count - 1 < selectLimitRows || data.Items.Count - 1 == getCountOfLines() || pagOf.Text.Equals(pageTo.Text)) return;
            
            page++;
            PageSort(selectedList);
        }

        private void previousPage_Click(object sender, RoutedEventArgs e)
        {
            if (filter.Count > 0)
            {
                selectedList = filter;
            }
            else
            {
                selectedList = db.Spravochnik.ToList();
            }


            if (page == 0) return;

            page--;
            PageSort(selectedList);
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            UserAdd add = new UserAdd();
            add.ShowDialog();
            if (add.DialogResult.Value == false)
            {
                MessageBox.Show("Операция отменения", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SpravochnikModel.Spravochnik human = new SpravochnikModel.Spravochnik();
            human.FamilyName = add.getFamilyName;
            human.Name = add.getName;
            human.Otchestvo = add.getOthcestvo;
            human.Telephone = add.getTelephone;
            human.Photo = add.getPhotoImage;
            human.Category = add.getCategory;

            
            db.Spravochnik.Add(human);
            if (filter.Count > 0)
            {
                selectedList = filter;
            }
            else
            {
                selectedList = db.Spravochnik.ToList();
            }
            selectedList.Add(human);
            await db.SaveChangesAsync();


            totalRows = selectedList.Count;

            PageSort(selectedList);
            MessageBox.Show("Запись добавлена!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            if ((data.SelectedItem as SpravochnikModel.Spravochnik) !=null)
            {
                if (data.SelectedItems.Count > 0)
                {
                    for (int i = 0; i < data.SelectedItems.Count; i++)
                    {
                        SpravochnikModel.Spravochnik human = data.SelectedItems[i] as SpravochnikModel.Spravochnik;
                        if (human != null)
                        {
                            if (filter.Count > 0)
                            {
                                selectedList = filter;
                            }
                            else
                            {
                                selectedList = db.Spravochnik.ToList();
                            }
                            db.Spravochnik.Remove(human);
                            selectedList.Remove(human);
                            await db.SaveChangesAsync();
                        }
                    }
                    
                    totalRows = getCountOfLines();

                    PageSort(selectedList);
                    updateCountOfRows();
                    MessageBox.Show("Запись(-и) удалена(-ы)!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }
            else
            {
                MessageBox.Show("Выберите запись!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }


        }

        void PageSort(List<SpravochnikModel.Spravochnik> list)
        {

            if (listView.SelectedIndex == listView.Items.Count - 1)
            {
                selectLimitRows = totalRows;
            }


            int skip = page * selectLimitRows;

            var result = (from u in list
                orderby u.id
                select u)    
                .Skip(skip)
                .Take(selectLimitRows)
                .ToList();
            data.ItemsSource = result;
            //data.Items.Refresh();
            updatePages();
            updateCountOfRows();
        }

        private int getCountOfLines()
        {
            return selectedList.Count();           
        }

        private void updatePages()
        {
            pagOf.Text = (page + 1).ToString();
            if (getCountOfLines() % selectLimitRows != 0)
            {
                pageTo.Text = (getCountOfLines() / selectLimitRows + 1).ToString();
            }
            else
            {
                pageTo.Text = (getCountOfLines() / selectLimitRows).ToString();

            }

            if(getCountOfLines() == 0)
            {
                pageTo.Text = "1";
            }
        }

        public void updateCountOfRows()
        {
            countOfRows.Text = selectedList.Count.ToString();
            showsRows.Text = (data.Items.Count - 1).ToString();
        }

        private void Refactor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = (data.Items[data.SelectedIndex] as SpravochnikModel.Spravochnik).id;
                SpravochnikModel.Spravochnik human = db.Spravochnik.Find(id);
                UserRefactor userRefactor = new UserRefactor(human.id, human.Name, human.FamilyName, human.Otchestvo, human.Telephone, human.Category, human.Photo);
                if (userRefactor.ShowDialog() == false)
                {
                    MessageBox.Show("Операция отменена", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                human.Name = userRefactor.getName;
                human.FamilyName = userRefactor.getFamilyName;
                human.Otchestvo = userRefactor.getOthcestvo;
                human.Telephone = userRefactor.getTelephone;
                human.Category = userRefactor.getCategory;
                human.Photo = userRefactor.getPhoto;

                db.SaveChangesAsync();
                data.Items.Refresh();
                MessageBox.Show("Изменения приняты!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(System.ArgumentOutOfRangeException)
            {
                MessageBox.Show("Выберите запись!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            catch (System.NullReferenceException)
            {
                MessageBox.Show("Выберите заполенную запись!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           

            int count = 0;
            for (int i = 0; i < checkBoxList.Items.Count; i++)
            {
                if ((checkBoxList.Items[i] as System.Windows.Controls.CheckBox).IsChecked.Value == false)
                {
                    count++;
                }
            }

            if (count != checkBoxList.Items.Count)
            {
                var result = filter.Where(x => x.FamilyName.ToLower().Contains(searchText.Text.ToLower()));
                selectedList = result.ToList();
                updatePages();

                if (Convert.ToInt32(pagOf.Text) > Convert.ToInt32(pageTo.Text))
                {
                    pagOf.Text = pageTo.Text;
                    page = Convert.ToInt32(pageTo.Text) - 1;
                }
           

            }
            else
            {
                if (searchText.Text != "")
                {

                    var result = db.Spravochnik.Where(x => x.FamilyName.Contains(searchText.Text)).ToList();
                    selectedList = result.ToList();

                    updatePages();

                    if (Convert.ToInt32(pagOf.Text) > Convert.ToInt32(pageTo.Text))
                    {
                        pagOf.Text = pageTo.Text;
                        page = Convert.ToInt32(pageTo.Text) - 1;
                    }
                }
                else
                {
                    selectedList = db.Spravochnik.ToList();
                    updatePages();

                    if (Convert.ToInt32(pagOf.Text) > Convert.ToInt32(pageTo.Text))
                    {
    
                        pagOf.Text = pageTo.Text;
                        page = Convert.ToInt32(pageTo.Text) - 1;

                    }
                }
            }

            PageSort(selectedList);
          

        }

        private void searchClear_Click(object sender, RoutedEventArgs e)
        {
            searchText.Text="";
        }


        void CategorySort(bool isChecked, string category)
        {
            


            if (isChecked)
            {
                filter.AddRange(db.Spravochnik.OrderBy(key => "id").Where(x => x.Category.Contains(category)));

            }
            else if(!isChecked)
            {
                filter.RemoveAll(x => x.Category.Contains(category));
     
            }

         

            if (filter.Count > 0)
            {
                if (!String.IsNullOrEmpty(searchText.Text))
                {
                    data.ItemsSource = filter.Where(x=>x.FamilyName.Contains(searchText.Text)).OrderBy(key => "id");
                }

            }

            selectedList = filter;

            totalRows = selectedList.Count;

            updatePages();

            if (Convert.ToInt32(pagOf.Text) > Convert.ToInt32(pageTo.Text))
            {
          
                pagOf.Text = pageTo.Text;
                page = Convert.ToInt32(pageTo.Text) - 1;

            }

           

            for (int i = 0, count = 0; i < checkBoxList.Items.Count; i++)
            {
                if ((checkBoxList.Items[i] as System.Windows.Controls.CheckBox).IsChecked.Value == false)
                {
                    count++;
                }
                if (count == checkBoxList.Items.Count)
                {
                    page = 0;
                    selectedList = db.Spravochnik.ToList();
                    totalRows = db.Spravochnik.Count();
                    PageSort(selectedList);
                }
            }
            //if (searchText.Text != "")
            //{

            //    var result = filter.Where(x => x.FamilyName.Contains(searchText.Text)).ToList();
            //    selectedList = result.ToList();

            //    updatePages();

            //    if (Convert.ToInt32(pagOf.Text) > Convert.ToInt32(pageTo.Text))
            //    {
            //        pagOf.Text = pageTo.Text;
            //        page = Convert.ToInt32(pageTo.Text) - 1;
            //    }
            //}
            PageSort(selectedList);


        }

        private void friend_Click(object sender, RoutedEventArgs e)
        {
            CategorySort(friend.IsChecked.Value, friend.Content.ToString());
        }

        private void collega_Click(object sender, RoutedEventArgs e)
        {
            CategorySort(collega.IsChecked.Value, collega.Content.ToString());
        }

        private void customer_Click(object sender, RoutedEventArgs e)
        {
            CategorySort(customer.IsChecked.Value, customer.Content.ToString());
        }

        private async void exoprt_Click(object sender, RoutedEventArgs e)
        {
            Export();
            MessageBox.Show("Экспорт завершён!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Export()
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Worksheet sheet1 = (Worksheet)workbook.Sheets[1];

            for (int j = 0; j < data.Columns.Count; j++)
            {
                Range myRange = (Range)sheet1.Cells[1, j + 1];
                sheet1.Cells[1, j + 1].Font.Bold = true;
                sheet1.Columns[j + 1].ColumnWidth = 15;
                myRange.Value2 = data.Columns[j].Header;
            }
            for (int i = 0, row = 1; i < data.Items.Count; i++, row++)
            {
                if ((data.Items[i] as SpravochnikModel.Spravochnik) != null)
                {
                    sheet1.Cells[row + 1, 1].Value = (data.Items[i] as SpravochnikModel.Spravochnik).id.ToString();
                    sheet1.Cells[row + 1, 2].Value = (data.Items[i] as SpravochnikModel.Spravochnik).FamilyName.ToString();
                    sheet1.Cells[row + 1, 3].Value = (data.Items[i] as SpravochnikModel.Spravochnik).Name.ToString();
                    sheet1.Cells[row + 1, 4].Value = (data.Items[i] as SpravochnikModel.Spravochnik).Otchestvo.ToString();
                    sheet1.Cells[row + 1, 5].Value = (data.Items[i] as SpravochnikModel.Spravochnik).Telephone.ToString();
                    if ((data.Items[i] as SpravochnikModel.Spravochnik).Photo != null)
                        sheet1.Cells[row + 1, 6].Value = ((byte[])(data.Items[i] as SpravochnikModel.Spravochnik).Photo).ToString();
                    else
                        sheet1.Cells[row + 1, 6].Value = "null";
                    if ((data.Items[i] as SpravochnikModel.Spravochnik).Category != null)
                        sheet1.Cells[row + 1, 7].Value = (data.Items[i] as SpravochnikModel.Spravochnik).Category.ToString();
                    else
                        sheet1.Cells[row + 1, 7].Value = "null";
                }
            }
        }
    }
    
}
