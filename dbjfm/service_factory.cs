#region Copyright © 2003-2005 DBJ*Solutions Ltd. All Rights Reserved
// This file and its contents are protected by United States and 
// International copyright laws. Unauthorized reproduction and/or 
// distribution of all or any portion of the code contained herein 
// is strictly prohibited and will result in severe civil and criminal 
// penalties. Any violations of this copyright will be prosecuted 
// to the fullest extent possible under law. 
// 
// UNDER NO CIRCUMSTANCES MAY ANY PORTION OF THE SOURCE
// CODE BE DISTRIBUTED, DISCLOSED OR OTHERWISE MADE AVAILABLE TO ANY 
// THIRD PARTY WITHOUT THE EXPRESS WRITTEN CONSENT.
// 
// THE END USER LICENSE AGREEMENT (EULA) ACCOMPANYING THE PRODUCT 
// PERMITS THE REGISTERED DEVELOPER TO REDISTRIBUTE THE PRODUCT IN 
// EXECUTABLE FORM ONLY IN SUPPORT OF APPLICATIONS WRITTEN USING 
// THE PRODUCT. IT DOES NOT PROVIDE ANY RIGHTS REGARDING THE 
// SOURCE CODE CONTAINED HEREIN. 
// 
// THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE. 
#endregion

using System;

namespace dbj.fm
{
	/// <summary>
	/// The enumumeration that uniquely identifies each service
	/// </summary>
	public enum ServiceID { 
		/// <summary>Id of the Configuration service</summary>
		Configuration, 
		/// <summary>Id of the Tracing service</summary>
		Tracing, 
		/// <summary>Id of the Logging service</summary>
		Logging, 
#if INCLUDE_DEPRECATED
		/// <summary>Id of the ErrorHandling service</summary>
		ErrorHandling, 
#endif
		/// <summary>Id of the XMLTransformations service</summary>
		XMLTransformations, 
		/// <summary>Id of the Data service</summary>
		Data ,
		/// <summary>Id of the Reflection service</summary>
		Reflection
	} ;
	/// <summary>
	/// Summary description for service_factory.
	/// </summary>
	internal class service_factory
	{
		static object locker = new object() ;

		private service_factory(){		}

		/// <summary>
		/// make and return actual service implementation.
		/// here we may use arguments list passed. this involves casting them to actual reauired types.
		/// </summary>
		/// <param name="args" >optional list of arguments contains calling assembly assembly ,connection string etc</param>
		/// <param name="which" >name of service</param>
		static public FactoryImplicitor get_service ( ServiceID which, params object[] args )
		{
			lock ( service_factory.locker )
			{
				switch ( which )
				{
					case ServiceID.Configuration : 
						if ( args.Length < 1 ) throw new Exception("Pleaseprovide assembly for which to make the configuration service.") ;
						return new FactoryImplicitor( new config.configurator((System.Reflection.Assembly)args[0]) ) ;
					case ServiceID.Tracing:
						if ( args.Length < 1 ) throw new Exception("Please provide assembly for which to make the tracer service.") ;
						return new FactoryImplicitor( new tracing.tracer((System.Reflection.Assembly)args[0]) ) ;
					case ServiceID.Logging :
						if ( args.Length < 1 ) 
							return new FactoryImplicitor( new fm.evlog.service( fm.core.DBJ_EVLOG_LOGNAME ) ) ;
						else
							return new FactoryImplicitor( new fm.evlog.service((string)args[0]) ) ;
#if INCLUDE_DEPRECATED
					case ServiceID.ErrorHandling :
						return new FactoryImplicitor( new fm.error_handler() ) ;
#endif
					case ServiceID.XMLTransformations :
						return new FactoryImplicitor( new fm.transformer() ) ;
					case ServiceID.Reflection :
						return new FactoryImplicitor( new fm.reflection.service() ) ;
					case ServiceID.Data :
					{
						if ( args.Length < 2 ) throw new Exception("To make data service corelib needs arguments : first the store id , second the connection string") ;
						fm.Idata.Store store = (fm.Idata.Store)args[0];
						string connection_string = (string)args[1] ;
						switch ( store )
						{
							case Idata.Store.ADO :
								return new FactoryImplicitor(new data.AdoNet( connection_string ) ) ;
							case Idata.Store.SQLSRV :
								return new FactoryImplicitor(new data.server2000( connection_string ) ) ;
							case Idata.Store.ORACLE :
								throw new Error.NotImplemented() ;
							default :
								throw new Error("Unknonw store ID for data service making") ;
						}
					}
					default :
						throw new Error("Illegal service id.") ;
				}
			}
		}
	}


