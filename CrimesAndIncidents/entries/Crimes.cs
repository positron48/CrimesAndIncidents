using System;
using System.Collections.Generic;
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
        public string DateRegistration { get; set; }
        public string DateInstitution { get; set; }
        public string DateCommit { get; set; }
        public string Story { get; set; }
        public string Damage { get; set; }
        public string DateVerdict { get; set; }
        public string Verdict { get; set; }
        public string NumberCase { get; set; }
        
        //для отображения в списке
        public string Accomplice { get; set; }
        public string Clause { get; set; }
    }
}
