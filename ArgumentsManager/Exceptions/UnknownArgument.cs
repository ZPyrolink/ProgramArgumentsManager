using System;

namespace ArgumentsManager.Exceptions
{
	internal class UnknownArgument : Exception
	{
		public UnknownArgument(string message) : base(message) { }
	}
}
