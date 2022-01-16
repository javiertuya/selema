using HtmlAgilityPack;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Giis.Selema.Portable.Selenium
{
    ///<summary>
    ///Gets a list of lists from an html table using HtmlAgilityPack (similar to jsoup on Java)
    ///to optimize reading of dom elements
    ///(selenium methods have very low performance when reading large tables).
    ///</summary>
    public class SeleniumTable
    {
        private readonly IList<IList<string>> ltable;

        /// <summary>
        /// Reads all table cells as a list of lists form a WebElement
        /// Tables can include thead and tbody, provided that th and td are just below tr
        /// </summary>
        public SeleniumTable(IWebElement element)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(element.GetAttribute("outerHTML"));
            ltable = new List<IList<string>>();
            //after parse we have a fragmetn with tags html, head y body
            //a level down we find the table
            HtmlNode table = doc.DocumentNode.FirstChild;
            foreach (HtmlNode row in table.SelectNodes("//tr"))
            {
                IList<string> lrow = new List<string>();
                foreach (HtmlNode cell in row.SelectNodes("th|td"))
                {
                    //HtmlAgiltyPack has innerText but returns same as InnerHtml without any decoding (v1.11.31), forces decoding
                    lrow.Add(HttpUtility.HtmlDecode(cell.InnerHtml));
                }
                ltable.Add(lrow);
            }
        }
        /// <summary>
        /// Returns all table cells as a list of lists (of strings)
        /// </summary>
        public List<List<string>> GetRowsAsList()
        {
            return (List<List<string>>)ltable;
        }
        /// <summary>
        /// Returns all table cells as bidimensional array (of strings)
        /// </summary>
        public string[][] GetRowsAsArray()
        {
            return ltable.Select(l => l.ToArray()).ToArray();
        }
    }
}
