using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;
using System.IO;
using System.Configuration;

namespace KabuInformationForTwitter
{
    public class TwitterCon
    {
        public Tokens tokens;
        //コンストラクタ　ツイッター操作Token生成
        public TwitterCon()
        {
            var twitterTokenConfig = ConfigurationManager.AppSettings;            
            tokens = CoreTweet.Tokens.Create
                        (
                            twitterTokenConfig.Get("consumerKey"),
                            twitterTokenConfig.Get("consumerSecret"),
                            twitterTokenConfig.Get("tokenKey"),
                            twitterTokenConfig.Get("tokenSecret")
                        );
        }
    }
    //ツイッター操作用拡張クラス
    static public class TwitterConExtension
    {
        //ツイッターテキストは140文字に成形しなおす　URL等文字制限に入らない文字列はextensionUrlTextに入れる
        static public void Tweet(this TwitterCon twitterCon, string tweetText, string extensionUrlText = "")
        {
            //140文字に収める
            if (tweetText.Length > 140)
            {
                tweetText = tweetText.Substring(0, 139);
            }
            try
            {
                twitterCon.tokens.Statuses.Update(tweetText + "\r\n" + extensionUrlText);
            }
            catch
            {
                Console.WriteLine("Error:ツイート失敗　" + DateTime.Now.ToString("yyyyMMdd") + tweetText);
            }
        }
    }
}
