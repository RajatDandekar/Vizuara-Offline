   M S B A M L   `   `   `  ???? H  DTeacherPortal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Vizuara.TeacherPortal.Pages.FileChooser'Vizuara.TeacherPortal.Pages.FileChooser  9    3Vizuara.TeacherPortal.Pages.FileChooser.FileChooser   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Glocal5clr-namespace:Vizuara.TeacherPortal.Pages.FileChooser       N? Title$  FileChooser??.??? .+?? $??0,30,0,0q?.+??? $
??Auto=?.???? $??Center=?$??	50,0,50,0q?????  ?? Path$ AvailableOptions????? .R?? $	??200??$	??100??$	??200??.+??? $??5,5,5,5q? J? Opacity$	 0.3Q? ?? CornerRadius$ 5??$??2q?
???   ?
???   ??? -   $??3,3,3,3q?$ 0Q????? *
optionName+?? $??Center=?$??Center=?
??????? J? IsHitTestVisible$ false??$
??Wrap=?$??5,5,5,5q????? *displayName+???? .R?? ?  ? ColumnDefinitions ?? $
??0.1* ??? $
??0.8* ??? $
??0.1* ? ? RowDefinitions ?? $
??0.1* ??? $
??0.9* ?.+?? .+??? -    ??Name$ BackText??$??Center=?$??Center=?$y?Hidden=?
???????
← Back?? -    ??Name$ 
BackButton??$ 0Q?$y?Hidden=?.??Back? $??2??.+??? -   $ 
DeleteText??$??Center=?$??Center=?$y?Visible=?
???????🗑 Delete All?? -   $ DeleteButton??$ 0Q?$y?Visible=?.??Delete