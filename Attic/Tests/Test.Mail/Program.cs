using System;
using Baltic.Mail;

namespace Test.Mail
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Visit our website: https://www.balticlsc.eu");

            var paragraphs = new Paragraph[2];
            paragraphs[0] = new Paragraph("Hey, hey, hello");
            paragraphs[1] = new Paragraph("Paragraph, para, para, paragraph...", false, true, false);

            var m = Message.Create()
                .AddresseeName("Patgres Bachamow")
                .AddresseeEmail("patgres.bachamow@gmail.com")
                .Subject("Builder test.")
                .AddParagraph(new Paragraph("I'm sending test email with attachments.", true, true, true))
                .AddParagraph(new Paragraph(
                    "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?"))
                .AddParagraphs(paragraphs)
                .AddAttachment("dadadada")
                .AddAttachment(@"C:\Users\rados\Desktop\LLRB.pdf")
                .Build();

            var client = new MailClient();
            client.SendMessage(m);
            client.Disconnect();

            Console.ReadKey();
        }
    }
}