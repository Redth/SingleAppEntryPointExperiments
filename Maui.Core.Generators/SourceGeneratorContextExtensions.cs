using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Maui.Core.Generators
{
	internal static class SourceGeneratorContextExtensions
	{
		public static string GetMSBuildItemMetadata(
			this GeneratorExecutionContext context,
			AdditionalText additionalText,
			string name,
			string defaultValue = default)
		{
			context.AnalyzerConfigOptions
					   .GetOptions(additionalText)
					   .TryGetValue($"build_metadata.AdditionalFiles.{name}", out var value);

			return value ?? defaultValue;
		}

		public static string GetMSBuildProperty(
		   this GeneratorExecutionContext context,
		   string name,
		   string defaultValue = "")
			{
				context
					.AnalyzerConfigOptions
					.GlobalOptions
					.TryGetValue($"build_property.{name}", out var value);
				return value ?? defaultValue;
			}

		public static string GetCompilationContextTFM(this GeneratorExecutionContext context)
		{
			var attr = context.Compilation.Assembly.GetAttributes();

			foreach (var a in attr)
			{
				if (a.AttributeClass.Name == "TargetFrameworkAttribute"
					|| a.AttributeClass.Name == "TargetFramework")
				{
					if (a.ConstructorArguments.Length > 0)
						return a.ConstructorArguments[0].Value?.ToString();
				}
			}

			return null;
		}

		public static IEnumerable<AttributeData> FindAttributes(this GeneratorExecutionContext context, string attributeTypeName)
		{
			var attr = context.Compilation.Assembly.GetAttributes();
			
			foreach (var a in context.Compilation.Assembly.GetAttributes())
			{
				if (a.AttributeClass.Name == attributeTypeName)
					yield return a;
			}

			foreach (var r in context.Compilation.References)
			{
				if (r.Properties.Kind == MetadataImageKind.Assembly)
				{
					var assembly = (IAssemblySymbol)context.Compilation.GetAssemblyOrModuleSymbol(r);

					foreach (var a in assembly.GetAttributes())
						if (a.AttributeClass.Name == attributeTypeName)
							yield return a;
				}
			}
		}

		public static string GetAssemblyMetadata(this GeneratorExecutionContext context, string key)
		{
			foreach (var a in context.FindAttributes("AssemblyMetadataAttribute"))
			{
				if (a.ConstructorArguments.Length == 2 && a.ConstructorArguments[0].Value?.ToString() == key)
					return a.ConstructorArguments[1].Value?.ToString();		
			}

			return null;
		}

		public static bool IsiOS(this GeneratorExecutionContext context)
			=> (context.GetMSBuildProperty("TargetFrameworkIdentifier")?.Contains("Xamarin.iOS") ?? false)
				|| (context.GetCompilationContextTFM()?.Contains("Xamarin.iOS") ?? false);

		public static bool IsAndroid(this GeneratorExecutionContext context)
			=> (context.GetMSBuildProperty("TargetFrameworkIdentifier")?.Contains("MonoAndroid") ?? false)
				|| (context.GetCompilationContextTFM() ?.Contains("MonoAndroid") ?? false);

		public static bool IsAppHead(this GeneratorExecutionContext context)
			=> context.GetMSBuildProperty("OutputType")?.Equals("Exe", StringComparison.OrdinalIgnoreCase) ?? false;

	}
}
