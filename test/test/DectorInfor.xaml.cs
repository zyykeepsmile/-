using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace test
{
    /// <summary>
    /// AddDector.xaml 的交互逻辑
    /// </summary>
    public partial class DectorInfo : Window
    {
        public DectorInfo()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            showinfor1();
        }
        //添加
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddDector ad = new AddDector();
            ad.ShowDialog();
            showinfor1();
        }
        public class DectorMember
        {
            public string na { get; set; }
            public string si { get; set; }
          
           
        }
        private void showinfor1()
        {
            ObservableCollection<DectorMember> memberData1 = new ObservableCollection<DectorMember>();
            memberData1.Clear();
            if (Global.DectorNamelist.Count > 0)
            {
                
                var ait = Global.DectorNamelist.GetEnumerator();
                var bit = Global.StationIdlist.GetEnumerator();
              
              
                while (ait.MoveNext() && bit.MoveNext() )
                {
                    memberData1.Add(new DectorMember()
                    {
                        na = ait.Current.ToString(),
                        si = bit.Current.ToString(),                  
                    });
                }

                dataGrid1.DataContext = memberData1;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            MainWindow win = new MainWindow();
            this.Close();
            win.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Window1 win1 = new Window1();
            this.Close();
            win1.Show();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Infor infor = new Infor();
            this.Close();
            infor.Show();

        }
    }
}
