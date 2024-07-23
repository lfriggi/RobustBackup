using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AutoReforcedBAckup
{
    public partial class Form1 : Form
    {
        private string sourceFolder;
        private string destinationFolder;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Inicialização, se necessário
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    sourceFolder = folderDialog.SelectedPath;
                    textBox1.Text = sourceFolder;
                }
                else
                {
                    textBox1.Text = "Nenhuma pasta Selecionada";
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    destinationFolder = folderDialog.SelectedPath;
                    textBox2.Text = destinationFolder;
                }
                else
                {
                    textBox2.Text = "Nenhuma pasta Selecionada";
                }
            }
        }

        private void DeleteOldestFolderWithAlphaFS(string path)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir a pasta: {ex.Message}");
            }
        }
        private async void button3_Click(object sender, EventArgs e)
        {
            progressBar.Visible = true;
            progressBar.Style = ProgressBarStyle.Marquee;

            try
            {
                await Task.Run(() => { 
                string[] folders = Directory.GetDirectories(textBox1.Text);
                string[] files = Directory.GetFiles(textBox1.Text);

                
                DateTime dateTime = DateTime.Today;
                string data = dateTime.ToString("yyyyMMdd");
                //Criar pasta com data atual
                string sourceFolderName = Path.GetFileName(textBox1.Text);
                string foldername = sourceFolderName + "_" + data;
                string dest = Path.Combine(textBox2.Text, foldername);
                Directory.CreateDirectory(dest);

                // Apagar a pasta mais antiga se houver mais de três
                string[] destinationFolders = Directory.GetDirectories(textBox2.Text, sourceFolderName + "_*");
                if (destinationFolders.Length > 2)
                {
                    var oldestFolder = destinationFolders
                        .Select(f => new DirectoryInfo(f))
                        .OrderBy(d => d.CreationTime)
                        .First();

                    DeleteOldestFolderWithAlphaFS(oldestFolder.FullName);
                }

                // Copy files
                foreach (string file in files)
                {
                    string filename = Path.GetFileName(file);
                    string desti = Path.Combine(textBox2.Text, filename);
                    File.Copy(file, desti, true);
                }

                });
                MessageBox.Show("Arquivos e pastas copiados com sucesso.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao copiar arquivos e pastas: {ex.Message}");
            }
        }
    


    private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar_Click(object sender, EventArgs e)
        {

        }
    }
}
