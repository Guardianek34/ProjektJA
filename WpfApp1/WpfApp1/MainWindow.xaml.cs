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
using System.Drawing;
using System.Windows.Interop;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public string fileName { get; set; } = string.Empty;
        public bool? isAsm { get; set; } = null;

        private string _time = string.Empty;
        public string time
        {
            get { return _time; }
            set
            {
                if (_time != value)
                {
                    _time = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainWindow()
        {
            this.DataContext = this;
            InitializeComponent();
            slValue.Value = (int)Environment.ProcessorCount;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void getFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "All supported graphics(*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|" +
            "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
            "Portable Network Graphic (*.png)|*.png|"+
            "Bitmaps (*.bmp)|*.bmp";
            openFileDialog.ShowDialog();

            if(openFileDialog.FileName == string.Empty)
            {
                getFile.Content = "You haven't selected a file. Try again!";
            }
            else
            {
                this.fileName = openFileDialog.FileName;
                imgPhoto.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void invokeProgram_Click(object sender, RoutedEventArgs e)
        {
            object unboxedLibrary = this.isAsm;
            if(this.fileName != string.Empty && unboxedLibrary != null)
            {
                
                runProgram.Content = "Thanks for using the program!";
                TaskManager program = new(this.fileName, (bool)unboxedLibrary, (int)slValue.Value);
                //(Bitmap, double) tmpT = program.manageAlgorithm();
                //this.time = Convert.ToString(tmpT.Item2) + "ms";
                //convertedPhoto.Source = ToBitmapImage(tmpT.Item1);
                program.generateBenchmark();
            }
            else
            {
                runProgram.Content = "Not all of the required fields " + Environment.NewLine + "are filled. Try again!";
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // splitting the string so that we get only ASM/C#
            // instead of some object description bullshit + language
            String selectedLib = chosenLanguage.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
            this.isAsm = selectedLib == "C#" ? false : true;
        }

        private void saveFile_Click(object sender, RoutedEventArgs e)
        {
            // Prepare a dummy string, thos would appear in the dialog
            string dummyFileName = "Save Here";

            SaveFileDialog sf = new SaveFileDialog();
            // Feed the dummy name to the save dialog
            sf.FileName = dummyFileName;
            sf.Filter = "All supported graphics(*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|" +
            "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
            "Portable Network Graphic (*.png)|*.png|" +
            "Bitmaps (*.bmp)|*.bmp";

            if (sf.ShowDialog() == true)
            {
                // Now here's our save folder
                string savePath = System.IO.Path.GetDirectoryName(sf.FileName);
                // Do whatever
            }
        }

        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

    }
}