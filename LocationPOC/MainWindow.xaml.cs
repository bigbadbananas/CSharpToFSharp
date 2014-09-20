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
using DataContracts;
using FSharpLib;
using System.Diagnostics;

namespace LocationPOC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LocationFilterMessage mFilter;

        public MainWindow()
        {
            InitializeComponent();
            ResultsButton.Visibility = Visibility.Hidden;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ResultText.Text = string.Empty;
                ResultsButton.Visibility = Visibility.Hidden;

                Stopwatch sw = new Stopwatch();

                sw.Start();
                mFilter = LocationCodeParser.ParseLocationCode(LocationEntry.Text, false);
                sw.Stop();

                ResultText.Text = "Parsing took " + sw.ElapsedMilliseconds + " milliseconds!";
                ResultsButton.Visibility = Visibility.Visible;
            }
        }

        private void ResultsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new LocationResultWindow(mFilter);
            window.Show();
        }
    }
}
