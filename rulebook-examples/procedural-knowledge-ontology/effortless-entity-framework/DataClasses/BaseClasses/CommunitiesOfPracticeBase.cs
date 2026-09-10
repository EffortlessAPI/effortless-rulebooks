
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("CommunitiesOfPractice")]
    public class CommunitiesOfPracticeBase : SoAEntityBase
    {
        [Key]
        public string CommunityOfPracticeId { get; set; }

        // Formula Name (rulebook: ={{Label}})
        public string? Name
        {
            get => this.Label; set { }
        }

        public string? Label { get; set; }
        public string? Purpose { get; set; }
        public string? Cadence { get; set; }
        public string? SemanticTypeIri { get; set; }

        public string? Organization { get; set; }
        public string? StewardRole { get; set; }

        private Organization _organization;

        [ForeignKey("Organization")]
        public virtual Organization Organization
        {
            get
            {
                if (_organization == null && !string.IsNullOrEmpty(Organization))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Organization - no database context is set. Organization: " + Organization + ".");
                        }
                        return null;
                    }
                    _organization = Context.Organizations.Find(Organization);
                    if (_organization != null)
                    {
                        Context.Attach(_organization);
                    }
                }
                return _organization;
            }
            set
            {
                if (_organization != value)
                {
                    _organization = value;
                    Organization = _organization == null ? default : _organization.OrganizationId;
                }
            }
        }

        private Role _role;

        [ForeignKey("StewardRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(StewardRole))
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. StewardRole: " + StewardRole + ".");
                        }
                        return null;
                    }
                    _role = Context.Roles.Find(StewardRole);
                    if (_role != null)
                    {
                        Context.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    StewardRole = _role == null ? default : _role.RoleId;
                }
            }
        }

        private ObservableCollection<Mentorship> _mentorships;

        [InverseProperty("CommunitiesOfPractice")]
        public virtual ObservableCollection<Mentorship> Mentorships
        {
            get
            {
                if (_mentorships == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Mentorships - no database context is set. CommunityOfPracticeId: " + this.CommunityOfPracticeId + ".");
                        }
                        _mentorships = new ObservableCollection<Mentorship>();
                    }
                    else
                    {
                        var items = Context.Mentorships.Where(x => x.CommunityOfPractice == this.CommunityOfPracticeId).ToList<Mentorship>();
                        _mentorships = new ObservableCollection<Mentorship>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _mentorships.CollectionChanged += Mentorships_CollectionChanged;
                }
                return _mentorships;
            }
            private set
            {
                if (_mentorships != null)
                {
                    _mentorships.CollectionChanged -= Mentorships_CollectionChanged;
                }
                _mentorships = value;
                if (_mentorships != null)
                {
                    _mentorships.CollectionChanged += Mentorships_CollectionChanged;
                }
            }
        }

        private void Mentorships_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<Mentorship>())
                {
                    item.CommunityOfPractice = this.CommunityOfPracticeId;
                }
            }
        }

        private ObservableCollection<LearningActivity> _learningActivities;

        [InverseProperty("CommunitiesOfPractice")]
        public virtual ObservableCollection<LearningActivity> LearningActivities
        {
            get
            {
                if (_learningActivities == null)
                {
                    if (Context == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access LearningActivities - no database context is set. CommunityOfPracticeId: " + this.CommunityOfPracticeId + ".");
                        }
                        _learningActivities = new ObservableCollection<LearningActivity>();
                    }
                    else
                    {
                        var items = Context.LearningActivities.Where(x => x.CommunityOfPractice == this.CommunityOfPracticeId).ToList<LearningActivity>();
                        _learningActivities = new ObservableCollection<LearningActivity>(items);
                        if (items.Any())
                        {
                            Context.AttachRange(items);
                        }
                    }
                    _learningActivities.CollectionChanged += LearningActivities_CollectionChanged;
                }
                return _learningActivities;
            }
            private set
            {
                if (_learningActivities != null)
                {
                    _learningActivities.CollectionChanged -= LearningActivities_CollectionChanged;
                }
                _learningActivities = value;
                if (_learningActivities != null)
                {
                    _learningActivities.CollectionChanged += LearningActivities_CollectionChanged;
                }
            }
        }

        private void LearningActivities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e?.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<LearningActivity>())
                {
                    item.CommunityOfPractice = this.CommunityOfPracticeId;
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.Organization;
            _ = this.Role;
            _ = this.Mentorships;
            _ = this.LearningActivities;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
