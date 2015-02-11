using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimesAndIncidents
{
    public class ClauseList
    {
        public ObservableCollection<Clause> values;

        public ClauseList(ObservableCollection<Clause> _list)
        {
            values = _list;
        }

        public void deleteById(int id)
        {
            for (int i = 0; i < values.Count; i++)
                if (values[i].Id == id)
                    values.RemoveAt(i);
        }
    }

    public class Clause
    {
        public bool isChanged = false;

        public int Id { get; set; }

        public override string ToString()
        {
            return (point == "" || point == null ? "" : "п.'" + point + "' ") +
                    (part == "" || part == null ? "" : "ч." + part + " ") +
                    (number == "" || number == null ? "" : "ст." + number + " ") +
                    (description == "" || description == null ? "" : " (" + description + ")");
        }
        
        public string Point
        { 
            get { return point; }
            set
            {
                if (value != "" && point != value)
                {
                    if (point != "")
                        isChanged = true;
                    point = value;
                }
            }  
        }
        
        public string Part 
        { 
            get { return part; }
            set
            {
                if (value != "" && part != value)
                {
                    if (part != "")
                        isChanged = true;
                    part = value;
                }
            } 
        }
        
        public string Number
        { 
            get { return number; }
            set
            {
                if (value != "" && number != value)
                {
                    if (number != "")
                        isChanged = true;
                    number = value;
                }
            } 
        }
        
        public string Description 
        { 
            get { return description; }
            set
            {
                if (value != "" && description != value)
                {
                    if (description != "")
                        isChanged = true;
                    description = value;
                }
            }  
        }

        private string point;
        private string part;
        private string number;
        private string description;
        
        public Clause(int _id, string _point, string _part, string _number, string _description)
        {
            Id = _id;
            Point = _point;
            Part = _part;
            Number = _number;
            Description = _description;
        }
    }
}
