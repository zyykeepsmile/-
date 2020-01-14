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
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using System.Drawing;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.UI.Controls;
using System.Reflection;
using System.IO;
using Esri.ArcGISRuntime.UI.GeoAnalysis;
using Color = System.Drawing.Color;
using MySql.Data.MySqlClient;
using System.Data;
using System.Timers;
using Esri.ArcGISRuntime.Location;
using System.Collections;
using Esri.ArcGISRuntime.UI;
using System.Diagnostics;

namespace test
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        //=========三维场景地图图层========
        // URL for a scene service of buildings in Brest, France
        private string _buildingsServiceUrl = @"https://tiles.arcgis.com/tiles/P3ePLMYs2RVChkJx/arcgis/rest/services/Buildings_Brest/SceneServer/layers/0";

        // URL for an image service to use as an elevation source
        private string _elevationSourceUrl = @"https://scene.arcgis.com/arcgis/rest/services/BREST_DTM_1M/ImageServer";

        // URL to the elevation service - provides terrain elevation
        private readonly Uri _elevationServiceUrl = new Uri("https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer");


        //敏感区
        //private readonly List<MapPoint> _artificialMapPoints = new List<MapPoint>();
        private List<List<MapPoint>> MapPointslist = new List<List<MapPoint>>();

        //预测轨迹

        // Randomizer. Provide a seed if you want predictable behavior.
        private readonly Random _randomizer = new Random();

        // Index keeps track of where on the fake track you are.
       // private int _locationIndex = 0;
        //private int dectorindex = 0;
        private Timer _animationTimer;

        // Camera controller for centering the camera on the airplane
        //private OrbitGeoElementCameraController _orbitCameraController;


        Camera observerCamera;

        private double heading;  
       //轨迹层
        private List<GraphicsOverlay> Dector2Doverlay = new List<GraphicsOverlay>();
        private List<GraphicsOverlay> Dector3Doverlay = new List<GraphicsOverlay>();
        //探空仪
        private List<Graphic> Dector2D = new List<Graphic>();
        private List<Graphic> Dector3D = new List<Graphic>();
        
        //连接数据库
        string constr = "server=localhost;User Id=root;password=root;Database=detector";

        //========================经纬度转墨卡托======================

        private double lon(double lon)
        {
            return lon * 20037508.34 / 180;

        }
        private double lat(double lat)
        {

            double y = Math.Log(Math.Tan((90 + lat) * Math.PI / 360)) / (Math.PI / 180);
            return y * 20037508.34 / 180;
        }


        //=========================墨卡托转经纬度================
        private double Mlon(double mlon)
        {
            return mlon / 20037508.34 * 180;

        }
        private double Mlat(double mlat)
        {

            double y = mlat / 20037508.34 * 180;
            return 180 / Math.PI * (2 * Math.Atan(Math.Exp(y * Math.PI / 180)) - Math.PI / 2);
        }


        public Window1()
        {
            InitializeComponent();
            //最大化窗体
            this.WindowState = WindowState.Maximized;
            //初始地图中心
            MyMapView.SetViewpointCenterAsync(new MapPoint(11800000, 4500000, SpatialReferences.WebMercator), 3e7);
            Initialize();                      
        }




        //============地图展示
        private async void Initialize()
        {          
            LoadData();
           //=====================放球站=============
            // Create overlay to where graphics are shown
            GraphicsOverlay overlay = new GraphicsOverlay();
            MyMapView.GraphicsOverlays.Add(overlay);
            try
            {
                await CreatePictureMarkerSymbolFromResources(overlay);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error");
            }

            //================== 二维场景探空仪展示 ==============
            //加载图片

           
            int count = Global.DectorNamelist.Count;
            if (count > 0)
            {            
                for (int i =0; i < count; i++)
                {
                    Assembly currentAssembly = Assembly.GetExecutingAssembly();
                    // Get image as a stream from the resources
                    // Picture is defined as EmbeddedResource and DoNotCopy
                    Stream resourceStream = currentAssembly.GetManifestResourceStream(
                        this.GetType(), "qiqiu.png");
                    // Create new symbol using asynchronous factory method from stream
                    PictureMarkerSymbol pinSymbol = await PictureMarkerSymbol.CreateAsync(resourceStream);
                    pinSymbol.Width = 35;
                    pinSymbol.Height = 35;
                    //位置
                    int index = 0;
                    double positionOffset = 0.01 * index;
                    MapPoint point = new MapPoint(111,30,0, SpatialReferences.Wgs84);
                    // Create the graphic from the geometry and the symbol.
                     Graphic _plane2D;
                     _plane2D = new Graphic(point, pinSymbol);
                     // Add the graphic to the overlay.
                     overlay.Graphics.Add(_plane2D);
                     Dector2D.Add(_plane2D);
                  
                }

            }

            //==========================场景基础地图===========
            // Create a new Scene with an imagery basemap
            Scene myScene = new Scene(Basemap.CreateImagery());

            // Create a scene layer to show buildings in the Scene
            ArcGISSceneLayer buildingsLayer = new ArcGISSceneLayer(new Uri(_buildingsServiceUrl));
            myScene.OperationalLayers.Add(buildingsLayer);

            // Create an elevation(海拔) source for the Scene
            ArcGISTiledElevationSource elevationSrc = new ArcGISTiledElevationSource(new Uri(_elevationSourceUrl));
            myScene.BaseSurface.ElevationSources.Add(elevationSrc);

            ElevationSource elevationSource = new ArcGISTiledElevationSource(_elevationServiceUrl);
            myScene.BaseSurface.ElevationSources.Add(elevationSource);
            // Add the Scene to the SceneView
            MySceneView.Scene = myScene;


            //======================三维探空仪====================
            // Create the graphics overlay.
            GraphicsOverlay overlay1 = new GraphicsOverlay();
            //=====放球站======
            try
            {
                await CreatePictureMarkerSymbolFromResources(overlay1);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error");
            }

            MySceneView.GraphicsOverlays.Add(overlay1);
            if (Global.DectorNamelist.Count > 0)
            {
                for (int i = 0; i < Global.DectorNamelist.Count; i++)
                {
                    GraphicsOverlay dectoroverlay = new GraphicsOverlay();

                    // Set the surface placement mode for the overlay.
                    dectoroverlay.SceneProperties.SurfacePlacement = SurfacePlacement.Absolute;
                    MapPoint point = new MapPoint(111, 30, 0, SpatialReferences.Wgs84);
                    SimpleMarkerSymbol circleSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Color.Blue, 20);

                    Graphic _plane3D;
                   // Create the graphic from the geometry and the symbol.
                    _plane3D = new Graphic(point, circleSymbol);

                    // Add the graphic to the overlay.
                    dectoroverlay.Graphics.Add(_plane3D);

                   // Show the graphics overlay in the scene.
                    MySceneView.GraphicsOverlays.Add(dectoroverlay);
                    Dector3D.Add(_plane3D);
                    /*
                     
                  _orbitCameraController = new OrbitGeoElementCameraController(_plane3D, 30.0)
                  {
                         CameraPitchOffset = 75.0
                  };
                  MySceneView.CameraController = _orbitCameraController;
                     */

                 

                }
            }

                    


            //===========跟随相机控制========
             


            //================加载探空仪位置信息===========
            



            //==================动态更新===============
            _animationTimer = new Timer(2000)
            {
                Enabled = true,
                AutoReset = true
            };

            //动态更新探空仪位置
            _animationTimer.Elapsed += AnimatePlane;

            //====二维预测轨迹=====

            //SimpleLineSymbol lineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, Color.FromArgb(0xFF, 0x80, 0x00, 0x80), 4);
            // _overlay.Renderer = new SimpleRenderer(lineSymbol);

            // MyMapView.GraphicsOverlays.Add(_overlay);
            //==========三维预测轨迹=========

            // SimpleLineSymbol lineSymbol1 = new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, Color.FromArgb(0xFF, 0x80, 0x00, 0x80), 4);
            //  _overlay1.Renderer = new SimpleRenderer(lineSymbol1);
            // // Set the surface placement mode for the overlay.
            for(int i = 0; i < Global.DectorNamelist.Count; i++)
            {
                GraphicsOverlay _overlay1 = new GraphicsOverlay();

                GraphicsOverlay _overlay = new GraphicsOverlay();

                 _overlay1.SceneProperties.SurfacePlacement = SurfacePlacement.Absolute;
                
                //MySceneView.GraphicsOverlays.Add(_overlay1);

                MySceneView.GraphicsOverlays.Add(_overlay1);
                MyMapView.GraphicsOverlays.Add(_overlay);
                Dector3Doverlay.Add(_overlay1);
                Dector2Doverlay.Add(_overlay);

            }




            //===================三维地图初始视点================
            heading = 344.488;
            // Set the viewpoint with a new camera focused on the castle in Brest
            observerCamera = new Camera(new MapPoint(112, 29, 2495, SpatialReferences.Wgs84), heading, 74.1212, 0.0);
            await MySceneView.SetViewpointCameraAsync(observerCamera);

            //====================通过鼠标点击改变三维地图视点==================
            MyMapView.GeoViewTapped += Map_Tapped;
            
             
              
              if (Global.namelist.Count > 0)
            {

                string[] strArr = new string[3];
                string sArguments = @"station.py"; //调用的python的文件名字
                strArr[0] = Global.lonlist[0].ToString();
                strArr[1] = Global.latlist[0].ToString();
                strArr[2] = Global.radiuslist[0].ToString();
                RunPythonScript(sArguments, "-u", strArr);
            }            
      
        }

        //====================从数据库中加载数据================
        private void LoadData()
        {
            MySqlConnection connection = new MySqlConnection(constr);
            try
            {
                connection.Open();
                Console.WriteLine("已经建立连接");
                MySqlCommand cmd = connection.CreateCommand();
                if (Global.DectorNamelist.Count > 0)
                {                       
                    for(int i = 0; i <Global.DectorNamelist.Count; i++)                   
                    {
                       //cmd.CommandText = "select * from predictlocation where name ='" + Global.DectorNamelist[i]+ "' and time >'"+ DateTime.Now.ToLocalTime().AddMinutes(-2).ToString()  + "'and time <='" + DateTime.Now.ToLocalTime().AddMinutes(-1).ToString() + "' ";
                        //MessageBox.Show(DateTime.Now.ToLocalTime().AddMinutes(-1).ToString()+ DateTime.Now.ToLocalTime().AddMinutes(-2).ToString());
                        cmd.CommandText = "select * from predictlocation where name ='" + Global.DectorNamelist[i]+"'";
                        MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
                        DataSet ds = new DataSet();                     
                        adap.Fill(ds);
                        List<MapPoint> _artificialMapPoints = new List<MapPoint>();
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                        // MessageBox.Show(" "+ds.Tables[0].Rows.Count);
                            foreach (DataRow r in ds.Tables[0].Rows)
                            {
                                string lon = r["lon"].ToString();
                                string lat = r["lat"].ToString();
                                string alevel = r["alevel"].ToString();
                                //Console.WriteLine(lon + " " + lat);
                                MapPoint point = new MapPoint(double.Parse(lon), double.Parse(lat), double.Parse(alevel));

                                _artificialMapPoints.Add(point);                           
                            }
                           MapPointslist.Add(_artificialMapPoints);
                        }                      
                    }               
                }          
            }
            catch (Exception)
            {
                Console.WriteLine("连接错误");
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();

                }
            }

        }
        //=================探空仪位置更新====================
        private void AnimatePlane(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            //LoadData();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (Global.namelist.Count > 0)
                {
                    var ait = Global.namelist.GetEnumerator();
                    var bit = Global.lonlist.GetEnumerator();
                    var cit = Global.latlist.GetEnumerator();
                    var dit = Global.radiuslist.GetEnumerator();
                    while (ait.MoveNext() && bit.MoveNext() && cit.MoveNext() && dit.MoveNext())
                    {
                        double lo = double.Parse(bit.Current.ToString());
                        double la = double.Parse(cit.Current.ToString());
                        double ra = double.Parse(dit.Current.ToString());
                        Createarea(lo, la, ra);
                    }
                }
            }));
            //if (_randomizer.Next(0, 10) > 8)
            //{
            // Don't send an update about 20% of the time.
            //   return;
            // }
            // _locationIndex++;
           
            int cnt = 0;
            MapPoint p= observerCamera.Location;
            double dis = 999999999;


            if (Dector2D.Count > 0 && MapPointslist.Count>0)
            {
                int count = Dector2D.Count;
                
                for (int i = 0; i < count; i++)
                {
                    int index = Global.DectorIndexIdlist[i];
                 
                    List<MapPoint> _artificialMapPoints = new List<MapPoint>();
                    if (MapPointslist[i].Count>0)
                    {
                        _artificialMapPoints = MapPointslist[i];
                        if(index > _artificialMapPoints.Count)
                        {
                            Global.DectorNamelist.Remove(Global.DectorNamelist[i]);
                            Global.StationIdlist.Remove(Global.StationIdlist[i]);
                            Global.DectorIndexIdlist.Remove(Global.DectorIndexIdlist[i]);
                        }
                        else
                        {
                            MapPoint selectedMapPoint = _artificialMapPoints[index % _artificialMapPoints.Count];


                            double dis1 = Distance(p.X, p.Y, p.Z, selectedMapPoint.X, selectedMapPoint.Y, selectedMapPoint.Z);
                            if (dis1 < dis)
                            {
                                cnt = i;
                                dis = dis1;
                            }
                            Graphic name2D = Dector2D[i];
                            name2D.Geometry = new MapPoint(selectedMapPoint.X, selectedMapPoint.Y, spatialReference: SpatialReference.Create(wkid: 4326));
                            Graphic name3D = Dector3D[i];
                            name3D.Geometry = new MapPoint(selectedMapPoint.X, selectedMapPoint.Y, selectedMapPoint.Z);

                            //=====预测轨迹显示====
                            // Create a purple simple line symbol
                            SimpleLineSymbol lineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, Color.FromArgb(0xFF, 0x80, 0x00, 0x80), 3);

                            SimpleLineSymbol lineSymbol1 = new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, Color.Red, 3);

                            Esri.ArcGISRuntime.Geometry.PointCollection points = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                            Esri.ArcGISRuntime.Geometry.PointCollection points1 = new Esri.ArcGISRuntime.Geometry.PointCollection();
                            if(index+120< _artificialMapPoints.Count)
                            {
                                 for (int j = index; j < index+120; j++)
                                 {
                                    // Get the next point .
                                    MapPoint selectedMapPoint1 = _artificialMapPoints[j % _artificialMapPoints.Count];
                                    points.Add(new MapPoint(selectedMapPoint1.X, selectedMapPoint1.Y, spatialReference: SpatialReference.Create(wkid: 4326)));
                                    points1.Add(new MapPoint(selectedMapPoint1.X, selectedMapPoint1.Y, selectedMapPoint1.Z));

                                 };                                                 
                            }
                        
                            // Create the polyline from the point collection
                            Esri.ArcGISRuntime.Geometry.Polyline polyline = new Esri.ArcGISRuntime.Geometry.Polyline(points);
                            Esri.ArcGISRuntime.Geometry.Polyline polyline1 = new Esri.ArcGISRuntime.Geometry.Polyline(points1);
                            GraphicsOverlay _overlay = Dector2Doverlay[i];
                            GraphicsOverlay _overlay1 = Dector3Doverlay[i];
                            _overlay.Graphics.Clear();
                            _overlay1.Graphics.Clear();
                            // Create the graphic with polyline and symbol
                            Graphic graphic = new Graphic(polyline, lineSymbol);
                            Graphic graphic1 = new Graphic(polyline1, lineSymbol1);
                            // Add graphic to the graphics overlay
                            _overlay.Graphics.Add(graphic);
                            _overlay1.Graphics.Add(graphic1);
                        }
                        
                    }
                }
            }
            if(MapPointslist.Count > 0)
            {
               List<MapPoint> _artificialMapPoints2 = new List<MapPoint>();
                _artificialMapPoints2 = MapPointslist[cnt];
                // Get the next point .
                MapPoint selectedMapPoint2 = _artificialMapPoints2[Global.DectorIndexIdlist[cnt] % _artificialMapPoints2.Count];
                string n = Global.DectorNamelist[cnt];
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    NameLabel.Text = n;
                    AboveSeaLevelLabel.Text = "  " + selectedMapPoint2.Z + "m";
                    LonLabel.Text = "  " + selectedMapPoint2.X;
                    LatLabel.Text = "  " + selectedMapPoint2.Y;
                }));
            }
            if (Global.DectorIndexIdlist.Count > 0)
            {
                for(int k = 0; k < Global.DectorIndexIdlist.Count; k++)
                {
                     Global.DectorIndexIdlist[k] = Global.DectorIndexIdlist[k] + 120;
                }
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
                // MessageBox.Show(output + output.GetType());
                // p.WaitForExit();//关键，等待外部程序退出后才能往下执行}
                // Console.Write(output);//输出
                //p.Close();
               //MessageBox.Show("Exception Occurred ");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception Occurred : " + ex.Message + ", " + ex.StackTrace.ToString());
            }


        }


        //===================更新MyScene视点位置============
        private async void Map_Tapped(object sender, GeoViewInputEventArgs e)
        {
            // Dismiss any existing callouts.
            MyMapView.DismissCallout();

            // Get the normalized geometry for the tapped location and use it as the feature's geometry.
            MapPoint tappedPoint = (MapPoint)GeometryEngine.NormalizeCentralMeridian(e.Location);
            // Set the viewpoint with a new camera focused on the castle in Brest
            observerCamera = new Camera(new MapPoint(Mlon(tappedPoint.X), Mlat(tappedPoint.Y), 100, SpatialReferences.Wgs84), heading, 74.1212, 0.0);

            // MapPoint p = observerCamera.Location;
            await MySceneView.SetViewpointCameraAsync(observerCamera);

        }



        //==========================放球站标记========================
        private async Task CreatePictureMarkerSymbolFromResources(GraphicsOverlay overlay)
        {
            // Get current assembly that contains the image
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            // Get image as a stream from the resources
            // Picture is defined as EmbeddedResource and DoNotCopy
            Stream resourceStream = currentAssembly.GetManifestResourceStream(
               this.GetType(), "pin_star_blue.png");

            // Create new symbol using asynchronous factory method from stream
            PictureMarkerSymbol pinSymbol = await PictureMarkerSymbol.CreateAsync(resourceStream);
            pinSymbol.Width = 50;
            pinSymbol.Height = 50;

            // Create location for the pint
            MapPoint pinPoint1 = new MapPoint(lon(111.35000), lat(30.73000), SpatialReferences.WebMercator);
            MapPoint pinPoint2 = new MapPoint(lon(114.05000), lat(30.60000), SpatialReferences.WebMercator);
            MapPoint pinPoint3 = new MapPoint(lon(112.78000), lat(28.010000), SpatialReferences.WebMercator);
            MapPoint pinPoint4 = new MapPoint(lon(115.00000), lat(25.87000), SpatialReferences.WebMercator);
            MapPoint pinPoint5 = new MapPoint(lon(115.90110), lat(28.59000), SpatialReferences.WebMercator);
            MapPoint pinPoint6 = new MapPoint(lon(116.97000), lat(30.62000), SpatialReferences.WebMercator);
            // Create graphic with the location and symbol
            Graphic pinGraphic1 = new Graphic(pinPoint1, pinSymbol);
            Graphic pinGraphic2 = new Graphic(pinPoint2, pinSymbol);
            Graphic pinGraphic3 = new Graphic(pinPoint3, pinSymbol);
            Graphic pinGraphic4 = new Graphic(pinPoint4, pinSymbol);
            Graphic pinGraphic5 = new Graphic(pinPoint5, pinSymbol);
            Graphic pinGraphic6 = new Graphic(pinPoint6, pinSymbol);
            // Add graphic to the graphics overlay
            overlay.Graphics.Add(pinGraphic1);
            overlay.Graphics.Add(pinGraphic2);
            overlay.Graphics.Add(pinGraphic3);
            overlay.Graphics.Add(pinGraphic4);
            overlay.Graphics.Add(pinGraphic5);
            overlay.Graphics.Add(pinGraphic6);
        }

       

        //=========================键盘控制三维场景视点=====================
        private void MainWindows_Keydown(object sender, KeyEventArgs e)
        {


            if (e.KeyStates == Keyboard.GetKeyStates(Key.A))
            {
                e.Handled = true;
                MapPoint p = observerCamera.Location;
                observerCamera = new Camera(new MapPoint(p.X - 0.1, p.Y, p.Z, SpatialReferences.Wgs84), heading, 74.1212, 0.0);
                MySceneView.SetViewpointCameraAsync(observerCamera);
                //MessageBox.Show("当前位置"+p.X+", "+ p.Y+", "+p.Z);
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.D))
            {
                e.Handled = true;
                MapPoint p = observerCamera.Location;
                observerCamera = new Camera(new MapPoint(p.X + 0.1, p.Y, p.Z, SpatialReferences.Wgs84), heading, 74.1212, 0.0);
                MySceneView.SetViewpointCameraAsync(observerCamera);
                // MessageBox.Show("当前位置" + p.X + ", " + p.Y + ", " + p.Z);
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.W))
            {
                e.Handled = true;
                MapPoint p = observerCamera.Location;
                observerCamera = new Camera(new MapPoint(p.X, p.Y + 0.1, p.Z, SpatialReferences.Wgs84), heading, 74.1212, 0.0);
                MySceneView.SetViewpointCameraAsync(observerCamera);
                // MessageBox.Show("当前位置" + p.X + ", " + p.Y + ", " + p.Z);
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.S))
            {
                e.Handled = true;
                MapPoint p = observerCamera.Location;
                observerCamera = new Camera(new MapPoint(p.X, p.Y - 0.1, p.Z, SpatialReferences.Wgs84), heading, 74.1212, 0.0);
                MySceneView.SetViewpointCameraAsync(observerCamera);
                // MessageBox.Show("当前位置" + p.X + ", " + p.Y + ", " + p.Z);
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Down))
            {
                e.Handled = true;
                MapPoint p = observerCamera.Location;
                observerCamera = new Camera(new MapPoint(p.X, p.Y, p.Z + 100, SpatialReferences.Wgs84), heading, 74.1212, 0.0);
                MySceneView.SetViewpointCameraAsync(observerCamera);
                // MessageBox.Show("当前位置" + p.X + ", " + p.Y + ", " + p.Z);
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Up))
            {
                e.Handled = true;
                MapPoint p = observerCamera.Location;
                observerCamera = new Camera(new MapPoint(p.X, p.Y, p.Z - 100, SpatialReferences.Wgs84), heading, 74.1212, 0.0);
                MySceneView.SetViewpointCameraAsync(observerCamera);
                // MessageBox.Show("当前位置" + p.X + ", " + p.Y + ", " + p.Z);
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Left))
            {
                e.Handled = true;
                MapPoint p = observerCamera.Location;

                if (heading > 360)
                {
                    heading = heading - 360;
                }

                observerCamera = new Camera(new MapPoint(p.X, p.Y, p.Z, SpatialReferences.Wgs84), heading - 10, 74.1212, 0.0);
                MySceneView.SetViewpointCameraAsync(observerCamera);
                // MessageBox.Show("当前位置" + p.X + ", " + p.Y + ", " + p.Z);
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Right))
            {
                e.Handled = true;
                MapPoint p = observerCamera.Location;

                if (heading > 360)
                {
                    heading = heading - 360;
                }
                observerCamera = new Camera(new MapPoint(p.X, p.Y, p.Z, SpatialReferences.Wgs84), heading + 10, 74.1212, 0.0);
                MySceneView.SetViewpointCameraAsync(observerCamera);
                // MessageBox.Show("当前位置" + p.X + ", " + p.Y + ", " + p.Z);
            }

        }


      

            //-======主页====
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            
            MainWindow win = new MainWindow();
            this.Close();
            win.Show();
        }
        public double fabs(double a )
        {
            if (a < 0)
                return -a;
            else return a;
        }

        //=======距离计算=======
        public double Distance(double lon1, double lat1, double high1,double lon2,double lat2, double high2)
        {
            double EARTH_RADIUS = 6378137;// 地球半径
            double radLat1 = lat1 * Math.PI / 180.0;
            double radLon1 = lon1 * Math.PI / 180.0;
            double radLat2 = lat2 * Math.PI / 180.0;
            double radLon2 = lon2 * Math.PI / 180.0;
            if (radLat1 < 0)
                radLat1 = Math.PI / 2 + fabs(radLat1);
            if (radLat1 > 0)
                radLat1 = Math.PI / 2 - fabs(radLat1);

            if (radLon1 < 0)
                radLon1 = Math.PI * 2 - fabs(radLon1);
            if (radLat2 < 0)
                radLat2 = Math.PI / 2 + fabs(radLat2);
            if (radLat2 > 0)
                radLat2 = Math.PI / 2 - fabs(radLat2);
            if (radLon2 < 0)
                radLon2 = Math.PI * 2 - fabs(radLon2);

            double x1 = EARTH_RADIUS * Math.Cos(radLon1) * Math.Sin(radLat1);

            double y1 = EARTH_RADIUS * Math.Sin(radLon1) * Math.Sin(radLat1);

            double z1 = EARTH_RADIUS * Math.Cos(radLat1);

            double x2 = EARTH_RADIUS * Math.Cos(radLon2) * Math.Sin(radLat2);

            double y2 = EARTH_RADIUS * Math.Sin(radLon2) * Math.Sin(radLat2);

            double z2 = EARTH_RADIUS * Math.Cos(radLat2);

            double d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2));

            double theta = Math.Acos(
                (EARTH_RADIUS * EARTH_RADIUS + EARTH_RADIUS * EARTH_RADIUS - d * d) / (2 * EARTH_RADIUS * EARTH_RADIUS));

            double dist = theta * EARTH_RADIUS;
            double high = fabs(high1 - high2);
            
            return Math.Sqrt(dist * dist + high * high);


        }

        //=======敏感区=====
        public void Createarea(double lon, double lat, double radius)
        {
           
            // Create the graphics overlay.
            GraphicsOverlay overlay1 = new GraphicsOverlay();
            GraphicsOverlay overlay2 = new GraphicsOverlay();
            // Create a green simple line symbol
            SimpleLineSymbol outlineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, System.Drawing.Color.FromArgb(0xFF, 0x00, 0x50, 0x00), 1);

            // Create a green mesh simple fill symbol
            SimpleFillSymbol fillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Cross, System.Drawing.Color.Yellow, outlineSymbol);
            Esri.ArcGISRuntime.Geometry.PointCollection boatPositions = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
            for (int i = 0; i < 36; i++)
            {

                double angle = (2 * Math.PI / 36) * i;
                // new MapPoint([lon + Math.sin(angle) * radius[0], lat[0] + Math.cos(angle) * radius[0]]);
                boatPositions.Add(new MapPoint(lon + Math.Sin(angle) * radius, lat + Math.Cos(angle) * radius));

            };

            // Create the polyline from the point collection
            Esri.ArcGISRuntime.Geometry.Polygon polygon = new Esri.ArcGISRuntime.Geometry.Polygon(boatPositions);

            // Create the graphic with polyline and symbol
            Graphic graphic = new Graphic(polygon, fillSymbol);
            Graphic graphic1 = new Graphic(polygon, fillSymbol);

            // Add graphic to the graphics overlay
            overlay1.Graphics.Add(graphic);
            overlay2.Graphics.Add(graphic1);
            //MessageBox.Show(lon + "  "+ lat + "添加成功");
            // Show the graphics overlay in the scene.
            MyMapView.GraphicsOverlays.Add(overlay1);
            MySceneView.GraphicsOverlays.Add(overlay2);


            int de=-1;
            int mi=-1;
            if (Dector2D.Count > 0)
            {
                int count = Dector2D.Count;

                for (int i = 0; i < count; i++)
                {
                    List<MapPoint> _artificialMapPoints = new List<MapPoint>();
                    

                    _artificialMapPoints = MapPointslist[i];

                    int index = Global.DectorIndexIdlist[i];
       
                    for (int j = index; j< index +120; j++)
                    {
                        MapPoint selectedMapPoint = _artificialMapPoints[j % _artificialMapPoints.Count];
                        double di = Distance(selectedMapPoint.X, selectedMapPoint.Y, selectedMapPoint.Z, lon, lat, 0);
                        if(di < radius * 100000)
                        {
                            de = i;
                            mi = j-index;
                            break;
                        }
                    }

                   // }
                    if (mi != -1)
                    {
                        break;
                    }
                }



               //double d = Distance(lon, lat, 0, lon1, lat1, radius1);
                StackPanel s = new StackPanel();
                Label l1 = new Label();
                Label l2 = new Label();
                Label l3 = new Label();
                s.Children.Add(l1);
                s.Children.Add(l2);
                s.Children.Add(l3);
                l1.Content = "longitude ：" + lon;
                l2.Content = "latitude : " + lat;
                if(de != -1)
                {
                   l3.Content ="探空仪 " +  Global.DectorNamelist[de] +" 将在"+ mi+"分钟后到达敏感区";
                }
                else
                {
                    /*
                     
                     string[] strArr = new string[3];
                    string sArguments = @"station.py"; //调用的python的文件名字
                    strArr[0] = lon.ToString();
                    strArr[1] = lat.ToString();
                    strArr[2] = (radius*111).ToString();
                    RunPythonScript(sArguments, "-u", strArr);
                     */



                    //int counter = 0;
                    //string line;
                    string result = "Please wait";
                    string[] lines = System.IO.File.ReadAllLines(@"E:\\VSProjects\\test\\data.txt", Encoding.GetEncoding("gb2312"));
                    //System.Console.WriteLine("Contents of WriteLines2.txt = ");
                   
                    foreach (string line in lines)
                    {
                        result = line;
                    }


                   // System.IO.StreamReader file = new System.IO.StreamReader(@"E:\\VSProjects\\test\\data.txt");

                   // while ((line = file.ReadLine()) != null)
                    //{
                        //System.Console.WriteLine(line);
                     //   counter++;
                   // }
                   /// MessageBox.Show( result);
                   // file.Close();
                    l3.Content = result;
                }

                MyMapView.ShowCalloutAt(new MapPoint(lon, lat, SpatialReferences.Wgs84), s);
            }
            else
            {
                //double d = Distance(lon, lat, 0, lon1, lat1, radius1);
                StackPanel s = new StackPanel();
                Label l1 = new Label();
                Label l2 = new Label();
                Label l3 = new Label();
                s.Children.Add(l1);
                s.Children.Add(l2);
                s.Children.Add(l3);
                l1.Content = "longitude ：" + lon;
                l2.Content = "latitude : " + lat;
                // l3.Content = "";

                /*
                 string[] strArr = new string[3];
                string sArguments = @"station.py"; //调用的python的文件名字
                strArr[0] = lon.ToString();
                strArr[1] = lat.ToString();
                strArr[2] = (radius * 111).ToString();
                RunPythonScript(sArguments, "-u", strArr);
                 */

                string result = "Please wait";
                string[] lines = System.IO.File.ReadAllLines(@"E:\\VSProjects\\test\\data.txt", Encoding.GetEncoding("gb2312"));
                //System.Console.WriteLine("Contents of WriteLines2.txt = ");

                foreach (string line in lines)
                {
                    result = line;
                }


                // System.IO.StreamReader file = new System.IO.StreamReader(@"E:\\VSProjects\\test\\data.txt");

                // while ((line = file.ReadLine()) != null)
                //{
                //System.Console.WriteLine(line);
                //   counter++;
                // }
               // MessageBox.Show("c" + result);
                // file.Close();
                l3.Content = "可选放球站：" + result;
                // string text = System.IO.File.ReadAllText(@"E:\\VSProjects\\test\\data.txt");



                MyMapView.ShowCalloutAt(new MapPoint(lon, lat, SpatialReferences.Wgs84), s);
            }






           
           // MyMapView.ShowCalloutAt(new MapPoint(lon, lat, SpatialReferences.Wgs84), s);



        }


        //查看敏感区
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
          

            Infor infor = new Infor();
            this.Close();
            infor.Show();
           
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            DectorInfo di = new DectorInfo();
            this.Close();
            di.Show();
        }

        

       /*  
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
             
             int count = Global.DectorNamelist.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    string[] strArr = new string[2];
                    string sArguments = @"test.py"; //调用的python的文件名字
                    strArr[0] = Global.DectorNamelist[i] + ".txt";
                    // strArr[1] = "3";
                    RunPythonScript(sArguments, "-u", strArr);
                }
            }           
            
        }  
         */

        
    }
}
