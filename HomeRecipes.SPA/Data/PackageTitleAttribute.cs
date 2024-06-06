namespace HomeRecipes
{
    [System.AttributeUsage(System.AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    sealed class PackageTitleAttribute : System.Attribute
    {
        public string Title { get; }
        public PackageTitleAttribute(string title)
        {
            this.Title = title;
        }
    }
}
