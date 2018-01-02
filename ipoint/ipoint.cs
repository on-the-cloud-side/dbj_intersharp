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


namespace dbj.integration
{
	using System;
	/// <summary>
	/// Integration Point component Interface 
	/// </summary>
	/// <remarks>
	/// Integration Point's  are conceptually small discrete components used 
	/// to communicate FROM some application you are writting TO one
	/// or more external (legacy) systems.<br/>
	/// This interface is implemented by these integration components. 
	/// The existence of this interface allows for run-time integration
	/// components management. Usualy this components are implemented as 
	/// COM+ components. Running application can 'discover' them through PROGID mechansim
	/// and use them without recompilation because they all have the same IPoint interface.<br/>
	/// One other side effect, is the ability to easier re-use components
	/// implementing one simple and ubiquitous interface.<br/>
	/// This interface supports SYNCHRONOUS communication from your application
	/// to the external system. The coomunication patern is one of a simple 'call'
	/// and wait for the return value.
	/// </remarks> 
	/// 
#if BUILD_AS_COM
	// none of this is strictly necessary
	[System.Runtime.InteropServices.ComVisible(true)]
	[System.Runtime.InteropServices.InterfaceTypeAttribute(
		 System.Runtime.InteropServices.ComInterfaceType.InterfaceIsDual  
		 )]
	[System.Runtime.InteropServices.Guid("d1a56bfd-c8d5-47ce-b077-0352b1223bf1")]
#endif
	public interface IPoint
	{
		/// <summary>
		/// method for 'calling' the external system.
		/// </summary>
		/// <param name="request_for_external_system">optional 'payload' to be sent to the external system.</param>
		/// <param name="response_from_external_system">message or data sent from 'outside', after the call has finished.</param>
		/// <returns>null or some other signaling value, depending on the implementation.</returns>
		/// <remarks>
		/// NOTE TO THE IMPLEMENTORS: Do not throw an exception from this method.
		/// Instead implement some signaling protocol, whose values will be returned upon error or warning or simillar.
		/// In general follow the COM rules when implementing this method
		/// </remarks>
#if BUILD_AS_COM
		// not strictly necessary
		[System.Runtime.InteropServices.ComVisible(true)]
		[System.Runtime.InteropServices.PreserveSig ]
#endif
		int call ( 
#if BUILD_AS_COM
			// not strictly necessary
			[System.Runtime.InteropServices.In]
#endif
			string request_for_external_system, 
#if BUILD_AS_COM
			// not strictly necessary
			[System.Runtime.InteropServices.Out]
#endif
			out string response_from_external_system 
		) ;
	}

} // eof namespace
