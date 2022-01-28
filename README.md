# Dianoga

An automatic image optimizer for the Sitecore media library. Reduce the size of your images served from Sitecore by 8-70%, completely automatically.

![Dianoga CI](https://github.com/kamsar/Dianoga/workflows/Dianoga%20CI/badge.svg) ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Dianoga?logo=nuget&link=https://www.nuget.org/packages/Dianoga)

When media images are requested, Dianoga automatically runs [mozjpeg](https://github.com/mozilla/mozjpeg), [PNGOptimizer](http://psydk.org/pngoptimizer), [SVGO](https://github.com/svg/svgo) or [WebP](https://developers.google.com/speed/webp/) on the image data immediately after it is placed in the Sitecore media cache. 

Dianoga ensures that your site is always serving fully optimised media library images even if you are using Sitecore's dynamic resizing features. Even if you have already optimized every image uploaded into the media library, after Sitecore performs image processing the result is _not_ optimized (an unfortunate limitation of most other image optimization libraries for Sitecore is that they only apply to the original image).

Dianoga is also great for situations where content editors may not be image editing experts and upload images that contain gobs of EXIF data and other nonessential metadata - these are removed automatically before being shown to visitors. 

## Format Support

Dianoga supports:
* JPEGs (via mozjpeg - lossless or lossy)
* PNGs (via PNGOptimizer - lossless / pngquant - lossy)
* SVGs (via SVGO - lossless, and automatic gzipping of SVG media responses)
* WebP (via cwebp - lossless or lossy)
* Avif (via avifenc - lossless or lossy)
* JPEG XL (via cjxl - lossless or lossy)
* Auto convert JPEG/PNG/GIF to WebP based on browser support
* Auto convert JPEG/PNG to Avif based on browser support
* Auto convert JPEG/PNG/GIF to JPEG XL (jxl) based on browser support

Additional format support is possible to add via new processors in the `dianogaOptimize` pipeline.

## Performance

By default, Dianoga runs asynchronously _after_ the image is saved into the media cache. This means it has practically no effect on the site's frontend performance (though it does use some CPU time in the background). The performance of Dianoga is logged into the Sitecore log. PNGs are very fast because it's a native P/Invoke call to a C DLL - about 20ms for a small one. JPEGs result in executing a program and are slightly slower - around 200ms for a medium sized image. Larger images several MB in size can take a second or two depending on the hardware in use.

Once the image is in cache for the requested size, there is no performance penalty as the file is read from the media cache unless it is changed again.

Note that because the cache is updated after the first image request, the _first_ hit to images will result in the unoptimized version being served, to preserve the responsiveness of the request. Subsequent requests will receive the optimized version from cache.

## Limitations

Dianoga depends on the Dianoga Tools folder that is installed by the NuGet package into the web project it is installed on. You can relocate these tools if you wish by changing the paths in the `App_Config/Include/Dianoga/(Dianoga.Jpeg.config|Dianoga.Png.config|Dianoga.Svg.config)` files.

## Options

### Optimization Strategies

In Dianoga, you can choose your optimization strategy. Most people if serving media locally are best off with the defaults, which optimize the media as it is placed into the media cache. This results in minimal impact to the experience for front end visitors.

However, for folks using CDNs those sources must be sent the optimized version of the media immediately. For these situations, you can enable `Dianoga.Strategy.GetMediaStreamSync.config.disabled` and disable `Dianoga.Strategy.MediaCacheAsync.config`. This will cause optimization to occur synchronously as the media is first requested, which is appropriate if the media is being sent to a CDN. This may however cause a delay for the first hit user before they start seeing images.

If you want to execute the Dianoga optimization pipeline programmatically (e.g. as part of a CDN upload background process), you can use the `MediaOptimizer` class to execute optimization on any Sitecore `MediaStream` object at will.

### Ignoring Paths

Dianoga 3+ allows for ignoring specific paths in the media library. See `Dianoga.ExcludePaths.config.example` for an example of this.

## Installation

Dianoga 5+ has a NuGet package for the main code and configs, and a separate package for the SVGO tools - `Dianoga.svgtools`. This was done as SVGO is an 80MB library.
Once Dianoga is installed, __clear your App_Data/MediaCache folder__, and you're done.

To perform a manual installation:

* Copy the `Dianoga Tools` folder to the `App_Data` folder of your website
* Copy `Default Config Files/*.config` to `App_Config\Include\Dianoga`
* Reference `Dianoga.dll` or the source project in your web project

### Dianoga.svgtools

If you are enabling the SVGO optimiser, you'll also need the [Dianoga.svgtools](https://www.nuget.org/packages/Dianoga.svgtools) NuGet package.
This is simply a prepackaged compiled version of SVGO called SVGOP from [here](https://github.com/twardoch/svgop).

## Next-gen Formats Support

Next-gen images use formats with superior compression and quality characteristics compared to their GIF, JPEG, and PNG ancestors. These image formats support advanced features designed to take up less data while maintaining a high quality level, making them perfect for web use. 

### How Next-gen Formats Optimization Works:

Browser sends request to server to get image. It sends list of accepted image formats in the `Accept` header, e.g. it can be `image/avif,image/webp,image/apng,image/*,*/*;q=0.8`. Presence of `image/webp` means that this browser supports `WebP` image format. Absense of `image/jxl` means that this browser does not support `JPEG XL` format. It is possible to check this header on server side and return `WebP` format image to browser instead of `JPEG`, `PNG` or `GIF`. If browser doesn't support any next-gen formats then other image optimizers are executed if they are enabled.

### How to Enable Next-gen Formats Support:

1. Open web.config and change line

`<add verb="*" path="sitecore_media.ashx" type="Sitecore.Resources.Media.MediaRequestHandler, Sitecore.Kernel" name="Sitecore.MediaRequestHandler" />`

to

`<add verb="*" path="sitecore_media.ashx" type="Dianoga.NextGenFormats.MediaRequestHandler, Dianoga" name="Sitecore.MediaRequestHandler" />`

OR if you use SXA

`<add verb="*" path="sitecore_media.ashx" type="Dianoga.NextGenFormats.MediaRequestHandlerXA, Dianoga" name="Sitecore.MediaRequestHandler" />`

OR if you have a custom `MediaRequestHandler` then you need to make some changes - see `MediaRequestHandler.cs`

2. If you run Sitecore under CDN: review and enable `Dianoga.NextGenFormats.CDN.config.disabled`. It will add `?extension=<list of supported extensions>` query parameter to all images present on your pages.

3. Enable any next-gen formats configuration files that you want. e.g.: `z.01.Dianoga.NextGenFormats.WebP.config.disabled`, `z.02.Dianoga.NextGenFormats.Avif.config.disabled`, `z.03.Dianoga.NextGenFormats.Jxl.config.disabled`

4. Review files that you have enabled and adjust any parameters if you require lossless or higher quality than the default

5. Adjust order of next-gen formats configuration files. 

WebP, Avif and JPEG XL formats are not convertable to each other. Dianoga works with file stream and сonsistently converts file stream using optimizers. 
e.g. JPEG > mozjpeg > Avif. Once stream is converted to one next-gen format it could not be easily reconverted to another format, e.g. Avif <=> WebP, because current encoders doesn't support it. 

Configuration files should be applied in **reversed** priority order. The first format should be applied in last confiration file.

E.g. We want to support all JPEG XL, Avif and WebP in next priority 1. JPEG XL 2. Avif 3. WebP. If browser supports JPEG XL then try to use it. If browser doesn't support JPEG XL then check if browser support Avif and try to use it. If browser doesn't support both JPEG XL and Avif then check if browser supports WebP and try to use it.

Then next-gen configuration file names should have prefixes to put files in the proper order: **z.01**.Dianoga.NextGenFormats.WebP.config.disabled, **z.02**.Dianoga.NextGenFormats.Avif.config.disabled, **z.03**.Dianoga.NextGenFormats.Jxl.config.disabled

### Next-gen formats list

#### WebP

WebP is is an image format employing both lossy and lossless compression. It is currently developed by Google, based on technology acquired with the purchase of On2 Technologies. WebP file size is [25%-34% smaller compared to JPEG file size](https://developers.google.com/speed/webp/docs/webp_study) and  [26% smaller for PNG](https://developers.google.com/speed/webp/docs/webp_lossless_alpha_study). [All evergreen browsers except Safari currently support WebP](https://caniuse.com/#feat=webp). 

By default WebP optimization is disabled since you need to do some due diligence around reviewing configs, how it works, and importantly if you are using a CDN you need to do some extra steps.

If you want to enable usage of `WebP` format, please rename `z.01.Dianoga.NextGenFormats.WebP.config.disabled` to `z.01.Dianoga.NextGenFormats.WebP.config`

#### Avif

Avif is a modern image format based on the AV1 video format. AVIF generally has better compression than WebP, JPEG, PNG and GIF and is designed to supersede them. AVIF competes with JPEG XL which has worse support, similar compression quality and is generally seen as more feature-rich than AVIF. [Avif is supported by Chrome and Firefox browsers](https://caniuse.com/#feat=avif).

By default Avif optimization is disabled since you need to do some due diligence around reviewing configs, how it works, and importantly if you are using a CDN you need to do some extra steps.

If you want to enable usage of `Avif` format, please rename `z.02.Dianoga.NextGenFormats.Avif.config.disabled` to `z.02.Dianoga.NextGenFormats.Avif.config`

#### JPEG XL

JPEG XL is a modern image format optimized for web environments. JPEG XL generally has better compression than WebP, JPEG, PNG and GIF and is designed to supersede them. JPEG XL competes with AVIF which has better support, similar compression quality but fewer features overall. JPEG XL is not supported yet by modern browsers. [But support could be enabled via flags in Chrome, Firefox, Opera and Edge](https://caniuse.com/?search=jpeg%20xl). 

By default JPEG XL optimization is disabled since you need to do some due diligence around reviewing configs, how it works, and importantly if you are using a CDN you need to do some extra steps.

If you want to enable usage of `JPEG XL` format, please rename `z.03.Dianoga.NextGenFormats.Jxl.config.disabled` to `z.02.Dianoga.NextGenFormats.Jxl.config`


## Upgrade

Upgrading from any previous version of Dianoga is fairly simple.

1. Upgrade the NuGet package.
2. Remove any `App_Config\Include\Dianoga.config` that may exist (config has changed, you must reapply settings if changed in the new scheme)
3. Remove `Dianoga Tools` if it exists (Dianoga 3+ places tools under App_Data\Dianoga Tools instead so that IIS defaults to not serving anything there)
4. Delete your MediaCache folder (defaults to App_Data/MediaCache) on all servers after deployment of Dianoga (otherwise unoptimized caches will remain present)
5. Have fun.

## Troubleshooting

If you're not seeing optimization take place, there are a couple of possibilities:

* The image might already be in the media cache (by default, `/App_Data/MediaCache`). Sitecore does not reprocess images in cache until needed, so Dianoga may never be called. Delete this folder, clear your browser cache or use ctrl-F5, and try again.
* The image may be in your browser cache. Check using the browser's network tab or Fiddler that Sitecore was not returning a `HTTP 304 Not Modified` response code for the media. If it was, the media cache was not evaluated for the response. Try a hard refresh instead.
* An error is occurring. The Sitecore logs catch all errors that occur when generating a media stream, so look there first. If an error occurs, the result of the pipeline is thrown away and the unmodified stream returned so you may not see broken images if this occurs, just oddly sized - if resizing - or unoptimized ones.

## Extending Dianoga

You can optimize any type of image that the media library supports with Dianoga.

Adding a new `Processor` to the config will enable you to call a micro-pipeline you define to process that type.

In your `Optimizer` micro-pipeline you can add or remove optimizers that are applied in order to the specified file type.

Comments are available in the config files and source for your perusal.
