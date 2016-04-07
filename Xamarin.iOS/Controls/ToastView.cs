using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MySIT.Mobile.iOS.Controls
{
    public enum ToastGravity
    {
        Top = 0,
        Bottom = 1,
        Center = 2
    }

    public class ToastView : NSObject
    {
        #region Fields

        private readonly string _text = null;
        private readonly ToastSettings _theSettings = new ToastSettings();

        private int _offsetLeft = 0;
        private int _offsetTop = 0;
        private UIButton _view;

        #endregion

        #region Constructors

        public ToastView(string text, int durationMilliseonds)
        {
            _text = text;
            _theSettings.Duration = durationMilliseonds;
        }

        #endregion

        #region Methods

        public ToastView SetGravity(ToastGravity gravity, int offSetLeft, int offSetTop)
        {
            _theSettings.Gravity = gravity;
            _offsetLeft = offSetLeft;
            _offsetTop = offSetTop;
            return this;
        }

        public ToastView SetPosition(PointF position)
        {
            _theSettings.Position = position;
            return this;
        }

        public void Show()
        {
            UIButton v = UIButton.FromType(UIButtonType.Custom);
            _view = v;

            UIFont font = UIFont.SystemFontOfSize(16);
            CGSize textSize = _text.StringSize(font, new CGSize(280, 60), UILineBreakMode.WordWrap);

            var label = new UILabel(new RectangleF(0, 0, (float) textSize.Width + 5, (float) textSize.Height + 5))
            {
                BackgroundColor = UIColor.Clear,
                TextColor = UIColor.White,
                Font = font,
                Text = _text,
                Lines = 0,
                ShadowColor = UIColor.DarkGray,
                ShadowOffset = new SizeF(1, 1)
            };


            v.Frame = new RectangleF(0, 0, (float) textSize.Width + 10, (float) textSize.Height + 10);
            label.Center = new PointF((float) v.Frame.Size.Width/2, (float) v.Frame.Height/2);
            v.AddSubview(label);

            v.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0.7f);
            v.Layer.CornerRadius = 5;

            UIWindow window = UIApplication.SharedApplication.Windows[0];

            var point = new PointF((float) window.Frame.Size.Width/2, (float) window.Frame.Size.Height/2);

            switch (_theSettings.Gravity)
            {
                case ToastGravity.Top:
                    point = new PointF((float) window.Frame.Size.Width/2, 45);
                    break;
                case ToastGravity.Bottom:
                    point = new PointF((float) window.Frame.Size.Width/2, (float) window.Frame.Size.Height - 45);
                    break;
                case ToastGravity.Center:
                    point = new PointF((float) window.Frame.Size.Width/2, (float) window.Frame.Size.Height/2);
                    break;
                default:
                    point = _theSettings.Position;
                    break;
            }

            point = new PointF(point.X + _offsetLeft, point.Y + _offsetTop);
            v.Center = point;
            window.AddSubview(v);
            v.AllTouchEvents += delegate { HideToast(null); };

            NSTimer.CreateScheduledTimer(_theSettings.DurationSeconds, HideToast);
        }


        private void HideToast(NSTimer timer)
        {
            UIView.BeginAnimations("");
            _view.Alpha = 0;
            UIView.CommitAnimations();
        }

        private void RemoveToast()
        {
            _view.RemoveFromSuperview();
        }

        #endregion
    }

    public class ToastSettings
    {
        #region Constructors

        public ToastSettings()
        {
            this.Duration = 500;
            this.Gravity = ToastGravity.Center;
        }

        #endregion

        #region Properties

        public int Duration { get; set; }

        public double DurationSeconds { get { return (double) Duration/1000; } }

        public ToastGravity Gravity { get; set; }

        public PointF Position { get; set; }

        #endregion
    }
}