using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ExtensionsLibrary;
using MySIT.Mobile.Droid.Controls;
using MySIT.Mobile.Droid.Extensions;
using MySIT.Mobile.Droid.Renderers;
using MySIT.Mobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ExtendedListView = MySIT.Mobile.Controls.ExtendedListView;
using ListView = Xamarin.Forms.ListView;

[assembly: ExportRenderer(typeof (ExtendedListView), typeof (RecyclerViewRenderer))]
namespace MySIT.Mobile.Droid.Renderers
{
    public class RecyclerViewRenderer : ViewRenderer<ExtendedListView, RecyclerView>, SwipeRefreshLayout.IOnRefreshListener
    {
        #region Fields

        private RecyclerAdapter _adapter;
        private int _columns = 1;
        private StaggeredGridLayoutManager _manager;
        private bool _needsReload = false;
        private RecyclerView _recyclerView;
        private SwipeRefreshLayout _refresh;

        #endregion

        #region Methods

        public void OnItemClick(object sender, int position)
        {
            RiseItemOnClick(position);
        }

        public void OnRefresh()
        {
            this.Element.BeginRefresh();
        }

        public void RiseItemOnClick(int position)
        {
            var methodInfo = typeof (Xamarin.Forms.ListView).GetMethod("NotifyRowTapped", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] {typeof (int), typeof (Cell)}, null);

            methodInfo?.Invoke(this.Element, new object[] {position, null});
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            // fix mSpan in StaggeredGridLayoutManager.LayoutParams is null
            //if (_needsReload && _adapter != null && _adapter.ItemCount > 0)
            _adapter.NotifyDataSetChanged();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            //_needsReload = true;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ExtendedListView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var list = e.OldElement.ItemsSource as INotifyCollectionChanged;

                if (list != null)
                {
                    list.CollectionChanged -= OnListOnCollectionChanged;
                }
            }

            if (e.NewElement != null)
            {
                _columns = this.Context.IsTablet() ? 2 : 1;
                _manager = new StaggeredGridLayoutManager(_columns, StaggeredGridLayoutManager.Vertical);
                _recyclerView = new RecyclerView(this.Context);
                _recyclerView.SetLayoutManager(_manager);
                _adapter = new RecyclerAdapter(_recyclerView, e.NewElement, _columns);
                _recyclerView.SetAdapter(_adapter);
                _recyclerView.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                _adapter.ItemClick += OnItemClick;

                //Setup SwipeRefreshLayout
                _refresh = new SwipeRefreshLayout(this.Context);
                _refresh.SetOnRefreshListener(this);
                _refresh.AddView(_recyclerView, 0);
                _refresh.SetColorSchemeResources(Resource.Color.dark_blue,
                    Resource.Color.orange,
                    Resource.Color.gray,
                    Resource.Color.green);

                this.SetNativeControl(_recyclerView, this._refresh);

                //this.Control.SetPadding(MetricConverter.ConvertFromDpToPixel(e.NewElement.Padding.Left)
                //    , MetricConverter.ConvertFromDpToPixel(e.NewElement.Padding.Top)
                //    , MetricConverter.ConvertFromDpToPixel(e.NewElement.Padding.Right)
                //    , MetricConverter.ConvertFromDpToPixel(e.NewElement.Padding.Bottom));

                var list = e.NewElement.ItemsSource as INotifyCollectionChanged;

                if (list != null)
                {
                    list.CollectionChanged += OnListOnCollectionChanged ;
                }
            }

            UpdateIsSwipeToRefreshEnabled();
            UpdateIsRefreshing();
        }

        private void OnListOnCollectionChanged(object o, NotifyCollectionChangedEventArgs args)
        {
            this._adapter.NotifyDataSetChanged();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ListView.ItemsSourceProperty.PropertyName)
            {
                var list = this.Element.ItemsSource as INotifyCollectionChanged;

                if (list != null)
                {
                    list.CollectionChanged += OnListOnCollectionChanged;
                }
            }
            else if (e.PropertyName == ExtendedListView.IsPullToRefreshEnabledProperty.PropertyName)
            {
                UpdateIsSwipeToRefreshEnabled();
            }
            else if (e.PropertyName == ExtendedListView.IsRefreshingProperty.PropertyName)
            {
                UpdateIsRefreshing();
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (this.Control != null)
            {
                this.Control.Measure(widthMeasureSpec, heightMeasureSpec);
                this.SetMeasuredDimension(this.Control.MeasuredWidth, this.Control.MeasuredHeight);
            }
            else
            {
                base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            }
        }

        private void UpdateIsRefreshing()
        {
            this._refresh.Refreshing = this.Element.IsRefreshing;
        }

        private void UpdateIsSwipeToRefreshEnabled()
        {
            this._refresh.Enabled = true; //= this.Element.IsPullToRefreshEnabled;
        }

        #endregion

        #region Nested Types

        class RecyclerAdapter : RecyclerView.Adapter
        {
            #region Fields

            private readonly int _columnCount;
            private readonly RecyclerView _control;

            private readonly ListView _listView;

            #endregion

            #region Constructors

            public RecyclerAdapter(RecyclerView recyclerView, ListView listView, int columnCount)
            {
                _listView = listView;
                _control = recyclerView;
                _columnCount = columnCount;
            }

            #endregion

            #region Events

            public event EventHandler<int> ItemClick;

            #endregion

            #region Properties

            public override int ItemCount => this._listView.ItemsSource?.Cast<object>().ToList().Count() ?? 0;

            #endregion

            #region Indexers

            private object this[int position]
            {
                get
                {
                    var items = _listView.ItemsSource.Cast<object>().ToList();
                    if (items.IsNullOrEmpty())
                    {
                        return null;
                    }

                    return position > items.Count - 1 ? null : items[position];
                }
            }

            #endregion

            #region Methods

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var item = this[position];
                var viewHolder = (MyViewHolder) holder;

                var renderer = (IVisualElementRenderer) ((ViewGroup) viewHolder.ItemView).GetChildAt(0);
                renderer.Element.BindingContext = item;

                double width = viewHolder.ItemView.Context.FromPixels((double) _control.Width/_columnCount);
                double height = viewHolder.ItemView.Context.FromPixels((double) _control.Height/_columnCount);

                var size = renderer.Element.GetSizeRequest(width, height);
                renderer.Element.Layout(new Rectangle(0, 0, size.Request.Width, size.Request.Height));

                var parms = new StaggeredGridLayoutManager.LayoutParams((int) viewHolder.ItemView.Context.ToPixels(size.Request.Width), (int) viewHolder.ItemView.Context.ToPixels(size.Request.Height));

                viewHolder.ItemView.LayoutParameters = parms;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var view = ((ViewCell) _listView.ItemTemplate.CreateContent()).View;
                var parentView = new LinearLayout(parent.Context);
                var renderer = Platform.GetRenderer(view) ?? Platform.CreateRenderer(view);

                view.Parent = _listView;
                renderer.SetElement(view);
                parentView.AddView(renderer.ViewGroup, 0);

                return new MyViewHolder(parentView, OnClick);
            }

            void OnClick(int position)
            {
                ItemClick?.Invoke(this, position);
            }

            #endregion
        }

        #endregion
    }
}