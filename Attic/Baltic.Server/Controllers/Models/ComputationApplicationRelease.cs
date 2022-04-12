using System;

#pragma warning disable 1591
namespace Baltic.Server.Controllers.Models
{
    [Obsolete]
    public class ComputationApplicationRelease
    {
        public string Id { get; set; }
        public string ComputationApplicationId { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }     // mówi czy daną wersję wolno użyć
        public string AuthorId { get; set; } // identyfikator użytkownika który stworzył aplikację        
        public string AssetId { get; set; } // identyfikator zasobu związanego z aplikacją murmur
    }
}
