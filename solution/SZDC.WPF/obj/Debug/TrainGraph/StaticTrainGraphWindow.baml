   M S B A M L   `   `   `  ����C  ?SZDC.WPF, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:SZDC.WpfSZDC.Wpf  5   	   9!clr-namespace:SZDC.Wpf.TrainGraphSZDC.Wpf.TrainGraph  5   	   5clr-namespace:SZDC.Wpf.DesignerSZDC.Wpf.Designer  5	   	   9!clr-namespace:SZDC.WPF.TrainGraphSZDC.WPF.TrainGraph  5
   	   0    *SZDC.Wpf.TrainGraph.StaticTrainGraphWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &wpfclr-namespace:SZDC.Wpf     5   	   8
timetables!clr-namespace:SZDC.Wpf.TrainGraph     5   	   4designerclr-namespace:SZDC.Wpf.Designer     5	   	   8
trainGraph!clr-namespace:SZDC.WPF.TrainGraph     5
   	   $
��1344��5   	   $	��864��6     � Title  5      `� 5   
    �� StringFormat$  {0} railway {1}, {2} - {3}��6   .1�5      ��  �� Path$! TimetableInfo.TimetableType�6   �� 5      $! TimetableInfo.RailwayNumber�6   �� 5      $$ TimetableInfo.FirstStationName�6   �� 5      $# TimetableInfo.LastStationName�6   5      .��5      �  � RowDefinitions 5   
   �� 5      $��70 �6   �� 5      $��* �6   �� 5      $��40 �6    � ColumnDefinitions 5   
   �� 5      $��* �6   .+�5"   
   *   $SZDC.Wpf.TrainGraph.TrainGraphHeader  $��0��6&   +   %SZDC.WPF.TrainGraph.TrainGraphControl  5#   
   $��1��6'   �� 5$   
   $��2��6   ��6    �� $ Errors�5%      5&      