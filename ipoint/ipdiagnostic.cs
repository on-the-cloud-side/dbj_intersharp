#region DBJ Copyright
//
//        DBJ*IPSAMPLE(tm) DBJ's Integration Point Interface Example implementation
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

#define COMPLUS_FEATURES

//
namespace dbj.integration
{
	using System;
	/// <summary>
	/// Sample implementation of the IPoint interface.
	/// Purpose of this is to provide proc-info from an COM+ object
	/// </summary>
#if BUILD_AS_COM
	[System.Runtime.InteropServices.ProgId("DBJIP.PROCINFO")]
#if COMPLUS_FEATURES
//	[System.EnterpriseServices.Synchronization(System.EnterpriseServices.SynchronizationOption.Supported)]
	[System.EnterpriseServices.JustInTimeActivation(true)]
	[System.EnterpriseServices.ObjectPooling(true,10,100)]
#endif
#endif
	public class ProcInfoIPoint
		: System.EnterpriseServices.ServicedComponent, IPoint
	{
		/// <summary> COM requires default constructor </summary>
		public ProcInfoIPoint() {	}
	
		/// <summary>
		/// IPoint method to be implemented here
		/// </summary>
		/// <param name="request_for_the_external_system">ignored</param>
		/// <param name="response_from_the_external_system">returns proc info in an XML</param>
		/// <returns></returns>
		public int call(string request_for_the_external_system, out string response_from_the_external_system)
		{
			response_from_the_external_system = "<root><![CDATA[ " + procinfo() + " ]]></root>" ;
			return 0;
		}

		//
		static string procinfo () 
		{
			System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess() ;
			System.Text.StringBuilder result = new System.Text.StringBuilder() ;
			result.AppendFormat("\n\nPrivate Bytes = {0} MB " , proc.PrivateMemorySize64 / (1024 * 1024 ) ) ;
			result.AppendFormat("\n\rProcess {0}", proc.ToString() ) ;
			result.AppendFormat("\n\rid = {0}\n\rMachine = {1}\n\rProcName = {2}", proc.Id, proc.MachineName , proc.ProcessName ) ; 
			return result.ToString() ;
		}
	
		/// <summary>
		///  Can be pooled always
		/// </summary>
		protected override bool CanBePooled()
		{
			return true ;
			// return base.CanBePooled ();
		}
	}
}
