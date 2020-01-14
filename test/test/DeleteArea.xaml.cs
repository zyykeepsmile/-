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
    /// DeleteArea.xaml 的交互逻辑
    /// </summary>
    public partial class DeleteArea : Window
    {
        public DeleteArea()
        {
            InitializeComponent();
        }
        string n;
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            n = nn.Text;
        }
        
        //确定
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //deletename = n;
            for (int i = 0; i < Global.namelist.Count; i++)
            {
                if (Global.namelist[i] == n )
                {
                    Global.namelist.Remove(Global.namelist[i]);
                    Global.lonlist.Remove(Global.lonlist[i]);
                    Global.latlist.Remove(Global.latlist[i]);
                    Global.radiuslist.Remove(Global.radiuslist[i]);
                }
            }
            this.Close();
        }
        //取消
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
    }
}
