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
        ObservableCollection<Crime> crimes = new ObservableCollection<Crime>();

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                sqlWorker = new SqliteWorker("CrimesAndIncidents");
                crimes = DataWorker.getCrimes(sqlWorker.selectData("SELECT "+
                    "C.story, C.dateCommit, C.dateInstitution, C.dateRegistration, R.shortName, A.shortName, Cl.point, Cl.part, Cl.number,Cl.description,  C.dateVerdict, C.verdict" +
                    " FROM Crime C INNER JOIN Portaking P ON C.idCrime = P.idCrime" +
                    " INNER JOIN Accomplice A ON P.idAccomplice = A.idAccomplice" +
                    " INNER JOIN Rank R ON A.idRank = R.idRank" +
                    " INNER JOIN Clause Cl ON C.idClause = Cl.idClause"));
                crimesDataGrid.ItemsSource = crimes;
                crimesDataGrid.CanUserAddRows = false;
                crimesDataGrid.IsReadOnly = true;
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

        private void crimesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
            RankList rl = new RankList(DataWorker.getRankList(sqlWorker.selectData("SELECT * FROM Rank")));

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
                    sqlWorker.selectData("SELECT R.shortName as rank, S.shortName as subUnit, SF.shortName as battalion, M.shortName as militaryUnit, A.* FROM Accomplice A " +
                        "INNER JOIN SubUnit S ON S.idSubUnit = A.idSubUnit " +
                        "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                        "LEFT JOIN SubUnit SF ON S.idFKSubUnit = SF.idSubUnit " +
                        "LEFT JOIN MilitaryUnit M ON M.idMilitaryUnit = S.idMilitaryUnit OR M.idMilitaryUnit = SF.idMilitaryUnit ")));

            Accomplices wndR = new Accomplices(al, sqlWorker);
            wndR.ShowDialog();
        }

        private void btnDeleteCrimeOrIncidents_Click_1(object sender, RoutedEventArgs e)
        {
            if (crimesDataGrid.SelectedItem != null && MessageBox.Show("Вы действительно хотите удалить выбранное преступление?","Подтвердите удаление",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (sqlWorker.deleteCrime((crimesDataGrid.SelectedItem as Crime).Id))
                    for (int i = 0; i < crimes.Count; i++)
                        if (crimes[i].Id == (crimesDataGrid.SelectedItem as Crime).Id)
                            crimes.RemoveAt(i);
            }
        }

    }
}
