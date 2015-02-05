using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimesAndIncidents
{
    public class BattalionList
    {
        public ObservableCollection<Battalion> values;

        public BattalionList(ObservableCollection<Battalion> _list)
        {
            values = _list;
        }

        public int newId()
        {
            int i = 0;
            foreach (Battalion b in values)
                if (b.Id > i)
                    i = b.Id;
            return i + 1;
        }

        public void deleteById(int id)
        {
            for (int i = 0; i < values.Count; i++)
                if (values[i].Id == id)
                    values.RemoveAt(i);
        }
    }

    public class Battalion
    {
        public Battalion(int id, string name, string shortName, int quantity, int idMilitaryUnit)
        {
            Id = id;
            Name = name;
            ShortName = shortName;
            Quantity = quantity;
            IdMilitaryUnit = idMilitaryUnit;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int Quantity { get; set; }
        public int IdMilitaryUnit { get; set; }

        public override string ToString() { return ShortName; }
    }
}
