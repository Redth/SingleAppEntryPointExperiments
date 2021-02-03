using Foundation;
using ObjCRuntime;
using System;
using UIKit;

namespace Maui.Core.App
{

	public abstract class iOSApplicationDelegateHandler : IiOSApplicationDelegateHandler
	{
		public virtual bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
			=> true;

		public virtual bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
			=> false;

		public virtual void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
		{ }
	}

	public interface IiOSApplicationDelegateHandler : IPlatformLifecycleHandler
	{
		bool FinishedLaunching(UIApplication application, NSDictionary launchOptions);

		void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler);

		bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options);
	}


	

	public partial class MauiApplicationDelegate : UIApplicationDelegate
	{
		public MauiApplicationDelegate() : base()
		{
			Init();
		}

		public MauiApplicationDelegate(IntPtr handle) : base(handle)
		{
			Init();
		}

		public virtual void Init()
		{
			Console.WriteLine("Default Init");
		}

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			var result = true;

			// TODO: Get app instance
			foreach (var h in Initializer.Application.CommonLifecycleHandlers)
				h.OnCreate(null);

			foreach (var h in Initializer.Application.PlatformLifecycleHandlers)
			{
				var r = h.FinishedLaunching(application, launchOptions);
				if (!r)
					result = r;
			}

			return result;
		}

		public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
		{
			base.PerformFetch(application, completionHandler);

			foreach (var h in Initializer.Application.PlatformLifecycleHandlers)
				h.PerformFetch(application, completionHandler);
		}

		public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
		{
			var baseResult = base.OpenUrl(app, url, options);

			foreach (var h in Initializer.Application.PlatformLifecycleHandlers)
			{
				var r = h.OpenUrl(app, url, options);
				if (r)
					return r;
			}

			return baseResult;
		}
	}
}
