using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Drawing.Text;

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
    [ProgId("AspImage.Image")]
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
            try
            {
                this._image.Dispose();
            }
            catch
            {
            }
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
        private bool _antiAliasText = false; //GF
        private int _backgroundColor = Color.White.ToArgb();
        private bool _bold = false; //GF
        private bool _constrainResize = true;
        private string _error = "";
        private string _filename = "";
        private int _fontColor = 0; //GF
        private string _fontName = "MS Sans Serif"; //GF
        private int _fontSize = 12; //GF
        private ImageFormats _imageFormat = ImageFormats.JPEG;
        private int _jpegQuality = 100;
        private bool _progressiveJPEGEncoding = false;
        private int _maxX = 0; //GF
        private int _maxY = 0; //GF
        private System.Drawing.Image _image;
        private string _registeredTo = "This Organization";

        #endregion

        // ====================================================================
        #region Public Properties

        // --------------------------------------------------------------------
        #region Legacy Properties To Be Implemented

        public bool AutoSize = true;
        public int BrushColor = 0;
        public int BrushStyle = 0;
        public int DPI = 96;
        public bool Italic = false;
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
        /// Gets or sets whether anti-aliased text is added on the image
        /// </summary>
        public bool AntiAliasText //GF
        {
            get
            {
                return this._antiAliasText;
            }
            set
            {
                this._antiAliasText = value;
            }
        }

        /// <summary>
        /// Integer value specifies the background color for NEW image manipulations. 
        /// It does not magically set the "background" to the specified color
        /// </summary>
        public int BackgroundColor //GF
        {
            get
            {
                return this._backgroundColor;
            }
            set
            {
                // set alpha channel to opaque, VBSCRIPT constants set it to 0 so the fill
                // does not work because 0 means transparent
                this._backgroundColor = (int)((uint)value | 0xFF000000);
            }
        }

        /// <summary>
        /// True/false value determines if font is bold or not 
        /// </summary>
        public bool Bold //GF
        {
            get
            {
                return this._bold;
            }
            set
            {
                this._bold = value;
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
        /// The integer FontColor specifies the color of the font 
        /// </summary>
        public int FontColor //GF
        {
            get
            {
                return this._fontColor;
            }
            set
            {
                this._fontColor = value;
            }
        }

        /// <summary>
        /// The string FontName specifies the name of the font
        /// </summary>
        public string FontName //GF
        {
            get
            {
                return this._fontName;
            }
            set
            {
                this._fontName = value;
            }
        }

        /// <summary>
        /// The integer FontSize specifies the size of the font
        /// </summary>
        public int FontSize //GF
        {
            get
            {
                return this._fontSize;
            }
            set
            {
                this._fontSize = value;
            }
        }

        /// <summary>
        /// Gets the classes .NET image instance
        /// </summary>
        public Image RawNetImage
        {
            get
            {
                return this._image;
            }
        }

        /// <summary>
        /// Gets or sets the image format to be used when saving the image. 
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
        /// The MaxX property determines the X size of the image
        /// </summary>
        public int MaxX //GF
        {
            get
            {
                if (_image != null)
                {
                    return _image.Width;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                this._maxX = value;
                // create the image if dimensions are already set
                if (_image == null && this._maxX > 0 && this._maxY > 0)
                {
                    this.ClearImage();
                }
            }
        }

        /// <summary>
        /// The MaxY property determines the Y size of the image
        /// </summary>
        public int MaxY //GF
        {
            get
            {
                if (_image != null)
                {
                    return _image.Height;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                this._maxY = value;
                // create the image if dimensions are already set
                if (_image == null && this._maxX > 0 && this._maxY > 0)
                {
                    this.ClearImage();
                }
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
        /// Returns the raw binary data for the image.  GF
        /// Replaces "Image" property in original component.
        /// </summary>
        public byte[] Image
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                switch (this.ImageFormat)
                {
                    case ImageFormats.BMP:
                        _image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case ImageFormats.GIF:
                        _image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case ImageFormats.PNG:
                        _image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case ImageFormats.JPEG:
                    default:
                        _image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                }
                return ms.ToArray();
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
                return "2.3.1.0 (ASPNetImage v0.2)";
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

        public void ClearTexture()
        {
        }

        public void CreateBlackWhite()
        {
        }

        public void CreateButton(int intBorder, bool bolSoft)
        {
        }

        public void DoMerge(string strFileName, int intPercent)
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
            //Graphics graphicsDest = Graphics.FromImage(this._image);
            //LinearGradientBrush thisBrush;

            //switch (intDirection)
            //{
            //    case 0:
            //        // up
            //        thisBrush = new LinearGradientBrush(
            //            new Point(this._image.Width / 2, this._image.Height - 1), 
            //            new Point(this._image.Width / 2, 0), 
            //            Color.FromArgb(intBeginColor), 
            //            Color.FromArgb(intEndColor));

            //}
        }

        public void GradientTwoWay(int intBeginColor, int intEndColor, int intDirection, int intInOut)
        {
        }

        public void LineTo(int intX, int intY)
        {
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
        { //TODO: vedere se si può rotare con un grado qualsiasi
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
        /// Adds a new image to the canvas using the intX and intY coordinates
        /// <param name="strFileName"></param>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        /// </summary>
        public bool AddImage(string strFileName, int intX, int intY) //GF
        {
            FileStream fileStream = null;
            Graphics graphicsDest = Graphics.FromImage(this._image);
            try
            {
                fileStream = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                System.Drawing.Image addedImage = System.Drawing.Image.FromStream(fileStream);
                graphicsDest.DrawImage(addedImage, intX, intY);
            }
            catch (Exception e)
            {
                this._error = e.ToString();
            }
            finally
            {
                graphicsDest.Dispose();
                // release image file in the file system
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
            return false;
        }

        /// <summary>
        /// Clears the entire image with the current BackgroundColor
        /// </summary>
        public void ClearImage()
        {
            Graphics graphicsDest = null;
            if (this._image == null)
            {
                Bitmap bitmapDest = new Bitmap(this._maxX, this._maxY);
                this._image = bitmapDest;
            }
            graphicsDest = Graphics.FromImage(this._image);
            graphicsDest.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            Color brushColor = Color.FromArgb(this.BackgroundColor);
            Brush coloredBrush = new SolidBrush(brushColor);
            graphicsDest.FillRectangle(coloredBrush, 0, 0, this._image.Width, this._image.Height);
            graphicsDest.DrawImage(this._image, 0, 0);

            graphicsDest.Dispose();
            coloredBrush.Dispose();
        }

        /// <summary>
        /// Modifies the image contrast. intDegree should be -100 to 100
        /// </summary>
        /// <param name="intDegree"></param>
        public void Contrast(int intDegree) //GF
        {
            if (intDegree < -100)
                intDegree = -100;
            if (intDegree > 100)
                intDegree = 100;
            float factor = (float)Math.Pow((100.0 + intDegree) / 100.0, 2.0);

            Graphics graphicsDest = Graphics.FromImage(this._image);
            ImageAttributes ia = new ImageAttributes();
            ColorMatrix cm = new ColorMatrix(new float[][]{   new float[]{factor,0f,0f,0f,0f}, 
                                                              new float[]{0f,factor,0f,0f,0f}, 
                                                              new float[]{0f,0f,factor,0f,0f}, 
                                                              new float[]{0f,0f,0f,1f,0f}, 
                                                              new float[]{0.001f,0.001f,0.001f,0f,1f}});
            ia.SetColorMatrix(cm);
            graphicsDest.DrawImage(_image, new Rectangle(0, 0, _image.Width, _image.Height), 0, 0, _image.Width, _image.Height, GraphicsUnit.Pixel, ia);

            graphicsDest.Dispose();
            ia.Dispose();
        }

        /// <summary>
        /// Turns the current image into a Gray Scale image
        /// </summary>
        public void CreateGrayScale() //GF
        {
            Graphics graphicsDest = Graphics.FromImage(this._image);
            ImageAttributes ia = new ImageAttributes();
            ColorMatrix cm = new ColorMatrix(new float[][]{   new float[]{0.299f, 0.299f, 0.299f, 0, 0}, 
                                                              new float[]{0.587f, 0.587f, 0.587f, 0, 0}, 
                                                              new float[]{0.114f, 0.114f, 0.114f, 0, 0}, 
                                                              new float[]{     0,      0,      0, 1, 0}, 
                                                              new float[]{     0,      0,      0, 0, 0}});
            ia.SetColorMatrix(cm);
            graphicsDest.DrawImage(_image, new Rectangle(0, 0, _image.Width, _image.Height), 0, 0, _image.Width, _image.Height, GraphicsUnit.Pixel, ia);

            graphicsDest.Dispose();
            ia.Dispose();
        }

        /// <summary>
        /// Creates a negative image effect of the current image
        /// </summary>
        public void CreateNegative() //GF
        {
            Graphics graphicsDest = Graphics.FromImage(this._image);
            ImageAttributes ia = new ImageAttributes();
            ColorMatrix cm = new ColorMatrix(new float[][]{   new float[]{-1, 0, 0, 0, 0}, 
                                                              new float[]{0, -1, 0, 0, 0}, 
                                                              new float[]{0, 0, -1, 0, 0}, 
                                                              new float[]{0, 0, 0, 1, 0}, 
                                                              new float[]{1, 1, 1, 0, 1}});
            ia.SetColorMatrix(cm);
            graphicsDest.DrawImage(_image, new Rectangle(0, 0, _image.Width, _image.Height), 0, 0, _image.Width, _image.Height, GraphicsUnit.Pixel, ia);
            graphicsDest.Dispose();
            ia.Dispose();
        }

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
            graphicsCrop.Clear(Color.FromArgb(this.BackgroundColor));
            Rectangle recDest = new Rectangle(0, 0, width, height);
            graphicsCrop.DrawImage(this._image, recDest, startX, startY, width, height, GraphicsUnit.Pixel);

            this._image.Dispose();
            this._image = croppedImage;

            graphicsCrop.Dispose();
        }

        /// <summary>
        /// Gives the current image an embossed look
        /// </summary>
        public void Emboss() //GF
        {
            Graphics graphicsDest = Graphics.FromImage(this._image);
            Bitmap imgTemp = new Bitmap(_image);
            imgTemp.SetResolution(_image.HorizontalResolution, _image.VerticalResolution);
            Rectangle rect = new Rectangle(0, 0, imgTemp.Width, imgTemp.Height);
            BitmapData bmpData = imgTemp.LockBits(rect, ImageLockMode.ReadWrite, _image.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int numBytes = imgTemp.Width * imgTemp.Height * 3;
            int rowLenght = imgTemp.Width * 3;
            byte[] rgbValues = new byte[numBytes];
            Marshal.Copy(ptr, rgbValues, 0, numBytes);

            for (int i = 0; i < rgbValues.Length; i += 3)
            {
                if (i < rgbValues.Length - rowLenght - 3)
                {
                    byte b = (byte)(rgbValues[i] - rgbValues[rowLenght + i + 3] + (byte)128);
                    rgbValues[i] = (byte)Math.Min((byte)Math.Abs(b), (byte)255);

                    b = (byte)(rgbValues[i + 1] - rgbValues[rowLenght + i + 4] + (byte)128);
                    rgbValues[i + 1] = (byte)Math.Min((byte)Math.Abs(b), (byte)255);

                    b = (byte)(rgbValues[i + 2] - rgbValues[rowLenght + i + 5] + (byte)128);
                    rgbValues[i + 2] = (byte)Math.Min((byte)Math.Abs(b), (byte)255);
                }
                else
                {
                    rgbValues[i] = rgbValues[i + 1] = rgbValues[i + 2] = 128;
                }
            }

            Marshal.Copy(rgbValues, 0, ptr, numBytes);
            imgTemp.UnlockBits(bmpData);
            graphicsDest.DrawImage(imgTemp, 0, 0);
            graphicsDest.Dispose();
            imgTemp.Dispose();
        }

        /// <summary>
        /// Fills the specified rectangle with the current PenColor
        /// </summary>
        /// <param name="intLeft"></param>
        /// <param name="intTop"></param>
        /// <param name="intRight"></param>
        /// <param name="intBottom"></param>
        public void FillRect(int intLeft, int intTop, int intRight, int intBottom)
        {
            Graphics graphicsDest = Graphics.FromImage(this._image);
            graphicsDest.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            Color brushColor = Color.FromArgb(this.PenColor);
            Brush coloredBrush = new SolidBrush(brushColor);
            graphicsDest.FillRectangle(coloredBrush, intLeft, intTop, intRight - intLeft, intBottom - intTop);
            graphicsDest.DrawImage(this._image, 0, 0);

            graphicsDest.Dispose();
            coloredBrush.Dispose();
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
                    int cropY = (newHeight == height ? 0 : Convert.ToInt32((newHeight - height) / 2));
                    int cropX = (newWidth == width ? 0 : Convert.ToInt32((newWidth - width) / 2));

                    this.CropImage(cropX, cropY, width, height);
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
        /// LoadBlob is designed to allow the loading of binary image data from other AspImage objects 
        /// (using the .Image property for ovBlob) or from other data sources where binary image data 
        /// is available via an OLE variant pointer. ovBlob is an OLE variant pointing to raw image data. 
        /// The raw image data is loaded onto the AspImage canvas.
        /// The parameter intType indicates what type of format the binary data is in. Valid intTypes are:
        ///     1: JPEG 
        ///     2: BMP 
        /// </summary>
        /// <param name="ovBlob"></param>
        /// <param name="intType"></param>
        /// <returns></returns>
        public bool LoadBlob(object ovBlob, int intType)
        {
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream((byte[])ovBlob);
                this._image = System.Drawing.Image.FromStream(ms);

                if (this._image != null)
                {
                    // GIMP thumbnails specifically cause System.Net.Image.Save() to fail and must be removed
                    // NET removes embedded thumbnails when the image is rotated, so we can easily strip them
                    // by flipping the image once, and then back again
                    this.FlipImage(FlipDirections.Horizontal);
                    this.FlipImage(FlipDirections.Horizontal);
                    if (intType == 1)
                        this.ImageFormat = ImageFormats.JPEG;
                    if (intType == 2)
                        this.ImageFormat = ImageFormats.BMP;

                    return true;
                }
                else
                    this._error = "Unknown error loading image";

            }
            catch (Exception e)
            {
                this._error = e.ToString();
            }
            finally
            {
                if (ms != null)
                {
                    ms.Close();
                    ms.Dispose();
                }
            }

            return false;
        }

        /// <summary>
        /// Loads image from the specified file path
        /// </summary>
        /// <param name="imageFilePath"></param>
        /// <returns></returns>
        /// <summary>
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
                {
                    // GIMP thumbnails specifically cause System.Net.Image.Save() to fail and must be removed
                    // NET removes embedded thumbnails when the image is rotated, so we can easily strip them
                    // by flipping the image once, and then back again
                    this.FlipImage(FlipDirections.Horizontal);
                    this.FlipImage(FlipDirections.Horizontal);

                    return true;
                }
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
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
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

                switch (this.ImageFormat)
                {
                    case ImageFormats.BMP:

                        this.Filename = Path.ChangeExtension(this.Filename, "bmp");

                        try
                        {
                            this._image.Save(this.Filename, System.Drawing.Imaging.ImageFormat.Bmp);
                        }
                        catch (Exception e)
                        {
                            this._error = e.ToString();
                        }
                        break;

                    case ImageFormats.GIF:

                        this.Filename = Path.ChangeExtension(this.Filename, "gif");

                        try
                        {
                            this._image.Save(this.Filename, System.Drawing.Imaging.ImageFormat.Gif);
                        }
                        catch (Exception e)
                        {
                            this._error = e.ToString();
                        }
                        break;

                    case ImageFormats.PNG:

                        this.Filename = Path.ChangeExtension(this.Filename, "png");

                        try
                        {
                            this._image.Save(this.Filename, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        catch (Exception e)
                        {
                            this._error = e.ToString();
                        }

                        break;

                    case ImageFormats.JPEG:
                    default:

                        imageCodecInfo = GetEncoderInfo("image/jpeg");
                        imageEncoder = Encoder.Quality;
                        imageEncoderParameters = new EncoderParameters(1);
                        imageEncoderParameter = new EncoderParameter(imageEncoder, this.JPEGQuality);
                        imageEncoderParameters.Param[0] = imageEncoderParameter;

                        this.Filename = Path.ChangeExtension(this.Filename, "jpg");

                        try
                        {
                            this._image.Save(this.Filename, imageCodecInfo, imageEncoderParameters);
                        }
                        catch (Exception e)
                        {
                            this._error = e.ToString();
                        }

                        break;
                }

                try
                {
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

        /// <summary>
        /// TextOut writes a text value using the current font, color and other 
        /// characteristics to the imageat the location specified by intX and intY. 
        /// If bol3d is true then the text is rendered using a 3d look
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        /// <param name="bol3d"></param>
        public void TextOut(string strText, int intX, int intY, bool bol3d) //GF
        {
            Graphics graphicsDest = Graphics.FromImage(this._image);
            if (AntiAliasText)
                graphicsDest.TextRenderingHint = TextRenderingHint.AntiAlias;

            Font font = new Font(FontName, Convert.ToSingle(FontSize));
            Color color = Color.FromArgb(Convert.ToInt32(FontColor % 256),
                                         Convert.ToInt32(Math.Round((float)(FontColor / 256), 0) % 256),
                                         Convert.ToInt32(Math.Round((float)(FontColor / 65536), 0) % 256));
            Brush brush = new SolidBrush(color);
            graphicsDest.DrawString(strText, font, brush, new Point(intX, intY));
            graphicsDest.Dispose();
            brush.Dispose();
            font.Dispose();
        }

        /// <summary>
        /// Returns the text height for strValue using the current font, font size and font characteristics
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public int TextHeight(string strValue) //GF
        {
            Graphics graphicsDest = Graphics.FromImage(this._image);
            Font font = new Font(FontName, Convert.ToSingle(FontSize));
            SizeF size = graphicsDest.MeasureString(strValue, font);
            graphicsDest.Dispose();
            font.Dispose();

            return Convert.ToInt32(size.Height);
        }

        /// <summary>
        /// Returns the text width for strValue using the current font, font size and font characteristics
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public int TextWidth(string strValue) //GF
        {
            Graphics graphicsDest = Graphics.FromImage(this._image);
            Font font = new Font(FontName, Convert.ToSingle(FontSize));
            SizeF size = graphicsDest.MeasureString(strValue, font);
            graphicsDest.Dispose();
            font.Dispose();

            return Convert.ToInt32(size.Width);
        }

        #endregion

    }
}
