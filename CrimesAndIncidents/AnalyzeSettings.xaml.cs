using System;
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

        public AnalyzeSettings()
        {
            InitializeComponent();
        }

        public AnalyzeSettings(SqliteWorker sqlWorker)
        {
            InitializeComponent();

            this.sqlWorker = sqlWorker;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dateLeft = DataWorker.getDateInTrueFormat(dpLeft.Text);
                string dateRight = DataWorker.getDateInTrueFormat(dpRight.Text);
                int countCrimes = Int32.Parse(sqlWorker.selectData("SELECT COUNT(C.idMilitaryUnit) FROM " +
                    "MilitaryUnit M LEFT JOIN " +
                    "Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration " +
                    "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "' AND C.idClause > -1").Rows[0][0].ToString());
                int countIncidents = Int32.Parse(sqlWorker.selectData("SELECT COUNT(C.idMilitaryUnit) FROM " +
                    "MilitaryUnit M LEFT JOIN " +
                    "Crime C ON C.idMilitaryUnit = M.idMilitaryUnit AND C.isRegistred = 1 AND C.dateRegistration " +
                    "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "' AND C.idClause = NULL").Rows[0][0].ToString());

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
                para1.Range.Text = "Анализ преступлений и происшествий за " +
                    ((dateLeft == "" && dateRight == "") ? "все время" :
                        ("период " +
                        (dateLeft == "" ? "" : "c " + dateLeft + " г.") +
                        (dateRight == "" ? "" : " по " + dateRight + " г.")));
                para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                para1.Range.InsertParagraphAfter();
                para1.Range.InsertParagraphAfter();
                
                para1.Range.Text = "Всего преступлений и происшествий: " + (countCrimes + countIncidents) + "(" + countCrimes + "преступлений, " + countIncidents +
                    " происшествий).";
                para1.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para1.Range.Font.Size = 14;
                para1.Range.InsertParagraphAfter();
                para1.Range.InsertParagraphAfter();


                //анализ по частям
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
                            "BETWEEN '" + dateLeft + "' AND '" + (dateRight == "" ? "9999.99.99" : dateRight) + "' AND C.idClause = NULL " +
                        "GROUP BY M.number, M.shortName " +
                        "ORDER BY M.idMilitaryUnit;");

                    for (int i = 0; i < tableCrimes.Rows.Count; i++)
                    {
                        para1.Range.Text = "- войсковая часть " + tableCrimes.Rows[i][0] + " (" + tableCrimes.Rows[i][1] + "): " +
                            tableCrimes.Rows[i][2] + " преступлений, " + tableIncidents.Rows[i][2] + " происшествий";
                        para1.Range.Font.Size = 14;
                        para1.Range.InsertParagraphAfter();
                    }
                }



                winword.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
