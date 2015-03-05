using System;

namespace Eto
{
	/// <summary>
	/// Exception thrown when the handler cannot be created either because it was not found or could not be instantiated
	/// </summary>
	[Serializable]
	public class HandlerInvalidException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.HandlerInvalidException"/> class
		/// </summary>
		public HandlerInvalidException ()
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.HandlerInvalidException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public HandlerInvalidException (string message) : base (message)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Eto.HandlerInvalidException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public HandlerInvalidException (string message, Exception inner) : base (message, inner)
		{
		}
	}
}

