using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimesAndIncidents
{
    static class DataWorker
    {


        static public ObservableCollection<KeyValue> getList(DataTable table)
        {
            ObservableCollection<KeyValue> list = new ObservableCollection<KeyValue>();

            if (table.Rows.Count > 0 && table.Columns.Count == 2)
            {
                try
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        KeyValue k = new KeyValue(Int32.Parse(table.Rows[i][0].ToString()), table.Rows[i][1].ToString());
                        list.Add(k);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Некорректные данные в БД:\n" + ex.Message);
                }
            }
            return list;
        }

        static public ObservableCollection<Clause> getClauseList(DataTable table)
        {
            ObservableCollection<Clause> list = new ObservableCollection<Clause>();

            if (table.Rows.Count > 0 && table.Columns.Count == 5)
            {
                try
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        Clause c = new Clause(
                            Int32.Parse(table.Rows[i][0].ToString()),
                            table.Rows[i][1].ToString(),
                            table.Rows[i][2].ToString(),
                            table.Rows[i][3].ToString(),
                            table.Rows[i][4].ToString());
                        list.Add(c);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Некорректные данные в БД:\n" + ex.Message);
                }
            }
            return list;
        }

        public static ObservableCollection<Rank> getRankList(DataTable table)
        {
            ObservableCollection<Rank> list = new ObservableCollection<Rank>();

            if (table.Rows.Count > 0 && table.Columns.Count == 4)
            {
                try
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        Rank r = new Rank(
                            Int32.Parse(table.Rows[i][0].ToString()),
                            table.Rows[i][1].ToString(),
                            table.Rows[i][2].ToString(),
                            table.Rows[i][3].ToString());
                        list.Add(r);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Некорректные данные в БД:\n" + ex.Message);
                }
            }
            return list;
        }

        internal static ObservableCollection<MilitaryUnit> getMilitaryUnitList(DataTable table)
        {
            ObservableCollection<MilitaryUnit> list = new ObservableCollection<MilitaryUnit>();

            if (table.Rows.Count > 0 && table.Columns.Count == 7)
            {
                try
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        MilitaryUnit r = new MilitaryUnit(
                            Int32.Parse(table.Rows[i][0].ToString()),
                            table.Rows[i][1].ToString(),
                            table.Rows[i][2].ToString(),
                            table.Rows[i][3].ToString(),
                            table.Rows[i][4].ToString(),
                            Int32.Parse(table.Rows[i][5].ToString() == "" ? "1" : table.Rows[i][5].ToString()),
                            Int32.Parse(table.Rows[i][6].ToString()));
                        list.Add(r);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Некорректные данные в БД:\n" + ex.Message);
                }
            }
            return list;
        }

        internal static ObservableCollection<SubUnit> getSubUnitList(DataTable table)
        {
            ObservableCollection<SubUnit> list = new ObservableCollection<SubUnit>();

            if (table.Rows.Count > 0 && table.Columns.Count == 6)
            {
                try
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        SubUnit r = new SubUnit(
                            Int32.Parse(table.Rows[i][0].ToString()),
                            table.Rows[i][1].ToString(),
                            table.Rows[i][2].ToString(),
                            table.Rows[i][3].ToString() == "" ? 0 : Int32.Parse(table.Rows[i][3].ToString()),
                            Int32.Parse(table.Rows[i][3].ToString() == "" ? "0" : table.Rows[i][4].ToString()),
                        Int32.Parse(table.Rows[i][3].ToString() == "" ? "0" : table.Rows[i][5].ToString()));
                        list.Add(r);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Некорректные данные в БД:\n" + ex.Message);
                }
            }
            return list;
        }

        internal static ObservableCollection<Accomplice> getAccompliceList(DataTable table)
        {
            ObservableCollection<Accomplice> list = new ObservableCollection<Accomplice>();

            if (table.Rows.Count > 0 && table.Columns.Count == 21)
            {
                try
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        Accomplice a = new Accomplice(
                            table.Rows[i][4].ToString() == "" ? 0 : Int32.Parse(table.Rows[i][4].ToString()),
                            table.Rows[i][5].ToString() == "" ? 0 : Int32.Parse(table.Rows[i][5].ToString()),
                            table.Rows[i][6].ToString() == "" ? 0 : Int32.Parse(table.Rows[i][6].ToString()),
                            table.Rows[i][7].ToString() == "" ? 0 : Int32.Parse(table.Rows[i][7].ToString()),
                            table.Rows[i][8].ToString() == "" ? 0 : Int32.Parse(table.Rows[i][8].ToString()),
                            table.Rows[i][9].ToString(),
                            table.Rows[i][10].ToString(),
                            table.Rows[i][11].ToString() == "0" ? false : true,
                            table.Rows[i][12].ToString() == "0" ? false : true,
                            table.Rows[i][13].ToString() == "" ? 0 : Int32.Parse(table.Rows[i][13].ToString()),
                            table.Rows[i][14].ToString(),
                            table.Rows[i][15].ToString(),
                            table.Rows[i][16].ToString() == "" ? 0 : Int32.Parse(table.Rows[i][16].ToString()),
                            table.Rows[i][17].ToString() == "0" ? false : true,
                            table.Rows[i][18].ToString(),
                            table.Rows[i][19].ToString() == "" ? 0 : Int32.Parse(table.Rows[i][19].ToString()),
                            table.Rows[i][0].ToString(),
                            table.Rows[i][1].ToString() + " " + table.Rows[i][2].ToString(),
                            table.Rows[i][3].ToString(),
                            table.Rows[i][20].ToString());
                        list.Add(a);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Некорректные данные в БД:\n" + ex.Message);
                }
            }
            return list;
        }

        internal static ObservableCollection<Crime> getCrimes(SqliteWorker sqlWorker, string leftDateRange = "", string rightDateRange = "9999.99.99")
        {
            ObservableCollection<Crime> list = new ObservableCollection<Crime>();

            DataTable tableCrimes = sqlWorker.selectData("SELECT  Cl.point, Cl.part, Cl.number, Cl.description, C.*, M.shortName FROM Crime C " +
                "LEFT JOIN Clause Cl ON C.idClause = Cl.idClause " +
                "INNER JOIN MilitaryUnit M ON M.idMilitaryUnit = C.idMilitaryUnit " +
                "WHERE C.dateRegistration BETWEEN '" + leftDateRange + "' AND '" + rightDateRange + "' " +
                "ORDER BY C.dateRegistration, C.dateInstitution, C.dateCommit, C.story;");

            if (tableCrimes.Rows.Count > 0)
            {
                try
                {
                    for (int i = 0; i < tableCrimes.Rows.Count; i++)
                    {
                        DataTable tableAccomplice = sqlWorker.selectData("SELECT R.shortName, A.shortName, A.isContrakt FROM Accomplice A " +
                            "INNER JOIN Portaking P ON P.idAccomplice = A.idAccomplice " +
                            "INNER JOIN Rank R ON A.idRank = R.idRank " +
                            "WHERE P.idCrime = " + Int32.Parse(tableCrimes.Rows[i][4].ToString()) + ";");
                        string accomplices = "";
                        for (int j = 0; j < tableAccomplice.Rows.Count; j++)
                        {
                            accomplices += (j == 0 ? "" : "\n") + tableAccomplice.Rows[j][0] + (tableAccomplice.Rows[j][2].ToString() == "1" ? " к/с " : " ") + tableAccomplice.Rows[j][1];
                        }

                        Crime c = new Crime(
                            tableCrimes.Rows[i][5].ToString() == "" ? 0 : Int32.Parse(tableCrimes.Rows[i][5].ToString()),
                            tableCrimes.Rows[i][6].ToString() == "" ? 0 : Int32.Parse(tableCrimes.Rows[i][6].ToString()),
                            tableCrimes.Rows[i][7].ToString() == "" ? 0 : Int32.Parse(tableCrimes.Rows[i][7].ToString()),
                            tableCrimes.Rows[i][8].ToString(),
                            tableCrimes.Rows[i][9].ToString(),
                            tableCrimes.Rows[i][10].ToString(),
                            tableCrimes.Rows[i][11].ToString(),
                            tableCrimes.Rows[i][12].ToString(),
                            tableCrimes.Rows[i][13].ToString(),
                            tableCrimes.Rows[i][14].ToString(),
                            tableCrimes.Rows[i][15].ToString(),
                            accomplices,
                                (tableCrimes.Rows[i][0].ToString() == "" ? "" : "п.'" + tableCrimes.Rows[i][0].ToString() + "' ") +
                                (tableCrimes.Rows[i][1].ToString() == "" ? "" : "ч." + tableCrimes.Rows[i][1].ToString() + " ") +
                                (tableCrimes.Rows[i][2].ToString() == "" ? "" : "ст." + tableCrimes.Rows[i][2].ToString() + " ") +
                                (tableCrimes.Rows[i][3].ToString() == "" ? "" : " (" + tableCrimes.Rows[i][3].ToString() + ")"),
                            (tableCrimes.Rows[i][2].ToString() == "" ? "" : tableCrimes.Rows[i][2].ToString()) +       //статья УК РФ
                            (tableCrimes.Rows[i][1].ToString() == "" ? "" : "," + tableCrimes.Rows[i][1].ToString()),
                            Int32.Parse(tableCrimes.Rows[i][16].ToString()),
                            tableCrimes.Rows[i][17].ToString()); //часть статьи

                        c.Id = tableCrimes.Rows[i][4].ToString() == "" ? 0 : Int32.Parse(tableCrimes.Rows[i][4].ToString());
                        list.Add(c);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Некорректные данные в БД:\n" + ex.Message);
                }
            }
            return list;

        }

        internal static string getDateInTrueFormat(string date)
        {
            //преобразование даты из 12.12.2012 в 2012.12.12
            return (date.IndexOf('.') > 2 || date=="" ? date :
                DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy.MM.dd"));
        }
    }
}
