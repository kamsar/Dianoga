# Dianoga

An automatic image optimizer for the Sitecore media library. Reduce the size of your images served from Sitecore by 8-70%, completely automatically.

When media images are requested, Dianoga automatically runs [mozjpeg](https://github.com/mozilla/mozjpeg), [PNGOptimizer](http://psydk.org/pngoptimizer), [nQuant](https://nquant.codeplex.com/), or [SVGO](https://github.com/svg/svgo) on the image data immediately after it is placed in the Sitecore media cache. 

Dianoga ensures that your site is always serving fully optimised media library images even if you are using Sitecore's dynamic resizing features (for example with [Adaptive Images](https://marketplace.sitecore.net/en/Modules/Sitecore_Adaptive_Images.aspx)). Even if you have already optimized every image uploaded into the media library, after Sitecore performs image processing the result is _not_ optimized (an unfortunate limitation of most other image optimization libraries for Sitecore is that they only apply to the original image).

Dianoga is also great for situations where content editors may not be image editing experts and upload images that contain gobs of EXIF data and other nonessential metadata - these are removed automatically before being shown to visitors. All of the optimizations done are lossless (no quantizing, etc) so you lose no image quality.

## Format Support

Dianoga supports:
* JPEGs (via mozjpeg - lossless optimization)
* PNGs (via PNGOptimizer - lossless, and/or nQuant - lossy)
* SVGs (via SVGO - lossless, and automatic gzipping of SVG media responses)

Additional format support is possible to add via new processors in the `dianogaOptimize` pipeline.

## Performance

By default, Dianoga runs asynchronously _after_ the image is saved into the media cache. This means it has practically no effect on the site's frontend performance (though it does use some CPU time in the background). The performance of Dianoga is logged into the Sitecore log. PNGs are very fast because it's a native P/Invoke call to a C DLL - about 20ms for a small one. JPEGs result in executing a program and are slightly slower - around 200ms for a medium sized image. Larger images several MB in size can take a second or two depending on the hardware in use.

Once the image is in cache for the requested size, there is no performance penalty as the file is read from the media cache unless it is changed again.

Note that because the cache is updated after the first image request, the _first_ hit to images will result in the unoptimized version being served, to preserve the responsiveness of the request. Subsequent requests will receive the optimized version from cache.

## Limitations

Because Dianoga uses a DLL version of PNGOptimizer, it is platform-specific and only runs on 64-bit application pools. You can still use nQuant for PNGs on 32-bit pools.

Dianoga depends on the Dianoga Tools folder that is installed by the NuGet package into the web project it is installed on. You can relocate these tools if you wish by changing the paths in the `App_Config/Include/Dianoga/(Dianoga.Jpeg.config|Dianoga.Png.config|Dianoga.Svg.config)` files.

## Options

In Dianoga 3, you can choose your optimization mode. Most people if serving media locally are best off with the defaults, which optimize the media as it is placed into the media cache. This results in minimal impact to the experience for front end visitors.

However, for folks using CDNs those sources must be sent the optimized version of the media immediately. For these situations, you can enable `Dianoga.Strategy.GetMediaStreamSync.config.disabled` and disable `Dianoga.Strategy.MediaCacheAsync.config`. This will cause optimization to occur synchronously as the media is first requested, which is appropriate if the media is being sent to a CDN. This may however cause a delay for the first hit user before they start seeing images.

If you want to execute the Dianoga optimization pipeline programmatically (e.g. as part of a CDN upload background process), you can use the `MediaOptimizer` class to execute optimization on any Sitecore `MediaStream` object at will.

## Installation

Dianoga has a NuGet package for Sitecore 7 and 8 (.NET 4.5). Just install it, __clear your App_Data/MediaCache folder__, and you're done.

The code should compile against Sitecore 6.x without issue.

To perform a manual installation:

* Copy the `Dianoga Tools` folder to the `App_Data` folder of your website
* Copy `Default Config Files/*.config` to `App_Config\Include\Dianoga`
* Reference `Dianoga.dll` or the source project in your web project

## Upgrade

Upgrading from Dianoga 2.0 to 3.0 is fairly simple.

1. Upgrade the NuGet package.
2. Remove any `App_Config\Include\Dianoga.config` that may exist (config has changed, you must reapply settings if changed in the new scheme)
3. Remove `Dianoga Tools` if it exists (Dianoga 3 places tools under App_Data\Dianoga Tools instead so that IIS defaults to not serving anything there)
4. Delete your MediaCache folder (defaults to App_Data/MediaCache) on all servers after deployment of Dianoga (otherwise unoptimized caches will remain present)
5. Have fun.

## Troubleshooting

If you're not seeing optimization take place, there are a couple of possibilities:

* The image might already be in the media cache (by default, `/App_Data/MediaCache`). Sitecore does not reprocess images in cache until needed, so Dianoga may never be called. Delete this folder, clear your browser cache or use ctrl-F5, and try again.
* An error is occurring. The Sitecore logs catch all errors that occur when generating a media stream, so look there first. If an error occurs, the result of the pipeline is thrown away and the unmodified stream returned so you may not see broken images if this occurs, just oddly sized - if resizing - or unoptimized ones.

## Extending Dianoga

You can optimize any type of image that the media library supports with Dianoga.

Adding a new `Processor` to the config will enable you to call a micro-pipeline you define to process that type.

In your `Optimizer` micro-pipeline you can add or remove optimizers (e.g. nQuant) that are applied in order to the specified file type.

Comments are available in the config files and source for your perusal.