namespace STRService.Pages.Components
{
    public class Index
    {
        public static string Render()
        {
            return @"<section>
<h1><strong>STR:</strong> ARC Swate template registry</h1>
<p><a href=""/templates"">Browse all available templates</a></p>
<p>For <b>programmatic access</b>, go checkout the <a href=""swagger"">API documentation</a>
<hr/>
</section>
<section>
<p>The STR indexes and hosts Swate templates for <strong>A</strong>nnotated <strong>R</strong>esearch <strong>C</strong>ontexts (ARCs) in a central registry. You can learn more about the concept of ARCs <a href=""https://nfdi4plants.org"">here</a></p>
<p>It is intended for 2 main use cases:
<ol>
<li>Summarize available templates and make them discoverable and inspectable for users that want to incorporate them in their <strong>C</strong>ontinuous <strong>Q</strong>uality <strong>C</strong>ontrol (CQC) workflows locally or on the <a href=""git.nfdi4plants.org"">DataHUB</a>. This is done by providing a <a href=""/about#browser"">template browser website</a></li>
<li>Provide programmatic access for downloading and veryfing Swate templates. This is done by providing <a href=""/about#api"">Public API Endpoints</a></li>
</ol>
</section>";
        }
    }
}
