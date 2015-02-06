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
    /// Логика взаимодействия для addMilitaryUnit.xaml
    /// </summary>
    public partial class addMilitaryUnit : Window
    {
        MilitaryUnit mUnit;

        public addMilitaryUnit()
        {
            InitializeComponent();

            txFullName.Focus();
        }

        public addMilitaryUnit(MilitaryUnit m)
        {
            InitializeComponent();

            txFullName.Focus();

            mUnit = m;
            txFullName.Text = m.FullName;
            txName.Text = m.Name;
            txShortName.Text = m.ShortName;
            txNumber.Text = m.Number;
            txQuantity.Text = m.Quantity.ToString();
        }

        internal static MilitaryUnit getNewMilitaryUnit()
        {
            addMilitaryUnit wndM = new addMilitaryUnit();
            wndM.ShowDialog();
            return wndM.mUnit;
        }

        public static MilitaryUnit getEditedMilitaryUnit(MilitaryUnit m)
        {
            addMilitaryUnit wndM = new addMilitaryUnit(m);
            wndM.ShowDialog();
            return wndM.mUnit;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (txName.Text != "" && txShortName.Text != "" && txNumber.Text != "")
            {
                if (mUnit == null)
                {
                    mUnit = new MilitaryUnit(
                        0,
                        txFullName.Text,
                        txName.Text,
                        txShortName.Text,
                        txNumber.Text,
                        txQuantity.Text == "" ? 0 : Int32.Parse(txQuantity.Text),
                        1);
                }
                else
                {
                    mUnit.FullName = txFullName.Text;
                    mUnit.Name = txName.Text;
                    mUnit.ShortName = txShortName.Text;
                    mUnit.Number = txNumber.Text;
                    mUnit.Quantity = txQuantity.Text == "" ? 0 : Int32.Parse(txQuantity.Text);
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Не заполнены поля обязательные поля!");
            }
        }
    }
}
