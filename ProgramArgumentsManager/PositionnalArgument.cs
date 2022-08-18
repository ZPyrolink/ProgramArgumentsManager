namespace ProgramArgumentsManager
{
	public class PositionnalArgument : Argument
	{
		private static int _CURRENT_INDEX = 0;
		private int _index;

		internal PositionnalArgument(string name, string desc) : base(name, desc) => _index = _CURRENT_INDEX++;

		internal PositionnalArgument(string[] names, string desc) : base(names, desc) => _index = _CURRENT_INDEX++;
	}
}
