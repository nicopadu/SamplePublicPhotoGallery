using PhotoGallery.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace PhotoGallery.Controllers
{
    public class GalleryController : Controller
    {
        //
        // GET: /Gallery/

        /// <summary>
        /// Default Action. List all the images in the current feed
        /// TODO: Filter by Tags
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            List<GalleryImage> images = null;

            try
            {
                //get feed URL from config
                string url = ConfigurationManager.AppSettings["RssUrl"];

                //for filtering xml extensions
                string mediaNs = "http://search.yahoo.com/mrss/";

                using (XmlReader reader = XmlReader.Create(url))
                {
                    SyndicationFeed feed = SyndicationFeed.Load(reader);

                    images = new List<GalleryImage>();
                    foreach (SyndicationItem item in feed.Items)
                    {
                        //filter media extensions
                        IEnumerable<SyndicationElementExtension> exts = item.ElementExtensions.Where<SyndicationElementExtension>(
                            e => e.OuterNamespace.Equals(mediaNs));

                        GalleryImage image = new GalleryImage();
                        image.Title = item.Title.Text;
                        image.Author = GetExtensionValue(exts, "credit");
                        image.TumbnailUrl = GetExtensionValue(exts, "thumbnail", "url");
                        image.ImageUrl = GetExtensionValue(exts, "content", "url");

                        images.Add(image);

                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

            return View(images);
        }

        /// <summary>
        /// Helper method to extract element values or attributes values from XML
        /// </summary>
        /// <param name="extensions">List of filtered XML extensions to the feed</param>
        /// <param name="name">Element name</param>
        /// <param name="attribute">Attribute name. Optional</param>
        /// <returns>Element's value (if no <code>attribute</code> is defined. Otherwise, Attribute's value</returns>
        private string GetExtensionValue(IEnumerable<SyndicationElementExtension> extensions, string name, string attribute = "")
        {
            try
            {
                XmlReader reader = extensions.FirstOrDefault<SyndicationElementExtension>(e => e.OuterName.Equals(name)).GetReader();
                if (reader != null)
                {
                    if (string.IsNullOrWhiteSpace(attribute))
                        return reader.ReadString();
                    else
                        return reader.GetAttribute(attribute);
                }
            }
            catch (Exception ex) 
            {
                throw new ApplicationException(ex.Message);
            }
            return string.Empty;
        }

    }
}
