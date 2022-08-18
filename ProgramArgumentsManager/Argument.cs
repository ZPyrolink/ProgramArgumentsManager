using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgramArgumentsManager
{
	public class Argument
	{
		public enum Ask
		{
			Required,
			Optional
		}

		public string[] Names { get; }
		public string Description { get; }
		public Ask Asked { get; internal set; }

		internal Argument(string name, string desc) : this(new[] { name }, desc) { }

		internal Argument(string[] names, string desc)
		{
			Names = names;
			Description = desc;

			if (desc.StartsWith("[OPTIONAL]", StringComparison.InvariantCultureIgnoreCase))
				Asked = Ask.Optional;
			else if (desc.StartsWith("[REQUIRED]", StringComparison.InvariantCultureIgnoreCase))
				Asked = Ask.Required;
			else
				Asked = Ask.Optional;
		}

		private bool Equals(Argument other) => other.Names.Any(s => Names.Contains(s) ||
			Names.Contains("-" + s) || Names.Contains("--" + s));

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