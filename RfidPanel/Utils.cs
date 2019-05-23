using System.IO;
using System.Windows.Media.Imaging;

namespace RfidPanel
{
    public static class Utils
    {
        public static BitmapImage LoadImageFromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;

            var image = new BitmapImage();

            // load bitmab image from bytes (bytes storing in database)
            using (var stream = new MemoryStream(bytes))
            {
                stream.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = stream;
                image.EndInit();
            }

            image.Freeze();
            return image;
        }
    }
}
