using MimeKit;
using Spire.Doc;
using Spire.Doc.Fields;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using TextRange = System.Windows.Documents.TextRange;

namespace PR9_ThunderstormBird
{
	internal static class smtp
	{
		public static async Task sendmessage(string to, string subject, MimeEntity body)
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(User.Address, User.Address));
			message.To.Add(new MailboxAddress(to, to));
			message.Subject = subject;
			message.Body = body;

			var msbox = new Wpf.Ui.Controls.MessageBox();
			try
			{
				using (var client = new MailKit.Net.Smtp.SmtpClient())
				{
					await client.ConnectAsync($"smtp.{User.server}.com", User.imapport, true);

					client.Authenticate(User.Address, User.Password);

					client.Send(message);
					client.Disconnect(true);

				}
			}
			catch (Exception exc)
			{
				var msb = new Wpf.Ui.Controls.MessageBox();
				msb.Show("Error", exc.Message);
				return;
			}
			msbox.Show("All is Ok", "Message Sended");
		}
	}
}
