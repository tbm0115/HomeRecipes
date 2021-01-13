using Schema.NET;
using System;

namespace HomeRecipes.Shared
{
    public class AggregateRating : Schema.NET.IAggregateRating
    {
        public OneOrMany<IThing> ItemReviewed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<int?> RatingCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<int?> ReviewCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Values<IOrganization, IPerson> Author { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Values<double?, string> BestRating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> RatingExplanation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Values<double?, string> RatingValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> ReviewAspect { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Values<double?, string> WorstRating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<Uri> AdditionalType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> AlternateName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> DisambiguatingDescription { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Values<IPropertyValue, string, Uri> Identifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Values<IImageObject, Uri> Image { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Values<ICreativeWork, Uri> MainEntityOfPage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<IAction> PotentialAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<Uri> SameAs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Values<ICreativeWork, IEvent> SubjectOf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<Uri> Url { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
