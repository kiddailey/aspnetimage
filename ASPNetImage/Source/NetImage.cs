using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Globalization;
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
    [ProgId("AspNetImage.NetImage")]
    public class NetImage
    {
        // ====================================================================
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public NetImage()
        {
            _dpi = 96;
            _y = 0;
            _x = 0;
            _penWidth = 0;
            _penStyle = 0;
            _penColor = 0;
            _textAngle = 0;
            _brushColor = 0;
            _registeredTo = "Open Source";
            _maxY = 0;
            _maxX = 0;
            ProgressiveJPEGEncoding = false;
            _jpegQuality = 100;
            ImageFormat = ImageFormats.JPEG;
            FontSize = 12;
            FontName = "MS Sans Serif";
            _fontColor = 0;
            _filename = "";
            _error = "";
            ConstrainResize = true;
            _backgroundColor = Color.FromArgb(255, 0, 0, 0).ToArgb(); // Default to black
            Bold = false;
            AutoClear = true;
            AntiAliasText = false;
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
                _image.Dispose();
            }
            catch
            {
            }
        }

        #endregion

        // ====================================================================
        #region Public Enumerations

        public enum BlobTypes : int
        {
            JPEG = 1,
            BMP = 2,
            PNG = 3
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

        public enum PenStyles
        {
            Solid = 0,
            Dash = 1,
            Dot = 2,
            DashDot = 3,
            DashDotDot = 4
            //Clear = 5,
            //InsideFrame = 6
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

        private int _backgroundColor;
        private string _error;
        private string _filename;
        private int _fontColor;
        private int _jpegQuality;
        private int _maxX;
        private int _maxY;
        private Image _image;
        private string _registeredTo;
        private Single _textAngle;
        private int _penColor;
        private int _penStyle;
        private int _penWidth;
        private int _x;
        private int _y;
        private int _dpi;
        private int _brushColor;

        #endregion

        private static uint FlipFirstAndThirdDWORDBytes(int value)
        {
            return (((0xFF & (uint) value) << 0x10) | (0xFF00 & (uint) value) | ((0xFF0000 & (uint) value) >> 0x10));
        }

        public static int VBScriptRGBToDotNETARGB(int value)
        {
            return (int) (FlipFirstAndThirdDWORDBytes(value) | 0xFF000000);
        }

        public static int DotNETARGBToVBScriptRGB(int value)
        {
            return (int) (FlipFirstAndThirdDWORDBytes(value) & 0x00FFFFFF);
        }

        // ====================================================================
        #region Public Properties

        // --------------------------------------------------------------------
        #region Legacy Properties To Be Implemented

        public bool AutoSize = true;
        public int BrushStyle = 0;
        public bool Italic = false;
        public int PadSize = 0;
        public int PixelFormat = 6;
        public bool Strikeout = false;
        public TextAlignments TextAlign = TextAlignments.Left;
        public int ThreeDColor = 0;
        public bool Transparent = false;
        public int TransparentColor = 0;
        public bool TransparentText = true;
        public bool Underline = false;

        #endregion

        /// <summary>
        /// Gets or sets whether anti-aliased text is added on the image
        /// </summary>
        public bool AntiAliasText { get; set; }

        /// <summary>
        /// Gets or sets whether the image should be disposed of from memory after
        /// a successful save to disk.
        /// </summary>
        public bool AutoClear { get; set; }

        /// <summary>
        /// Integer value specifies the background color for NEW image manipulations. 
        /// It does not magically set the "background" to the specified color
        /// </summary>
        public int BackgroundColor
        {
            get => DotNETARGBToVBScriptRGB(_backgroundColor);
            set => _backgroundColor = VBScriptRGBToDotNETARGB(value);
        }

        public int BrushColor 
        {
            get => DotNETARGBToVBScriptRGB(_brushColor);
            set => _brushColor = VBScriptRGBToDotNETARGB(value);
        }

        /// <summary>
        /// True/false value determines if font is bold or not 
        /// </summary>
        public bool Bold { get; set; }

        /// <summary>
        /// Gets or sets whether the resize method should constrain the image's original
        /// aspect ratio and if so, center-crops the image if needed to fit the dimensions
        /// </summary>
        public bool ConstrainResize { get; set; }

        /// <summary>
        /// Set DPI
        /// </summary>
        public int DPI
        {
            get => _dpi;
            set
            {
                _dpi = value;

                if (_image == null)
                {
                    ClearImage();
                }

                var tempImage = new Bitmap(_image);
                try
                {
                    tempImage.SetResolution(value, value);
                    if (Math.Abs(tempImage.HorizontalResolution - tempImage.VerticalResolution) <= 0.0)
                    {
                        _dpi = Convert.ToInt32(tempImage.HorizontalResolution);
                        _image.Dispose();
                        _image = tempImage;
                    }
                }
                catch
                {
                    tempImage.Dispose();
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets the last error string that occurred with the object
        /// </summary>
        public string Error => _error;

        /// <summary>
        /// For compatibility, returns a future date that the component supposedly expires
        /// </summary>
        public string Expires => DateTime.Now.AddYears(50).ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets or sets the full path and filename used by the SaveImage() method
        /// </summary>
        public string Filename
        {
            get => _filename;
            set => _filename = value.Trim();
        }

        /// <summary>
        /// The integer FontColor specifies the color of the font 
        /// </summary>
        public int FontColor
        {
            get => DotNETARGBToVBScriptRGB(_fontColor);
            set => _fontColor = VBScriptRGBToDotNETARGB(value);
        }

        /// <summary>
        /// The string FontName specifies the name of the font
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// The integer FontSize specifies the size of the font
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// Returns the raw binary data for the image.
        /// Replaces "Image" property in original component.
        /// </summary>
        public byte[] Image
        {
            get
            {
                using (var ms = new MemoryStream())
                {
                    switch (ImageFormat)
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
        }

        /// <summary>
        /// Gets or sets the image format to be used when saving the image. 
        /// </summary>
        public ImageFormats ImageFormat { get; set; }

        /// <summary>
        /// Gets or sets the quality percentage for saved JPEG images
        /// </summary>
        public int JPEGQuality
        {
            get => _jpegQuality;
            set
            {
                _jpegQuality = value;

                if (_jpegQuality > 100)
                {
                    _jpegQuality = 100;
                }
                else if (_jpegQuality < 0)
                {
                    _jpegQuality = 0;
                }
            }
        }

        /// <summary>
        /// The MaxX property determines the X size of the image
        /// </summary>
        public int MaxX
        {
            get => _image?.Width ?? 0;
            set
            {
                _maxX = value;
                // create the image if dimensions are already set
                if (_image == null && _maxX > 0 && _maxY > 0)
                {
                    ClearImage();
                }
            }
        }

        /// <summary>
        /// The MaxY property determines the Y size of the image
        /// </summary>
        public int MaxY
        {
            get
            {
                if (_image != null)
                {
                    return _image.Height;
                }
                
                return 0;
            }
            set
            {
                _maxY = value;
                // create the image if dimensions are already set
                if (_image == null && _maxX > 0 && _maxY > 0)
                {
                    ClearImage();
                }
            }
        }

        /// <summary>
        /// Pen color RGB
        /// </summary>
        public int PenColor
        {
            get => DotNETARGBToVBScriptRGB(_penColor);
            set => _penColor = VBScriptRGBToDotNETARGB(value);
        }

        /// <summary>
        /// PenStyle 
        /// Style determines the style in which the pen draws lines.
        /// Value	Type	Description
        /// 0	Solid	A solid line.
        /// 1	Dash	A line made up of a series of dashes.
        /// 2 	Dot	A line made up of a series of dots.
        /// 3	DashDot	A line made up of alternating dashes and dots.
        /// 4	DashDotDot	A line made up of a serious of dash-dot-dot combinations.
        /// </summary>
        public int PenStyle
        {
            get => _penStyle;
            set
            {
                if (value >= 0 && value <= 4)
                {
                    _penStyle = value;
                }
                else
                {
                    _penStyle = 0;
                }
            }
        }

        /// <summary>
        /// PenWidth specifies the maximum width of the pen in pixels.
        /// Example:
        /// Image.PenWidth = 2
        /// </summary>
        public int PenWidth
        {
            get => _penWidth;
            set
            {
                _penWidth = value;

                if (_penWidth < 1)
                {
                    _penWidth = 1;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether progressive JPEG encoding should be used when saving
        /// the image.  Not currently supported.
        /// </summary>
        public bool ProgressiveJPEGEncoding { get; set; }

        /// <summary>
        /// Gets the classes .NET image instance
        /// </summary>
        public Image RawNetImage => _image;

        /// <summary>
        /// Gets or sets who the component is registered to.  Provided for compatibility
        /// </summary>
        public string RegisteredTo
        {
            get => _registeredTo;
            set => _registeredTo = value.Trim();
        }

        /// <summary>
        /// Gets or sets the text angle
        /// </summary>
        public Single TextAngle
        { 
            get => _textAngle;
            set
            {
                _textAngle = value;

                if (_textAngle < 0)
                {
                    _textAngle = 0;
                }
                else if (_textAngle > 360)
                {
                    _textAngle = 360;
                }
            } 
        }

        /// <summary>
        /// Returns the current version number of the component
        /// </summary>
        public string Version => "2.3.1.0 (ASPNetImage v0.3)";

        /// <summary>
        /// Gets or sets the current X coordinate of the pen
        /// </summary>
        public Int32 X
        {
            get => _x;
            set
            {
                _x = value;

                if (value > MaxX)
                {
                    _x = MaxX;
                }
                else if (_x < 0)
                {
                    _x = 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current Y coordinate of the pen
        /// </summary>
        public Int32 Y
        {
            get => _y;
            set
            {
                _y = value;

                if (_y > MaxY)
                {
                    _y = MaxY;
                }
                else if (_y < 0)
                {
                    _y = 0;
                }
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
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

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
        /// <param name="adjustmentValue"></param>
        private void AdjustBrightness(float adjustmentValue)
        {

            if (adjustmentValue < -255)
                adjustmentValue = -255;

            if (adjustmentValue > 255)
                adjustmentValue = 255;

            // Based off James Craig's ColorMatrix blog entry at:
            // http://www.gutgames.com/post/Adjusting-Brightness-of-an-Image-in-C.aspx

            using (var tempImage = new Bitmap(_image))
            {
                float finalValue = adjustmentValue / 255.0f;
                var newBitmap = new Bitmap(tempImage.Width, tempImage.Height);
                try
                {
                    using (Graphics newGraphics = Graphics.FromImage(newBitmap))
                    {

                        float[][] floatColorMatrix =
                        {
                            new float[] {1, 0, 0, 0, 0},
                            new float[] {0, 1, 0, 0, 0},
                            new float[] {0, 0, 1, 0, 0},
                            new float[] {0, 0, 0, 1, 0},
                            new float[] {finalValue, finalValue, finalValue, 1, 1}
                        };
                        var colorMtrx = new ColorMatrix(floatColorMatrix);
                        using (var imageAttr = new ImageAttributes())
                        {
                            imageAttr.SetColorMatrix(colorMtrx);

                            newGraphics.DrawImage(tempImage, new Rectangle(0, 0, tempImage.Width, tempImage.Height), 0,
                                0,
                                tempImage.Width, tempImage.Height, GraphicsUnit.Pixel, imageAttr);

                            // Get rid of the old image data first
                            _image.Dispose();
                            _image = newBitmap;
                        }
                    }
                }
                catch
                {
                    newBitmap.Dispose();
                    throw;
                }
            }
        }

        /// <summary>
        /// Converte un byte array in una System.Drawing.Image
        /// </summary>
        /// <param name="imageIn"></param>
        /// <param name="imgFormat"></param>
        /// <returns></returns>
        private byte[] ImageToByteArray(Image imageIn, ImageFormat imgFormat)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imgFormat);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Converte una System.Drawing.Image in un byte array
        /// </summary>
        /// <param name="byteArrayIn"></param>
        /// <returns></returns>
        private Image ByteArrayToImage(byte[] byteArrayIn)
        {
            using (var ms = new MemoryStream(byteArrayIn))
            {
                Image returnImage = System.Drawing.Image.FromStream(ms);
                return returnImage;
            }
        }

        /// <summary>
        ///Devo fare questo giro perché a volte l'immagine ha un formato particolare e quindi va in eccezione
        ///Eccezione: "Impossibile creare un oggetto Graphics da un'immagine con formato a pixel indicizzato"
        /// </summary>
        /// <param name="image">immagine</param>
        /// <param name="tipo">estensione dell'immagine</param>
        /// <returns></returns>
        private Graphics GetGraphicsImage(Image image, string tipo)
        {
            Graphics gra;

            try
            {
                gra = Graphics.FromImage(image);
            }
            catch (Exception)
            {
                using (var ms = new MemoryStream())
                {
                    switch (tipo.ToLower())
                    {
                        case "gif":
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                            break;
                        case "png":
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        default:
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                    }

                    Image
                        img = System.Drawing.Image
                            .FromStream(
                                ms); // ToDo: What about the lifecycle of the img object? Will the Graphics object gra Dispose() it?
                    gra = Graphics.FromImage(img);
                }
            }

            return gra;
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
            using (var graphics = Graphics.FromImage(_image))
            {
                var brushColor = Color.FromArgb(_backgroundColor);
                using (Brush coloredBrush = new SolidBrush(brushColor))
                {
                    var pen = new Pen(Color.FromArgb(VBScriptRGBToDotNETARGB(_penColor)))
                    {
                        Width = PenWidth,
                        DashStyle = (DashStyle) PenStyle
                    };
                    var rect = new Rectangle(intX1, intY1, intX2 - intX1 + 1, intY2 - intY1 + 1);
                    graphics.DrawEllipse(pen, rect);
                    graphics.FillEllipse(coloredBrush, rect);
                }
            }
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

        public void PolyBezier(object[] aryPoints)
        {
        }

        public void Polygon(object aryPoints)
        {
            using (var graphics = Graphics.FromImage(_image))
            {
                var brushColor = Color.FromArgb(_brushColor);
                using (Brush coloredBrush = new SolidBrush(brushColor))
                {
                    var pen = new Pen(Color.FromArgb(VBScriptRGBToDotNETARGB(_penColor)))
                    {
                        Width = PenWidth,
                        DashStyle = (DashStyle) PenStyle
                    };

                    var points = new Point[((object[,]) aryPoints).Length / 2];
                    for (var i = 0; i < ((object[,]) aryPoints).Length / 2; i++)
                    {
                        points[i].X = Convert.ToInt32(((object[,]) aryPoints)[i, 0]);
                        points[i].Y = Convert.ToInt32(((object[,]) aryPoints)[i, 1]);
                    }

                    graphics.DrawPolygon(pen, points);
                    graphics.FillPolygon(coloredBrush, points);
                }
            }
        }

        public void PolyLine([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType=VarEnum.VT_I4)] int[,] aryPoint)
        {
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
            using (var pen = new Pen(Color.FromArgb(VBScriptRGBToDotNETARGB(intColor)))
            {
                Width = PenWidth,
                DashStyle = (DashStyle) PenStyle
            })
            {
                using (var graphics = Graphics.FromImage(_image))
                {
                    graphics.DrawLine(pen, new Point(intX, intY), new Point(intX, intY));
                }
            }
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
        public bool AddImage(string strFileName, int intX, int intY)
        {
            FileStream fileStream = null;
            using (var graphicsDest = Graphics.FromImage(_image))
            {

                try
                {
                    fileStream = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    Image addedImage = System.Drawing.Image.FromStream(fileStream);
                    graphicsDest.DrawImage(addedImage, intX, intY);
                }
                catch (Exception e)
                {
                    _error = e.ToString();
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                        fileStream.Dispose();
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Adds a new image to the canvas using the intX and intY coordinates passing an Image
        /// <param name="imageToAdd"></param>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        /// </summary>
        public bool AddImage(byte[] imageToAdd, int intX, int intY)
        {
            var graphicsDest = Graphics.FromImage(_image);
            try
            {
                graphicsDest.DrawImage(ByteArrayToImage(imageToAdd), intX, intY);
            }
            catch (Exception e)
            {
                _error = e.ToString();
            }
            finally
            {
                graphicsDest.Dispose();
            }

            return false;
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

            AdjustBrightness(255f * ((float)percentage / 100f));
        }

        /// <summary>
        /// Clears the entire image with the current BackgroundColor
        /// </summary>
        public void ClearImage()
        {
            Graphics graphicsDest = null;

            if (_image == null)
            {
                if (_maxX <= 0)
                {
                    _maxX = 1;
                }

                if (_maxY <= 0)
                {
                    _maxY = 1;
                }

                var bitmapDest = new Bitmap(_maxX, _maxY);
                _image = bitmapDest;
            }

            using (graphicsDest = Graphics.FromImage(_image))
            {
                graphicsDest.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                var brushColor = Color.FromArgb(_backgroundColor);
                using (Brush coloredBrush = new SolidBrush(brushColor))
                {
                    graphicsDest.FillRectangle(coloredBrush, 0, 0, _image.Width, _image.Height);
                    graphicsDest.DrawImage(_image, 0, 0);
                }
            }
        }

        /// <summary>
        /// Modifies the image contrast. intDegree should be -100 to 100
        /// </summary>
        /// <param name="intDegree"></param>
        public void Contrast(int intDegree)
        {
            if (intDegree < -100)
                intDegree = -100;
            if (intDegree > 100)
                intDegree = 100;
            float factor = (float)Math.Pow((100.0 + intDegree) / 100.0, 2.0);

            using (var graphicsDest = Graphics.FromImage(_image))
            {
                using (var imageAttr = new ImageAttributes())
                {
                    var colorMtrx = new ColorMatrix(new float[][]
                    {
                        new float[] {factor, 0f, 0f, 0f, 0f},
                        new float[] {0f, factor, 0f, 0f, 0f},
                        new float[] {0f, 0f, factor, 0f, 0f},
                        new float[] {0f, 0f, 0f, 1f, 0f},
                        new float[] {0.001f, 0.001f, 0.001f, 0f, 1f}
                    });
                    imageAttr.SetColorMatrix(colorMtrx);
                    graphicsDest.DrawImage(_image, new Rectangle(0, 0, _image.Width, _image.Height), 0, 0, _image.Width,
                        _image.Height, GraphicsUnit.Pixel, imageAttr);
                }
            }
        }

        /// <summary>
        /// Turns the current image into a Gray Scale image
        /// </summary>
        public void CreateGrayScale()
        {
            using (Graphics graphicsDest = Graphics.FromImage(_image))
            {

                using (var imageAttr = new ImageAttributes())
                {
                    var colorMtrx = new ColorMatrix(new float[][]
                    {
                        new float[] {0.299f, 0.299f, 0.299f, 0, 0},
                        new float[] {0.587f, 0.587f, 0.587f, 0, 0},
                        new float[] {0.114f, 0.114f, 0.114f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 0}
                    });

                    imageAttr.SetColorMatrix(colorMtrx);
                    graphicsDest.DrawImage(_image, new Rectangle(0, 0, _image.Width, _image.Height), 0, 0, _image.Width,
                        _image.Height, GraphicsUnit.Pixel, imageAttr);
                }
            }
        }

        /// <summary>
        /// Creates a negative image effect of the current image
        /// </summary>
        public void CreateNegative()
        {
            using (Graphics graphicsDest = Graphics.FromImage(this._image))
            {

                using (var imageAttr = new ImageAttributes())
                {
                    var colorMtrx = new ColorMatrix(new float[][]
                    {
                        new float[] {-1, 0, 0, 0, 0},
                        new float[] {0, -1, 0, 0, 0},
                        new float[] {0, 0, -1, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {1, 1, 1, 0, 1}
                    });
                    imageAttr.SetColorMatrix(colorMtrx);
                    graphicsDest.DrawImage(_image, new Rectangle(0, 0, _image.Width, _image.Height), 0, 0, _image.Width,
                        _image.Height, GraphicsUnit.Pixel, imageAttr);
                }
            }
        }

        /// <summary>
        /// Crops the image with the specified dimensions
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void CropImage(int startX, int startY, int width, int height)
        {
            var croppedImage = new Bitmap(width, height);
            try
            {
                using (Graphics graphicsCrop = Graphics.FromImage(croppedImage))
                {
                    graphicsCrop.Clear(Color.FromArgb(_backgroundColor));
                    var recDest = new Rectangle(0, 0, width, height);
                    graphicsCrop.DrawImage(_image, recDest, startX, startY, width, height, GraphicsUnit.Pixel);

                    _image.Dispose();
                    _image = croppedImage;
                }
            }
            catch
            {
                croppedImage.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Darkens the image by the specified percentage
        /// </summary>
        /// <param name="percentage"></param>
        public void DarkenImage(int percentage)
        {
            if (percentage <= 0)
                percentage = 1;

            if (percentage > 100)
                percentage = 100;

            AdjustBrightness(-(255f * ((float)percentage / 100f)));
        }

        /// <summary>
        /// Gives the current image an embossed look
        /// </summary>
        public void Emboss()
        {
            using (Graphics graphicsDest = Graphics.FromImage(_image))
            {
                using (var imgTemp = new Bitmap(_image))
                {
                    imgTemp.SetResolution(_image.HorizontalResolution, _image.VerticalResolution);
                    var rect = new Rectangle(0, 0, imgTemp.Width, imgTemp.Height);
                    BitmapData bmpData = imgTemp.LockBits(rect, ImageLockMode.ReadWrite, _image.PixelFormat);
                    IntPtr ptr = bmpData.Scan0;
                    int numBytes = imgTemp.Width * imgTemp.Height * 3;
                    int rowLenght = imgTemp.Width * 3;
                    var rgbValues = new byte[numBytes];
                    Marshal.Copy(ptr, rgbValues, 0, numBytes);

                    for (int i = 0; i < rgbValues.Length; i += 3)
                    {
                        if (i < rgbValues.Length - rowLenght - 3)
                        {
                            byte b = (byte) (rgbValues[i] - rgbValues[rowLenght + i + 3] + (byte) 128);
                            rgbValues[i] = (byte) Math.Min((byte) Math.Abs(b), (byte) 255);

                            b = (byte) (rgbValues[i + 1] - rgbValues[rowLenght + i + 4] + (byte) 128);
                            rgbValues[i + 1] = (byte) Math.Min((byte) Math.Abs(b), (byte) 255);

                            b = (byte) (rgbValues[i + 2] - rgbValues[rowLenght + i + 5] + (byte) 128);
                            rgbValues[i + 2] = (byte) Math.Min((byte) Math.Abs(b), (byte) 255);
                        }
                        else
                        {
                            rgbValues[i] = rgbValues[i + 1] = rgbValues[i + 2] = 128;
                        }
                    }

                    Marshal.Copy(rgbValues, 0, ptr, numBytes);
                    imgTemp.UnlockBits(bmpData);
                    graphicsDest.DrawImage(imgTemp, 0, 0);
                }
            }
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
            using (Graphics graphicsDest = Graphics.FromImage(_image))
            {
                graphicsDest.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                Color brushColor = Color.FromArgb(_backgroundColor);
                using (Brush coloredBrush = new SolidBrush(brushColor))
                {
                    graphicsDest.FillRectangle(coloredBrush, intLeft, intTop, intRight - intLeft + 1,
                        intBottom - intTop + 1);
                    graphicsDest.DrawImage(_image, 0, 0);
                }
            }
        }

        /// <summary>
        /// Flips the image vertically (intDirection == 2) or horizontally (intDirection == 1)
        /// </summary>
        /// <param name="direction"></param>
        public void FlipImage(FlipDirections direction)
        {
            if (direction == FlipDirections.Horizontal)
            {
                _image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            else if (direction == FlipDirections.Vertical)
            {
                _image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }
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

            using (var thisImage = System.Drawing.Image.FromFile(filePath))
            {

                if (thisImage != null)
                {
                    width = thisImage.Width;
                    height = thisImage.Height;
                }
            }
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

            if (_image != null)
            {
                width = _image.Width;
                height = _image.Height;
            }
        }

        /// <summary>
        /// Returns the color value (argb) for the pixel at the specified x and y coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int GetPixel(int x, int y)
        {
            if (_image != null && x < _image.Width && y <= _image.Height)
            {
                return DotNETARGBToVBScriptRGB(((Bitmap)_image).GetPixel(x, y).ToArgb());
            }

            return 0;
        }

        /// <summary>
        /// Draws a line from the current pen coordinates to the specified destination with the current pen color and width
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void LineTo(int x, int y)
        {
            using (Graphics graphicsDest = Graphics.FromImage(_image))
            {

                var pen = new Pen(Color.FromArgb(_penColor))
                {
                    Width = PenWidth,
                    DashStyle = (DashStyle) PenStyle
                };

                graphicsDest.DrawLine(pen, X, Y, x, y);

                // Update pen coordinates
                _x = x;
                _y = y;
            }
        }

        /// <summary>
        /// LoadBlob is designed to allow the loading of binary image data from other AspImage objects 
        /// (using the .Image property for ovBlob) or from other data sources where binary image data 
        /// is available via an OLE variant pointer. ovBlob is an OLE variant pointing to raw image data. 
        /// The raw image data is loaded onto the AspImage canvas.
        /// The parameter intType indicates what type of format the binary data is in. Valid intTypes are:
        ///     1: JPEG 
        ///     2: BMP 
        ///     3: PNG 
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
                _image = System.Drawing.Image.FromStream(ms);

                if (_image != null)
                {
                    // GIMP thumbnails specifically cause System.Net.Image.Save() to fail and must be removed
                    // NET removes embedded thumbnails when the image is rotated, so we can easily strip them
                    // by flipping the image once, and then back again
                    FlipImage(FlipDirections.Horizontal);
                    FlipImage(FlipDirections.Horizontal);
                    if (intType == 1)
                        ImageFormat = ImageFormats.JPEG;
                    if (intType == 2)
                        ImageFormat = ImageFormats.BMP;
                    if (intType == 3)
                        ImageFormat = ImageFormats.PNG;

                    return true;
                }
                else
                    _error = "Unknown error loading image";

            }
            catch (Exception e)
            {
                _error = e.ToString();
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
        public bool LoadImage(string imageFilePath)
        {
            FileStream fileStream = null;

            try
            {

                // We're not using System.Drawing.Image.FromFile() here because it locks the file
                // until the object is disposed and we may need to save and/or manipulate the
                // raw file in the meantime.

                fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                _image = System.Drawing.Image.FromStream(fileStream);

                if (_image != null)
                {
                    // GIMP thumbnails specifically cause System.Net.Image.Save() to fail and must be removed
                    // NET removes embedded thumbnails when the image is rotated, so we can easily strip them
                    // by flipping the image once, and then back again
                    FlipImage(FlipDirections.Horizontal);
                    FlipImage(FlipDirections.Horizontal);

                    return true;
                }
                else
                    _error = "Unknown error loading image";

            }
            catch (Exception e)
            {
                _error = e.ToString();
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
        /// Draws a rectangle with the coordinates of the upper-left and lower-right coordinates provided.
        /// </summary>
        /// <param name="intX1"></param>
        /// <param name="intY1"></param>
        /// <param name="intX2"></param>
        /// <param name="intY2"></param>
        public void Rectangle(int intX1, int intY1, int intX2, int intY2)
        {
            using (Graphics graphicsDest = GetGraphicsImage(_image, ImageFormat.ToString()))
            {

                var pen = new Pen(Color.FromArgb(_penColor))
                {
                    Width = PenWidth,
                    DashStyle = (DashStyle) PenStyle
                };

                graphicsDest.DrawRectangle(pen, intX1, intY1, intX2 - intX1 + 1, intY2 - intY1 + 1);
                FillRect(intX1, intY1, intX2, intY2);
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
            if (_image.Width != width || _image.Height != height)
            {
                int originalWidth = _image.Width;
                int originalHeight = _image.Height;
                int newWidth = width;
                int newHeight = height;

                Bitmap resizedImage;
                Graphics graphicsDest = null;

                // Constraining the proportions of the image will resize the image
                // maintaining the same aspect ratio and crop as needed
                if (ConstrainResize)
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

                try
                {
                    // Check again since we may have changed our scale dimensions and only 
                    // rescale the image if needed.
                    if (originalWidth != newWidth || originalHeight != newHeight)
                    {
                        resizedImage = new Bitmap(newWidth, newHeight);
                        try
                        {
                            graphicsDest = Graphics.FromImage(resizedImage);
                            graphicsDest.DrawImage(_image, 0, 0, newWidth + 1, newHeight + 1);

                            _image.Dispose();
                            _image = resizedImage;
                        }
                        catch
                        {
                            resizedImage.Dispose();
                            throw;
                        }
                    }

                    // Crop the image to the final size if necessary
                    if ((newHeight > height) || (newWidth > width))
                    {
                        // Use the center of the image as our basis for cropping
                        int cropY = (newHeight == height ? 0 : Convert.ToInt32((newHeight - height) / 2));
                        int cropX = (newWidth == width ? 0 : Convert.ToInt32((newWidth - width) / 2));

                        CropImage(cropX, cropY, width, height);
                    }
                }
                catch
                {
                    if (graphicsDest != null)
                        graphicsDest.Dispose();
                }
            }
        }

        /// <summary>
        /// Resize proportionally based on height
        /// </summary>
        /// <param name="height"></param>
        public void ResizeOnHeight(int height) {
            int width = (int)Math.Round(((double)height / (double)this.MaxY) * (double)this.MaxX);
            Resize(width, height);
        }

        /// <summary>
        /// Resize proportionally based on width
        /// </summary>
        /// <param name="width"></param>
        public void ResizeOnWidth(int width) {
            int height = (int)Math.Round(((double)width / (double)this.MaxX) * (double)this.MaxY);
            Resize(width, height);
        }

        /// <summary>
        /// Resizes the image using a resampling to produce better quality. Not implemented, but calls
        /// Resize() method.
        /// </summary>
        /// <param name="intWidth"></param>
        /// <param name="intHeigh"></param>
        public void ResizeR(int intWidth, int intHeigh)
        {
            Resize(intWidth, intHeigh);
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
                        _image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;

                    case 180:
                        _image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;

                    case 270:
                        _image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
            }
        }

        /// <summary>
        /// Saves the image to the file path specified by the object's Filename property.  Note
        /// that at this time, only JPG format is supported for output.
        /// </summary>
        /// <returns></returns>
        public bool SaveImage()
        {
            if (Filename.Length > 0)
            {
                ImageCodecInfo imageCodecInfo;
                Encoder imageEncoder;
                EncoderParameter imageEncoderParameter;
                EncoderParameters imageEncoderParameters;

                switch (ImageFormat)
                {
                    case ImageFormats.BMP:

                        Filename = Path.ChangeExtension(Filename, "bmp");

                        try
                        {
                            _image.Save(Filename, System.Drawing.Imaging.ImageFormat.Bmp);
                        }
                        catch (Exception e)
                        {
                            _error = e.ToString();
                        }
                        break;

                    case ImageFormats.GIF:

                        Filename = Path.ChangeExtension(Filename, "gif");

                        try
                        {
                            _image.Save(Filename, System.Drawing.Imaging.ImageFormat.Gif);
                        }
                        catch (Exception e)
                        {
                            _error = e.ToString();
                        }
                        break;

                    case ImageFormats.PNG:

                        Filename = Path.ChangeExtension(Filename, "png");

                        try
                        {
                            _image.Save(Filename, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        catch (Exception e)
                        {
                            _error = e.ToString();
                        }

                        break;

                    case ImageFormats.JPEG:
                    default:

                        imageCodecInfo = GetEncoderInfo("image/jpeg");
                        imageEncoder = Encoder.Quality;
                        imageEncoderParameters = new EncoderParameters(1);
                        imageEncoderParameter = new EncoderParameter(imageEncoder, JPEGQuality);
                        imageEncoderParameters.Param[0] = imageEncoderParameter;

                        Filename = Path.ChangeExtension(Filename, "jpg");

                        try
                        {
                            _image.Save(Filename, imageCodecInfo, imageEncoderParameters);
                        }
                        catch (Exception e)
                        {
                            _error = e.ToString();
                        }

                        break;
                }

                try
                {
                    if (File.Exists(Filename))
                    {
                        if (AutoClear)
                            _image.Dispose();

                        return true;
                    }
                    
                    this._error = "Unknown error saving image";
                }
                catch (Exception e)
                {
                    _error = e.ToString();
                }
            }
            else
                _error = "Filename property not set";

            return false;
        }

        /// <summary>
        /// Returns the text height for strValue using the current font, font size and font characteristics
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public int TextHeight(string strValue)
        {
            using (Graphics graphicsDest = Graphics.FromImage(_image))
            {
                using (var font = new Font(FontName, Convert.ToSingle(FontSize)))
                {
                    SizeF size = graphicsDest.MeasureString(strValue, font);
                    return Convert.ToInt32(size.Height);
                }
            }
        }

        /// <summary>
        /// TextOut writes a text value using the current font, color, angle and other 
        /// characteristics to the image at the location specified by intX and intY. 
        /// If bol3d is true then the text is rendered using a 3D look
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        /// <param name="bol3D"></param>
        public void TextOut(string strText, int intX, int intY, bool bol3D)
        {
            using (Graphics graphicsDest = Graphics.FromImage(_image))
            {
                if (AntiAliasText)
                    graphicsDest.TextRenderingHint = TextRenderingHint.AntiAlias;

                using (var font = new Font(FontName, Convert.ToSingle(FontSize)))
                {
                    Color color = Color.FromArgb(_fontColor);
                    using (Brush brush = new SolidBrush(color))
                    {
                        if (TextAngle == 0)
                        {
                            graphicsDest.DrawString(strText, font, brush, new Point(intX, intY));
                        }
                        else
                        {
                            // reduce angle to 0°-360° range 
                            Single angle = TextAngle;
                            if (angle > 360)
                            {
                                while (angle > 360)
                                {
                                    angle = angle - 360;
                                }
                            }
                            else if (angle < 0)
                            {
                                while (angle < 0)
                                {
                                    angle = angle + 360;
                                }
                            }

                            // save graphics state
                            GraphicsState state = graphicsDest.Save();
                            // clear any traformations already in progress
                            graphicsDest.ResetTransform();
                            // rotate
                            graphicsDest.RotateTransform(angle);
                            // translate origin to compensate rotation
                            graphicsDest.TranslateTransform(intX, intY, MatrixOrder.Append);
                            // write text
                            graphicsDest.DrawString(strText, font, brush, 0, 0);
                            // restore graphic state
                            graphicsDest.Restore(state);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the text width for strValue using the current font, font size and font characteristics
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public int TextWidth(string strValue)
        {
            using (Graphics graphicsDest = Graphics.FromImage(_image))
            {
                using (var font = new Font(FontName, Convert.ToSingle(FontSize)))
                {
                    SizeF size = graphicsDest.MeasureString(strValue, font);
                    return Convert.ToInt32(size.Width);
                }
            }
        }

        #endregion

    }
}
