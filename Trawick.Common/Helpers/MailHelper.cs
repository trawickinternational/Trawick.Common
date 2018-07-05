using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Trawick.Common.Helpers
{
	public static class MailHelper
	{

		const string	SMTP_USERNAME = "info@trawickinternational.com";
		const string	SMTP_PASSWORD = "Damu9319";

		//const string HOST = "ses-smtp-user.20170925-094007";
		const string	SMTP_HOST = "smtp.office365.com";
		const int			SMTP_PORT = 587;

		const string	DEFAULT_RECIPIENT = "info@trawickinternational.com";


		private static SmtpClient GetSmtpClient(bool ssl = true)
		{
			SmtpClient client = new SmtpClient(SMTP_HOST, SMTP_PORT);
			client.Credentials = new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
			client.EnableSsl = ssl;
			return client;
		}


		public static bool SendEmail(string subject, string message, string from, string to = "", string cc = "")
		{
			SmtpClient client = GetSmtpClient();

			MailMessage mail = new MailMessage
			{
				From = new MailAddress(from),
				Subject = subject,
				IsBodyHtml = true,
				Body = message
			};

			if (!string.IsNullOrEmpty(to))
			{
				foreach (var email in to.Split(','))
				{
					mail.To.Add(new MailAddress(email));
				}
			}
			else
			{
				mail.To.Add(new MailAddress(DEFAULT_RECIPIENT));
			}

			if (!string.IsNullOrEmpty(cc))
			{
				foreach (var email in cc.Split(','))
				{
					mail.CC.Add(new MailAddress(email));
				}
			}

			try
			{
				client.Send(mail);
				return true;
			}
			catch (Exception ex)
			{
				string x = ex.Message;
				return false;
			}
		}


		public static bool SendEmail(string message, List<string> to, List<string> cc)
		{
			string subject = "Alumni Travel Protect Contact";
			string from = "info@trawickinternational.com";
			return SendEmail(subject, message, from, string.Join(",", to), string.Join(",", cc));
		}



		public static string ErrorMessage(string contact = "")
		{
			List<string> msgList = new List<string>
			{
				"We're very sorry, there was a network problem and your message was not sent."
			};

			string tryAgain = "Please try again";
			if (!string.IsNullOrEmpty(contact))
			{
				tryAgain += " or contact " + contact;
			}
			msgList.Add(tryAgain + '.');

			return string.Join(" ", msgList).Trim();
		}


	}
}