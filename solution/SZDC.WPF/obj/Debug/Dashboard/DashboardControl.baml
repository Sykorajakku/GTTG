   M S B A M L   `   `   `  ����C  ?SZDC.WPF, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:SZDC.Wpf.DesignerSZDC.Wpf.Designer  5      9!clr-namespace:SZDC.Wpf.ConvertersSZDC.Wpf.Converters  5      F BSZDC.Editor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\=clr-namespace:SZDC.Editor.ModelProviders;assembly=SZDC.EditorSZDC.Editor.ModelProviders 5      7 clr-namespace:SZDC.WPF.DashboardSZDC.WPF.Dashboard  5	      T9clr-namespace:SZDC.Editor.Interfaces;assembly=SZDC.EditorSZDC.Editor.Interfaces 5
      )    #SZDC.WPF.Dashboard.DashboardControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      4designerclr-namespace:SZDC.Wpf.Designer     5      8
converters!clr-namespace:SZDC.Wpf.Converters     5      XmodelProviders=clr-namespace:SZDC.Editor.ModelProviders;assembly=SZDC.Editor     5      6	dashboard clr-namespace:SZDC.WPF.Dashboard     5	      P
interfaces9clr-namespace:SZDC.Editor.Interfaces;assembly=SZDC.Editor     5
        � 	Resources  5      -   'SZDC.Wpf.Converters.IsSelectedConverter   IsSelectedConverter1   +SZDC.Wpf.Converters.RailwaySegmentConverter  RailwaySegmentConverter.   (SZDC.Wpf.Converters.OpenCommandConverter  OpenCommandConverter  TitleTextBlockStyleKey �� 
TargetType �� Property �� Value  DefaultTextBlockStyleKey  SelectedTimetableTextBlock � RowDefinitions �� StringFormat �� 	Converter  SelectableContentStyle �� Triggers �� Value	 �� Binding
 `� 	Converter �� Path��%�  &	        &	      &	 &     &	 9     &	 �     &	 r    7  7  7  &	 �    7   7  7     5   
   6)     5   
   6-     5   
   6*   �� 5   
   " ��6/   .�5      ��  � ��$ 0q�6'   �� 5       � ��$ 18(�6)   �� 5       � ��$ Center=�62   5      �� 5   
   " ��61   .�5      ��  � ��$ 13(�6)   �� 5       �@��TextAlignment$ Center=�6.   �� 5       � ��$ Center=�62   5      �� 5!   
   .��5"      �  5#      �� 5$      $	��50* �6$   �� 5%      $	��50* �6$   .+�5(      �� $��0��6   8��  ��6[   �� *(&InfrastructureSelector.SelectedRailway+$ 	Open: {0}���� 5)      $��1��6   8�� ��6[   �� */-InfrastructureSelector.SelectedRailwaySection+8  5*      5+      �� 5-   
   " ��6/    5.      �� 50       True6   	 51      `� 52      8
   .1�53      �� $, &InfrastructureSelector.SelectedRailway�6&   �� 54      $3 -InfrastructureSelector.SelectedRailwaySection�6&   55      .a�57      ��  � ��8  60   �� 58       � {� . 62   �� 59       � ��
 ���� 63   5:      �� 5<      	 False6   	 5=      `� 5>      8
  .1�5?      �� $, &InfrastructureSelector.SelectedRailway�6&   �� 5@      $3 -InfrastructureSelector.SelectedRailwaySection�6&   5A      .a�5C      ��  � ��$&  Select railway, railway segment.��60   �� 5D       � {� .  62   5E      5H      5J      .��5L      � $	��800��6   $	��600��6   $��0,5,0,0q�6*   $��Stretch=�6;    5M   
   �� 5N      $
��0.2* �6   �� 5O      $��50 �6   �� 5P      $��30 �6   �� 5Q      $��1* �6   �� 5R      $��1* �6   �� 5S      $��30 �6   �� 5T      $��1* �6   �� 5U      $��30 �6   �� 5V      $��1* �6   �� 5W      $��20 �6   �� 5X      $
��0.2* �6    � ColumnDefinitions 5Z   
   �� 5[      $��2* �6   �� 5\      $
��Auto �6   �� 5]      $��2* �6   �� 5^      $	��20* �6   �� 5_      $��2* �6   �� 5`      $	��30* �6   �� 5a      $��2* �6   .+�5d   
   �� $'��!SZDC railways train traffic graph��6   #��[ 6=   $��1��6m   $��3��6}   $��1��6�   �� 5e   
   -   5        $��TimeSelector��5e      $"��Select timetable information��6(   $��1��6L   $��5��6\   #��[ 6i   -   'SZDC.WPF.Dashboard.TimetableTypeControl  5g   
   $��1��6)   $��6��69   $��2��6F   �� 5i   
   -   5        $��
OpenButton��5k      $��1��5i      $��3��6!   #��[ 5j      ��5l      �� $  TimeSelector.TimetableType�# [ ��5m      ��  �� ElementName$ TimeSelector��$ ActualWidth��� 5o   
   $��3��6   $��2��6$   $��Railways��61   #��[ 6A   �� 5p   
   -   5         ��Name$ AvailableRailways��5p      $��3��6-   $��3��6=   $��6��6J   ��5q      �� $. (InfrastructureSelector.AvailableRailways� �� SelectedItem 5r      �� $, &InfrastructureSelector.SelectedRailway� �� Mode$ TwoWay=�5s      �� 5u   
   $��5��6   $��2��6$   $"��Segments of selected railway��61   #��[ 6U   �� 5v   
   -   5        $ AvailableRailwaySections��5v      $��5��64   $��3��6D   $��2��6Q   ��5w      �� $5 /InfrastructureSelector.AvailableRailwaySegments� 5x      �� $3 -InfrastructureSelector.SelectedRailwaySection�$ TwoWay=���5y      �� 5z       �� DataType;  5SZDC.Editor.Interfaces.RailwaySegmentBriefDescription"  6   .R�5{      �� ��6    �� *.+# [ 5|      5~      �� 5�   
   $��5��6   $��5��6$   $"��Stations in selected segment��61   #��[ 6U   �� 5�   
   -   5        $( "SelectedRailwaySectionStationsList��5�      $��6��6>   $��5��6K   $��3��6[   ��5�      �� $E ?InfrastructureSelector.SelectedRailwaySection.StationsInSegment���5�      �� 5�      " ��6   .�5�      ��  � |� .  62   5�      5�      5�      5�      