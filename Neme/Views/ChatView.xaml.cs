using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using Neme.ViewModels;


namespace Neme.Views
{
    /// <summary>
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : UserControl
    {
        public ChatView()
        {
            InitializeComponent();
            DataContext = new ChatViewModel();
        }
        private void OpenFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
            }
        }

        private void ListBox_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }

        private void ShowEmojiPicker(object sender, RoutedEventArgs e)
        {
            if (EmojiPanel.Children.Count == 0)
            {
                LoadEmojis();
            }
            EmojiPopup.IsOpen = !EmojiPopup.IsOpen;
        }

        private void LoadEmojis()
        {
            string[] emojis = new string[]
            {
        "😀", "😁", "😂", "🤣", "😃", "😄", "😅", "😆", "😉", "😊", "😋", "😎", "😍", "😘", "🥰",
        "😗", "😙", "😚", "🙂", "🤗", "🤩", "🤔", "🤨", "😐", "😑", "😶", "🙄", "😏", "😣", "😥"
                // Add all Unicode emojis here or load from a file
            };
            Style EmojiStyle = (Style)this.FindResource("EmojiButton");
            foreach (string emoji in emojis)
            {
                Button emojiButton = new()
                {
                    Content = emoji,
                    Width = 40,
                    Height = 40,
                    Margin = new Thickness(1),
                    Style = EmojiStyle

                };

                emojiButton.Click += InsertEmoji;
                
                EmojiPanel.Children.Add(emojiButton);
            }
        }

        private void InsertEmoji(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                MessageInput.Text += btn.Content.ToString();
            }
            EmojiPopup.IsOpen = false;
        }

        private void OpenFileDialog(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Select a file to send",
                Filter = "Images and Documents (*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.pdf;*.docx;*.xlsx)|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.pdf;*.docx;*.xlsx|All Files (*.*)|*.*",
                Multiselect = false
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filePath = dialog.FileName;

                // Check file size
                const long maxSizeInMB = 5;
                FileInfo fileInfo = new FileInfo(filePath);
                long sizeInMB = fileInfo.Length / (1024 * 1024);

                if (sizeInMB > maxSizeInMB)
                {
                    MessageBox.Show($"The selected file exceeds the {maxSizeInMB} MB limit.", "File Too Large", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Proceed to send or open file
             //   OpenFile(filePath);
                // Or pass file to ViewModel
                // ((ChatViewModel)this.DataContext).SendFile(filePath);
            }

        }
    }
}
