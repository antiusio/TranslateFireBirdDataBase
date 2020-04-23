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
using Translate;
using DataBase;
using System.IO;
using System.Threading;

namespace TranslateBase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainDataContext mainDataContext;
        WorkWithBase w;
        public MainWindow()
        {
            
            w = new WorkWithBase(Directory.GetCurrentDirectory() + "\\dataBase\\TEMV1.GDB");
            mainDataContext = new MainDataContext(w);
            //mainDataContext.Countries = w.Countries;
            //mainDataContext.TranslatedCountries = w.TranslatedCountries;
            DataContext = mainDataContext;

            InitializeComponent();
            
        }
    }
}
