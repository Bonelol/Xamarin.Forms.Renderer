using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace MySIT.Mobile.Droid.Extensions
{
    public static class ViewExtensions
    {
        private static readonly Type _platformType = Type.GetType("Xamarin.Forms.Platform.Android.Platform, Xamarin.Forms.Platform.Android", true);
        private static BindableProperty _rendererProperty;

        public static BindableProperty RendererProperty
        {
            get
            {
                _rendererProperty = (BindableProperty)_platformType.GetField("RendererProperty", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    .GetValue(null);

                return _rendererProperty;
            }
        }

        public static IVisualElementRenderer GetRenderer(this VisualElement visualElement)
        {
            var renderer = Platform.GetRenderer(visualElement);

            if (renderer == null)
            {
                renderer = Platform.CreateRenderer(visualElement);
                Platform.SetRenderer(visualElement, renderer);
            }

            return renderer;
        }

        public static void SetRenderer(this BindableObject bindableObject, IVisualElementRenderer renderer)
        {
            var setRendererMethod = _platformType.GetMethod("SetRenderer");
            setRendererMethod.Invoke(null, new object[] { bindableObject, renderer });
        }

        public static Android.Views.View GetNativeView(this VisualElement visualElement)
        {
            var renderer = visualElement.GetRenderer();
            var viewGroup = renderer.ViewGroup;
            var rootView = viewGroup.RootView;
            return rootView;
        }
    }
}