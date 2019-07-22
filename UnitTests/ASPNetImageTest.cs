using System;
using System.Diagnostics;
using System.Globalization;
using ASPNetImage;
using NUnit.Framework;
using System.Security.Cryptography;
using System.Drawing;

namespace UnitTests
{
    [TestFixture]
    public class ASPNetImageTest
    {

        // ====================================================================
        #region Public Property Tests

        [Test]
        public void TestPropertyAutoClear()
        {
            var thisImage = new NetImage();
            var initValue = thisImage.AutoClear;
            thisImage.AutoClear = !thisImage.AutoClear;
            var modifiedValue = thisImage.AutoClear;

            Assert.AreNotEqual(initValue, modifiedValue);
        }

        [Test]
        public void TestPropertyAntiAliasText()
        {
            var thisImage = new NetImage();
            var initValue = thisImage.AntiAliasText;
            thisImage.AntiAliasText = !thisImage.AntiAliasText;
            var modifiedValue = thisImage.AntiAliasText;

            Assert.AreNotEqual(initValue, modifiedValue);
        }

        [Test]
        public void TestPropertyBackgroundColor()
        {
            var thisImage = new NetImage();
            var testValue = 0xFFFF00FF; // Solid Cyan
            thisImage.BackgroundColor = 0x7FFF00FF; // Try setting it to 50% transparent cyan

            Assert.AreEqual(testValue & 0x00FFFFFF, thisImage.BackgroundColor);
        }

        [Test]
        public void TestPropertyBold()
        {
            var thisImage = new NetImage();
            var initValue = thisImage.AntiAliasText;
            thisImage.Bold = !thisImage.Bold;
            var modifiedValue = thisImage.Bold;

            Assert.AreNotEqual(initValue, modifiedValue);
        }

        [Test]
        public void TestPropertyContrainResize()
        {
            var thisImage = new NetImage();
            var initValue = thisImage.ConstrainResize;
            thisImage.ConstrainResize = !thisImage.ConstrainResize;
            var modifiedValue = thisImage.ConstrainResize;

            Assert.AreNotEqual(initValue, modifiedValue);
        }

        [Test]
        public void TestPropertyExpires()
        {
            var thisImage = new NetImage();

            Assert.That(Convert.ToDateTime(thisImage.Expires) > DateTime.Now);
        }

        [Test]
        public void TestPropertyFilename()
        {
            var thisImage = new NetImage {Filename = "c:\test.jpg"};

            Assert.AreEqual("c:\test.jpg", thisImage.Filename);
        }

        [Test]
        public void TestPropertyFontColor()
        {
            var thisImage = new NetImage();
            const int testValue = -8421505; //System.Drawing.Color.FromArgb(255, 127, 127, 127).ToArgb();
            thisImage.FontColor = NetImage.DotNETARGBToVBScriptRGB(System.Drawing.Color.FromArgb(255, 127, 127, 127).ToArgb());

            Assert.AreEqual(testValue, NetImage.VBScriptRGBToDotNETARGB(thisImage.FontColor));
        }

        [Test]
        public void TestPropertyFontName()
        {
            var thisImage = new NetImage { FontName = "Arial Test Font Name" };

            Assert.AreEqual("Arial Test Font Name", thisImage.FontName);
        }

        [Test]
        public void TestPropertyFontSize()
        {
            var thisImage = new NetImage { FontSize = 255 };

            Assert.AreEqual(255, thisImage.FontSize);
        }

