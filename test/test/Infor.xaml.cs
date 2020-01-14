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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.ComponentModel;

namespace test
{
    /// <summary>
    /// Infor.xaml 的交互逻辑
    /// </summary>
    public partial class Infor : Window
    {
        public Infor()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            showinfor();
        }
       // public event PropertyChangedEventHandler PropertyChanged;
        public class Member
        {
            public string Name { get; set; }
            public string Lon { get; set; }
            public string Lat { get; set; }
            public string Radius { get; set; }
        }
        //====添加======
      

       
        //添加
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddArea area = new AddArea();
            
           // Window1 win = new Window1();
            area.ShowDialog();
           
            showinfor();



        }
        private void showinfor()
        {
            ObservableCollection<Member> memberData = new ObservableCollection<Member>();
            memberData.Clear();
            if (Global.namelist.Count > 0)
            {
                var ait = Global.namelist.GetEnumerator();
                var bit = Global.lonlist.GetEnumerator();
                var cit = Global.latlist.GetEnumerator();
                var dit = Global.radiuslist.GetEnumerator();
                while (ait.MoveNext() && bit.MoveNext() && cit.MoveNext() && dit.MoveNext())
                {
                    memberData.Add(new Member()
                    {
                        Name = ait.Current.ToString(),
                        Lon = bit.Current.ToString(),
                        Lat = cit.Current.ToString(),
                        Radius = dit.Current.ToString()

                    });
                }
                
                dataGrid.DataContext = memberData;
            }
        }
        //删除
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DeleteArea da = new DeleteArea();
            da.ShowDialog();     
            int k = Global.namelist.Count;    
            showinfor();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            DectorInfo di = new DectorInfo();
            this.Close();
            di.Show();

        }
    }
}
