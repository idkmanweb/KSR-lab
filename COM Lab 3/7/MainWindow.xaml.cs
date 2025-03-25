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
using System.Windows.Forms.Integration;
using AxWMPLib;
using AxAcroPDFLib;
using System.Windows.Forms;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SHDocVw.WebBrowser axWeb;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Integration.WindowsFormsHost host =
            new System.Windows.Forms.Integration.WindowsFormsHost();
            AxWindowsMediaPlayer axWmp = new AxWindowsMediaPlayer();
            host.Child = axWmp;
            this.grid1.Children.Add(host);
            axWmp.URL = @"C:\Users\potek\source\repos\WpfApp4\WpfApp4\Ring01.wav";

            System.Windows.Forms.Integration.WindowsFormsHost host1 =
            new System.Windows.Forms.Integration.WindowsFormsHost();
            AxAcroPDF axPdf = new AxAcroPDF();
            host1.Child = axPdf;
            this.grid2.Children.Add(host1);
            axPdf.LoadFile(@"C:\Users\potek\source\repos\WpfApp4\WpfApp4\Get_Started_With_Smallpdf.pdf");

            System.Windows.Forms.Integration.WindowsFormsHost host2 =
            new System.Windows.Forms.Integration.WindowsFormsHost();
            System.Windows.Forms.WebBrowser webBrowser = new System.Windows.Forms.WebBrowser();
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.Navigate("https://www.google.com");

            host2.Child = webBrowser;
            grid3.Children.Add(host2);

        }
    }
}
