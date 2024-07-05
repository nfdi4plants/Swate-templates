using STRService.Models;

namespace STRService.Pages.Components
{
    public class TemplateAvailableVersion
    {
        public static string Render(string packageName, string version) => $@"<a href=""/template/{packageName}/{version}"">{version}</a>";

        public static string RenderVersionTable(SwateTemplateMetadata[] templates)
        {

            var content = $@"<table>
  <thead>
    <tr>
      <th scope=""col"">Version</th>
      <th scope=""col"">Released on</th>
   </tr>
  </thead>
  <tbody>
    {string.Join(
        System.Environment.NewLine, 
        templates
            .OrderByDescending(p => p.MajorVersion)
            .ThenByDescending(p => p.MinorVersion)
            .ThenByDescending(p => p.PatchVersion)
            .Select(p => $@"    <tr>
      <td>{Render(p.Name, p.GetSemanticVersionString())}</td>
      <td>{p.ReleaseDate}</td>
    </tr>"))}
  </tbody>
</table>";
            return content;

        }
    }
}
