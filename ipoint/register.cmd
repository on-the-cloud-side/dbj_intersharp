
@rem
@rem Use this CMD to REGISTER DBJIPOINT.DLL as COM+ component
@rem
@rem Just drag assembly dll and drop it on this cmd
@rem
@rem For this CMD to work .NET 1.1 must be on the installed
@rem
@rem 23AUG04 DBJ
@rem
@rem $Revision: 3 $
@rem $JustDate: 26/08/04 $
@rem
@rem
@SET FrameworkDir=%WINDIR%\Microsoft.NET\Framework
@REM @SET FrameworkVersion=v1.1.4322
@SET FrameworkVersion=v2.0.50727
@set REGISTRATOR=%FrameworkDir%\%FrameworkVersion%\regsvcs.exe

%REGISTRATOR% %1

@pause

@SET FrameworkDir=
@SET FrameworkVersion=
@set REGISTRATOR=
