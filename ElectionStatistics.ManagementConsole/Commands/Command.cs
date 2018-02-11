using System;

namespace ElectionStatistics.ManagementConsole
{
	public abstract class Command
	{
		public abstract string Name { get; }
		public abstract void Execute(IServiceProvider services, string[] arguments);
	}
}