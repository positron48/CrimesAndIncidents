using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimesAndIncidents
{
    static class DataWorker
    {
        static public ObservableCollection<Crime> getCrimes(DataTable table)
        {
            ObservableCollection<Crime> crimes = new ObservableCollection<Crime>();

            if (table.Rows.Count > 0 && table.Columns.Count == 12)
            {
                try
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        Crime c = new Crime(table.Rows[i][0].ToString(),
                            table.Rows[i][1].ToString(),
                            table.Rows[i][2].ToString(),
                            table.Rows[i][3].ToString(),
                            table.Rows[i][4].ToString(),
                            table.Rows[i][5].ToString(),
                            table.Rows[i][6].ToString(),
                            table.Rows[i][7].ToString(),
                            table.Rows[i][8].ToString(),
                            table.Rows[i][9].ToString(),
                            table.Rows[i][10].ToString(),
                            table.Rows[i][11].ToString());
                        crimes.Add(c);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Некорректные данны в БД:\n" + ex.Message);
                }
            }
            return crimes;
        }

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

            if (table.Rows.Count > 0 && table.Columns.Count == 20)
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
                            Boolean.Parse(table.Rows[i][11].ToString()),
                            Boolean.Parse(table.Rows[i][12].ToString()),
                            table.Rows[i][13].ToString() == "" ? 0 : Int32.Parse(table.Rows[i][13].ToString()),
                            table.Rows[i][14].ToString(),
                            table.Rows[i][15].ToString(),
                            table.Rows[i][16].ToString() == "" ? 0 : Int32.Parse(table.Rows[i][16].ToString()),
                            Boolean.Parse(table.Rows[i][17].ToString()),
                            table.Rows[i][18].ToString(),
                            table.Rows[i][19].ToString() == "" ? 0 : Int32.Parse(table.Rows[i][19].ToString()),
                            table.Rows[i][0].ToString(),
                            table.Rows[i][1].ToString() + " " + table.Rows[i][2].ToString(),
                            table.Rows[i][3].ToString());
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
    }
}
