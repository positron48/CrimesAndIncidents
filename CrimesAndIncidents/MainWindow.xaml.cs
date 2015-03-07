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
using System.Data;
using System.Collections.ObjectModel;
using Microsoft.Office.Interop;

namespace CrimesAndIncidents
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqliteWorker sqlWorker;
        ObservableCollection<Crime> crimes;
        CollectionViewSource coll;

        MilitaryUnitList mList;

        public MainWindow()
        {
            InitializeComponent();

            dpLeft.Text = "01.01." + DateTime.Now.ToString("yyyy");
            dpRight.Text = DateTime.Now.ToString("dd.MM.yyyy");

            crimes = new ObservableCollection<Crime>();
            try
            {
                sqlWorker = new SqliteWorker("CrimesAndIncidents");
                

                //изменение формата дат, оставлю, вдруг понадобится
                //for (int i = 0; i < crimes.Count; i++)
                //{
                //    sqlWorker.executeQuery("UPDATE Crime SET " +
                //        "dateRegistration = " + (crimes[i].DateRegistration == "" ? "NULL, " : "'" +crimes[i].DateRegistration + "', ") +
                //        "dateInstitution = " + (crimes[i].DateInstitution == "" ? "NULL, " : "'" + crimes[i].DateInstitution + "', ") +
                //        "dateCommit = " + (crimes[i].DateCommit == "" ? "NULL, " : "'" + crimes[i].DateCommit + "' ") +
                //        "WHERE idCrime = " + crimes[i].Id);
                //}
                coll = new CollectionViewSource();


                btnOk_Click_1(null, null);

                crimesDataGrid.CanUserAddRows = false;

                rowFilter.Height = new GridLength(0);

                cbRegistred.SelectedIndex = 1;

                mList = new MilitaryUnitList(DataWorker.getMilitaryUnitList(sqlWorker.selectData("SELECT * FROM MilitaryUnit")));
                mList.values.Add(new MilitaryUnit(-1,"","все","все","",0,1));
                
                cbFilterMilitaryUnit.ItemsSource = mList.values;
                cbFilterMilitaryUnit.SelectedIndex = mList.values.Count-1;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Во время загрузки приложения возникли неполадки:\n" + ex.Message);
            }
        }

        void View_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            tbCountAll.Text = crimes.Count.ToString();
            tbCountFilter.Text = crimesDataGrid.Items.Count.ToString();
            tbCountSelected.Text = crimesDataGrid.SelectedItems.Count.ToString();
        }

        private void MenuExit_Click_1(object sender, RoutedEventArgs e)
        {
            //запрос на сохранение изменений (если вносились)
            this.Close();
        }
        
        private void btnAddCrimeOrIncidents_Click_1(object sender, RoutedEventArgs e)
        {
            Crime c = AddCrime.gtNewCrime(sqlWorker);
            if(c!=null)
                crimes.Add(c);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            String rusName = (sender as MenuItem).Header.ToString();
            String tableName;
            switch (rusName)
            {
                case "Должность":
                    tableName = "Post";
                    break;
                case "Образование":
                    tableName = "Education";
                    break;
                case "Категория":
                    tableName = "Category";
                    break;
                case "Кем призван":
                    tableName = "Draft";
                    break;
                case "Кем возбуждено УД":
                    tableName = "Organ";
                    break;
                case "Семейное положение":
                    tableName = "FamilyStatus";
                    break;
                default:
                    tableName = rusName;
                    break;
            }

            DBList dblist = new DBList(tableName, DataWorker.getList(sqlWorker.selectData("SELECT * FROM " + tableName)));
            //statusBar.Text = dblist.values.Count + " строки" + (dblist.values.Count>0?"; {" + dblist.values[0].Key + ", " + dblist.values[0].Value + "}":"");

            EditDBList wnd = new EditDBList(rusName, dblist, sqlWorker);
            wnd.ShowDialog();
        }

        private void editClause_Click_2(object sender, RoutedEventArgs e)
        {
            ClauseList cl = new ClauseList(DataWorker.getClauseList(sqlWorker.selectData("SELECT * FROM Clause")));

            EditClauseList wndC = new EditClauseList(cl, sqlWorker);
            wndC.ShowDialog();
        }

        private void editRank_Click(object sender, RoutedEventArgs e)
        {
            RankList rl = new RankList(DataWorker.getRankList(sqlWorker.selectData("SELECT * FROM Rank ORDER BY priority")));

            EditRank wndR = new EditRank(rl, sqlWorker);
            wndR.ShowDialog();
        }

        private void editStructureUnit_Click(object sender, RoutedEventArgs e)
        {
            MilitaryUnitList mList = new MilitaryUnitList(DataWorker.getMilitaryUnitList(sqlWorker.selectData("SELECT * FROM MilitaryUnit")));
            EditStructure wE = new EditStructure(sqlWorker, mList);
            wE.ShowDialog();
        }

        private void menuAccomplice_Click(object sender, RoutedEventArgs e)
        {
            AccompliceList al = new AccompliceList(
                DataWorker.getAccompliceList(
                    sqlWorker.selectData("SELECT R.shortName as rank, S.shortName as subUnit, SF.shortName as battalion, M.shortName as militaryUnit, A.*, R.priority " +
                        "FROM Accomplice A " +
                        "INNER JOIN SubUnit S ON S.idSubUnit = A.idSubUnit " +
                        "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                        "LEFT JOIN SubUnit SF ON S.idFKSubUnit = SF.idSubUnit " +
                        "LEFT JOIN MilitaryUnit M ON M.idMilitaryUnit = S.idMilitaryUnit OR M.idMilitaryUnit = SF.idMilitaryUnit ")));

            Accomplices wndR = new Accomplices(al, sqlWorker);
            wndR.ShowDialog();
        }

        private void btnDeleteCrimeOrIncidents_Click_1(object sender, RoutedEventArgs e)
        {
            if (crimesDataGrid.SelectedItem != null && MessageBox.Show("Вы действительно хотите удалить выбранное преступление?",
                "Подтвердите удаление",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int id = (crimesDataGrid.SelectedItem as Crime).Id;
                if (sqlWorker.deleteCrime((crimesDataGrid.SelectedItem as Crime).Id))
                    for (int i = 0; i < crimes.Count; i++)
                        if (crimes[i].Id == id)
                            crimes.RemoveAt(i);
            }
        }

        private void crimesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (crimesDataGrid.SelectedItem != null)
            {
                Crime newC = AddCrime.gtNewCrime(sqlWorker, crimesDataGrid.SelectedItem as Crime);
                for (int i = 0; i < crimes.Count; i++)
                    if (newC.Id == crimes[i].Id)
                        crimes[i] = newC;
                crimes = DataWorker.getCrimes(
                   sqlWorker,
                   dpLeft.Text == "" ? "" : dpLeft.SelectedDate.Value.ToString("yyyy.MM.dd"),
                   dpRight.Text == "" ? "9999.99.99" : dpRight.SelectedDate.Value.ToString("yyyy.MM.dd"));
                coll.Source = crimes;
                coll.Filter += coll_Filter;
                crimesDataGrid.ItemsSource = coll.View;
                coll.View.GroupDescriptions.Clear();
                if (chkGroupMU.IsChecked.Value) coll.View.GroupDescriptions.Add(new PropertyGroupDescription("MilitaryUnit"));
                if (cbGroupOn.SelectedIndex == 1) coll.View.GroupDescriptions.Add(new PropertyGroupDescription("Accomplice"));
                if (cbGroupOn.SelectedIndex == 2) coll.View.GroupDescriptions.Add(new PropertyGroupDescription("Clause"));
                coll.View.Refresh();
            }
        }

        private void crimesDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void btnOk_Click_1(object sender, RoutedEventArgs e)
        {
            crimes = DataWorker.getCrimes(
               sqlWorker,
               dpLeft.Text == "" ? "" : dpLeft.SelectedDate.Value.ToString("yyyy.MM.dd"),
               dpRight.Text == "" ? "9999.99.99" : dpRight.SelectedDate.Value.ToString("yyyy.MM.dd"));
            coll.Source = crimes;
            coll.Filter += coll_Filter;
            coll.View.CollectionChanged += View_CollectionChanged;
            coll.View.GroupDescriptions.Clear();
            if (chkGroupMU.IsChecked.Value) coll.View.GroupDescriptions.Add(new PropertyGroupDescription("MilitaryUnit"));
            if (cbGroupOn.SelectedIndex == 1) coll.View.GroupDescriptions.Add(new PropertyGroupDescription("Accomplice"));
            if (cbGroupOn.SelectedIndex == 2) coll.View.GroupDescriptions.Add(new PropertyGroupDescription("Clause"));
            crimesDataGrid.ItemsSource = coll.View;
            crimesDataGrid.CanUserAddRows = false;
            coll.View.Refresh();

        }

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (crimesDataGrid.SelectedItem != null)
                {
                    Crime c;
                    c = crimesDataGrid.SelectedItem as Crime;
                    if ((sender as CheckBox).IsChecked.Value)
                    {
                        sqlWorker.executeQuery("UPDATE Crime SET isRegistred = 1 WHERE idCrime = " + c.Id);
                        c.IsRegistred = true;
                    }
                    else
                    {
                        sqlWorker.executeQuery("UPDATE Crime SET isRegistred = 0 WHERE idCrime = " + c.Id);
                        c.IsRegistred = false;
                    }
                    coll.View.Refresh();
                }

            }
            catch
            {

            }
        }

        private void btnFilter_Click_1(object sender, RoutedEventArgs e)
        {
            if (rowFilter.Height.Value == 25)
            {
                rowFilter.Height = new GridLength(0);
                btnFilter.Content = "Фильтр ▼";
            }
            else
            {
                rowFilter.Height = new GridLength(25);
                btnFilter.Content = "Фильтр ▲";
            }
        }

        private void btnClearFilter_Click_1(object sender, RoutedEventArgs e)
        {
            txFilterAccomplice.Text = "";
            txFilterClause.Text = "";
            txFilterFabula.Text = "";
            coll.View.Refresh();
        }

        private void txFilterFabula_TextChanged(object sender, TextChangedEventArgs e)
        {
            coll.View.Refresh();
        }
        
        void coll_Filter(object sender, FilterEventArgs e)
        {
            if(cbFilterMilitaryUnit.SelectedItem != null)
                e.Accepted = (((e.Item as Crime).Story.ToLower().IndexOf(txFilterFabula.Text.ToLower()) >= 0|| txFilterFabula.Text=="")  &&
                    ((e.Item as Crime).Accomplice.ToLower().IndexOf(txFilterAccomplice.Text.ToLower()) >= 0 || txFilterAccomplice.Text == "" )&&
                    ((e.Item as Crime).Clause.ToLower().IndexOf(txFilterClause.Text.ToLower()) >= 0  || txFilterClause.Text == "" ) &&
                    ((e.Item as Crime).IdMilitaryUnit == (cbFilterMilitaryUnit.SelectedItem as MilitaryUnit).Id ||
                        ((cbFilterMilitaryUnit.SelectedItem as MilitaryUnit).Id == -1)) &&
                    ((cbRegistred.SelectedIndex == 1 && (e.Item as Crime).IsRegistred) ||
                        (cbRegistred.SelectedIndex == 2 && !(e.Item as Crime).IsRegistred) ||
                        cbRegistred.SelectedIndex == 0) &&
                    ((cbType.SelectedIndex == 1 && (e.Item as Crime).IdClause != 0) ||
                        (cbType.SelectedIndex == 2 && (e.Item as Crime).IdClause == 0) || 
                        cbType.SelectedIndex == 0));
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (coll != null)
                coll.View.Refresh();
        }

        private void crimesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbCountSelected.Text = crimesDataGrid.SelectedItems.Count.ToString();
        }

        private void cbFilterMilitaryUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            coll.View.Refresh();
        }

        private void MenuAnalyze_Click(object sender, RoutedEventArgs e)
        {
            AnalyzeSettings wndAnalyze = new AnalyzeSettings(sqlWorker);
            wndAnalyze.ShowDialog();
        }
        
        private void btnToWord_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Office.Interop.Word.Application winword =
                    new Microsoft.Office.Interop.Word.Application();

                winword.Visible = false;

                //Заголовок документа
                winword.Documents.Application.Caption = "CrimesAndIncidents";

                object missing = System.Reflection.Missing.Value;

                //Создание нового документа
                Microsoft.Office.Interop.Word.Document document = winword.Documents.Add(ref missing, ref missing, ref missing, ref missing);

                //Добавление текста в документ
                document.PageSetup.Orientation = Microsoft.Office.Interop.Word.WdOrientation.wdOrientLandscape;
                document.Content.SetRange(0, 0);

                //Добавление текста со стилем Заголовок 1
                Microsoft.Office.Interop.Word.Paragraph para1 = document.Content.Paragraphs.Add(ref missing);
                //para1.Range.set_Style(styleHeading1);
                para1.Range.Font.Size = 14;
                para1.Range.Text = "Преступления и происшествия за " +
                    ((dpLeft.Text == "" && dpRight.Text == "") ? "все время" :
                        ("период " +
                        (dpLeft.Text == "" ? "" : "c " + dpLeft.Text) +
                        (dpRight.Text == "" ? "" : " по " + dpRight.Text)));
                para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                para1.Range.InsertParagraphAfter(); 
                para1.Range.InsertParagraphAfter();

                //Создание таблицы 5х5
                Microsoft.Office.Interop.Word.Table firstTable = document.Tables.Add(
                    para1.Range,
                    crimesDataGrid.Items.Count + 1, //число строк
                    crimesDataGrid.Columns.Count,  //число столбцов нужно динамически, после того как будет выбор столбцов
                    ref missing,
                    ref missing);

                firstTable.Range.Font.Size = 12;
                firstTable.Range.Font.Name = "Times New Roman";
                firstTable.Borders.Enable = 1;

                for (int i = 0; i < firstTable.Rows.Count; i++)
                {
                    for (int j = 0; j < firstTable.Columns.Count; j++)
                    {
                        //Заголовок таблицы
                        if (i == 0)
                        {
                            firstTable.Rows[1].Cells[j + 1].Range.Text = j == 0 ? "№\nп/п" : crimesDataGrid.Columns[j].Header.ToString();

                            //Выравнивание текста в заголовках столбцов по центру
                            firstTable.Rows[i + 1].Cells[j + 1].VerticalAlignment =
                                Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            firstTable.Rows[i + 1].Cells[j + 1].Range.ParagraphFormat.Alignment =
                                Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                        }
                        //Значения ячеек
                        else if (j == 0)
                        {
                            firstTable.Rows[i + 1].Cells[j + 1].Range.Text = i.ToString();
                        }
                    }

                    if (i != 0)
                    {
                        firstTable.Rows[i + 1].Cells[2].Range.Text = (crimesDataGrid.Items[i - 1] as Crime).Story;
                        firstTable.Rows[i + 1].Cells[3].Range.Text = (crimesDataGrid.Items[i - 1] as Crime).DateCommit;
                        firstTable.Rows[i + 1].Cells[4].Range.Text = (crimesDataGrid.Items[i - 1] as Crime).DateInstitution;
                        firstTable.Rows[i + 1].Cells[5].Range.Text = (crimesDataGrid.Items[i - 1] as Crime).DateRegistration;
                        firstTable.Rows[i + 1].Cells[6].Range.Text = (crimesDataGrid.Items[i - 1] as Crime).Accomplice;
                        firstTable.Rows[i + 1].Cells[7].Range.Text = (crimesDataGrid.Items[i - 1] as Crime).Clause;
                        firstTable.Rows[i + 1].Cells[8].Range.Text = (crimesDataGrid.Items[i - 1] as Crime).MilitaryUnit;
                    }
                }
                firstTable.AutoFitBehavior(Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitContent);

                winword.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (coll != null)
                coll.View.Refresh();
        }

        private void chkGroupMU_Checked(object sender, RoutedEventArgs e)
        {
            coll.View.GroupDescriptions.Clear();
            if (chkGroupMU.IsChecked.Value) coll.View.GroupDescriptions.Add(new PropertyGroupDescription("MilitaryUnit"));
            if (cbGroupOn.SelectedIndex == 1) coll.View.GroupDescriptions.Add(new PropertyGroupDescription("Accomplice"));
            if (cbGroupOn.SelectedIndex == 2) coll.View.GroupDescriptions.Add(new PropertyGroupDescription("Clause"));
            coll.View.Refresh();
        }

        private void chkGroupMU_Unchecked_1(object sender, RoutedEventArgs e)
        {
            coll.View.GroupDescriptions.Clear();
            if (chkGroupMU.IsChecked.Value) coll.View.GroupDescriptions.Add(new PropertyGroupDescription("MilitaryUnit"));
            if (cbGroupOn.SelectedIndex == 1) coll.View.GroupDescriptions.Add(new PropertyGroupDescription("Accomplice"));
            if (cbGroupOn.SelectedIndex == 2) coll.View.GroupDescriptions.Add(new PropertyGroupDescription("Clause"));
            coll.View.Refresh();
        }

        private void cbGroupOn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (coll != null)
            {
                coll.View.GroupDescriptions.Clear();
                if (chkGroupMU.IsChecked.Value) coll.View.GroupDescriptions.Add(new PropertyGroupDescription("MilitaryUnit"));
                if (cbGroupOn.SelectedIndex == 1) coll.View.GroupDescriptions.Add(new PropertyGroupDescription("Accomplice"));
                if (cbGroupOn.SelectedIndex == 2) coll.View.GroupDescriptions.Add(new PropertyGroupDescription("Clause"));
                coll.View.Refresh();
            }
        }


        
    }
}