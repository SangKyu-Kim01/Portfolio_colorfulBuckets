using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PaintShopManagement.Converters
{

    public class BlobToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is byte[] blobData && blobData.Length > 0)
                {
                    BitmapImage image = new BitmapImage();
                    using (MemoryStream stream = new MemoryStream(blobData))
                    {
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();
                    }

                    // Freeze the BitmapImage to make it cross-thread accessible
                    image.Freeze();

                    return image;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BlobToImageConverter: {ex.Message}");
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}
