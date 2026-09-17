
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
    [Table("RecordsRetentionPolicies")]
    public class RecordsRetentionPolicyBase : SoAEntityBase
    {
        [Key]
        public string RecordsRetentionPolicyId { get; set; }

        // Formula Name (rulebook: ={{RecordClass}} & " / " & {{RetentionDriver}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.RecordClass)), F.S(" / "), F.Text(F.Of(this.RetentionDriver))))); set { }
        }

        public string? RecordClass { get; set; }
        public string? RetentionDriver { get; set; }
        public int? RetentionYears { get; set; }
        public int? KnowledgeUsefulLifeYears { get; set; }
        // Formula ProgramSponsorDiscipline (rulebook: =INDEX(CorporateGovernancePrograms!{{SponsorDiscipline}}, MATCH({{GovernanceProgram}}, CorporateGovernancePrograms!{{CorporateGovernanceProgramId}}, 0)))
        [NotMapped]
        public string? ProgramSponsorDiscipline
        {
            get => F.AsString(F.Memo(this, "ProgramSponsorDiscipline", () => F.Lookup<CorporateGovernanceProgram>(this, "CorporateGovernancePrograms", "CorporateGovernanceProgramId", __c => __c.CorporateGovernancePrograms, __r => F.Of(__r.CorporateGovernanceProgramId), F.Of(this.GovernanceProgram), __r => F.Of(__r.SponsorDiscipline), () => F.Of(new CorporateGovernanceProgram().SponsorDiscipline)))); set { }
        }

        // Formula IsLegalLedKnowledgeDestruction (rulebook: =AND({{ProgramSponsorDiscipline}} = "Legal", {{RetentionDriver}} = "ComplianceRisk", {{RetentionYears}} < {{KnowledgeUsefulLifeYears}}))
        [NotMapped]
        public bool? IsLegalLedKnowledgeDestruction
        {
            get => F.AsBool(F.Memo(this, "IsLegalLedKnowledgeDestruction", () => F.And(F.Bool3(F.Eq(F.Of(this.ProgramSponsorDiscipline), F.S("Legal"))), F.Bool3(F.Eq(F.Nullif(F.Of(this.RetentionDriver)), F.S("ComplianceRisk"))), F.Bool3(F.Cmp(F.Nullif(F.Of(this.RetentionYears)), "<", F.Nullif(F.Of(this.KnowledgeUsefulLifeYears))))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? GovernanceProgram { get; set; }

        private CorporateGovernanceProgram _corporateGovernanceProgram;

        [ForeignKey("GovernanceProgram")]
        public virtual CorporateGovernanceProgram CorporateGovernanceProgram
        {
            get
            {
                if (_corporateGovernanceProgram == null && !string.IsNullOrEmpty(GovernanceProgram))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access CorporateGovernanceProgram - no database context is set. GovernanceProgram: " + GovernanceProgram + ".");
                        }
                        return null;
                    }
                    _corporateGovernanceProgram = base.SoAContext.CorporateGovernancePrograms.Find(GovernanceProgram);
                    if (_corporateGovernanceProgram != null)
                    {
                        base.SoAContext.Attach(_corporateGovernanceProgram);
                    }
                }
                return _corporateGovernanceProgram;
            }
            set
            {
                if (_corporateGovernanceProgram != value)
                {
                    _corporateGovernanceProgram = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_corporateGovernanceProgram != null)
                    {
                        GovernanceProgram = _corporateGovernanceProgram.CorporateGovernanceProgramId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.CorporateGovernanceProgram;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
