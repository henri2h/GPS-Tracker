using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Gps_tracker.UI.FilesView
{
    public sealed partial class FileElement : Button
    {
        public string name { get; set; }
        public string path { get; set; }
        public bool isDirectory { get; set; }

        public FileElement(string name, string path, bool isDirectory)
        {
            this.InitializeComponent();
            this.name = name;
            this.path = path;
            this.isDirectory = isDirectory;
            UITbName.Text = name;

            if (isDirectory)
            {

                UIImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/appbar.folder.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                UIImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/appbar.page.png", UriKind.RelativeOrAbsolute));
            }
        }
    }
}
