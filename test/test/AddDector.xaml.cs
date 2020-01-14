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

namespace test
{
    /// <summary>
    /// Add.xaml 的交互逻辑
    /// </summary>
    public partial class AddDector : Window
    {
        public AddDector()
        {
            InitializeComponent();
        }
        string dectorname;
        string stationid;
     

        private void StationId_TextChanged(object sender, TextChangedEventArgs e)
        {
            stationid = StationId.Text;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Global.DectorNamelist.Add(dectorname);
            Global.StationIdlist.Add(stationid);
            Global.DectorIndexIdlist.Add(0);
          
            if (Global.DectorNamelist.Count > 0)
            {
                MessageBox.Show("添加成功");

                this.Close();
            }
        }

        //删除
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DectorName_TextChanged(object sender, TextChangedEventArgs e)
        {
            dectorname = DectorName.Text;
        }
    }
}
