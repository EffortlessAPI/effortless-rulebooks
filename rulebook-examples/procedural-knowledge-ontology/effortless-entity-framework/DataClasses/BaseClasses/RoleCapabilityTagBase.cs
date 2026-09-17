
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SqlOnAir.DotNet.Lib.DataClasses;
using F = SqlOnAir.DotNet.Lib.DataClasses.Formulas.EfFormulaFns;

namespace SqlOnAir.DotNet.Lib.DataClasses.BaseClasses
{
    [Table("RoleCapabilityTags")]
    public class RoleCapabilityTagBase : SoAEntityBase
    {
        [Key]
        public string RoleCapabilityTagId { get; set; }

        // Formula Name (rulebook: ={{Role}} & " can " & {{CapabilityTerm}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.Role)), F.S(" can "), F.Text(F.Of(this.CapabilityTerm))))); set { }
        }

        // Formula CapabilitySchemeDimension (rulebook: =INDEX(VocabularyTerms!{{SchemeGovernedDimension}}, MATCH({{CapabilityTerm}}, VocabularyTerms!{{VocabularyTermId}}, 0)))
        [NotMapped]
        public string? CapabilitySchemeDimension
        {
            get => F.AsString(F.Memo(this, "CapabilitySchemeDimension", () => F.Lookup<VocabularyTerm>(this, "VocabularyTerms", "VocabularyTermId", __c => __c.VocabularyTerms, __r => F.Of(__r.VocabularyTermId), F.Of(this.CapabilityTerm), __r => F.Of(__r.SchemeGovernedDimension), () => F.Of(new VocabularyTerm().SchemeGovernedDimension)))); set { }
        }

        // Formula IsTagOutsideCapabilityScheme (rulebook: ={{CapabilitySchemeDimension}} <> "AgentCapability")
        [NotMapped]
        public bool? IsTagOutsideCapabilityScheme
        {
            get => F.AsBool(F.Memo(this, "IsTagOutsideCapabilityScheme", () => F.Ne(F.Of(this.CapabilitySchemeDimension), F.S("AgentCapability")))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? Role { get; set; }
        public string? CapabilityTerm { get; set; }

        private Role _roleRef;

        [ForeignKey("Role")]
        public virtual Role RoleRef
        {
            get
            {
                if (_roleRef == null && !string.IsNullOrEmpty(Role))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleRef - no database context is set. Role: " + Role + ".");
                        }
                        return null;
                    }
                    _roleRef = base.SoAContext.Roles.Find(Role);
                    if (_roleRef != null)
                    {
                        base.SoAContext.Attach(_roleRef);
                    }
                }
                return _roleRef;
            }
            set
            {
                if (_roleRef != value)
                {
                    _roleRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleRef != null)
                    {
                        Role = _roleRef.RoleId;
                    }
                }
            }
        }

        private VocabularyTerm _vocabularyTerm;

        [ForeignKey("CapabilityTerm")]
        public virtual VocabularyTerm VocabularyTerm
        {
            get
            {
                if (_vocabularyTerm == null && !string.IsNullOrEmpty(CapabilityTerm))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access VocabularyTerm - no database context is set. CapabilityTerm: " + CapabilityTerm + ".");
                        }
                        return null;
                    }
                    _vocabularyTerm = base.SoAContext.VocabularyTerms.Find(CapabilityTerm);
                    if (_vocabularyTerm != null)
                    {
                        base.SoAContext.Attach(_vocabularyTerm);
                    }
                }
                return _vocabularyTerm;
            }
            set
            {
                if (_vocabularyTerm != value)
                {
                    _vocabularyTerm = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_vocabularyTerm != null)
                    {
                        CapabilityTerm = _vocabularyTerm.VocabularyTermId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.RoleRef;
            _ = this.VocabularyTerm;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
