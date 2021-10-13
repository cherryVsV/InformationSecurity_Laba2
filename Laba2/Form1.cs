using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace Laba2
{
    public partial class Form1 : Form
    {
        private static bool text_is_loaded = false;
        private static bool words_is_loaded = false;
        private static string text_for_hack = String.Empty;
        private static List<string> words = new List<string>();
        private static int lengthKeys = 8;
        private static int Number_matching_words = 0;
        private static bool flag_end_hack = false;
        private static int length_text = 0;
        private static int count_words = 0;
        private static decimal attempt = 0;
        // параметры взлома
        private int A = 1; //3
        private int B =1; //5
        private int m=1; //4
        private int Y0=0; //2
        List<string> success_words = new List<string>();
        Dictionary<string, decimal> dict = new Dictionary<string, decimal>();
        public Form1()
        {
            InitializeComponent();
        }
        public bool isOddNumber(int num) // нечетное число
        {
            if (num % 2 == 1)
            {
                return true;
            }
            return false;
        }
        public bool isPowerTwo(int num) // степень двойки
        {
            return (num != 0) && ((num & (num - 1)) == 0);
        }

        public int IsCoprime(int a, int b) // наибольший общий делитель == 1
        {
            while (b != 0)
            {
                var t = b;
                b = a % b;
                a = t;
            }
            return a;
        }
        public bool isCorrectY0(int y, int m)
        {
            return (0 <= y && y < m);
        }

        public void hack()
        {
            while (!flag_end_hack)
            {

                try
                {
                    string timeNow = DateTime.Now.ToString("G");
                    attempt++;
                    if (!dict.ContainsKey(timeNow))
                    {
                        dict.Add(timeNow, attempt);
                        chart1.Invoke(new Action(()=>chart1.Series[0].Points.AddXY(timeNow, attempt)));
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    success_words.Clear();
                    textBox1.Invoke(new Action(() => textBox1.Text = A.ToString()));
                    textBox2.Invoke(new Action(() => textBox2.Text = m.ToString()));
                    textBox3.Invoke(new Action(() => textBox3.Text = B.ToString()));
                    textBox4.Invoke(new Action(() => textBox4.Text = Y0.ToString()));
                    string result = String.Empty;
                    int[] Keys = new int[8];
                    Keys[0] = Y0;
                    for (int i = 1; i < Keys.Length; i++)
                    {
                        Keys[i] = (A * Keys[i - 1] + B) % m;
                    }
                    ///взлом
                    for (int i = 0; i < length_text; i += 8)
                    {
                        string temp = text_for_hack.Substring(i, 8);
                        for (int k = 0; k < temp.Length; k++)
                        {
                            result += (char)(temp[k] ^ Keys[k]);
                        }
                    }
                    //MessageBox.Show(result);
                    //richTextBox2.Text = result;
                    List<string> words_in_decrypted_text = result.Split(' ').ToList();
                    Number_matching_words = 0;
                    for (int i = 0; i < words_in_decrypted_text.Count; i++)
                    {
                        if (words.Contains(words_in_decrypted_text[i].ToLower()))
                        {
                            if (!success_words.Contains(words_in_decrypted_text[i].ToLower()))
                            {
                                success_words.Add(words_in_decrypted_text[i].ToLower());
                                Number_matching_words++;
                            }
                        }
                    }
                    if (Number_matching_words >= count_words)
                    {
                        richTextBox2.Invoke(new Action(() => richTextBox2.Text = result.Trim()));                    
                        chart1.Invoke(new Action(() => chart1.Series[0].Points.AddXY(DateTime.Now.ToString("G"), attempt)));
                        flag_end_hack = true;
                    }
                    else
                    {
                        Y0++;
                        if (!isCorrectY0(Y0, m))
                        {
                            Y0 = 0;
                            while (true)
                            {
                                try
                                {
                                    B++;
                                    if (IsCoprime(B, m) == 1)
                                    {
                                        break;
                                    }
                                }
                                catch (OverflowException ex1)
                                {
                                    try
                                    {
                                        B = 1;
                                        m *= 2;
                                    }
                                    catch (OverflowException ex2)
                                    {
                                        MessageBox.Show(ex2.Message);
                                        try
                                        {
                                            m = 1;
                                            A += 2;
                                        }
                                        catch (OverflowException ex3)
                                        {
                                            MessageBox.Show("Взлом не удался");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Legends[0].Title = "График зависимости количества попыток от времени";
            chart1.Series[0].LegendText = "График зависимости количества попыток от времени";
            chart1.Series[0].IsVisibleInLegend = false;
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
        }

        private void загрузитьСловарьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                words.Clear();
                dataGridView1.Rows.Clear();
                if (dataGridView1.Columns.Count == 0)
                {
                    DataGridViewTextBoxColumn column0 = new DataGridViewTextBoxColumn();
                    column0.Name = "Words";
                    column0.HeaderText = "Слова";
                    dataGridView1.Columns.Add(column0);
                }
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    words = File.ReadAllLines(ofd.FileName, Encoding.Default).ToList();
                    
                    for (int i = 0; i < words.Count; i++)
                    {
                        words[i] = words[i].ToLower();
                        dataGridView1.Rows.Add(words[i].ToLower());
                    }
                    numericUpDown1.Minimum = 1;
                    numericUpDown1.Maximum = words.Count;
                    groupBox1.Visible = true;
                    words_is_loaded = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void загрузитьТекстДляВзломаToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Stream fs = new FileStream(ofd.FileName, FileMode.Open);
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                        if (sr.CurrentEncoding.ToString() == "System.Text.UTF8Encoding")
                        {
                            text_for_hack = sr.ReadToEnd();
                            richTextBox1.Text = text_for_hack;
                            text_is_loaded = true;
                        }
                        else
                        {
                            MessageBox.Show("Выберите текстовый файл с кодировкой UTF8");
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (words_is_loaded && text_is_loaded)
            {
                numericUpDown1.Enabled = false;
                button1.Enabled = false;
                button1.Text = "Идёт взлом...";
                menuStrip1.Visible = false;
                dataGridView1.Enabled = false;
                richTextBox1.Enabled = false;
                richTextBox2.Enabled = false;
                count_words = Convert.ToInt32(numericUpDown1.Value);
                text_for_hack = richTextBox1.Text;
                length_text = richTextBox1.Text.Length;
                groupBox2.Visible = true;
                Task task = new Task(hack);
                task.Start();
            }
            else
            {
                MessageBox.Show("Для начала взлома загрузите текст для взлома и словарь");
            }
        }
    }
}
