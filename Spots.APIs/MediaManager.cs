using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spots.APIs
{
    public class MediaManager
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public MediaManager(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }
        public string UploadPhoto(byte[] fileBytes)
        {
            if (fileBytes == null)
            {
                return "";
            }
            // get this environment's web root path (the path
            // from which static content, like an image, is served)
            var webRootPath = hostEnvironment.WebRootPath;

            // create the filename
            string fileName = Guid.NewGuid().ToString() + ".jpg";

            // the full file path
            var filePath = Path.Combine($"{webRootPath}/images/{fileName}");

            // write bytes and auto-close stream
            System.IO.File.WriteAllBytes(filePath, fileBytes);
            // fill out the filename
            return fileName;

        }
        public void DeletePhoto(string fileName)
        {
            var webRootPath = hostEnvironment.WebRootPath;

            var imagePath = Path.Combine($"{webRootPath}/images/{fileName}");
            if ((System.IO.File.Exists(imagePath)))
            {
                System.IO.File.Delete(imagePath);
            }
        }
    }
}
