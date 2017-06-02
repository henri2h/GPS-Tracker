using System;
using System.AppCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Gps_tracker.UI
{
    public sealed partial class ConsoleView : UserControl
    {
        String FileName
        {
            get => AppCore.Core.FileName; set => AppCore.Core.FileName = value;
        }

        const int maxLenght = 800;
        public string Text { get; set; }
        public ConsoleView()
        {
            if (FileName == null) FileName = Files.GetFileName("Console", "consoleReport", ".list");
            this.InitializeComponent();
            Console.consoles.Add(this);
        }

        public void WriteLine(string text)
        {
            this.Text += text + Environment.NewLine;
            if (this.Text.Length > maxLenght)
            {
                this.Text = this.Text.Substring(this.Text.Length - maxLenght);
            }
            try
            {
                if (FileName != null)
                {
                    // TODO : detect if the file is currently under writing
                    File.AppendAllText(FileName, "[" + DateTime.Now.ToString() + "] : " + text + Environment.NewLine);

                }
            }
            catch (Exception ex)
            {
                ex.Source = "ConsoleView.WriteLine";
                ErrorMessage.SaveOut(ex);
            }

            try
            {
                var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ConsoleUItextBox.Text = this.Text;
                    //  ConsoleUITextBoxScroll.ChangeView(ConsoleUITextBoxScroll.ScrollableHeight, 0, ConsoleUITextBoxScroll.ZoomFactor);
                });
            }
            catch (Exception ex)
            {
                ex.Source = "ConsoleView.WriteLine (Console)";
                ErrorMessage.PrintOut(ex);
            }
        }
        private void ConsoleUIbtReturn_Click(object sender, RoutedEventArgs e)
        {
            string comm = ConsoleUIReadBox.Text;
            var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Console.setNewLine(comm); });
            ConsoleUIReadBox.Text = "";
        }
        public void Clear()
        {
            ConsoleUItextBox.Text = "";
        }
    }
}
