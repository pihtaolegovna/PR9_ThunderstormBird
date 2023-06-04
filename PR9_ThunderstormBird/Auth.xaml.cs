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
					break;
				case 1:
					User.Address = MailTbx.Text + "@rambler.ru";
					break;
				case 2:
					User.Address = MailTbx.Text + "@yandex.ru";
					break;


			}
			User.imapport = 465;
			User.smtpport = 993;

			User.Password = PasswordTbx.Text;

			MainWindow secondWindow = new MainWindow();

			try
			{
				using (var client = new ImapClient())
				{
					await client.ConnectAsync("imap.gmail.com", User.smtpport, true);
					await client.AuthenticateAsync(User.Address, User.Password);
				}
				secondWindow.Show();
				this.Close();
			}
			catch
			{
				Authorization.IsEnabled = true;
				MessageBox.Show("Ошибка авторизации");
			}

		}
	}
}
