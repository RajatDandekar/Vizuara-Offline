   M S B A M L   `   `   `  ????H  DTeacherPortal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Vizuara.TeacherPortal.Pages.FileChooser'Vizuara.TeacherPortal.Pages.FileChooser  5      U QVlc.DotNet.Wpf, Version=3.1.0.0, Culture=neutral, PublicKeyToken=84529da31f4eb963G4clr-namespace:Vlc.DotNet.Wpf;assembly=Vlc.DotNet.WpfVlc.DotNet.Wpf 6K   9    3Vizuara.TeacherPortal.Pages.FileChooser.VideoPlayer   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Glocal5clr-namespace:Vizuara.TeacherPortal.Pages.FileChooser     5      Dwpf4clr-namespace:Vlc.DotNet.Wpf;assembly=Vlc.DotNet.Wpf     6K   -   5          N? Title$  VideoPlayer??5	      .??5      ? .+?5   
   ? -   5         ?Name$ VLCParentControl??5       ? RowDefinitions 5      ?? 5      $
??0.9* ?5   ,   5      ?? 5      $
??0.1* ?5   ,   5      5      ? 5   
   .+?5      ? .+?5      ?  5      ?? 5       $
??0.9* ?5#   4   5$      ?? 5%      $
??0.1* ?5(   4   5)      .+?5+      ? -   5        $ BackgroundGrid??5+      $??1??5.   $   ??5/      ??50      	Black??52       J? Opacity$
 0.75Q?56   (   57      ? 58      -   5        $ ControlGrid??58      $??1??5;   $    5<      ?? 5=      $
??0.5* ?5@   8   5A      ?? 5B      $
??0.5* ?5E   8   5F      .+?5H      ? -   5        $ 
SliderGrid??5H       ? ColumnDefinitions 5J      ?? 5K   "   $
??0.1* ?5N   >   5O   #   ?? 5P   "   $
??0.8* ?5S   >   5T   #   ?? 5U   "   $
??0.1* ?5X   >   5Y   #   .+?5[      (? $??5,5,5,5q?6&   $??0??5^   /   .??5_   "   ?? -   5         ??Name$ CurrentTime??5_   ?   
???????5a   #   5b      (? 5c      $??5,5,5,5q?6&   $??2??5f   /   .??5g   "   ?? -   5        $ 	TotalTime??5g   ?   
???????5i   #   5j      ? 5k      $??1??5n   /   .+?5o   "   ?? -   5         ??Name$ SliderControl??5o   f    ? Maximum$	 100Q?6)    ?? TickPlacement$ BottomRight=?67   	 ?? TickFrequency$	 1Q?6S   6?   5p      5q      ? 5s      -	   5        $ MainControlGrid??5s      $??1??5w   (    5x      ?? 5y   "   $
??0.1* ?5|   >   5}   #   ?? 5~   "   $
??0.1* ?5?   >   5?   #   ?? 5?   "   $
??0.1* ?5?   >   5?   #   ?? 5?   "   $
??0.1* ?5?   >   5?   #   ?? 5?   "   $??0.075* ?5?   >   5?   #   ?? 5?   "   $??0.075* ?5?   >   5?   #   ?? 5?   "   $??0.075* ?5?   >   5?   #   ?? 5?   "   $
??0.1* ?5?   >   5?   #   ?? 5?   "   $
??0.1* ?5?   >   5?   #   ?? 5?   "   $
??0.1* ?5?   >   5?   #   ?? 5?   "   $
??0.1* ?5?   >   5?   #   .+?5?      ?? -
   5        
 ??Name$
 PlayActionImage??5?   $   $??5??5?   /   $!??/Sprites/UI/PauseButton.png??5?   0   5?      ?? 5?      -   5         ??Name$ PlayActionButton??5?   %   $
 0.01Q?6^   $??5??5?   /   5?      5?      5?      5?      5?      5?      5?      5?      