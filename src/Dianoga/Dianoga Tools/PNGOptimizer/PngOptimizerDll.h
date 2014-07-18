/////////////////////////////////////////////////////////////////////////////////////
// PngOptimizerDll - A DLL to get access to PngOptimizer optimization engine
// Copyright (C) 2012/2013 Hadrien Nilsson - psydk.org
// This DLL is distributed under the terms of the GNU LESSER GENERAL PUBLIC LICENSE
// See License.txt for the full license.
/////////////////////////////////////////////////////////////////////////////////////
#ifndef PNGOPTIMIZERDLL_H
#define PNGOPTIMIZERDLL_H

// Version: 2.4.0

#ifndef PNGOPTIMIZERDLL_IMPL
#define PNGOPTIMIZERDLL_API extern "C" __declspec(dllimport)
#else
#define PNGOPTIMIZERDLL_API extern "C" __declspec(dllexport)
#endif

// Remarks
// - Windows types like BOOL are used for better C# integration.
// - Allocated strings in PO_GetSettings and PO_GetLastErrorString.
//   should be freed with CoTaskMemFree(). This is automatically done when
//   unmarshalling with C#. Check TestPODll for an example.
// - The API is to be accessed by one thread at a time. A thread attempting
//   to call the API will be blocked if another thread is alreay calling.

enum
{
	POChunkOption_Remove = 0, // PNG chunk will be removed unconditionally
	POChunkOption_Keep = 1,   // PNG chunk will be kept if it is found
	POChunkOption_Force = 2   // PNG chunk will be modified or added if not found
};

struct POSettings
{
	BOOL    BackupOldPngFiles; // Creates a backup (default: TRUE)
	BOOL    KeepInterlacing;   // Keeps interlacing if present (default: FALSE)
	BOOL    IE6Compatible;     // Avoids some types of PNG that do not work on IE6 (default: FALSE)
	BOOL    SkipAnimatedGifs;  // Avoids converting a GIF if it is animated (default: FALSE)
	BOOL    KeepFileDate;      // Keeps the date of the original file (default: FALSE)

	DWORD   BkgdOption;  // POChunkOption (default: Remove)
	DWORD   BkgdColor;   // Forced color. 0xAARRGGBB (default: 0xFF000000)

	DWORD   TextOption;  // POChunkOption (default: Remove)
	PCWCH   TextKeyword; // Forced text keyword (default: "")
	PCWCH   TextData;    // Forced text data (default: "")

	DWORD   PhysOption;  // POChunkOption (default: Remove)
	DWORD   PhysPpmX;    // Forced pixels per meter X (default: 2834)
	DWORD   PhysPpmY;    // Forced pixels per meter Y (default: 2834)
};

// Optimizes one PNG or APNG file. If the file is a GIF/BMP/TGA, it is converted to PNG.
// [in]  filePath    Path of the file to optimize or convert
// [ret] TRUE upon success
PNGOPTIMIZERDLL_API BOOL  PO_OptimizeFile(PCWCH filePath);

// Optimizes one PNG or APNG file loaded in memory. If the file is a GIF/BMP/TGA, it is
// converted to PNG.
// [in]   image           File content to optimize/convert
// [in]   imageSize       Size in bytes of the file content to optimize/convert
// [out]  result          Result buffer. To be allocated by the caller.
// [in]   resultCapacity  Capacity in bytes of the result buffer
// [out]  pResultSize     Size in bytes of the result
// [ret] TRUE upon success
PNGOPTIMIZERDLL_API BOOL  PO_OptimizeFileMem(LPCVOID* image, INT imageSize,
                                             LPVOID* result, INT resultCapacity, INT* pResultSize);

// Gets a description of the error when PO_OptimizeFile returns FALSE.
// [ret] Last error description. Free the string with CoTaskMemFree() after usage.
PNGOPTIMIZERDLL_API PCWCH PO_GetLastErrorString();

// Sets the settings used when optimizing with PO_OptimizeFile.
// [in]  s    Settings structure to copy from
// [ret] TRUE upon success
PNGOPTIMIZERDLL_API BOOL  PO_SetSettings(const POSettings* s);

// Gets the settings used when optimizing with PO_OptimizeFile.
// [out] s    Settings structure receiving the settings. Strings should be
//            freed with CoTaskMemFree() after usage.
// [ret] TRUE upon success
PNGOPTIMIZERDLL_API BOOL  PO_GetSettings(POSettings* s);

#endif
