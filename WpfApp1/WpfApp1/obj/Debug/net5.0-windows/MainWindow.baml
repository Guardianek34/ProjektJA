   M S B A M L   `   `   `  ����B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  5   	       WpfApp1.MainWindow   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:WpfApp1     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $	��400 �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $	��200 �6   �� 5      $��* �6   .+�5   
   ]� $��0��6   $��0��6$   $	��Top=�61   $��10q�6I   .+�5      �� $��Core Amount��6    �� TextAlignment$
 Left=�6+   $	��Top=�6@   �� 5       �� TextAlignment$ Right=�5      $��40��6,   $��Right=�67   ��5      ��  �� ElementName$ slValue�� �� Path$ Value� �� UpdateSourceTrigger$ PropertyChanged=�5   7   �� 5      -   5        $��slValue��5   0    � Minimum$ 1Q�5      	 � Maximum$	 64Q�6"   
 �� TickPlacement$
 BottomRight=�6/    �� TickFrequency$ 1Q�6K    �� IsSnapToTickEnabled$
 True��5      $	��Top=�6?   5      �� 5   
   $��0��6   $��0��6"   $��5q�62   $��Center=�6=   .+�5      �� Choose a library!6+   �� 5      -   5         ��Name$ chosenLanguage��5      .@�5      �� $��	Assembler��6   �� 5      $��C#��6   5       5!      �� 5#   
   -   5        $��
runProgram��5#   /   $��0��6   $��1��6"   $��Run the program!��5$      $��16(�6,   6:   �� 5&   
   -   5        $��getFile��5&   .   $��1��6   $��0��6!   $��Click to add the file!��5'      $��16(�62   5(      ]� 5*   
   $��1��6   $��1��6$   $��Center=�61   $��Center=�6L   $��10q�6i   .+�5+      �� $$��Time it has taken to complete:��6   $
 Left=�6>   $	��Top=�6S   $��16(�6h   �� 5,      $��Center=�61    �� 
IsReadOnly$
 True��5-      $��16(�6(   ��5,      �� $
 Time�5-   (   5.      50      52      