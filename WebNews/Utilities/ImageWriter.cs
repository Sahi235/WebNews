using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebNews.Utilities
{
    public class ImageWriter : IImageWriter
    {
        public async Task<string> UploadImage(IFormFile file, string owner, string imgname)
        {
            if (CheckifImageFile(file))
                return await WriteFile(file, owner, imgname);
            return "invalid";
        }
        private bool CheckifImageFile(IFormFile file)
        {
            byte[] FileBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                FileBytes = ms.ToArray();
            }
            return WriterHelper.GetImageFormat(FileBytes) != WriterHelper.ImageFormat.unknown;
        }
        public async Task<string> WriteFile(IFormFile file, string owner, string imgname)
        {
            try
            {
                string dir = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{owner}");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{owner}", imgname);
                using (FileStream bits = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(bits);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return imgname;
        }
        public string RemoveImage(string owner, string imgname)
        {
            return RemoveFile(owner, imgname);
        }
        public string RenameImage(string owner, string oldname, string newname)
        {
            return MoveFile(owner, oldname, newname);
        }
        public string RemoveFile(string owner, string imgname)
        {
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{owner}", imgname);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "removed";
        }
        public string MoveFile(string owner, string oldname, string newname)
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{owner}", oldname);
                var newpath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{owner}", newname);
                if (File.Exists(path))
                {
                    File.Move(path, newpath);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "renamed";
        }
    }

}
