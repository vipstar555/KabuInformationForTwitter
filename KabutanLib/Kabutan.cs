using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using System.Net;
using AngleSharp.Parser.Html;
using System.ComponentModel.DataAnnotations;        // 参照を追加。
using System.ComponentModel.DataAnnotations.Schema; // 参照を追加。
using System.IO;

namespace KabutanLib
{
    //株探の適時開示操作クラス
    static public class KaiziClient
    {
        //{0}:何ページ目
        const string KaiziURL = @"https://kabutan.jp/disclosures/?kubun=&page={0}";

        //株探の適時開示から開示クラスを作成する
        static public IEnumerable<KaiziItem> WebDownloadKaiziItems(int page)
        {
            var wc = new System.Net.WebClient();    //株探クライアント
            Stream stream;  //適時開示html
            try
            {
                stream = wc.OpenRead(string.Format(KaiziURL, page));    //株探開示からHtml取得
            }
            catch (Exception e)
            {
                //html取得失敗でエラーログ
                Console.WriteLine("Error:株探開示取得失敗　" + DateTime.Now.ToString("yyyyMMdd") + e.ToString());
                yield break;
            }
            var parser = new HtmlParser();
            var doc = parser.Parse(stream); //html解析用
            //適時開示
            var trs = doc.QuerySelectorAll("#main > div.disclosure_box > table > tbody > tr");       
            //開示クラスを生成して返す
            foreach (var tr in trs)
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
    [Table("KabutanKaizi")]
    public class KaiziItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("code")]
        public int Code { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("market")]
        public string Market { get; set; }
        [Column("infotype")]
        public string InfoType { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("datetime")]
        public DateTime DateTime { get; set; }
        [Column("pdfurl")]
        public string PDFURL { get; set; }
    }
}
