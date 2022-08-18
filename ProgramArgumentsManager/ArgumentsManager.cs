using ProgramArgumentsManager.Exceptions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgramArgumentsManager
{
	public partial class ArgumentsManager
	{
		private const int _DEFAULT_PAD_LEFT = 20;

		public string AppName { get; }
		public string AppDescription { get; }
		public string Version { get; }

		public string FullName => AppName + " " + Version;

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

			char[] delimiters = { ',', ';' };
			string[] formats = format.Split(delimiters);

			if (formats.Any(f => !f.StartsWith("-") && !f.StartsWith("--")))
				throw new ArgumentException("Le format '" + format + " n'est pas comaptible en tant que paramètre !",
					nameof(format));

			AddArgument(new Argument(formats, description));
		}

		private void AddArgument(Argument arg) => _options.Add(arg, new Argument.ArgValues());

		// public void AddPositionnalArgument(string name, )

		public void AddHelpArgument() => AddArguments("-?, --help", "Show this help page");

		public bool CheckHelp() => IsSpecified("?");

		public void CheckHelp<T>(Action callback)
		{
			if (IsSpecified("?"))
				callback();
		}

		public void CheckHelp<T1>(Action<T1> callback, T1 arg1)
		{
			if (IsSpecified("?"))
				callback(arg1);
		}

		public void CheckHelp<T1, T2>(Action<T1, T2> callback, T1 arg1, T2 arg2)
		{
			if (IsSpecified("?"))
				callback(arg1, arg2);
		}

		public void CheckHelp<TResult>(Func<TResult> callback)
		{
			if (IsSpecified("?"))
				callback();
		}

		public void CheckHelp<TResult, T1>(Func<T1, TResult> callback, T1 param1)
		{
			if (IsSpecified("?"))
				callback(param1);
		}

		public void CheckHelp<TResult, T1, T2>(Func<T1, T2, TResult> callback, T1 param1, T2 param2)
		{
			if (IsSpecified("?"))
				callback(param1, param2);
		}

		public void AddVersionArgument() => AddArguments("-v, --version", "Show the app version");

		public bool CheckVersion() => IsSpecified("v");

		public void Parse(string[] args)
		{
			string lastArg = null;
			foreach (string arg in args)
			{
				try
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
				catch (KeyNotFoundException)
				{
					int i = lastArg is null ?
						throw new FirstNoArgumentException($"The first command ({arg}) isn't an argument !") :
						throw new UnknownArgumentException(lastArg, $"{lastArg} doesn't exist !");
				}
			}

			foreach (Argument.ArgValues argValues in _options.Values.Where(argValues => !argValues.Specified || !argValues.Values.Any()))
				argValues.Values = null;
		}

		public bool IsSpecified(string arg) => _options[arg].Specified;

		public List<string> GetValues(string arg) => _options[arg].Values;

		public string GetValue(string arg) => _options[arg].Values is null ? null : string.Join(" ", _options[arg].Values);

		public bool HasValue(string arg) => _options[arg].Values != null;

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

		public void ForceState(string arg, Argument.Ask ask) => _options.Keys.First(k => k.Names.Contains(arg)).Asked = ask;

		public void CheckRequired()
		{
			foreach (Argument argument in _options.Keys.Where(a => a.Asked == Argument.Ask.Required && !IsSpecified(a.Names[0])))
				throw new ArgumentRequiredException(argument);
		}

		public List<Argument> GetMissingArguments() => GetMissingArguments(false);
		public List<Argument> GetMissingArguments(bool permitNull)
		{
			Func<Argument, bool> predicate = a => permitNull ?
				a.Asked == Argument.Ask.Required && !IsSpecified(a.Names[0]) :
				!HasValue(a.Names[0]);

			return _options.Keys.Where(predicate).ToList();
		}
	}
}