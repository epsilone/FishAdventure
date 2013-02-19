@echo off
thrift-0.9.0.exe -v --gen csharp lego.thrift
thrift-0.9.0.exe -v --gen py:new_style lego.thrift
pause