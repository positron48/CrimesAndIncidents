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

    }
}
