   M S B A M L   `   `   `  ����C  ?SZDC.WPF, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:SZDC.Wpf.ComponentsSZDC.Wpf.Components  5      9!clr-namespace:SZDC.Wpf.ConvertersSZDC.Wpf.Converters  5      5clr-namespace:SZDC.Wpf.DesignerSZDC.Wpf.Designer  5	      +    %SZDC.WPF.TrainGraph.TrainGraphControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      8
components!clr-namespace:SZDC.Wpf.Components     5      8
converters!clr-namespace:SZDC.Wpf.Converters     5      4designerclr-namespace:SZDC.Wpf.Designer     5	        � 	Resources  5      .   (SZDC.Wpf.Converters.CurrentTimeConverter   CurrentTimeConverter  CurrentTimeStyle �� 
TargetType �� Property �� Value��%�   &	        &	        5   
   6*   �� 5   
   " ��6)   .�5      ��  �@��TextAlignment$ Center=�6.   �� 5       � ��$ Center=�62   �� 5       � ��$ 0,5,0,5q�6'   69   5      5      .��5      �  � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      ��6   ��  �� Path$@ :Components.StationsSidebarComponent.RequiredComponentWidth� �� Mode$ OneWay=��� 5      $��* �6   �� 5      ��6   �� $L FComponents.RailwayDistanceComponent.RailwayDistanceColumnRequiredWidth�$ OneWay=�.+�5!   
   (� .��5"      �� #��[ 6   $��0��6�   $��0��6�   ��6B   �� *+)Tools.CurrentDateTimeTool.CurrentDateTime+ �� 	Converter# [  6�   5#      +   %SZDC.Wpf.Components.TimeAxisComponent  5%   
   $��0��6'   $��1��64   ��5&   '   �� 	 �� ElementName$	 TrainGraphComponent��$ Width�.   (SZDC.Wpf.Components.StationListComponent  5(   
   $��1��6*   $��0��67   ��5)   *   �� $	 TrainGraphComponent��$ Height�2   ,SZDC.Wpf.Components.RailwayDistanceComponent  5+   
   $��1��6.   $��2��6;   ��6K   �� $	 TrainGraphComponent��$ Height��� 5-   
   $��1��6   $��1��6   $��1q�6.   
���   �._�5.      -   'SZDC.Wpf.Components.TrainGraphComponent  -   5        
  Name$
 TrainGraphComponent��5.   -   5/      50      51      