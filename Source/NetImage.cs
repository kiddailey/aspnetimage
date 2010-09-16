using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace ASPNetImage
{
    /// <summary>
    /// ASPNetImage
    /// 
    /// A COM Interop Wrapper for .NET System.Drawing meant as a drop-in replacement 
    /// for the third-party ASPImage 2.x component by ServerObjects Inc.
    /// 
    /// ASPNetImage Copyright 2010 John Dailey
    /// kiddailey at hotmail dot com
    /// Distributed under the GNU GENERAL PUBLIC LICENSE v3
    /// See the enclosed gpl-3.0.txt for more information.
    ///
    /// ASPImage Copyright ServerObjects Inc.
    /// </summary>
    public class NetImage
    {
        // ====================================================================
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public NetImage()
        {
        }

        #endregion

        // ====================================================================
        #region Destructor

        /// <summary>
        /// Destructor to free up memory and call the Garbage Collector to 
        /// release our file system hold on the file and allocated memory immediately
        /// </summary>
        ~NetImage()
        {
            this._image.Dispose();
            GC.Collect();
        }

        #endregion

        // ====================================================================
        #region Public Enumerations

        public enum BlobTypes : int
        {
            JPEG = 1,
            BMP = 2
        }

        public enum BrushStyles : int
        {
            Solid = 0,
            Clear = 1,
            Horizontal = 2,
            Vertical = 3,
            FDiagonal = 4,
            BDiagonal = 5,
            Cross = 6,
            DiagCross = 7
        }

        public enum FillStyles : int
        {
            Surface = 0,
            Border = 1
        }

        public enum FlipDirections : int
        {
            Horizontal = 1,
            Vertical = 2
        }

        public enum GradientDirections : int
        {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3
        }

        public enum ImageFormats : int
        {
            JPEG = 1,
            BMP = 2,
            PNG = 3,
            WBMP = 4,
            GIF = 5,
            TGA = 6,
            PCX = 7
        }

        public enum PenStyles : int
        {
            Solid = 0,
            Dash = 1,
            Dot = 2,
            DashDot = 3,
            DashDotDot = 4,
            Clear = 5,
            InsideFrame = 6
        }

        public enum PixelFormats : int
        {
            FourBit = 2,
            EightBit = 3,
            TwentyFourBit = 6
        }

        public enum TextAlignments : int
        {
            Left = 0,
            Right = 2,
            Center = 6
        }

        #endregion

        // ====================================================================
        #region Private Properties

        private bool _autoClear = true;
        private bool _constrainResize = true;
        private string _error;
        private string _filename = "";
        private ImageFormats _imageFormat = ImageFormats.JPEG;
        private int _jpegQuality = 100;
        private bool _progressiveJPEGEncoding = false;
        private System.Drawing.Image _image;
        private string _registeredTo = "This Organization";

        #endregion

        // ====================================================================
        #region Public Properties

        // --------------------------------------------------------------------
        #region Legacy Properties To Be Implemented

        public bool AntiAliasText = true;
        public bool AutoSize = true;
        public int BackgroundColor = 0;
        public int BrushColor = 0;
        public int BrushStyle = 0;
        public bool Bold = false;
        public int DPI = 96;
        public int FontColor = 0;
        public string FontName = "MS Sans Serif";
        public int FontSize = 12;
        public bool Italic = false;
        public int MaxX = 0;
        public int MaxY = 0;
        public int PadSize = 0;
        public int PenColor = 0;
        public int PenStyle = 0;
        public int PenWidth = 0;
        public int PixelFormat = 6;
        public bool Strikeout = false;
        public TextAlignments TextAlign = TextAlignments.Left;
        public int TextAngle = 0;
        public int ThreeDColor = 0;
        public bool Transparent = false;
        public int TransparentColor = 0;
        public bool TransparentText = true;
        public bool Underline = false;
        public int X = 0;
        public int Y = 0;

        #endregion

        /// <summary>
        /// Gets or sets whether the image should be disposed of from memory after
        /// a successful save to disk.
        /// </summary>
        public bool AutoClear
        {
            get
            {
                return this._autoClear;
            }
            set
            {
                this._autoClear = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the resize method should constrain the image's original
        /// aspect ratio and if so, center-crops the image if needed to fit the dimensions
        /// </summary>
        public bool ConstrainResize
        {
            get
            {
                return this._constrainResize;
            }
            set
            {
                this._constrainResize = value;
            }
        }

        /// <summary>
        /// Gets the last error string that occurred with the object
        /// </summary>
        public string Error
        {
            get
            {
                return this._error;
            }
        }

        /// <summary>
        /// For compatibility, returns a future date that the component supposedly expires
        /// </summary>
        public string Expires
        {
            get
            {
                // TODO: Verify date format returned from original component
                return DateTime.Now.AddYears(50).ToString();
            }
        }

        /// <summary>
        /// Gets or sets the full path and filename used by the SaveImage() method
        /// </summary>
        public string Filename
        {
            get
            {
                return this._filename;
            }
            set
            {
                this._filename = value.Trim();
            }
        }

        /// <summary>
        /// Gets or sets the image format to be used when saving the image. Not currently
        /// supported and only JPG format is output.
        /// </summary>
        public ImageFormats ImageFormat
        {
            get
            {
                return this._imageFormat;
            }
            set
            {
                this._imageFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets the quality percentage for saved JPEG images
        /// </summary>
        public int JPEGQuality
        {
            get
            {
                return this._jpegQuality;
            }
            set
            {
                this._jpegQuality = value;

                if (this._jpegQuality > 100)
                    this._jpegQuality = 100;
                if (this._jpegQuality < 0)
                    this._jpegQuality = 0;
            }
        }

        /// <summary>
        /// Gets or sets whether progressive JPEG encoding should be used when saving
        /// the image.  Not currently supported.
        /// </summary>
        public bool ProgressiveJPEGEncoding
        {
            get
            {
                return this._progressiveJPEGEncoding;
            }
            set
            {
                this._progressiveJPEGEncoding = value;
            }
        }

        /// <summary>
        /// Returns the raw binary data for the image.  Not currently implemented. 
        /// Replaces "Image" property in original component.
        /// </summary>
        public byte[] Image
        {
            get
            {
                return new byte[] { };
            }
        }

        /// <summary>
        /// Gets or sets who the component is registered to.  Provided for compatibility
        /// </summary>
        public string RegisteredTo
        {
            get
            {
                return this._registeredTo;
            }
            set
            {
                this._registeredTo = value.Trim();
            }
        }

        /// <summary>
        /// Returns the current version number of the component
        /// </summary>
        public string Version
        {
            get
            {
                return "2.3.1.0 (ASPNetImage v0.1)";
            }
        }

        #endregion

        // ====================================================================
        #region Private Methods

        /// <summary>
        /// Delegate for encoding with SaveImage() method
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        /// <summary>
        /// Adjusts the brightness of the image from -255 (full black) to 255 (full white)
        /// </summary>
        /// <param name="percentage"></param>
        private void AdjustBrightness(float adjustmentValue)
        {

            if (adjustmentValue < -255)
                adjustmentValue = -255;

            if (adjustmentValue > 255)
                adjustmentValue = 255;

            // Based off James Craig's ColorMatrix blog entry at:
            // http://www.gutgames.com/post/Adjusting-Brightness-of-an-Image-in-C.aspx

            Bitmap tempImage = new Bitmap(this._image);
            float finalValue = adjustmentValue / 255.0f;
            Bitmap newBitmap = new Bitmap(tempImage.Width, tempImage.Height);
            Graphics newGraphics = Graphics.FromImage(newBitmap);

            float[][] floatColorMatrix = {
                    new float[] {1, 0, 0, 0, 0},
                    new float[] {0, 1, 0, 0, 0},
                    new float[] {0, 0, 1, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {finalValue, finalValue, finalValue, 1, 1}
                };
            ColorMatrix newColorMatrix = new ColorMatrix(floatColorMatrix);
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(newColorMatrix);

            newGraphics.DrawImage(tempImage, new Rectangle(0, 0, tempImage.Width, tempImage.Height), 0, 0, tempImage.Width, tempImage.Height, GraphicsUnit.Pixel, attributes);

            // Get rid of the old image data first
            this._image.Dispose();
            this._image = newBitmap;

            // Cleanup
            attributes.Dispose();
            newGraphics.Dispose();
        }

        #endregion

        // ====================================================================
        #region Public Methods

        // --------------------------------------------------------------------
        #region Legacy Methods To Be Implemented

        public void AddAnimationControl(int intDelay, bool bolTransparent, int intTransparentColor)
        {
        }

        public bool AddFont(string strFontFileName)
        {
            return false;
        }

        public bool AddImage(string strFileName, int intX, int intY)
        {
            return false;
        }

        public void AddImageToAnimation()
        {
        }

        public bool AddImageTransparent(string strFileName, int intX, int intY, int intTransparentColor)
        {
            return false;
        }

        public void AngleArc(int intX, int intY, int intRadius, double dblStartDegrees, double dblSweepDegrees)
        {
        }

        public void Arc(int intX1, int intY1, int intX2, int intY2, int intX3, int intY3, int intX4, int intY4)
        {
        }

        public void BeginPath()
        {
        }

        public void Blur(int intTimes)
        {
        }

        public void Chord(int intX1, int intY1, int intX2, int intY2, int intX3, int intY3, int intX4, int intY4)
        {
        }

        public void ClearImage()
        {
        }

        public void ClearTexture()
        {
        }

        public void Contrast(int intDegree)
        {
        }

        public void CreateBlackWhite()
        {
        }

        public void CreateGrayScale()
        {
        }

        public void CreateButton(int intBorder, bool bolSoft)
        {
        }

        public void CreateNegative()
        {
        }

        public void DoMerge(string strFileName, int intPercent)
        {
        }

        public void Emboss()
        {
        }

        public void EndPath()
        {
        }

        public void Ellipse(int intX1, int intY1, int intX2, int intY2)
        {
        }

        public void FillPath()
        {
        }

        public void FillRect(int intLeft, int intTop, int intRight, int intBottom)
        {
        }

        public void FishEye(int intDegree)
        {
        }

        public void FloodFill(int intX, int intY, int intColor, int intFillStyl)
        {
        }

        public void FrameRect(int intLeft, int intTop, int intRight, int intBottom)
        {
        }

        public int GetPixel(int intX, int intY)
        {
            return 0;
        }

        public void GradientOneWay(int intBeginColor, int intEndColor, int intDirection)
        {
        }

        public void GradientTwoWay(int intBeginColor, int intEndColor, int intDirection, int intInOut)
        {
        }

        public void LineTo(int intX, int intY)
        {
        }

        public bool LoadBlob(object ovBlob, int intType)
        {
            return false;
        }

        public bool LoadTexture(string strFileName)
        {
            return false;
        }

        public void Mosaic(int intX, int intY)
        {
        }

        public void Pie(int intX1, int intY1, int intX2, int intY2, int intX3, int intY3, int intX4, int intY4)
        {
        }

        public void PolyBezier(int[,] aryPoints)
        {
        }

        public void Polygon(int[,] aryPoints)
        {
        }

        public void PolyLine(int[,] aryPoint)
        {
        }

        public void Rectangle(int intX1, int intY1, int intX2, int intY2)
        {
        }

        /// <summary>
        /// Resizes the image using a resampling to produce better quality. Not implemented, but calls
        /// Resize() method.
        /// </summary>
        /// <param name="intWidth"></param>
        /// <param name="intHeigh"></param>
        public void ResizeR(int intWidth, int intHeigh)
        {
            this.Resize(intWidth, intHeigh);
        }

        /// <summary>
        /// Rotates the image the specified number of degrees. Partially implemented and only
        /// currently supports 90, 180, and 270 degree clockwise rotations.
        /// </summary>
        /// <param name="intDegrees"></param>
        public void RotateImage(int intDegrees)
        {
            if (intDegrees == 90 || intDegrees == 180 || intDegrees == 270)
            {
                switch (intDegrees)
                {
                    case 90:
                        this._image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;

                    case 180:
                        this._image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;

                    case 270:
                        this._image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
            }
        }

        public void RoundRect(int intX1, int intY1, int intX2, int intY2, int intX3, int intY3)
        {
        }

        public void Saturation(int intDegree)
        {
        }

        public bool SaveAnimation()
        {
            return false;
        }

        public void SetPixel(int intX, int intY, int intColor)
        {
        }

        public void Sharpen(int intValue)
        {
        }

        public void StartAnimation(bool bolLoop)
        {
        }

        public void StrokeAndFillPath()
        {
        }

        public void TextOut(string strText, int intX, int intY, bool bol3d)
        {
        }

        public int TextHeight(string strValue)
        {
            return 0;
        }

        public int TextWidth(string strValue)
        {
            return 0;
        }

        public void TintImage(int intColor)
        {
        }

        public void Twist(int intDegree)
        {
        }

        public void Wave(int intGraphicSize, int intWaveSize)
        {
        }

        #endregion

        /// <summary>
        /// Crops the image with the specified dimensions
        /// </summary>
        /// <param name="intStartX"></param>
        /// <param name="intStartY"></param>
        /// <param name="intWidth"></param>
        /// <param name="intHeight"></param>
        public void CropImage(int startX, int startY, int width, int height)
        {
            Bitmap croppedImage = new Bitmap(width, height);
            Graphics graphicsCrop = Graphics.FromImage(croppedImage);
            Rectangle recDest = new Rectangle(0, 0, width, height);
            graphicsCrop.DrawImage(this._image, recDest, startX, startY, width, height, GraphicsUnit.Pixel);

            this._image.Dispose();
            this._image = croppedImage;

            graphicsCrop.Dispose();
        }

        /// <summary>
        /// Flips the image vertically (intDirection == 2) or horizontally (intDirection == 1)
        /// </summary>
        /// <param name="intDirection"></param>
        public void FlipImage(FlipDirections direction)
        {
            if (direction == FlipDirections.Horizontal)
                this._image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            else if (direction == FlipDirections.Vertical)
                this._image.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }

        /// <summary>
        /// Returns the image width and height of the specified file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void GetImageFileSize(string filePath, out int width, out int height)
        {
            width = 0;
            height = 0;

            System.Drawing.Image thisImage = System.Drawing.Image.FromFile(filePath);

            if (thisImage != null)
            {
                width = thisImage.Width;
                height = thisImage.Height;
            }

            thisImage.Dispose();
        }

        /// <summary>
        /// Returns the image width and height of the currently loaded image
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void GetImageSize(out int width, out int height)
        {
            width = 0;
            height = 0;

            if (this._image != null)
            {
                width = this._image.Width;
                height = this._image.Height;
            }
        }

        /// <summary>
        /// Resizes the image to specified width and height
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Resize(int width, int height)
        {
            // Don't scale and crop the original image if we don't need to.
            if (this._image.Width != width || this._image.Height != height)
            {
                int originalWidth = this._image.Width;
                int originalHeight = this._image.Height;
                int newWidth = width;
                int newHeight = height;

                Bitmap resizedImage;
                Graphics graphicsDest = null;

                // Constraining the proportions of the image will resize the image
                // maintaining the same aspect ratio and crop as needed
                if (this.ConstrainResize)
                {
                    newHeight = Convert.ToInt32((originalHeight * width) / originalWidth);

                    // If the original height of the image is greater than our newly
                    // calculated height, the image is either very short and wide or portrait,
                    // so we need to shift our constraint calculation to height bias rather
                    // than width bias.
                    if (newHeight < height)
                    {
                        newHeight = height;
                        newWidth = Convert.ToInt32((originalWidth * height) / originalHeight);
                    }
                }

                // Check again since we may have changed our scale dimensions and only 
                // rescale the image if needed.
                if (originalWidth != newWidth || originalHeight != newHeight)
                {
                    resizedImage = new Bitmap(newWidth, newHeight);
                    graphicsDest = Graphics.FromImage(resizedImage);
                    graphicsDest.DrawImage(this._image, 0, 0, newWidth + 1, newHeight + 1);

                    this._image.Dispose();
                    this._image = resizedImage;
                }

                // Crop the image to the final size if necessary
                if ((newHeight > height) || (newWidth > width))
                {
                    // Use the center of the image as our basis for cropping
                    int cropY = Convert.ToInt32((newHeight - height) / 2);
                    int cropX = Convert.ToInt32((newWidth - width) / 2);

                    this.CropImage(cropX, cropX, width, height);
                }

                graphicsDest.Dispose();
            }
        }

        /// <summary>
        /// Brightens an image by the specified percentage
        /// </summary>
        /// <param name="percentage"></param>
        public void BrightenImage(int percentage)
        {
            if (percentage <= 0)
                percentage = 1;

            if (percentage > 100)
                percentage = 100;

            this.AdjustBrightness(255f * ((float)percentage / 100f));
        }

        /// <summary>
        /// Darkens the image by the specified percentage
        /// </summary>
        /// <param name="intDegree"></param>
        public void DarkenImage(int percentage)
        {
            if (percentage <= 0)
                percentage = 1;

            if (percentage > 100)
                percentage = 100;

            this.AdjustBrightness(-(255f * ((float)percentage / 100f)));
        }

        /// <summary>
        /// Loads image from the specified file path
        /// </summary>
        /// <param name="imageFilePath"></param>
        /// <returns></returns>
        public bool LoadImage(string imageFilePath)
        {
            FileStream fileStream = null;

            try
            {

                // We're not using System.Drawing.Image.FromFile() here because it locks the file
                // until the object is disposed and we may need to save and/or manipulate the
                // raw file in the meantime.

                fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                this._image = System.Drawing.Image.FromStream(fileStream);

                if (this._image != null)
                    return true;
                else
                    this._error = "Unknown error loading image";

            }
            catch (Exception e)
            {
                this._error = e.ToString();
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
                fileStream.Dispose();
            }

            return false;
        }

        /// <summary>
        /// Saves the image to the file path specified by the object's Filename property.  Note
        /// that at this time, only JPG format is supported for output.
        /// </summary>
        /// <returns></returns>
        public bool SaveImage()
        {
            if (this.Filename.Length > 0)
            {
                ImageCodecInfo imageCodecInfo;
                Encoder imageEncoder;
                EncoderParameter imageEncoderParameter;
                EncoderParameters imageEncoderParameters;

                imageCodecInfo = GetEncoderInfo("image/jpeg");
                imageEncoder = Encoder.Quality;
                imageEncoderParameters = new EncoderParameters(1);
                imageEncoderParameter = new EncoderParameter(imageEncoder, this.JPEGQuality);
                imageEncoderParameters.Param[0] = imageEncoderParameter;

                this.Filename = Path.ChangeExtension(this.Filename, "jpg");

                try
                {
                    this._image.Save(this.Filename, imageCodecInfo, imageEncoderParameters);

                    if (File.Exists(this.Filename))
                    {
                        if (this.AutoClear)
                            this._image.Dispose();

                        return true;
                    }
                    else
                        this._error = "Unknown error saving image";
                }
                catch (Exception e)
                {
                    this._error = e.ToString();
                }
            }
            else
                this._error = "Filename property not set";

            return false;
        }

        #endregion

    }
}
