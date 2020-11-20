using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace bake_json_into_badge
{
    public class Function
    {
        /// <summary>
        /// The main entry point for the custom runtime.
        /// </summary>
        /// <param name="args"></param>
        private static async Task Main(string[] args)
        {
            // Func<string, ILambdaContext, string> func = FunctionHandler;
            // using (var handlerWrapper = HandlerWrapper.GetHandlerWrapper(func, new LambdaJsonSerializer()))
            // using (var bootstrap = new LambdaBootstrap(handlerWrapper))
            // {
            //     await bootstrap.RunAsync();
            // }

            //byte[] imgBadge = Convert.FromBase64String("data:image/png;base64,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");
            byte[] imgBadge = Convert.FromBase64String("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");
            string json = "{ 'test':'json' }";

            Bitmap bmp = new Bitmap(new MemoryStream(imgBadge));
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            WriteableBitmap img = new WriteableBitmap(bitmapSource);

            // ベーキングの箱「img」に必要なJSON Metadataを入れて、ベーキングをし、PNGとしてしあげ、メモリ上のPNG情報をbadge_byteに入れる。
            byte[] badge_byte;
            using (MemoryStream stream = new MemoryStream())
            {
                var enc = new PngBitmapEncoder();
                var pngMetadata = new BitmapMetadata("png");
                pngMetadata.SetQuery("/iTXt/Keyword", "openbadges".ToCharArray());
                pngMetadata.SetQuery("/iTXt/TextEntry", json);

                var frame = BitmapFrame.Create(img, null, pngMetadata, null);
                enc.Frames.Add(frame);
                enc.Save(stream);

                badge_byte = stream.ToArray();
            }

            // DBで利用する形のbase64をbadge_byteから変換する
            string badge_image_base64 = Convert.ToBase64String(badge_byte);
            Console.Write(badge_image_base64);
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        ///
        /// To use this handler to respond to an AWS event, reference the appropriate package from 
        /// https://github.com/aws/aws-lambda-dotnet#events
        /// and change the string input parameter to the desired event type.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string FunctionHandler(string input, ILambdaContext context)
        {
            return input?.ToUpper();
        }
    }
}
