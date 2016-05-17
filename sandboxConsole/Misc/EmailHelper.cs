using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace sandboxConsole.Misc
{
    public class EmailHelper
    {
        //private const string SendGridUsername = "alanswan";
        //private const string SendGridPassword = "clusprac99";
        //private const string EmailFromAddress = "alan.swan@wearedandify.com";

        //public object HttpContext { get; private set; }

        public void SendErrorEmail(string message)
        {
            System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
                 new System.Net.Mail.MailAddress("mycheekyprofits@wearedandify.com", "My Cheeky Profits"),
                 new System.Net.Mail.MailAddress("swan_e@hotmail.co.uk"));

            m.Subject = "Service Error";
  

            //HTML Message
            m.Body = "You have an error: " + message;

            m.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("secure170.inmotionhosting.com");
            smtp.Credentials = new System.Net.NetworkCredential("mycheekyprofits@wearedandify.com", "clusprac99");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }

       

    }

}
