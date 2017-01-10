using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace z5_1.Controllers
{
    public class SendingController : Controller
    {
        // GET: Sending
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public FileContentResult Send(Models.SendingFile m)
        {
            int sum = 0;
            var s = m.File.InputStream;
            for (int i = 0; i < s.Length; i++)
                sum = (sum + s.ReadByte()) % 0xFFFF;
            var opis = new Opis();
            opis.rozmiar = s.Length;
            opis.suma = sum;
            opis.nazwa = m.File.FileName;
            XmlSerializer x = new XmlSerializer(opis.GetType());
            StringWriter w = new StringWriter();
            x.Serialize(w, opis);

            return new FileContentResult(Encoding.ASCII.GetBytes(w.ToString()), "text/html");
        }
    }

    [SerializableAttribute]
    public class Opis 
    {
        public string nazwa;
        public long rozmiar;
        public int suma;
    }
}
