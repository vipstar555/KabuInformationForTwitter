using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;        // 参照を追加。
using System.ComponentModel.DataAnnotations.Schema; // 参照を追加。
using Npgsql;
using System.IO;
using System.Configuration;

namespace KabutanLib
{
    //Entityframework=>株探DB操作クラス
    public class KabutanPostgreSQLContext : DbContext
    {
        public string DefaultSchema { get; private set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
            => modelBuilder.HasDefaultSchema(DefaultSchema);

        public KabutanPostgreSQLContext()
            : base(Con(), true)
        {
            DefaultSchema = "";
        }
        static private NpgsqlConnection Con()
        {
            var kabutanPostgreSQLcon = ConfigurationManager.AppSettings;
            var sb = new StringBuilder();
                sb.Append($"Server={kabutanPostgreSQLcon.Get("Server")};")
                .Append($"Port={kabutanPostgreSQLcon.Get("Port")};")
                .Append($"Database={kabutanPostgreSQLcon.Get("Database")};")
                .Append($"User Id={kabutanPostgreSQLcon.Get("User Id")};")
                .Append($"Password={kabutanPostgreSQLcon.Get("Password")};");
                return new NpgsqlConnection(sb.ToString());
        }
        public DbSet<KaiziItem> KaiziItems { get; set; }
    }
    //株探DB操作の拡張用
    static public class KabutanPostgreSQLContextExtension
    {
        //重複チェック付きの適時開示DB登録
        static public void RegistrationKaiziItemsWithIsDuplicate(this KabutanPostgreSQLContext con, IEnumerable<KaiziItem> kaiziItems)
        {
            foreach(var kaiziItem in kaiziItems)
            {                
                //重複していたら飛ばす
                if(IsDuplicatePDFURL(con, kaiziItem.PDFURL))
                {
                    continue;                    
                }
                //適時開示をDBへ登録
                con.KaiziItems.Add(kaiziItem);
            }
            //DB更新
            con.SaveChanges();            
        }

        //最新のpdfURLを返す
        static public string LatestPDFURL(this KabutanPostgreSQLContext con)
        {            
            //テーブルが空なら空文字
            if(con.KaiziItems.Count() == 0)
            {
                return "";
            }            
            var latestDatetimeKaiziItem = con.KaiziItems.OrderByDescending(x => x.DateTime).FirstOrDefault();
            if(latestDatetimeKaiziItem == null)
            {
                return "";
            }
            return con.KaiziItems.Where(x => x.DateTime == latestDatetimeKaiziItem.DateTime).OrderBy(x => x.Id).First().PDFURL;
        }
        //pdfURLに重複があればtrue 無ければfalse
        static public bool IsDuplicatePDFURL(this KabutanPostgreSQLContext con, string pdfURL)
        {
            //テーブルが空ならfalse
            if (con.KaiziItems.Count() == 0)
            {
                return false;
            }
            var tmp = con.KaiziItems.FirstOrDefault(x => x.PDFURL == pdfURL);
            //pdfURLに重複があればtrue 無ければfalse
            return (tmp != null) ? true : false;

        }
    }
}
