using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgramArgumentsManager
{
    public class ArgumentsManager
    {
        private const int _DEFAULT_PAD_LEFT = 20;

        public string AppName { get; }
        public string AppDescription { get; }
        public string Version { get; }

        private readonly Dictionary<Argument, Argument.ArgValues> _options;

        public ArgumentsManager(string name) : this(name, "1.0.0") { }

        public ArgumentsManager(string name, string version) : this(name, version, null) { }

        public ArgumentsManager(string name, string version, string description)
        {
            AppName = name;
            Version = version;
            AppDescription = description;

            _options = new Dictionary<Argument, Argument.ArgValues>();
        }

        private static bool IsArgument(string s) => s.StartsWith("-") || s.StartsWith("--");

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

            _options.Add(new Argument(formats, description), new Argument.ArgValues());
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

        public void ShowUsage(int padLeft = _DEFAULT_PAD_LEFT) => Console.WriteLine(GetUsage(padLeft));

        public string GetUsage(int padLeft = _DEFAULT_PAD_LEFT)
        {
            string usage = AppName + ":\n\n";
            if (!(AppDescription is null))
                usage += "\t" + AppDescription.Replace("\n", "\n\t") + "\n\n";

            usage += "USAGE: " + AppDomain.CurrentDomain.FriendlyName + "\n";

            return _options.Aggregate(usage,
                (current, option) =>
                    current + option.Key.ToString().PadLeft(padLeft) + " = " + option.Key.Description + "\n");
        }

        public class Argument
        {
            private enum Ask
            {
                Required,
                Optional
            }

            public string[] Names { get; }
            public string Description { get; }
            private readonly Ask _ask;

            internal Argument(string name, string desc) : this(new[] {name}, desc) { }

            internal Argument(string[] names, string desc)
            {
                Names = names;
                Description = desc;

                if (desc.StartsWith("[OPTIONAL]", StringComparison.InvariantCultureIgnoreCase))
                    _ask = Ask.Optional;
                else if (desc.StartsWith("[REQUIRED]", StringComparison.InvariantCultureIgnoreCase))
                    _ask = Ask.Required;
                else
                    _ask = Ask.Required;
            }

            private bool Equals(Argument other) => other.Names.Any(s => Names.Contains(s));

            public override string ToString() => string.Join(", ", Names);

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

            public static implicit operator Argument(string s) => new Argument(s, "Converted argument");

            public class ArgValues
            {
                public List<string> Values { get; internal set; }
                public bool Specified { get; internal set; }

                internal ArgValues()
                {
                    Values = new List<string>();
                    Specified = false;
                }
            }
        }
    }
}