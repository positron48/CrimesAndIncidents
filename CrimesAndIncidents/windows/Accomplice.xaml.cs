using System;
using System.Collections.Generic;
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

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (txName.Text != "" &&
                cbRank.SelectedItem != null &&
                cbPost.SelectedItem != null &&
                cbBattalion.SelectedItem != null)
            {
                a = new Accomplice(0,
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
                    (cbRank.SelectedItem as Rank).FullName,
                    cbSubUnit.SelectedItem != null ? (cbSubUnit.SelectedItem as SubUnit).Name : (cbBattalion.SelectedItem as SubUnit).Name,
                    (cbMilitaryUnit.SelectedItem as MilitaryUnit).Name);

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
    }
}
