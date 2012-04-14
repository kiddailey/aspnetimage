using System;
using System.Collections.Generic;
using System.Text;
using ASPNetImage;
using NUnit.Framework;
using System.Security.Cryptography;
using System.Drawing;

namespace UnitTests
{
    [TestFixture]
    public class ASPNetImageTest
    {
        [Test]
        public void TestVersion()
        {
            ASPNetImage.NetImage thisImage = new NetImage();

            Assert.That(thisImage.Version == "2.3.1.0 (ASPNetImage v0.2)");
        }

        [Test]
        public void TestRegisteredTo()
        {
            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.RegisteredTo = "Registered To Test";

            Assert.That(thisImage.RegisteredTo == "Registered To Test");
        }

        [Test]
        public void TestExpirationDate()
        {
            ASPNetImage.NetImage thisImage = new NetImage();

            Assert.That(Convert.ToDateTime(thisImage.Expires) > DateTime.Now);
        }

        [Test]
        public void TestGetImageSize()
        {
            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");

            int width, height;
            thisImage.GetImageSize(out width, out height);

            Assert.That(width == 1024 && height == 768, "Image dimensions do not match");
        }

        [Test]
        public void TestAutoClear()
        {
            string outputFilePath = "../../Output/AutoClear.jpg";

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.AutoClear = true;
            thisImage.Filename = outputFilePath;
            thisImage.SaveImage();

            try
            {
                thisImage.RotateImage(90);
                Assert.Fail("Image data not cleared");
            }
            catch
            {
            }
        }

