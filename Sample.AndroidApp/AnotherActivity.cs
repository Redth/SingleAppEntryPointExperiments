using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.AndroidApp
{
	[Application]
	public class MyAApp : Android.App.Application
	{
		public override void OnCreate()
		{
			base.OnCreate();

			Console.WriteLine("ON CREATE");
		}
	}
	
	[Activity(Label = "AnotherActivity", MainLauncher = true)]
	public class AnotherActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
		}
	}
}