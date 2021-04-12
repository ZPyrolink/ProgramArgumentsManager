using System;
using System.CodeDom;
using System.Collections.Generic;

namespace ProgramArgumentsManager
{
    public class Arguments
    {
        public string ApplicationName { get; }
        public string Version { get; }

        private Dictionary<string, string> _options;

        public Arguments(string name) : this(name, "1.0.0") { }

        public Arguments(string name, string version)
        {
            ApplicationName = name;
            Version = version;

            _options = new Dictionary<string, string>();
        }

        public void AddArguments(string format, string description)
        {
            format = format.Trim();
            
            if (!format.Contains("-") && !format.Contains("--"))
                throw new ArgumentException("Le format '" + format + " n'est pas comaptible en tant que paramètre !",
                    nameof(format));

            _options.Add(format, description);
        }
    }
}