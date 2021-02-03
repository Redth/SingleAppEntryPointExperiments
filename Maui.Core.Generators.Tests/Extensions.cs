using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Maui.Core.Generators.Tests
{
	public static class Extensions
	{
		public static void AssertContent(this Compilation compile, string content)
			=> Assert.Contains(compile.SyntaxTrees, s => s.ToString().Contains(content));
		public static void AssertNotContent(this Compilation compile, string content)
			=> Assert.DoesNotContain(compile.SyntaxTrees, s => s.ToString().Contains(content));
	}
}
