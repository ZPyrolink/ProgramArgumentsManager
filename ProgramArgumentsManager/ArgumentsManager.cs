using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgramArgumentsManager
{
    public class ArgumentsManager
    {
        public string ApplicationName { get; }
        public string Version { get; }

        private Dictionary<Argument, Argument.ArgValues> _options;

        public ArgumentsManager(string name) : this(name, "1.0.0") { }

        public ArgumentsManager(string name, string version)
        {
            ApplicationName = name;
            Version = version;

            _options = new Dictionary<Argument, Argument.ArgValues>();
        }

        private bool IsArgument(string s) => s.StartsWith("-") || s.StartsWith("--");

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

            _options.Add(new Argument(formats), new Argument.ArgValues(description));
        }

        public void Parse(string[] args)
        {
            string lastArg = null;
            foreach (string arg in args)
            {
                if (IsArgument(arg))
                {
                    lastArg = arg;
                    _options[lastArg].Specified = true;
                }
                else
                {
                    _options[lastArg].Values.Add(arg);
                }
            }

            foreach (Argument.ArgValues argValues in _options.Values.Where(argValues => !argValues.Specified || !argValues.Values.Any()))
                argValues.Values = null;
        }

        public bool IsSpecified(string arg) => _options[arg].Specified;

        public List<string> GetValues(string arg) => _options[arg].Values;

        public string GetValue(string arg) => _options[arg].Values is null ? null : string.Join(" ", _options[arg].Values);

        private class Argument
        {
            private readonly string[] _names;

            private Argument(string name)
            {
                _names = new []{name};
            }
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

            public static implicit operator Argument(string s) => new Argument(s);

            public class ArgValues
            {
                public string Description { get; set; }

                public List<string> Values { get; set; }
                public bool Specified { get; set; }

                public ArgValues(string description)
                {
                    Description = description;
                    Values = new List<string>();
                    Specified = false;
                }
            }
        }
    }
}