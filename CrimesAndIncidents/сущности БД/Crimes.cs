using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimesAndIncidents
{
    class Crime
    {
        public Crime(string story, string dateCommit, string dateInstitution, string dateRegistration,
            string rank, string accomplice, string point, string part, string clause, string description, 
            string dateVerdict, string verdict)
        {
            Story = story;
            DateCommit = dateCommit;
            DateInstitution = dateInstitution;
            DateRegistration = dateRegistration;
            Rank = rank;
            Accomplice = accomplice;
            Clause = (point == "" ? "" : "п.'" + point + "' ") +
                (part == "" ? "" : "ч." + part + " ") +
                (clause == "" ? "" : "ст." + clause + " ") +
                (description == "" ? "" : " (" + description + ")");
            DateVerdict = dateVerdict;
            Verdict = verdict;
        }

        public string Story { get; set; }
        public string DateCommit { get; set; }
        public string DateInstitution { get; set; }
        public string DateRegistration { get; set; }
        public string Rank { get; set; }
        public string Accomplice { get; set; }
        public string Clause { get; set; }
        public string DateVerdict { get; set; }
        public string Verdict { get; set; }
    }
}
