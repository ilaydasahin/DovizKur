using DovizKur.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DovizKur.Data;
using System.Xml;

namespace DovizKur.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(DateTime tarih)
        {
            if (tarih.DayOfWeek == DayOfWeek.Saturday || tarih.DayOfWeek == DayOfWeek.Sunday)
            {
                ViewBag.Uyari = "Seçtiğiniz tarih hafta sonuna denk geliyor.";
                return View();
            }

            if (tarih > DateTime.Today)
            {
                ViewBag.Uyari = "Gelecekteki bir tarih için kur bilgisi bulunmamaktadır.";
                return View();
            }

            DovizKuru kur = db.DovizKurlari.FirstOrDefault(k => k.Tarih == tarih);

            if (kur == null)
            {
                string xml = GetDovizKurlariFromTCMB(tarih);

                if (string.IsNullOrEmpty(xml))
                {
                    ViewBag.Uyari = "Döviz kurları TCMB'den alınamadı.";
                    return View();
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);

                decimal euro = Convert.ToDecimal(xmlDoc.SelectSingleNode("//Currency[@Kod='EUR']/BanknoteSelling").InnerXml);
                decimal dolar = Convert.ToDecimal(xmlDoc.SelectSingleNode("//Currency[@Kod='USD']/BanknoteSelling").InnerXml);

                kur = new DovizKuru
                {
                    Tarih = tarih,
                    Euro = euro,
                    Dolar = dolar
                };

                db.DovizKurlari.Add(kur);
                db.SaveChanges();

                ViewBag.Uyari = "Döviz kurları başarıyla alındı ve kaydedildi.";
            }
            else
            {
                ViewBag.Uyari = "Döviz kurları kayıtlı olarak bulundu.";
            }

            ViewBag.Tarih = kur.Tarih.ToString("dd.MM.yyyy");
            ViewBag.Euro = kur.Euro;
            ViewBag.Dolar = kur.Dolar;

            return View();
        }

        private string GetDovizKurlariFromTCMB(DateTime tarih)
        {
            string url = "https://www.tcmb.gov.tr/kurlar/" + tarih.ToString("yyyyMM") + "/" + tarih.ToString("ddMMyyyy") + ".xml";


            using (WebClient client = new WebClient())
            {
                try
                {
                    string xmlData = client.DownloadString(url);
                    return xmlData;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
