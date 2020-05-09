using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Utilities
{
    public interface IImageWriter
    {
        Task<string> UploadImage(IFormFile file, string owner, string imgname);
        string RemoveImage(string owner, string imgname);
        string RenameImage(string owner, string oldname, string newname);
    }
}
