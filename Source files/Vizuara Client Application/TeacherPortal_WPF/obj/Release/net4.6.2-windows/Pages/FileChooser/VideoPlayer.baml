   M S B A M L   `   `   `  ???? H  DTeacherPortal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Vizuara.TeacherPortal.Pages.FileChooser'Vizuara.TeacherPortal.Pages.FileChooser  U QVlc.DotNet.Wpf, Version=3.1.0.0, Culture=neutral, PublicKeyToken=84529da31f4eb963G4clr-namespace:Vlc.DotNet.Wpf;assembly=Vlc.DotNet.WpfVlc.DotNet.Wpf 9    3Vizuara.TeacherPortal.Pages.FileChooser.VideoPlayer   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Glocal5clr-namespace:Vizuara.TeacherPortal.Pages.FileChooser     Dwpf4clr-namespace:Vlc.DotNet.Wpf;assembly=Vlc.DotNet.Wpf       N? Title$  VideoPlayer??.??? .+?? -    ?Name$ VLCParentControl?? ? RowDefinitions ?? $
??0.9* ??? $
??0.1* ?? .+?? .+??  ?? $
??0.9* ??? $
??0.1* ?.+?? -   $ BackgroundGrid??$??1??????	Black?? J? Opacity$
 0.75Q?? -   $ ControlGrid??$??1?? ?? $
??0.5* ??? $
??0.5* ?.+?? -   $ 
SliderGrid??.+??? -    ??Name$ SliderControl?? ? Maximum$	 100Q? ?? TickPlacement$ BottomRight=? ?? TickFrequency$ 1Q?$??	50,0,50,0q?? -   $ MainControlGrid??$??1?? ? ColumnDefinitions ?? $
??0.1* ??? $
??0.1* ??? $
??0.1* ??? $
??0.1* ??? $??0.075* ??? $??0.075* ??? $??0.075* ??? $
??0.1* ??? $
??0.1* ??? $
??0.1* ??? $
??0.1* ?.+??? -   	 ??Name$	 PlayActionImage??$??5??$!??/Sprites/UI/PauseButton.png???? -   
 ??Name$
 PlayActionButton??$
 0.01Q?$??5??