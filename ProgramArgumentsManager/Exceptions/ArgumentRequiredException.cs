using ProgramArgumentsManager;

using System;
using System.Runtime.Serialization;

namespace ProgramArgumentsManager.Exceptions
{
    [Serializable]
    public class ArgumentRequiredException : Exception
    {
        internal Argument Argument;

        public ArgumentRequiredException() { }

        internal ArgumentRequiredException(Argument argument) : this($"The argument {argument} is required !")
        {
            Argument = argument;
        }
        public ArgumentRequiredException(string message) : base(message) { }
        public ArgumentRequiredException(string message, Exception inner) : base(message, inner) { }
        protected ArgumentRequiredException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}