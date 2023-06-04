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
using MailKit.Security;
using System.Security.Cryptography;
using ListBox = System.Windows.Controls.ListBox;
using ImapClient = MailKit.Net.Imap.ImapClient;
using System.Text;
using Application = System.Windows.Application;
using Wpf.Ui.Appearance;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MahApps.Metro.Controls;
using System.Net.Http;

namespace PR9_ThunderstormBird
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : UiWindow
	{
		public static string myemailaddress = "f71251@gmail.com";
		public static string mypassword = "tlkpihvlhaioagci";
		public static string serveraddress = "gmail.com";
		public static int smtpport = 993;
		public static int imapport = 465;

		private readonly Regex _regex = new Regex("[^0-9\\.]+");
		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = _regex.IsMatch(e.Text);
		}
		public MainWindow()
		{
			InitializeComponent();
			LoadMailAsync(false, null);
			Theme.Apply(ThemeType.Unknown, BackgroundType.Unknown, true);
		}


		private async void MessagesLbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var cts = new CancellationTokenSource();
			

			if (MessagesLbx.SelectedIndex > -1)
			{
				webview.Visibility = Visibility.Visible;
				string dateString = message.messageslist[MessagesLbx.SelectedIndex].Date.ToString();
				try
				{
					DateTime dateTime = DateTime.ParseExact(dateString, "dd.MM.yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture);

					int latestPart = 30; // Replace with the latest part value
					TimeSpan latestTime = TimeSpan.FromMinutes(latestPart);
					dateTime = dateTime.Add(latestTime);
					dateString = dateTime.ToString();
				}
				catch { }


				MailOpenedSender.Content = message.messageslist[MessagesLbx.SelectedIndex].From + " " + dateString;
				MailOpenedSubject.Content = message.messageslist[MessagesLbx.SelectedIndex].Subject;
				string html = message.messageslist[MessagesLbx.SelectedIndex].HtmlBody;
				using (File.Create("sample.html")) { }
				File.WriteAllText("sample.html", html.Replace("<body", "<body style=\"background-color: black;\">"));

				webview.NavigateToString(html);

				return;

				SautinSoft.HtmlToRtf h = new SautinSoft.HtmlToRtf();
				string inputFile = @"sample.html";
				// You want to save in RTF.
				string outputFile = @"result.rtf";
				h.OpenHtml(inputFile);
				h.ToRtf(outputFile);
				MailOpened.Document.Blocks.Clear();
				FileStream filesStream = new FileStream("result.rtf", FileMode.Open);
				TextRange range = new TextRange(MailOpened.Document.ContentStart, MailOpened.Document.ContentEnd);
				range.Load(filesStream, System.Windows.DataFormats.Rtf);
				filesStream.Close();

				return;

				//string dateString = message.messageslist[MessagesLbx.SelectedIndex].Date.ToString();
				try
				{
					DateTime dateTime = DateTime.ParseExact(dateString, "dd.MM.yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture);

					int latestPart = 30; // Replace with the latest part value
					TimeSpan latestTime = TimeSpan.FromMinutes(latestPart);
					dateTime = dateTime.Add(latestTime);
					dateString = dateTime.ToString();
				}
				catch { }

				MailOpenedSender.Content = message.messageslist[MessagesLbx.SelectedIndex].From + " " + dateString;
				MailOpenedSubject.Content = message.messageslist[MessagesLbx.SelectedIndex].Subject;
				try
				{
					//string html = message.messageslist[MessagesLbx.SelectedIndex].HtmlBody;
					await Task.Run(() =>
					{
						cts.Token.ThrowIfCancellationRequested();
						try
						{
							
							
							
						}
						catch { 
							cts.Cancel();
						}
						
					});
					

					Application.Current.Dispatcher.Invoke(() =>
					{

						Task.Run(() => {
							Application.Current.Dispatcher.Invoke(() =>
							{
								

								using (FileStream fileStream = new FileStream("result.rtf", FileMode.Open))
								{
									MailOpened.Document.Blocks.Clear();
									MailOpened.Document.Blocks.Add(new Paragraph(new Run("Loading 50%")));
									TextRange textRange = new TextRange(MailOpened.Document.ContentStart, MailOpened.Document.ContentEnd);
									MailOpened.Document.Blocks.Clear();
									MailOpened.Document.Blocks.Add(new Paragraph(new Run("Loading 80%")));

									try
									{
										MailOpened.Document.Blocks.Clear();
										textRange.Load(fileStream, System.Windows.DataFormats.Rtf);
									}
									catch (Exception ащибка)
									{
										System.Windows.MessageBox.Show(ащибка.Message + ащибка.InnerException + ащибка.Data);
									}

									System.Windows.MessageBox.Show("Loaded");

								}
								
							});

						});
						Task.Delay(300).ContinueWith(task => {
							Application.Current.Dispatcher.Invoke(() =>
							{
								File.Delete("msg.html");
								File.Delete("msg.rtf");
							});

						});
						
					});
				}
				catch (Exception expt)
				{
					System.Windows.MessageBox.Show(expt.Message + expt.Source + expt.InnerException + expt.Data);
					MailOpened.Document.Blocks.Clear();
					MailOpened.Document.Blocks.Add(new Paragraph(new Run("There are nothing")));
					try
					{
						MailOpened.Document.Blocks.Clear();
						MailOpened.Document.Blocks.Add(new Paragraph(new Run(message.messageslist[MessagesLbx.SelectedIndex].TextBody)));
					}
					catch (Exception exce)
					{
						System.Windows.MessageBox.Show(exce.Message + exce.Source + exce.InnerException + exce.Data);
						MailOpened.Document.Blocks.Clear();
						MailOpened.Document.Blocks.Add(new Paragraph(new Run("There are nothing")));
					}

				}
			}
		}



		private async void Update_Click(object sender, RoutedEventArgs e)
		{
			await LoadMailAsync(false, null);
		}

		public async Task LoadMailAsync(bool folder, string foldername)
		{
			
			using (var client = new ImapClient())
			{
				await client.ConnectAsync("imap.gmail.com", 993, true);
				await client.AuthenticateAsync(myemailaddress, mypassword);

				IMailFolder inbox;

				if (folder)
				{
					inbox = client.GetFolder(foldername);
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
					msgerrors.Show("Troubles with opening this folder");
					return;
				}

				

				await inbox.OpenAsync(FolderAccess.ReadOnly);
				var folders = client.GetFolders(client.PersonalNamespaces[0]);
				FoldersLbx.ItemsSource = folders;

				List<string> msg = new List<string>();
				msg.Clear();
				message.messageslist.Clear();
				Progress.Maximum = Convert.ToInt32(MsgAmount.Text);
				Progress.Value = 0;


				for (int i = inbox.Count; i > inbox.Count - Convert.ToInt32(MsgAmount.Text) - 1; i--)
				{
					
					AppLabel.Content = (Convert.ToInt32(MsgAmount.Text) - inbox.Count + i).ToString();
					Progress.Value += 1;
					try
					{
						var message = await inbox.GetMessageAsync(i);

						string email = message.From.ToString();

						string dateString = message.Date.ToString();
						try
						{
							DateTime dateTime = DateTime.ParseExact(dateString, "dd.MM.yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture);

							int latestPart = 30; // Replace with the latest part value
							TimeSpan latestTime = TimeSpan.FromMinutes(latestPart);
							dateTime = dateTime.Add(latestTime);

							dateString = dateTime.ToString();
						}
						catch
						{
							dateString = "Missed date";
						}

						int indexStart = email.IndexOf('"') + 1;
						int indexEnd = email.IndexOf('"', indexStart);
						string firstPart = email.Substring(indexStart, indexEnd - indexStart);
						string secondPart = email.Substring(email.IndexOf('<') + 1, email.IndexOf('>') - email.IndexOf('<') - 1);
						string subject = "Missing";
						string date = "Missing";
						string htmlBody = "Missing";
						string textBody = "Missing";

						try
						{
							subject = message.Subject.ToString();
						}
						catch (Exception ex)
						{

						}

						try
						{
							date = message.Date.ToString();
						}
						catch (Exception ex)
						{

						}

						try
						{
							htmlBody = message.HtmlBody.ToString();
						}
						catch (Exception ex)
						{

						}

						try
						{
							textBody = message.TextBody;
						}
						catch (Exception ex)
						{

						}

						
						PR9_ThunderstormBird.message.messageslist.Add(new message(subject, secondPart, firstPart, date, htmlBody, textBody));
						msg.Add(message.Subject + "\n" + firstPart + "\n" + secondPart + "\n" + dateString);
						if ((Convert.ToInt32(MsgAmount.Text) - inbox.Count + i)%50 == 0)
						{
							MessagesLbx.ItemsSource= null;
							MessagesLbx.ItemsSource= msg;
						}
					}
					catch (Exception ex)
					{

					}
				}
				

				AppLabel.Content = "ThunderstormBird";
				Progress.Value = 0;
				MessagesLbx.ItemsSource= msg;
				if (Convert.ToInt32(MsgAmount.Text) != MessagesLbx.Items.Count)
				{
					msgerrors.Show((Convert.ToInt32(MsgAmount.Text) - MessagesLbx.Items.Count - 1).ToString() + " Messages are not loaded", $"There are some errors - {(Convert.ToInt32(MsgAmount.Text) - MessagesLbx.Items.Count)} of {MsgAmount.Text} messages are not loaded");


				}
				else
				{
					msgerrors.Show("Success\n", $"All {MsgAmount.Text} messages are loaded");

				}
				System.Windows.MessageBox.Show($"{msg.Count} {message.messageslist.Count}");
				client.Disconnect(true);

				
			}

			
		}

		private async void SendBtn_Click(object sender, RoutedEventArgs e)
		{
			

			
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(myemailaddress, myemailaddress));
			message.To.Add(new MailboxAddress(SendTxb.Text, SendTxb.Text));
			message.Subject = ThemeTxb.Text;
			message.Body = new TextPart("plain")
			{
				Text = new TextRange(MessageTxb.Document.ContentStart, MessageTxb.Document.ContentEnd).Text
			};

			try
			{
				using (var client = new MailKit.Net.Smtp.SmtpClient())
				{
					await client.ConnectAsync("smtp.gmail.com", 465, true);

					client.Authenticate(myemailaddress, mypassword);

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

		private async void FoldersLbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (FoldersLbx.SelectedItem != null)
			{
				await LoadMailAsync(true, FoldersLbx.SelectedItem.ToString());
			}
		}


		private void ListTgb_Click(object sender, RoutedEventArgs e)
		{
			if (ListTgb.IsChecked == false)
			{
				Left.MinWidth = 0;
				ColumnDefinition col = Root.ColumnDefinitions[0]; // Replace 0 with the index of the column you want to animate
				GridLengthAnimation anim = new GridLengthAnimation();
				anim.From = new GridLength(2, GridUnitType.Pixel);
				anim.To = new GridLength(0, GridUnitType.Pixel);
				anim.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
				col.BeginAnimation(ColumnDefinition.WidthProperty, anim);

				//Left.Width = new GridLength(0);

			}
			if (ListTgb.IsChecked == true)
			{
				
				ColumnDefinition col = Root.ColumnDefinitions[0]; // Replace 0 with the index of the column you want to animate
				GridLengthAnimation anim = new GridLengthAnimation();
				anim.From = new GridLength(0, GridUnitType.Pixel);
				anim.To = new GridLength(2, GridUnitType.Pixel);
				anim.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
				col.BeginAnimation(ColumnDefinition.WidthProperty, anim);
				

				Task.Delay(200).ContinueWith(task => {
					Application.Current.Dispatcher.Invoke(() =>
					{
						Left.MinWidth = 215;
						Left.Width = new GridLength(Right.ActualWidth, GridUnitType.Pixel);
					});
					
				});
			}
		}

		private void WriteTgb_Click(object sender, RoutedEventArgs e)
		{
			if (WriteTgb.IsChecked == false)
			{
				Right.MinWidth = 0;
				ColumnDefinition col = Root.ColumnDefinitions[2]; // Replace 0 with the index of the column you want to animate
				GridLengthAnimation anim = new GridLengthAnimation();
				anim.From = new GridLength(2.4, GridUnitType.Pixel);
				anim.To = new GridLength(0, GridUnitType.Pixel);
				anim.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
				col.BeginAnimation(ColumnDefinition.WidthProperty, anim);

			}
			if (WriteTgb.IsChecked == true)
			{
				Right.MinWidth = 100;
				ColumnDefinition col = Root.ColumnDefinitions[2]; // Replace 0 with the index of the column you want to animate
				GridLengthAnimation anim = new GridLengthAnimation();
				anim.From = new GridLength(0, GridUnitType.Pixel);
				anim.To = new GridLength(2.4, GridUnitType.Pixel);
				anim.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
				col.BeginAnimation(ColumnDefinition.WidthProperty, anim);

				Task.Delay(300).ContinueWith(task => {
					Application.Current.Dispatcher.Invoke(() =>
					{
						Right.MinWidth = 215;
						Right.Width = new GridLength(Right.ActualWidth, GridUnitType.Pixel);

					});
					
				});

			}
		}

		private void FoldersTgb_Click(object sender, RoutedEventArgs e)
		{
			if (FoldersTgb.IsChecked == false)
			{
				RowDefinition col = Root.RowDefinitions[0]; // Replace 0 with the index of the column you want to animate
				GridLengthAnimation anim = new GridLengthAnimation();
				anim.From = new GridLength(1.5, GridUnitType.Pixel);
				anim.To = new GridLength(0.60, GridUnitType.Pixel);
				anim.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 400));
				col.BeginAnimation(RowDefinition.HeightProperty, anim);
				

				Task.Delay(300).ContinueWith(task => {
					Application.Current.Dispatcher.Invoke(() =>
					{
						Root.RowDefinitions[0].MaxHeight = 40;

					});

				});
			}
			if (FoldersTgb.IsChecked == true)
			{
				Root.RowDefinitions[0].MaxHeight = 90;
				RowDefinition col = Root.RowDefinitions[0]; // Replace 0 with the index of the column you want to animate
				GridLengthAnimation anim = new GridLengthAnimation();
				anim.From = new GridLength(0.60, GridUnitType.Pixel);
				anim.To = new GridLength(1.5, GridUnitType.Pixel);
				anim.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 400));
				col.BeginAnimation(RowDefinition.HeightProperty, anim);
				
			}
		}

		private void ThemeTgb_Click(object sender, RoutedEventArgs e)
		{
			
			if (ThemeTgb.IsChecked == true)
			{
				Theme.Apply(ThemeType.Light, BackgroundType.Tabbed, true, true);
				MailOpened.Foreground = Brushes.Black;

			}
			if (ThemeTgb.IsChecked == false)
			{
				Theme.Apply(ThemeType.Dark, BackgroundType.Tabbed, true, true);
				MailOpened.Foreground = Brushes.White;
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
