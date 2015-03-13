# ASPNetImage

A COM Interop wrapper written in C# for the .NET 2.0 System.Drawing namespace meant as a drop-in replacement for the third-party ASPImage component by ServerObjects Inc.

While attemting to perform a web server update to Windows Server 2008 for a classic ASP app, I discovered that the long-used ASPImage 2.x COM object no longer functions, exiting with a memory error.  Research into the issue for a solution unearthed a handful of reports from people experiencing the same issue and no luck with customer support or locating an inexpensive, drop-in alternative.  Thus this replacement was born :)

The ASPNetImage component essentially exposes part of the System.Drawing namespace to classic ASP via an interop COM object using the interface outlined by ServerObjects' ASPImage 2.x component.  Switching from ASPImage to ASPNetImage requires you to only register the new DLL and change the object reference in Server.CreateObject() calls in your code.  No other changes are required.

A debug and release build of the DLL is included in the repository, or you can compile it yourself after generating a signing key.  Checkout a working copy of the source repository or you may download the current release in the Downloads section.


# Properties and Methods
ASPNetImage is a work in progress and any help completing the object is appreciated. All properties and methods are stubbed out, but as of this writing, currently only a small subset of properties and methods are implemented either in part or whole.


## Properties

   * *AutoClear*:boolean<br />If true, class will automatically dispose image data when the SaveImage method is called.  Default is true.

   * *ConstrainResize*:boolean<br />When set to true, automatically crops the image during a resize to fit the dimensions and retain the original image's aspect ratio.  Default is true. _New with ASPNetImage._

   * *Expires*:DateTime<br />Returns the date that the component expires. Included for compatibility. Always returns future date

   * *Error*:string<br />_Partially implemented._ Returns the last error from LoadImage and SaveImage methods only.

   * *Filename*:string<br />Gets or sets the full path and filename to be used by the SaveImage method

   * *ImageFormat*:int<br />_Partially implemented._ Gets or sets the image format to be used by the SaveImage method.  Currently only JPEG format is supported for output.  Enumerated.  See ASPNetImage.ImageFormats.

   * *JPEGQuality*:int<br />Gets or sets the output quality percentage of JPEG images when SaveImage is called to save a JPEG. Valid amounts are 1 to 100.

   * *RegisteredTo*:string<br />Gets or sets the name that the component is registered to.  Included for compatibility. Returns "This Organization" by default, but can be set to any string

   * *Version*:string<br />Returns last known ASPImage version with "(ASPNetImage)" appended


## Methods

  * *BrightenImage*<br />Increases the brightness of the image

  * *ClearImage*<br />Clears the image

  * *CropImage*<br />Crops the image

  * *DarkenImage*<br />Darkens the image

  * *FillRect*<br />Fills a rectangle area of the image with a given color.

  * *FlipImage*<br />Flips the image

  * *GetImageFileSize*<br />Returns the image dimensions for the image specified by full file path and name. Currently incurs memory hit as image has to be loaded to determine dimensions.

  * *GetImageSize*<br />Returns the image dimensions for the currently loaded image. _New with ASPNetImage._

  * *LoadImage*<br />Loads an image from disk at the specified filepath

  * *Resize*<br />Resizes the image

  * *ResizeR*<br />_Partially implemented._ Calls the Resize method.

  * *RotateImage*<br />_Partially implemented._ Rotates the image.  Currently only rotates at 90, 180 and 270 degrees clockwise

  * *SaveImage*<br />_Partially implemented._ Saves the image, but currently only in JPEG format.


# Troubleshooting

**Troubleshooting Installation Issues**

* Verify that the .NET Framework v2.0 is installed and working properly.

* Ensure that you've registered the DLL with REGASM.EXE with the /tlb and /codebase command line switches (see the included register.bat file).  You may want to unregister the DLL and re-register them to confirm.

* Make sure the DLL and TLB files have the correct file permissions.  Give read/write/execute access to NETWORK, NETWORK_SERVICE and the IUSER_[machineName] accounts.

If you are using an unsigned DLL, you will receive the following error message when registering the DLL with REGASM.EXE:

> RegAsm : warning RA0000 : Registering an unsigned assembly with /codebase can cause your assembly to interfere with other applications that may be installed on the same computer. The /codebase switch is intended to be used only with signed assemblies. Please give your assembly a strong name and re-register it.""

The object should still work, but I'd recommend unregistering it, compiling and signing the DLL yourself before registering it again so that you don't receive the warning message. Visual Studio 2005 and up can do this automatically from the Project Properties panel on the "Signing" tab. Just pick "< New >" and follow the prompts.
