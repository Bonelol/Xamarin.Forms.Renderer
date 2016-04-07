using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using CoreGraphics;
using Foundation;
using MySIT.Mobile.Controls;
using MySIT.Mobile.iOS.Controls;
using MySIT.Mobile.iOS.Extensions;
using MySIT.Mobile.iOS.Renderers;
using MySIT.Mobile.iOS.Services;
using MySIT.Mobile.Models.SocialMedias;
using MySIT.Mobile.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PostListView), typeof(PostUICollectionViewRenderer))]
namespace MySIT.Mobile.iOS.Renderers
{
    public class PostUICollectionViewRenderer : ViewRenderer<PostListView, WaterfallCollectionView>
    {
        private FormsUIRefreshControl _uiRefreshControl;

        protected override void OnElementChanged(ElementChangedEventArgs<PostListView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var waterfallCollectionLayout = new WaterfallCollectionLayout {ColumnCount = Device.Idiom == TargetIdiom.Tablet ? 2 : 1};
                waterfallCollectionLayout.SizeForItem += (collectionView, layout, indexPath) =>
                {
                    var source = collectionView.DataSource as WaterfallCollectionSource;
                    var size = source?.GetSize(indexPath);

                    return size ?? new CGSize(0, 0);
                };

                var waterfallCollectionView = new WaterfallCollectionView(new CGRect(0, 0, 300, 300), waterfallCollectionLayout) {AlwaysBounceVertical = true};
                var datasource = new WaterfallCollectionSource(this, waterfallCollectionView, Device.Idiom == TargetIdiom.Tablet ? 2 : 1);
                _uiRefreshControl = new FormsUIRefreshControl {RefreshCommand = e.NewElement.RefreshCommand};
                waterfallCollectionView.Source = datasource;
                waterfallCollectionView.Add(_uiRefreshControl);
                this.SetNativeControl(waterfallCollectionView);
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            //if (this.Element != null)
            //{
            //    var request1 = this.Element.GetSizeRequest(this.Bounds.Width, this.Bounds.Height);
            //    var request2 = this.GetDesiredSize(this.Bounds.Width, this.Bounds.Height);
            //    this.Element.Layout(new Rectangle(0, 0, request2.Request.Width, request2.Request.Height));
            //}

            //if (this.Control != null)
            //{
            //    this.Control.Frame = new CGRect((nfloat)0, (nfloat)0, (nfloat)this.Element.Width, (nfloat)this.Element.Height);
            //    this.Control.Bounds = this.Bounds;
            //    this.Control.Frame = this.Frame;
            //}
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "ItemsSource")
            {
                //this.Control.ReloadData();

                var list = this.Element.ItemsSource as PostCollection;

                if (list == null || this.Control == null) return;

                list.CollectionChanged += (o, args) =>
                {
                    this.Control?.SetContentOffset(CGPoint.Empty, false);
                    this.Control?.ReloadData();
                };
            }
            else if (e.PropertyName == ListView.IsRefreshingProperty.PropertyName)
            {
                _uiRefreshControl.IsRefreshing = this.Element.IsRefreshing;
            }
            else if (e.PropertyName == ListView.RefreshCommandProperty.PropertyName)
            {
                _uiRefreshControl.RefreshCommand = this.Element.RefreshCommand;
            }
        }
    }
}
