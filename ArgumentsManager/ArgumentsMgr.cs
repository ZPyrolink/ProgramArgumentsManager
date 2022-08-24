using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using ArgumentsManager.Exceptions;

namespace ArgumentsManager
{
	public class ArgumentsMgr
	{
		public string AppName { get; }
		public string AppDescription { get; }
		public Version Version { get; }

		public string FullName => $"{AppName} {Version}";

		private readonly List<Argument> _arguments;

		public ArgumentsMgr() : this(Assembly.GetEntryAssembly().FullName) { }

		public ArgumentsMgr(string name) : this(name, null) { }

		public ArgumentsMgr(string name, string description) : this(name, description, new()) { }

		public ArgumentsMgr(string name, string description, Version version)
		{
			AppName = name;
			AppDescription = description;
			Version = version;

			_arguments = new();
		}

		private static bool IsArgument(string s) => s.StartsWith("-") || s.StartsWith("--");

		public void Argument(string format, string description)
		{
			format = format.Replace(" ", string.Empty);

			string[] formats = format.Split(',', ';');

			if (formats.Any(f => !IsArgument(f)))
				throw new ArgumentException($"{format} is not a compatible arguments !");

			Argument(new(formats, description));
		}

		private void Argument(Argument arg) => _arguments.Add(arg);

		public void HelpArgument() => HelpArgument("Show this help page");

		public void HelpArgument(string description) => Argument("-?, --help", description);

		public void VersionArgument() => VersionArgument("Show the application version");

		public void VersionArgument(string description) => Argument("-v, --version", description);

		public void Parse(string[] args)
		{
			Argument argument = null;

			foreach (string arg in args)
			{
				if (IsArgument(arg))
				{
					try
					{
						argument = _arguments.First(a => a.Names.Contains(arg));
					}
					catch (InvalidOperationException)
					{
						int _ = argument is null ?
							throw new NoFirstArgument($"The first argument ({arg}) isn't valid!") :
							throw new UnknownArgument($"{arg} doesn't exists!");
					}
					argument.IsSpecified = true;
				}
				else
				{
					argument.AddValue(arg);
				}
			}
		}
	}
}
