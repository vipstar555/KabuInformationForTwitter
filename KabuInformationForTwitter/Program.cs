using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KabutanLib;
using System.Threading;
using System.Data;
using System.Configuration;
using System.Collections.Specialized;

namespace KabuInformationForTwitter
{
    class Program
    {
        static void Main(string[] args)
        {
            var con = new YahooFinanceDbContext();  //時価総額取得用のDBクラス  
            var tweetCon = new TwitterCon();    //ツイッター操作クラス            
            var kaiziCon = new KabutanPostgreSQLContext();  //開示用PostgreSQL操作クラス            
            IEnumerable<KaiziItem> kaiziItems;  //開示内容保存用List
            string latestPdfURL = kaiziCon.LatestPDFURL();       //最もDatetimeが新しいPDFのURL(同時刻の場合は主キーIDが最新のもの優先)
            
            int kabutanMaxPage = 99;   //株探適時開示MAXページ数
            bool pageBreakFlag = true;
            while (true)
            {
                //trueならツイート&DB登録を続ける
                pageBreakFlag = true;
                //最新PDFURL又は指定ページ数に到達するまで適時開示のツイート＆DB登録
                for (int page = 1; page <= kabutanMaxPage && pageBreakFlag; page++)
                {
                    //株探適時開示ページ取得
                    kaiziItems = KaiziClient.WebDownloadKaiziItems(page);
                    //株探ツイート+DB操作
                    foreach (var kaiziItem in kaiziItems)
                    {
                        //通知済の時点でツイートとDB登録をしない
                        //（ツイートはしないけどDBだけ登録処理書くかも　それは別プログラム？）
                        if (kaiziItem.PDFURL == latestPdfURL)
                        {
                            //DB登録＆ツイート終了
                            pageBreakFlag = false;
                            break;
                        }
                        //最新の時価総額取得
                        var capTradeIndex = con.TradeIndexs.Where(x => x.code == kaiziItem.Code).OrderByDescending(x => x.date).FirstOrDefault();
                        long latestMarketCapitalization = 0;
                        if (capTradeIndex == null)
                        {
                            latestMarketCapitalization = 0;
                        }
                        else
                        {
                            latestMarketCapitalization = capTradeIndex.marketCapitalization;
                        }
                        //DB登録
                        if (kaiziCon.IsDuplicatePDFURL(kaiziItem.PDFURL) == false)
                        {
                            kaiziCon.KaiziItems.Add(kaiziItem);
                        }
                        //ツイート関連
                        string strMarketCap = "-";
                        //0除算エラー回避
                        if (latestMarketCapitalization != 0)
                        {
                            strMarketCap = string.Format($"{latestMarketCapitalization / 100000000.0:0.00}");
                        }
                        var tweetText = string.Format($"{kaiziItem.Code} {kaiziItem.Name} {strMarketCap}億\r\n{kaiziItem.Title}");
                        tweetCon.Tweet(tweetText, kaiziItem.PDFURL);
                        Console.WriteLine($"{DateTime.Now} Tweet" + tweetText + "\r\n" + kaiziItem.PDFURL);
                    }      
                    //規制対策
                    Thread.Sleep(500);
                }
                //適時開示DB更新
                kaiziCon.SaveChanges();
                //最新通知済開示内容を記録
                latestPdfURL = kaiziCon.LatestPDFURL();       //最もDatetimeが新しいPDFのURL(同時刻の場合は主キーIDが最新のもの優先
                Thread.Sleep(10000);
            }

            
        }
    }
}
