using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KabutanLib;

namespace KabuInformationForTwitter
{
    class Program
    {
        static void Main(string[] args)
        {
            var kabutans = KaiziClient.GetKaiziItems(1);
            var con = new KabutanPostgreSQLContext();
            foreach(var kabutan in kabutans)
            {
                con.KaiziItems.Add(kabutan);
            }
            con.SaveChanges();
        }
    }
}
