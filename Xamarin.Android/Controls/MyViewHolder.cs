using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace MySIT.Mobile.Droid.Controls
{
    public class MyViewHolder : RecyclerView.ViewHolder
    {
        public MyViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public MyViewHolder(View itemView, Action<int> listener) : base(itemView)
        {

            this.ItemView.Click += (sender, e) => listener(base.AdapterPosition);
        }
    }
}