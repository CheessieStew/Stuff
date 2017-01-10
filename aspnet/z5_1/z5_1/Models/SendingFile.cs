using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace z5_1.Models
{
    public class SendingFile
    {
        [Required]
        public HttpPostedFileBase File { get; set; }
    }
}