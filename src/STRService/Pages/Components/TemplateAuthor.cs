namespace STRService.Pages.Components
{
    public class TemplateAuthor
    {
        public static string RenderLink(string authorName) => $@"<u>{authorName}</u>";

        public static string RenderAllLinksInline(string[]? authorNames)
        {
            if (authorNames == null)
            {
                return "";
            }
            else
            {
                return String.Join("; ", authorNames.Select(t => RenderLink(t)));
            }
        }
    }
}
