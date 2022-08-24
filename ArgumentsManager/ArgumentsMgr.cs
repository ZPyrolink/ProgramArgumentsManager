using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using ArgumentsManager.Exceptions;

namespace ArgumentsManager
{
	public class ArgumentsMgr : IEnumerable<Argument>
	{
		private const int _DEFAULT_PAD_LEFT = 20;

		#region Properties

		public string AppName { get; }
		public string AppDescription { get; }
		public Version Version { get; }

		public string FullName => $"{AppName} {Version}";

		private readonly List<Argument> _arguments;

		private Argument this[string arg] => this.First(a => a.Names.Contains(arg));

		#endregion

		#region Constructors

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

		#endregion

		private static bool IsArgument(string s) => s.StartsWith("-") || s.StartsWith("--");

		#region Add Arguments

		public void Argument(string format, string description)
		{
			format = format.Replace(" ", string.Empty);

			string[] formats = format.Split(',', ';');

			if (formats.Any(f => !IsArgument(f)))
				throw new ArgumentException($"{format} is not a compatible arguments !");

			Argument(new(formats, description));
		}

		private void Argument(Argument arg) => _arguments.Add(arg);

		#region Help

		public void HelpArgument() => HelpArgument("Show this help page");

		public void HelpArgument(string description) => Argument("-?, --help", description);

		#endregion

		#region Version

		public void VersionArgument() => VersionArgument("Show the application version");

		public void VersionArgument(string description) => Argument("-v, --version", description);

		#endregion

		#endregion

		public void ShowUsage(int padLeft = _DEFAULT_PAD_LEFT) => Console.WriteLine(GetUsage(padLeft));

		public string GetUsage(int padLeft = _DEFAULT_PAD_LEFT)
		{
			string usage = AppName + ":\n\n";
			if (AppDescription is not null)
				usage += "\t" + AppDescription.Replace("\n", "\n\t") + "\n\n";

			usage += "USAGE: " + AppDomain.CurrentDomain.FriendlyName + "\n";

			return _arguments.Aggregate(usage,
				(current, option) =>
					current + option.ToString().PadLeft(padLeft) + " = " + option.Description + "\n");
		}

		public void Parse(string[] args)
		{
			Argument argument = null;

			foreach (string arg in args)
			{
				if (IsArgument(arg))
				{
					try
					{
						argument = this[arg];
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

		public IEnumerator<Argument> GetEnumerator() => _arguments.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _arguments).GetEnumerator();
	}
}
