﻿using System;
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
    }
}
