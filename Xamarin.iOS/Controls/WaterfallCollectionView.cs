using System;
using System.Collections.Generic;
using System.Text;
using Foundation;
using MySIT.Mobile.Controls;
using UIKit;

namespace MySIT.Mobile.iOS.Controls
{
    [Register("WaterfallCollectionView")]
    public class WaterfallCollectionView : UICollectionView
    {
        #region Constructors
        public WaterfallCollectionView(IntPtr handle)
            : base(handle)
        {
            Delegate = new WaterfallCollectionDelegate(this);
        }

        public WaterfallCollectionView(CoreGraphics.CGRect frame, UIKit.UICollectionViewLayout layout): base(frame, layout)
        {
            this.RegisterClassForCell(typeof(PostUICollectionViewCell), "TableCell");
            Delegate = new WaterfallCollectionDelegate(this);

            this.BackgroundColor = UIColor.White;
        }
        #endregion

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            // Initialize
            DataSource = new WaterfallCollectionSource(this);
            //Delegate = new WaterfallCollectionDelegate(this);
        }

        public event EventHandler<EventArgs<int>> ItemClicked;

        public virtual void OnItemClicked(int index)
        {
            ItemClicked?.Invoke(this, new EventArgs<int>(index));
        }
    }
}
