using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using RisqueServer.Security;

namespace RisqueServer.Communication {
    public class Mailer {
        private User mailSender = null;
        private int securityValue = -1;
        private List<MailboxAddress> statusList;
        private MailboxAddress mainUpdate;
        private readonly string office365address = "smtp.office365.com";
        private const int officePort = 587;

        public Mailer(User mailSender, int securityValue) {
            this.mailSender = mailSender;
            this.securityValue = securityValue;
            this.statusList = new List<MailboxAddress>();
            this.statusList.Add(new MailboxAddress("Ryan Everett", "everettr@purdue.edu"));
            this.mainUpdate = new MailboxAddress("everettr@purdue.edu");
        }
        public void sendCompletion(int ticketId, string ticketLoc) {
            //http://www.mimekit.net/docs/html/CreatingMessages.htm
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("RisqueServer", mailSender.getEmail(securityValue)));
            foreach (MailboxAddress address in statusList) {
                message.To.Add(address);
            }
            message.Subject = String.Format("RISQUESERVER Completed Ticket: {0}", ticketId);
            message.Body = new TextPart("plain") {
                Text = String.Format("Completed Ticket: {0}", ticketId)
            };
            sendMail(message, false);
        }
        public void sendStartup() {
            MimeMessage startMess = new MimeMessage();
            startMess.From.Add(new MailboxAddress("RisqueServer", mailSender.getEmail(securityValue)));
            //only to send to mainUpdate
            startMess.To.Add(mainUpdate);
            startMess.Subject = "RISQUESERVER Server startup";
            startMess.Body = new TextPart("plain") {
                Text = "Server has successfully started up."
            };
            sendMail(startMess, false);
        }
        public void sendShutdown(string shutdownReason) {
            //Shutdown reason is mostly future proofing, but what the hell
            MimeMessage shutMess = new MimeMessage();
            shutMess.From.Add(new MailboxAddress("RisqueServer", mailSender.getEmail(securityValue)));
            //only to send to mainUpdate
            shutMess.To.Add(mainUpdate);
            shutMess.Subject = "RISQUESERVER Server shutdown";
            shutMess.Body = new TextPart("plain") {
                Text = "Server has shutdown, reason: " + '\n' + shutdownReason
            };
            sendMail(shutMess, true);
        }
        private void sendMail(MimeMessage message, bool wait) {
            System.Threading.Tasks.Task send = new Task(() => {
                Console.WriteLine("Sending email");
                using (var client = new SmtpClient()) {
                    Console.WriteLine("Sending message");
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(office365address, officePort, false);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(mailSender.getEmail(securityValue), mailSender.getPass(securityValue));
                    client.Send(message);
                    client.Disconnect(true);
                    Console.WriteLine("Sent Message");
                }
            });
            send.Start();
            if (wait) {
                send.Wait();
            }
        }
    }
}
