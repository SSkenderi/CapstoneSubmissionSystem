using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapstoneSubmissionSystem.ViewModels
{
    public class UploadDokument
    {

        public string Title { get; set; }
        public int Doctype { get; set; }
       
        public HttpPostedFileBase FileContent { get; set; }
       
    }
}