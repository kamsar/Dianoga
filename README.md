# Dianoga

An automatic image optimizer for the Sitecore media library.

When media images are requested, Dianoga automatically runs [jpegtran](http://jpegclub.org/jpegtran/) or [PNGOptimizer](http://psydk.org/pngoptimizer) on the media image before it is placed in the Sitecore media cache. This makes sure your site is always serving fully optimised media library images even if you are using Sitecore's dynamic resizing features (for example with [Adaptive Images](https://marketplace.sitecore.net/en/Modules/Sitecore_Adaptive_Images.aspx)) which result in multiple versions of the image in the cache - and the resized ones are not fully optimized even if the original was.

Dianoga is also great for situations where content editors may not be image editing experts and upload images that contain gobs of EXIF data and other nonessentials - these are removed automatically before being shown to visitors. All of the optimizations done are lossless (no quantizing, etc) so you lose no image quality.

In some basic testing, Dianoga was able to losslessly reduce the size of media images served anywhere from 4-88% (usually more like 8-20%). The largest gains are usually with PNGs.

## Performance

There is an initial performance hit when the image is first generated and placed in the media cache. The performance of Dianoga is logged into the Sitecore log. PNGs are very fast because it's a native P/Invoke call to a C DLL - about 20ms for a small one. JPEGs result in executing a program and are slightly slower - around 200ms for a medium sized image.

Once the image is in cache for the requested size, there is no performance penalty as the file is read from the media cache unless it is changed again.

## Limitations

Because Dianoga uses a DLL version of PNGOptimizer, it is platform-specific and only runs on 64-bit application pools.

Dianoga depends on the Dianoga Tools folder that is installed by the NuGet package into the web project it is installed on. Do not rename or delete this folder, or it will not work.

## Installation

Dianoga has a NuGet package for Sitecore 7 and later (.NET 4.5). Just install it and you're done.

The code should compile against Sitecore 6.x without issue.

To perform a manual installation:

* Copy the Dianoga Tools folder to the root of your website
* Copy Dianoga.config to App_Config\Include
* Reference Dianoga.dll or the source project in your web project

## Troubleshooting

If you're not seeing optimization take place, there are a couple of possibilities:

* The image might already be in the media cache (by default, `/App_Data/MediaCache`). Sitecore does not reprocess images in cache until needed, so Dianoga may never be called. Delete this folder, clear your browser cache or use ctrl-F5, and try again.
* An error is occurring. The Sitecore logs catch all errors that occur when generating a media stream, so look there first. If an error occurs, the result of the pipeline is thrown away and the unmodified stream returned so you may not see broken images if this occurs, just oddly sized - if resizing - or unoptimized ones.

This software is based in part on the work of the Independent JPEG Group