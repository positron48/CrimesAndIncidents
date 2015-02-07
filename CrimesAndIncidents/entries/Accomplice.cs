using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrimesAndIncidents
{
    public class AccompliceList
    {
        public ObservableCollection<Accomplice> values;

        public AccompliceList()
        {
            values = new ObservableCollection<Accomplice>();
        }

        public AccompliceList(ObservableCollection<Accomplice> _values)
        {
            values = _values;
        }

        public void deleteById(int id)
        {
            for (int i = 0; i < values.Count; i++)
                if (values[i].Id == id)
                    values.RemoveAt(i);
        }

        internal void update(Accomplice s)
        {
            for (int i = 0; i < values.Count; i++)
                if (values[i].Id == s.Id)
                    values[i] = s;
        }
    }

    public class Accomplice
    {
        public int Id { get; set; }
        public int IdPost { get; set; }
        public int IdRank { get; set; }
        public int IdSubUnit { get; set; }
        public int IdDraft { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public bool IsContrakt { get; set; }
        public bool IsMedic { get; set; }
        public int NumberContrakt { get; set; }
        public string DateOfFirstContrakt { get; set; }
        public string DateOfLastContrakt { get; set; }
        public int IdEducation { get; set; }
        public bool Sex { get; set; }
        public string DateOfBirth { get; set; }
        public int IdFamilyStatus { get; set; }

        public string Rank { get; set; }
        public string SubUnit { get; set; }
        public string MilitaryUnit { get; set; }

        public Accomplice(
            int id,
            int idPost,
            int idRank,
            int idSubUnit,
            int idDraft,
            string fullName,
            string shortName,
            bool isContrakt,
            bool isMedic,
            int numberContrakt,
            string dateOfFirstContrakt,
            string dateOfLastContrakt,
            int idEducation,
            bool sex,
            string dateOfBirth,
            int idFamilyStatus)
        {
            Id = id;
            IdPost = idPost;
            IdRank = idRank;
            IdSubUnit = idSubUnit;
            IdDraft = idDraft;
            FullName = fullName;
            ShortName = shortName;
            IsContrakt = isContrakt;
            IsMedic = isMedic;
            NumberContrakt = numberContrakt;
            DateOfFirstContrakt = dateOfFirstContrakt;
            DateOfLastContrakt = dateOfLastContrakt;
            IdEducation = idEducation;
            Sex = sex;
            DateOfBirth = dateOfBirth;
            IdFamilyStatus = idFamilyStatus;
        }

        public Accomplice(
            int id,
            int idPost,
            int idRank,
            int idSubUnit,
            int idDraft,
            string fullName,
            string shortName,
            bool isContrakt,
            bool isMedic,
            int numberContrakt,
            string dateOfFirstContrakt,
            string dateOfLastContrakt,
            int idEducation,
            bool sex,
            string dateOfBirth,
            int idFamilyStatus,
            string rank,
            string subUnit,
            string militaryUnit)
        {
            Id = id;
            IdPost = idPost;
            IdRank = idRank;
            IdSubUnit = idSubUnit;
            IdDraft = idDraft;
            FullName = fullName;
            ShortName = shortName;
            IsContrakt = isContrakt;
            IsMedic = isMedic;
            NumberContrakt = numberContrakt;
            DateOfFirstContrakt = dateOfFirstContrakt;
            DateOfLastContrakt = dateOfLastContrakt;
            IdEducation = idEducation;
            Sex = sex;
            DateOfBirth = dateOfBirth;
            IdFamilyStatus = idFamilyStatus;

            Rank = rank;
            SubUnit = subUnit;
            MilitaryUnit = militaryUnit;
        }
    }
}
