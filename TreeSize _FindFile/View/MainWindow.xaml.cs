using System.Threading;
using System.Windows;
using TreeSize__FindFile.ModelView;
using TreeSize.ViewModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text.RegularExpressions;

namespace TreeSize__FindFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ChoiceFolder.Text = WorkWithFile.ReadPath();
            ChooseFile_TextBox.Text = WorkWithFile.ReadNameFile();
            if (ChoiceFolder.Text.Length != 0)
                CancelButton.IsEnabled = true;
        }

        CancellationTokenSource cts;
        ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        bool tokenButton;

        public void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openFileDlg = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (openFileDlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ChoiceFolder.Text = openFileDlg.FileName;
                
                CancelButton.IsEnabled = true;
                cts = new CancellationTokenSource();
            }
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            string choiceFolder = ChoiceFolder.Text;
            ViewItems _model = new ViewItems();
            Regex regex;
            MyViewModel start;

            if (tokenButton)
            {
                tokenButton = false;
                manualResetEvent.Reset();
                CancelButton.Content = "Start";
            }
            else
            {
                 if ((CancelButton.Content.ToString() == "Find") || (ChoiceFolder.Text != WorkWithFile.ReadPath()) || (ChooseFile_TextBox.Text != WorkWithFile.ReadNameFile()) || cts.Token.IsCancellationRequested)
                 {
                    regex = new Regex(@"(\w*)" + ChooseFile_TextBox.Text + @"(\w*)");
                    cts = new CancellationTokenSource();
                    start = new MyViewModel(_model, regex, cts, manualResetEvent);
                    start.GetItemFromPathAsync(choiceFolder);
                    WorkWithFile.SaveToFile(choiceFolder + "\n" + ChooseFile_TextBox.Text);
                    DataContext = _model;
                 }
                manualResetEvent.Set();
                tokenButton = true;
                CancelButton.Content = "Stop";
            }
        }
    }
}
