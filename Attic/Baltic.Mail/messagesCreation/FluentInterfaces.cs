using MimeKit;

namespace Baltic.Mail
{
    public interface INeedAddresseeName
    {
        INeedAddresseeEmail AddresseeName(string name);
    }

    public interface INeedAddresseeEmail
    {
        INeedSubject AddresseeEmail(string name);
    }

    public interface INeedSubject
    {
        INeedContent Subject(string subject);
    }

    public interface INeedContent
    {
        INeedText Text(string content);
        INeedParagraphs AddParagraph(Paragraph paragraph);
        INeedParagraphs AddParagraphs(Paragraph[] paragraphs);
    }

    public interface INeedParagraphs
    {
        INeedParagraphs AddParagraph(Paragraph paragraph);
        INeedParagraphs AddParagraphs(Paragraph[] paragraphs);
        INeedAttachments AddAttachment(string attachmentPath);
        INeedAttachments AddAttachments(string[] attachmentsPaths);
        MimeMessage Build();
    }

    public interface INeedAttachments
    {
        INeedAttachments AddAttachment(string attachmentPath);
        INeedAttachments AddAttachments(string[] attachmentsPaths);
        MimeMessage Build();
    }

    public interface INeedText
    {
        INeedAttachments AddAttachment(string attachmentPath);
        INeedAttachments AddAttachments(string[] attachmentsPaths);
        MimeMessage Build();
    }
}