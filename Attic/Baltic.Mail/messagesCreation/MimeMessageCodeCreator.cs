using System;
using System.IO;
using System.Text;
using MimeKit;
using MimeKit.Utils;
using System.Reflection;
using Serilog;

namespace Baltic.Mail
{
    internal static class MimeMessageCodeCreator
    {
        private const string MessageEnding = @"
                <br>
                <br>
                <br>
                <div style=""font-style: italic"">
                    <h4>
                        <center>
                                This message was generated automatically. Do not reply.
                        </center>
                    </h4>
                </div>
                <hr>
                <div style=""color: #0063a5"">
                    <h5>
                        <center>
                                Warsaw University of Technology, Poland
                        </center>
                    </h5>
                    <h5>
                        <center>
                                RISE Research Institutes of Sweden AB, Sweden
                        </center>
                    </h5>
                    <h5>
                        <center>
                                Institute of Mathematics and Computer Science, University of Latvia, Latvia
                        </center>
                    </h5>
                    <h5>
                        <center>
                                EurA AG, Germany
                        </center>
                    </h5>
                    <h5>
                        <center>
                                Municipality of Vejle, Denmark
                        </center>
                    </h5>
                    <h5>
                        <center>
                                Lithuanian Innovation Center, Lithuania
                        </center>
                    </h5>
                    <h5>
                        <center>
                                Machine Technology Center Turku Ltd., Finland
                        </center>
                    </h5>
                    <h5>
                        <center>
                                Tartu Science Park Foundation, Estonia
                        </center>
                    </h5>
                </div>
                <br>
                <h4>
                    <center>
                            If you want to find out more visit our website: <a href=""https://www.balticlsc.eu"">www.balticlsc.eu</a>
                    </center>
                </h4>
                </div>";

        public static MimeMessage Create(Message message)
        {
            var bodyBuilder = new BodyBuilder();

            AddLogo(bodyBuilder);
            AddTextOrParagraphs(bodyBuilder, message);
            AddAttachments(bodyBuilder, message);

            return CreateMimeMessageWithProperties(bodyBuilder, message);
        }

        private static void AddLogo(BodyBuilder bodyBuilder)
        {
            var imageStream = Assembly.GetCallingAssembly()?
                .GetManifestResourceStream("Baltic.Mail.resources.logo.png");

            var image = bodyBuilder.LinkedResources.Add("logo.png", imageStream);
            image.ContentId = MimeUtils.GenerateMessageId();

            var messageBeginning = $@"
                <div style=""max-width:800px"">
                <img src=""cid:{image.ContentId}"">";

            bodyBuilder.HtmlBody += messageBeginning;
        }

        private static void AddTextOrParagraphs(BodyBuilder bodyBuilder, Message message)
        {
            var contentBuilder = new StringBuilder();
            if (message.WithText)
            {
                contentBuilder.Append(message.Text);
                contentBuilder.Append(MessageEnding);
            }
            else
            {
                foreach (var paragraph in message.Paragraphs)
                {
                    contentBuilder.Append(paragraph.GetParagraphContent());
                }

                contentBuilder.Append(MessageEnding);
            }

            bodyBuilder.HtmlBody += contentBuilder.ToString();
        }

        private static void AddAttachments(BodyBuilder bodyBuilder, Message message)
        {
            if (0 != message.AttachmentsPaths.Count)
            {
                foreach (var attachmentPath in message.AttachmentsPaths)
                {
                    try
                    {
                        bodyBuilder.Attachments.Add(attachmentPath);
                    }
                    catch (FileNotFoundException)
                    {
                        Log.Error("Attachment {attachmentPath} not found.", attachmentPath);
                    }
                }
            }
        }

        private static MimeMessage CreateMimeMessageWithProperties(BodyBuilder bodyBuilder, Message message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.To.Add(new MailboxAddress(message.AddresseeName, message.AddresseeEmail));
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            return mimeMessage;
        }
    }
}