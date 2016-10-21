#region using
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Routing;
#endregion
namespace HttpModuleAndHandlerDemo.Framework
{
    /// <summary>
    /// http://www.hanselman.com/blog/BackToBasicsDynamicImageGenerationASPNETControllersRoutingIHttpHandlersAndRunAllManagedModulesForAllRequests.aspx
    /// </summary>
    public class WatermarkImageHandler : IRouteHandler, IHttpHandler
    {
        #region RouteHandler
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }
        #endregion

        #region HttpHandler
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public string GetContentType(string path)
        {
            switch (Path.GetExtension(path))
            {
                case ".bmp": return "Image/bmp";
                case ".gif": return "Image/gif";
                case ".jpg": return "Image/jpeg";
                case ".png": return "Image/png";
                default: break;
            }
            return string.Empty;
        }

        public ImageFormat GetImageFormat(string path)
        {
            switch (Path.GetExtension(path).ToLower())
            {
                case ".bmp": return ImageFormat.Bmp;
                case ".gif": return ImageFormat.Gif;
                case ".jpg": return ImageFormat.Jpeg;
                case ".png": return ImageFormat.Png;
                default: return null;
            }
        }

        protected byte[] WatermarkImage(HttpContext context)
        {

            byte[] imageBytes = null;
            if (File.Exists(context.Request.PhysicalPath))
            {
                // Normally you'd put this in a config file somewhere.
                string watermark = "TOTO - © EXAMPLE Company 2016";

                Image image = Image.FromFile(context.Request.PhysicalPath);

                Graphics graphic;
                if (image.PixelFormat != PixelFormat.Indexed &&
                    image.PixelFormat != PixelFormat.Format8bppIndexed &&
                    image.PixelFormat != PixelFormat.Format4bppIndexed &&
                    image.PixelFormat != PixelFormat.Format1bppIndexed)
                {
                    // Graphic is not a Indexed (GIF) image
                    graphic = Graphics.FromImage(image);
                }
                else
                {
                    /* Cannot create a graphics object from an indexed (GIF) image. 
                     * So we're going to copy the image into a new bitmap so 
                     * we can work with it. */
                    Bitmap indexedImage = new Bitmap(image);
                    graphic = Graphics.FromImage(indexedImage);

                    // Draw the contents of the original bitmap onto the new bitmap. 
                    graphic.DrawImage(image, 0, 0, image.Width, image.Height);
                    image = indexedImage;
                }
                graphic.SmoothingMode = SmoothingMode.AntiAlias & SmoothingMode.HighQuality;

                Font myFont = new Font("Arial", 15);
                SolidBrush brush = new SolidBrush(Color.FromArgb(80, Color.White));

                /* This gets the size of the graphic so we can determine 
                 * the loop counts and placement of the watermarked text. */
                SizeF textSize = graphic.MeasureString(watermark, myFont);

                // Write the text across the image. 
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        PointF pointF = new PointF(x, y);
                        graphic.DrawString(watermark, myFont, brush, pointF);
                        x += Convert.ToInt32(textSize.Width);
                    }
                    y += Convert.ToInt32(textSize.Height);
                }


                using (MemoryStream memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, GetImageFormat(context.Request.PhysicalPath));
                    imageBytes = memoryStream.ToArray();
                }

            }
            return imageBytes;
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = GetContentType(context.Request.PhysicalPath);
            byte[] imageBytes = WatermarkImage(context);
            if (imageBytes != null)
            {
                context.Response.OutputStream.Write(imageBytes, 0, imageBytes.Length);
            }
            else
            {
                // No bytes = no image which equals NO FILE. 🙂  
                // Therefore send a 404 - not found response. 
                context.Response.StatusCode = 404;
            }
            context.Response.End();
        }
        #endregion
    }
}