# Dianoga

An automatic image optimizer for the Sitecore media library. Reduce the size of your images served from Sitecore by 8-40%, completely automatically.

When media images are requested, Dianoga automatically runs [jpegtran](http://jpegclub.org/jpegtran/) or [PNGOptimizer](http://psydk.org/pngoptimizer) on the image data immediately after it is placed in the Sitecore media cache. 

Dianoga ensures that your site is always serving fully optimised media library images even if you are using Sitecore's dynamic resizing features (for example with [Adaptive Images](https://marketplace.sitecore.net/en/Modules/Sitecore_Adaptive_Images.aspx)). Even if you have already optimized every image uploaded into the media library, after Sitecore performs image processing the result is _not_ optimized (an unfortunate limitation of most other image optimization libraries for Sitecore is that they only apply to the original image).

Dianoga is also great for situations where content editors may not be image editing experts and upload images that contain gobs of EXIF data and other nonessential metadata - these are removed automatically before being shown to visitors. All of the optimizations done are lossless (no quantizing, etc) so you lose no image quality.

## Performance

Dianoga runs asynchronously _after_ the image is saved into the media cache. This means it has practically no effect on the site's frontend performance (though it does use some CPU time in the background). The performance of Dianoga is logged into the Sitecore log. PNGs are very fast because it's a native P/Invoke call to a C DLL - about 20ms for a small one. JPEGs result in executing a program and are slightly slower - around 200ms for a medium sized image. Larger images several MB in size can take a second or two depending on the hardware in use.

Once the image is in cache for the requested size, there is no performance penalty as the file is read from the media cache unless it is changed again.

Note that because the cache is updated after the first image request, the _first_ hit to images will result in the unoptimized version being served, to preserve the responsiveness of the request. Subsequent requests will receive the optimized version from cache.

## Limitations

Because Dianoga uses a DLL version of PNGOptimizer, it is platform-specific and only runs on 64-bit application pools.

Dianoga depends on the Dianoga Tools folder that is installed by the NuGet package into the web project it is installed on. You can relocate these tools if you wish by changing the paths in the `App_Config/Include/Dianoga.config` file.

## Installation

Dianoga has a NuGet package for Sitecore 7 and 8 (.NET 4.5). Just install it and you're done.

The code should compile against Sitecore 6.x without issue.

To perform a manual installation:

* Copy the Dianoga Tools folder to the root of your website
* Copy Dianoga.config to App_Config\Include
* Reference Dianoga.dll or the source project in your web project

## Upgrade

Upgrading from Dianoga 1.0 to 2.0 is fairly simple; upgrade the NuGet package. You can choose to overwrite your Dianoga.config file (recommended), or merge it with the latest in GitHub if you don't want to.

## Troubleshooting

If you're not seeing optimization take place, there are a couple of possibilities:

* The image might already be in the media cache (by default, `/App_Data/MediaCache`). Sitecore does not reprocess images in cache until needed, so Dianoga may never be called. Delete this folder, clear your browser cache or use ctrl-F5, and try again.
* An error is occurring. The Sitecore logs catch all errors that occur when generating a media stream, so look there first. If an error occurs, the result of the pipeline is thrown away and the unmodified stream returned so you may not see broken images if this occurs, just oddly sized - if resizing - or unoptimized ones.

## Extending Dianoga

You can define your own optimizers for additional media types if you wish, in `App_Config/Include/Dianoga.config`

This software is based in part on the work of the Independent JPEG Group