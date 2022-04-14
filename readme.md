Usage: Drag another .net assembly onto SystemLoggingTrick.exe and watch it fill up the logfile.

Description:

This project allows you to capture literally ALL activity happening in the System.Net class

You can capture activity of your own assembly or even let it load an external assembly. (Assembly.Load)

The logfile that results from this will contain EVERYTHING you could ever need.


This project can be adapted to any kind of namespace and theoretically allows to log every single method that uses Tracing. 
(you can check if a method uses logging by opening mscorlib.dll in dnSpy)
