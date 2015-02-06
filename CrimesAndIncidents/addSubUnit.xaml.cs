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
    /// Логика взаимодействия для addSubUnit.xaml
    /// </summary>
    public partial class addSubUnit : Window
    {
        SubUnit subUnit;

        int idMilitaryUnit;
        int idFKSubUnit;

        public addSubUnit(int _idMilitaryUnit, int _idFKSubUnit)
        {
            InitializeComponent();

            txName.Focus();

            idMilitaryUnit = _idMilitaryUnit;
            idFKSubUnit = _idFKSubUnit;
        }

        public addSubUnit(SubUnit s)
        {
            InitializeComponent();

            txName.Focus();

            subUnit = s;
        }
        
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (txName.Text != "" && txShortName.Text != "")
            {
                if (subUnit == null)
                {
                    subUnit = new SubUnit(
                        0,
                        txName.Text,
                        txShortName.Text,
                        txQuantity.Text == "" ? 0 : Int32.Parse(txQuantity.Text),
                        idFKSubUnit,
                        idMilitaryUnit);
                }
                else
                {
                    subUnit.Name = txName.Text;
                    subUnit.ShortName = txShortName.Text;
                    subUnit.Quantity = txQuantity.Text == "" ? 0 : Int32.Parse(txQuantity.Text);
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Не заполнены поля полноги и(или) краткого наименования!");
            }
        }

        public static SubUnit getNewSubUnit(int _idMilitaryUnit, int _idFKSubUnit)
        {
            addSubUnit wndS = new addSubUnit(_idMilitaryUnit, _idFKSubUnit);
            wndS.ShowDialog();
            return wndS.subUnit;
        }

        public static SubUnit getEditedSubUnit(SubUnit s)
        {
            addSubUnit wndS = new addSubUnit(s);
            wndS.ShowDialog();
            return wndS.subUnit;
        }
    }
}
