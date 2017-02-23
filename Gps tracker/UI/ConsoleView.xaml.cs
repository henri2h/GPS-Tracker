using System;
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
        string text { get; set; }
        public ConsoleView()
        {
            this.InitializeComponent();
        }

        public void WriteLine(string text)
        {
            this.text += text + Environment.NewLine;
            if (this.text.Length > 200)
            {
                this.text = this.text.Substring(this.text.Length - 200);
            }

            try
            {
                var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ConsoleUItextBox.Text = this.text;
                    //  ConsoleUITextBoxScroll.ChangeView(ConsoleUITextBoxScroll.ScrollableHeight, 0, ConsoleUITextBoxScroll.ZoomFactor);
                });
            }
            catch (Exception ex)
            {
                ex.Source = "ConsoleView.WriteLine (Console)";
                ErrorMessage.printOut(ex);
            }
        }

    }
}
