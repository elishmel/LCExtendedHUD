using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace LCExtendedHUD.Utility {
    internal class ResourceLoader {
        private ResourceLoader() { }
        public static byte[] GetResourceFromCurrentAssemblyAsBytes(in string resouce) {

            if(resouce == null) return null;

            Assembly assembly = Assembly.GetExecutingAssembly();
            string fullResourceName = $"{assembly.GetName().Name}.{resouce}";

            using (System.IO.Stream resourceStream = assembly.GetManifestResourceStream(fullResourceName)) {
                if (resourceStream == null) {
                    ExtendedHUD.Log.LogError($"Unable to fetch resouce {fullResourceName}.");
                    throw new ArgumentException($"Resource {fullResourceName} not found");
                }
            
                using (MemoryStream memoryStream = new MemoryStream()) {
                    resourceStream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }

            }
        }
    }
}
