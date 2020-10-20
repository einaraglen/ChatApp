files included in project:

App.xaml and App.xaml.cs	from .NET Core package
Assemblyinfo.cs				    from .NET core package

MainWindow.xaml				    GUI structure
MainWindow.xaml.cs			  GUI controller

Client.cs					        Handles connection to server, (sending and receiving)
Listener.cs					      Substitute for ChatListener inferface from template
TextMessage					      Copy of Template code 
User.cs						        Gives each User a random color on login
UserList.cs					      Manages when users connect and disconnect so we dont lose custom colors
