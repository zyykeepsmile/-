   M S B A M L   `   `   `  ����?  ;test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:testtest  5   	       test.DectorInfo   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   $localclr-namespace:test     5   	     � Title$  
DectorInfo��5   	   $	��450��6   $	��800��6)   .��5	      � .+�5
   
   o� -   5         o�Name$
 menu��5
      $
��Left=�6   $��20��68   $��0,0,0,0q�6D   $	��Top=�6U   .@�5      m� -   5        $��主页��5      5      m� 5      -   5        $��轨迹显示��5      5      m� 5      -   5        $��	敏感区��5      5      5      &   System.Windows.Controls.DataGrid  5   
   -   5        $��	dataGrid1��5         AutoGenerateColumns .  $��185,139,126.6,61q�6X   ��6$   ��    ColumnHeaderStyle 5      �� 5       �� 
TargetType=  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5      ��  �� Property � �� �� Value$ 24(�61   �� 5       � ��$ Center=�6A   6R   �� 5       � ��$ Center=�6C   6T   5         Columns 5"      0  *System.Windows.Controls.DataGridTextColumn  5#      ,  &System.Windows.Controls.DataGridColumn   Header$ 探空仪编号��6%   	   Width5  /System.Windows.Controls.DataGridLengthConverter$		 150 64   
   FontSize$
 20Q�6�   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6@   �� *na+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�     5$      $ 放球站站号��6%   $		 150 64   $
 20Q�6�    6@   �� *si+$ TwoWay=�$ PropertyChanged=�6�   5(      �� 5)   
   $
��Left=�6    �� TextAlignment$ Center=�60   $��
120,30,0,0q�6G   $
��Wrap=�6[   $��探空仪信息��6o   $�� 宋体)�6|   $	�� 50(�6�   $	��Top=�6�   $��57��6�   $	��561��6�   �� 5+   
   -   5        $��添加��5+      $
��Left=�6   $��184,106,0,0q�69   $	��Top=�6N   $��75��6f   6q   �� 5,   
   -   5        $��删除��5,      $
��Left=�6   $��264,106,0,0q�69   $	��Top=�6N   $��75��6f   6q   5-      5.      