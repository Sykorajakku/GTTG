   M S B A M L   `   `   `  ����C  ?SZDC.WPF, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:SZDC.WpfSZDC.Wpf  5   	   9!clr-namespace:SZDC.Wpf.TrainGraphSZDC.Wpf.TrainGraph  5   	   7 clr-namespace:SZDC.Wpf.ModifiersSZDC.Wpf.Modifiers  5   	   5clr-namespace:SZDC.Wpf.DesignerSZDC.Wpf.Designer  5	   	   9!clr-namespace:SZDC.Wpf.ConvertersSZDC.Wpf.Converters  5
   	   9!clr-namespace:SZDC.WPF.TrainGraphSZDC.WPF.TrainGraph  5   	   1    +SZDC.Wpf.TrainGraph.DynamicTrainGraphWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &wpfclr-namespace:SZDC.Wpf     5   	   8
trainGraph!clr-namespace:SZDC.Wpf.TrainGraph     5   	   6	modifiers clr-namespace:SZDC.Wpf.Modifiers     5   	   4designerclr-namespace:SZDC.Wpf.Designer     5	   	   8
converters!clr-namespace:SZDC.Wpf.Converters     5
   	   9trainGraph1!clr-namespace:SZDC.WPF.TrainGraph     5   	   $
��1344��5   	   $	��864��6     � Title  5      `� 5   
    �� StringFormat$  {0} railway {1}, {2} - {3}��6   .1�5      ��  �� Path$! TimetableInfo.TimetableType�6   �� 5      $! TimetableInfo.RailwayNumber�6   �� 5      $$ TimetableInfo.FirstStationName�6   �� 5      $# TimetableInfo.LastStationName�6   5       � 	Resources 5      3   -SZDC.Wpf.Converters.HasSelectedTrainConverter   HasSelectedTrainConverter  
IsModified �� 
TargetType �� Triggers �� Binding �� Value �� Property	 �� Value��%k  &	        &	        5   
   6/   �� 5   
   " ��6#    5      �� 5       6+   �� $- 'Tools.TrainModificationTool.IsModifying� True6   .a�5      ��  � ��$
	 Stop��60   5       �� 5"       6,   �� $- 'Tools.TrainModificationTool.IsModifying�	 False6   .a�5#      ��  � ��$	 Start��60   5$      5&      5(      .��5*      � 
 � RowDefinitions
 5+   
   �� 5,      $��35 �6   �� 5-      $��35 �6   �� 5.      $��* �6   �� 5/      $��10 �6    � ColumnDefinitions 51   
   �� 52      $��70 �6   �� 53      $��* �6   .+�56   
   �� $��0��6   $��Modify train:��6!   $��Center=�66   
�������6Q   &    SZDC.Wpf.Modifiers.TrainModifier  57   
   $��1��6"   $��0��6/   #��[ 59   "   {�58   "   �� $0 *Tools.TrainSelectionTool.SelectedTrainView� �� 	Converter# [  59   "   *   $SZDC.Wpf.TrainGraph.TrainGraphHeader  5:   
   $��0��6&   $��2��63   $��1��6D   +   %SZDC.WPF.TrainGraph.TrainGraphControl  5;   
   $��2��6(   $��0��65   $��2��6E   5=      5>      