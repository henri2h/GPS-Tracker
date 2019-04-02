using System;
using System.AppCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Console = System.AppCore.Console;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Gps_tracker.UI.FilesView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FileView : Page
    {

        bool FViewLoaded
        {
            get => AppCore.Core.FViewLoaded; set => AppCore.Core.FViewLoaded = value;
        }

        List<directoryObject> objects
        {
            get => AppCore.Core.FViewObj; set => AppCore.Core.FViewObj = value;
        }

        public FileView()
        {
            this.InitializeComponent();
            LoadPage();
            ButtonGoBack();
        }

        public string CurrentPath { get => AppCore.Core.CurrentPath; set { AppCore.Core.CurrentPath = value; } }

        public void LoadPage()
        {

            Console.WriteLine("Loading files of : " + CurrentPath);

            if(FViewLoaded == false)
             objects = FilesManager.ListFiles(CurrentPath);
            FViewLoaded = true;


            UIStackFiles.Children.Clear();
            foreach (directoryObject dir in objects)
            {
                FileElement fileElem = new FileElement(dir.name, dir.path, dir.isDirectory)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                fileElem.Click += FileElem_Click;

                UIStackFiles.Children.Add(fileElem);
            }
            ButtonGoBack();
        }
        void ButtonGoBack()
        {
            if (CurrentPath == AppCore.Core.localFolder.Path) { UIBtParent.IsEnabled = false; }
            else { UIBtParent.IsEnabled = true; }
        }

        private void FileElem_Click(object sender, RoutedEventArgs e)
        {
            FileElement fElem = (FileElement)sender;
            if (fElem.isDirectory == true)
            {
                CurrentPath = fElem.path;
                FViewLoaded = false;
                LoadPage();
            }
            else
            {
                AppCore.Core.SelectedFilePath = fElem.path;
                Frame rootFrame = Window.Current.Content as Frame;
                bool canChange = rootFrame.Navigate(typeof(FileViewer));
            }
        }

        private void UITbParent_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo dF = Directory.GetParent(CurrentPath);
            CurrentPath = dF.FullName;
            FViewLoaded = false;
            LoadPage();
        }

        private void RetrunToHomeView_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            bool canChange = rootFrame.Navigate(typeof(MainPage));
        }
    }
}
