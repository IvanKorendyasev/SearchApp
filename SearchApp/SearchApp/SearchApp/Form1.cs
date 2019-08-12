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
using System.Threading;

namespace SearchApp
{
    public partial class Form1 : Form
    {
        string directory;
        
        Thread searching;

        
        public Form1()
        {
            InitializeComponent();
            searching = new Thread(Search);
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.ShowDialog();
            directory = FBD.SelectedPath;
            textBox3.Text = directory;
            button1.Enabled = true;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (searching.IsAlive)
            {
                searching.Resume();
            }
            else
            {
                searching = new Thread(Search);
                searching.Start();
            }
            button1.Enabled = false;
            button2.Enabled = true;
            button4.Enabled = true;


        }

        public void Search()
        {
            int progress;
            TimeSpan ts;
            DateTime t1, t2;
            string time;
            t1 = DateTime.Now;
            int count = 0;
            string filename;
            string nametype = "*";
            
            if ((textBox4.Text != ""))
            {
                nametype = "*." + textBox4.Text;
            }
            string[] files = Directory.GetFiles(directory, nametype, SearchOption.AllDirectories);
            foreach (string file in files)
            {
                count++;
                bool matchtext = false;
                bool matchname = false;
                bool hasnamecriteria = false;
                if (textBox1.Text != "") //проверка отсутствия критериев текста, имени или типа файла
                {
                    hasnamecriteria = true;
                }
                else
                {
                    matchname = true;
                }

                if (textBox2.Text == "")
                {
                    matchtext = true;
                }

                if ((hasnamecriteria) & (file.Contains(textBox1.Text))) //проверка имени
                {
                    matchname = true;
                }

                filename = file.Insert(2, "\\"); //корректный путь к файлу
                try
                {
                    StreamReader SR = new StreamReader(filename, Encoding.GetEncoding("windows-1251"));
                    if (SR.ReadToEnd().Contains(textBox2.Text))
                    {
                        matchtext = true;
                    }
                }
                catch (Exception)
                {

                }
                if ((matchtext) & (matchname))
                { //вывод пути к файлу, если удовлетворяет требованиям
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            listBox1.Items.Add(file);
                        }
                        ));
                    }
                }
                progress = (count * 100) / files.Length;
                t2 = DateTime.Now;
                ts = t2 - t1;
                time = ts.Minutes.ToString() + "мин. " + ts.Seconds.ToString() + "сек.";
                displayresult(count, progress, file, time);
            }

        }

        public void displayresult(int counttoshow, int progresstoshow, string currentfile, string timetoshow)
        { //вывод текущей информации о поиске
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    label11.Text = timetoshow;
                    progressBar1.Value = progresstoshow;
                    label10.Text = currentfile;
                    label12.Text = counttoshow.ToString();
                }
                ));
            }

        }
        private void Button2_Click(object sender, EventArgs e)
        {
            searching.Suspend();
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (searching.ThreadState.ToString() == "Suspended")
            {
                searching.Resume();
            }
            searching.Abort();
            Properties.Settings.Default.name = textBox1.Text;
            Properties.Settings.Default.type = textBox4.Text;
            Properties.Settings.Default.directory = textBox3.Text;
            Properties.Settings.Default.text = textBox2.Text;
            Properties.Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.name;
            textBox4.Text = Properties.Settings.Default.type;
            textBox3.Text = Properties.Settings.Default.directory;
            directory = Properties.Settings.Default.directory;
            textBox2.Text = Properties.Settings.Default.text;
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (searching.ThreadState.ToString()=="Suspended")
            {
                searching.Resume();
            }
            searching.Abort();
            button1.Enabled = true;
            button2.Enabled = false;
            button4.Enabled = false;
            listBox1.Items.Clear();
            label11.Text = "";
            progressBar1.Value = 0;
            label10.Text = "";
            label12.Text = "";

        }
    }
}
