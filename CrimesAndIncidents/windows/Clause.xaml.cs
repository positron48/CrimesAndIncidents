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
    /// Логика взаимодействия для AddClause.xaml
    /// </summary>
    public partial class AddClause : Window
    {
        public Clause clause = null;

        public AddClause()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.CanMinimize;
            txDescription.Focus(); 
        }

        public static Clause getClause()
        {
            AddClause wndC = new AddClause();
            wndC.ShowDialog();
            return wndC.clause;
        }

        private void btnAddClause_Click(object sender, RoutedEventArgs e)
        {
            string notFilled = "";
            if (txNumber.Text == "")
                notFilled += "номер статьи";
            if (txDescription.Text == "")
                notFilled += notFilled == "" ? "Описание статьи" : "\nописание статьи";
            if (notFilled != "")
            {
                MessageBox.Show("Не все обязательные поля заполнены:\n" + notFilled);
            }
            else
            {
                clause = new Clause(0, txPoint.Text, txPart.Text, txNumber.Text, txDescription.Text);
                this.Close();
            }
        }
    }
}
