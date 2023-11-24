using ImageMagick;

namespace QuickServerDemo
{
    internal class ImageValidation
    {

        internal static bool IsImageValid(string filename, Stream imData, out byte[] converted, out string errDetail)
        {
            try
            {
               
                
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png",".bmp",".webp" };


                if (!Array.Exists(allowedExtensions, ext => ext.Equals(Path.GetExtension(filename))))
                {
                    converted = new byte[0];
                    errDetail = "Unsuported extension";
                    return false;
                }
                
                using (MagickImage image = new MagickImage(imData))
                {

                    byte[] pixelBytes = image.ToByteArray(MagickFormat.Jpeg);

                    converted = pixelBytes;
                    errDetail = "";
                    return true;
                }
            }
            catch (Exception ex)
            {
                // An exception occurred, indicating that the file is not a valid image
                converted= new byte[0];
                errDetail = ex.Message;
                return false;
            }
        }

        
    }
}
