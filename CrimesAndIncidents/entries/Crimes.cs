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
            string postAccomplice,
            string accomplice,
            string clause,
            string numberClause,
            int isRegistred,
            string militaryUnit)
        {
            IdOrgan = idOrgan;
            IdClause = idClause;
            IdMilitaryUnit = idMilitaryUnit;
            DateRegistration = dateRegistration == "" ? "" : (dateRegistration.IndexOf('.') > 2 ? 
                dateRegistration : 
                DateTime.ParseExact(dateRegistration, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy.MM.dd"));
            DateInstitution = dateInstitution == "" ? "" : (dateInstitution.IndexOf('.') > 2 ?
                dateInstitution :
                DateTime.ParseExact(dateInstitution, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy.MM.dd"));
            DateCommit = dateCommit == "" ? "" : (dateCommit.IndexOf('.') > 2 ?
                dateCommit :
                DateTime.ParseExact(dateCommit, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy.MM.dd")); ;
            Story = story;
            Damage = damage;
            DateVerdict = dateVerdict == "" ? "" : (dateVerdict.IndexOf('.') > 2 ?
                dateVerdict :
                DateTime.ParseExact(dateVerdict, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy.MM.dd")); ;
            Verdict = verdict;
            NumberCase = numberCase;
            
            Accomplice = accomplice;
            Clause = clause;
            string t = numberClause.Replace('.', ',');
            if (t.IndexOf(',') != t.LastIndexOf(','))
                t = t.Remove(t.LastIndexOf(','), 1);
            NumberClause = t==""?0:Double.Parse(t);

            IsRegistred = isRegistred == 1 ? true : false;

            MilitaryUnit = militaryUnit;

            PostAccomplice = postAccomplice;
        }

        //в бд
        public int Id { get; set; }
        public int IdOrgan { get; set; }
        public int IdClause { get; set; }
        public int IdMilitaryUnit { get; set; }
        public string Story { get; set; }           //фабула
        public string Damage { get; set; }          //-------------материальный ущерб
        public string Verdict { get; set; }         //-------------решение суда
        public string NumberCase { get; set; }      //-------------номер уголовного дела
        public bool IsRegistred { get; set; }       //учет

        public string DateRegistration { get; set; }//дата учета
        public string DateInstitution { get; set; } //дата возбуждения
        public string DateCommit { get; set; }      //дата совершения
        public string DateVerdict { get; set; }     //-------------дата суда

        //для отображения в списке
        public string Accomplice { get; set; }      //участники
        public string Clause { get; set; }          //статья   
        public double NumberClause { get; set; }    //номер статьи для сортировки
        public string MilitaryUnit { get; set; }    //вч
        public string PostAccomplice { get; set; }  //-------------должности
    }
}