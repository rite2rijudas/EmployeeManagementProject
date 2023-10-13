using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace EMPLOYEE_MANAGEMENT.Models
{
    public class CustomeDataProvider
    {



        public string Encryption(string password)
        {
            byte[] encryptedbyte = new byte[password.Length];
            encryptedbyte = System.Text.Encoding.UTF8.GetBytes(password);
            string encrypteddata = Convert.ToBase64String(encryptedbyte);

            return encrypteddata;
        }

        public string Decryption(string encrpassword)
        {
            byte[] decryptedbyte = Convert.FromBase64String(encrpassword);
            string decrypteddata = System.Text.Encoding.UTF8.GetString(decryptedbyte);

            return decrypteddata;
        }


        #region OTP Send



        //bvkj zyoy dmve cjfx
        public bool SendEmail(string from, string to, string subject, string body)
        {
            // SMTP server details
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587; // Use the appropriate port for your SMTP server
            string smtpUsername = "rite2rainbowdas@gmail.com";
            string smtpPassword = "bvkjzyoydmvecjfx";

            // Sender and recipient addresses
            string senderEmail = (string.IsNullOrEmpty(to)) ? "rite2rijudas@gmail.com" : to;
            string recipientEmail = (string.IsNullOrEmpty(to)) ? "" : to;

            // Create a new MailMessage
            MailMessage message = new MailMessage(senderEmail, recipientEmail);
            message.Subject = subject;
            message.Body = body;

            // Create a SmtpClient
            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.EnableSsl = true; // Enable SSL if required by your SMTP server

            try
            {
                // Send the email
                smtpClient.Send(message);
                Console.WriteLine("Email sent successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to send email: " + ex.Message);

                return false;
            }
            finally
            {
                // Dispose of the SmtpClient and MailMessage to release resources
                message.Dispose();
                smtpClient.Dispose();
            }
        }
        #endregion
    }
}