using Esri.ArcGISRuntime.UI;
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

namespace test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
        }
        //探空仪轨迹
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window1 win1 = new Window1();
            this.Close();
            win1.Show();
        }
        //选择放球站
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Infor infor = new Infor();
            this.Close();
            infor.Show();

        }
       
       
        //探空仪
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            DectorInfo di = new DectorInfo();
            this.Close();
            di.Show();
        }



        // Map initialization logic is contained in MapViewModel.cs
    }
}
