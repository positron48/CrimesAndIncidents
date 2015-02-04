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
                    throw new Exception("Некорректные данны в БД:\n" + ex.Message);
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
                    throw new Exception("Некорректные данны в БД:\n" + ex.Message);
                }
            }
            return list;
        }
    }
}