        /// <summary>
        /// Tests the ClearImage method by taking an all white image, clearing it to all black, 
        /// and then compare it to a known all-black image using a SHA hash comparison
        /// </summary>
        [Test]
        public void TestClearImage()
        {
            string outputFilePath = "../../Output/ClearImage.png";

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768-white.png");
            thisImage.ImageFormat = NetImage.ImageFormats.PNG;
            thisImage.Filename = outputFilePath;
            thisImage.BackgroundColor = System.Drawing.Color.FromArgb(0, 0, 0).ToArgb();
            thisImage.ClearImage();
            thisImage.AutoClear = false;
            thisImage.SaveImage();

            ASPNetImage.NetImage clearedImage = new NetImage();
            clearedImage.LoadImage("../../Resources/1024x768-black.png");

            Bitmap modifiedImage = new Bitmap(thisImage.RawNetImage);
            Bitmap comparisonImage = new Bitmap(clearedImage.RawNetImage);

            ImageConverter thisConverter = new ImageConverter();
            byte[] modifiedImageBytes = new byte[0];
            byte[] comparisonImageBytes = new byte[0];
            modifiedImageBytes = (byte[])thisConverter.ConvertTo(modifiedImage, modifiedImageBytes.GetType());
            comparisonImageBytes = (byte[])thisConverter.ConvertTo(comparisonImage, comparisonImageBytes.GetType());

            SHA256Managed hasher = new SHA256Managed();
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
        public void TestFillRect()
        {
            string outputFilePath = "../../Output/FillRect.png";

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768-white.png");
            thisImage.ImageFormat = NetImage.ImageFormats.PNG;
            thisImage.Filename = outputFilePath;
            thisImage.PenColor = System.Drawing.Color.FromArgb(0, 0, 0).ToArgb();
            thisImage.FillRect(10, 10, 20, 20);
            thisImage.SaveImage();
        }

        [Test]
        public void TestCrop()
        {
            string outputFilePath = "../../Output/Crop.jpg";

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.Filename = outputFilePath;
            thisImage.CropImage(10, 50, 800, 600);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.That(width == 800 && height == 600, "Image dimensions do not match");
        }

        [Test]
        public void TestResizeSameAspectRatioWithConstraint()
        {
            string outputFilePath = "../../Output/Resize-Same_aspect_ratio-Constrained.jpg";

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.Filename = outputFilePath;
            thisImage.ConstrainResize = true;
            thisImage.Resize(800, 600);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.That(width == 800 && height == 600, "Image dimensions do not match");
        }

        [Test]
        public void TestResizeLandscapeWithConstraint()
        {
            string outputFilePath = "../../Output/Resize-Variant_aspect_ratio_landscape-Constrained.jpg";

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1440x900.png");
            thisImage.Filename = outputFilePath;
            thisImage.ConstrainResize = true;
            thisImage.Resize(800, 600);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.That(width == 800 && height == 600, "Image dimensions do not match");
        }

        [Test]
        public void TestResizePortraitWithConstraint()
        {
            string outputFilePath = "../../Output/Resize-Variant_aspect_ratio_portrait-Constrained.jpg";

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/900x1440.png");
            thisImage.Filename = outputFilePath;
            thisImage.ConstrainResize = true;
            thisImage.Resize(800, 600);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.That(width == 800 && height == 600, "Image dimensions do not match");
        }

        [Test]
        public void TestResizeLandscapeWithoutConstraint()
        {
            string outputFilePath = "../../Output/Resize-Variant_aspect_ratio_landscape-Unconstrained.jpg";

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1440x900.png");
            thisImage.Filename = outputFilePath;
            thisImage.ConstrainResize = false;
            thisImage.Resize(800, 600);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.That(width == 800 && height == 600, "Image dimensions do not match");
        }

        [Test]
        public void TestResizePortraitWithoutConstraint()
        {
            string outputFilePath = "../../Output/Resize-Variant_aspect_ratio_portrait-Unconstrained.jpg";

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/900x1440.png");
            thisImage.Filename = outputFilePath;
            thisImage.ConstrainResize = false;
            thisImage.Resize(800, 600);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.That(width == 800 && height == 600, "Image dimensions do not match");
        }

        [Test]
        public void TestBrightenImage()
        {
            string outputFilePath = "../../Output/Brightness-Brighten_image.jpg";

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768-black.png");
            thisImage.Filename = outputFilePath;
            thisImage.BrightenImage(50);
            thisImage.SaveImage();

            Assert.That(true == true); // TODO : Check RGB values
        }

        [Test]
        public void TestDarkenImage()
        {
            string outputFilePath = "../../Output/Brightness-Darken_image.jpg";

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768-white.png");
            thisImage.Filename = outputFilePath;
            thisImage.DarkenImage(50);
            thisImage.SaveImage();

            Assert.That(true == true); // TODO : Check RGB values
        }

        [Test]
        public void TestRotate90Degrees()
        {
            string outputFilePath = "../../Output/Rotate-90_degrees.jpg";

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.Filename = outputFilePath;
            thisImage.RotateImage(90);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.That(width == 768 && height == 1024, "Image dimensions do not match");
        }

        [Test]
        public void TestRotate90DegreesWithCrop()
        {
            string outputFilePath = "../../Output/Rotate-90_degrees_with_crop.jpg";
            int originalWidth = 0;
            int originalHeight = 0;

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.GetImageSize(out originalWidth, out originalHeight);
            thisImage.Filename = outputFilePath;
            thisImage.RotateImage(90);
            thisImage.ConstrainResize = true;
            thisImage.Resize(originalWidth, originalHeight);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.That(width == originalWidth && height == originalHeight, "Image dimensions do not match");
        }

        [Test]
        public void TestRotate180Degrees()
        {
            string outputFilePath = "../../Output/Rotate-180_degrees.jpg";

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.Filename = outputFilePath;
            thisImage.RotateImage(180);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.That(width == 1024 && height == 768, "Image dimensions do not match");
        }

        [Test]
        public void TestFlipHorizontal()
        {
            string outputFilePath = "../../Output/Flip-Horizontal.jpg";

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.Filename = outputFilePath;
            thisImage.FlipImage(NetImage.FlipDirections.Horizontal);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.That(width == 1024 && height == 768, "Image dimensions do not match");
        }

        [Test]
        public void TestFlipVertical()
        {
            string outputFilePath = "../../Output/Flip-Vertical.jpg";

            ASPNetImage.NetImage thisImage = new NetImage();
            thisImage.LoadImage("../../Resources/1024x768.png");
            thisImage.Filename = outputFilePath;
            thisImage.FlipImage(NetImage.FlipDirections.Vertical);
            thisImage.SaveImage();

            int width, height;
            thisImage.GetImageFileSize(outputFilePath, out width, out height);

            Assert.That(width == 1024 && height == 768, "Image dimensions do not match");
        }

        [Test]
        public void TestGIMPImage()
        {
            string outputFilePath = "../../Output/gimpimage.jpg";

            ASPNetImage.NetImage thisImage = new NetImage();
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
                        System.IO.FileInfo fileInfo = new System.IO.FileInfo(outputFilePath);
                        Assert.That(fileInfo.Length > 0, "Image filesize is zero bytes");
                    }
                    else
                        Assert.Fail("SaveImage() error: " + thisImage.Error);
                }
                else
                    Assert.Fail("Loaded image sizes incorrect - width: " + width.ToString() + ", height: " + height.ToString());
            }
            else
                Assert.Fail("LoadImage() error: " + thisImage.Error);
        }
    }
}
