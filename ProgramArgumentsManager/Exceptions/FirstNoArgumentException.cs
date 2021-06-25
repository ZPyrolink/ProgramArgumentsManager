using System;
using System.Runtime.Serialization;

namespace ProgramArgumentsManager.Exceptions
{
	[Serializable]
	public class FirstNoArgumentException : Exception
	{
		public FirstNoArgumentException() { }
		public FirstNoArgumentException(string message) : base(message) { }
		public FirstNoArgumentException(string message, Exception inner) : base(message, inner) { }
		protected FirstNoArgumentException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}