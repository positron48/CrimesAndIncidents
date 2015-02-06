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
    /// Логика взаимодействия для AddRank.xaml
    /// </summary>
    public partial class AddRank : Window
    {
        public Rank rank = null;

        public AddRank()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.CanMinimize;
            txFullName.Focus();
        }

        public static Rank getRank()
        {
            AddRank wndR = new AddRank();
            wndR.ShowDialog();
            return wndR.rank;
        }

        private void btnAddClause_Click(object sender, RoutedEventArgs e)
        {
            string notFilled = "";
            if (txFullName.Text == "")
                notFilled += "полное наименование";
            if (txShortName.Text == "")
                notFilled += notFilled == "" ? "сокращенно" : "\nсокращенно";
            if (notFilled != "")
            {
                MessageBox.Show("Не все обязательные поля заполнены:\n" + notFilled);
            }
            else
            {
                rank = new Rank(0, txFullName.Text, txShortName.Text, txPriority.Text);
                this.Close();
            }
        }
    }
}