		/// <summary>
		/// Allow implicit conversion of the service factory results. 
		/// Used as a type of the result of service_factory.get_service()
		/// makes casting of the result 'invisible' and not required by callers.
		/// </summary>
		public sealed class FactoryImplicitor
		{
			object val_  ;

			private FactoryImplicitor () { /* forbidden */ }
			/// <summary>
			/// Constructor of FactoryImplicitor class
			/// </summary>
			/// <param name="val"> value </param>
			public FactoryImplicitor( object val ) 
			{
				if ( val != null )
				{
					if ( val.GetType().Equals(this)) 
						throw new Exception("Having nested Implicitors is probably NOT a good idea.") ;
					val_ = val ; 
				}
				else
					val_ = null ;
			}

			/// <summary>
			/// Make Error instance
			/// </summary>
			/// <param name="msg">error message</param>
			/// <returns>The DBJ*FM++ Error</returns>
			Error mex ( string msg ) 
			{
				Error  x = new Error ("FactoryImplicitor for value:" + this.val_.ToString() + ",has failed, error is: " + msg ) ;
				x.Source = "FactoryImplicitor" ;
				return x ;
			}
			
			/// <summary>
			/// allow implict conversion to Itypeinfo
			/// </summary>
			/// <param name="ep" >FactoryImplicitor conting val to be type casted to Itypeinfo</param>
			public static implicit operator Itypeinfo (FactoryImplicitor ep) 
			{
				if ( ep.val_ == null ) return null ;
				try 
				{
					return (Itypeinfo)ep.val_ ;
				} 	
				catch ( Exception ){throw ep.mex("Failed to convert to Itypeinfo " ) ;}
			}

			/// <summary>
			/// allow implict conversion to Idata
			/// </summary>
			/// <param name="ep" >FactoryImplicitor conting val to be type casted to Idata</param>
			public static implicit operator Idata (FactoryImplicitor ep) 
			{
				if ( ep.val_ == null ) return null ;
				try 
				{
					return (Idata)ep.val_ ;
				} 	
				catch ( Exception ){throw ep.mex("Failed to convert to Idata " ) ;}
			}

			/// <summary>
			/// allow implict conversion to Iconfiguration
			/// </summary>
			/// <param name="ep" >FactoryImplicitor conting val to be type casted to Iconfiguration</param>
			public static implicit operator Iconfiguration (FactoryImplicitor ep) 
			{
				if ( ep.val_ == null ) return null ;
				try 
				{
					return (Iconfiguration)ep.val_ ;
				} 	
				catch ( Exception ){throw ep.mex("Failed to convert to Iconfiguration " ) ;}
			}

#if INCLUDE_DEPRECATED
			/// <summary>
			/// allow implict conversion to Ierror_handler
			/// </summary>
			/// <param name="ep" >FactoryImplicitor conting val to be type casted to fm.Ierror_handler</param>
			public static implicit operator fm.Ierror_handler (FactoryImplicitor ep) 
			{
				if ( ep.val_ == null ) return null ;
				try 
				{
					return (Ierror_handler)ep.val_ ;
				} 	
				catch ( Exception ){throw ep.mex("Failed to convert to Ierror_handler " ) ;}
			}
#endif
			/// <summary>
			/// allow implict conversion to Ievent_logger
			/// </summary>
			/// <param name="ep" >FactoryImplicitor conting val to be type casted to fm.Ievent_logger</param>
			public static implicit operator fm.Ievent_logger (FactoryImplicitor ep) 
			{
				if ( ep.val_ == null ) return null ;
				try 
				{
					return (Ievent_logger)ep.val_ ;
				} 	
				catch ( Exception ){throw ep.mex("Failed to convert to Ievent_logger " ) ;}
			}

