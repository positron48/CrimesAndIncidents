using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace CrimesAndIncidents
{
    /// <summary>
    /// Логика взаимодействия для Analyze.xaml
    /// </summary>
    public partial class Analyze : Window
    {
        private SqliteWorker sqlWorker;
        PleaseWait wndP = new PleaseWait();

        public Analyze(SqliteWorker _sqlWorker)
        {
            sqlWorker = _sqlWorker;

            InitializeComponent();

            dpLeft.Text = "01.01." + DateTime.Now.ToString("yyyy");
            dpRight.Text = DateTime.Now.ToString("dd.MM.yyyy");
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            AnalyzeSettings wndAnalyze = new AnalyzeSettings(sqlWorker);
            wndAnalyze.ShowDialog();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            wndP.Show();

            try
            {
                string dateLeft = DataWorker.getDateInTrueFormat(dpLeft.Text);
                string dateRight = DataWorker.getDateInTrueFormat(dpRight.Text);

                string dateLeftPrev = DataWorker.getDateInTrueFormat(dpLeft.Text, -1);
                string dateRightPrev = DataWorker.getDateInTrueFormat(dpRight.Text, -1);

                DataTable tableCrimes = sqlWorker.selectData("SELECT Tab1.number || ' (' || Tab1.shortName || ')' as вч,Tab1.countCrimes преступления, Tab2.призывники as призывники, Tab2.контрактники as контрактники, Tab2.прапорщики as прапорщики, Tab2.офицеры as офицеры FROM " +
                    "(SELECT M.number, M.shortName, COUNT(C.idMilitaryUnit) as countCrimes FROM " +
                        "MilitaryUnit M LEFT JOIN " +
                        "Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration BETWEEN '" + dateLeft + "' AND '" + dateRight + "' AND C.idClause > -1 " +
                    "GROUP BY M.number, M.shortName " +
                    "ORDER BY M.idMilitaryUnit) Tab1 " +
                    "INNER JOIN " +
                    "(SELECT M1.number, M1.name, T1.n1 as призывники, T2.n2 as контрактники, T3.n3 as прапорщики, T4.n4 as офицеры FROM  " +
                    "(SELECT M.number, M.name FROM MilitaryUnit M) M1 LEFT JOIN  " +
                    "(SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n1 FROM " +
                        "MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit " +
                        "LEFT JOIN Portaking P ON C.idCrime = P.idCrime " +
                        "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " +
                        "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                    "WHERE A.isContrakt = 0 " +
                        "AND C.isRegistred = 1  " +
                        "AND C.dateRegistration  " +
                        "BETWEEN '" + dateLeft + "' AND '" + dateRight + "'  " +
                        "AND C.idClause > -1 " +
                    "GROUP BY M.idMilitaryUnit " +
                    "ORDER BY M.idMilitaryUnit) T1 ON T1.number = M1.number  " +
                    "LEFT JOIN " +
                    "(SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n2 FROM " +
                        "MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit " +
                        "LEFT JOIN Portaking P ON C.idCrime = P.idCrime " +
                        "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " +
                        "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                    "WHERE A.isContrakt = 1 " +
                        "AND R.priority < 60 " +
                        "AND C.isRegistred = 1  " +
                        "AND C.dateRegistration  " +
                        "BETWEEN '" + dateLeft + "' AND '" + dateRight + "'  " +
                        "AND C.idClause > -1 " +
                    "GROUP BY M.idMilitaryUnit " +
                    "ORDER BY M.idMilitaryUnit) T2 ON T2.number = M1.number  " +
                    "LEFT JOIN " +
                    "(SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n3 FROM " +
                        "MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit " +
                        "LEFT JOIN Portaking P ON C.idCrime = P.idCrime " +
                        "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " +
                        "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                    "WHERE A.isContrakt = 1 " +
                        "AND R.priority BETWEEN 60 AND 75 " +
                        "AND C.isRegistred = 1  " +
                        "AND C.dateRegistration  " +
                        "BETWEEN '" + dateLeft + "' AND '" + dateRight + "'  " +
                        "AND C.idClause > -1 " +
                    "GROUP BY M.idMilitaryUnit " +
                    "ORDER BY M.idMilitaryUnit) T3  ON T3.number = M1.number  " +
                    "LEFT JOIN " +
                    "(SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n4 FROM " +
                        "MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit " +
                        "LEFT JOIN Portaking P ON C.idCrime = P.idCrime " +
                        "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " +
                        "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                    "WHERE A.isContrakt = 1 " +
                        "AND R.priority > 75 " +
                        "AND C.isRegistred = 1  " +
                        "AND C.dateRegistration  " +
                        "BETWEEN '" + dateLeft + "' AND '" + dateRight + "'  " +
                        "AND C.idClause > -1 " +
                    "GROUP BY M.idMilitaryUnit " +
                    "ORDER BY M.idMilitaryUnit) T4  ON T4.number = M1.number) Tab2 " +
                    "ON Tab1.number = Tab2.number");

                dgCrimes.ItemsSource = tableCrimes.DefaultView;

                dgCrimes.CanUserAddRows = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так: " + ex.Message);
           
            }
            wndP.Hide();
        }
    }
}
