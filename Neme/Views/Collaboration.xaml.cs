using Neme.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;


namespace Neme.Views
{
    /// <summary>
    /// Interaction logic for Collaboration.xaml
    /// </summary>
    public partial class Collaboration : UserControl
    {

        ObservableCollection<SharedFile> sharedFiles;
       public string CurrentUser = "";
        public Collaboration()
        {
            sharedFiles = new ObservableCollection<SharedFile>();
            InitializeComponent();
            LoadFiles();
            CurrentUser = GetLoggedInUser();
        }

        private void InviteUser_Click(object sender, RoutedEventArgs e)
        {
            if (SharedFilesListBox.SelectedItem is SharedFile selectedFile)
            {
                var inviteWindow = new InviteUser(selectedFile.Id);
                inviteWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a file to invite collaborators.");
            }
        }

        private void LoadFiles()
        {
            var SharedFolder = System.IO.Directory.GetFiles("C:\\Users\\Public\\Documents");
            foreach(var file in SharedFolder)
            {

                sharedFiles.Add(new SharedFile
                {
                    FileName = System.IO.Path.GetFileName(file),
                    FilePath = File.Exists(file)? System.IO.Path.GetDirectoryName(file): "",
                    OwnerUsername = GetLoggedInUser(),
                    Id = Guid.NewGuid(),
                    UploadedAt = System.IO.File.GetLastWriteTime(file)
                });
            }
            SharedFilesListBox.Items.Add(sharedFiles);
        }

        private string GetLoggedInUser()
        {
            if(this.Parent is MainView mainView)
            {
                 return mainView.Username;
               
            }
            return "Adm";
        }
    }
}
