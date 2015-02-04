using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimesAndIncidents
{
    public class RankList
    {
        public ObservableCollection<Rank> values;

        public RankList(ObservableCollection<Rank> _list)
        {
            values = _list;
        }

        public int newId()
        {
            int i = 0;
            foreach (Rank r in values)
                if (r.Id > i)
                    i = r.Id;
            return i + 1;
        }

        public void deleteById(int id)
        {
            for (int i = 0; i < values.Count; i++)
                if (values[i].Id == id)
                    values.RemoveAt(i);
        }
    }

    public class Rank
    {
        public bool isChanged = false;

        public int Id { get; set; }
        public string FullName
        {
            get { return fullName; }
            set
            {
                if (value != "" && fullName != value)
                {
                    if (fullName != "")
                        isChanged = true;
                    fullName = value;
                }
            }
        }
        public string ShortName
        {
            get { return shortName; }
            set
            {
                if (value != "" && shortName != value)
                {
                    if (shortName != "")
                        isChanged = true;
                    shortName = value;
                }
            }
        }
        public string Priority
        {
            get { return priority; }
            set
            {
                if (value != "" && priority != value)
                {
                    if (priority != "")
                        isChanged = true;
                    priority = value;
                }
            }
        }

        private string fullName;
        private string shortName;
        private string priority;

        public Rank(int _id, string _fullName, string _shortName, string _priority = "")
        {
            Id = _id;
            FullName = _fullName;
            ShortName = _shortName;
            Priority = _priority;
        }
    }
}