			/// <summary>
			/// allow implict conversion to Itracer
			/// </summary>
			/// <param name="ep" >FactoryImplicitor conting val to be type casted to fm.Itracer</param>
			public static implicit operator fm.Itracer (FactoryImplicitor ep) 
			{
				if ( ep.val_ == null ) return null ;
				try 
				{
					return (Itracer)ep.val_ ;
				} 	
				catch ( Exception ){throw ep.mex("Failed to convert to Itracer " ) ;}
			}

			/// <summary>
			/// allow implict conversion to Itransformer
			/// </summary>
			/// <param name="ep">FactoryImplicitor conting val to be type casted to fm.Itransformer</param> 
			public static implicit operator fm.Itransformer (FactoryImplicitor ep) 
			{
				if ( ep.val_ == null ) return null ;
				try 
				{
					return (Itransformer)ep.val_ ;
				} 	
				catch ( Exception ){throw ep.mex("Failed to convert to Itransformer " ) ;}
			}


			//----------------------------------------------------------------------
			/// <summary>
			/// For VB.NET and other languages, implict operators are far from elegant
			/// this is why we have to resort to this non-oo goo bellow ;)
			/// </summary>

			public  Idata as_data_svc () 
			{
				if ( val_ == null ) return null ;
				try 
				{
					return (Idata)val_ ;
				} 	
				catch ( Exception ){throw mex("Failed to convert to Idata " ) ;}
			}

			/// <summary>
			/// allow implict conversion to Iconfiguration
			/// </summary>
			public  Iconfiguration as_config () 
			{
				if ( val_ == null ) return null ;
				try 
				{
					return (Iconfiguration)val_ ;
				} 	
				catch ( Exception ){throw mex("Failed to convert to Iconfiguration " ) ;}
			}

			/*
			/// <summary>
			/// allow implict conversion to Ierror_handler
			/// </summary>
			public  fm.Ierror_handler as_errhandler () 
			{
				if ( val_ == null ) return null ;
				try 
				{
					return (Ierror_handler)val_ ;
				} 	
				catch ( Exception ){throw mex("Failed to convert to Ierror_handler " ) ;}
			}
			*/
			/// <summary>
			/// allow implict conversion to Ievent_logger
			/// </summary>
			public  fm.Ievent_logger as_evlogger() 
			{
				if ( val_ == null ) return null ;
				try 
				{
					return (Ievent_logger)val_ ;
				} 	
				catch ( Exception ){throw mex("Failed to convert to Ievent_logger " ) ;}
			}

			/// <summary>
			/// allow implict conversion to Itracer
			/// </summary>
			public  fm.Itracer as_tracer() 
			{
				if ( val_ == null ) return null ;
				try 
				{
					return (Itracer)val_ ;
				} 	
				catch ( Exception ){throw mex("Failed to convert to Itracer " ) ;}
			}

			/// <summary>
			/// allow implict conversion to Itransformer
			/// </summary>
			public  fm.Itransformer as_transformer() 
			{
				if ( val_ == null ) return null ;
				try 
				{
					return (Itransformer)val_ ;
				} 	
				catch ( Exception ){throw mex("Failed to convert to Itransformer " ) ;}
			}

			// ---------------------------------------------------------------------
			/// <summary>
			/// returns string form of the val_ defined in this class
			/// </summary>
			/// <returns>string</returns>
			public override string ToString() { return val_.ToString(); }
			// ---------------------------------------------------------------------
			// quit the compiler warnings with these two overloads
			/// <summary>
			/// 
			/// </summary>
			/// <param name="o"></param>
			/// <returns>bool</returns>
			public override bool Equals(object o) {   return base.Equals(o); }
			/// <summary>
			/// gets hashcode of the base
			/// </summary>
			/// <returns>int</returns>
			public override int GetHashCode() {	return base.GetHashCode(); 	}
		}
	}

