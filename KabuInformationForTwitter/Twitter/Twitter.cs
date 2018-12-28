using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;
using System.IO;

namespace KabuInformationForTwitter
{
    public class Twitter
    {
        const int limit = 140;
        const string tokenFilePath = "../../../metadata/twetterAPI.txt";
        static public Tokens GetToken()
        {
            using (var sr = new StreamReader(tokenFilePath))
            {
                return CoreTweet.Tokens.Create
                        (
                            sr.ReadLine(),
                            sr.ReadLine(),
                            sr.ReadLine(),
                            sr.ReadLine()
                        );
            }
        }
    }
}
