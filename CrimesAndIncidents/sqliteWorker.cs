using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace CrimesAndIncidents
{
    public class SqliteWorker
    {
        SQLiteConnection dbConnection;

        public SqliteWorker(string dbFilePath)
        {
            if (dbFilePath != "")
                try
                {
                    dbConnection = new SQLiteConnection("Data Source=" + dbFilePath + ";Version=3;foreign keys=true;");
                    dbConnection.Open();
                }
                catch (Exception ex)
                {
                    throw new Exception("Ошибка чтения файла базы данных \n" + ex.Message);
                }
        }

        public void executeQuery(string query)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(query, dbConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка выполнения запроса \n" + ex.Message);
            }
        }

        public DataTable selectData(string query)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(query, dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();

                DataTable schema = reader.GetSchemaTable();
                DataTable table = new DataTable();

                //добавление столбцов 
                if (schema != null)
                    for (int i = 0; i < schema.Rows.Count; i++)
                    {
                        table.Columns.Add();
                        //table.Columns.Add(getRusName(schema.Rows[i]["BaseTableName"].ToString(), schema.Rows[i][0].ToString())); 
                        //table.Columns.Add(schema.Rows[i]["BaseTableName"].ToString()+" "+ schema.Rows[i][0].ToString());
                    }

                //заполнение строк
                while (reader.Read())
                {
                    DataRow row = table.NewRow();
                    for (int i = 0; i < table.Columns.Count; i++)
                        row[i] = reader[i];
                    table.Rows.Add(row);
                }
                return table;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения данных \n" + ex.Message);
            }

        }

        private string getRusName(string table, string engName)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT rusName FROM RusNamesFields WHERE engName = '" + table + "." + engName + "'", dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            DataTable schema = reader.GetSchemaTable();
            string rusName;
            if (reader.Read())
                rusName = reader[0].ToString();
            else
                rusName = table + " " + engName;
            return rusName;
        }

        public bool addInDBList(string tableName, int p, string newItem)
        {
            try
            {
                executeQuery("INSERT INTO " + tableName + " VALUES(" + p + ", '" + newItem + "');");
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool deleteItemById(string tableName, int id)
        {
            try
            {
                executeQuery("DELETE FROM " + tableName + " WHERE id" + tableName + " = " + id + ";");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool updateElement(string tableName, int id, string description)
        {
            try
            {
                executeQuery("UPDATE " + tableName + " SET description = '" + description + "' WHERE id" + tableName + " = " + id + ";");
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool addClause(int id, Clause newItem)
        {
            try
            {
                executeQuery("INSERT INTO Clause VALUES(" + 
                    id + ",'" + 
                    newItem.Point + "','" + 
                    newItem.Part + "','" + 
                    newItem.Number + "','" + 
                    newItem.Description + "');");

                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool updateClause(Clause clause)
        {
            try
            {
                executeQuery("UPDATE Clause SET point = '" + clause.Point +
                    "', part = '" + clause.Part +
                    "', number = '" + clause.Number +
                    "', description = '" + clause.Description +
                    "' WHERE idClause = " + clause.Id + ";");
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool addRank(int id, Rank newItem)
        {
            try
            {
                executeQuery("INSERT INTO Rank VALUES(" +
                    id + ",'" +
                    newItem.FullName + "','" +
                    newItem.ShortName + "','" +
                    newItem.Priority + "');");

                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool updateRank(Rank rank)
        {
            try
            {
                executeQuery("UPDATE Rank SET fullName = '" + rank.FullName +
                    "', shortName = '" + rank.ShortName +
                    "', priority = '" + rank.Priority +
                    "' WHERE idRank = " + rank.Id + ";");
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool addSubUnit(int id, SubUnit newItem)
        {
            try
            {
                executeQuery("INSERT INTO SubUnit VALUES(" +
                    id + ",'" +
                    newItem.Name + "','" +
                    newItem.ShortName + "'," +
                    (newItem.Quantity == 0 ? "NULL" : newItem.Quantity.ToString()) + "," +
                    (newItem.IdFKSubUnit == -1 ? "NULL" : newItem.IdFKSubUnit.ToString()) + "," +
                    (newItem.IdMilitaryUnit == -1 ? "NULL" : newItem.IdMilitaryUnit.ToString()) + ");");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int getNewId(string tableName)
        {
            DataTable t = selectData("SELECT MAX(id" + tableName + ") FROM "+ tableName );

            return Int16.Parse(t.Rows[0][0].ToString()) + 1;
        }

        internal bool updateSubUnit(SubUnit s)
        {
            try
            {
                executeQuery("UPDATE SubUnit SET name = '" + s.Name +
                    "', shortName = '" + s.ShortName +
                    "', quantity = " + (s.Quantity==0?"NULL":s.Quantity.ToString()) +
                    " WHERE idSubUnit = " + s.Id + ";");
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool addMilitaryUnit(int id, MilitaryUnit newItem)
        {
            try
            {
                executeQuery("INSERT INTO MilitaryUnit VALUES(" +
                    id + ",'" +
                    newItem.FullName + "','" +
                    newItem.Name + "','" +
                    newItem.ShortName + "','" +
                    newItem.Number + "'," +
                    (newItem.Quantity == 0 ? "NULL" : newItem.Quantity.ToString()) + "," +
                    "1);");

                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool updateMilitaryUnit(MilitaryUnit m)
        {
            try
            {
                executeQuery("UPDATE MilitaryUnit SET "+
                    "fullName = " + (m.FullName == "" ? "NULL" : "'" + m.FullName + "'") +
                    ", name = '" + m.Name +
                    "', shortName = '" + m.ShortName +
                    "', number = '" + m.Number +
                    "', quantity = " + (m.Quantity == 0 ? "NULL" : m.Quantity.ToString()) +
                    " WHERE idMilitaryUnit = " + m.Id + ";");
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool addAccomplice(int id, Accomplice newItem)
        {
            try
            {
                executeQuery("INSERT INTO Accomplice VALUES(" +
                    id + "," +
                    (newItem.IdPost == 0 ? "NULL" : newItem.IdPost.ToString()) + "," +
                    newItem.IdRank + "," +
                    newItem.IdSubUnit + "," +
                    (newItem.IdDraft == 0 ? "NULL" : newItem.IdDraft.ToString()) + "," +
                    (newItem.FullName == "" ? "NULL" : "'" + newItem.FullName + "'") + ",'" +
                    newItem.ShortName + "','" +
                    newItem.IsContrakt + "','" +
                    newItem.IsMedic + "'," +
                    (newItem.NumberContrakt == 0 ? "NULL" : newItem.NumberContrakt.ToString()) + "," +
                    (newItem.DateOfFirstContrakt == "" ? "NULL" : "'" + newItem.DateOfFirstContrakt + "'") + "," +
                    (newItem.DateOfLastContrakt == "" ? "NULL" : "'" + newItem.DateOfLastContrakt + "'") + "," +
                    (newItem.IdEducation == 0 ? "NULL" : newItem.IdEducation.ToString()) + ",'" +
                    newItem.Sex + "'," +
                    (newItem.DateOfBirth == "" ? "NULL" : "'" + newItem.DateOfBirth + "'") + "," +
                    (newItem.IdFamilyStatus == 0 ? "NULL" : newItem.IdFamilyStatus.ToString()) +
                    ");");

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
