namespace STRService.Pages.Components
{
    public class TemplateTag
    {
        public static string RenderLink(string tagName) => $@"<a href=""/templates?tag={tagName}"">{tagName}</a>";
        
        public static string RenderAllLinksInline(string[]? tagNames)
        {             
            if (tagNames == null)
            {
                return "";
            }
            else
            {
                return String.Join("; ", tagNames.Select(t => RenderLink(t)));
            }
        }
    }
}
