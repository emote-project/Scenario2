set "p=%cd%"
%comspec% /k ""C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" & cd %p% & xsd bml.xsd /classes /n:GBML & copy bml.cs ..\..\..\GBML\ & PAUSE" x86
exit
