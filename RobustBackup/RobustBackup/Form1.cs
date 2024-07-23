using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Alphaleonis.Win32.Filesystem; // Alphaleonis namespace para operações de arquivos com caminhos longos

namespace RobustBackup
{
    public partial class Form1 : Form
    {
        private string sourceFolder = String.Empty;
        private string destinationFolder = String.Empty;

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
                Alphaleonis.Win32.Filesystem.Directory.Delete(path, true);
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
                await Task.Run(() =>
                {
                    // Obter diretórios e arquivos usando Alphaleonis
                    string[] folders = Alphaleonis.Win32.Filesystem.Directory.GetDirectories(textBox1.Text);
                    string[] files = Alphaleonis.Win32.Filesystem.Directory.GetFiles(textBox1.Text);

                    DateTime dateTime = DateTime.Today;
                    string data = dateTime.ToString("yyyyMMdd");

                    // Criar pasta com data atual
                    string sourceFolderName = Alphaleonis.Win32.Filesystem.Path.GetFileName(textBox1.Text);
                    string foldername = sourceFolderName + "_" + data;
                    string dest = Alphaleonis.Win32.Filesystem.Path.Combine(textBox2.Text, foldername);
                    Alphaleonis.Win32.Filesystem.Directory.CreateDirectory(dest);

                    // Apagar a pasta mais antiga se houver mais de três
                    string[] destinationFolders = Alphaleonis.Win32.Filesystem.Directory.GetDirectories(textBox2.Text, sourceFolderName + "_*");
                    if (destinationFolders.Length > 2)
                    {
                        var oldestFolder = destinationFolders
                            .Select(f => new Alphaleonis.Win32.Filesystem.DirectoryInfo(f))
                            .OrderBy(d => d.CreationTime)
                            .First();

                        DeleteOldestFolderWithAlphaFS(oldestFolder.FullName);
                    }

                    // Copiar arquivos
                    foreach (string file in files)
                    {
                        string filename = Alphaleonis.Win32.Filesystem.Path.GetFileName(file);
                        string desti = Alphaleonis.Win32.Filesystem.Path.Combine(dest, filename);
                        Alphaleonis.Win32.Filesystem.File.Copy(file, desti, true);
                    }
                });
                MessageBox.Show("Arquivos e pastas copiados com sucesso.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao copiar arquivos e pastas: {ex.Message}");
            }
            finally
            {
                progressBar.Visible = false;
            }
        }
    }
}
