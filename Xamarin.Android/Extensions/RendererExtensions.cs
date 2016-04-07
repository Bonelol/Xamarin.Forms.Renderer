using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MySIT.Mobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

namespace MySIT.Mobile.Droid.Extensions
{
    public static class RendererExtensions
    {
        public static void FixActionBarBackgroundColor(this IVisualElementRenderer renderer)
        {
            //if (Build.VERSION.SdkInt < BuildVersionCodes.Kitkat)
            //{
            //    var activity = (Activity) Forms.Context;

            //    if(activity.ActionBar == null) return;

            //    activity.ActionBar.SetDisplayShowTitleEnabled(false);
            //    activity.ActionBar.SetDisplayShowTitleEnabled(true);
            //}
        }

        public static void SetNativeControl<TControl, TNative>(this ViewRenderer<TControl, TNative> renderer, View view, ViewGroup parent) where TNative : View where TControl : Xamarin.Forms.View
        {
            var type = renderer.GetType();
            //var genericType = type.MakeGenericType(new Type[] {typeof(ExtendedListView), typeof(ListView)});
            var methodInfo = type.GetMethod("SetNativeControl"
                , BindingFlags.NonPublic | BindingFlags.Instance
                , null
                , new Type[] { typeof(TNative), typeof(ViewGroup) }
                , null);

            methodInfo?.Invoke(renderer, new object[] { view, parent });
        }
    }
}