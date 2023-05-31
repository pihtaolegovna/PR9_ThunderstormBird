using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Spire.Doc;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using Wpf.Ui.Controls;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Forms;

namespace PR9_ThunderstormBird
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : UiWindow
	{
		public static string user_address;
		private readonly Regex _regex = new Regex("[^0-9\\.]+");
		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = _regex.IsMatch(e.Text);
		}
		public MainWindow()
		{
			InitializeComponent();
		}
		private void MessagesLbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			MailOpened.Document.Blocks.Clear();
			MailOpened.Document.Blocks.Add(new Paragraph(new Run("Loading")));

			try
			{
				MailOpened.Document.Blocks.Clear();

				if (message.messageslist[MessagesLbx.SelectedIndex].TextBody == "No Content Available") throw new Exception();
				MailOpened.Document.Blocks.Add(new Paragraph(new Run(message.messageslist[MessagesLbx.SelectedIndex].TextBody)));

				
			}
			catch (Exception er)
			{
				try
				{
					try { File.Delete("msg.rtf"); } catch { }
					File.WriteAllText("msg.html", message.messageslist[MessagesLbx.SelectedIndex].HtmlBody);
					var d = new Document("msg.html", FileFormat.Html);
					d.SaveToFile("msg.rtf", FileFormat.Rtf);
					d.Close();
					MailOpened.Document.Blocks.Clear();
					MailOpened.Document.Blocks.Add(new Paragraph(new Run("Rtf Generated")));
					using (FileStream fileStream = new FileStream("msg.rtf", FileMode.Open))
					{
						TextRange textRange = new TextRange(MailOpened.Document.ContentStart, MailOpened.Document.ContentEnd);
						textRange.Load(fileStream, System.Windows.DataFormats.Rtf);
					}
					File.Delete("msg.html");
					File.Delete("msg.rtf");
				}
				catch
				{
					try
					{
						MailOpened.Document.Blocks.Clear();
						MailOpened.Document.Blocks.Add(new Paragraph(new Run(message.messageslist[MessagesLbx.SelectedIndex].TextBody)));
					}
					catch
					{
						MailOpened.Document.Blocks.Clear();
						MailOpened.Document.Blocks.Add(new Paragraph(new Run("There are nothing")));
					}
				}
			}
			Thread thread = new Thread(() =>
			{
				
			});

			thread.Start();
		}


		private async void Update_Click(object sender, RoutedEventArgs e)
		{
			await LoadMailAsync();
		}

		public async Task LoadMailAsync()
		{
			MessagesLbx.ItemsSource = null;
			using (var client = new ImapClient())
			{
				await client.ConnectAsync("imap.gmail.com", 993, true);
				await client.AuthenticateAsync("f71251@gmail.com", "tlkpihvlhaioagci");

				var inbox = client.Inbox;
				await inbox.OpenAsync(FolderAccess.ReadOnly);

				List<string> msg = new List<string>();
				Progress.Maximum = Convert.ToInt32(MsgAmount.Text);
				Progress.Value = 0;

				for (int i = inbox.Count; i > inbox.Count - Convert.ToInt32(MsgAmount.Text); i--)
				{
					AppLabel.Content = (Convert.ToInt32(MsgAmount.Text) - inbox.Count + i).ToString();
					Progress.Value += 1;
					try
					{
						var message = await inbox.GetMessageAsync(i);
						string email = message.From.ToString();

						string dateString = message.Date.ToString();
						DateTime dateTime = DateTime.ParseExact(dateString, "dd.MM.yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture);

						int latestPart = 30; // Replace with the latest part value
						TimeSpan latestTime = TimeSpan.FromMinutes(latestPart);
						dateTime = dateTime.Add(latestTime);

						int indexStart = email.IndexOf('"') + 1;
						int indexEnd = email.IndexOf('"', indexStart);
						string firstPart = email.Substring(indexStart, indexEnd - indexStart);
						string secondPart = email.Substring(email.IndexOf('<') + 1, email.IndexOf('>') - email.IndexOf('<') - 1);
						PR9_ThunderstormBird.message.messageslist.Add(new message(message.Subject.ToString(), secondPart, firstPart, message.Date.ToString(), message.HtmlBody.ToString(), (message.TextBody != null ? message.TextBody.ToString() : "No Content Available")));
						msg.Add(message.Subject + "\n" + firstPart + "\n" + secondPart + "\n" + dateTime);
					}
					catch {
						
					}
				}
				AppLabel.Content = "ThunderstormBird";
				Progress.Value = 0;
				MessagesLbx.ItemsSource= msg;

				client.Disconnect(true);
			}
		}

		private async void SendBtn_Click(object sender, RoutedEventArgs e)
		{
			

			var sendmessage = new message(null, user_address, SendTxb.Text, ThemeTxb.Text, DateTime.Now.ToString(), null, new TextRange(MessageTxb.Document.ContentStart, MessageTxb.Document.ContentEnd).Text);

			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("f71251@gmail.com", "f71251@gmail.com"));
			message.To.Add(new MailboxAddress("f71251@gmail.com", "f71251@gmail.com"));
			message.Subject = "How you doin'?";

			message.Body = new TextPart("plain")
			{
				Text = new TextRange(MessageTxb.Document.ContentStart, MessageTxb.Document.ContentEnd).Text
			};

			try
			{
				using (var client = new MailKit.Net.Smtp.SmtpClient())
				{
					await client.ConnectAsync("smtp.gmail.com", 465, true);

					// Note: only needed if the SMTP server requires authentication
					client.Authenticate("f71251@gmail.com", "tlkpihvlhaioagci");

					client.Send(message);
					client.Disconnect(true);
				}
			}
			catch (Exception exc)
			{
				var msb = new Wpf.Ui.Controls.MessageBox();
				msb.Show("Error", exc.Message);
			}
			var msbox = new Wpf.Ui.Controls.MessageBox();
			msbox.Show("All is Ok", "Message Sended");
		}
	}
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

		public message(string subject, string from, string name, string date, string htmlBody, string textBody)
		{
			Subject = subject;
			From = from;
			Date = date;
			HtmlBody = htmlBody;
			TextBody = textBody;
			Name = name;
		}
		public message(string subject, string from, string to, string name, string date, string htmlBody, string textBody)
		{
			Subject = subject;
			From = from;
			Date = date;
			To = to;
			HtmlBody = htmlBody;
			TextBody = textBody;
			Name = name;
		}
	}

	public class SmtpClient
	{

	}
}
