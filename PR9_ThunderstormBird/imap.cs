using ImapX;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ImapClient = MailKit.Net.Imap.ImapClient;

namespace PR9_ThunderstormBird
{
	internal class imap
	{
		public static List<MimeMessage> Messages;
		public async Task GetMessages(string folder, int amount)
		{
			var Messages = new List<MimeMessage>();

			using (var client = new ImapClient())
			{
				await client.ConnectAsync($"imap.{User.server}.com", User.smtpport, true);
				await client.AuthenticateAsync(User.Address, User.Password);
				IMailFolder inbox;
				if (folder != null)
				{
					inbox = client.GetFolder(folder);
				}
				else
				{
					inbox = client.Inbox;
				}
				try
				{
					inbox.Open(FolderAccess.ReadOnly);
				}
				catch
				{

				}
				await inbox.OpenAsync(FolderAccess.ReadOnly);

				

				for (int i = inbox.Count; i > inbox.Count - amount - 1; i--)
				{
					try
					{
						var message = await inbox.GetMessageAsync(i);
						Messages.Add(message);
					}
					catch { }
				}
			}
		}

		public async Task<IList<IMailFolder>> GetFolders()
		{
			IList<IMailFolder> folders;
			using (var client = new ImapClient())
			{
				await client.ConnectAsync($"imap.{User.server}.com", User.smtpport, true);
				await client.AuthenticateAsync(User.Address, User.Password);

				folders = client.GetFolders(client.PersonalNamespaces[0]);
				return folders;
			}
		}
	}
}
