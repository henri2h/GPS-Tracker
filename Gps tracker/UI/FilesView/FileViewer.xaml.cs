﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public FileViewer()
        {
            this.InitializeComponent();
            LoadContent();
        }
        private void LoadContent()
        {
            string content = File.ReadAllText(AppCore.Core.selectedFilePath);
            UITbFileContent.Text = content;
        }
        private void UIBtShare_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UIBtReturn_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            bool canChange = rootFrame.Navigate(typeof(FileView));
        }
    }
}