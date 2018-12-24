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

namespace KabutanLib
{
    public class KabutanPostgreSQLContext : DbContext
    {
        const string configFilePath = "../../../metadata/test.txt";

        public string DefaultSchema { get; private set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
            => modelBuilder.HasDefaultSchema(DefaultSchema);

        public KabutanPostgreSQLContext()
            : base(Con(), true)
        {
            DefaultSchema = "";
        }
        static public NpgsqlConnection Con()
        {
            using (var sr = new StreamReader(configFilePath))
            {
                var sb = new StringBuilder();
                sb.Append($"Server={sr.ReadLine()};")
                .Append($"Port={sr.ReadLine()};")
                .Append($"Database={sr.ReadLine()};")
                .Append($"User Id={sr.ReadLine()};")
                .Append($"Password={sr.ReadLine()};");
                return new NpgsqlConnection(sb.ToString());
            }
        }
        public DbSet<KaiziItem> KaiziItems { get; set; }
    }
}
