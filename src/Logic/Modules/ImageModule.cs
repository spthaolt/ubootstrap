using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using Bootstrap.Logic.Utils;

namespace Bootstrap.Logic.Modules
{
    public class ImageModule : IHttpModule
    {
        private readonly List<string> extensions;

        public ImageModule()
        {
            var imageEncoders = ImageCodecInfo.GetImageEncoders();
            extensions = imageEncoders
                .Select(x => x.FilenameExtension.ToLowerInvariant().Replace("*", string.Empty).Split(';'))
                .SelectMany(y => y).ToList();
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
        }

        void OnBeginRequest(object sender, EventArgs e)
        {
            var extension = HttpContext.Current.Request.CurrentExecutionFilePathExtension;
            if (string.IsNullOrEmpty(extension) || !extensions.Exists(x => x == extension.ToLowerInvariant()))
            {
                return;
            }

            var physicalPath = HttpContext.Current.Request.PhysicalPath;
            if (!File.Exists(physicalPath))
            {
                return;
            }

            var url = HttpContext.Current.Request.Url;
            if (string.IsNullOrEmpty(url.Query))
            {
                return;
            }

            var nvc = HttpUtility.ParseQueryString(url.Query);
            var filePath = HttpContext.Current.Request.CurrentExecutionFilePath;
            nvc.Add("image", filePath);
            var queryString = nvc.ConstructQueryString();
            var path = "/ImageGen.ashx?" + queryString;
            HttpContext.Current.Server.TransferRequest(path);
        }

        public void Dispose()
        {
        }
    }
}
