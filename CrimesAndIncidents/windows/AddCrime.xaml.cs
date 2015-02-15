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
    /// Логика взаимодействия для AddCrime.xaml
    /// </summary>
    public partial class AddCrime : Window
    {
        SqliteWorker sqlWorker;

        AccompliceList accompliceList;
        MilitaryUnitList militaryList;
        ClauseList clauseList;
        DBList organList;
        DBList categoryList;

        Crime c = null;

        bool isEditing = false;

        public AddCrime()
        {
            InitializeComponent();
        }

        public AddCrime(SqliteWorker _sqlWorker, Crime _c = null)
        {
            InitializeComponent();

            sqlWorker = _sqlWorker;

            organList = new DBList("Organ", DataWorker.getList(sqlWorker.selectData("SELECT * FROM Organ ORDER BY description")));
            categoryList = new DBList("Category", DataWorker.getList(sqlWorker.selectData("SELECT * FROM Category ORDER BY description")));
            militaryList = new MilitaryUnitList(DataWorker.getMilitaryUnitList(sqlWorker.selectData("SELECT * FROM MilitaryUnit")));
            clauseList = new ClauseList(DataWorker.getClauseList(sqlWorker.selectData("SELECT * FROM Clause ORDER BY number, part, point")));

            cbOrgan.ItemsSource = organList.values;
            cbMilitaryUnit.ItemsSource = militaryList.values;
            cbClause.ItemsSource = clauseList.values;
            lbCategoty.ItemsSource = categoryList.values;

            if (_c != null)
            {
                isEditing = true;

                c = _c;
                txDamage.Text = c.Damage;
                txDateCommit.Text = c.DateCommit;
                txDateInstitution.Text = c.DateInstitution;
                txDateRegistration.Text = c.DateRegistration;
                txDateVerdict.Text = c.DateVerdict;
                txnumberCase.Text = c.NumberCase;
                txStory.Text = c.Story;
                txVerdict.Text = c.Verdict;

                for (int i = 0; i < cbOrgan.Items.Count; i++)
                    if ((cbOrgan.Items[i] as KeyValue).Key == c.IdOrgan)
                        cbOrgan.SelectedIndex = i;

                for (int i = 0; i < cbClause.Items.Count; i++)
                    if ((cbClause.Items[i] as Clause).Id == c.IdClause)
                        cbClause.SelectedIndex = i;

                for (int i = 0; i < cbMilitaryUnit.Items.Count; i++)
                    if ((cbMilitaryUnit.Items[i] as MilitaryUnit).Id == c.IdMilitaryUnit)
                        cbMilitaryUnit.SelectedIndex = i;

                accompliceList = new AccompliceList(
                DataWorker.getAccompliceList(
                    sqlWorker.selectData("SELECT R.shortName as rank, S.shortName as subUnit, SF.shortName as battalion, M.shortName as militaryUnit, A.* " +
                        "FROM Accomplice A " +
                        "INNER JOIN SubUnit S ON S.idSubUnit = A.idSubUnit " +
                        "INNER JOIN Portaking P ON P.idAccomplice = A.idAccomplice " +
                        "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                        "LEFT JOIN SubUnit SF ON S.idFKSubUnit = SF.idSubUnit " +
                        "LEFT JOIN MilitaryUnit M ON M.idMilitaryUnit = S.idMilitaryUnit OR M.idMilitaryUnit = SF.idMilitaryUnit " + 
                        "WHERE P.idCrime = " + c.Id)));

                lbAccomplice.ItemsSource = accompliceList.values;

                //categoryList
                DBList categoryUsed = new DBList("", DataWorker.getList(sqlWorker.selectData("SELECT * FROM InCategory WHERE idCrime = " + c.Id + ";")));
                for (int i = 0; i < categoryUsed.values.Count; i++)
                    for (int j = 0; j < categoryList.values.Count; j++)
                        if (categoryUsed.values[i].Key == categoryList.values[j].Key)
                            categoryList.values[j].IsCheked = true;
            }
        }

        internal static Crime gtNewCrime(SqliteWorker sqlWorker, Crime _c = null)
        {
            AddCrime wndCrime = new AddCrime(sqlWorker, _c);
            wndCrime.ShowDialog();
            return wndCrime.c;
        }

        private void btnAddClause_Click(object sender, RoutedEventArgs e)
        {
            Clause newItem = AddClause.getClause();
            if (newItem != null)
            {
                //добавить в БД
                int id = sqlWorker.getNewId("Clause");
                if (sqlWorker.addClause(id, newItem))
                {
                    newItem.Id = id;
                    //если успешное добавление в БД
                    clauseList.values.Add(newItem);
                    cbClause.Items.Refresh();
                    cbClause.SelectedIndex = cbClause.Items.Count - 1;
                }
                else
                    MessageBox.Show("Ошибка при добавлении данных");
            }
        }

        private void btnAddOrgan_Click(object sender, RoutedEventArgs e)
        {
            string newItem = InputBox.input("Введите кем возбуждено дело");
            if (newItem != "")
            {
                int newId = sqlWorker.getNewId("Organ");
                //добавить в БД
                if (sqlWorker.addInDBList("Organ", newId, newItem))
                {
                    //если успешное добавление в БД
                    organList.values.Add(new KeyValue(newId, newItem));
                    cbOrgan.Items.Refresh();
                }
                else
                    MessageBox.Show("Ошибка при добавлении данных");
            }
        }

        private void btnAddAccomplice_Click(object sender, RoutedEventArgs e)
        {
            accompliceList = SelectAccomplice.getList(sqlWorker, accompliceList);
            lbAccomplice.ItemsSource = accompliceList.values;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            int newId = sqlWorker.getNewId("Crime");
            

            if (rbCrime.IsChecked == true &&
                cbClause.SelectedItem != null &&
                cbMilitaryUnit.SelectedItem != null &&
                txStory.Text != "" &&
                txDateRegistration.Text != "")
            {
                //заполнен минимум для преступления
                string accomplices = "";
                if (accompliceList != null)
                    for (int i = 0; i < accompliceList.values.Count; i++)
                        accomplices += (i == 0 ? "" : "\n") + accompliceList.values[i].Rank + " " + accompliceList.values[i].ShortName;

                if (isEditing) newId = c.Id;

                c = new Crime(
                    cbOrgan.SelectedItem==null?0:(cbOrgan.SelectedItem as KeyValue).Key,
                    cbClause.SelectedItem==null?0:(cbClause.SelectedItem as Clause).Id,
                    cbMilitaryUnit.SelectedItem == null ? 0 : (cbMilitaryUnit.SelectedItem as MilitaryUnit).Id,
                    txDateRegistration.Text,
                    txDateInstitution.Text,
                    txDateCommit.Text,
                    txStory.Text,
                    txDamage.Text,
                    txDateVerdict.Text,
                    txVerdict.Text,
                    txnumberCase.Text,
                    accomplices,
                    cbClause.SelectedItem == null ? "" : (cbClause.SelectedItem as Clause).ToString());
            }
            else if (rbCrime.IsChecked == false &&
                cbMilitaryUnit.SelectedItem != null &&
                txStory.Text != "" &&
                txDateRegistration.Text != "")
            {
                //введен минимум для происшествия
                string accomplices = "";
                for (int i = 0; i < accompliceList.values.Count; i++)
                    accomplices += (i == 0 ? "" : "\n") + accompliceList.values[i].Rank + " " + accompliceList.values[i].ShortName;
                
                if (isEditing) newId = c.Id;

                c = new Crime(
                    cbOrgan.SelectedItem == null ? 0 : (cbOrgan.SelectedItem as KeyValue).Key,
                    cbClause.SelectedItem == null ? 0 : (cbClause.SelectedItem as Clause).Id,
                    cbMilitaryUnit.SelectedItem == null ? 0 : (cbMilitaryUnit.SelectedItem as MilitaryUnit).Id,
                    txDateRegistration.Text,
                    txDateInstitution.Text,
                    txDateCommit.Text,
                    txStory.Text,
                    txDamage.Text,
                    txDateVerdict.Text,
                    txVerdict.Text,
                    txnumberCase.Text,
                    accomplices,
                    cbClause.SelectedItem == null ? "" : (cbClause.SelectedItem as Clause).ToString());
            }
            else
            {
                MessageBox.Show("Не все обязательные поля заполнены!");
                return;
            }
            //похоже единственный случай когда изменения в бд нужно производить не из главного окна
            //т.к. нужно вносить изменения в 2 дополнительные таблицы - Portaking и InCategory
            c.Id = newId;

            if (!isEditing && sqlWorker.addCrime(c, accompliceList, categoryList))
                this.Close();
            else if (isEditing && sqlWorker.updateCrime(c, accompliceList, categoryList))
                this.Close();
            else
                MessageBox.Show("Ошибка при добавлении преступления в базу данных");
        }

        private void rbCrime_Checked(object sender, RoutedEventArgs e)
        {
            if (rowClause.Height.Value == 0)
            {
                rowClause.Height = new GridLength(90);
                rowDateInstitution.Height = new GridLength(30);
                rowVerdict.Height = new GridLength(60);
                Height += 180;
            }
        }

        private void rbIncident_Checked(object sender, RoutedEventArgs e)
        {
            if (rowClause.Height.Value == 90)
            {
                rowClause.Height = new GridLength(0);
                rowDateInstitution.Height = new GridLength(0);
                rowVerdict.Height = new GridLength(0);
                Height -= 180;
            }
        }
    }
}
