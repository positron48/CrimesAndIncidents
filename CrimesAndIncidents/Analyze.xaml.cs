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

        public Analyze(SqliteWorker _sqlWorker)
        {
            sqlWorker = _sqlWorker;

            InitializeComponent();

            dpLeft.Text = "01.01." + DateTime.Now.ToString("yyyy");
            dpRight.Text = DateTime.Now.ToString("dd.MM.yyyy");

            btnOk_Click(this, null);
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            AnalyzeSettings wndAnalyze = new AnalyzeSettings(sqlWorker);
            wndAnalyze.ShowDialog();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dateLeft = DataWorker.getDateInTrueFormat(dpLeft.Text);
                string dateRight = DataWorker.getDateInTrueFormat(dpRight.Text);

                string dateLeftPrev = DataWorker.getDateInTrueFormat(dpLeft.Text, -1);
                string dateRightPrev = DataWorker.getDateInTrueFormat(dpRight.Text, -1);

                DataTable tableCrimes;

                switch (cbType.SelectedIndex)
                {
                    case 0:
                        tableCrimes = getTableAccomplice(dateLeft, dateRight);
                        dgCrimes.ItemsSource = tableCrimes.DefaultView;
                        break;
                    case 1:
                        tableCrimes = getTableCategories(dateLeft, dateRight);
                        dgCrimes.ItemsSource = tableCrimes.DefaultView;
                        break;
                    case 2:
                        tableCrimes = getTableYearCommit(dateLeft, dateRight);
                        dgCrimes.ItemsSource = tableCrimes.DefaultView;
                        break;
                    default:
                        break;
                }
                dgCrimes.CanUserAddRows = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так: " + ex.Message);
           
            }
        }
                
        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            dgCrimes.ItemsSource = null;
            btnOk_Click(this, null);
        }

        private DataTable getTableCategories(string dateLeft, string dateRight)
        {
            DataTable tableMilitaryUnits = sqlWorker.selectData("SELECT shortName FROM MilitaryUnit ORDER BY idMilitaryUnit");
            DataTable tableCategories = sqlWorker.selectData("SELECT description FROM Category;");
            DataTable tableData = sqlWorker.selectData("SELECT M.idMilitaryUnit, Cat.idCategory as 'категория', COUNT(C.story) as 'количество преступлений'   " +
                "FROM MilitaryUnit M "+
                    "INNER JOIN Crime C ON M.idMilitaryUnit = C.idMilitaryUnit  " +
                        "AND C.isRegistred = 1  "+
                        "AND C.idClause > -1 "+
                        "AND C.dateRegistration BETWEEN '" + dateLeft + "' AND '" + dateRight + "' " +
                    "INNER JOIN InCategory InC ON C.idCrime = InC.idCrime " +
                    "INNER JOIN Category Cat ON InC.idCategory = Cat.idCategory " +
                "GROUP BY M.shortName, Cat.description " +
                "ORDER BY M.idMilitaryUnit");

            DataTable tableCrimes = new DataTable();
            tableCrimes.Columns.Add("Категория");
            tableCrimes.Columns.Add("Всего");

            for (int i = 0; i < tableMilitaryUnits.Rows.Count; i++)
                tableCrimes.Columns.Add(tableMilitaryUnits.Rows[i][0].ToString());

            for (int i = 0; i < tableCategories.Rows.Count; i++)
            {
                tableCrimes.Rows.Add(tableCrimes.NewRow());
                tableCrimes.Rows[i][0] = tableCategories.Rows[i][0].ToString();
            }
            tableCrimes.Rows.Add(tableCrimes.NewRow());
            tableCrimes.Rows[tableCategories.Rows.Count][0] = "Всего";

            for (int i = 0; i < tableData.Rows.Count; i++)
            {
                int idMil = Int32.Parse(tableData.Rows[i][0].ToString());
                int idCat = Int32.Parse(tableData.Rows[i][1].ToString());
                string val = tableData.Rows[i][2].ToString();
                tableCrimes.Rows[idCat - 1][idMil +1] = val;
            }

            int[] valuesOnMilitaryUnit = new int[tableMilitaryUnits.Rows.Count];
            int[] valuesOnCategories = new int[tableCategories.Rows.Count];

            for (int i = 0; i < tableMilitaryUnits.Rows.Count; i++)
            {
                valuesOnMilitaryUnit[i] = 0;
                for (int j = 0; j < tableCategories.Rows.Count; j++)
                {
                    if (tableCrimes.Rows[j][i + 2].ToString() != "")
                        valuesOnMilitaryUnit[i] += Int32.Parse(tableCrimes.Rows[j][i + 2].ToString());
                }
                tableCrimes.Rows[tableCategories.Rows.Count][i + 2] = valuesOnMilitaryUnit[i];
            }

            int sum = 0;
            for (int i = 0; i < tableCategories.Rows.Count; i++)
            {
                valuesOnCategories[i] = 0;
                for (int j = 0; j < tableMilitaryUnits.Rows.Count; j++)
                {
                    if (tableCrimes.Rows[i][j + 2].ToString() != "")
                        valuesOnCategories[i] += Int32.Parse(tableCrimes.Rows[i][j + 2].ToString());
                }
                tableCrimes.Rows[i][1] = valuesOnCategories[i];
                sum += valuesOnCategories[i];
            }
            tableCrimes.Rows[tableCategories.Rows.Count][1] = sum;

            return tableCrimes;
        }
       
        private DataTable getTableAccomplice(string dateLeft, string dateRight)
        {
            DataTable tableCrimes = sqlWorker.selectData("SELECT Tab1.number || ' (' || Tab1.shortName || ')' as вч,Tab1.countCrimes преступления, Tab3.quantity as происшествия, Tab2.призывники as призывники, Tab2.контрактники as контрактники, Tab2.прапорщики as прапорщики, Tab2.офицеры as офицеры FROM " +
                "(SELECT M1.number, M1.shortName, T2.n2 as countCrimes  FROM "+
                    "(SELECT M.number, M.shortName FROM MilitaryUnit M) M1 LEFT JOIN  "+
                    "(SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n2 FROM "+
                        "MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit "+
                        "LEFT JOIN Portaking P ON C.idCrime = P.idCrime "+
                        "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice "+
                    "WHERE C.isRegistred = 1  "+
                        "AND C.dateRegistration  "+
                        "BETWEEN '" + dateLeft + "' AND '" + dateRight + "'   " +
                        "AND C.idClause > -1 "+
                    "GROUP BY M.idMilitaryUnit "+
                    "ORDER BY M.idMilitaryUnit) T2 ON T2.number = M1.number ) Tab1 " +
                "INNER JOIN " +
                    "(SELECT M1.number, M1.shortName, T2.n2 as quantity FROM "+
                        "(SELECT M.number, M.shortName FROM MilitaryUnit M) M1 LEFT JOIN  "+
                        "(SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n2 FROM "+
                            "MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit "+
                            "LEFT JOIN Portaking P ON C.idCrime = P.idCrime "+
                            "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice "+
                        "WHERE C.isRegistred = 1  "+
                            "AND C.dateRegistration  "+
                            "BETWEEN '" + dateLeft + "' AND '" + dateRight + "'  " +
                            "AND C.idClause IS NULL "+
                        "GROUP BY M.idMilitaryUnit "+
                        "ORDER BY M.idMilitaryUnit) T2 ON T2.number = M1.number) Tab3 " +
                    "ON Tab1.number = Tab3.number " +
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

            tableCrimes.Columns[0].ColumnName = "вч";
            tableCrimes.Columns[1].ColumnName = "участники";
            tableCrimes.Columns[2].ColumnName = "проиcшествия";
            tableCrimes.Columns[3].ColumnName = "призывники";
            tableCrimes.Columns[4].ColumnName = "контрактники";
            tableCrimes.Columns[5].ColumnName = "прапорщики";
            tableCrimes.Columns[6].ColumnName = "офицеры";

            tableCrimes.Rows.Add(tableCrimes.NewRow());

            int[] values = new int[6];
            values[0] = values[1] = values[2] = values[3] = values[4] = values[5] = 0;

            for (int i = 0; i < tableCrimes.Rows.Count - 1; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (tableCrimes.Rows[i][j + 1].ToString() != "")
                        values[j] += Int32.Parse(tableCrimes.Rows[i][j + 1].ToString());
                }
            }

            tableCrimes.Rows[tableCrimes.Rows.Count - 1][0] = "Всего:";
            for (int j = 0; j < 6; j++)
            {
                tableCrimes.Rows[tableCrimes.Rows.Count - 1][j + 1] = values[j].ToString();
            }
            return tableCrimes;
        }

        private DataTable getTableYearCommit(string dateLeft, string dateRight)
        {
            DataTable tableMilitaryUnits = sqlWorker.selectData("SELECT shortName FROM MilitaryUnit ORDER BY idMilitaryUnit");
            DataTable tableYears = sqlWorker.selectData("SELECT SUBSTR(C.dateCommit,1,4) FROM Crime C " +
                "WHERE C.isRegistred = 1 " +
                    "AND C.dateRegistration BETWEEN '" + dateLeft + "' AND '" + dateRight + "'  " +
                    "AND C.idClause > -1 " +
                "GROUP BY SUBSTR(C.dateCommit,1,4) " +
                "ORDER BY SUBSTR(C.dateCommit,1,4);");
            DataTable tableData = sqlWorker.selectData("SELECT M.idMilitaryUnit, SUBSTR(C.dateCommit,1,4), COUNT(C.idCrime) FROM MilitaryUnit M " +
                    "LEFT JOIN Crime C ON C.idMilitaryUnit = M.idMilitaryUnit " +
                        "AND C.isRegistred = 1 " +
                        "AND C.dateRegistration BETWEEN '" + dateLeft + "' AND '" + dateRight + "' " +
                        "AND C.idClause > -1 " +
                "GROUP BY M.shortName, SUBSTR(C.dateCommit,1,4) " +
                "ORDER BY M.idMilitaryUnit, SUBSTR(C.dateCommit,1,4);");

            DataTable tableCrimes = new DataTable();
            tableCrimes.Columns.Add("Год совершения");
            tableCrimes.Columns.Add("Всего");

            for (int i = 0; i < tableYears.Rows.Count; i++)
                tableCrimes.Columns.Add(tableYears.Rows[i][0].ToString());

            for (int i = 0; i < tableMilitaryUnits.Rows.Count; i++)
            {
                tableCrimes.Rows.Add(tableCrimes.NewRow());
                tableCrimes.Rows[i][0] = tableMilitaryUnits.Rows[i][0].ToString();
            }
            tableCrimes.Rows.Add(tableCrimes.NewRow());
            tableCrimes.Rows[tableMilitaryUnits.Rows.Count][0] = "Всего";

            for (int i = 0; i < tableData.Rows.Count; i++)
            {
                int idMil = Int32.Parse(tableData.Rows[i][0].ToString());
                string year = tableData.Rows[i][1].ToString();
                string val = tableData.Rows[i][2].ToString();
                if(year != "")
                    tableCrimes.Rows[idMil - 1][year] = val;
            }

            int[] valuesOnMilitaryUnit = new int[tableMilitaryUnits.Rows.Count];
            int[] valuesOnYear = new int[tableYears.Rows.Count];

            for (int i = 0; i < tableYears.Rows.Count; i++)
            {
                valuesOnYear[i] = 0;
                for (int j = 0; j < tableMilitaryUnits.Rows.Count; j++)
                {
                    if (tableCrimes.Rows[j][i + 2].ToString() != "")
                        valuesOnYear[i] += Int32.Parse(tableCrimes.Rows[j][i + 2].ToString());
                }
                tableCrimes.Rows[tableMilitaryUnits.Rows.Count][i + 2] = valuesOnYear[i];
            }
            
            int sum = 0;
            for (int i = 0; i < tableMilitaryUnits.Rows.Count; i++)
            {
                valuesOnMilitaryUnit[i] = 0;
                for (int j = 0; j < tableYears.Rows.Count; j++)
                {
                    if (tableCrimes.Rows[i][j + 2].ToString() != "")
                        valuesOnMilitaryUnit[i] += Int32.Parse(tableCrimes.Rows[i][j + 2].ToString());
                }
                tableCrimes.Rows[i][1] = valuesOnMilitaryUnit[i];
                sum += valuesOnMilitaryUnit[i];
            }
            tableCrimes.Rows[tableMilitaryUnits.Rows.Count][1] = sum;

            return tableCrimes;
        }

        private void btnPrevYear_Click(object sender, RoutedEventArgs e)
        {
            dpLeft.SelectedDate = new DateTime(dpLeft.SelectedDate.Value.Year - 1, dpLeft.SelectedDate.Value.Month, dpLeft.SelectedDate.Value.Day);
            dpRight.SelectedDate = new DateTime(dpRight.SelectedDate.Value.Year - 1, dpRight.SelectedDate.Value.Month, dpRight.SelectedDate.Value.Day);
            btnOk_Click(this, null);
        }

        private void btpNextYear_Click(object sender, RoutedEventArgs e)
        {
            dpLeft.SelectedDate = new DateTime(dpLeft.SelectedDate.Value.Year + 1, dpLeft.SelectedDate.Value.Month, dpLeft.SelectedDate.Value.Day);
            dpRight.SelectedDate = new DateTime(dpRight.SelectedDate.Value.Year + 1, dpRight.SelectedDate.Value.Month, dpRight.SelectedDate.Value.Day);
            btnOk_Click(this, null);
        }
    }
}
