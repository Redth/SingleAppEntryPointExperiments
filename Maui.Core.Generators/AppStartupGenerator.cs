using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using System.Diagnostics;
using System.Composition;
using System.Linq;
using System.Collections.Generic;

namespace Maui.Core.Generators
{
	[Generator]
	public class AppStartupGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
			context.RegisterForSyntaxNotifications(() => new AppStartupSyntaxReceiver());
#if DEBUG
			if (!Debugger.IsAttached)
			{
				//Debugger.Launch();
			}
#endif
		}

		class AppStartupSyntaxReceiver : ISyntaxReceiver
		{
			public bool HasiOSMainMethod { get; set; } = false;
			public bool HasiOSAppDelegateSubclass { get; set; } = false;
			public string iOSPartialAppDelegateSubclassType { get; set; } = null;

			public bool HasAndroidApplicationSubclass { get; set; } = false;

			public bool HasAndroidMainLauncher { get; set; } = false;

			public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
			{
				// Look for a Main method
				if (!HasiOSMainMethod
					&& syntaxNode is MethodDeclarationSyntax method
					&& method.Modifiers.Any(m =>
						m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.StaticKeyword)
						&& m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.VoidKeyword))
					&& method.Identifier.Text.Equals("Main", System.StringComparison.OrdinalIgnoreCase))
					HasiOSMainMethod = true;

				if (!HasAndroidMainLauncher && syntaxNode is AttributeSyntax attrSyntax && attrSyntax.Name.ToFullString() == "Activity")
				{
					HasAndroidMainLauncher = attrSyntax.ArgumentList != null
							&& attrSyntax.ArgumentList.ChildNodes().Any(cn => cn is AttributeArgumentSyntax aas
								&& aas.NameEquals?.Name?.Identifier.ValueText == "MainLauncher"
								&& aas.Expression is ExpressionSyntax exs
								&& exs.Kind() == Microsoft.CodeAnalysis.CSharp.SyntaxKind.TrueLiteralExpression);
				}

				// Look for an AppDelegate implementation of some sort
				if (syntaxNode is SimpleBaseTypeSyntax baseTypeSyntax)
				{
					var baseId = baseTypeSyntax.ToFullString().Trim();

					if (baseId.Equals("Maui.Core.App.MauiApplicationDelegate"))
					{
						HasiOSAppDelegateSubclass = true;

						// Check if it's a partial declaration, if so, get the type name back and see if it's partial to us
						// or if it's something else
						var partialClassSyntax = baseTypeSyntax.Ancestors().FirstOrDefault(n =>
							n is ClassDeclarationSyntax cp
							&& cp.Modifiers.Any(m => m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)));

						if (partialClassSyntax is ClassDeclarationSyntax classDeclParent)
						{
							iOSPartialAppDelegateSubclassType = classDeclParent.GetFullName();
							if (!iOSPartialAppDelegateSubclassType.StartsWith("global::"))
								iOSPartialAppDelegateSubclassType = "global::" + iOSPartialAppDelegateSubclassType;
						}
					}
					else if (baseId.Equals("UIKit.UIApplicationDelegate"))
					{
						HasiOSAppDelegateSubclass = true;
					}

					if (!HasAndroidApplicationSubclass && baseId.Equals("Maui.Core.App.MauiAndroidApplication")
						|| baseId.Equals("Android.App.Application"))
						HasAndroidApplicationSubclass = true;
				}
			}
		}

		public void Execute(GeneratorExecutionContext context)
		{
			var isIos = context.IsiOS();
			var isAndroid = context.IsAndroid();

			if (!isIos && !isAndroid)
				return;

			var syntaxReceiver = (AppStartupSyntaxReceiver)context.SyntaxReceiver;

			var namespaceName = context.Compilation.GlobalNamespace.Name;

			if (string.IsNullOrEmpty(namespaceName))
				namespaceName = context.Compilation.AssemblyName ?? "GeneratedMauiApp";

			if (context.IsAppHead())
			{
				// Look for a `[assembly: MauiStartup(typeof(StartupClass))]` attribute
				var startupAttributes = context.FindAttributes("MauiStartupAttribute")?.ToList();

				// If we have multiple, throw an error
				if ((startupAttributes?.Count ?? 0) > 1)
				{
					context.ReportDiagnostic(Diagnostic.Create("MAUI1010", "Compiler", "More than one MauiStartupAttribute found.", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0));
					return;
				}

				var startupAttribute = startupAttributes.FirstOrDefault();

				// If we don't have any, 
				if (startupAttribute != null)
				{
					// Get the constructor arg which we expect to be the startup class type name
					var ctorTypeArg = startupAttribute.ConstructorArguments.FirstOrDefault();

					var startupClassName = ctorTypeArg.Value?.ToString();

					if (!string.IsNullOrEmpty(startupClassName))
					{
						// Prefix with global:: when we generate code to be safe
						if (!startupClassName.StartsWith("global::"))
							startupClassName = $"global::{startupClassName}";

						if (isIos)
						{
							// Prefix with global:: when we generate code to be safe
							var appDelegateClassName = $"{namespaceName}.AppDelegate";
							if (!appDelegateClassName.StartsWith("global::"))
								appDelegateClassName = $"global::{appDelegateClassName}";

							// Create an app delegate
							// We check to see if there's already an app delegate
							// If there is, if it's partial and the same name as the one we generate, that's fine too
							if (!syntaxReceiver.HasiOSAppDelegateSubclass || syntaxReceiver.iOSPartialAppDelegateSubclassType.Equals(appDelegateClassName))
							{
								context.AddSource("Maui_Generated_MauiGeneratedAppDelegate.cs",
									GenerateiOSAppDelegate(namespaceName, startupClassName));
							}
							else
								context.ReportDiagnostic(Diagnostic.Create("MAUI1020", "Compiler", "UIApplicationDelegate implementation already exists, not generating one.", DiagnosticSeverity.Warning, DiagnosticSeverity.Warning, true, 3));

							// Create a main method
							if (!syntaxReceiver.HasiOSMainMethod)
							{
								context.AddSource("Maui_Generated_MauiGeneratedMain.cs",
									GenerateiOSMain(namespaceName, appDelegateClassName));
							}
							else
								context.ReportDiagnostic(Diagnostic.Create("MAUI1021", "Compiler", "Main method implementation already exists, not generating one.", DiagnosticSeverity.Warning, DiagnosticSeverity.Warning, true, 3));
						}
						else if (isAndroid)
						{
							var appName = context.GetMSBuildProperty("ApplicationTitle") ?? "App1";

							context.AddSource("Maui_Generated_MauiGeneratedAndroidActivity.cs",
								GenerateAndroidMainActivity(appName, namespaceName, startupClassName));

							if (!syntaxReceiver.HasAndroidMainLauncher)
							{
								context.AddSource("Maui_Generated_MauiGeneratedAndroidApplication.cs",
									GenerateAndroidApplication(namespaceName, startupClassName));
							}
							else
							{
								context.ReportDiagnostic(Diagnostic.Create("MAUI1021", "Compiler", "Activity with MainLauncher=true already exists, not generating one.", DiagnosticSeverity.Warning, DiagnosticSeverity.Warning, true, 3));
							}
						}
					}
				}
			}
		}

		string GenerateAndroidMainActivity(string appName, string namespaceName, string startupClassName)
			=> @"
