using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimesAndIncidents
{
    public class MilitaryUnitList
    {
        public ObservableCollection<MilitaryUnit> values;

        public MilitaryUnitList(ObservableCollection<MilitaryUnit> _list)
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

    public class MilitaryUnit
    {
        public MilitaryUnit(int id, string fullName, string name, string shortName, string number, int quantity, int idDivision)
        {
            Id = id;
            FullName = fullName;
            Name = name;
            ShortName = shortName;
            Number = number;
            Quantity = quantity;
            IdDivision = idDivision;
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Number { get; set; }
        public int Quantity { get; set; }
        public int IdDivision { get; set; }
        public override string ToString() { return ShortName; }
    }
}
