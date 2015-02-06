using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimesAndIncidents
{
    public class SubUnitList
    {
        public ObservableCollection<SubUnit> values;

        //пустой список
        public SubUnitList()
        {
            values = new ObservableCollection<SubUnit>();
        }

        public SubUnitList(ObservableCollection<SubUnit> _list)
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

    public class SubUnit
    {
        public SubUnit(int id, string name, string shortName, int quantity, int idFKSubUnit, int idMilitaryUnit)
        {
            Id = id;
            Name = name;
            ShortName = shortName;
            Quantity = quantity;
            IdFKSubUnit = idFKSubUnit;
            IdMilitaryUnit = idMilitaryUnit;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int Quantity { get; set; }
        public int IdFKSubUnit { get; set; }
        public int IdMilitaryUnit { get; set; }

        public override string ToString() { return ShortName; }
    }
}
