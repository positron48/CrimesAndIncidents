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
    /// Логика взаимодействия для EditDBList.xaml
    /// </summary>
    public partial class EditDBList : Window
    {
        public DBList dblist;
        private SqliteWorker sqlWorker;

        public EditDBList(string rusName, DBList list, SqliteWorker _sqlWorker)
        {
            InitializeComponent();
            
            sqlWorker = _sqlWorker;
            dblist = list;

            dataGrid.Columns[0].Header = rusName;
            dataGrid.AutoGenerateColumns = false;
            dataGrid.ItemsSource = dblist.values;
        }

        //Нажата кнопка "Добавить"
        private void btnAddItem_Click_1(object sender, RoutedEventArgs e)
        {
            string newItem = InputBox.input("Введите " + dataGrid.Columns[0].Header);
            if (newItem != "")
            {
                //добавить в БД
                if (sqlWorker.addInDBList(dblist.TableName, dblist.newId(), newItem))
                {
                    //если успешное добавление в БД
                    dblist.values.Add(new KeyValue(dblist.newId(), newItem));
                }
                else
                    MessageBox.Show("Ошибка при добавлении данных");
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            dataGrid.CancelEdit();
            dataGrid.Items.Refresh();
            for (int i = 0; i < dblist.values.Count; i++)
            {
                if (dblist.values[i].isChanged)
                {
                    if(!sqlWorker.updateElement(dblist.TableName, dblist.values[i].Key, dblist.values[i].Value))
                        MessageBox.Show("Ошибка при обновлении элементов");
                }
            }
            this.Close();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить этот элемент?", "Подтвердите действие", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                KeyValue kv = dataGrid.SelectedItem as KeyValue;
                if (sqlWorker.deleteItemById(dblist.TableName, kv.Key))
                    dblist.deleteById(kv.Key);
                else
                    MessageBox.Show("Ошибка удаления элемента, возможно он уже используется.");
            }

        }


    }
}
