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
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using System.IO;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double Time { get; set; } = 11;
        public string Filename { get; set; } = string.Empty;
        public bool? IsAsm { get; set; } = null;
        public int AmountOfThreads { get; set; }


        public MainWindow()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        private void getFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = ".bmp Files|*.bmp";
            openFileDialog.ShowDialog();

            if(openFileDialog.FileName == string.Empty)
            {
                getFile.Content = "You haven't selected a file. Try again!";
            }
            else
            {
                getFile.Content = openFileDialog.FileName;
                this.Filename = openFileDialog.FileName;
            }
        }

        private void invokeProgram_Click(object sender, RoutedEventArgs e)
        {
            if(this.Filename != string.Empty && this.IsAsm != null)
            {
                runProgram.Content = "Thanks for using the program!";
                this.AmountOfThreads = (int)slValue.Value; // amount of cores to use

                ThreadManager program = new(this.Filename, this.IsAsm, this.AmountOfThreads);
            }
            else
            {
                runProgram.Content = "Not all of the required fields are filled. Try again!";
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // splitting the string so that we get only ASM/C#
            // instead of some object description bullshit + language
            String selectedLib = chosenLanguage.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
            this.IsAsm = selectedLib == "C#" ? false : true;
        }
    }
}
