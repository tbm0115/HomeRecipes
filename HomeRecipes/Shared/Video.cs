﻿using Schema.NET;
using System;

namespace HomeRecipes.Shared
{
    //public class Video : Schema.NET.IVideoObject
    //{
    //    public OneOrMany<IPerson> Actor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IMediaObject, string> Caption { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IPerson> Director { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IMusicGroup, IPerson> MusicBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IImageObject> Thumbnail { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> Transcript { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> VideoFrameSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> VideoQuality { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<INewsArticle> AssociatedArticle { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> Bitrate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> ContentSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<Uri> ContentUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<TimeSpan?> Duration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<Uri> EmbedUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<ICreativeWork> EncodesCreativeWork { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<DateTimeOffset?, TimeSpan?> EndTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<string, IQuantitativeValue, int> Height { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> PlayerType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IOrganization> ProductionCompany { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IPlace> RegionsAllowed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<bool?, IMediaSubscription> RequiresSubscription { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<DateTimeOffset?, TimeSpan?> StartTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<int?, DateTime?> UploadDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<string, IQuantitativeValue, int> Width { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IThing> About { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> Abstract { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> AccessibilityAPI { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> AccessibilityControl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> AccessibilityFeature { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> AccessibilityHazard { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> AccessibilitySummary { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> AccessMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IItemList> AccessModeSufficient { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IPerson> AccountablePerson { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<ICreativeWork, Uri> AcquireLicensePage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IAggregateRating> AggregateRating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> AlternativeHeadline { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> Assesses { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IMediaObject> AssociatedMedia { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IAudience> Audience { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IAudioObject, IClip, IMusicRecording> Audio { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IOrganization, IPerson> Author { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> Award { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IPerson> Character { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<ICreativeWork, string> Citation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IComment> Comment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<int?> CommentCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> ConditionsOfAccess { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IPlace> ContentLocation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IRating, string> ContentRating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<DateTimeOffset?> ContentReferenceTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IOrganization, IPerson> Contributor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IOrganization, IPerson> CopyrightHolder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<int?> CopyrightYear { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<string, Uri> Correction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> CreativeWorkStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IOrganization, IPerson> Creator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<int?, DateTime?, DateTimeOffset?> DateCreated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<int?, DateTime?, DateTimeOffset?> DateModified { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<int?, DateTime?, DateTimeOffset?> DatePublished { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<Uri> DiscussionUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<string, Uri> EditEIDR { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IPerson> Editor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IAlignmentObject> EducationalAlignment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<string, Uri> EducationalLevel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> EducationalUse { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IMediaObject> Encoding { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<string, Uri> EncodingFormat { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<ICreativeWork> ExampleOfWork { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<int?, DateTime?> Expires { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IOrganization, IPerson> Funder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<string, Uri> Genre { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<ICreativeWork> HasPart { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> Headline { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<ILanguage, string> InLanguage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IInteractionCounter> InteractionStatistic { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> InteractivityType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<bool?> IsAccessibleForFree { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<ICreativeWork, IProduct, Uri> IsBasedOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<bool?> IsFamilyFriendly { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<ICreativeWork, Uri> IsPartOf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> Keywords { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> LearningResourceType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<ICreativeWork, Uri> License { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IPlace> LocationCreated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IThing> MainEntity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IOrganization, IPerson> Maintainer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IProduct, string, Uri> Material { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IQuantitativeValue, string> MaterialExtent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IThing> Mentions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IDemand, IOffer> Offers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> Pattern { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<int?, string> Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IOrganization, IPerson> Producer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IOrganization, IPerson> Provider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IPublicationEvent> Publication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IOrganization, IPerson> Publisher { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IOrganization> PublisherImprint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<ICreativeWork, Uri> PublishingPrinciples { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IEvent> RecordedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IPublicationEvent> ReleasedEvent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IReview> Review { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<string, Uri> SchemaVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<int?, DateTime?> SdDatePublished { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<ICreativeWork, Uri> SdLicense { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IOrganization, IPerson> SdPublisher { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IQuantitativeValue, string> Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IOrganization> SourceOrganization { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IPlace> Spatial { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IPlace> SpatialCoverage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IOrganization, IPerson> Sponsor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> Teaches { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<DateTimeOffset?, string> Temporal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<DateTimeOffset?, string, Uri> TemporalCoverage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> Text { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<Uri> ThumbnailUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<TimeSpan?> TimeRequired { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<ICreativeWork> TranslationOfWork { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IOrganization, IPerson> Translator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> TypicalAgeRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<ICreativeWork, Uri> UsageInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<double?, string> Version { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<ICreativeWork> WorkExample { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<ICreativeWork> WorkTranslation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<Uri> AdditionalType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> AlternateName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<string> DisambiguatingDescription { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IPropertyValue, string, Uri> Identifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<IImageObject, Uri> Image { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<ICreativeWork, Uri> MainEntityOfPage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<IAction> PotentialAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<Uri> SameAs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public Values<ICreativeWork, IEvent> SubjectOf { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    public OneOrMany<Uri> Url { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //    Values<IClip, IVideoObject> ICreativeWork.Video { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //}
}
