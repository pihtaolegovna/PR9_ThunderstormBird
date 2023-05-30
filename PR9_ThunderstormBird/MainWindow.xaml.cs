using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Spire.Doc;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Xml.Linq;
using Wpf.Ui.Controls;
using System.Windows.Forms;
using Wpf.Ui.Appearance;
using System.Threading.Tasks;

namespace PR9_ThunderstormBird
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : UiWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}
		private void MessagesLbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				File.WriteAllText("msg.html", message.messageslist[MessagesLbx.SelectedIndex].HtmlBody);
				var d = new Document("msg.html", FileFormat.Html);
				d.SaveToFile("msg.rtf", FileFormat.Rtf);
				d.Close();
				using (FileStream fileStream = new FileStream("msg.rtf", FileMode.Open))
				{
					Dispatcher.BeginInvoke(new Action(() =>
					{
						TextRange textRange = new TextRange(MailOpened.Document.ContentStart, MailOpened.Document.ContentEnd);
						textRange.Load(fileStream, System.Windows.DataFormats.Rtf);
					}));
				}
				File.Delete("msg.html");
			}
			catch (Exception er)
			{
				System.Windows.MessageBox.Show(er.Message);
			}
		}

		private async void Update_Click(object sender, RoutedEventArgs e)
		{
			await LoadMailAsync();
		}

		public async Task LoadMailAsync()
		{
			using (var client = new ImapClient())
			{
				await client.ConnectAsync("imap.gmail.com", 993, true);
				await client.AuthenticateAsync("f71251@gmail.com", "tlkpihvlhaioagci");

				var inbox = client.Inbox;
				await inbox.OpenAsync(FolderAccess.ReadOnly);

				List<string> msg = new List<string>();
				Progress.Maximum = 100;
				Progress.Value = 0;

				for (int i = 3570; i < 3580; i++)
				{
					
					Progress.Value += 1;
					try
					{
						var message = await inbox.GetMessageAsync(i);
						string email = message.From.ToString();
						int indexStart = email.IndexOf('"') + 1;
						int indexEnd = email.IndexOf('"', indexStart);
						string firstPart = email.Substring(indexStart, indexEnd - indexStart);
						string secondPart = email.Substring(email.IndexOf('<') + 1, email.IndexOf('>') - email.IndexOf('<') - 1);
						PR9_ThunderstormBird.message.messageslist.Add(new message(message.Subject.ToString(), secondPart, firstPart, message.Date.ToString(), message.HtmlBody.ToString()));
						msg.Add(message.Subject + "\n" + firstPart + "\n" + secondPart + "\n" + message.Date);
					}
					catch { 
					
					}
				}

				MessagesLbx.ItemsSource= msg;
			}
		}

	}
	public class message
	{
		public static List<message> messageslist = new List<message>();

		public string Subject;
		public string From;
		public string Name;
		public string Date;
		public string HtmlBody;

		public message(string subject, string from, string name, string date, string htmlBody)
		{
			Subject = subject;
			From = from;
			Date = date;
			HtmlBody = htmlBody;
			Name = name;
		}
	}
}
