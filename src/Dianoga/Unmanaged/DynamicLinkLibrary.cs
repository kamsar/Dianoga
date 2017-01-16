using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Web.Hosting;

namespace Dianoga.Unmanaged
{
    public class DynamicLinkLibrary : IDisposable
    {
        private readonly string originalPathToDll;
        private readonly object loaderLock = new object();
        private string tempPathToDll;
        private IntPtr pointerLibrary;

        public DynamicLinkLibrary(string pathToDll)
        {
            originalPathToDll = pathToDll.StartsWith("~") || pathToDll.StartsWith("/")
                ? HostingEnvironment.MapPath(pathToDll)
                : pathToDll;
            pointerLibrary = LoadModule();
        }

        private IntPtr LoadModule()
        {
            lock (loaderLock)
            {
                if (pointerLibrary != IntPtr.Zero) return pointerLibrary;

                if (!File.Exists(originalPathToDll))
                    throw new FileNotFoundException($"Unable to load DLL from {originalPathToDll}");

                tempPathToDll = Path.GetTempFileName();

                CopyFile();

                pointerLibrary = NativeMethods.LoadLibrary(tempPathToDll);

                if (pointerLibrary == IntPtr.Zero)
                    throw new Exception($"Unable to load DLL from {originalPathToDll}");
                return pointerLibrary;
            }
        }

        private void CopyFile()
        {
            using (var stream = File.OpenRead(originalPathToDll))
            using (var writeStream = new FileStream(tempPathToDll, FileMode.Create, FileAccess.Write))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(writeStream);
            }
        }

        private IntPtr LoadMethodOrFunction(string name)
        {
            var addressOfMethod = NativeMethods.GetProcAddress(pointerLibrary, name);
            if (addressOfMethod == IntPtr.Zero)
                throw new Exception("Can't find {functionName} funtion in {originalPathToDll}");
            return addressOfMethod;
        }

        public object GetDelegateFunction(string functionName, Type type)
        {
            var addressOfMethod = LoadMethodOrFunction(functionName);
            var method = Marshal.GetDelegateForFunctionPointer(addressOfMethod, type);
            return method;
        }

        public void Dispose()
        {
            //Dispose the library
            if (pointerLibrary != IntPtr.Zero)
                NativeMethods.FreeLibrary(pointerLibrary);

            //Delete temporary File
            if (File.Exists(tempPathToDll))
                File.Delete(tempPathToDll);
        }
    }
}
