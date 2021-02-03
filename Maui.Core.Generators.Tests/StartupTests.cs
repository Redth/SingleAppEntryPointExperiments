using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Maui.Core.Generators.Tests
{
	public class StartupTests : BaseSourceGeneratorTests<Maui.Core.Generators.AppStartupGenerator>
	{
		public StartupTests(ITestOutputHelper output)
			: base(output, "Maui.Core.App")
		{

			Generator.AddMSBuildProperty("TargetFrameworkIdentifier", "Xamarin.iOS");
			Generator.AddMSBuildProperty("OutputType", "Exe");

			// Add our startup so the startup generators will even run
			this.Generator.AddSource(@"
using Maui.Core.App;

[assembly: MauiStartup(typeof(MyApp.Startup))]

namespace MyApp
{
	public class Startup : IStartup
	{
		public void Configure(IApplicationBuilder app)
		{
		}
	}
}
");
		}

		[Fact]
		public void AppDelegateCreated()
		{
			
			this.RunGenerator();
			this.Compilation.AssertContent("AppDelegate");
			
		}
	}
}
