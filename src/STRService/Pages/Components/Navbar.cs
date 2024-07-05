namespace STRService.Pages.Components
{
    public static class Navbar
    {
        public static string RenderNavbarItem(string active, string item, string link)
        {
            if (active == item)
            {
                return $@"<li><strong><a aria-current=""page"" href=""{link}""><u>{item}</u></a></strong></li>";
            }
            else
            {
                return $@"<li><a href=""{link}"">{item}</a></li>";
            }
        }
        public static string Render(string active)
        {
    // this should eventually point to knowledge base articles
            return $@"<nav>
  <ul>
    <li><strong>STR - a service by <a href=""https://nfdi4plants.org/"">DataPLANT</a></strong></li>
  </ul>
  <ul>
    {RenderNavbarItem(active, "Home", "/")}
    {RenderNavbarItem(active, "Browse Packages", "/templates")}
    {RenderNavbarItem(active, "About", "/about")}
    <li><a href=""https://github.com/nfdi4plants/arc-validate-template-registry?tab=readme-ov-file#validation-template-staging-area"">Submit a template</a></li>
  </ul>
</nav>";
        }
    }
}
