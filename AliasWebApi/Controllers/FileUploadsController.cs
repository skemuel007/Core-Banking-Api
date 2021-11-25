using AliasWebApiCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using PdfRpt.Core.Helper;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    public class FileUploadsController : Controller
    {
        private readonly AppDbContext _db;

        public FileUploadsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("api/upload/{filetype}")]
        public async Task<IActionResult> UploadFile(string filetype, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("File not selected");
            if (string.IsNullOrEmpty(filetype))
                return Content("File type required");
            string[] filedetails = file.FileName.Split('.');
            filedetails[0] = DateTime.Now.ToString(@"yyyy-MM-dd ") + "T" + DateTime.Now.ToString(@" hh mm ss");
            string filename = string.Concat(filedetails[0],".",filedetails[1]);
            var path = Path.Combine(
                Directory.GetCurrentDirectory(), $"Files/{filetype}",
                filename);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok($"{filename}");
        }    
        
        [HttpPost("api/Files")]
        public async Task<IActionResult> Download([FromBody] FileDownloadModel model)
        {
            if (model.Filename == null)
                return BadRequest("No filename specified");

            var path = Path.Combine(
                Directory.GetCurrentDirectory(),"Files",
                model.Type, model.Filename);
            
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                    {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
            }
    }

    public class FileDownloadModel
    {
        public string Type { get; set; }
        public string Filename { get; set; }
    }
}