        [Test]
        public void TestPropertyRawNetImage()
        {
            var thisImage = new NetImage();
            thisImage.BackgroundColor = System.Drawing.Color.FromArgb(255, 0, 0, 0).ToArgb();
            thisImage.MaxX = 1024;
            thisImage.MaxY = 768;
            thisImage.ClearImage();

            if (thisImage.RawNetImage == null)
                Assert.Fail();

            var imageSize = thisImage.RawNetImage.Size;
            var pixelColor = ((Bitmap) thisImage.RawNetImage).GetPixel(0, 0);

            Assert.AreEqual(1024, imageSize.Width);
            Assert.AreEqual(768, imageSize.Height);
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 0, 0, 0), pixelColor);
        }

        [Test]
        public void TestPropertyImageFormat()
        {
            var thisImage = new NetImage();

            thisImage.ImageFormat = NetImage.ImageFormats.BMP;
            Assert.AreEqual(NetImage.ImageFormats.BMP, thisImage.ImageFormat);

            thisImage.ImageFormat = NetImage.ImageFormats.GIF;
            Assert.AreEqual(NetImage.ImageFormats.GIF, thisImage.ImageFormat);

            thisImage.ImageFormat = NetImage.ImageFormats.JPEG;
            Assert.AreEqual(NetImage.ImageFormats.JPEG, thisImage.ImageFormat);

            thisImage.ImageFormat = NetImage.ImageFormats.PCX;
            Assert.AreEqual(NetImage.ImageFormats.PCX, thisImage.ImageFormat);

            thisImage.ImageFormat = NetImage.ImageFormats.PNG;
            Assert.AreEqual(NetImage.ImageFormats.PNG, thisImage.ImageFormat);

            thisImage.ImageFormat = NetImage.ImageFormats.TGA;
            Assert.AreEqual(NetImage.ImageFormats.TGA, thisImage.ImageFormat);

            thisImage.ImageFormat = NetImage.ImageFormats.WBMP;
            Assert.AreEqual(NetImage.ImageFormats.WBMP, thisImage.ImageFormat);
        }

        [Test]
        public void TestPropertyJPGQuality()
        {
            var thisImage = new NetImage();

            thisImage.JPEGQuality = 100;
            Assert.AreEqual(100, thisImage.JPEGQuality);

            thisImage.JPEGQuality = 200;
            Assert.AreEqual(100, thisImage.JPEGQuality);

            thisImage.JPEGQuality = 0;
            Assert.AreEqual(0, thisImage.JPEGQuality);

            thisImage.JPEGQuality = -52;
            Assert.AreEqual(0, thisImage.JPEGQuality);
        }

        [Test]
        public void TestPropertyMaxX()
        {
            var thisImage = new NetImage();
            Assert.AreEqual(0, thisImage.MaxX);

            thisImage.MaxX = 1024;
            // maxY value not set, so image is still null
            Assert.AreEqual(0, thisImage.MaxX);

            thisImage.MaxY = 768;
            Assert.AreEqual(1024, thisImage.MaxX);
        }

        [Test]
        public void TestPropertyMaxY()
        {
            var thisImage = new NetImage();
            Assert.AreEqual(0, thisImage.MaxY);

            thisImage.MaxY = 768;
            // maxX value not set, so image is still null
            Assert.AreEqual(0, thisImage.MaxY);

            thisImage.MaxX = 1024;
            Assert.AreEqual(768, thisImage.MaxY);
        }

        [Test]
        public void TestPropertyProgressiveJPEGEncoding()
        {
            var thisImage = new NetImage();
            var initValue = thisImage.ProgressiveJPEGEncoding;
            thisImage.ProgressiveJPEGEncoding = !thisImage.ProgressiveJPEGEncoding;
            var modifiedValue = thisImage.ProgressiveJPEGEncoding;

            Assert.AreNotEqual(initValue, modifiedValue);
        }

        [Test]
        public void TestPropertyImage()
        {
            // TODO: Test raw image data
            Assert.Pass();
        }

        [Test]
        public void TestPropertyPenColor()
        {
            var penColor = 0xFFFE10;
            var thisImage = new NetImage { PenColor = penColor };

            Assert.AreEqual(penColor, thisImage.PenColor);
        }

        [Test]
        public void TestPropertyPenStyle()
        {
            var thisImage = new NetImage();

            thisImage.PenStyle = 0;
            Assert.AreEqual(0, thisImage.PenStyle);

            thisImage.PenStyle = 1;
            Assert.AreEqual(1, thisImage.PenStyle);

            thisImage.PenStyle = 2;
            Assert.AreEqual(2, thisImage.PenStyle);

            thisImage.PenStyle = 3;
            Assert.AreEqual(3, thisImage.PenStyle);

            thisImage.PenStyle = 4;
            Assert.AreEqual(4, thisImage.PenStyle);

            thisImage.PenStyle = -1;
            Assert.AreEqual(0, thisImage.PenStyle);

            thisImage.PenStyle = 66;
            Assert.AreEqual(0, thisImage.PenStyle);
        }

        [Test]
        public void TestPropertyPenWidth()
        {
            var thisImage = new NetImage();

            thisImage.PenWidth = 0;
            Assert.AreEqual(1, thisImage.PenWidth);

            thisImage.PenWidth = 10;
            Assert.AreEqual(10, thisImage.PenWidth);

            thisImage.PenWidth = -50;
            Assert.AreEqual(1, thisImage.PenWidth);

            thisImage.PenWidth = 200;
            Assert.AreEqual(200, thisImage.PenWidth);
        }

        [Test]
        public void TestPropertyRegisteredTo()
        {
            var thisImage = new NetImage
            {
                RegisteredTo = "Registered To Test"
            };

            Assert.AreEqual("Registered To Test", thisImage.RegisteredTo);
        }

        [Test]
        public void TestPropertyTextAngle()
        {
            var thisImage = new NetImage();

            thisImage.TextAngle = 0;
            Assert.AreEqual(0f, thisImage.TextAngle);

            thisImage.TextAngle = 360;
            Assert.AreEqual(360f, thisImage.TextAngle);

            thisImage.TextAngle = 500;
            Assert.AreEqual(360f, thisImage.TextAngle);
        }

        [Test]
        public void TestPropertyX()
        {
            var thisImage = new NetImage();

            Assert.AreEqual(0, thisImage.X);

            thisImage.X = 50;
            Assert.AreEqual(0, thisImage.X); // Should be zero still because image dimensions haven't been defined

            thisImage.MaxX = 1024;
            thisImage.MaxY = 768;

            thisImage.X = 50;
            Assert.AreEqual(50, thisImage.X);

            thisImage.X = -10;
            Assert.AreEqual(0, thisImage.X);
        }

        [Test]
        public void TestPropertyY()
        {
            var thisImage = new NetImage();

            Assert.AreEqual(0, thisImage.Y);

            thisImage.Y = 50;
            Assert.AreEqual(0, thisImage.Y); // Should be zero still because image dimensions haven't been defined

            thisImage.MaxX = 1024;
            thisImage.MaxY = 768;

            thisImage.Y = 50;
            Assert.AreEqual(50, thisImage.Y);

            thisImage.Y = -10;
            Assert.AreEqual(0, thisImage.Y);
        }

        [Test]
        public void TestPropertyDPI()
        {
            var thisImage = new NetImage();

            Assert.AreEqual(96, thisImage.DPI); // New images default to 96 dpi

            thisImage.DPI = 72;
            Assert.AreEqual(72, thisImage.DPI);
        }

        #endregion


        // ====================================================================
        #region Method Tests

        [Test]
        public void TestAddImage()
        {
            Trace.WriteLine("TestAddImage not implemented");
        }

        [Test]
        public void TestBrightenImage()
        {
            const string outputFilePath = "../../Output/Brightness-Brighten_image.jpg";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768-black.png");
            thisImage.Filename = outputFilePath;
            thisImage.BrightenImage(50);
            thisImage.SaveImage();

            // TODO : Check RGB values
            Assert.Pass("BrightenImage passed but does not verify RGB Values");
        }

        /// <summary>
        /// Tests the ClearImage method by taking an all white image, clearing it to all black, 
        /// and then compare it to a known all-black image using a SHA hash comparison
        /// </summary>
        [Test]
        public void TestClearImage()
        {
            const string outputFilePath = "../../Output/ClearImage.png";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768-white.png");
            thisImage.ImageFormat = NetImage.ImageFormats.PNG;
            thisImage.Filename = outputFilePath;
            thisImage.BackgroundColor = Color.FromArgb(0, 0, 0).ToArgb();
            thisImage.ClearImage();
            thisImage.AutoClear = false;
            thisImage.SaveImage();

            var clearedImage = new NetImage();
            clearedImage.LoadImage("../../Resources/1024x768-black.png");

            var modifiedImage = new Bitmap(thisImage.RawNetImage);
            var comparisonImage = new Bitmap(clearedImage.RawNetImage);

            var thisConverter = new ImageConverter();
            var modifiedImageBytes = new byte[0];
            var comparisonImageBytes = new byte[0];
            modifiedImageBytes = (byte[])thisConverter.ConvertTo(modifiedImage, modifiedImageBytes.GetType());
            comparisonImageBytes = (byte[])thisConverter.ConvertTo(comparisonImage, comparisonImageBytes.GetType());

            var hasher = new SHA256Managed();
            byte[] modifiedImageHash = hasher.ComputeHash(modifiedImageBytes);
            byte[] comparisonImageHash = hasher.ComputeHash(comparisonImageBytes);

            bool isMatch = true;
            for (int i = 0; i < modifiedImageHash.Length && i < comparisonImageHash.Length && isMatch; i++)
            {
                if (modifiedImageHash[i] != comparisonImageHash[i])
                    isMatch = false;
            }

            Assert.IsTrue(isMatch);
        }

        [Test]
        public void TestAutoClear()
        {
            const string outputFilePath = "../../Output/AutoClear.jpg";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.AutoClear = true;
            thisImage.Filename = outputFilePath;
            thisImage.SaveImage();

            try
            {
                // On SaveImage() call, the image data should be null and
                // generate an exception.  If it doesn't, we know the 
                // data was not cleared.
                thisImage.RotateImage(90);
                Assert.Fail("Image data not cleared");
            }
            catch (Exception)
            {
                Assert.Pass();
            }
        }


        [Test]
        public void TestContrast()
        {
            Trace.WriteLine("TestContrast not implemented");
        }

        [Test]
        public void TestDarkenImage()
        {
            const string outputFilePath = "../../Output/Brightness-Darken_image.jpg";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768-white.png");
            thisImage.Filename = outputFilePath;
            thisImage.DarkenImage(50);
            thisImage.SaveImage();

            // TODO : Check RGB values
            Assert.Pass("DarkenImage passed but does not verify RGB Values");
        }

        [Test]
        public void TestCreateGrayScale()
        {
            Trace.WriteLine("TestCreateGrayScale not implemented");
        }

        [Test]
        public void TestCreateNegative()
        {
            Trace.WriteLine("TestCreateGrayScale not implemented");
        }

        [Test]
        public void TestCropImage()
        {
            const string outputFilePath = "../../Output/CropImage.jpg";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.Filename = outputFilePath;
            thisImage.CropImage(10, 50, 800, 600);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.AreEqual(800, width);
            Assert.AreEqual(600, height);
        }

        [Test]
        public void TestEmboss()
        {
            Trace.WriteLine("TestEmboss not implemented");
        }

        [Test]
        public void TestFillRect()
        {
            const string outputFilePath = "../../Output/FillRect.png";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768-white.png");
            thisImage.ImageFormat = NetImage.ImageFormats.PNG;
            thisImage.Filename = outputFilePath;
            thisImage.BackgroundColor = NetImage.DotNETARGBToVBScriptRGB(Color.Red.ToArgb());
            thisImage.FillRect(10, 10, 20, 20);
            thisImage.AutoClear = false; // Don't clear on save so we can still access raw image
            thisImage.SaveImage();

            var color1 = ((Bitmap)thisImage.RawNetImage).GetPixel(9, 9).ToArgb();
            var color2 = ((Bitmap)thisImage.RawNetImage).GetPixel(10, 10).ToArgb();
            var color3 = ((Bitmap)thisImage.RawNetImage).GetPixel(20, 20).ToArgb();
            var color4 = ((Bitmap)thisImage.RawNetImage).GetPixel(21, 21).ToArgb();

            Assert.AreEqual(Color.White.ToArgb(), color1);
            Assert.AreEqual(Color.Red.ToArgb(), color2);
            Assert.AreEqual(Color.Red.ToArgb(), color3);
            Assert.AreEqual(Color.White.ToArgb(), color4);
        }

        [Test]
        public void TestFlipHorizontal()
        {
            const string outputFilePath = "../../Output/Flip-Horizontal.jpg";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.Filename = outputFilePath;
            thisImage.FlipImage(NetImage.FlipDirections.Horizontal);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.AreEqual(1024, width);
            Assert.AreEqual(768, height);
        }

        [Test]
        public void TestFlipVertical()
        {
            const string outputFilePath = "../../Output/Flip-Vertical.jpg";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.Filename = outputFilePath;
            thisImage.FlipImage(NetImage.FlipDirections.Vertical);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.AreEqual(1024, width);
            Assert.AreEqual(768, height);
        }

        [Test]
        public void TestGetImageFileSize()
        {
            Trace.WriteLine("TestGetImageFileSize not implemented");
        }

        [Test]
        public void TestGetImageSize()
        {
            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");

            int width, height;
            thisImage.GetImageSize(out width, out height);

            Assert.AreEqual(1024, width);
            Assert.AreEqual(768, height);
        }

        [Test]
        public void TestGetPixel()
        {
            var thisImage = new NetImage();
            thisImage.ImageFormat = NetImage.ImageFormats.PNG;

            // No image data yet, value returned is zero
            var pixelColor = thisImage.GetPixel(640, 480);
            Assert.AreEqual(0, pixelColor);

            // Initialize image
            thisImage.BackgroundColor = NetImage.DotNETARGBToVBScriptRGB(0x32FF0000);
            thisImage.MaxX = 1024;
            thisImage.MaxY = 768;

            pixelColor = thisImage.GetPixel(640, 480);

            // Backgrounds can only be solid colors
            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 255, 0, 0).ToArgb(), NetImage.VBScriptRGBToDotNETARGB(pixelColor));

            // Verify color change
            thisImage.BackgroundColor = NetImage.DotNETARGBToVBScriptRGB(System.Drawing.Color.FromArgb(50, 0, 0, 255).ToArgb());
            thisImage.ClearImage();
            pixelColor = thisImage.GetPixel(640, 480);

            Assert.AreEqual(System.Drawing.Color.FromArgb(255, 0, 0, 255).ToArgb(), NetImage.VBScriptRGBToDotNETARGB(pixelColor));
        }

        [Test]
        public void TestLineTo()
        {
            const string outputFilePath = "../../Output/LineTo.png";
            var redColor = System.Drawing.Color.FromArgb(255, 255, 0, 0).ToArgb();
            var blueColor = System.Drawing.Color.FromArgb(255, 0, 0, 255).ToArgb();

            var thisImage = new NetImage();
            thisImage.BackgroundColor = NetImage.DotNETARGBToVBScriptRGB(System.Drawing.Color.FromArgb(255, 255, 255, 255).ToArgb());
            thisImage.MaxX = 1024;
            thisImage.MaxY = 1024;
            thisImage.Filename = outputFilePath;
            thisImage.ImageFormat = NetImage.ImageFormats.PNG; // bitmap form to preserve pixels
            thisImage.AutoClear = false;

            thisImage.X = 0;
            thisImage.Y = 0;
            thisImage.PenColor = NetImage.DotNETARGBToVBScriptRGB(redColor);
            thisImage.PenWidth = 1;

            thisImage.LineTo(1023, 1023);
            thisImage.PenColor = NetImage.DotNETARGBToVBScriptRGB(blueColor);
            thisImage.LineTo(1023, 0);

            thisImage.SaveImage();

            // Verify diagonal pixels
            for (int x = 0; x <= 1022; x++) // Only to 1022 because blue line covers up bottom right pixel
            {
                Assert.AreEqual(redColor, NetImage.VBScriptRGBToDotNETARGB(thisImage.GetPixel(x, x)));
            }

            // Verify vertical pixels
            for (int y = 0; y <= 1023; y++)
            {
                Assert.AreEqual(blueColor, NetImage.VBScriptRGBToDotNETARGB(thisImage.GetPixel(1023, y)));
            }

        }

        [Test]
        public void TestLoadBlob()
        {
            Trace.WriteLine("TestLoadBlob not implemented");
        }

        [Test]
        public void TestLoadImage()
        {
            Trace.WriteLine("TestLoadImage not implemented");
        }

        [Test]
        public void TestRectangle()
        {
            Trace.WriteLine("TestRectangle not implemented");
        }

        [Test]
        public void TestResizeSameAspectRatioWithConstraint()
        {
            const string outputFilePath = "../../Output/Resize-Same_aspect_ratio-Constrained.jpg";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.Filename = outputFilePath;
            thisImage.ConstrainResize = true;
            thisImage.Resize(800, 600);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.AreEqual(800, width);
            Assert.AreEqual(600, height);
        }

        [Test]
        public void TestResizeLandscapeWithConstraint()
        {
            const string outputFilePath = "../../Output/Resize-Variant_aspect_ratio_landscape-Constrained.jpg";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1440x900.png");
            thisImage.Filename = outputFilePath;
            thisImage.ConstrainResize = true;
            thisImage.Resize(800, 600);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.AreEqual(800, width);
            Assert.AreEqual(600, height);
        }

        [Test]
        public void TestResizePortraitWithConstraint()
        {
            const string outputFilePath = "../../Output/Resize-Variant_aspect_ratio_portrait-Constrained.jpg";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/900x1440.png");
            thisImage.Filename = outputFilePath;
            thisImage.ConstrainResize = true;
            thisImage.Resize(800, 600);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.AreEqual(800, width);
            Assert.AreEqual(600, height);
        }

        [Test]
        public void TestResizeLandscapeWithoutConstraint()
        {
            const string outputFilePath = "../../Output/Resize-Variant_aspect_ratio_landscape-Unconstrained.jpg";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1440x900.png");
            thisImage.Filename = outputFilePath;
            thisImage.ConstrainResize = false;
            thisImage.Resize(800, 600);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.AreEqual(800, width);
            Assert.AreEqual(600, height);
        }

        [Test]
        public void TestResizePortraitWithoutConstraint()
        {
            const string outputFilePath = "../../Output/Resize-Variant_aspect_ratio_portrait-Unconstrained.jpg";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/900x1440.png");
            thisImage.Filename = outputFilePath;
            thisImage.ConstrainResize = false;
            thisImage.Resize(800, 600);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.AreEqual(800, width);
            Assert.AreEqual(600, height);
        }

        [Test]
        public void TestRotate90Degrees()
        {
            const string outputFilePath = "../../Output/Rotate-90_degrees.jpg";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.Filename = outputFilePath;
            thisImage.RotateImage(90);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.AreEqual(768, width);
            Assert.AreEqual(1024, height);
        }

        [Test]
        public void TestRotate90DegreesWithCrop()
        {
            const string outputFilePath = "../../Output/Rotate-90_degrees_with_crop.jpg";
            int originalWidth = 0;
            int originalHeight = 0;

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.GetImageSize(out originalWidth, out originalHeight);
            thisImage.Filename = outputFilePath;
            thisImage.RotateImage(90);
            thisImage.ConstrainResize = true;
            thisImage.Resize(originalWidth, originalHeight);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.AreEqual(originalWidth, width);
            Assert.AreEqual(originalHeight, height);
        }

        [Test]
        public void TestRotate180Degrees()
        {
            const string outputFilePath = "../../Output/Rotate-180_degrees.jpg";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.Filename = outputFilePath;
            thisImage.RotateImage(180);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.AreEqual(1024, width);
            Assert.AreEqual(768, height);
        }

        [Test]
        public void TestSaveImage()
        {
            Trace.WriteLine("TestSaveImage not implemented");
        }

        [Test]
        public void TestTextHeight()
        {
            Trace.WriteLine("TestTextHeight not implemented");
        }

        [Test]
        public void TestTextOut()
        {
            Trace.WriteLine("TestTextOut not implemented");
        }

        [Test]
        public void TestTextWidth()
        {
            Trace.WriteLine("TestTextWidth not implemented");
        }

        #endregion


        // ====================================================================
        #region Miscellaneous Tests

        [Test]
        public void TestCreateCOM()
        {
            var type = Type.GetTypeFromProgID("AspNetImage.NetImage");
            Assert.AreNotEqual(null, type, "type is null");
            var obj = Activator.CreateInstance(type);
            Assert.AreNotEqual(null, obj, "obj is null");
        }

        [Test]
        public void TestGIMPImage()
        {
            const string outputFilePath = "../../Output/gimpimage.jpg";

            var thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/gimpimage.jpg");
            if (thisImage.Error.Length <= 0)
            {
                int width = 0;
                int height = 0;
                thisImage.GetImageSize(out width, out height);

                if (width > 0 && height > 0)
                {
                    thisImage.Filename = outputFilePath;
                    thisImage.SaveImage();
                    if (thisImage.Error.Length <= 0)
                    {
                        var fileInfo = new System.IO.FileInfo(outputFilePath);
                        Assert.That(fileInfo.Length > 0, "Image filesize is zero bytes");
                    }
                    else
                        Assert.Fail("SaveImage() error: " + thisImage.Error);
                }
                else
                    Assert.Fail("Loaded image sizes incorrect - width: " + width.ToString(CultureInfo.InvariantCulture) + ", height: " + height.ToString(CultureInfo.InvariantCulture));
            }
            else
                Assert.Fail("LoadImage() error: " + thisImage.Error);
        }

        #endregion
    }
}
