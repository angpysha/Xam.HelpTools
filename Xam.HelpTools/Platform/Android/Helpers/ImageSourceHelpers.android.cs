using Android.Content;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Xam.HelpTools.Platform.Android.Helpers
{
    public static class ImageSourceHelper
    {
        private static FileImageSourceHandler fileImageSourceHandler;
        private static ImageLoaderSourceHandler imageLoaderSourceHandler;
        private static StreamImagesourceHandler streamImagesourceHandler;
        static ImageSourceHelper()
        {
            fileImageSourceHandler = new FileImageSourceHandler();
            imageLoaderSourceHandler = new ImageLoaderSourceHandler();
            streamImagesourceHandler = new StreamImagesourceHandler();
        }

        private static IImageSourceHandler GetHandler(ImageSource source)
        {
            IImageSourceHandler returnValue = null;
            if (source is UriImageSource)
            {
                returnValue = new ImageLoaderSourceHandler();
            }
            else if (source is FileImageSource)
            {
                returnValue = new FileImageSourceHandler();
            }
            else if (source is StreamImageSource)
            {
                returnValue = new StreamImagesourceHandler();
            }
            return returnValue;
        }

        public static async Task<Bitmap> ToBitmap(this ImageSource imageSource, Context context)
        {
            if (imageSource == null)
                return null;
            var handler = GetHandler(imageSource);

            var image = await handler.LoadImageAsync(imageSource, context);

            return image;
        }
    }
}
