namespace STRService.Pages.Components
{
    public class TemplateDescription
    {
        public static string Render(string description)
        {
            return String.Join(
                System.Environment.NewLine,
                description
                    .Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                    .Select(l => $@"<p style=""display:block"">{l}</p>")
            );
            
        }
        public static string RenderSmall(string description)
        {
            return String.Join(
                System.Environment.NewLine,
                description
                    .Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                    .Select(l => $@"<small style=""display:block"">{l}</small>")
            );

        }
    }
}
