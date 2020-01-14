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
    /// DeleteDector.xaml 的交互逻辑
    /// </summary>
    public partial class DeleteDector : Window
    {
        public DeleteDector()
        {
            InitializeComponent();
        }
        string d;
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            d = dd.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < Global.DectorNamelist.Count; i++)
            {
                if (Global.DectorNamelist[i]==d){
                    Global.DectorNamelist.Remove(d);
                    Global.StationIdlist.Remove(Global.StationIdlist[i]);
                    Global.DectorNamelist.Remove(Global.DectorNamelist[i]);
                }
            }
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
