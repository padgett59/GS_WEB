using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace GS_WEB.Models
{
    public static class Notify
    {
        private static SmtpClient mailServer = new SmtpClient("mail.sevensciencequestions.com", 8889);
        //MUST be same as From address
        private static NetworkCredential ordersCredentials = new NetworkCredential("orders@sevensciencequestions.com", "T9mail!");
        private static NetworkCredential postmasterCredentials = new NetworkCredential("postmaster@sevensciencequestions.com", "T9mail!");

        public static void OrderReceived(MVC.GSWeb.Models.ProofOrder proofOrder)
        {
            String fromOrders = ordersCredentials.UserName;
            String subjectOrder = "Seven Science Questions New Online Order";
            String orderProcessor = MVC.GSWeb.Constants.AUTHOR_EMAIL;

            StringBuilder buildString = new StringBuilder();
            String bodyOrderInvoiceRequired = "A new Online order has been submitted. \nPlease send an invoice to: ";
            buildString.Append(String.Format("\n{0} {1}", bodyOrderInvoiceRequired, proofOrder.Email));
            buildString.Append(String.Format("\n{0} {1}", proofOrder.FirstName, proofOrder.LastName));
            buildString.Append(String.Format("\n{0}", proofOrder.Address1));
            buildString.Append(String.Format("\n{0}", proofOrder.Address2));
            buildString.Append(String.Format("\n{0}, {1}   {2}", proofOrder.City, proofOrder.State, proofOrder.Zip));
            buildString.Append(String.Format("\n\nIn the amount of ${0}", proofOrder.Total.ToString()));
            buildString.Append(String.Format("\n\nReceived: {0}", proofOrder.DateOrdered));

            SendEmail(fromOrders, new List<String>() { orderProcessor }, subjectOrder, buildString.ToString(), ordersCredentials);
        }

        public static void WelcomeSubscriber(MVC.GSWeb.Models.NewReader newReader)
        {
            String fromAddress = postmasterCredentials.UserName;
            String subject = "Welcome Seven Science Questions Newletter Subscriber";
            String toAddress = newReader.Email;

            StringBuilder buildString = new StringBuilder();
            buildString.Append(String.Format("Thank you {0}, for subscribing to the Seven Science Questions Newletter", newReader.ReaderName));
            buildString.Append(String.Format("\nYour first newsletter will be arriving shortly."));
            buildString.Append(String.Format("\n"));
            buildString.Append(String.Format("\nYou can also read regular blog entries at {0} to learn even more.", MVC.GSWeb.Constants.FACEBOOK));
            SendEmail(fromAddress, new List<String>() { toAddress }, subject, buildString.ToString(), postmasterCredentials);
        }

        public static void CheckReview(MVC.GSWeb.Models.ReaderReview newReview)
        {
            String fromAddress = postmasterCredentials.UserName;
            String subject = "A new review has been submitted.";
            String toAddress = MVC.GSWeb.Constants.AUTHOR_EMAIL;
            StringBuilder buildString = new StringBuilder();
            buildString.Append(String.Format("A new review of Seven Science Questions has been submitted from  {0}.", newReview.Email));
            buildString.Append(String.Format("\nThis is a reminder to check and approve it."));
            SendEmail(fromAddress, new List<String>() { toAddress }, subject, buildString.ToString(), postmasterCredentials);
        }


        private static void SendEmail(string from, List<string> to, string subject, string body, NetworkCredential credentials)
        {
            MailMessage outgoing = new MailMessage();
            outgoing.From = new MailAddress(from);
            to.ForEach(t => outgoing.To.Add(t));
            outgoing.Subject = subject;
            outgoing.Body = body;
            mailServer.Credentials = credentials;
            mailServer.Send(outgoing);
        }
    }
}