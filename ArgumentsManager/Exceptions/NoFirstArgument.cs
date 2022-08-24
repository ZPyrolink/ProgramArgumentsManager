using System;

namespace ArgumentsManager.Exceptions
{
	internal class NoFirstArgument : Exception
	{
		public NoFirstArgument(string message) : base(message) { }
	}
}
