using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimesAndIncidents
{
    public class Crime
    {
        public Crime(
            int idOrgan,
            int idClause,
            int idMilitaryUnit,
            string dateRegistration,
            string dateInstitution,
            string dateCommit,
            string story,
            string damage,
            string dateVerdict,
            string verdict,
            string numberCase,
            string accomplice,
            string clause,
            string numberClause,
            int isRegistred,
            string militaryUnit)
        {
            IdOrgan = idOrgan;
            IdClause = idClause;
            IdMilitaryUnit = idMilitaryUnit;
            DateRegistration = dateRegistration.IndexOf('.') > 2 ? 
                dateRegistration : 
                DateTime.ParseExact(dateRegistration, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy.MM.dd");
            DateInstitution = dateInstitution;
            DateCommit = dateCommit;
            Story = story;
            Damage = damage;
            DateVerdict = dateVerdict;
            Verdict = verdict;
            NumberCase = numberCase;
            
            Accomplice = accomplice;
            Clause = clause;
            string t = numberClause.Replace('.', ',');
            if (t.IndexOf(',') != t.LastIndexOf(','))
                t = t.Remove(t.LastIndexOf(','), 1);
            NumberClause = Double.Parse(t);

            IsRegistred = isRegistred == 1 ? true : false;

            MilitaryUnit = militaryUnit;
        }

        //в бд
        public int Id { get; set; }
        public int IdOrgan { get; set; }
        public int IdClause { get; set; }
        public int IdMilitaryUnit { get; set; }
        public string Story { get; set; }
        public string Damage { get; set; }
        public string Verdict { get; set; }
        public string NumberCase { get; set; }
        public bool IsRegistred { get; set; }

        public string DateRegistration { get; set; }
        public string DateInstitution { get; set; }
        public string DateCommit { get; set; }
        public string DateVerdict { get; set; }

        //для отображения в списке
        public string Accomplice { get; set; }
        public string Clause { get; set; }
        public double NumberClause { get; set; }
        public string MilitaryUnit { get; set; }
    }
}