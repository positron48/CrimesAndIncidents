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
        DataTable data;

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
    }
}
