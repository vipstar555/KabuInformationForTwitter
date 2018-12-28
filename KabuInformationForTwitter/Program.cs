using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KabutanLib;
using System.Threading;

namespace KabuInformationForTwitter
{
    class Program
    {
        static void Main(string[] args)
        {
            string latestPdfURL = "";
            var financeConnect = new CodeListDbContext.FinanceConnect();
            var con = financeConnect.GetContext();
            IEnumerable<KaiziItem> kaiziItems;
            var tweetToken = Twitter.GetToken();
            while (true)
            {
                //とりあえず15個分で→初期値どこかで決定する（DB化実用始めたらそこまで読み取るとかも）
                kaiziItems = KaiziClient.GetKaiziItems(1);
                
                if(latestPdfURL == "" || kaiziItems.Count() > 0)
                {
                    latestPdfURL = kaiziItems.FirstOrDefault().PDFURL;
                }
                
                foreach(var kaiziItem in kaiziItems)
                {
                    if(kaiziItem.PDFURL == latestPdfURL )
                    {
                        break;
                    }
                    //最新の時価総額
                    var latestMarketCapitalization = con.TradeIndexs.Where(x => x.code == kaiziItem.Code).OrderByDescending(x => x.date).FirstOrDefault().marketCapitalization;
                    //ツイート内容
                    var tweetText = string.Format($"{kaiziItem.Code} {kaiziItem.Name} 時価{latestMarketCapitalization/100000000:0.00}億\r\n{kaiziItem.Title}");
                    //140文字に収める
                    if(tweetText.Length > 140)
                    {
                        tweetText = tweetText.Substring(0, 139);
                    }                    
                    tweetToken.Statuses.Update(tweetText + "\r\n" + kaiziItem.PDFURL);
                    Console.WriteLine($"{DateTime.Now} Tweet" + tweetText + "\r\n" + kaiziItem.PDFURL);
                }
                //最新通知済開示内容を記録
                latestPdfURL = kaiziItems.FirstOrDefault().PDFURL;
                Thread.Sleep(10000);
            }

            
        }
    }
}
