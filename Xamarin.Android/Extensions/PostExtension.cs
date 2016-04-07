using System;
using MySIT.Mobile.Models.SocialMedias;

namespace MySIT.Mobile.Droid.Extensions
{
    public static class PostExtension
    {
        public static int GetReousrceId(this Post post)
        {
            int resourceId = 0;

            switch (post.Type)
            {
                case SocialMediaType.Facebook:
                    resourceId = Resource.Drawable.icon_facebook;
                    break;
                case SocialMediaType.Flickr:
                    resourceId = Resource.Drawable.icon_flickr;
                    break;
                case SocialMediaType.Instagram:
                    resourceId = Resource.Drawable.icon_instagram;
                    break;
                case SocialMediaType.SitNews:
                    resourceId = Resource.Drawable.icon_rss;
                    break;
                case SocialMediaType.Youtube:
                    resourceId = Resource.Drawable.icon_youtube;
                    break;
                case SocialMediaType.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return resourceId;
        }
    }
}