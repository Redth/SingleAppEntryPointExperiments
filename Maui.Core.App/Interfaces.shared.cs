using System;
using System.Collections.Generic;
using System.Text;

namespace Maui.Core.App
{

	public class MauiStartupAttribute : Attribute
	{
		public MauiStartupAttribute(Type startupType)
			: base()
		{
			StartupType = startupType;
		}

		public Type StartupType { get; set; }
	}

	public interface IStartup
	{
		void Configure(IApplicationBuilder app);
	}

	public interface IService
	{

	}

	public interface IServiceCollection : IList<IService>
	{
	}

	public interface IApplication
	{

	}

	public interface IApplicationBuilder
	{
		IApplication Build();

		void RegisterPlatformLifecycleHandler<THandler>() where THandler : IPlatformLifecycleHandler;
		void RegisterCommonLifecycleHandler<THandler>() where THandler : ICommonLifecycleHandler;
	}

	public interface IApplicationHostEnvironment
	{

	}

	public interface IPlatformLifecycleHandler
	{

	}

	public abstract class CommonLifecycleHandler : ICommonLifecycleHandler
	{
		public virtual void OnCreate(MauiApplication app)
		{
		}

		public virtual void OnDestroy(MauiApplication app)
		{
		}

		public virtual void OnPause(MauiApplication app)
		{
		}

		public virtual void OnResume(MauiApplication app)
		{
		}
	}

	public interface ICommonLifecycleHandler : IPlatformLifecycleHandler
	{
		void OnCreate(MauiApplication app);

		void OnResume(MauiApplication app);

		void OnPause(MauiApplication app);

		void OnDestroy(MauiApplication app);
	}
}
