using STRService.Models;
using System.Text;
using static STRIndex.Domain;

namespace STRService.Pages.Components
{
    public class Template
    {
        public static string Render(
            string templateName,
            string templateVersion,
            string templateDescription,
            string templateReleaseNotes,
            string templateContent,
            DateOnly templateReleaseDate,
            string[] templateTags,
            Author[] templateAuthors,
            string versionTable,
            int downloads
        )
        {
            return $@"<section>
  <hgroup>
    <h1>Swate template <mark>{templateName}</mark></h1>
    <p>{TemplateTag.RenderAllLinksInline(templateTags)}</p>
    <p><strong>v{templateVersion}</strong> released on {templateReleaseDate}</p>
    <p>by {TemplateAuthor.RenderAllLinksInline(templateAuthors.Select(a => a.FullName).ToArray())}</p>
    <p>{downloads} Downloads</p>
  </hgroup>
</section>
<hr />
<section>
  <h4>Install with <a href=""https://github.com/nfdi4plants/arc-validate"">arc-validate</a></h4>
  <pre><code> arc-validate tamplate install {templateName} --version {templateVersion}</code></pre>
  <h4>Include in a <a href=""https://doi.org/10.1111/tpj.16474"">PLANTDataHUB CQC pipeline</a></h4>
  <pre><code class=""language-yaml"">validation_templates:
  - name: {templateName}
    version: {templateVersion}</code></pre>
</section>
<hr />
<section> 
  <h2>Description</h2>
  {TemplateDescription.Render(templateDescription)}
</section>
<hr />
<section>
  <details>
    <summary role=""button"" class=""primary"">Release notes</summary>
      {TemplateReleaseNotes.Render(templateReleaseNotes)}
  </details>
  <hr />
  <details>
    <summary role=""button"" class=""primary"">Browse code (v{templateVersion})</summary>
      <pre><code class=""language-fsharp"">{System.Security.SecurityElement.Escape(templateContent)}</code></pre>
  </details>
  <hr />
  <details>
    <summary role=""button"" class=""primary"">Available versions</summary>
    {versionTable}
  </details>
</section>
<hr />
";
        }
    }
}
