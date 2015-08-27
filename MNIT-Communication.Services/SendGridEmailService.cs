using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using MNIT_Communication.Domain;
using SendGrid;

namespace MNIT_Communication.Services
{
	public class SendGridEmailService : ISendEmail
	{
	    private readonly IAuditService auditService;

	    public SendGridEmailService(IAuditService auditService)
	    {
	        this.auditService = auditService;
	    }

	    public async Task Send(string from, List<string> to, string subject, string body)
		{
			// Create the email object first, then add the properties.
			var myMessage = new SendGridMessage();

			// Add the message properties.
			myMessage.From = new MailAddress(from);

			// Add multiple addresses to the To field.
			myMessage.AddTo(to);

			myMessage.Subject = subject;

			//Add the HTML and Text bodies
			myMessage.Text = body;

			// Create credentials, specifying your user name and password.
			var credentials = new NetworkCredential("azure_853e23752ff2b9ce7c30020b435ea889@azure.com",
				CloudConfigurationManager.GetSetting("SendGridPassword"));

			// Create an Web transport for sending email.
			var transportWeb = new Web(credentials);

			// Send the email.
			await transportWeb.DeliverAsync(myMessage);

            await auditService.LogAuditEventAsync(new AuditEvent
            {
                AuditType = AuditType.EmailMessageSent,
                Details = "An Email message has been sent to: " + to,
                Data = myMessage
            });
        }
	}
}
