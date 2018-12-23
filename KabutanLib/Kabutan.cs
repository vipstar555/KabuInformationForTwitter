using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using System.Net;
using AngleSharp.Parser.Html;

namespace KabutanLib
{
    static public class KaiziClient
    {
        //{0}:何ページ目
        const string KaiziURL = @"https://kabutan.jp/disclosures/?kubun=&page={0}";
        
        static public IEnumerable<KaiziItem> GetKaiziItems(int page)
        {
            var wc = new System.Net.WebClient();
            var stream = wc.OpenRead(string.Format(KaiziURL, page));
            var parser = new HtmlParser();
            var doc = parser.Parse(stream);
            var trs = doc.QuerySelectorAll("#main > div.disclosure_box > table > tbody > tr");
            foreach(var tr in trs)
            {
                //[0]:Code　[1]:Market
                var tacs = tr.QuerySelectorAll("td.tac");
                //[0]:InfoType [1]:Title
                var tals = tr.QuerySelectorAll("td.tal");
                yield return new KaiziItem
                {
                    Code = (int.TryParse(tacs[0].TextContent, out var code)) ? code : -1,
                    Name = tr.QuerySelector("th.tal").TextContent,
                    Market = tacs[1].TextContent,
                    InfoType = tals[0].TextContent,
                    Title = tals[1].TextContent,
                    DateTime = DateTime.TryParse(tr.QuerySelector("time").GetAttribute("datetime"), out var datetime) ? datetime : DateTime.MinValue,
                    PDFURL = tals[1].QuerySelector("a").GetAttribute("href").ToString(),
            };

            }

        }
    }

    public class KaiziItem
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Market { get; set; }
        public string InfoType { get; set; }
        public string Title { get; set; }
        public DateTime DateTime { get; set; }
        public string PDFURL { get; set; }
    }
}
