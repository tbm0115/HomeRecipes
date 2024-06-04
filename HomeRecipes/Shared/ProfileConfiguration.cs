using System.Collections.Generic;

namespace HomeRecipes.Shared
{
    /// <summary>
    /// Represents the user profile configuration
    /// </summary>
    public class ProfileConfiguration
    {
        /// <summary>
        /// Store of the user email.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Collection of recipe collections for the user.
        /// </summary>
        public List<CollectionConfig> Collections { get; set; } = new List<CollectionConfig>();
    }
}
