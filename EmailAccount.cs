using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ActiveUp.Net.Mail;

namespace FMLeadRouter
{
    public class EmailAccount
    {
        private Imap4Client _client = null;

        public EmailAccount(EmailCredentials emailCredentials) : 
            this(emailCredentials.MailServer, emailCredentials.Port, emailCredentials.Ssl, emailCredentials.Login, emailCredentials.Password)
        {
            
        }

        public EmailAccount(string mailServer, int port, bool ssl, string login, string password)
        {
            if (ssl)
            {
                Client.ConnectSsl(mailServer, port);
            }
            else
            {
                Client.Connect(mailServer, port);
                Client.Login(login, password);
            }
        }

        protected Imap4Client Client
        {
            get { return _client ?? (_client = new Imap4Client()); }
        }

        #region properties

        public string MailServer { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool Ssl { get; set; }

        #endregion properties

        #region helpers

        public IEnumerable<Message> GetAllMails(string mailBox)
        {
            return GetMails(mailBox, "ALL").Cast<Message>();
        }

        public IEnumerable<Message> GetUnreadMails(string mailBox)
        {
            return GetMails(mailBox, "UNSEEN").Cast<Message>();            
        }

        public void StartIdleImap(string mailBox)
        {
            if (Client.IsConnected)
            {
                //Client.StopIdle();
                Console.WriteLine("Client is connected...");
            }
            else
            {
                Console.WriteLine("Client is NOT connected");
            }
            bool stop = false;
            Console.WriteLine("Checking Mail...");
            try
            {
                using (Client)
                {
                    Client.NewMessageReceived += client_NewMessageReceived;
                    while (!stop)
                    {
                        Console.WriteLine("Idle, waiting...");

                        Mailbox inbox = Client.SelectMailbox(mailBox);
                        inbox.Subscribe();
                        Client.StartIdle();

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retreiving email:{0}",ex);
            }
        }

        private void client_NewMessageReceived(object sender, NewMessageReceivedEventArgs e)
        {
            Client.StopIdle();
        }

        public void StopIdleImap()
        {
            Client.StopIdle();
        }

        private MessageCollection GetMails(string mailBox, string searchPhrase)
        {
            Mailbox mails = Client.SelectMailbox(mailBox);
            MessageCollection messages = mails.SearchParse(searchPhrase);
            return messages;
        }

        #endregion helpers
    }

   
}
