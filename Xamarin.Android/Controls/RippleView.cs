using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;

namespace MySIT.Mobile.Droid.Controls
{
    public class RippleView : RelativeLayout, RippleView.IOnRippleCompleteListener
    {
        private int _width;
        private int _height;
        private Handler _canvasHandler;
        private float _radiusMax = 0;
        private bool _animationRunning = false;
        private int _timer = 0;
        private int _timerEmpty = 0;
        private int _durationEmpty = -1;
        private float _x = -1;
        private float _y = -1;
        private ScaleAnimation _scaleAnimation;
        private int _rippleType;
        private Paint _paint;
        private Bitmap _originBitmap;
        private GestureDetector _gestureDetector;

        private readonly Runnable _runnable;

        public RippleView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public RippleView(Context context) : base(context)
        {
            _runnable = new Runnable(this.Invalidate);
            Init(context);
        }

        public RippleView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _runnable = new Runnable(this.Invalidate);
            Init(context, attrs);
        }

        public RippleView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            _runnable = new Runnable(this.Invalidate);
            Init(context, attrs);
        }

        public RippleView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            _runnable = new Runnable(this.Invalidate);
            Init(context, attrs);
        }

        private void Init(Context context)
        {
            if (this.IsInEditMode)
                return;

            RippleDuration = 300;
            FrameRate = 20;

            _canvasHandler = new Handler();
            _paint = new Paint
            {
                Color = new Color(RippleColor),
                Alpha = RippleAlpha,
                AntiAlias = true
            };
            _paint.SetStyle(Paint.Style.Fill);
            this.SetWillNotDraw(false);

            _gestureDetector = new GestureDetector(context, new MySimpleOnGestureListener((e) =>
            {
                AnimateRipple(e);
                SendClickEvent(true);
            }));

            this.DrawingCacheEnabled = true;
            this.Clickable = true;
        }

        private void Init(Context context, IAttributeSet attrs)
        {
            if (this.IsInEditMode)
                return;

            TypedArray typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.RippleView);
            RippleColor = typedArray.GetColor(Resource.Styleable.RippleView_rv_color, Resources.GetColor(Resource.Color.rippelColor));
            _rippleType = typedArray.GetInt(Resource.Styleable.RippleView_rv_type, 0);
            IsZooming = typedArray.GetBoolean(Resource.Styleable.RippleView_rv_zoom, false);
            IsCentered = typedArray.GetBoolean(Resource.Styleable.RippleView_rv_centered, false);
            RippleDuration = typedArray.GetInteger(Resource.Styleable.RippleView_rv_rippleDuration, RippleDuration);
            FrameRate = typedArray.GetInteger(Resource.Styleable.RippleView_rv_framerate, FrameRate);
            RippleAlpha = typedArray.GetInteger(Resource.Styleable.RippleView_rv_alpha, RippleAlpha);
            RipplePadding = typedArray.GetInteger(Resource.Styleable.RippleView_rv_ripplePadding, 0);
            ZoomScale = typedArray.GetFloat(Resource.Styleable.RippleView_rv_zoomScale, 1.03f);
            ZoomDuration = typedArray.GetInteger(Resource.Styleable.RippleView_rv_zoomDuration, 200);
            typedArray.Recycle();
            _canvasHandler = new Handler();
            _paint = new Paint
            {
                Color = new Color(RippleColor),
                Alpha = RippleAlpha,
                AntiAlias = true
            };
            _paint.SetStyle(Paint.Style.Fill);
            this.SetWillNotDraw(false);

            _gestureDetector = new GestureDetector(context, new MySimpleOnGestureListener((e) =>
            {
                AnimateRipple(e);
                SendClickEvent(true);
            }));

            this.DrawingCacheEnabled = true;
            this.Clickable = true;
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);

            if (_animationRunning)
            {
                canvas.Save();

                if (RippleDuration <= _timer*FrameRate)
                {
                    _animationRunning = false;
                    _timer = 0;
                    _durationEmpty = -1;
                    _timerEmpty = 0;
                    canvas.Restore();
                    Invalidate();

                    OnComplete(this);

                    return;
                }

                _canvasHandler.PostDelayed(_runnable, FrameRate);

                if (_timer == 0)
                    canvas.Save();

                canvas.DrawCircle(_x, _y, (_radiusMax * (((float) _timer * FrameRate / RippleDuration))), _paint);

                _paint.Color = Color.ParseColor("#ffff4444");

                if(_rippleType == 1 && _originBitmap != null && (((float)_timer * FrameRate) / RippleDuration) > 0.4f)
                {
                    if (_durationEmpty == -1)
                        _durationEmpty = RippleDuration - _timer * FrameRate;

                    _timerEmpty++;
                    Bitmap tmpBitmap = GetCircleBitmap((int)((_radiusMax) * (((float)_timerEmpty * FrameRate) / (_durationEmpty))));
                    canvas.DrawBitmap(tmpBitmap, 0, 0, _paint);
                    tmpBitmap.Recycle();
                }

                _paint.Color = new Color(RippleColor);

                if (_rippleType == 1)
                {
                    if ((((float)_timer * FrameRate) / RippleDuration) > 0.6f)
                        _paint.Alpha = (int)(RippleAlpha - ((RippleAlpha) * (((float)_timerEmpty * FrameRate) / (_durationEmpty))));
                    else
                        _paint.Alpha = RippleAlpha;
                }
                else
                    _paint.Alpha = (int)(RippleAlpha - ((RippleAlpha) * (((float)_timer * FrameRate) / RippleDuration)));

                _timer++;
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            _width = w;
            _height = h;

            _scaleAnimation = new ScaleAnimation(1.0f, ZoomScale, 1.0f, ZoomScale, w/2, h/2)
            {
                Duration = ZoomDuration,
                RepeatMode = RepeatMode.Reverse,
                RepeatCount = 1
            };
        }

        public void AnimateRipple(MotionEvent e)
        {
            CreateAnimation(e.GetX(), e.GetY());
        }

        public void AnimateRipple(float x, float y)
        {
            CreateAnimation(x, y);
        }

        private void CreateAnimation(float x, float y)
        {
            if (this.Enabled && !_animationRunning)
            {
                if (IsZooming)
                    this.StartAnimation(_scaleAnimation);

                _radiusMax = System.Math.Max(_width, _height);

                if (_rippleType != 2)
                    _radiusMax /= 2;

                _radiusMax -= RipplePadding;

                if (IsCentered || _rippleType == 1)
                {
                    this._x = MeasuredWidth / 2;
                    this._y = MeasuredHeight / 2;
                }
                else {
                    this._x = x;
                    this._y = y;
                }

                _animationRunning = true;

                if (_rippleType == 1 && _originBitmap == null)
                    _originBitmap = GetDrawingCache(true);

                Invalidate();
            }
        }


        public override bool OnTouchEvent(MotionEvent e)
        {
            if (_gestureDetector.OnTouchEvent(e)) 
            {
                AnimateRipple(e);
                SendClickEvent(false);
            }

            return base.OnTouchEvent(e);
        }

        public override bool OnInterceptTouchEvent(MotionEvent e)
        {
            this.OnTouchEvent(e);
            return base.OnInterceptTouchEvent(e);
        }

        private void SendClickEvent(bool isLongClick)
        {
            var view = this.Parent as AdapterView;

            if (view != null) {
                AdapterView adapterView = view;
                int position = adapterView.GetPositionForView(this);
                long id = adapterView.GetItemIdAtPosition(position);

                if (isLongClick)
                {
                    adapterView.OnItemLongClickListener?.OnItemLongClick(adapterView, this, position, id);
                }
                else
                {
                    adapterView.OnItemClickListener?.OnItemClick(adapterView, this, position, id);
                }
            }
        }

        private Bitmap GetCircleBitmap(int radius)
        {
            Bitmap output = Bitmap.CreateBitmap(_originBitmap.Width, _originBitmap.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(output);
            Paint paint = new Paint();
            Rect rect = new Rect((int)(_x - radius), (int)(_y - radius), (int)(_x + radius), (int)(_y + radius));

            paint.AntiAlias = true;
            canvas.DrawARGB(0, 0, 0, 0);
            canvas.DrawCircle(_x, _y, radius, paint);

            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
            canvas.DrawBitmap(_originBitmap, rect, rect, paint);

            return output;
        }

        public int RippleColor { get; set; }

        public RippleType RippleType => (RippleType) _rippleType;

        public bool IsCentered { get; set; }

        public int RipplePadding { get; set; }

        public bool IsZooming { get; set; }

        public float ZoomScale { get; set; }

        public int ZoomDuration { get; set; }

        public int RippleDuration { get; set; } = 400;

        public int FrameRate { get; set; } = 10;

        public int RippleAlpha { get; set; } = 90;


        private class MySimpleOnGestureListener : GestureDetector.SimpleOnGestureListener
        {
            private readonly Action<MotionEvent> _action;

            public MySimpleOnGestureListener(Action<MotionEvent> action)
            {
                _action = action;
            }

            public override void OnLongPress(MotionEvent e)
            {
                base.OnLongPress(e);

                _action?.Invoke(e);
            }

            public override bool OnSingleTapConfirmed(MotionEvent e)
            {
                return true;
            }

            public override bool OnSingleTapUp(MotionEvent e)
            {
                return true;
            }
        }

        public interface IOnRippleCompleteListener
        {
            void OnComplete(RippleView rippleView);
        }

        public void OnComplete(RippleView rippleView)
        {

        }
    }

    public enum RippleType
    {
        Simple = 0,
        Double = 1,
        Rectangle = 2
    }
}