using sandboxConsole.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace sandboxConsole.Misc
{
    public static class ErrorHelper
    {
        //private const string SendGridUsername = "alanswan";
        //private const string SendGridPassword = "clusprac99";
        //private const string EmailFromAddress = "alan.swan@wearedandify.com";

        //public object HttpContext { get; private set; }

        public static void CreateError(string bookmaker, string message)
        {
            omproEntities db = new omproEntities();
            EmailHelper email = new EmailHelper();
            email.SendErrorEmail(bookmaker + " has not completed properly.<br/><br/>" + message);

            db.Errors.Add(new Error()
            {
                Error1 = message,
                bookmaker = bookmaker,
                ErrorDateTime = DateTime.Now
            });
            db.SaveChanges();
        }

       

    }

}
