using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maui.Core.App
{
	public interface IAndroidLifecycleHandler : IPlatformLifecycleHandler
	{
		void ApplicationOnCreate(Android.App.Application app);


		void ActivityOnCreate(Activity activity, Bundle savedInstanceState);

		void ActivityOnPause(Activity activity);

		void ActivityOnResume(Activity activity);

		void ApplicationOnActivityCreated(Activity activity, Bundle savedInstanceState);

	}

	public class AndroidLifecycleHandler : IAndroidLifecycleHandler
	{
		public virtual void ActivityOnCreate(Activity activity, Bundle savedInstanceState)
		{
		}

		public virtual void ActivityOnPause(Activity activity)
		{
		}

		public virtual void ActivityOnResume(Activity activity)
		{
		}

		public virtual void ApplicationOnCreate(Application app)
		{
		}

		public virtual void ApplicationOnActivityCreated(Activity activity, Bundle savedInstanceState)
		{
		}
	}

	public class MauiAndroidApplication : Android.App.Application
	{

		LifecycleCallbacks lifecycleCallbacks;

		public override void OnCreate()
		{
			base.OnCreate();

			Init();

			
			var mauiApp = Initializer.Application;

			lifecycleCallbacks = new LifecycleCallbacks(mauiApp);
			
			RegisterActivityLifecycleCallbacks(lifecycleCallbacks);


			foreach (var h in Initializer.Application.PlatformLifecycleHandlers)
				h.ApplicationOnCreate(this);

			// TODO: Get app instance
			foreach (var h in Initializer.Application.CommonLifecycleHandlers)
				h.OnCreate(null);
		}

		public virtual void Init()
		{
			Console.WriteLine("Default Init");
		}

	}

	class LifecycleCallbacks : Java.Lang.Object, Application.IActivityLifecycleCallbacks
	{
		public LifecycleCallbacks(MauiApplication mauiApplication)
		{
			mauiApp = mauiApplication;
		}

		readonly MauiApplication mauiApp;

		public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
		{
			foreach (var h in mauiApp.PlatformLifecycleHandlers)
				h.ApplicationOnActivityCreated(activity, savedInstanceState);
		}

		public void OnActivityDestroyed(Activity activity)
		{
		}

		public void OnActivityPaused(Activity activity)
		{
		}

		public void OnActivityResumed(Activity activity)
		{
		}

		public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
		{
		}

		public void OnActivityStarted(Activity activity)
		{
		}

		public void OnActivityStopped(Activity activity)
		{
		}
	}

	public class MauiActivity : AppCompatActivity
	{
		public virtual void Init()
		{
			Console.WriteLine("Default Init");
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			Init();

			foreach (var h in Initializer.Application.PlatformLifecycleHandlers)
				h.ActivityOnCreate(this, savedInstanceState);
		}

		protected override void OnResume()
		{
			base.OnResume();

			foreach (var h in Initializer.Application.PlatformLifecycleHandlers)
				h.ActivityOnResume(this);
		}

		protected override void OnPause()
		{
			foreach (var h in Initializer.Application.PlatformLifecycleHandlers)
				h.ActivityOnPause(this);

			base.OnPause();
		}
	}
}
