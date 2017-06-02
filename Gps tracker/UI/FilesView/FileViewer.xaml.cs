using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Gps_tracker.UI.FilesView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FileViewer : Page
    {
        protected Frame rootPage = Window.Current.Content as Frame;
        private DataTransferManager dataTransferManager;

        String fileContent;

        public FileViewer()
        {
            this.InitializeComponent();
            LoadContent();

            // Register the current page as a share source.
            this.dataTransferManager = DataTransferManager.GetForCurrentView();
            this.dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);


        }


        private void LoadContent()
        {
            fileContent = File.ReadAllText(AppCore.Core.SelectedFilePath);
            UITbFileContent.Text = fileContent;
        }

        private void UIBtShare_Click(object sender, RoutedEventArgs e)
        {
            // If the user clicks the share button, invoke the share flow programatically.
            DataTransferManager.ShowShareUI();
        }

        private async void GetShareContent(DataRequest request)
        {
            string dataPackageText = fileContent;
            StorageFile str = await Windows.Storage.StorageFile.GetFileFromPathAsync(AppCore.Core.SelectedFilePath);
            List<StorageFile> files = new List<StorageFile>();
            files.Add(str);
            if (!String.IsNullOrEmpty(dataPackageText))
            {
                DataPackage requestData = request.Data;
                requestData.Properties.Title = "Text";
                requestData.Properties.Description = "File detail"; // The description is optional.
                requestData.Properties.ContentSourceApplicationLink = ApplicationLink;

                
                //                requestData.SetText(dataPackageText);
                requestData.SetStorageItems(files);
            }
        }

        private void UIBtReturn_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            bool canChange = rootFrame.Navigate(typeof(FileView));
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            GetShareContent(args.Request);
        }

        protected Uri ApplicationLink
        {
            get
            {
                return GetApplicationLink(GetType().Name);
            }
        }



        public static Uri GetApplicationLink(string sharePageName)

        {

            return new Uri("ms-sdk-sharesourcecs:navigate?page=" + sharePageName);

        }
    }
}