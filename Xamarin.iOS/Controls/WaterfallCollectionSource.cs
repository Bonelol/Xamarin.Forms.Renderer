using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using CoreGraphics;
using ExtensionsLibrary;
using FFImageLoading;
using FFImageLoading.Work;
using Foundation;
using MySIT.Mobile.Extensions;
using MySIT.Mobile.iOS.Renderers;
using MySIT.Mobile.iOS.Views;
using MySIT.Mobile.Models.SocialMedias;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace MySIT.Mobile.iOS.Controls
{
    public class WaterfallCollectionSource : UICollectionViewSource
    {
        private const string CELL_IDENTIFIER = "TableCell";
        private readonly Dictionary<int, Vec2> _caches;
        private readonly Dictionary<int, ImageSize> _sizes;
        private readonly PostUICollectionViewRenderer _renderer;
        private readonly bool _loadImages;
        private readonly nfloat _listViewWidth;
        private readonly int _columnCount = 1;

       #region Computed Properties
        public WaterfallCollectionView CollectionView { get; set;}
        #endregion

        #region Constructors
        public WaterfallCollectionSource (WaterfallCollectionView collectionView, int columnCount = 1)
        {
            // Initialize
            CollectionView = collectionView;
            _columnCount = columnCount;
        }

        public WaterfallCollectionSource(PostUICollectionViewRenderer renderer, WaterfallCollectionView collectionView, int columnCount = 1)
        {
            _renderer = renderer;
            _columnCount = 1;// renderer.Element.ColumnCount;
            _caches = new Dictionary<int, Vec2>();
            _sizes = new Dictionary<int, ImageSize>();
            //_listViewWidth = renderer.Control.Bounds.Width;

            int option = (int) NSUserDefaults.StandardUserDefaults.IntForKey("displayImageOption");
            _loadImages = option < 3 && (option != 2 || CrossConnectivity.Current.ConnectionTypes.Contains(ConnectionType.WiFi));

            CollectionView = collectionView;

            _columnCount = columnCount;
        }
        #endregion

        #region Override Methods
        public override nint NumberOfSections (UICollectionView collectionView) 
        {
            // We only have one section
            return 1;
        }

        public override nint GetItemsCount (UICollectionView collectionView, nint section) 
        {
            var list = _renderer.Element?.ItemsSource as PostCollection;

            return list?.Count ?? 0;
        }

        public override bool CanMoveItem (UICollectionView collectionView, NSIndexPath indexPath) 
        {
            return false;
        }

        public Post GetItem(NSIndexPath indexPath)
        {
            if (_renderer.Element?.BindingContext == null)
                return new Post();

            var list = _renderer.Element.ItemsSource as PostCollection;

            if (list == null) return new Post();

            var item = list.ElementAtOrDefault(indexPath.Row);

            return item;
        }

        public override UICollectionViewCell GetCell(UICollectionView tableView, NSIndexPath indexPath)
        {
            if (indexPath == null) //TODO indexPath == null
            {
                var tempCell = new UICollectionViewCell
                {
                    Bounds = new CGRect(0, 0, 0, 0),
                    Frame = new CGRect(0, 0, 0, 0),
                    Hidden = true
                };
                return tempCell;
            }

            var item = this.GetItem(indexPath);
            
            if (item == null) return new UICollectionViewCell();

            var cell = tableView.DequeueReusableCell(CELL_IDENTIFIER, indexPath) as PostUICollectionViewCell ?? new PostUICollectionViewCell
            {
                ClipsToBounds = true,
                Tag = indexPath.Row
            };

            cell.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                var methodInfo = this._renderer.Element.GetType()
                    .GetMethod("NotifyRowTapped", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(int), typeof(Cell) }, null);

                //MessagingCenter.Send<IVisualElementRenderer>((IVisualElementRenderer)collectionView.Superview, "Xamarin.ResignFirstResponder");
                methodInfo.Invoke(this._renderer.Element, new object[] { indexPath.Row, null });
            }));

            cell.PostView.SetMaxWidth(_renderer.Bounds.Width/_columnCount);

            cell.PostView.TitleTextView.Text = item.Title;

            NSError error = null;
            var attributedString = new NSAttributedString(NSData.FromString(item.Type == SocialMediaType.Flickr ? item.Content.RemoveTags().Ellipsize(255) : item.Content.Ellipsize(255)), new NSAttributedStringDocumentAttributes() { DocumentType = NSDocumentType.HTML }, ref error);
            cell.PostView.ContentTextView.AttributedText = attributedString;
            //cell.PostView.ContentTextView.Text = item.Content.Ellipsize(255);
            cell.PostView.NameTextView.Text = item.Name;
            attributedString = new NSAttributedString(NSData.FromString(item.Description.Ellipsize(100)), new NSAttributedStringDocumentAttributes() { DocumentType = NSDocumentType.HTML }, ref error);
            cell.PostView.DescTextView.AttributedText = attributedString;
            //cell.PostView.DescTextView.Text = item.Description.Ellipsize(100);
            cell.PostView.ThemeBar.BackgroundColor = item.ThemeColor.ToUIColor();
            cell.PostView.IconImageView.Image = UIImage.FromFile(item.IconUrl);

            // if it has a photo
            if (!string.IsNullOrEmpty(item.ImageUrl) && _loadImages)
            {
                var image = UIImage.FromFile("placeholder_news.png");
                cell.PostView.PhotoImageView.Image = image;
                cell.PostView.PhotoImageView.Hidden = false;

                cell.PostView.LoadNewsPhoto(item.ImageUrl);
            }
            else
            {
                cell.PostView.PhotoImageView.Image = UIImage.FromFile("placeholder_news.png");
                cell.PostView.PhotoImageView.Hidden = true;
            }

            cell.PostView.UpdateLayout();

            return cell;
        }


        public CGSize GetSize(Post item)
        {
            var cell = new PostUICollectionViewCell();

            cell.PostView.SetMaxWidth(_renderer.Bounds.Width / _columnCount);

            cell.PostView.TitleTextView.Text = item.Title;

            NSError error = null;
            var attributedString = new NSAttributedString(NSData.FromString(item.Type == SocialMediaType.Flickr ? item.Content.RemoveTags().Ellipsize(255) : item.Content.Ellipsize(255)), new NSAttributedStringDocumentAttributes() { DocumentType = NSDocumentType.HTML }, ref error);
            cell.PostView.ContentTextView.AttributedText = attributedString;
            //cell.PostView.ContentTextView.Text = item.Content.Ellipsize(255);
            cell.PostView.NameTextView.Text = item.Name;
            attributedString = new NSAttributedString(NSData.FromString(item.Description.Ellipsize(100)), new NSAttributedStringDocumentAttributes() { DocumentType = NSDocumentType.HTML }, ref error);
            cell.PostView.DescTextView.AttributedText = attributedString;
            //cell.PostView.DescTextView.Text = item.Description.Ellipsize(100);
            cell.PostView.ThemeBar.BackgroundColor = item.ThemeColor.ToUIColor();
            cell.PostView.IconImageView.Image = UIImage.FromFile(item.IconUrl);

            // if it has a photo
            if (!string.IsNullOrEmpty(item.ImageUrl) && _loadImages)
            {
                var image = UIImage.FromFile("placeholder_news.png");
                cell.PostView.PhotoImageView.Image = image;
                cell.PostView.PhotoImageView.Hidden = false;
            }
            else
            {
                cell.PostView.PhotoImageView.Image = UIImage.FromFile("placeholder_news.png");
                cell.PostView.PhotoImageView.Hidden = true;
            }

            cell.PostView.UpdateLayout();

            return cell.PostView.Bounds.Size;
        }

        public CGSize GetSize(NSIndexPath indexPath)
        {
            if (indexPath == null)
                return new CGSize(0, 0);

            var item = GetItem(indexPath);
            return GetSize(item);
        }

        #endregion
    }
}
