using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ArgumentsManager
{
	public class Argument
	{
		public const string REQUIRED = "[REQUIRED]";
		public const string OPTIONAL = "[OPTIONAL]";

		public string[] Names { get; }

		public string Description { get; }

		public bool IsRequired { get; }

		public List<string> Values { get; private set; }

		public bool IsSpecified { get; internal set; }

		internal Argument(string name, string desc) : this(new[]{name}, desc) {}

		internal Argument(string[] names, string desc)
		{
			Names = names;
			Description = desc;

			IsRequired = desc.StartsWith(REQUIRED, StringComparison.InvariantCultureIgnoreCase);
		}

		internal void AddValue(string value)
		{
			Values ??= new();
			Values.Add(value);
		}
	}
}
