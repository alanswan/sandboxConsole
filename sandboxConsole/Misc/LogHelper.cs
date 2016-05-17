using sandboxConsole.EF;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace sandboxConsole.Misc
{
    public static class LogHelper
    {
        //private const string SendGridUsername = "alanswan";
        //private const string SendGridPassword = "clusprac99";
        //private const string EmailFromAddress = "alan.swan@wearedandify.com";

        //public object HttpContext { get; private set; }

        public static void LogTimes(int time, string bookmaker)
        {
            omproEntities db = new omproEntities();
            db.LoadTimes.Add(new LoadTime() {ElapsedTime = time, bookmaker = bookmaker, LoadDateTime = DateTime.Now });
            db.SaveChanges();

            SqlConnection conn = new SqlConnection();
            var ConnectionString =
                "data source=mssql2.gear.host;initial catalog=ompro;persist security info=True;user id=ompro;password=Co31?i!8iF74;MultipleActiveResultSets=True;";

            using (SqlConnection sc = new SqlConnection(ConnectionString))
            {
                sc.Open();
                using (var cmd = sc.CreateCommand())
                {
                    //server date is different to machine date (7 hours behind), so +5 hours should delete anything in the last 2 hours
                    cmd.CommandText = "delete from loadtimes where loaddatetime < dateadd(HH, +5, getdate()) and bookmaker = @name ";
                    cmd.Parameters.AddWithValue("@name", bookmaker);
                    cmd.ExecuteNonQuery();
                }
                
            }


        }
    }

}
