using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimesAndIncidents
{
    public class DBList
    {
        public string TableName { get; set; }
        public ObservableCollection<KeyValue> values;

        public DBList(string tableName, ObservableCollection<KeyValue> t)
        {
            TableName = tableName;
            values = t;
            
        }
    }

    public class KeyValue
    {
        public int Key { get; set; }
        public string Value { get; set; }
        public KeyValue(int k, string v)
        {
            Key = k;
            Value = v;
        }
    }
}
