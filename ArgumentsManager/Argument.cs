using System;
using System.Collections;
using System.Collections.Generic;

namespace ArgumentsManager
{
	public class Argument : IEnumerable<string>
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

		public override string ToString() => string.Join(", ", Names);

		public IEnumerator<string> GetEnumerator() => Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) Values).GetEnumerator();
	}
}
