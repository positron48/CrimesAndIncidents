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
    /// Логика взаимодействия для editStructure.xaml
    /// </summary>
    public partial class EditStructure : Window
    {
        SqliteWorker sqlWorker;
        MilitaryUnitList mList;
        BattalionList bList;
        SubUnitList sList;

        public EditStructure(SqliteWorker _sqlWorker, MilitaryUnitList _mList)
        {
            InitializeComponent();

            sqlWorker = _sqlWorker;
            mList = _mList;

            lbMilitaryUnit.ItemsSource = mList.values;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lbMilitaryUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sList != null) sList.values.Clear();
            if (lbMilitaryUnit.SelectedItem != null)
            {
                int idMilitaryUnit = (lbMilitaryUnit.SelectedItem as MilitaryUnit).Id;
                bList = new BattalionList(DataWorker.getBattalionList(sqlWorker.selectData("SELECT * FROM Battalion WHERE idMilitaryUnit = " + idMilitaryUnit)));
                lbBattalion.ItemsSource = bList.values;
            }
        }

        private void lbBattalion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbBattalion.SelectedItem != null)
            {
                int idBattalion = (lbBattalion.SelectedItem as Battalion).Id;
                sList = new SubUnitList(DataWorker.getSubUnitList(sqlWorker.selectData("SELECT idSubUnit, name, shortName, quantity, idBattalion FROM SubUnit WHERE idBattalion = " + idBattalion)));
                lbSubUnit.ItemsSource = sList.values;
            }
        }
    }
}
