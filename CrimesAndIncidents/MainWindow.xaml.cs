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

namespace CrimesAndIncidents
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqliteWorker sqlWorker;
        ObservableCollection<Crime> crimes;

        public MainWindow()
        {
            InitializeComponent();
            crimes = new ObservableCollection<Crime>();
            try
            {
                sqlWorker = new SqliteWorker("CrimesAndIncidents");
                crimes = DataWorker.getCrimes(sqlWorker);

                //изменение формата дат, оставлю, вдруг понадобится
                //for (int i = 0; i < crimes.Count; i++)
                //{
                //    sqlWorker.executeQuery("UPDATE Crime SET " +
                //        "dateRegistration = " + (crimes[i].DateRegistration == "" ? "NULL, " : "'" +crimes[i].DateRegistration + "', ") +
                //        "dateInstitution = " + (crimes[i].DateInstitution == "" ? "NULL, " : "'" + crimes[i].DateInstitution + "', ") +
                //        "dateCommit = " + (crimes[i].DateCommit == "" ? "NULL, " : "'" + crimes[i].DateCommit + "' ") +
                //        "WHERE idCrime = " + crimes[i].Id);
                //}

                crimesDataGrid.ItemsSource = crimes;
                crimesDataGrid.CanUserAddRows = false;
                //crimesDataGrid.IsReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Во время загрузки приложения возникли неполадки:\n" + ex.Message);
            }
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
                crimes = DataWorker.getCrimes(sqlWorker);
                crimesDataGrid.ItemsSource = crimes;
            }
        }

        private void crimesDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void dpRight_CalendarClosed(object sender, RoutedEventArgs e)
        {
            crimes = DataWorker.getCrimes(
                sqlWorker,
                dpLeft.Text == "" ? "" : dpLeft.SelectedDate.Value.ToString("yyyy.MM.dd"),
                dpRight.Text=="" ? "9999.99.99" : dpRight.SelectedDate.Value.ToString("yyyy.MM.dd"));
            crimesDataGrid.ItemsSource = crimes;
        }

        private void dpLeft_CalendarClosed(object sender, RoutedEventArgs e)
        {
            crimes = DataWorker.getCrimes(
                sqlWorker,
                dpLeft.Text == "" ? "" : dpLeft.SelectedDate.Value.ToString("yyyy.MM.dd"),
                dpRight.Text == "" ? "9999.99.99" : dpRight.SelectedDate.Value.ToString("yyyy.MM.dd"));
            crimesDataGrid.ItemsSource = crimes;
        }

        private void dpLeft_LostFocus(object sender, RoutedEventArgs e)
        {
            crimes = DataWorker.getCrimes(
                sqlWorker,
                dpLeft.Text == "" ? "" : dpLeft.SelectedDate.Value.ToString("yyyy.MM.dd"),
                dpRight.Text == "" ? "9999.99.99" : dpRight.SelectedDate.Value.ToString("yyyy.MM.dd"));
            crimesDataGrid.ItemsSource = crimes;
        }

        private void dpRight_LostFocus(object sender, RoutedEventArgs e)
        {
            crimes = DataWorker.getCrimes(
                sqlWorker,
                dpLeft.Text == "" ? "" : dpLeft.SelectedDate.Value.ToString("yyyy.MM.dd"),
                dpRight.Text == "" ? "9999.99.99" : dpRight.SelectedDate.Value.ToString("yyyy.MM.dd"));
            crimesDataGrid.ItemsSource = crimes;
        }

        private void btnOk_Click_1(object sender, RoutedEventArgs e)
        {
            crimes = DataWorker.getCrimes(
               sqlWorker,
               dpLeft.Text == "" ? "" : dpLeft.SelectedDate.Value.ToString("yyyy.MM.dd"),
               dpRight.Text == "" ? "9999.99.99" : dpRight.SelectedDate.Value.ToString("yyyy.MM.dd"));
            crimesDataGrid.ItemsSource = crimes;
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (crimesDataGrid.SelectedItem != null)
                {
                    Crime c;
                    c = crimesDataGrid.SelectedItem as Crime;
                    sqlWorker.executeQuery("UPDATE Crime SET isRegistred = 1 WHERE idCrime = " + c.Id);
                    c.IsRegistred = true;
                }

            }
            catch
            {

            }
        }

        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (crimesDataGrid.SelectedItem != null)
                {
                    Crime c;
                    c = crimesDataGrid.SelectedItem as Crime;
                    sqlWorker.executeQuery("UPDATE Crime SET isRegistred = 0 WHERE idCrime = " + c.Id);
                    c.IsRegistred = false;
                }

            }
            catch
            {

            }
        }
    }
}