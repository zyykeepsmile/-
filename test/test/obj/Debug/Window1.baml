   M S B A M L   `   `   `  ����?  ;test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:testtest  5   	       test.Window1   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   [ WEsri.ArcGISRuntime, Version=100.6.0.0, Culture=neutral, PublicKeyToken=8fc3cc631e44ad866esri+http://schemas.esri.com/arcgis/runtime/2013  5   	   localclr-namespace:test  5   	   -   5          � Title$  Detector��5
   	   $	��800��6   $
��1200��6'    � WindowStartupLocation$ CenterScreen=�5   	    � WindowState$ Normal=�5   	    � 	Resources 5         test.MapViewModel   MapViewModel��%   &	          5   
   6   5      .��5      � -   5        $��MyGrid��5       � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   ,  &Esri.ArcGISRuntime.UI.Controls.MapView  -   5          Name$ 	MyMapView��5      $��0��6*   
   Map. 5      $  Esri.ArcGISRuntime.Mapping.Map     OperationalLayers. 5      1  +Esri.ArcGISRuntime.Mapping.ArcGISTiledLayer  -   5        Basemap1��   Source$^ Xhttp://cache1.arcgisonline.cn/arcgis/rest/services/ChinaOnlineCommunity_Mobile/MapServer?�5   (   5      5      .  (Esri.ArcGISRuntime.UI.Controls.SceneView  5   
   -   5        	  Name$	 MySceneView��5      $��1��6/   o� 5   
   -   5        
 o�Name$

 menu��5      $
��Left=�6   $��30��68   $
��1000��6D   $��0,0,0,0q�6Q   $	��Top=�6b   .@�5      m� -   5        $��主页��5      $��30��6:   $��60��6F   $��14(�6Q   5       m� 5!      -   5        $��	探空仪��5!      $��30��6>   $��60��6J   $��14(�6U   5"      m� 5#      -	   5        $��	敏感区��5#      $��30��6>   $��60��6J   $��14(�6U   5$      5&      �� 5'   
   $��1q�6   $��1��6%   $��Right=�65   $	��200��6Q   $��
53.2,0,0,0q�6^   $	��Top=�6r   $	��100��6�   ��5)      �� 5*       �� Color$ White��6"    �� Opacity$	 0.7Q�60   ._�5,      �� $��5q�6   .+�5-      �  5/      �� 51      $��* �6+   �� 53      $��40 �6+   57      �� 59      �� 5;      $��16(�6   $��探空仪编号:��6&   �� 5=      -
   5         ��Name$ 	NameLabel��5=      �� 5?      $��16(�6   $��AboveSeaLevel:��6&   �� 5A      -   5        $ AboveSeaLevelLabel��5A      �� 5C      $��16(�6   $��
longitude:��6&   �� 5D      -   5        $ LonLabel��5D      �� 5E      $��16(�6   $��	Latitude:��6&   �� 5F      -   5        $ LatLabel��5F      5G      5K      5O      5P      