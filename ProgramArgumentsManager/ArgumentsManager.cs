using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgramArgumentsManager
{
    public class ArgumentsManager
    {
        public string ApplicationName { get; }
        public string Version { get; }

        private Dictionary<Argument, string> _options;

        public ArgumentsManager(string name) : this(name, "1.0.0") { }

        public ArgumentsManager(string name, string version)
        {
            ApplicationName = name;
            Version = version;

            _options = new Dictionary<Argument, string>();
        }

        public void AddArguments(string format, string description)
        {
            format = format.Replace(" ", string.Empty);

            if (!format.Contains("-") && !format.Contains("--"))
                throw new ArgumentException("Le format '" + format + " n'est pas comaptible en tant que paramètre !",
                    nameof(format));

            char[] delimiters = {',', ';'};
            string[] formats = format.Split(delimiters);

            if (formats.Any(f => !f.StartsWith("-") && !f.StartsWith("--")))
                throw new ArgumentException("Le format '" + format + " n'est pas comaptible en tant que paramètre !",
                    nameof(format));

            _options.Add(new Argument(formats), description);
        }

        private class Argument
        {
            private readonly string[] _names;

            public Argument(string[] names)
            {
                _names = names;
            }

            private bool Equals(Argument other) => other._names.Any(s => _names.Contains(s));

            public override bool Equals(object obj)
            {
                if (obj is null)
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;

                return obj.GetType() == GetType() && Equals((Argument) obj);
            }

            public override int GetHashCode()
            {
                // Same HashCode for each instance to force the use of the Equals method (for the Dictionnary key)
                return 0;
            }

            public static bool operator ==(Argument left, Argument right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Argument left, Argument right)
            {
                return !Equals(left, right);
            }
        }
    }
}