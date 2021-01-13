using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Schema.NET;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HomeRecipes.Shared
{
    public class Recipe
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<RecipeIngredient> Ingredients { get; set; }

        public IEnumerable<RecipeInstruction> Instructions { get; set; }

        public string ThumbnailUrl { get; set; }
        
        public TimeSpan? CookTime { get; set; }

        public TimeSpan? PrepTime { get; set; }

        public string RecipeYield { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DatePublished { get; set; }

        public DateTime? DateModified { get; set; }


        [JsonExtensionData]
        private IDictionary<string, JToken> _additionalData;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            Name = (string)_additionalData["title"];
            string ingredientsRaw = (string)_additionalData["ingredients"];
            ingredientsRaw = ingredientsRaw.Substring(ingredientsRaw.IndexOf("<ul>\r\n"));
            ingredientsRaw = ingredientsRaw.Substring(0, ingredientsRaw.LastIndexOf("</ul>"));
            string[] ingredientListItems = ingredientsRaw.Split("</li>\r\n<li>", StringSplitOptions.RemoveEmptyEntries);
        }

        //public OneOrMany<string> CookingMethod { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<TimeSpan?> CookTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<INutritionInformation> Nutrition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> RecipeCategory { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> RecipeCuisine { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //public List<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
        //public OneOrMany<string> RecipeIngredient { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //public Values<ICreativeWork, IItemList, string> RecipeInstructions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IQuantitativeValue, string> RecipeYield { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<RestrictedDiet?> SuitableForDiet { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IMonetaryAmount, string> EstimatedCost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<TimeSpan?> PerformTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<TimeSpan?> PrepTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<ICreativeWork, IHowToSection, IHowToStep, string> Step { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IHowToSupply, string> Supply { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IHowToTool, string> Tool { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<TimeSpan?> TotalTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IQuantitativeValue, string> Yield { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IThing> About { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> Abstract { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> AccessibilityAPI { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> AccessibilityControl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> AccessibilityFeature { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> AccessibilityHazard { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> AccessibilitySummary { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> AccessMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IItemList> AccessModeSufficient { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IPerson> AccountablePerson { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<ICreativeWork, Uri> AcquireLicensePage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IAggregateRating> AggregateRating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> AlternativeHeadline { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> Assesses { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IMediaObject> AssociatedMedia { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IAudience> Audience { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IAudioObject, IClip, IMusicRecording> Audio { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IOrganization, IPerson> Author { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> Award { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IPerson> Character { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<ICreativeWork, string> Citation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IComment> Comment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<int?> CommentCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> ConditionsOfAccess { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IPlace> ContentLocation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IRating, string> ContentRating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<DateTimeOffset?> ContentReferenceTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IOrganization, IPerson> Contributor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IOrganization, IPerson> CopyrightHolder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<int?> CopyrightYear { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<string, Uri> Correction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> CreativeWorkStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IOrganization, IPerson> Creator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<int?, DateTime?, DateTimeOffset?> DateCreated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<int?, DateTime?, DateTimeOffset?> DateModified { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<int?, DateTime?, DateTimeOffset?> DatePublished { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<Uri> DiscussionUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<string, Uri> EditEIDR { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IPerson> Editor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IAlignmentObject> EducationalAlignment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<string, Uri> EducationalLevel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> EducationalUse { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IMediaObject> Encoding { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<string, Uri> EncodingFormat { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<ICreativeWork> ExampleOfWork { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<int?, DateTime?> Expires { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IOrganization, IPerson> Funder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<string, Uri> Genre { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<ICreativeWork> HasPart { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> Headline { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<ILanguage, string> InLanguage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IInteractionCounter> InteractionStatistic { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> InteractivityType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<bool?> IsAccessibleForFree { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<ICreativeWork, IProduct, Uri> IsBasedOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<bool?> IsFamilyFriendly { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<ICreativeWork, Uri> IsPartOf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> Keywords { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> LearningResourceType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<ICreativeWork, Uri> License { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IPlace> LocationCreated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IThing> MainEntity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IOrganization, IPerson> Maintainer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IProduct, string, Uri> Material { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IQuantitativeValue, string> MaterialExtent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IThing> Mentions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IDemand, IOffer> Offers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> Pattern { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<int?, string> Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IOrganization, IPerson> Producer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IOrganization, IPerson> Provider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IPublicationEvent> Publication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IOrganization, IPerson> Publisher { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IOrganization> PublisherImprint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<ICreativeWork, Uri> PublishingPrinciples { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IEvent> RecordedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IPublicationEvent> ReleasedEvent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IReview> Review { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<string, Uri> SchemaVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<int?, DateTime?> SdDatePublished { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<ICreativeWork, Uri> SdLicense { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IOrganization, IPerson> SdPublisher { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IQuantitativeValue, string> Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IOrganization> SourceOrganization { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IPlace> Spatial { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IPlace> SpatialCoverage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IOrganization, IPerson> Sponsor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> Teaches { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<DateTimeOffset?, string> Temporal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<DateTimeOffset?, string, Uri> TemporalCoverage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<Uri> ThumbnailUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<TimeSpan?> TimeRequired { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<ICreativeWork> TranslationOfWork { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IOrganization, IPerson> Translator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> TypicalAgeRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<ICreativeWork, Uri> UsageInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<double?, string> Version { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IClip, IVideoObject> Video { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<ICreativeWork> WorkExample { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<ICreativeWork> WorkTranslation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<Uri> AdditionalType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> AlternateName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<string> DisambiguatingDescription { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IPropertyValue, string, Uri> Identifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<IImageObject, Uri> Image { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<ICreativeWork, Uri> MainEntityOfPage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<IAction> PotentialAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<Uri> SameAs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public Values<ICreativeWork, IEvent> SubjectOf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public OneOrMany<Uri> Url { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
