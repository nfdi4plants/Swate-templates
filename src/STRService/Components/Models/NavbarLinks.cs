namespace STRService.Components.Models
{
    public class NavbarLinks
    {
        public string Name { get; set; }
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static List<NavbarLinks> DefaultLinks
        {
            get
            {
                return new List<NavbarLinks>
                {
                    new NavbarLinks {Name = "Home", Url = "" },
                    new NavbarLinks {Name = "Browse Templates", Url = "/templates" },
                    new NavbarLinks {Name = "About", Url = "/about" },
                    new NavbarLinks {Name = "Submit a Template", Url = "https://nfdi4plants.github.io/nfdi4plants.knowledgebase/swate/swate-template-contribution/"} //"https://github.com/nfdi4plants/arc-validate-template-registry?tab=readme-ov-file#validation-template-staging-area"
                };
            }
        }
    }
}
