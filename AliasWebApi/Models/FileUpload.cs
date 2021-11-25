using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class FileUpload : BaseModel
    {
        [Key]
        public string FileId { get; set; }

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public long FileLength { get; set; }

    }
}