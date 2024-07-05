namespace STRService.Pages.Components
{
    public static class Layout
    {
        public static string Render(string activeNavbarItem, string title, string content)
        {
            return $@"<!DOCTYPE html>
<html>
  <head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <meta name=""color-scheme"" content=""light dark"" />
    <link rel=""stylesheet"" href=""/css/pico.cyan.min.css"" />
    <link rel=""stylesheet"" href=""/css/highlightjs.atom-one-dark.min.css"" />
    <script src=""/js/highlight.min.js""></script>
    <script>hljs.highlightAll();</script>
    <title>{title}</title>
  </head>
  <body>
    <header class=""container"">
      <section>
        {Navbar.Render(active: activeNavbarItem)}
      </section>
    </header>
    <main class=""container"">
      <section>
        {content}
      </section>
    </main>
      <section>
      {Footer.Render(active: activeNavbarItem)}
      </section>
  </body>
</html>";
        }
    }
}
