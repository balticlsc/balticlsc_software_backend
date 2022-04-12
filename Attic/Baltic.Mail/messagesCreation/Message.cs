using System.Collections.Generic;
using MimeKit;

namespace Baltic.Mail
{
    public class Message
    {
        public string AddresseeName { get; set; }
        public string AddresseeEmail { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public List<Paragraph> Paragraphs { get; }
        public bool WithText { get; set; }

        public List<string> AttachmentsPaths { get; set; }

        private Message()
        {
            Paragraphs = new List<Paragraph>();
            AttachmentsPaths = new List<string>();
            WithText = false;
        }

        public static INeedAddresseeName Create()
        {
            var message = new Message();
            return new MessageBuilder(message);
        }

        public MimeMessage BuildMimeMessage()
        {
            return MimeMessageCodeCreator.Create(this);
        }
    }

    #region FluentMessageBuilder

    public class MessageBuilder : INeedAddresseeName, INeedAddresseeEmail, INeedSubject, INeedContent,
        INeedParagraphs, INeedText, INeedAttachments
    {
        private readonly Message _beingConstructed;

        public MessageBuilder(Message beingConstructed)
        {
            _beingConstructed = beingConstructed;
        }

        public INeedAddresseeEmail AddresseeName(string name)
        {
            _beingConstructed.AddresseeName = name;
            return this;
        }

        public INeedSubject AddresseeEmail(string email)
        {
            _beingConstructed.AddresseeEmail = email;
            return this;
        }

        public INeedContent Subject(string subject)
        {
            _beingConstructed.Subject = subject;
            return this;
        }

        public INeedText Text(string content)
        {
            _beingConstructed.WithText = true;
            _beingConstructed.Text = content;
            return this;
        }

        INeedParagraphs INeedContent.AddParagraph(Paragraph paragraph)
        {
            _beingConstructed.Paragraphs.Add(paragraph);
            return this;
        }

        public INeedParagraphs AddParagraphs(Paragraph[] paragraphs)
        {
            foreach (var paragraph in paragraphs)
            {
                _beingConstructed.Paragraphs.Add(paragraph);
            }

            return this;
        }

        INeedAttachments INeedParagraphs.AddAttachment(string attachmentPath)
        {
            _beingConstructed.AttachmentsPaths.Add(attachmentPath);
            return this;
        }

        INeedAttachments INeedAttachments.AddAttachments(string[] attachmentsPaths)
        {
            foreach (var attachmentsPath in attachmentsPaths)
            {
                _beingConstructed.AttachmentsPaths.Add(attachmentsPath);
            }

            return this;
        }

        INeedAttachments INeedAttachments.AddAttachment(string attachmentPath)
        {
            _beingConstructed.AttachmentsPaths.Add(attachmentPath);
            return this;
        }

        INeedAttachments INeedText.AddAttachments(string[] attachmentsPaths)
        {
            foreach (var attachmentsPath in attachmentsPaths)
            {
                _beingConstructed.AttachmentsPaths.Add(attachmentsPath);
            }

            return this;
        }

        INeedAttachments INeedText.AddAttachment(string attachmentPath)
        {
            _beingConstructed.AttachmentsPaths.Add(attachmentPath);
            return this;
        }

        INeedAttachments INeedParagraphs.AddAttachments(string[] attachmentsPaths)
        {
            foreach (var attachmentsPath in attachmentsPaths)
            {
                _beingConstructed.AttachmentsPaths.Add(attachmentsPath);
            }

            return this;
        }

        INeedParagraphs INeedParagraphs.AddParagraph(Paragraph paragraph)
        {
            _beingConstructed.Paragraphs.Add(paragraph);
            return this;
        }

        public MimeMessage Build()
        {
            return _beingConstructed.BuildMimeMessage();
        }
    }

    #endregion
}