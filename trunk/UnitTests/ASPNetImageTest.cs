using System;
using System.Collections.Generic;
using System.Text;
using ASPNetImage;
using NUnit.Framework;

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
    }
}
