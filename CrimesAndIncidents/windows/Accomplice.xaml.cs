using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CrimesAndIncidents
{
    /// <summary>
    /// Логика взаимодействия для Accomplice.xaml
    /// </summary>
    public partial class AddAccomplice : Window
    {
        DBList postList;
        DBList draftList;
        DBList educationList;
        DBList familyStatusList;

        RankList rankList;
        MilitaryUnitList militaryList;
        SubUnitList battalionList;
        SubUnitList subUnitList;

        SqliteWorker sqlWorker;
        Accomplice a;

        int idA = 0;

        public AddAccomplice(SqliteWorker _sqlWorker)
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.CanMinimize;
            
            sqlWorker = _sqlWorker;

            postList = new DBList("Post", DataWorker.getList(sqlWorker.selectData("SELECT * FROM Post")));
            draftList = new DBList("Draft", DataWorker.getList(sqlWorker.selectData("SELECT * FROM Draft")));
            educationList = new DBList("Education", DataWorker.getList(sqlWorker.selectData("SELECT * FROM Education")));
            familyStatusList = new DBList("FamilyStatus", DataWorker.getList(sqlWorker.selectData("SELECT * FROM FamilyStatus")));

            rankList = new RankList(DataWorker.getRankList(sqlWorker.selectData("SELECT * FROM Rank")));
            militaryList = new MilitaryUnitList(DataWorker.getMilitaryUnitList(sqlWorker.selectData("SELECT * FROM MilitaryUnit")));

            cbPost.ItemsSource = postList.values;
            cbDraft.ItemsSource = draftList.values;
            cbEducation.ItemsSource = educationList.values;
            cbFamilyStatus.ItemsSource = familyStatusList.values;

            cbRank.ItemsSource = rankList.values;
            cbMilitaryUnit.ItemsSource = militaryList.values;

            rowSubUnit.Height = new GridLength(0);
            rowContrakt.Height = new GridLength(0);
            this.Height -= 150;
        }

        public AddAccomplice(Accomplice accomplice, SqliteWorker _sqlWorker)
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.CanMinimize;

            sqlWorker = _sqlWorker;

            postList = new DBList("Post", DataWorker.getList(sqlWorker.selectData("SELECT * FROM Post")));
            draftList = new DBList("Draft", DataWorker.getList(sqlWorker.selectData("SELECT * FROM Draft")));
            educationList = new DBList("Education", DataWorker.getList(sqlWorker.selectData("SELECT * FROM Education")));
            familyStatusList = new DBList("FamilyStatus", DataWorker.getList(sqlWorker.selectData("SELECT * FROM FamilyStatus")));

            rankList = new RankList(DataWorker.getRankList(sqlWorker.selectData("SELECT * FROM Rank")));
            militaryList = new MilitaryUnitList(DataWorker.getMilitaryUnitList(sqlWorker.selectData("SELECT * FROM MilitaryUnit")));

            cbPost.ItemsSource = postList.values;
            cbDraft.ItemsSource = draftList.values;
            cbEducation.ItemsSource = educationList.values;
            cbFamilyStatus.ItemsSource = familyStatusList.values;

            cbRank.ItemsSource = rankList.values;
            cbMilitaryUnit.ItemsSource = militaryList.values;

            rowSubUnit.Height = new GridLength(0);
            rowContrakt.Height = new GridLength(0);
            this.Height -= 150;

            txFullName.Text = accomplice.FullName;
            txName.Text = accomplice.ShortName;
            txNumberContrakt.Text = (accomplice.NumberContrakt == 0 ? "" : accomplice.NumberContrakt.ToString());
            txDateOfBirthday.Text = accomplice.DateOfBirth;
            txDateFirstContrakt.Text = accomplice.DateOfFirstContrakt;
            txDateLastContrakt.Text = accomplice.DateOfLastContrakt;

            chkbContrakt.IsChecked = accomplice.IsContrakt;
            chkbMedic.IsChecked = accomplice.IsMedic;

            rSex.IsChecked = accomplice.Sex;
            fSex.IsChecked = !accomplice.Sex;

            for (int i = 0; i < cbPost.Items.Count; i++)
                if ((cbPost.Items[i] as KeyValue).Key == accomplice.IdPost)
                    cbPost.SelectedIndex = i;

            for (int i = 0; i < cbDraft.Items.Count; i++)
                if ((cbDraft.Items[i] as KeyValue).Key == accomplice.IdDraft)
                    cbDraft.SelectedIndex = i;

            for (int i = 0; i < cbEducation.Items.Count; i++)
                if ((cbEducation.Items[i] as KeyValue).Key == accomplice.IdEducation)
                    cbEducation.SelectedIndex = i;

            for (int i = 0; i < cbFamilyStatus.Items.Count; i++)
                if ((cbFamilyStatus.Items[i] as KeyValue).Key == accomplice.IdFamilyStatus)
                    cbFamilyStatus.SelectedIndex = i;

            for (int i = 0; i < cbRank.Items.Count; i++)
                if ((cbRank.Items[i] as Rank).Id == accomplice.IdRank)
                    cbRank.SelectedIndex = i;

            //получение id воинской части и выбор этой части в комбобоксе
            DataTable dt = sqlWorker.selectData("SELECT M.idMilitaryUnit FROM SubUnit S " +
                "LEFT JOIN SubUnit SF ON S.idFKSubUnit = SF.idSubUnit " +
                "LEFT JOIN MilitaryUnit M ON S.idMilitaryUnit = M.idMilitaryUnit OR SF.idMilitaryUnit = M.idMilitaryUnit " +
                "WHERE S.idSubUnit = " + accomplice.IdSubUnit);
            int idT = Int32.Parse(dt.Rows[0][0].ToString());
            for (int i = 0; i < cbMilitaryUnit.Items.Count; i++)
                if ((cbMilitaryUnit.Items[i] as MilitaryUnit).Id == idT)
                    cbMilitaryUnit.SelectedIndex = i;

            dt = sqlWorker.selectData("SELECT SF.idSubUnit FROM SubUnit S " +
                "LEFT JOIN SubUnit SF ON S.idFKSubUnit = SF.idSubUnit " +
                "WHERE S.idSubUnit = " + accomplice.IdSubUnit);
            if (dt.Rows[0][0].ToString() != "")
            {
                //есть промежуточное подразделение (батальон)
                idT = Int32.Parse(dt.Rows[0][0].ToString());
                for (int i = 0; i < cbBattalion.Items.Count; i++)
                    if ((cbBattalion.Items[i] as SubUnit).Id == idT)
                        cbBattalion.SelectedIndex = i;
            }
            else
            {
                //подразделение непосредственного подчинения воинской части
                for (int i = 0; i < cbBattalion.Items.Count; i++)
                    if ((cbBattalion.Items[i] as SubUnit).Id == accomplice.IdSubUnit)
                        cbBattalion.SelectedIndex = i;
            }

            idA = accomplice.Id;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (txName.Text != "" &&
                cbRank.SelectedItem != null &&
                cbPost.SelectedItem != null &&
                cbBattalion.SelectedItem != null)
            {
                a = new Accomplice(idA,
                    (cbPost.SelectedItem as KeyValue).Key,
                    (cbRank.SelectedItem as Rank).Id,
                    cbSubUnit.SelectedItem != null ? (cbSubUnit.SelectedItem as SubUnit).Id : (cbBattalion.SelectedItem as SubUnit).Id,
                    (cbDraft.SelectedItem != null && chkbContrakt.IsChecked == true) ? (cbDraft.SelectedItem as KeyValue).Key : 0,
                    txFullName.Text,
                    txName.Text,
                    chkbContrakt.IsChecked == true,
                    chkbMedic.IsChecked == true,
                    chkbContrakt.IsChecked == true && txNumberContrakt.Text != "" ? Int32.Parse(txNumberContrakt.Text) : 0,
                    txDateFirstContrakt.Text,
                    txDateLastContrakt.Text,
                    (cbEducation.SelectedItem != null) ? (cbEducation.SelectedItem as KeyValue).Key : 0,
                    rSex.IsChecked == true,
                    txDateOfBirthday.Text,
                    (cbFamilyStatus.SelectedItem != null) ? (cbFamilyStatus.SelectedItem as KeyValue).Key : 0,
                    (cbRank.SelectedItem as Rank).ShortName,
                    cbSubUnit.SelectedItem != null ? (cbSubUnit.SelectedItem as SubUnit).ShortName + " " + (cbBattalion.SelectedItem as SubUnit).ShortName : (cbBattalion.SelectedItem as SubUnit).ShortName,
                    (cbMilitaryUnit.SelectedItem as MilitaryUnit).ShortName);

                this.Close();
            }
            else
            {
                MessageBox.Show("Не все обязательные поля заполнены!");
            }
        }

        internal static Accomplice getAccomplice(SqliteWorker _sqlWorker)
        {
            AddAccomplice wndA = new AddAccomplice(_sqlWorker);
            wndA.ShowDialog();
            return wndA.a;
        }

        private void cbMilitaryUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbSubUnit.IsEnabled = false;
            if (rowSubUnit.Height.Value > 0)
            {
                rowSubUnit.Height = new GridLength(0);
                this.Height -= 30;
            }
            if (subUnitList != null) subUnitList.values.Clear();

            battalionList = new SubUnitList(DataWorker.getSubUnitList(sqlWorker.selectData("SELECT * FROM SubUnit WHERE idMilitaryUnit = " + (cbMilitaryUnit.SelectedItem as MilitaryUnit).Id)));
            cbBattalion.ItemsSource = battalionList.values;
            cbBattalion.IsEnabled = true;
        }

        private void cbBattalion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbBattalion.SelectedItem != null)
            {
                subUnitList = new SubUnitList(DataWorker.getSubUnitList(sqlWorker.selectData("SELECT * FROM SubUnit WHERE idFKSubUnit = " + (cbBattalion.SelectedItem as SubUnit).Id)));
                cbSubUnit.ItemsSource = subUnitList.values;
                if (subUnitList != null && subUnitList.values.Count != 0)
                {
                    cbSubUnit.IsEnabled = true;
                    if (rowSubUnit.Height.Value == 0)
                    {
                        rowSubUnit.Height = new GridLength(30);
                        this.Height += 30;
                    }
                }
                else
                {
                    cbSubUnit.IsEnabled = false;
                    if (rowSubUnit.Height.Value > 0)
                    {
                        rowSubUnit.Height = new GridLength(0);
                        this.Height -= 30;
                    }
                }
            }
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            rowContrakt.Height = new GridLength(120);
            this.Height += 120;
        }

        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            rowContrakt.Height = new GridLength(0);
            this.Height -= 120;
        }

        internal static Accomplice getEditedAccomplice(Accomplice accomplice, SqliteWorker _sqlWorker)
        {
            AddAccomplice wndA = new AddAccomplice(accomplice, _sqlWorker);
            wndA.ShowDialog();
            return wndA.a;
        }
    }
}
