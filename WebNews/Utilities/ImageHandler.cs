using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Utilities
{
    public interface IImageHandler
    {
        Task<IActionResult> UploadImage(IFormFile file, string owner, string imgname);
        IActionResult RemoveImage(string owner, string imgname);
        IActionResult RenameImage(string owner, string oldname, string newname);
    }
    public class ImageHandler : IImageHandler
    {
        private readonly IImageWriter _imageWriter;
        public ImageHandler(IImageWriter imageWriter)
        {
            _imageWriter = imageWriter;
        }

        public async Task<IActionResult> UploadImage(IFormFile file, string owner, string imgname)
        {
            var result = await _imageWriter.UploadImage(file, owner, imgname);
            return new ObjectResult(result);
        }
        public IActionResult RemoveImage(string owner, string imgname)
        {
            string result = _imageWriter.RemoveImage(owner, imgname);
            return new ObjectResult(result);
        }
        public IActionResult RenameImage(string owner, string oldname, string newname)
        {
            string result = _imageWriter.RenameImage(owner, oldname, newname);
            return new ObjectResult(result);
        }
    }
}
