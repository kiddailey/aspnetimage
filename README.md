# ASPNetImage

A COM Interop wrapper written in C# for the .NET 2.0 System.Drawing namespace meant as a drop-in replacement for the third-party ASPImage component by ServerObjects Inc.

While attemting to perform a web server update to Windows Server 2008 for a classic ASP app, I discovered that the long-used ASPImage 2.x COM object no longer functions, exiting with a memory error.  Research into the issue for a solution unearthed a handful of reports from people experiencing the same issue and no luck with customer support or locating an inexpensive, drop-in alternative.  Thus this replacement was born :)

The ASPNetImage component essentially exposes part of the System.Drawing namespace to classic ASP via an interop COM object using the interface outlined by ServerObjects' ASPImage 2.x component.  Switching from ASPImage to ASPNetImage requires you to only register the new DLL and change the object reference in Server.CreateObject() calls in your code.  No other changes are required.

A debug and release build of the DLL is included in the repository, or you can compile it yourself after generating a signing key.  Checkout a working copy of the source repository or you may download the current release in the Downloads section.


# Implementation Details
It is a work in progress and any help completing the object is appreciated. All properties and methods are stubbed out, but as of this writing, currently only a small subset of properties and methods are implemented either in part or whole.


**Fully Implemented**

*Properties:*

  * AutoClear
  * Expires - Always returns future date
  * Filename
  * JPEGQuality
  * RegisteredTo - Returns "This Organization" by default, but can be set to any string
  * Version - Returns last known ASPImage version with "(ASPNetImage)" appended

*Methods:*

  * BrightenImage
  * ClearImage
  * CropImage
  * DarkenImage
  * FillRect
  * FlipImage
  * GetImageFileSize
  * LoadImage
  * Resize


**Partially Implemented**

*Properties:*

  * Error - Returns the error from LoadImage and SaveImage methods only.

  * ImageFormat - Only JPEG, BMP, GIF and PNG formats are supported for output

*Methods:*

  * ResizeR - Calls the Resize method.

  * RotateImage - Only rotates at 90, 180 and 270 degrees clockwise

  * SaveImage - Saves the image, but currently only in JPEG format.


**Additional Properties And Methods**

  * ConstrainResize - When set to true, automatically crops the image during a resize to fit the dimensions and retain the original image's aspect ratio

  * GetImageSize - Returns the image dimensions for the currently loaded image
