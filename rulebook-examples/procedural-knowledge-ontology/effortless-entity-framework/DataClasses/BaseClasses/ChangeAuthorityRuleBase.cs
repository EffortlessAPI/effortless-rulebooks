
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
    [Table("ChangeAuthorityRules")]
    public class ChangeAuthorityRuleBase : SoAEntityBase
    {
        [Key]
        public string ChangeAuthorityRuleId { get; set; }

        // Formula Name (rulebook: ={{GovernedModel}} & " " & {{ChangeLayer}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.GovernedModel)), F.S(" "), F.Text(F.Of(this.ChangeLayer))))); set { }
        }

        public string? ChangeLayer { get; set; }
        public string? RequiredRoute { get; set; }
        public string? WhenPermitted { get; set; }
        public string? BreakageResponse { get; set; }
        // Formula MisroutedRequestCount (rulebook: =COUNTIFS(ModelChangeRequests!{{RuleKey}}, {{ChangeAuthorityRuleId}}, ModelChangeRequests!{{IsMisrouted}}, TRUE))
        [NotMapped]
        public int? MisroutedRequestCount
        {
            get => F.AsInt(F.Memo(this, "MisroutedRequestCount", () => F.Integer((base.SoAContext == null ? F.Null : F.CountIfs(F.Rows<ModelChangeRequest>(base.SoAContext, "ModelChangeRequests", __c => __c.ModelChangeRequests), __r => F.CritField(F.Of(__r.RuleKey), F.Of(this.ChangeAuthorityRuleId)) && F.CritLiteral(F.Of(__r.IsMisrouted), F.B(true))))))); set { }
        }

        // Formula IsRuleBypassed (rulebook: ={{MisroutedRequestCount}} > 0)
        [NotMapped]
        public bool? IsRuleBypassed
        {
            get => F.AsBool(F.Memo(this, "IsRuleBypassed", () => F.Cmp(F.Of(this.MisroutedRequestCount), ">", F.I(0)))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? GovernedModel { get; set; }
        public string? PermittedRole { get; set; }
        public string? ApprovalRole { get; set; }

        private GovernedModel _governedModelRef;

        [ForeignKey("GovernedModel")]
        public virtual GovernedModel GovernedModelRef
        {
            get
            {
                if (_governedModelRef == null && !string.IsNullOrEmpty(GovernedModel))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access GovernedModelRef - no database context is set. GovernedModel: " + GovernedModel + ".");
                        }
                        return null;
                    }
                    _governedModelRef = base.SoAContext.GovernedModels.Find(GovernedModel);
                    if (_governedModelRef != null)
                    {
                        base.SoAContext.Attach(_governedModelRef);
                    }
                }
                return _governedModelRef;
            }
            set
            {
                if (_governedModelRef != value)
                {
                    _governedModelRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_governedModelRef != null)
                    {
                        GovernedModel = _governedModelRef.GovernedModelId;
                    }
                }
            }
        }

        private Role _role;

        [ForeignKey("PermittedRole")]
        public virtual Role Role
        {
            get
            {
                if (_role == null && !string.IsNullOrEmpty(PermittedRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access Role - no database context is set. PermittedRole: " + PermittedRole + ".");
                        }
                        return null;
                    }
                    _role = base.SoAContext.Roles.Find(PermittedRole);
                    if (_role != null)
                    {
                        base.SoAContext.Attach(_role);
                    }
                }
                return _role;
            }
            set
            {
                if (_role != value)
                {
                    _role = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_role != null)
                    {
                        PermittedRole = _role.RoleId;
                    }
                }
            }
        }

        private Role _roleRef;

        [ForeignKey("ApprovalRole")]
        public virtual Role RoleRef
        {
            get
            {
                if (_roleRef == null && !string.IsNullOrEmpty(ApprovalRole))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleRef - no database context is set. ApprovalRole: " + ApprovalRole + ".");
                        }
                        return null;
                    }
                    _roleRef = base.SoAContext.Roles.Find(ApprovalRole);
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
                        ApprovalRole = _roleRef.RoleId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.GovernedModelRef;
            _ = this.Role;
            _ = this.RoleRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
