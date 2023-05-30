using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;
using ImapX;
using ImapX.Collections;
using Org.BouncyCastle.Crypto;

namespace PR9_ThunderstormBird
{
	internal class LoggedUser
	{
		public string Email { get; set; }
		public string Pass { get; set; }
		public string SmtpHost { get; set; }
	}
	internal class ImapHelper
	{
		private static ImapClient client { get; set; }
		private static LoggedUser loggedUser { get; set; } = new LoggedUser();
		public static void Initialize(string host)
		{
			client = new ImapClient(host, true);
			if (!client.Connect())
			{
				throw new Exception("Error connecting to the client.");
			}
			else
			{
				loggedUser.SmtpHost = host.Replace("imap", "smtp");
			}
		}
		public static bool Login(string u, string p)
		{
			loggedUser.Email = u;
			loggedUser.Pass = p;
			return client.Login(u, p);

		}
		public static void Logout()
		{
			// Remove the login value from the client.
			if (client.IsAuthenticated)
			{
				client.Logout();
				client.Dispose();
			}
		}
		public static CommonFolderCollection GetFolders()
		{
			client.Folders.Inbox.StartIdling(); // And continue to listen for more.
			client.Folders.Inbox.OnNewMessagesArrived += Inbox_OnNewMessagesArrived;
			return client.Folders;
		}
		private static void Inbox_OnNewMessagesArrived(object sender, IdleEventArgs e)
		{
			// Show a dialog
			System.Windows.MessageBox.Show($"Пришло новое сообщение в папку {e.Folder.Name}.");
		}
		public static MessageCollection GetMessagesForFolder(string name)
		{
			client.Folders[name].Messages.Download(); // Start the download process;
			return client.Folders[name].Messages;
		}
		public static LoggedUser GetCredentials()
		{
			return loggedUser;
		}
	}
}
