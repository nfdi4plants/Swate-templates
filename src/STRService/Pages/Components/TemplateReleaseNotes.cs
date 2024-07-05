namespace STRService.Pages.Components
{
    public class TemplateReleaseNotes
    {
        public static string Render(string? releaseNotes)
        {
            if (releaseNotes == null)
            {
                return "<p>No release notes available for this version</p>";
            }
            else
            {
                return String.Join(
                    System.Environment.NewLine,
                    releaseNotes
                        .Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                        .Select(l => $@"<p style=""display:block"">{l}</p>")
                );
            }
        }
        public static string RenderSmall(string? releaseNotes)
        {
            if (releaseNotes == null)
            {
                return "<p>No release notes available for this version</p>";
            }
            else
            {
                return String.Join(
                    System.Environment.NewLine,
                    releaseNotes
                        .Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)
                        .Select(l => $@"<small style=""display:block"">{l}</small>")
                );

            }
        }
    }
}