#if __ANDROID__
namespace " + namespaceName + @"
{
	[global::System.CodeDom.Compiler.GeneratedCode(""Maui.Generators"", ""1.0.0.0"")]
	[global::Android.App.Activity(Label = """ + appName + @""", MainLauncher = true)]
	public partial class GeneratedAndroidMainActivity : global::AndroidX.AppCompat.App.AppCompatActivity
	{
	}
}
#endif
";

		string GenerateAndroidApplication(string namespaceName, string startupClassName)
			=> @"
#if __ANDROID__
namespace " + namespaceName + @"
{
	[global::System.CodeDom.Compiler.GeneratedCode(""Maui.Generators"", ""1.0.0.0"")]
	[global::Android.App.Application]
	public partial class GeneratedAndroidApplication : global::Android.App.Application
	{
		public override void OnCreate()
		{
			global::Maui.Core.App.Initializer.Startup(new " + startupClassName + @"());
			base.OnCreate();
		}
	}
}
#endif
";
		string GenerateiOSMain(string namespaceName, string appDelegateClassName)
			=> @"
#if __IOS__
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace " + namespaceName + @"
{
	[global::System.CodeDom.Compiler.GeneratedCode(""Maui.Generators"", ""1.0.0.0"")]
    public partial class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            UIApplication.Main(args, null, typeof(" + appDelegateClassName + @"));
        }
    }
}
#endif
";

		string GenerateiOSAppDelegate(string namespaceName, string startupClassName)
			=> @"
#if __IOS__
using UIKit;
using Foundation;

namespace " + namespaceName + @"
{
	[global::System.CodeDom.Compiler.GeneratedCode(""Maui.Generators"", ""1.0.0.0"")]
	[Register(nameof(AppDelegate))]
	public partial class AppDelegate : global::Maui.Core.App.MauiApplicationDelegate
	{
		public override void Init()
		{
			global::Maui.Core.App.Initializer.Startup(new " + startupClassName + @"());
		}
	}
}
#endif
";
	}
}
