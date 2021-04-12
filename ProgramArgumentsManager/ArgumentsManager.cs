using System;
using System.CodeDom;
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
            format = format.Trim();
            
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
            private string[] _names { get; }

            public Argument(string name) : this(new []{name}) { }
            public Argument(string name1, string name2) : this(new[] {name1, name2}) { }
            public Argument(string[] names)
            {
                _names = names;
            }

            public bool Contain(string name) => _names.Contains(name);
        }
    }
}