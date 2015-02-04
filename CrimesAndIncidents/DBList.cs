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

        public int newId()
        {
            int i=0;
            foreach(KeyValue k in values)
                if(k.Key > i)
                    i = k.Key;
            return i + 1;
        }

        public void deleteById(int p)
        {
            for (int i = 0; i < values.Count; i++)
                if (values[i].Key == p)
                    values.RemoveAt(i);
        }
    }

    public class KeyValue
    {
        public bool isChanged = false;
        public int Key { get; set; }
        private string _value="";

        public string Value 
        {
            get
            {
                return _value;
            }
            set
            {
                if (value != "" && _value != value)
                {
                    if(_value!="")
                        isChanged = true;
                    _value = value;
                }
            } 
        }

        public KeyValue(int k, string v)
        {
            Key = k;
            Value = v;
        }
    }
}
