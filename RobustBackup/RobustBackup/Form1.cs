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
                    textBox1.Text = "No folder selected";
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
                    textBox2.Text = "No folder selected";
                }
            }
        }
        private void CopyDirectoryRecursively(string sourceDir, string destDir)
        {
            Alphaleonis.Win32.Filesystem.DirectoryInfo dir = new Alphaleonis.Win32.Filesystem.DirectoryInfo(sourceDir);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory is not found: {dir.FullName}");
            }

            // Copiar todos os arquivos
            Alphaleonis.Win32.Filesystem.FileInfo[] files = dir.GetFiles();
            foreach (Alphaleonis.Win32.Filesystem.FileInfo file in files)
            {
                string targetFilePath = Alphaleonis.Win32.Filesystem.Path.Combine(destDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            Alphaleonis.Win32.Filesystem.DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (Alphaleonis.Win32.Filesystem.DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Alphaleonis.Win32.Filesystem.Path.Combine(destDir, subDir.Name);
                Alphaleonis.Win32.Filesystem.Directory.CreateDirectory(newDestinationDir);
                CopyDirectoryRecursively(subDir.FullName, newDestinationDir);
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
                MessageBox.Show($"Error to delete folder: {ex.Message}");
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
                    // Get Directories
                    string sourcePath = textBox1.Text;
                    string destPath = textBox2.Text;

                    DateTime dateTime = DateTime.Today;
                    string data = dateTime.ToString("yyyyMMdd");

                    // Get the source folder name
                    string sourceFolderName = Alphaleonis.Win32.Filesystem.Path.GetFileName(sourcePath.TrimEnd(Alphaleonis.Win32.Filesystem.Path.DirectorySeparatorChar));
                    string foldername = sourceFolderName + "_" + data;
                    string destFolder = Alphaleonis.Win32.Filesystem.Path.Combine(destPath, foldername);

               
                    Alphaleonis.Win32.Filesystem.Directory.CreateDirectory(destFolder);
                    CopyDirectoryRecursively(sourcePath, destFolder);
                    string[] destinationFolders = Alphaleonis.Win32.Filesystem.Directory.GetDirectories(destPath, sourceFolderName + "_*");
                    if (destinationFolders.Length > 2)
                    {
                        var oldestFolder = destinationFolders
                            .Select(f => new Alphaleonis.Win32.Filesystem.DirectoryInfo(f))
                            .OrderBy(d => d.CreationTime)
                            .First();

                        DeleteOldestFolderWithAlphaFS(oldestFolder.FullName);
                    }
                });
                MessageBox.Show("Backup finished");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                progressBar.Visible = false;
            }
        }
        
    }
}
