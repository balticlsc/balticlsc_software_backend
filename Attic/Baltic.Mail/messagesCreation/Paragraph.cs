using System.Text;

namespace Baltic.Mail
{
    public class Paragraph
    {
        private string Text { get; set; }
        private bool Bold { get; set; }
        private bool Italic { get; set; }
        private bool Underline { get; set; }

        public Paragraph()
        {
            Bold = false;
            Italic = false;
            Underline = false;
            Text = string.Empty;
        }

        public Paragraph(string text)
        {
            Bold = false;
            Italic = false;
            Underline = false;
            Text = text;
        }

        public Paragraph(string text, bool bold, bool italic, bool underline)
        {
            Text = text;
            Bold = bold;
            Italic = italic;
            Underline = underline;
        }

        public string GetParagraphContent()
        {
            var stringBuilder = new StringBuilder(Text);
            stringBuilder.Append("<br><br>");
            if (Bold)
            {
                stringBuilder.Insert(0, "<b>");
                stringBuilder.Append("</b>");
            }

            if (Italic)
            {
                stringBuilder.Insert(0, "<i>");
                stringBuilder.Append("</i>");
            }

            if (Underline)
            {
                stringBuilder.Insert(0, "<u>");
                stringBuilder.Append("</u>");
            }

            return stringBuilder.ToString();
        }
    }
}