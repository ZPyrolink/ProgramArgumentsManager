using System;
using System.Runtime.Serialization;

namespace ProgramArgumentsManager.Exceptions
{
	[Serializable]
	public class UnknownArgumentException : Exception
	{
		public string Argument;

		public UnknownArgumentException(string argument, string message) : base(message) { Argument = argument; }
		protected UnknownArgumentException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}