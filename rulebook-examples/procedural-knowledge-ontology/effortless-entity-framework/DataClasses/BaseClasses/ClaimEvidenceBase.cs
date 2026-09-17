
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
    [Table("ClaimEvidence")]
    public class ClaimEvidenceBase : SoAEntityBase
    {
        [Key]
        public string ClaimEvidenceId { get; set; }

        // Formula Name (rulebook: ={{ArticleClaim}} & " <- " & {{EvidenceKind}})
        [NotMapped]
        public string? Name
        {
            get => F.AsString(F.Memo(this, "Name", () => F.Concat(F.Text(F.Of(this.ArticleClaim)), F.S(" <- "), F.Text(F.Of(this.EvidenceKind))))); set { }
        }

        public string? EvidenceKind { get; set; }
        public string? Justification { get; set; }
        // Formula ClaimKind (rulebook: =INDEX(ArticleClaims!{{ClaimKind}}, MATCH({{ArticleClaim}}, ArticleClaims!{{ArticleClaimId}}, 0)))
        [NotMapped]
        public string? ClaimKind
        {
            get => F.AsString(F.Memo(this, "ClaimKind", () => F.Lookup<ArticleClaim>(this, "ArticleClaims", "ArticleClaimId", __c => __c.ArticleClaims, __r => F.Of(__r.ArticleClaimId), F.Of(this.ArticleClaim), __r => F.Of(__r.ClaimKind), () => F.Of(new ArticleClaim().ClaimKind)))); set { }
        }

        // Formula FieldCatalogName (rulebook: =INDEX(RulebookFields!{{FieldName}}, MATCH({{RulebookField}}, RulebookFields!{{RulebookFieldId}}, 0)))
        [NotMapped]
        public string? FieldCatalogName
        {
            get => F.AsString(F.Memo(this, "FieldCatalogName", () => F.Lookup<RulebookField>(this, "RulebookFields", "RulebookFieldId", __c => __c.RulebookFields, __r => F.Of(__r.RulebookFieldId), F.Of(this.RulebookField), __r => F.Of(__r.FieldName), () => F.Of(new RulebookField().FieldName)))); set { }
        }

        // Formula FieldIsWitness (rulebook: =INDEX(RulebookFields!{{IsWitness}}, MATCH({{RulebookField}}, RulebookFields!{{RulebookFieldId}}, 0)))
        [NotMapped]
        public bool? FieldIsWitness
        {
            get => F.AsBool(F.Memo(this, "FieldIsWitness", () => F.Lookup<RulebookField>(this, "RulebookFields", "RulebookFieldId", __c => __c.RulebookFields, __r => F.Of(__r.RulebookFieldId), F.Of(this.RulebookField), __r => F.Of(__r.IsWitness), () => F.Of(new RulebookField().IsWitness)))); set { }
        }

        // Formula FieldHasData (rulebook: =INDEX(RulebookFields!{{HasMeasuredData}}, MATCH({{RulebookField}}, RulebookFields!{{RulebookFieldId}}, 0)))
        [NotMapped]
        public bool? FieldHasData
        {
            get => F.AsBool(F.Memo(this, "FieldHasData", () => F.Lookup<RulebookField>(this, "RulebookFields", "RulebookFieldId", __c => __c.RulebookFields, __r => F.Of(__r.RulebookFieldId), F.Of(this.RulebookField), __r => F.Of(__r.HasMeasuredData), () => F.Of(new RulebookField().HasMeasuredData)))); set { }
        }

        // Formula FieldIsDiscriminating (rulebook: =INDEX(RulebookFields!{{IsDiscriminating}}, MATCH({{RulebookField}}, RulebookFields!{{RulebookFieldId}}, 0)))
        [NotMapped]
        public bool? FieldIsDiscriminating
        {
            get => F.AsBool(F.Memo(this, "FieldIsDiscriminating", () => F.Lookup<RulebookField>(this, "RulebookFields", "RulebookFieldId", __c => __c.RulebookFields, __r => F.Of(__r.RulebookFieldId), F.Of(this.RulebookField), __r => F.Of(__r.IsDiscriminating), () => F.Of(new RulebookField().IsDiscriminating)))); set { }
        }

        // Formula FieldIsContested (rulebook: =INDEX(RulebookFields!{{IsSubstrateContested}}, MATCH({{RulebookField}}, RulebookFields!{{RulebookFieldId}}, 0)))
        [NotMapped]
        public bool? FieldIsContested
        {
            get => F.AsBool(F.Memo(this, "FieldIsContested", () => F.Lookup<RulebookField>(this, "RulebookFields", "RulebookFieldId", __c => __c.RulebookFields, __r => F.Of(__r.RulebookFieldId), F.Of(this.RulebookField), __r => F.Of(__r.IsSubstrateContested), () => F.Of(new RulebookField().IsSubstrateContested)))); set { }
        }

        // Formula TableHasRows (rulebook: =INDEX(RulebookTables!{{HasMeasuredRows}}, MATCH({{RulebookTable}}, RulebookTables!{{RulebookTableId}}, 0)))
        [NotMapped]
        public bool? TableHasRows
        {
            get => F.AsBool(F.Memo(this, "TableHasRows", () => F.Lookup<RulebookTable>(this, "RulebookTables", "RulebookTableId", __c => __c.RulebookTables, __r => F.Of(__r.RulebookTableId), F.Of(this.RulebookTable), __r => F.Of(__r.HasMeasuredRows), () => F.Of(new RulebookTable().HasMeasuredRows)))); set { }
        }

        // Formula QuestionIsAnswered (rulebook: =INDEX(RoleQuestions!{{IsAnswered}}, MATCH({{RoleQuestion}}, RoleQuestions!{{RoleQuestionId}}, 0)))
        [NotMapped]
        public bool? QuestionIsAnswered
        {
            get => F.AsBool(F.Memo(this, "QuestionIsAnswered", () => F.Lookup<RoleQuestion>(this, "RoleQuestions", "RoleQuestionId", __c => __c.RoleQuestions, __r => F.Of(__r.RoleQuestionId), F.Of(this.RoleQuestion), __r => F.Of(__r.IsAnswered), () => F.Of(new RoleQuestion().IsAnswered)))); set { }
        }

        // Formula QuestionWitnessedAnswer (rulebook: =INDEX(RoleQuestions!{{WitnessedAnswer}}, MATCH({{RoleQuestion}}, RoleQuestions!{{RoleQuestionId}}, 0)))
        [NotMapped]
        public string? QuestionWitnessedAnswer
        {
            get => F.AsString(F.Memo(this, "QuestionWitnessedAnswer", () => F.Lookup<RoleQuestion>(this, "RoleQuestions", "RoleQuestionId", __c => __c.RoleQuestions, __r => F.Of(__r.RoleQuestionId), F.Of(this.RoleQuestion), __r => F.Of(__r.WitnessedAnswer), () => F.Of(new RoleQuestion().WitnessedAnswer)))); set { }
        }

        // Formula ProfileMappingCount (rulebook: =INDEX(OntologyProfiles!{{MappingCount}}, MATCH({{OntologyProfile}}, OntologyProfiles!{{OntologyProfileId}}, 0)))
        [NotMapped]
        public int? ProfileMappingCount
        {
            get => F.AsInt(F.Memo(this, "ProfileMappingCount", () => F.Integer(F.Lookup<OntologyProfile>(this, "OntologyProfiles", "OntologyProfileId", __c => __c.OntologyProfiles, __r => F.Of(__r.OntologyProfileId), F.Of(this.OntologyProfile), __r => F.Of(__r.MappingCount), () => F.Of(new OntologyProfile().MappingCount))))); set { }
        }

        // Formula MethodIsApplied (rulebook: =INDEX(KnowledgeMethods!{{IsApplied}}, MATCH({{KnowledgeMethod}}, KnowledgeMethods!{{KnowledgeMethodId}}, 0)))
        [NotMapped]
        public bool? MethodIsApplied
        {
            get => F.AsBool(F.Memo(this, "MethodIsApplied", () => F.Lookup<KnowledgeMethod>(this, "KnowledgeMethods", "KnowledgeMethodId", __c => __c.KnowledgeMethods, __r => F.Of(__r.KnowledgeMethodId), F.Of(this.KnowledgeMethod), __r => F.Of(__r.IsApplied), () => F.Of(new KnowledgeMethod().IsApplied)))); set { }
        }

        // Formula ProcedureExecutionCount (rulebook: =INDEX(Procedures!{{ExecutionCount}}, MATCH({{Procedure}}, Procedures!{{ProcedureId}}, 0)))
        [NotMapped]
        public int? ProcedureExecutionCount
        {
            get => F.AsInt(F.Memo(this, "ProcedureExecutionCount", () => F.Integer(F.Lookup<Procedure>(this, "Procedures", "ProcedureId", __c => __c.Procedures, __r => F.Of(__r.ProcedureId), F.Of(this.Procedure), __r => F.Of(__r.ExecutionCount), () => F.Of(new Procedure().ExecutionCount))))); set { }
        }

        // Formula HasJustification (rulebook: ={{Justification}} <> "")
        [NotMapped]
        public bool? HasJustification
        {
            get => F.AsBool(F.Memo(this, "HasJustification", () => F.IsNotBlank(F.Of(this.Justification)))); set { }
        }

        // Formula IsWitnessProof (rulebook: =AND({{EvidenceKind}} = "Field", {{FieldCatalogName}} <> "", {{FieldIsWitness}}, {{FieldIsDiscriminating}}))
        [NotMapped]
        public bool? IsWitnessProof
        {
            get => F.AsBool(F.Memo(this, "IsWitnessProof", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.EvidenceKind)), F.S("Field"))), F.Bool3(F.IsNotBlank(F.Of(this.FieldCatalogName))), F.Bool3(F.Of(this.FieldIsWitness)), F.Bool3(F.Of(this.FieldIsDiscriminating))))); set { }
        }

        // Formula IsStructuralProof (rulebook: =OR(AND({{EvidenceKind}} = "Table", {{TableHasRows}}), AND({{EvidenceKind}} = "Field", {{FieldCatalogName}} <> "", {{FieldHasData}})))
        [NotMapped]
        public bool? IsStructuralProof
        {
            get => F.AsBool(F.Memo(this, "IsStructuralProof", () => F.Or(F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.EvidenceKind)), F.S("Table"))), F.Bool3(F.Of(this.TableHasRows)))), F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.EvidenceKind)), F.S("Field"))), F.Bool3(F.IsNotBlank(F.Of(this.FieldCatalogName))), F.Bool3(F.Of(this.FieldHasData))))))); set { }
        }

        // Formula IsQuestionProof (rulebook: =AND({{EvidenceKind}} = "RoleQuestion", {{QuestionIsAnswered}}, {{QuestionWitnessedAnswer}} <> ""))
        [NotMapped]
        public bool? IsQuestionProof
        {
            get => F.AsBool(F.Memo(this, "IsQuestionProof", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.EvidenceKind)), F.S("RoleQuestion"))), F.Bool3(F.Of(this.QuestionIsAnswered)), F.Bool3(F.IsNotBlank(F.Of(this.QuestionWitnessedAnswer)))))); set { }
        }

        // Formula IsStandardProof (rulebook: =OR(AND({{EvidenceKind}} = "OntologyProfile", {{ProfileMappingCount}} > 0), AND({{EvidenceKind}} = "KnowledgeMethod", {{MethodIsApplied}})))
        [NotMapped]
        public bool? IsStandardProof
        {
            get => F.AsBool(F.Memo(this, "IsStandardProof", () => F.Or(F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.EvidenceKind)), F.S("OntologyProfile"))), F.Bool3(F.Cmp(F.Of(this.ProfileMappingCount), ">", F.I(0))))), F.Bool3(F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.EvidenceKind)), F.S("KnowledgeMethod"))), F.Bool3(F.Of(this.MethodIsApplied))))))); set { }
        }

        // Formula IsScenarioProof (rulebook: =AND({{EvidenceKind}} = "Procedure", {{ProcedureExecutionCount}} > 0))
        [NotMapped]
        public bool? IsScenarioProof
        {
            get => F.AsBool(F.Memo(this, "IsScenarioProof", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.EvidenceKind)), F.S("Procedure"))), F.Bool3(F.Cmp(F.Of(this.ProcedureExecutionCount), ">", F.I(0)))))); set { }
        }

        // Formula IsValid (rulebook: =AND({{HasJustification}}, OR(AND(OR({{ClaimKind}} = "Prescription", {{ClaimKind}} = "Illustration"), {{IsWitnessProof}}), AND({{ClaimKind}} = "Concept", {{IsStructuralProof}}), AND({{ClaimKind}} = "CompetencyQuestion", {{IsQuestionProof}}), AND({{ClaimKind}} = "Standard", {{IsStandardProof}}), AND({{ClaimKind}} = "Scenario", {{IsScenarioProof}}))))
        [NotMapped]
        public bool? IsValid
        {
            get => F.AsBool(F.Memo(this, "IsValid", () => F.And(F.Bool3(F.Of(this.HasJustification)), F.Bool3(F.Or(F.Bool3(F.And(F.Bool3(F.Or(F.Bool3(F.Eq(F.Of(this.ClaimKind), F.S("Prescription"))), F.Bool3(F.Eq(F.Of(this.ClaimKind), F.S("Illustration"))))), F.Bool3(F.Of(this.IsWitnessProof)))), F.Bool3(F.And(F.Bool3(F.Eq(F.Of(this.ClaimKind), F.S("Concept"))), F.Bool3(F.Of(this.IsStructuralProof)))), F.Bool3(F.And(F.Bool3(F.Eq(F.Of(this.ClaimKind), F.S("CompetencyQuestion"))), F.Bool3(F.Of(this.IsQuestionProof)))), F.Bool3(F.And(F.Bool3(F.Eq(F.Of(this.ClaimKind), F.S("Standard"))), F.Bool3(F.Of(this.IsStandardProof)))), F.Bool3(F.And(F.Bool3(F.Eq(F.Of(this.ClaimKind), F.S("Scenario"))), F.Bool3(F.Of(this.IsScenarioProof))))))))); set { }
        }

        // Formula IsContested (rulebook: =AND({{EvidenceKind}} = "Field", {{FieldIsContested}}))
        [NotMapped]
        public bool? IsContested
        {
            get => F.AsBool(F.Memo(this, "IsContested", () => F.And(F.Bool3(F.Eq(F.Nullif(F.Of(this.EvidenceKind)), F.S("Field"))), F.Bool3(F.Of(this.FieldIsContested))))); set { }
        }

        // Formula IsAgreedEvidence (rulebook: =AND({{IsValid}}, {{IsContested}} = FALSE))
        [NotMapped]
        public bool? IsAgreedEvidence
        {
            get => F.AsBool(F.Memo(this, "IsAgreedEvidence", () => F.And(F.Bool3(F.Of(this.IsValid)), F.Bool3(F.Eq(F.Of(this.IsContested), F.B(false)))))); set { }
        }

        public string? SemanticTypeIri { get; set; }

        public string? ArticleClaim { get; set; }
        public string? RulebookField { get; set; }
        public string? RulebookTable { get; set; }
        public string? RoleQuestion { get; set; }
        public string? OntologyProfile { get; set; }
        public string? KnowledgeMethod { get; set; }
        public string? Procedure { get; set; }

        private ArticleClaim _articleClaimRef;

        [ForeignKey("ArticleClaim")]
        public virtual ArticleClaim ArticleClaimRef
        {
            get
            {
                if (_articleClaimRef == null && !string.IsNullOrEmpty(ArticleClaim))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ArticleClaimRef - no database context is set. ArticleClaim: " + ArticleClaim + ".");
                        }
                        return null;
                    }
                    _articleClaimRef = base.SoAContext.ArticleClaims.Find(ArticleClaim);
                    if (_articleClaimRef != null)
                    {
                        base.SoAContext.Attach(_articleClaimRef);
                    }
                }
                return _articleClaimRef;
            }
            set
            {
                if (_articleClaimRef != value)
                {
                    _articleClaimRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_articleClaimRef != null)
                    {
                        ArticleClaim = _articleClaimRef.ArticleClaimId;
                    }
                }
            }
        }

        private RulebookField _rulebookFieldRef;

        [ForeignKey("RulebookField")]
        public virtual RulebookField RulebookFieldRef
        {
            get
            {
                if (_rulebookFieldRef == null && !string.IsNullOrEmpty(RulebookField))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookFieldRef - no database context is set. RulebookField: " + RulebookField + ".");
                        }
                        return null;
                    }
                    _rulebookFieldRef = base.SoAContext.RulebookFields.Find(RulebookField);
                    if (_rulebookFieldRef != null)
                    {
                        base.SoAContext.Attach(_rulebookFieldRef);
                    }
                }
                return _rulebookFieldRef;
            }
            set
            {
                if (_rulebookFieldRef != value)
                {
                    _rulebookFieldRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookFieldRef != null)
                    {
                        RulebookField = _rulebookFieldRef.RulebookFieldId;
                    }
                }
            }
        }

        private RulebookTable _rulebookTableRef;

        [ForeignKey("RulebookTable")]
        public virtual RulebookTable RulebookTableRef
        {
            get
            {
                if (_rulebookTableRef == null && !string.IsNullOrEmpty(RulebookTable))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RulebookTableRef - no database context is set. RulebookTable: " + RulebookTable + ".");
                        }
                        return null;
                    }
                    _rulebookTableRef = base.SoAContext.RulebookTables.Find(RulebookTable);
                    if (_rulebookTableRef != null)
                    {
                        base.SoAContext.Attach(_rulebookTableRef);
                    }
                }
                return _rulebookTableRef;
            }
            set
            {
                if (_rulebookTableRef != value)
                {
                    _rulebookTableRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_rulebookTableRef != null)
                    {
                        RulebookTable = _rulebookTableRef.RulebookTableId;
                    }
                }
            }
        }

        private RoleQuestion _roleQuestionRef;

        [ForeignKey("RoleQuestion")]
        public virtual RoleQuestion RoleQuestionRef
        {
            get
            {
                if (_roleQuestionRef == null && !string.IsNullOrEmpty(RoleQuestion))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access RoleQuestionRef - no database context is set. RoleQuestion: " + RoleQuestion + ".");
                        }
                        return null;
                    }
                    _roleQuestionRef = base.SoAContext.RoleQuestions.Find(RoleQuestion);
                    if (_roleQuestionRef != null)
                    {
                        base.SoAContext.Attach(_roleQuestionRef);
                    }
                }
                return _roleQuestionRef;
            }
            set
            {
                if (_roleQuestionRef != value)
                {
                    _roleQuestionRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_roleQuestionRef != null)
                    {
                        RoleQuestion = _roleQuestionRef.RoleQuestionId;
                    }
                }
            }
        }

        private OntologyProfile _ontologyProfileRef;

        [ForeignKey("OntologyProfile")]
        public virtual OntologyProfile OntologyProfileRef
        {
            get
            {
                if (_ontologyProfileRef == null && !string.IsNullOrEmpty(OntologyProfile))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access OntologyProfileRef - no database context is set. OntologyProfile: " + OntologyProfile + ".");
                        }
                        return null;
                    }
                    _ontologyProfileRef = base.SoAContext.OntologyProfiles.Find(OntologyProfile);
                    if (_ontologyProfileRef != null)
                    {
                        base.SoAContext.Attach(_ontologyProfileRef);
                    }
                }
                return _ontologyProfileRef;
            }
            set
            {
                if (_ontologyProfileRef != value)
                {
                    _ontologyProfileRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_ontologyProfileRef != null)
                    {
                        OntologyProfile = _ontologyProfileRef.OntologyProfileId;
                    }
                }
            }
        }

        private KnowledgeMethod _knowledgeMethodRef;

        [ForeignKey("KnowledgeMethod")]
        public virtual KnowledgeMethod KnowledgeMethodRef
        {
            get
            {
                if (_knowledgeMethodRef == null && !string.IsNullOrEmpty(KnowledgeMethod))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access KnowledgeMethodRef - no database context is set. KnowledgeMethod: " + KnowledgeMethod + ".");
                        }
                        return null;
                    }
                    _knowledgeMethodRef = base.SoAContext.KnowledgeMethods.Find(KnowledgeMethod);
                    if (_knowledgeMethodRef != null)
                    {
                        base.SoAContext.Attach(_knowledgeMethodRef);
                    }
                }
                return _knowledgeMethodRef;
            }
            set
            {
                if (_knowledgeMethodRef != value)
                {
                    _knowledgeMethodRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_knowledgeMethodRef != null)
                    {
                        KnowledgeMethod = _knowledgeMethodRef.KnowledgeMethodId;
                    }
                }
            }
        }

        private Procedure _procedureRef;

        [ForeignKey("Procedure")]
        public virtual Procedure ProcedureRef
        {
            get
            {
                if (_procedureRef == null && !string.IsNullOrEmpty(Procedure))
                {
                    if (base.SoAContext == null)
                    {
                        if (SoAEFContext.ThrowErrorOnContextMissing)
                        {
                            throw new InvalidOperationException("Cannot access ProcedureRef - no database context is set. Procedure: " + Procedure + ".");
                        }
                        return null;
                    }
                    _procedureRef = base.SoAContext.Procedures.Find(Procedure);
                    if (_procedureRef != null)
                    {
                        base.SoAContext.Attach(_procedureRef);
                    }
                }
                return _procedureRef;
            }
            set
            {
                if (_procedureRef != value)
                {
                    _procedureRef = value;
                    // Only push the FK when associating a real parent. EF's relationship fixup
                    // assigns this navigation to null whenever the parent isn't tracked yet (e.g.
                    // while a query is materializing children before parents); nulling the scalar
                    // FK there would CORRUPT the raw fact (the row's FK silently becomes null),
                    // which then breaks every SUMIFS/COUNTIFS that filters on it. Assigning a
                    // non-null parent still keeps the FK in sync.
                    if (_procedureRef != null)
                    {
                        Procedure = _procedureRef.ProcedureId;
                    }
                }
            }
        }


        protected override void LazyLoadProperties()
        {
            _ = this.ArticleClaimRef;
            _ = this.RulebookFieldRef;
            _ = this.RulebookTableRef;
            _ = this.RoleQuestionRef;
            _ = this.OntologyProfileRef;
            _ = this.KnowledgeMethodRef;
            _ = this.ProcedureRef;
        }

        public override string ToString()
        {
            return this.Name?.ToString() ?? base.ToString() ?? "";
        }
    }
}
