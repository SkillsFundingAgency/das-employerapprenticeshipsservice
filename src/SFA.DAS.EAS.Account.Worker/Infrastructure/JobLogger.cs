using System;
using System.IO;

namespace SFA.DAS.EAS.Account.Worker.Infrastructure
{
	static class JobLogger
	{
		public static void WriteLine(TextWriter textWriter, string message)
		{
			Console.WriteLine(message);
			textWriter.WriteLine(message);
		}
	}
}