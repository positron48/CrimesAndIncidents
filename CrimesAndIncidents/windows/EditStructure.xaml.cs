﻿using System;
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
        SubUnitList bList;
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
                bList = new SubUnitList(DataWorker.getSubUnitList(sqlWorker.selectData("SELECT * FROM SubUnit WHERE idMilitaryUnit = " + idMilitaryUnit + " ORDER BY name")));
                lbBattalion.ItemsSource = bList.values;
            }
        }

        private void lbBattalion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbBattalion.SelectedItem != null)
            {
                int idSubUnit = (lbBattalion.SelectedItem as SubUnit).Id;
                sList = new SubUnitList(DataWorker.getSubUnitList(sqlWorker.selectData("SELECT * FROM SubUnit WHERE idFKSubUnit = " + idSubUnit + " ORDER BY name")));
                lbSubUnit.ItemsSource = sList.values;
            }
        }

        private void btnDeleteSubUnit_Click(object sender, RoutedEventArgs e)
        {
            if (lbSubUnit.SelectedItem != null && MessageBox.Show("Вы действительно хотите удалить выбранное подразделение?", "Подтвердждение на удаление элемента", MessageBoxButton.YesNo)==MessageBoxResult.Yes)
            {
                int idSubUnit = (lbSubUnit.SelectedItem as SubUnit).Id;
                if (sqlWorker.deleteItemById("SubUnit", idSubUnit))
                {
                    sList.deleteById(idSubUnit);
                }
                else
                {
                    MessageBox.Show("При удалении элемента возникла ошибка, возможно, он уже используется");
                }
            }
        }

        private void btnDeleteBattalion_Click(object sender, RoutedEventArgs e)
        {
            if (lbBattalion.SelectedItem != null && MessageBox.Show("Вы действительно хотите удалить выбранное подразделение?", "Подтвердждение на удаление элемента", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int idSubUnit = (lbBattalion.SelectedItem as SubUnit).Id;
                if (sqlWorker.deleteItemById("SubUnit", idSubUnit))
                {
                    bList.deleteById(idSubUnit);
                }
                else
                {
                    MessageBox.Show("При удалении элемента возникла ошибка, возможно, он уже используется");
                }
            }
        }

        private void btnDeleteMilitaryUnit_Click(object sender, RoutedEventArgs e)
        {
            if (lbMilitaryUnit.SelectedItem != null && MessageBox.Show("Вы действительно хотите удалить выбранную часть?", "Подтвердждение на удаление элемента", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int idMilitaryUnit = (lbMilitaryUnit.SelectedItem as MilitaryUnit).Id;
                if (sqlWorker.deleteItemById("MilitaryUnit", idMilitaryUnit))
                {
                    mList.deleteById(idMilitaryUnit);
                }
                else
                {
                    MessageBox.Show("При удалении элемента возникла ошибка, возможно, он уже используется");
                }
            }
        }

        private void btnAddSubUnit_Click(object sender, RoutedEventArgs e)
        {
            if (lbBattalion.SelectedItem != null)
            {
                int idFKSubUnit = (lbBattalion.SelectedItem as SubUnit).Id;
                SubUnit s = addSubUnit.getNewSubUnit(-1, idFKSubUnit);
                if (s != null)
                {
                    if (sList == null)
                        sList = new SubUnitList();

                    int id = sqlWorker.getNewId("SubUnit");
                    s.Id = id;
                    if (sqlWorker.addSubUnit(id, s))
                        sList.values.Add(s);
                    else
                        MessageBox.Show("Ошибка при добавлении элемента");
                }
            }
        }

        private void btnAddBattalion_Click(object sender, RoutedEventArgs e)
        {
            if (lbMilitaryUnit.SelectedItem != null)
            {
                int idMilitaryUnit = (lbMilitaryUnit.SelectedItem as MilitaryUnit).Id;
                SubUnit s = addSubUnit.getNewSubUnit(idMilitaryUnit, -1);
                if (s != null)
                {
                    if (bList == null)
                        bList = new SubUnitList();

                    int id = sqlWorker.getNewId("SubUnit");
                    s.Id = id;
                    if (sqlWorker.addSubUnit(id, s))
                        bList.values.Add(s);
                    else
                        MessageBox.Show("Ошибка при добавлении элемента");
                }
            }
        }
        
        private void lbSubUnit_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbSubUnit.SelectedItem != null)
            {
                SubUnit s = addSubUnit.getEditedSubUnit(lbSubUnit.SelectedItem as SubUnit);
                if (s != null)
                {
                    if (sqlWorker.updateSubUnit(s))
                    {
                        sList.update(s);
                        lbSubUnit.Items.Refresh();
                    }
                    else
                        MessageBox.Show("Ошибка при изменении элемента");
                }
            }
        }

        private void lbBattalion_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbMilitaryUnit.SelectedItem != null)
            {
                SubUnit s = addSubUnit.getEditedSubUnit(lbBattalion.SelectedItem as SubUnit);
                if (s != null)
                {
                    if (sqlWorker.updateSubUnit(s))
                    {
                        bList.update(s);
                        lbBattalion.Items.Refresh();
                    }
                    else
                        MessageBox.Show("Ошибка при изменении элемента");
                }
            }
        }

        private void btnAddMilitaryUnit_Click(object sender, RoutedEventArgs e)
        {
            MilitaryUnit m = addMilitaryUnit.getNewMilitaryUnit();
            if (m != null)
            {
                if (mList == null)
                    mList = new MilitaryUnitList();

                int id = sqlWorker.getNewId("MilitaryUnit");
                m.Id = id;
                if (sqlWorker.addMilitaryUnit(id, m))
                    mList.values.Add(m);
                else
                    MessageBox.Show("Ошибка при добавлении элемента");
            }
        }

        private void lbMilitaryUnit_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MilitaryUnit m = addMilitaryUnit.getEditedMilitaryUnit(lbMilitaryUnit.SelectedItem as MilitaryUnit);
                if (m != null)
                {
                    if (sqlWorker.updateMilitaryUnit(m))
                    {
                        mList.update(m);
                        lbMilitaryUnit.Items.Refresh();
                    }
                    else
                        MessageBox.Show("Ошибка при изменении элемента");
                }
        }
    }
}
