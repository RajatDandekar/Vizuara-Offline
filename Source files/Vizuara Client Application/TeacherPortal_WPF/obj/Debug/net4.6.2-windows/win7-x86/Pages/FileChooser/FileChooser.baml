   M S B A M L   `   `   `  ????H  DTeacherPortal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Vizuara.TeacherPortal.Pages.FileChooser'Vizuara.TeacherPortal.Pages.FileChooser  5      9    3Vizuara.TeacherPortal.Pages.FileChooser.FileChooser   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Glocal5clr-namespace:Vizuara.TeacherPortal.Pages.FileChooser     5        N? Title$  FileChooser??5	      .??5      ? .+?5   
   ? $??0,30,0,0q?6   .+?5      ?? $
??Auto=?6   .??5      ?? $??Center=?6O   $??	50,0,50,0q?6l   ??6   ??  ?? Path$ AvailableOptions???5      ?? 5      .R?5      ? $	??200??5   1   $	??100??5   2   $	??200??5   1   .+?5   "   ?? $??5,5,5,5q?6)    J? Opacity$	 0.3Q?6:    ?? CornerRadius$ 5??5   ;   $??2q?5!   >   
???   ?
???   ?5(   #   ?? 5)   "   -   5        $??3,3,3,3q?5)   H   $ 0Q?6n   ??6)   ?? *
optionName+5*   #   ?? 5+   "   $??Center=?6I   $??Center=?6f   
??????? J? IsHitTestVisible$ false??6?   $
??Wrap=?6?   $??5,5,5,5q?6?   ??6,   ?? *displayName+6?   5,      5-      ??5/      ?? 50      .R?51      ? 52      54      55      56      ? 58   
    ? ColumnDefinitions 59      ?? 5:      $
??0.1* ?5=   .   5>      ?? 5?      $
??0.8* ?5B   .   5C      ?? 5D      $
??0.1* ?5G   .   5H       ? RowDefinitions 5J      ?? 5K      $
??0.1* ?5N   ,   5O      ?? 5P      $
??0.9* ?5S   ,   5T      .+?5W      ? .+?5X      ?? -   5         ??Name$ BackText??5X   T   $??Center=?6   $??Center=?69   $y?Hidden=?6f   
???????
← Back5Z      ?? 5[      -   5         ??Name$ 
BackButton??5[   %   $ 0Q?6   $y?Hidden=?69   .??6\   Back5]      5^      ? 5_      $??2??5b      .+?5c      ?? -   5        $ 
DeleteText??5c   T   $??Center=?6   $??Center=?69   $y?Visible=?6h   
???????🗑 Delete All5e      ?? 5f      -   5        $ DeleteButton??5f   %   $ 0Q?6   $y?Visible=?6;   .??6b   Delete5h      5i      5j      5k      5l      