using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR9_ThunderstormBird
{
	public class message
	{
		public static List<message> messageslist = new List<message>();
		public string Subject;
		public string From;
		public string Name;
		public string Date;
		public string TextBody;
		public string HtmlBody;
		public string To;
		public MimeMessage MimeMessage;
		public message(string subject, string from, string name, string date, string htmlBody, string textBody, MimeMessage mimeMessage)
		{
			Subject = subject;
			From = from;
			Date = date;
			HtmlBody = htmlBody;
			TextBody = textBody;
			Name = name;
			MimeMessage = mimeMessage;

		}
		public message(string subject, string from, string to, string name, string date, string htmlBody)
		{
			Subject = subject;
			From = from;
			Date = date;
			To = to;
			HtmlBody = htmlBody;
			Name = name;
		}
	}
	public static class User
	{
		public static string Address, Password = "";
		public static int imapport = 0;
		public static int smtpport = 0;
		public static string server = "google";
	}
}
