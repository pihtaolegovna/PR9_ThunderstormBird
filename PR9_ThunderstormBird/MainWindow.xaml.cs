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
using DataFormats = System.Windows.DataFormats;
using IDataObject = System.Windows.IDataObject;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using ImapX;
using System.Net;

namespace PR9_ThunderstormBird
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : UiWindow
	{
		

		private readonly Regex _regex = new Regex("[^0-9\\.]+");
		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = _regex.IsMatch(e.Text);
		}
		public MainWindow()
		{
			InitializeComponent();
			LoadMailAsync(false, null);
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

					int latestPart = 30;
					TimeSpan latestTime = TimeSpan.FromMinutes(latestPart);
					dateTime = dateTime.Add(latestTime);
					dateString = dateTime.ToString();
				}
				catch { }
				MailOpenedSender.Content = message.messageslist[MessagesLbx.SelectedIndex].From + " " + dateString;
				MailOpenedSubject.Content = message.messageslist[MessagesLbx.SelectedIndex].Subject;
				string html = message.messageslist[MessagesLbx.SelectedIndex].HtmlBody;

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

				MailOpened.Document.Blocks.Clear();
				MailOpened.Document.Blocks.Add(new Paragraph(new Run(message.messageslist[MessagesLbx.SelectedIndex].TextBody)));
				
				return;

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
			try
			{
				using (var client = new ImapClient())
				{
					await client.ConnectAsync($"imap.{User.server}.com", User.smtpport, true);
					await client.AuthenticateAsync(User.Address, User.Password);
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
						webview.Visibility = Visibility.Hidden;
						msgerrors.Show("Troubles with opening this folder", foldername);
						return;
					}
					await inbox.OpenAsync(FolderAccess.ReadOnly);
					var folders = client.GetFolders(client.PersonalNamespaces[0]);
					FoldersLbx.ItemsSource = folders;
					List<string> msg = new List<string>();
					message.messageslist.Clear();
					msg.Clear();
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
							string dateString;
							try
							{
								DateTime dateTime = DateTime.ParseExact(message.Date.ToString(), "dd.MM.yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture);
								int latestPart = 30; //	
								TimeSpan latestTime = TimeSpan.FromMinutes(latestPart);
								dateTime = dateTime.Add(latestTime);
								dateString = dateTime.ToString();
							}
							catch
							{
								dateString = message.Date.ToString();
							}
							int indexStart = email.IndexOf('"') + 1;
							int indexEnd = email.IndexOf('"', indexStart);
							string firstPart = email.Substring(indexStart, indexEnd - indexStart);
							string secondPart = email.Substring(email.IndexOf('<') + 1, email.IndexOf('>') - email.IndexOf('<') - 1);
							string date = "Missing";
							string htmlBody = "Missing";
							string textBody = "Missing";
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
							try
							{
								PR9_ThunderstormBird.message.messageslist.Add(new message(message.Subject, secondPart, firstPart, date, htmlBody, textBody, message));
								msg.Add(message.Subject + "\n" + firstPart + "\n" + secondPart + "\n" + dateString);
							}
							catch { }

							if ((Convert.ToInt32(MsgAmount.Text) - inbox.Count + i)%10 == 0)
							{
								MessagesLbx.ItemsSource= null;
								MessagesLbx.ItemsSource= msg;
							}
						}
						catch
						{ }
					}
					AppLabel.Content = "ThunderstormBird";
					Progress.Value = 0;
					MessagesLbx.ItemsSource= msg;
					if (Convert.ToInt32(MsgAmount.Text) != MessagesLbx.Items.Count)
					{
						webview.Visibility = Visibility.Hidden;
						msgerrors.Show((Convert.ToInt32(MsgAmount.Text) - MessagesLbx.Items.Count - 1).ToString() + " Messages are not loaded", $"There are some errors - {(Convert.ToInt32(MsgAmount.Text) - MessagesLbx.Items.Count)} of {MsgAmount.Text} messages are not loaded");
					}
					else
					{
						webview.Visibility = Visibility.Hidden;
						msgerrors.Show("Success\n", $"All {MsgAmount.Text} messages are loaded");
					}
					client.Disconnect(true);
				}
			}
			catch (System.Net.Sockets.SocketException)
			{
				Wpf.Ui.Controls.MessageBox mbx = new Wpf.Ui.Controls.MessageBox();

				mbx.Show("Ошибка авторизации", "Нет подключения к интернету");
			}
			catch (MailKit.Security.AuthenticationException)
			{
				Wpf.Ui.Controls.MessageBox mbx = new Wpf.Ui.Controls.MessageBox();
				mbx.Show("Ошибка авторизации", "Проблемы с сессией");
			}
		}
		private void exit()
		{
			User.Address = "";
			User.Password = "";
			User.imapport = 0;
			User.smtpport = 0;
			User.server = "";
			this.Close();
			new Auth().Show();
		}
		private void BoldText()
		{
			if (MessageTxb.Selection.GetPropertyValue(TextElement.FontWeightProperty).Equals(FontWeights.Bold))
			{
				MessageTxb.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
			}
			else
			{
				MessageTxb.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
			}
			MessageTxb.Focus();
		}

		private void ItalicText()
		{
			if (MessageTxb.Selection.GetPropertyValue(TextElement.FontStyleProperty).Equals(FontStyles.Italic))
			{
				MessageTxb.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
			}
			else
			{
				MessageTxb.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
			}
			MessageTxb.Focus();
		}

		private void BoldBtn_Click(object sender, RoutedEventArgs e)
		{
			BoldText();
		}

		private void ItalicBtn_Click(object sender, RoutedEventArgs e)
		{
			ItalicText();
		}

		private void UnderlineBtn_Click(object sender, RoutedEventArgs e)
		{
			UnderlineText();
		}

		private void UnderlineText()
		{
			if (MessageTxb.Selection.GetPropertyValue(Inline.TextDecorationsProperty).Equals(TextDecorations.Underline))
			{
				MessageTxb.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
			}
			else
			{
				TextDecorationCollection textDecorations = new TextDecorationCollection();
				textDecorations.Add(TextDecorations.Underline);
				MessageTxb.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
			}
			MessageTxb.Focus();
		}

		private void Clear_Click(object sender, RoutedEventArgs e)
		{
			MessageTxb.Document.Blocks.Clear();
		}

		private void StrikethroughBtn_Click(object sender, RoutedEventArgs e)
		{
			if (MessageTxb.Selection.GetPropertyValue(TextBlock.TextDecorationsProperty).Equals(TextDecorations.Strikethrough))
			{
				MessageTxb.Selection.ApplyPropertyValue(TextBlock.TextDecorationsProperty, null);
			}
			else
			{
				MessageTxb.Selection.ApplyPropertyValue(TextBlock.TextDecorationsProperty, TextDecorations.Strikethrough);
			}
			MessageTxb.Focus();
		}

		private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			try
			{
				SendTxb.Text = PR9_ThunderstormBird.message.messageslist[MessagesLbx.SelectedIndex].From;
			}
			catch { }
		}

		private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			SendTxb.Text = null;
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
				anim.To = new GridLength(0, GridUnitType.Star);
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
						Right.Width = new GridLength(2, GridUnitType.Star);
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
			try
			{
				if (ThemeTgb.IsChecked == true)
				{
					Theme.Apply(ThemeType.Light, BackgroundType.Unknown, true, true);
					MailOpened.Foreground = Brushes.Black;

				}
				if (ThemeTgb.IsChecked == false)
				{
					Theme.Apply(ThemeType.Dark, BackgroundType.Unknown, true, true);
					MailOpened.Foreground = Brushes.White;
				}
			}
			catch (Exception емае)
			{
			}
		}

		private async void SendBtn_Click(object sender, RoutedEventArgs e)
		{
			var msbox = new Wpf.Ui.Controls.MessageBox();
			if (MessagesLbx.SelectedIndex > -1 && SendTxb.Text != null && ThemeTxb.Text != null)
			{
				try
				{
					TextRange textRange = new TextRange(MessageTxb.Document.ContentStart, MessageTxb.Document.ContentEnd);
					var fs = new FileStream("send.rtf", FileMode.Create);
					textRange.Save(fs, DataFormats.Rtf);
					fs.Close();
					var d = new Document("send.rtf", FileFormat.Rtf);
					d.SaveToFile("send.html", FileFormat.Html);
					d.Close();
					File.Delete("send.rtf");
					var body = new TextPart("html")
					{
						Text = File.ReadAllText("send.html")
					};
					await smtp.sendmessage(SendTxb.Text, ThemeTxb.Text, body);
				}
				catch (Exception Exc)
				{
					msbox.Show("Error", $"Error while Sending Message Occured\n{Exc.Message}");
				}
			}
		}
		private async void Reply_Click(object sender, RoutedEventArgs e)
		{
			var msbox = new Wpf.Ui.Controls.MessageBox();
			if (MessagesLbx.SelectedIndex > -1 && SendTxb.Text != null && ThemeTxb.Text != null)
			{
				try
				{
					TextRange textRange = new TextRange(MessageTxb.Document.ContentStart, MessageTxb.Document.ContentEnd);
					var fs = new FileStream("send.rtf", FileMode.Create);
					textRange.Save(fs, DataFormats.Rtf);
					fs.Close();
					var d = new Document("send.rtf", FileFormat.Rtf);
					d.SaveToFile("send.html", FileFormat.Html);
					d.Close();
					File.Delete("send.rtf");
					var htmlBody = File.ReadAllText("send.html");
					var existingBody = PR9_ThunderstormBird.message.messageslist[MessagesLbx.SelectedIndex].MimeMessage.Body;
					var multipart = new Multipart("mixed");
					multipart.Add(existingBody);
					multipart.Add(new TextPart("html", htmlBody));
					await smtp.sendmessage(SendTxb.Text, ThemeTxb.Text, multipart);
				}
				catch (Exception Exc)
				{
					msbox.Show("Error", $"Error while Sending Message Occured\n{Exc.Message}");
				}
			}
		}

		private async void Forward_Click(object sender, RoutedEventArgs e)
		{
			var msbox = new Wpf.Ui.Controls.MessageBox();
			if (MessagesLbx.SelectedIndex > -1 && SendTxb.Text != null && ThemeTxb.Text != null)
			{
				try
				{
					await smtp.sendmessage(SendTxb.Text, ThemeTxb.Text, PR9_ThunderstormBird.message.messageslist[MessagesLbx.SelectedIndex].MimeMessage.Body);
				}
				catch (Exception Exc)
				{
					msbox.Show("Error", $"Error while Sending Message Occured\n{Exc.Message}");
				}
			}
		}
		private void SwitchTgb_Click(object sender, RoutedEventArgs e)
		{
			if (webview.Visibility == Visibility.Hidden) webview.Visibility = Visibility.Hidden; else webview.Visibility = Visibility.Visible;
		}
		private void ThemeTxb_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (ThemeTxb.Text != null && ThemeTxb.Text != String.Empty && ThemeTxb.Text.Length > 0 && SendTxb.Text != null && SendTxb.Text != String.Empty && SendTxb.Text.Length > 0)
			{
				SendBtn.IsEnabled = true;
				ReplyBtn.IsEnabled = true;
				ForwardBtn.IsEnabled = true;
			}
			else
			{
				SendBtn.IsEnabled = false;
				ReplyBtn.IsEnabled = false;
				ForwardBtn.IsEnabled = false;
			}
			if (ThemeTxb.Text != null && ThemeTxb.Text != String.Empty && ThemeTxb.Text.Length > 0)
			{
				ReplyBtn.IsEnabled = true;
			}
			else
			{
				ReplyBtn.IsEnabled = false;
			}
		}

		private void SendTxb_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (ThemeTxb.Text != null && ThemeTxb.Text != String.Empty && ThemeTxb.Text.Length > 0 && SendTxb.Text != null && SendTxb.Text != String.Empty && SendTxb.Text.Length > 0)
			{
				SendBtn.IsEnabled = true;
				ForwardBtn.IsEnabled = true;
			}
			else
			{
				SendBtn.IsEnabled = false;
				ForwardBtn.IsEnabled = false;
			}
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			exit();
		}
	}
}
