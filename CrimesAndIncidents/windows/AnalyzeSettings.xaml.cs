﻿using System;
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
    /// Логика взаимодействия для AnalyzeSettings.xaml
    /// </summary>
    public partial class AnalyzeSettings : Window
    {
        private SqliteWorker sqlWorker;
        PleaseWait wndP = new PleaseWait();

        public AnalyzeSettings()
        {
            InitializeComponent();
        }

        public AnalyzeSettings(SqliteWorker sqlWorker)
        {
            InitializeComponent();

            dpLeft.Text = "01.01." + DateTime.Now.ToString("yyyy");
            dpRight.Text = DateTime.Now.ToString("dd.MM.yyyy");

            this.sqlWorker = sqlWorker;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            wndP.Show();

            try
            {
                string dateLeft = DataWorker.getDateInTrueFormat(dpLeft.Text);
                string dateRight = DataWorker.getDateInTrueFormat(dpRight.Text);

                string dateLeftPrev = DataWorker.getDateInTrueFormat(dpLeft.Text, -1);
                string dateRightPrev = DataWorker.getDateInTrueFormat(dpRight.Text, -1);
                
                int countCrimes = Int32.Parse(sqlWorker.selectData("SELECT COUNT(C.idMilitaryUnit) FROM " +
                    "MilitaryUnit M LEFT JOIN " +
                    "Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration " +
                    "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "' AND C.idClause > -1").Rows[0][0].ToString());
                int countIncidents = Int32.Parse(sqlWorker.selectData("SELECT COUNT(C.idMilitaryUnit) FROM " +
                    "MilitaryUnit M LEFT JOIN " +
                    "Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration " +
                    "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "' AND C.idClause IS NULL").Rows[0][0].ToString());
                int countCrimesPrev=0, countIncidentsPrev=0;
                if (rbPrevPeriod.IsChecked.Value)
                {
                    countCrimesPrev = Int32.Parse(sqlWorker.selectData("SELECT COUNT(C.idMilitaryUnit) FROM " +
                        "MilitaryUnit M LEFT JOIN " +
                        "Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration " +
                        "BETWEEN '" + dateLeftPrev + "' AND '" + (dateRightPrev == "" ? "9999.99.99" : dateRightPrev) + "' AND C.idClause > -1").Rows[0][0].ToString());
                    countIncidentsPrev = Int32.Parse(sqlWorker.selectData("SELECT COUNT(C.idMilitaryUnit) FROM " +
                        "MilitaryUnit M LEFT JOIN " +
                        "Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration " +
                        "BETWEEN '" + dateLeftPrev + "' AND '" + (dateRightPrev == "" ? "9999.99.99" : dateRightPrev) + "' AND C.idClause IS NULL").Rows[0][0].ToString());
                }
                Microsoft.Office.Interop.Word.Application winword =
                    new Microsoft.Office.Interop.Word.Application();

                winword.Visible = false;

                //Заголовок документа
                winword.Documents.Application.Caption = "CrimesAndIncidents";

                object missing = System.Reflection.Missing.Value;

                //Создание нового документа
                Microsoft.Office.Interop.Word.Document document = winword.Documents.Add(ref missing, ref missing, ref missing, ref missing);

                //Добавление текста в документ
                document.Content.SetRange(0, 0);

                //Добавление текста со стилем Заголовок 1
                Microsoft.Office.Interop.Word.Paragraph para1 = document.Content.Paragraphs.Add(ref missing);
                //para1.Range.set_Style(styleHeading1);
                para1.Range.Font.Size = 14;
                para1.Range.Font.Bold = 14;
                para1.Range.Text = "Анализ преступлений и происшествий за " +
                    ((dateLeft == "" && dateRight == "") ? "все время" :
                        ("период " +
                        (dateLeft == "" ? "" : "c " + dateLeft + " г.") +
                        (dateRight == "" ? "" : " по " + dateRight + " г.")));
                para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                para1.Range.InsertParagraphAfter();
                para1.Range.InsertParagraphAfter();

                para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para1.Range.Font.Size = 14;
                para1.Range.Text = "Всего преступлений и происшествий: " + (countCrimes + countIncidents) + " (" + countCrimes + " преступлен" + 
                    DataWorker.numberInPlugue(countCrimes) + ", " + countIncidents +
                    " происшеств" +
                    DataWorker.numberInPlugue(countIncidents) + ")" +

                    (!rbPrevPeriod.IsChecked.Value ? "" :
                        "\n\t" + (dpRight.SelectedDate.Value.Year-1) +" г. - "+(countCrimesPrev + countIncidentsPrev) + " (" + countCrimesPrev + " преступлен" +
                        DataWorker.numberInPlugue(countCrimesPrev) + ", " + countIncidentsPrev +
                        " происшеств" +
                        DataWorker.numberInPlugue(countIncidentsPrev) + ")");
                
                para1.Range.InsertParagraphAfter();
                para1.Range.InsertParagraphAfter();

                #region анализ по воинским частям
                if (chkOnMilitaryUnit.IsChecked.Value)
                {
                    para1.Range.Text = "По воинским частям:";
                    para1.Range.InsertParagraphAfter();

                    DataTable tableCrimes = sqlWorker.selectData("SELECT M.number, M.shortName, COUNT(C.idMilitaryUnit) FROM " +
                            "MilitaryUnit M LEFT JOIN " +
                            "Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration " +
                            "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "' AND C.idClause > -1 " +
                        "GROUP BY M.number, M.shortName " +
                        "ORDER BY M.idMilitaryUnit;");
                    DataTable tableIncidents = sqlWorker.selectData("SELECT M.number, M.shortName, COUNT(C.idMilitaryUnit) FROM " +
                            "MilitaryUnit M LEFT JOIN " +
                            "Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration " +
                            "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "' AND C.idClause IS NULL " +
                        "GROUP BY M.number, M.shortName " +
                        "ORDER BY M.idMilitaryUnit;");
                    DataTable tableCrimesPrev = new DataTable(), tableIncidentsPrev = new DataTable();
                    if (rbPrevPeriod.IsChecked.Value)
                    {
                        tableCrimesPrev = sqlWorker.selectData("SELECT M.number, M.shortName, COUNT(C.idMilitaryUnit) FROM " +
                               "MilitaryUnit M LEFT JOIN " +
                               "Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration " +
                               "BETWEEN '" + dateLeftPrev + "' AND '" + (dateRightPrev == "" ? "9999.99.99" : dateRightPrev) + "' AND C.idClause > -1 " +
                            "GROUP BY M.number, M.shortName " +
                            "ORDER BY M.idMilitaryUnit;");
                        tableIncidentsPrev = sqlWorker.selectData("SELECT M.number, M.shortName, COUNT(C.idMilitaryUnit) FROM " +
                                "MilitaryUnit M LEFT JOIN " +
                                "Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration " +
                                "BETWEEN '" + dateLeftPrev + "' AND '" + (dateRightPrev == "" ? "9999.99.99" : dateRightPrev) + "' AND C.idClause IS NULL " +
                            "GROUP BY M.number, M.shortName " +
                            "ORDER BY M.idMilitaryUnit;");
                    }

                    for (int i = 0; i < tableCrimes.Rows.Count; i++)
                    {
                        para1.Range.Text = "- войсковая часть " + tableCrimes.Rows[i][0] + " (" + tableCrimes.Rows[i][1] + "): " +
                            tableCrimes.Rows[i][2] + " пре, " + tableIncidents.Rows[i][2] + " про" +

                            (!rbPrevPeriod.IsChecked.Value ? "" :
                                " (" + (dpRight.SelectedDate.Value.Year - 1) + " г. - " +
                                tableCrimesPrev.Rows[i][2] + " и " + tableIncidentsPrev.Rows[i][2] + ")");
                        para1.Range.Font.Size = 14;
                        if (chkOnSubUnit.IsChecked.Value) para1.Range.Font.Bold = 14;
                        para1.Range.InsertParagraphAfter();

                        if (chkOnSubUnit.IsChecked.Value)
                        {
                            DataTable tableSubUnitCrime = sqlWorker.selectData("SELECT Name, COUNT(idCrime) FROM " +
                                    "(SELECT M.number, M.shortName, S.Name, C.idCrime FROM " +
                                        "Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime " +
                                        "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " +
                                        "LEFT JOIN SubUnit S ON S.idSubUnit = A.idSubUnit  " +
                                        "LEFT JOIN MilitaryUnit M ON M.idMilitaryUnit = C.idMilitaryUnit " +
                                    "WHERE C.isRegistred = 1 AND C.dateRegistration  " +
                                        "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "' AND C.idClause > -1 " +
                                    "GROUP BY C.idCrime " +
                                    "ORDER BY M.idMilitaryUnit) " +
                                "WHERE number = '" + tableCrimes.Rows[i][0] + "' " +
                                "GROUP BY Name " +
                                "ORDER BY Name");

                            for (int j = 0; j < tableSubUnitCrime.Rows.Count; j++)
                            {
                                para1.Range.Font.Bold = 0;
                                para1.Range.Font.Size = 14;
                                para1.Range.Text = "\t" + (tableSubUnitCrime.Rows[j][0].ToString() == "" ? "не установлено" : tableSubUnitCrime.Rows[j][0].ToString()) + ": " +
                                    tableSubUnitCrime.Rows[j][1] + " преступлен" +
                                    DataWorker.numberInPlugue(Int32.Parse(tableSubUnitCrime.Rows[j][1].ToString())) + "";
                                para1.Range.Font.Size = 14;
                                para1.Range.InsertParagraphAfter();
                            }

                            if (rbPrevPeriod.IsChecked.Value)
                            {
                                DataTable tableSubUnitCrimePrev = sqlWorker.selectData("SELECT Name, COUNT(idCrime) FROM " +
                                    "(SELECT M.number, M.shortName, S.Name, C.idCrime FROM " +
                                        "Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime " +
                                        "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " +
                                        "LEFT JOIN SubUnit S ON S.idSubUnit = A.idSubUnit  " +
                                        "LEFT JOIN MilitaryUnit M ON M.idMilitaryUnit = C.idMilitaryUnit " +
                                    "WHERE C.isRegistred = 1 AND C.dateRegistration  " +
                                        "BETWEEN '" + dateLeftPrev + "' AND '" + (dateRightPrev == "" ? "9999.99.99" : dateRightPrev) + "' AND C.idClause > -1 " +
                                    "GROUP BY C.idCrime " +
                                    "ORDER BY M.idMilitaryUnit) " +
                                "WHERE number = '" + tableCrimes.Rows[i][0] + "' " +
                                "GROUP BY Name " +
                                "ORDER BY Name");

                                if (tableSubUnitCrimePrev.Rows.Count > 0)
                                {
                                    para1.Range.Font.Bold = 14;
                                    para1.Range.Text = "\t" + (dpRight.SelectedDate.Value.Year - 1) + " г.:";
                                    para1.Range.InsertParagraphAfter();
                                }
                                for (int j = 0; j < tableSubUnitCrimePrev.Rows.Count; j++)
                                {
                                    para1.Range.Font.Bold = 0;
                                    para1.Range.Font.Size = 14;
                                    para1.Range.Text = "\t" +
                                        (tableSubUnitCrimePrev.Rows[j][0].ToString() == "" ? "не установлено" : tableSubUnitCrimePrev.Rows[j][0].ToString()) + ": " +
                                        tableSubUnitCrimePrev.Rows[j][1] + " преступлен" +
                                        DataWorker.numberInPlugue(Int32.Parse(tableSubUnitCrimePrev.Rows[j][1].ToString())) + "";
                                    para1.Range.Font.Size = 14;
                                    para1.Range.InsertParagraphAfter();
                                }
                            }
                        }
                    }
                    para1.Range.InsertParagraphAfter();
                }
                #endregion

                #region по участникам
                if (chkOnAccomplice.IsChecked.Value)
                {
                    DataTable tableAccompliceCrimePrev = new DataTable(),
                        tableAccompliceCrime = sqlWorker.selectData("SELECT * FROM " +
                        "(SELECT COUNT(DISTINCT A.idAccomplice) as призывники FROM " + 
                            "Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime " + 
                            "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " + 
                            "LEFT JOIN Rank R ON R.idRank = A.idRank " + 
                        "WHERE A.isContrakt = 0  " + 
                            "AND C.isRegistred = 1  " + 
                            "AND C.dateRegistration  " + 
                            "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "'  " + 
                            "AND C.idClause > -1), " +
                        "(SELECT COUNT(DISTINCT A.idAccomplice) as контрактники FROM " + 
                            "Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime " + 
                            "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " + 
                            "LEFT JOIN Rank R ON R.idRank = A.idRank " + 
                        "WHERE A.isContrakt = 1 " + 
                            "AND R.priority < 60  " + 
                            "AND C.isRegistred = 1  " + 
                            "AND C.dateRegistration  " + 
                            "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "'  " + 
                            "AND C.idClause > -1), " +
                        "(SELECT COUNT(DISTINCT A.idAccomplice) as прапорщики FROM " + 
                            "Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime " + 
                            "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " + 
                            "LEFT JOIN Rank R ON R.idRank = A.idRank " + 
                        "WHERE A.isContrakt = 1 " + 
                            "AND R.priority BETWEEN 60 AND 75 " + 
                            "AND C.isRegistred = 1  " + 
                            "AND C.dateRegistration  " + 
                            "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "'  " + 
                            "AND C.idClause > -1), " +
                        "(SELECT COUNT(DISTINCT A.idAccomplice) as офицеры FROM " + 
                            "Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime " + 
                            "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " + 
                            "LEFT JOIN Rank R ON R.idRank = A.idRank " + 
                        "WHERE A.isContrakt = 1 " + 
                            "AND R.priority > 75 " + 
                            "AND C.isRegistred = 1  " + 
                            "AND C.dateRegistration  " +
                            "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "'  " + 
                            "AND C.idClause > -1);");

                    if(rbPrevPeriod.IsChecked.Value)
                        tableAccompliceCrimePrev = sqlWorker.selectData("SELECT * FROM " +
                        "(SELECT COUNT(DISTINCT A.idAccomplice) as призывники FROM " +
                            "Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime " +
                            "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " +
                            "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                        "WHERE A.isContrakt = 0  " +
                            "AND C.isRegistred = 1  " +
                            "AND C.dateRegistration  " +
                            "BETWEEN '" + dateLeftPrev + "' AND '" + (dateRightPrev == "" ? "9999.99.99" : dateRightPrev) + "'  " +
                            "AND C.idClause > -1), " +
                        "(SELECT COUNT(DISTINCT A.idAccomplice) as контрактники FROM " +
                            "Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime " +
                            "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " +
                            "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                        "WHERE A.isContrakt = 1 " +
                            "AND R.priority < 60  " +
                            "AND C.isRegistred = 1  " +
                            "AND C.dateRegistration  " +
                            "BETWEEN '" + dateLeftPrev + "' AND '" + (dateRightPrev == "" ? "9999.99.99" : dateRightPrev) + "'  " +
                            "AND C.idClause > -1), " +
                        "(SELECT COUNT(DISTINCT A.idAccomplice) as прапорщики FROM " +
                            "Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime " +
                            "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " +
                            "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                        "WHERE A.isContrakt = 1 " +
                            "AND R.priority BETWEEN 60 AND 75 " +
                            "AND C.isRegistred = 1  " +
                            "AND C.dateRegistration  " +
                            "BETWEEN '" + dateLeftPrev + "' AND '" + (dateRightPrev == "" ? "9999.99.99" : dateRightPrev) + "'  " +
                            "AND C.idClause > -1), " +
                        "(SELECT COUNT(DISTINCT A.idAccomplice) as офицеры FROM " +
                            "Crime C LEFT JOIN Portaking P ON C.idCrime = P.idCrime " +
                            "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " +
                            "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                        "WHERE A.isContrakt = 1 " +
                            "AND R.priority > 75 " +
                            "AND C.isRegistred = 1  " +
                            "AND C.dateRegistration  " +
                            "BETWEEN '" + dateLeftPrev + "' AND '" + (dateRightPrev == "" ? "9999.99.99" : dateRightPrev) + "'  " +
                            "AND C.idClause > -1);");

                    para1.Range.InsertParagraphAfter();
                    para1.Range.Text = "По участникам: ";
                    para1.Range.InsertParagraphAfter();
                    
                    para1.Range.Font.Bold = 0;
                    para1.Range.Font.Size = 14;
                    para1.Range.Text = "-солдаты и сержанты по призыву: " + tableAccompliceCrime.Rows[0][0] +
                        (!rbPrevPeriod.IsChecked.Value ? "" :
                            " (" + (dpRight.SelectedDate.Value.Year - 1) + " г. - " + tableAccompliceCrimePrev.Rows[0][0] + ")") + Environment.NewLine +
                        "-солдаты и сержанты по контракту: " + tableAccompliceCrime.Rows[0][1] +
                        (!rbPrevPeriod.IsChecked.Value ? "" :
                            " (" + (dpRight.SelectedDate.Value.Year - 1) + " г. - " + tableAccompliceCrimePrev.Rows[0][1] + ")") + Environment.NewLine +
                        "-прапорщики: " + tableAccompliceCrime.Rows[0][2] +
                        (!rbPrevPeriod.IsChecked.Value ? "" :
                            " (" + (dpRight.SelectedDate.Value.Year - 1) + " г. - " + tableAccompliceCrimePrev.Rows[0][2] + ")") + Environment.NewLine +
                        "-офицеры: " + tableAccompliceCrime.Rows[0][3] +
                        (!rbPrevPeriod.IsChecked.Value ? "" :
                            " (" + (dpRight.SelectedDate.Value.Year - 1) + " г. - " + tableAccompliceCrimePrev.Rows[0][3] + ")");
                    para1.Range.InsertParagraphAfter(); 
                    para1.Range.InsertParagraphAfter();

                    if (chkOnMilitaryUnit.IsChecked.Value)
                    {
                        DataTable tableAccompliceMilitaryUnitPrev = new DataTable(),
                            tableAccompliceMilitaryUnit = sqlWorker.selectData("SELECT M1.number, M1.name, T1.n1 as призывники, T2.n2 as контрактники, T3.n3 as прапорщики, T4.n4 as офицеры FROM  " + 
                            "(SELECT M.number, M.name FROM MilitaryUnit M) M1 LEFT JOIN  " + 
                            "(SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n1 FROM " + 
                                "MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit " + 
                                "LEFT JOIN Portaking P ON C.idCrime = P.idCrime " + 
                                "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " + 
                                "LEFT JOIN Rank R ON R.idRank = A.idRank " + 
                            "WHERE A.isContrakt = 0 " + 
                                "AND C.isRegistred = 1  " + 
                                "AND C.dateRegistration  " + 
                                "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "'  " + 
                                "AND C.idClause > -1 " + 
                            "GROUP BY M.idMilitaryUnit " + 
                            "ORDER BY M.idMilitaryUnit) T1 ON T1.number = M1.number  " + 
                            "LEFT JOIN " + 
                            "(SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n2 FROM " + 
                                "MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit " + 
                                "LEFT JOIN Portaking P ON C.idCrime = P.idCrime " + 
                                "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " + 
                                "LEFT JOIN Rank R ON R.idRank = A.idRank " + 
                            "WHERE A.isContrakt = 1 " + 
                                "AND R.priority < 60 " + 
                                "AND C.isRegistred = 1  " + 
                                "AND C.dateRegistration  " + 
                                "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "'  " + 
                                "AND C.idClause > -1 " + 
                            "GROUP BY M.idMilitaryUnit " + 
                            "ORDER BY M.idMilitaryUnit) T2 ON T2.number = M1.number  " + 
                            "LEFT JOIN " + 
                            "(SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n3 FROM " + 
                                "MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit " + 
                                "LEFT JOIN Portaking P ON C.idCrime = P.idCrime " + 
                                "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " + 
                                "LEFT JOIN Rank R ON R.idRank = A.idRank " + 
                            "WHERE A.isContrakt = 1 " + 
                                "AND R.priority BETWEEN 60 AND 75 " + 
                                "AND C.isRegistred = 1  " + 
                                "AND C.dateRegistration  " + 
                                "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "'  " + 
                                "AND C.idClause > -1 " + 
                            "GROUP BY M.idMilitaryUnit " + 
                            "ORDER BY M.idMilitaryUnit) T3  ON T3.number = M1.number  " + 
                            "LEFT JOIN " + 
                            "(SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n4 FROM " + 
                                "MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit " + 
                                "LEFT JOIN Portaking P ON C.idCrime = P.idCrime " + 
                                "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " + 
                                "LEFT JOIN Rank R ON R.idRank = A.idRank " + 
                            "WHERE A.isContrakt = 1 " + 
                                "AND R.priority > 75 " + 
                                "AND C.isRegistred = 1  " + 
                                "AND C.dateRegistration  " +
                                "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "'  " + 
                                "AND C.idClause > -1 " + 
                            "GROUP BY M.idMilitaryUnit " + 
                            "ORDER BY M.idMilitaryUnit) T4  ON T4.number = M1.number ");

                        if(rbPrevPeriod.IsChecked.Value)
                            tableAccompliceMilitaryUnitPrev = sqlWorker.selectData("SELECT M1.number, M1.name, T1.n1 as призывники, T2.n2 as контрактники, T3.n3 as прапорщики, T4.n4 as офицеры FROM  " +
                            "(SELECT M.number, M.name FROM MilitaryUnit M) M1 LEFT JOIN  " +
                            "(SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n1 FROM " +
                                "MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit " +
                                "LEFT JOIN Portaking P ON C.idCrime = P.idCrime " +
                                "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " +
                                "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                            "WHERE A.isContrakt = 0 " +
                                "AND C.isRegistred = 1  " +
                                "AND C.dateRegistration  " +
                                "BETWEEN '" + dateLeftPrev + "' AND '" + (dateRightPrev == "" ? "9999.99.99" : dateRightPrev) + "'  " +
                                "AND C.idClause > -1 " +
                            "GROUP BY M.idMilitaryUnit " +
                            "ORDER BY M.idMilitaryUnit) T1 ON T1.number = M1.number  " +
                            "LEFT JOIN " +
                            "(SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n2 FROM " +
                                "MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit " +
                                "LEFT JOIN Portaking P ON C.idCrime = P.idCrime " +
                                "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " +
                                "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                            "WHERE A.isContrakt = 1 " +
                                "AND R.priority < 60 " +
                                "AND C.isRegistred = 1  " +
                                "AND C.dateRegistration  " +
                                "BETWEEN '" + dateLeftPrev + "' AND '" + (dateRightPrev == "" ? "9999.99.99" : dateRightPrev) + "'  " +
                                "AND C.idClause > -1 " +
                            "GROUP BY M.idMilitaryUnit " +
                            "ORDER BY M.idMilitaryUnit) T2 ON T2.number = M1.number  " +
                            "LEFT JOIN " +
                            "(SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n3 FROM " +
                                "MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit " +
                                "LEFT JOIN Portaking P ON C.idCrime = P.idCrime " +
                                "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " +
                                "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                            "WHERE A.isContrakt = 1 " +
                                "AND R.priority BETWEEN 60 AND 75 " +
                                "AND C.isRegistred = 1  " +
                                "AND C.dateRegistration  " +
                                "BETWEEN '" + dateLeftPrev + "' AND '" + (dateRightPrev == "" ? "9999.99.99" : dateRightPrev) + "'  " +
                                "AND C.idClause > -1 " +
                            "GROUP BY M.idMilitaryUnit " +
                            "ORDER BY M.idMilitaryUnit) T3  ON T3.number = M1.number  " +
                            "LEFT JOIN " +
                            "(SELECT M.number, M.name, COUNT(DISTINCT A.idAccomplice) as n4 FROM " +
                                "MilitaryUnit M LEFT JOIN  Crime C On M.idMilitaryUnit = C.idMilitaryUnit " +
                                "LEFT JOIN Portaking P ON C.idCrime = P.idCrime " +
                                "LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice " +
                                "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                            "WHERE A.isContrakt = 1 " +
                                "AND R.priority > 75 " +
                                "AND C.isRegistred = 1  " +
                                "AND C.dateRegistration  " +
                                "BETWEEN '" + dateLeftPrev + "' AND '" + (dateRightPrev == "" ? "9999.99.99" : dateRightPrev) + "'  " +
                                "AND C.idClause > -1 " +
                            "GROUP BY M.idMilitaryUnit " +
                            "ORDER BY M.idMilitaryUnit) T4  ON T4.number = M1.number ");

                        for (int i = 0; i < tableAccompliceMilitaryUnit.Rows.Count; i++)
                        {
                            if (tableAccompliceMilitaryUnit.Rows[i][2].ToString() != "" ||
                                tableAccompliceMilitaryUnit.Rows[i][3].ToString() != "" ||
                                tableAccompliceMilitaryUnit.Rows[i][4].ToString() != "" ||
                                tableAccompliceMilitaryUnit.Rows[i][5].ToString() != "" ||
                                (rbPrevPeriod.IsChecked.Value && (tableAccompliceMilitaryUnitPrev.Rows[i][2].ToString() != "" ||
                                tableAccompliceMilitaryUnitPrev.Rows[i][3].ToString() != "" ||
                                tableAccompliceMilitaryUnitPrev.Rows[i][4].ToString() != "" ||
                                tableAccompliceMilitaryUnitPrev.Rows[i][5].ToString() != "")))
                            {
                                para1.Range.Font.Size = 14;
                                para1.Range.Font.Bold = 14;
                                para1.Range.Text = "- войсковая часть " + tableAccompliceMilitaryUnit.Rows[i][0] + " (" + tableAccompliceMilitaryUnit.Rows[i][1] + "): ";
                                para1.Range.InsertParagraphAfter();

                                para1.Range.Font.Size = 14;
                                para1.Range.Font.Bold = 0;
                                if (!rbPrevPeriod.IsChecked.Value)
                                {
                                    para1.Range.Text = (tableAccompliceMilitaryUnit.Rows[i][2].ToString() == "" ?
                                            "" :
                                            "\tсолдаты и сержанты по призыву: " + tableAccompliceMilitaryUnit.Rows[i][2] + Environment.NewLine) +
                                        (tableAccompliceMilitaryUnit.Rows[i][3].ToString() == "" ?
                                            "" :
                                            "\tсолдаты и сержанты по контракту: " + tableAccompliceMilitaryUnit.Rows[i][3] + Environment.NewLine) +
                                        (tableAccompliceMilitaryUnit.Rows[i][4].ToString() == "" ?
                                            "" :
                                            "\tпрапорщики: " + tableAccompliceMilitaryUnit.Rows[i][4] + Environment.NewLine) +
                                        (tableAccompliceMilitaryUnit.Rows[i][5].ToString() == "" ?
                                            "" :
                                            "\tофицеры: " + tableAccompliceMilitaryUnit.Rows[i][5]);
                                }
                                else
                                {
                                    para1.Range.Text = (tableAccompliceMilitaryUnit.Rows[i][2].ToString() == "" &&
                                        tableAccompliceMilitaryUnitPrev.Rows[i][2].ToString() == "" ?
                                            "" :
                                            "\tсолдаты и сержанты по призыву: " +
                                            (tableAccompliceMilitaryUnit.Rows[i][2].ToString() == "" ? "0" : tableAccompliceMilitaryUnit.Rows[i][2]) +
                                            (tableAccompliceMilitaryUnitPrev.Rows[i][2].ToString() == "" ? "" : " (" + (dpRight.SelectedDate.Value.Year - 1) + " г. - " +
                                            tableAccompliceMilitaryUnitPrev.Rows[i][2] + ")") +
                                            Environment.NewLine) +
                                        (tableAccompliceMilitaryUnit.Rows[i][3].ToString() == "" &&
                                        tableAccompliceMilitaryUnitPrev.Rows[i][3].ToString() == "" ?
                                            "" :
                                            "\tсолдаты и сержанты по контракту: " +
                                            (tableAccompliceMilitaryUnit.Rows[i][3].ToString() == "" ? "0" : tableAccompliceMilitaryUnit.Rows[i][3]) +
                                            (tableAccompliceMilitaryUnitPrev.Rows[i][3].ToString() == "" ? "" : " (" + (dpRight.SelectedDate.Value.Year - 1) + " г. - " +
                                            tableAccompliceMilitaryUnitPrev.Rows[i][3] + ")") +
                                             Environment.NewLine) +
                                        (tableAccompliceMilitaryUnit.Rows[i][4].ToString() == "" &&
                                        tableAccompliceMilitaryUnitPrev.Rows[i][4].ToString() == "" ?
                                            "" :
                                            "\tпрапорщики: " +
                                            (tableAccompliceMilitaryUnit.Rows[i][4].ToString() == "" ? "0" : tableAccompliceMilitaryUnit.Rows[i][4]) +
                                            (tableAccompliceMilitaryUnitPrev.Rows[i][4].ToString() == "" ? "" : " (" + (dpRight.SelectedDate.Value.Year - 1) + " г. - " +
                                            tableAccompliceMilitaryUnitPrev.Rows[i][4] + ")") +
                                             Environment.NewLine) +
                                        (tableAccompliceMilitaryUnit.Rows[i][5].ToString() == "" &&
                                        tableAccompliceMilitaryUnitPrev.Rows[i][5].ToString() == "" ?
                                            "" :
                                            "\tофицеры: " +
                                            (tableAccompliceMilitaryUnit.Rows[i][5].ToString() == "" ? "0" : tableAccompliceMilitaryUnit.Rows[i][5]) +
                                            (tableAccompliceMilitaryUnitPrev.Rows[i][5].ToString() == "" ? "" : " (" + (dpRight.SelectedDate.Value.Year - 1) + " г. - " +
                                            tableAccompliceMilitaryUnitPrev.Rows[i][5] + ")"));
                                }
                                para1.Range.InsertParagraphAfter();
                            }
                        }
                    }

                }
                #endregion

                #region по видам преступлений
                if (chkOnClause.IsChecked.Value)
                {
                    para1.Range.InsertParagraphAfter();
                    para1.Range.Text = "По видам преступлений:";
                    para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    para1.Range.Font.Size = 14;
                    para1.Range.Font.Bold = 14;
                    para1.Range.InsertParagraphAfter();

                    DataTable tableOnClausePrev= new DataTable(), 
                        tableOnClause = sqlWorker.selectData("SELECT Cl.description, COUNT(C.idCrime) FROM " +
                            "Crime C LEFT JOIN Clause Cl ON Cl.idClause = C.idClause "+
                        "WHERE C.isRegistred = 1 AND C.dateRegistration BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + 
                            "' AND C.idClause > -1 " +
                        "GROUP BY Cl.description "+
                        "ORDER BY Cl.description;");

                    

                    for (int i = 0; i < tableOnClause.Rows.Count; i++)
                    {
                        para1.Range.Font.Size = 14;
                        para1.Range.Font.Bold = 0; 
                        para1.Range.Text = "-" + tableOnClause.Rows[i][0] + ": " + tableOnClause.Rows[i][1];
                        para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                        para1.Range.InsertParagraphAfter();
                    }

                    if (rbPrevPeriod.IsChecked.Value)
                    {
                        tableOnClausePrev = sqlWorker.selectData("SELECT Cl.description, COUNT(C.idCrime) FROM " +
                            "Crime C LEFT JOIN Clause Cl ON Cl.idClause = C.idClause " +
                        "WHERE C.isRegistred = 1 AND C.dateRegistration BETWEEN '" + dateLeftPrev + "' AND '" + (dateRightPrev == "" ? "9999.99.99" : dateRightPrev) +
                            "' AND C.idClause > -1 " +
                        "GROUP BY Cl.description " +
                        "ORDER BY Cl.description;");

                        para1.Range.Font.Bold = 14;
                        para1.Range.Text = (dpRight.SelectedDate.Value.Year-1) +" г.:";
                        para1.Range.InsertParagraphAfter();

                        for (int i = 0; i < tableOnClausePrev.Rows.Count; i++)
                        {
                            para1.Range.Font.Size = 14;
                            para1.Range.Font.Bold = 0;
                            para1.Range.Text = "-" + tableOnClausePrev.Rows[i][0] + ": " + tableOnClausePrev.Rows[i][1];
                            para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            para1.Range.InsertParagraphAfter();
                        }
                    }

                    para1.Range.InsertParagraphAfter();

                    if (chkOnMilitaryUnit.IsChecked.Value)
                    {
                        tableOnClause = sqlWorker.selectData("SELECT M.number, M.shortName, Cl.description, COUNT(C.idCrime) FROM " +
                                "Crime C LEFT JOIN MilitaryUnit M ON M.idMilitaryUnit = C.idMilitaryUnit " +
                                "LEFT JOIN Clause Cl ON Cl.idClause = C.idClause " +
                            "WHERE C.isRegistred = 1 AND C.dateRegistration BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) +
                                "' AND C.idClause > -1 " +
                            "GROUP BY Cl.description, M.idMilitaryUnit " +
                            "ORDER BY M.idMilitaryUnit;");

                        if (tableOnClause.Rows.Count != 0)
                        {
                            string oldUnit = tableOnClause.Rows[0][0].ToString();
                            para1.Range.Text = "-войсковая часть " + tableOnClause.Rows[0][0] + " (" + tableOnClause.Rows[0][1] + "):";
                            para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            para1.Range.Font.Size = 14;
                            para1.Range.Font.Bold = 14;
                            para1.Range.InsertParagraphAfter();

                            for (int i = 0; i < tableOnClause.Rows.Count; i++)
                            {
                                string newUnit = tableOnClause.Rows[i][0].ToString();
                                if (newUnit != oldUnit)
                                {
                                    para1.Range.Text = "-войсковая часть " + tableOnClause.Rows[i][0] + " (" + tableOnClause.Rows[i][1] + "):";
                                    para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                                    para1.Range.Font.Size = 14;
                                    para1.Range.Font.Bold = 14;
                                    para1.Range.InsertParagraphAfter();
                                    oldUnit = newUnit;
                                }

                                para1.Range.Font.Size = 14;
                                para1.Range.Font.Bold = 0;
                                para1.Range.Text = "\t" + tableOnClause.Rows[i][2] + ": " + tableOnClause.Rows[i][3];
                                para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                                para1.Range.InsertParagraphAfter();
                            }
                        }

                        if (rbPrevPeriod.IsChecked.Value)
                        {
                            tableOnClausePrev = sqlWorker.selectData("SELECT M.number, M.shortName, Cl.description, COUNT(C.idCrime) FROM " +
                                "Crime C LEFT JOIN MilitaryUnit M ON M.idMilitaryUnit = C.idMilitaryUnit " +
                                "LEFT JOIN Clause Cl ON Cl.idClause = C.idClause " +
                            "WHERE C.isRegistred = 1 AND C.dateRegistration BETWEEN '" + dateLeftPrev + "' AND '" + (dateRightPrev == "" ? "9999.99.99" : dateRightPrev) +
                                "' AND C.idClause > -1 " +
                            "GROUP BY Cl.description, M.idMilitaryUnit " +
                            "ORDER BY M.idMilitaryUnit;");

                            para1.Range.Font.Bold = 14;
                            para1.Range.Text = (dpRight.SelectedDate.Value.Year - 1) + " г.:";
                            para1.Range.InsertParagraphAfter();

                            if (tableOnClausePrev.Rows.Count != 0)
                            {
                                string oldUnit = tableOnClausePrev.Rows[0][0].ToString();
                                para1.Range.Text = "-войсковая часть " + tableOnClausePrev.Rows[0][0] + " (" + tableOnClausePrev.Rows[0][1] + "):";
                                para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                                para1.Range.Font.Size = 14;
                                para1.Range.Font.Bold = 14;
                                para1.Range.InsertParagraphAfter();

                                for (int i = 0; i < tableOnClausePrev.Rows.Count; i++)
                                {
                                    string newUnit = tableOnClausePrev.Rows[i][0].ToString();
                                    if (newUnit != oldUnit)
                                    {
                                        para1.Range.Text = "-войсковая часть " + tableOnClausePrev.Rows[i][0] + " (" + tableOnClausePrev.Rows[i][1] + "):";
                                        para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                                        para1.Range.Font.Size = 14;
                                        para1.Range.Font.Bold = 14;
                                        para1.Range.InsertParagraphAfter();
                                        oldUnit = newUnit;
                                    }

                                    para1.Range.Font.Size = 14;
                                    para1.Range.Font.Bold = 0;
                                    para1.Range.Text = "\t" + tableOnClausePrev.Rows[i][2] + ": " + tableOnClausePrev.Rows[i][3];
                                    para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                                    para1.Range.InsertParagraphAfter();
                                }
                            }
                        }
                    }
                }
                #endregion

                #region по дате совершения преступления
                if (chkOnDateCommit.IsChecked.Value)
                {
                    para1.Range.InsertParagraphAfter();
                    para1.Range.Text = "По дате совершения преступления:";
                    para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    para1.Range.Font.Size = 14;
                    para1.Range.Font.Bold = 14;
                    para1.Range.InsertParagraphAfter();

                    DataTable tableOnClause = sqlWorker.selectData("SELECT SUBSTR(C.dateCommit,1,4), COUNT(C.idCrime) FROM Crime C " +
                        "WHERE C.isRegistred = 1 AND C.dateRegistration " +
                        "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) +
                                "' AND C.idClause > -1 " +
                        "GROUP BY SUBSTR(C.dateCommit,1,4);");

                    for (int i = 0; i < tableOnClause.Rows.Count; i++)
                    {
                        para1.Range.Font.Size = 14;
                        para1.Range.Font.Bold = 0;
                        para1.Range.Text = "- " + tableOnClause.Rows[i][0] + " год: " + tableOnClause.Rows[i][1] + " преступлен" +
                            DataWorker.numberInPlugue(Int32.Parse(tableOnClause.Rows[i][1].ToString()));
                        para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                        para1.Range.InsertParagraphAfter();
                    }
                    para1.Range.InsertParagraphAfter();

                    if (chkOnMilitaryUnit.IsChecked.Value)
                    {
                        tableOnClause = sqlWorker.selectData("SELECT M.number, M.shortName, SUBSTR(C.dateCommit,1,4), COUNT(C.idCrime) FROM "+
                                "Crime C LEFT JOIN MilitaryUnit M ON M.idMilitaryUnit = C.idMilitaryUnit "+
                            "WHERE C.isRegistred = 1 AND C.dateRegistration "+
                            "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) +
                                "' AND C.idClause > -1 " +
                            "GROUP BY M.idMilitaryUnit, SUBSTR(C.dateCommit,1,4) "+
                            "ORDER BY M.idMilitaryUnit, SUBSTR(C.dateCommit,1,4);");

                        if (tableOnClause.Rows.Count != 0)
                        {
                            string oldUnit = tableOnClause.Rows[0][0].ToString();
                            para1.Range.Text = "-войсковая часть " + tableOnClause.Rows[0][0] + " (" + tableOnClause.Rows[0][1] + "):";
                            para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            para1.Range.Font.Size = 14;
                            para1.Range.Font.Bold = 14;
                            para1.Range.InsertParagraphAfter();

                            for (int i = 0; i < tableOnClause.Rows.Count; i++)
                            {
                                string newUnit = tableOnClause.Rows[i][0].ToString();
                                if (newUnit != oldUnit)
                                {
                                    para1.Range.Text = "-войсковая часть " + tableOnClause.Rows[i][0] + " (" + tableOnClause.Rows[i][1] + "):";
                                    para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                                    para1.Range.Font.Size = 14;
                                    para1.Range.Font.Bold = 14;
                                    para1.Range.InsertParagraphAfter();
                                    oldUnit = newUnit;
                                }

                                para1.Range.Font.Size = 14;
                                para1.Range.Font.Bold = 0;
                                para1.Range.Text = "\t" + tableOnClause.Rows[i][2] + " год: " + tableOnClause.Rows[i][3] + " преступлен" +
                                    DataWorker.numberInPlugue(Int32.Parse(tableOnClause.Rows[i][3].ToString()));
                                para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                                para1.Range.InsertParagraphAfter();
                            }
                        }
                    }
                }
                #endregion

                winword.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            wndP.Close();
        }
    }
}
