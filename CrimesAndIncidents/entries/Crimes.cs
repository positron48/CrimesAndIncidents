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
            string clause)
        {
            IdOrgan = idOrgan;
            IdClause = idClause;
            IdMilitaryUnit = idMilitaryUnit;
            DateRegistration = dateRegistration;
            DateInstitution = dateInstitution;
            DateCommit = dateCommit;
            Story = story;
            Damage = damage;
            DateVerdict = dateVerdict;
            Verdict = verdict;
            NumberCase = numberCase;
            
            Accomplice = accomplice;
            Clause = clause;
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

        public string DateRegistration {
            get { return dateRegistration.Year == 9999 ? "" : dateRegistration.ToString("dd.MM.yyyy"); }
            set { dateRegistration = value != "" ? DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture) : new DateTime(9999, 1, 1); }
        }
        public string DateInstitution {
            get { return dateInstitution.Year == 9999 ? "" : dateInstitution.ToString("dd.MM.yyyy"); }
            set { dateInstitution = value != "" ? DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture) : new DateTime(9999, 1, 1); }
        }
        public string DateCommit {
            get { return dateCommit.Year == 9999 ? "" : dateCommit.ToString("dd.MM.yyyy"); }
            set { dateCommit = value != "" ? DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture) : new DateTime(9999, 1, 1); }
        }
        public string DateVerdict {
            get { return dateVerdict.Year == 9999 ? "" : dateVerdict.ToString("dd.MM.yyyy"); }
            set { dateVerdict = value != "" ? DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture) : new DateTime(9999,1,1); }
        }

        //ужасный костыль для работы сортировки
        public DateTime dateRegistration { get; set; }
        public DateTime dateInstitution { get; set; }
        public DateTime dateCommit { get; set; }
        public DateTime dateVerdict { get; set; }
        
        //для отображения в списке
        public string Accomplice { get; set; }
        public string Clause { get; set; }
    }
}