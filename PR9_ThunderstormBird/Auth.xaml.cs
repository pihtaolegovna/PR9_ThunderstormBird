using MailKit.Net.Imap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;

namespace PR9_ThunderstormBird
{
	/// <summary>
	/// Interaction logic for Auth.xaml
	/// </summary>
	public partial class Auth : UiWindow
	{
		public Auth()
		{
			InitializeComponent();
		}

		private async void Authorization_Click(object sender, RoutedEventArgs e)
		{
			Authorization.IsEnabled = false;
			switch (MailCbx.SelectedIndex)
			{
				case 0:
					User.Address = MailTbx.Text + "@gmail.com";
					User.server = "gmail";
					break;
				case 1:
					User.Address = MailTbx.Text + "@rambler.ru";
					User.server = "rambler";
					break;
				case 2:
					User.Address = MailTbx.Text + "@yandex.ru";
					User.server = "yandex";
					break;


			}
			User.imapport = 465;
			User.smtpport = 993;

			User.Password = PasswordTbx.Text;

			

			

			try
			{
				using (var client = new ImapClient())
				{
					await client.ConnectAsync($"imap.{User.server}.com", User.smtpport, true);
					await client.AuthenticateAsync(User.Address, User.Password);
					await client.DisconnectAsync(true);
				}
				new MainWindow().Show();
				this.Close();
			}
			catch (System.Net.Sockets.SocketException)
			{
				Authorization.IsEnabled = true;

				Wpf.Ui.Controls.MessageBox mbx = new Wpf.Ui.Controls.MessageBox();

				mbx.Show("Ошибка авторизации", "Нет подключения к интернету");
			}
			catch (MailKit.Security.AuthenticationException)
			{
				Authorization.IsEnabled = true;

				Wpf.Ui.Controls.MessageBox mbx = new Wpf.Ui.Controls.MessageBox();

				mbx.Show("Ошибка авторизации", "Указан неверный логин или пароль");
			}

		}
	}
}
