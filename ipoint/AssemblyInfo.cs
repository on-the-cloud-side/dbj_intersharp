#region DBJ Copyright
//
//        DBJ*IP(tm) DBJ's Integration Point Interface
//
//        Copyright (c)  2005-2006 by DBJ*Solutions Ltd. All Rights Reserved
//
//        THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF DBJ*Solutions Ltd..
//
//        The copyright notice above does not evidence any
//        actual or intended publication of such source code.
//
//  $Author: dusan $
//  $Date: 30.06.06 14:20 $
//  $Revision: 1 $
//
#endregion

using System.Reflection;
using System.Runtime.CompilerServices;

#if BUILD_AS_COM
[assembly:System.EnterpriseServices.ApplicationName("DBJIP")]
[assembly:System.EnterpriseServices.ApplicationAccessControl(false)]
[assembly:System.EnterpriseServices.Description("DBJ*IP COM+ Application.")]
[assembly:System.Runtime.InteropServices.ComVisible(true)]
[assembly:System.EnterpriseServices.ApplicationActivation(System.EnterpriseServices.ActivationOption.Server ) ] 
#endif

[assembly:System.CLSCompliant(true)]

[assembly: AssemblyTitle("DBJ*IP(tm)")]
[assembly: AssemblyDescription("DBJ's Integration Point Interface")]
[assembly: AssemblyCompany("DBJ*Solutions Ltd.")]
#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#endif
#if DEBUG
[assembly: AssemblyProduct("DBJ*IP(tm) Debug build")]
#else
[assembly: AssemblyProduct("DBJ*IP(tm) Release build")]
#endif

[assembly: AssemblyCopyright("(c) 2005-2006 by DBJ*Solutions Ltd.")]
[assembly: AssemblyTrademark("DBJ*IP is an worldwide trademark.")]
[assembly: AssemblyCulture("")]		

[assembly: AssemblyVersionAttribute("5.0.0.*")]

[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("../../dbj.key")]
[assembly: AssemblyKeyName("")]
