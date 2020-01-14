using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// AddArea.xaml 的交互逻辑
    /// </summary>
    public partial class AddArea : Window
    {
        public AddArea()
        {
            InitializeComponent();
        }

       

        //List arealist = new List();
      
        string name1;
        string lon1;
        string lat1;
        string radius1;
        //敏感区编号
        private void Areaname_TextChanged(object sender, TextChangedEventArgs e)
        {
            //areaname = areaname.Text;
            name1 = areaname.Text;
        }
        //敏感区经度
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           // area.arealon = lon.Text;
            lon1 = lon.Text;
        }
        //敏感区纬度
        private void Lat_TextChanged(object sender, TextChangedEventArgs e)
        {
            lat1 = lat.Text;
            //area.arealat = lat.Text;
        }
        //敏感区半径
        private void Radius_TextChanged(object sender, TextChangedEventArgs e)
        {
            radius1 = radius.Text;
            //area.arearadius = radius.Text;
        }

        //添加
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Global.namelist.Add(name1);
            Global.lonlist.Add(lon1);
            Global.latlist.Add(lat1);
            Global.radiuslist.Add(radius1);
            
            

           // arealist.Add(area);
            if (Global.namelist.Count > 0)
            {
                MessageBox.Show("添加成功");
                //string[] strArr = new string[3];
                //string sArguments = @"station.py"; //调用的python的文件名字
                //strArr[0] = lon1.ToString();
                //strArr[1] = lat1.ToString();
                //strArr[2] = radius1.ToString();
               // RunPythonScript(sArguments, "-u", strArr);


                
                this.Close();
            }
        }


        public static void RunPythonScript(string sArgName, string args = "", params string[] teps)
        {




            try
            {
                Process p = new Process();
                //string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + sArgName;// 获得python文件的绝对路径（将文件放在c#的debug文件夹中可以这样操作）
                string path = @"E:\\VSProjects\\test\" + sArgName;
                p.StartInfo.FileName = @"D:\Users\13734\AppData\Local\Programs\Python\Python37\python.exe";//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
                string sArguments = path;
                foreach (string sigstr in teps)
                {
                    sArguments += " " + sigstr;//传递参数
                }

                p.StartInfo.Arguments = sArguments;

                p.StartInfo.UseShellExecute = false;

                p.StartInfo.RedirectStandardOutput = false;

                p.StartInfo.RedirectStandardInput = true;

                p.StartInfo.RedirectStandardError = true;

                p.StartInfo.CreateNoWindow = true;

                p.Start();



                //var output = p.StandardOutput.ReadToEnd();

                // output.GetType();
                //string output1 = p.OutputDataReceived();
                 //MessageBox.Show(output + output.GetType());
                 //p.WaitForExit();//关键，等待外部程序退出后才能往下执行}
                // Console.Write(output);//输出
                //p.Close();
                MessageBox.Show("Exception Occurred ");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception Occurred : " + ex.Message + ", " + ex.StackTrace.ToString());
            }


        }

        //取消
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        
    }
}
