using Schema.NET;
using System;

namespace HomeRecipes.Shared
{
    public class NutritionInformation : Schema.NET.INutritionInformation
    {
        public OneOrMany<string> Calories { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> CarbohydrateContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> CholesterolContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> FatContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> FiberContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> ProteinContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> SaturatedFatContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> ServingSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> SodiumContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> SugarContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> TransFatContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public OneOrMany<string> UnsaturatedFatContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
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
