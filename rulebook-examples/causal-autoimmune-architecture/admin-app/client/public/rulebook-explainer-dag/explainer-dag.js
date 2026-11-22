/**
 * Portable Explainer DAG — vanilla-JS client for any HTML host. Feature-parity
 * port of rulebook-to-explainer-dag-react:
 *   • Provenance toggle pill (top-right)      — glyphs on/off; OFF still allows dbl-click
 *   • Narration slider (RuleSpeak®·English·Formula) — global, remembered
 *   • Settings gear (top-right)               — checkboxes gating which sections
 *                                               show in the hover card AND the page
 *   • Hover micro-page over cells             — sticky, dialect follows narration
 *   • Double-click a cell                     — opens the full DAG page
 *   • Full field page                         — hero + table link, rule (RuleTree),
 *                                               inputs, consumers (direct+transitive),
 *                                               ground-truth leaves
 * Requires: embedded-graph.js, dag-resolver.js, routing.js, dag.css
 */
(function (global) {
  const R = () => global.EffortlessDagResolver;

  // ════════════════════════════════════════════════════════════════════════
  //  CHROME i18n — the fixed UI text around every page (buttons, section
  //  titles, hover-card labels, tooltips), one dict per language registered in
  //  rulebook-rulespeak-core. This is deliberately NOT where the RuleSpeak®
  //  sentence bodies live: those are rendered per language at build time by the
  //  shared RuleSpeak® engine and baked into embedded-graph.js (rulespeakByLang).
  //  `lang` is set once, from init(), by whatever host page embeds this module —
  //  see tt() below.
  // ════════════════════════════════════════════════════════════════════════
  let lang = "en";
  // Russian counts: 1 строка, 2 строки, 5 строк (and 11-14 строк).
  const slavicCount = (n, one, few, many) => {
    const a = Math.abs(n) % 10, b = Math.abs(n) % 100;
    return a === 1 && b !== 11 ? one : a >= 2 && a <= 4 && (b < 12 || b > 14) ? few : many;
  };
  const CHROME = {
    en: {
      back: "← back", backTitle: "Go back one step",
      home: "🏠 Home", homeTitle: "Leave the explainer — back to the app",
      tablesCrumb: "Tables",
      narrRulespeak: "Docs", narrEnglish: "Simple", narrFormula: "Formula",
      narrRulespeakHint: "The declarative business rule (only if / must / priority)",
      narrEnglishHint: "The formula read as a plain English sentence",
      narrFormulaHint: "The raw spreadsheet-style formula, with clickable fields",
      narrAriaLabel: "How to explain this rule",
      gearTitle: "Settings — choose what shows in the hover card and on the field page",
      gearAriaLabel: "Settings",
      gearPopTitle: "Show in popup & on page",
      gearReset: "Reset to all on",
      elemDesc: "Description", elemDescHint: "The field's plain-language description.",
      elemRulespeak: "Docs", elemRulespeakHint: "The declarative business rule.",
      elemEnglish: "Simple", elemEnglishHint: "The formula read as a plain sentence.",
      elemFormula: "Formula", elemFormulaHint: "The raw spreadsheet-style formula.",
      elemInputs: "Inputs", elemInputsHint: "The fields this one-line function calls.",
      elemConsumers: "Consumers", elemConsumersHint: "Fields whose function calls this one.",
      provOn: "Provenance · ON", provOff: "Provenance · off",
      provOnTitle: "Hide ƒ provenance glyphs. Double-click on cells still opens the DAG.",
      provOffTitle: "Show ƒ provenance glyphs on cells with explorable provenance.",
      downloadXlsx: "Download workbook", downloadXlsxTitle: "Download the rulebook as an Excel workbook (.xlsx)",
      gearShowAs: "Show as", gearLanguage: "Language",
      navSearch: "Filter tables and fields…",
      navDerivedOnly: "Derived only",
      navNoMatch: "Nothing matches.",
      groupOther: "Other",
      tabAll: "All",
      tabDerived: "Derived",
      tabStored: "Stored",
      tabRelationships: "Relationships",
      viewGroups: "Groups",
      viewList: "A–Z",
      viewGraph: "Graph",
      tablesCount: (n) => `${n} ${n === 1 ? "table" : "tables"}`,
      graphHint: "Each box is a table; a line runs from a table to the table it refers to. Click a table to open it.",
      filterFields: "Filter fields…",
      cellHintOn: "Hover the badge to peek; double-click to open the full DAG page",
      cellHintOff: "Double-click to see provenance (toggle ƒ glyphs in header)",
      ontologyTitle: "The ontology",
      ontologyLead: "Every table in the rulebook. Click a table to see its columns.",
      fieldsCount: (n) => `${n} fields`, derivedCount: (n) => `${n} derived`, relCount: (n) => `${n} rel`,
      tableNotFound: "Table not found.", fieldNotFound: "Field not found in rulebook.",
      nullable: "nullable",
      hopsFromGround: (n) => (n === 1 ? "1 hop from ground truth" : `${n} hops from ground truth`),
      relationshipPointer: "Relationship pointer", groundTruth: "Ground truth",
      relPointerBody: (link) => `This field is a pointer to ${link}. It is not derived from anything else — it’s the link the rest of the DAG hangs off.`,
      relPointerBodyNoLink: "This field is a pointer to another table. It is not derived from anything else — it’s the link the rest of the DAG hangs off.",
      groundTruthBody: (dt) => `This cell is a leaf. It is <em>written</em> directly to the database (datatype <code>${dt}</code>) and is not computed from anything else. Everything else upstream eventually traces back to values like this one.`,
      openTable: (t) => `Open ${t} →`,
      ruleCardTitle: "This cell is the value of one declarative rule",
      oneLineFnCardTitle: "This cell is the value of one one-line function",
      inRuleSpeak: "In Docs", inEnglish: "In simple terms", asFormula: "As a formula",
      mechanicalFlag: "⚠︎ mechanical", mechanicalTitle: "Faithful but clunky wording — a flag for an optional reword pass",
      noRuleRendered: (t) => `No declarative rule was rendered for this ${t} field.`,
      obligations: "Obligations",
      ruleSpeakHintFormula: "This is the same logic the formula encodes, written as a business rule — rendered from the rulebook by the shared Docs engine.",
      ruleSpeakHintNoFormula: "Rendered from the rulebook by the shared Docs engine.",
      englishHint: "Each underlined field is itself a one-line function. Click any to drill in.",
      noFormulaSentence: (t) => `This ${t} has no spreadsheet formula to read as a sentence — it is a roll-up defined structurally over a relationship. Switch to <strong>Docs</strong> to see how it’s defined.`,
      formulaHint: "Each chip is itself a one-line function. Click any chip to drill in.",
      noFormulaAtAll: (t) => `This ${t} has no spreadsheet formula — it is a roll-up defined structurally over a relationship. Switch to <strong>Docs</strong> to see how it’s defined.`,
      inputsTitle: "Inputs", inputsLead: "Fields this one-line function calls.",
      groundTruthInline: (dt) => `Ground truth · datatype ${dt}`,
      consumersTitle: "Consumers",
      consumersDirect: "direct", consumersTransitive: "transitive",
      consumersEmpty: "Nothing else in the rulebook references this cell yet.",
      consumersLead: "Fields whose one-line function calls this one.",
      showTransitive: (n) => `Show ${n} transitive consumers (downstream of downstream)`,
      leavesTitle: "Ground truth at the leaves",
      leavesLead: "Ultimately, this cell’s value depends on these raw values. Editing any of these and rebuilding propagates here automatically.",
      hoverGroundTruth: (dt) => `Ground truth — written directly to the database${dt ? ` (datatype <code>${dt}</code>)` : ""}.`,
      hoverRelPointer: "Relationship pointer — the link the rest of the DAG hangs off.",
      hoverNoRule: "No declarative rule rendered.",
      hoverNoFormula: "No formula — a structural roll-up. See Docs.",
      hoverInputs: "Inputs",
      hoverHint: "Double-click the cell to open the full DAG page.",
      hoverOpenPage: "Open the full page",
      // The witness (witness.js): one row's value and the values it came from.
      witnessTitle: "Witnessed, down to ground truth",
      witnessLead: "This cell's value for one row, and every value it came from, down to the stored ones. Each was computed here, just now, by this project's generated TypeScript (rulebook-to-typescript). Open a line to see what it was made from.",
      witnessRow: "Row",
      witnessForRow: (label) => `For ${label}`,
      witnessComputing: "Computing this row with the generated TypeScript…",
      witnessNoRows: (t) => `${t} has no rows to witness.`,
      witnessFailed: "The witness could not be computed.",
      witnessGroundTruth: "ground truth",
      witnessPointsTo: (t, label) => `points to ${t} ${label}`,
      witnessPointsNowhere: (t) => `names no ${t} row`,
      witnessBlank: "blank",
      witnessEmptyText: "empty text",
      witnessError: (m) => `could not be computed: ${m}`,
      witnessStep: (n, of) => `Step ${n} of ${of} applied.`,
      witnessNoStep: "No step applied, so the value is blank.",
      // `noun` is what one row of the table is called ("case"); "row" only when the
      // graph does not say (rowNoun).
      witnessStepMark: (noun) => `applied to this ${noun || "row"}`,
      witnessLookup: (key, value, t, label) => `Looked up through ${key} = ${value}: read from ${t} ${label}.`,
      witnessLookupMiss: (key, value, t) => `Looked up through ${key} = ${value}, which names no ${t} row.`,
      witnessOver: (op, n, t) => ({
        COUNTIFS: `Counted: ${n} ${t} ${n === 1 ? "row matches" : "rows match"}.`,
        COUNT: `Counted: ${n} ${t} ${n === 1 ? "row matches" : "rows match"}.`,
        SUM: `Added up over the ${n} ${t} ${n === 1 ? "row that matches" : "rows that match"}.`,
        AVERAGE: `Averaged over the ${n} ${t} ${n === 1 ? "row that matches" : "rows that match"}.`,
        MIN: `The smallest over the ${n} ${t} ${n === 1 ? "row that matches" : "rows that match"}.`,
        MAX: `The largest over the ${n} ${t} ${n === 1 ? "row that matches" : "rows that match"}.`,
      }[op] || `Computed over the ${n} ${t} ${n === 1 ? "row that matches" : "rows that match"}.`),
      witnessOverUnknown: "Computed over rows this page cannot list.",
      witnessMoreRows: (n) => `… and ${n} more ${n === 1 ? "row" : "rows"}`,
      witnessSeenAbove: "already shown above",
      type_raw: "Ground truth", type_calculated: "Calculated", type_aggregation: "Aggregation", type_relationship: "Relationship", type_lookup: "Lookup",
    },
    ja: {
      back: "← 戻る", backTitle: "1つ前に戻る",
      home: "🏠 ホーム", homeTitle: "エクスプレイナーを離れてアプリに戻る",
      tablesCrumb: "テーブル",
      narrRulespeak: "ドキュメント", narrEnglish: "平易な読み下し", narrFormula: "数式",
      narrRulespeakHint: "宣言的なビジネスルール（only if / must / priority）",
      narrEnglishHint: "数式を平易な文として読んだもの",
      narrFormulaHint: "生の表計算形式の数式（クリックできるフィールド付き）",
      narrAriaLabel: "このルールの説明方法",
      gearTitle: "設定 — ホバーカードとフィールドページに表示する項目を選択",
      gearAriaLabel: "設定",
      gearPopTitle: "ポップアップとページに表示する項目",
      gearReset: "すべてオンに戻す",
      elemDesc: "説明", elemDescHint: "フィールドの平易な説明文。",
      elemRulespeak: "ドキュメント", elemRulespeakHint: "宣言的なビジネスルール。",
      elemEnglish: "平易な読み下し", elemEnglishHint: "数式を平易な文として読んだもの。",
      elemFormula: "数式", elemFormulaHint: "生の表計算形式の数式。",
      elemInputs: "入力", elemInputsHint: "この一行関数が呼び出しているフィールド。",
      elemConsumers: "参照元", elemConsumersHint: "この関数を呼び出しているフィールド。",
      provOn: "由来表示 · ON", provOff: "由来表示 · off",
      provOnTitle: "ƒ 由来バッジを非表示にします。セルのダブルクリックではDAGが開きます。",
      provOffTitle: "由来を確認できるセルに ƒ バッジを表示します。",
      downloadXlsx: "ワークブックをダウンロード", downloadXlsxTitle: "ルールブックをExcelワークブック（.xlsx）としてダウンロードします",
      gearShowAs: "表示形式", gearLanguage: "言語",
      navSearch: "テーブルとフィールドを絞り込む…",
      navDerivedOnly: "導出のみ",
      navNoMatch: "一致するものがありません。",
      groupOther: "その他",
      tabAll: "すべて",
      tabDerived: "導出",
      tabStored: "保存値",
      tabRelationships: "リレーション",
      viewGroups: "グループ",
      viewList: "A–Z",
      viewGraph: "グラフ",
      tablesCount: (n) => `${n} テーブル`,
      graphHint: "各ボックスはテーブルです。線は、そのテーブルが参照する先のテーブルへ延びています。クリックで開きます。",
      filterFields: "フィールドを絞り込む…",
      cellHintOn: "バッジにカーソルを合わせるとプレビュー、ダブルクリックでDAGページを開きます",
      cellHintOff: "ダブルクリックで由来を確認（ヘッダーで ƒ バッジの表示を切替）",
      ontologyTitle: "オントロジー",
      ontologyLead: "ルールブック内のすべてのテーブル。テーブルをクリックすると列を確認できます。",
      fieldsCount: (n) => `${n} フィールド`, derivedCount: (n) => `${n} 算出`, relCount: (n) => `${n} リレーション`,
      tableNotFound: "テーブルが見つかりません。", fieldNotFound: "ルールブックにこのフィールドは見つかりません。",
      nullable: "null許可",
      hopsFromGround: (n) => `根拠データから ${n} ホップ`,
      relationshipPointer: "リレーションのポインタ", groundTruth: "根拠データ",
      relPointerBody: (link) => `このフィールドは ${link} へのポインタです。他の何かから導出されたものではなく、DAGの残りがぶら下がるリンクです。`,
      relPointerBodyNoLink: "このフィールドは別テーブルへのポインタです。他の何かから導出されたものではなく、DAGの残りがぶら下がるリンクです。",
      groundTruthBody: (dt) => `このセルは末端です。データベースに直接<em>書き込まれた</em>値（データ型 <code>${dt}</code>）で、他の何からも算出されていません。この先の上流はすべて、最終的にこのような値にたどり着きます。`,
      openTable: (t) => `${t} を開く →`,
      ruleCardTitle: "このセルは1つの宣言的ルールの値です",
      oneLineFnCardTitle: "このセルは1つの一行関数の値です",
      inRuleSpeak: "ドキュメント では", inEnglish: "平易な言葉では", asFormula: "数式として",
      mechanicalFlag: "⚠︎ 機械的", mechanicalTitle: "意味は正確ですが表現がぎこちない箇所です — 表現の見直し候補",
      noRuleRendered: (t) => `この ${t} フィールドには宣言的ルールが生成されませんでした。`,
      obligations: "義務（Obligations）",
      ruleSpeakHintFormula: "これは数式が表しているのと同じロジックを、ビジネスルールとして書き直したものです — 共有の ドキュメント エンジンがルールブックから生成しています。",
      ruleSpeakHintNoFormula: "共有の ドキュメント エンジンがルールブックから生成しています。",
      englishHint: "下線付きの各フィールドもそれ自体が一行関数です。クリックすると詳細を確認できます。",
      noFormulaSentence: (t) => `この ${t} には文として読める数式がありません — リレーションに基づいて構造的に定義されたロールアップです。定義内容を確認するには <strong>ドキュメント</strong> に切り替えてください。`,
      formulaHint: "各チップもそれ自体が一行関数です。クリックすると詳細を確認できます。",
      noFormulaAtAll: (t) => `この ${t} には数式がありません — リレーションに基づいて構造的に定義されたロールアップです。定義内容を確認するには <strong>ドキュメント</strong> に切り替えてください。`,
      inputsTitle: "入力", inputsLead: "この一行関数が呼び出しているフィールド。",
      groundTruthInline: (dt) => `根拠データ · データ型 ${dt}`,
      consumersTitle: "参照元",
      consumersDirect: "直接", consumersTransitive: "間接",
      consumersEmpty: "ルールブック内で、このセルを参照しているものは他にありません。",
      consumersLead: "この一行関数を呼び出しているフィールド。",
      showTransitive: (n) => `間接的な参照元を ${n} 件表示（さらに下流の下流）`,
      leavesTitle: "末端の根拠データ",
      leavesLead: "このセルの値は最終的にこれらの生データに依存しています。これらのいずれかを編集して再ビルドすると、自動的にここまで反映されます。",
      hoverGroundTruth: (dt) => `根拠データ — データベースに直接書き込まれます${dt ? `（データ型 <code>${dt}</code>）` : ""}。`,
      hoverRelPointer: "リレーションのポインタ — DAGの残りがぶら下がるリンクです。",
      hoverNoRule: "宣言的ルールは生成されませんでした。",
      hoverNoFormula: "数式がありません — 構造的なロールアップです。ドキュメント を参照してください。",
      hoverInputs: "入力",
      hoverHint: "セルをダブルクリックすると、DAGページ全体が開きます。",
      hoverOpenPage: "ページ全体を開く",
      // The witness (witness.js): one row's value and the values it came from.
      witnessTitle: "根拠データまでたどった実証",
      witnessLead: "ある 1 行についてのこのセルの値と、その元になったすべての値を、保存されている値までたどって示します。どの値も、このプロジェクトの生成済み TypeScript（rulebook-to-typescript）が、いまここで計算したものです。行を開くと、その値の元になった値を確認できます。",
      witnessRow: "行",
      witnessForRow: (label) => `${label} について`,
      witnessComputing: "生成済み TypeScript でこの行を計算しています…",
      witnessNoRows: (t) => `${t} には実証できる行がありません。`,
      witnessFailed: "実証を計算できませんでした。",
      witnessGroundTruth: "根拠データ",
      witnessPointsTo: (t, label) => `${t} の ${label} を指しています`,
      witnessPointsNowhere: (t) => `${t} のどの行も指していません`,
      witnessBlank: "空",
      witnessEmptyText: "空のテキスト",
      witnessError: (m) => `計算できませんでした: ${m}`,
      witnessStep: (n, of) => `${of} 個のステップのうち、ステップ ${n} が適用されました。`,
      witnessNoStep: "どのステップも適用されなかったため、値は空です。",
      witnessStepMark: (noun) => (noun ? `この「${noun}」に適用` : "この行に適用"),
      witnessLookup: (key, value, t, label) => `${key} = ${value} で参照: ${t} の ${label} から読み取りました。`,
      witnessLookupMiss: (key, value, t) => `${key} = ${value} で参照しましたが、${t} に該当する行がありません。`,
      witnessOver: (op, n, t) => ({
        COUNTIFS: `件数: 一致する ${t} の行は ${n} 件です。`,
        COUNT: `件数: 一致する ${t} の行は ${n} 件です。`,
        SUM: `一致する ${t} の ${n} 行を合計しました。`,
        AVERAGE: `一致する ${t} の ${n} 行の平均です。`,
        MIN: `一致する ${t} の ${n} 行のうち最小の値です。`,
        MAX: `一致する ${t} の ${n} 行のうち最大の値です。`,
      }[op] || `一致する ${t} の ${n} 行から計算しました。`),
      witnessOverUnknown: "このページでは一覧できない行から計算しました。",
      witnessMoreRows: (n) => `… ほか ${n} 行`,
      witnessSeenAbove: "上に表示済み",
      type_raw: "根拠データ", type_calculated: "算出", type_aggregation: "集計", type_relationship: "リレーション", type_lookup: "参照",
    },
    de: {
      back: "← zurück", backTitle: "Einen Schritt zurück",
      home: "🏠 Start", homeTitle: "Den Explainer verlassen — zurück zur App",
      tablesCrumb: "Tabellen",
      narrRulespeak: "Dokumentation", narrEnglish: "Klartext", narrFormula: "Formel",
      narrRulespeakHint: "Die deklarative Geschäftsregel (nur wenn / muss / Priorität)",
      narrEnglishHint: "Die Formel als einfacher Satz gelesen",
      narrFormulaHint: "Die rohe Formel im Tabellenkalkulationsstil, mit anklickbaren Feldern",
      narrAriaLabel: "Wie diese Regel erklärt wird",
      gearTitle: "Einstellungen — wählen, was in der Hover-Karte und auf der Feldseite erscheint",
      gearAriaLabel: "Einstellungen",
      gearPopTitle: "Im Popup und auf der Seite anzeigen",
      gearReset: "Alles wieder einschalten",
      elemDesc: "Beschreibung", elemDescHint: "Die Beschreibung des Feldes in Alltagssprache.",
      elemRulespeak: "Dokumentation", elemRulespeakHint: "Die deklarative Geschäftsregel.",
      elemEnglish: "Klartext", elemEnglishHint: "Die Formel als einfacher Satz gelesen.",
      elemFormula: "Formel", elemFormulaHint: "Die rohe Formel im Tabellenkalkulationsstil.",
      elemInputs: "Eingaben", elemInputsHint: "Die Felder, die diese einzeilige Funktion aufruft.",
      elemConsumers: "Verwender", elemConsumersHint: "Felder, deren Funktion diese aufruft.",
      provOn: "Herkunft · AN", provOff: "Herkunft · aus",
      provOnTitle: "ƒ-Herkunftssymbole ausblenden. Ein Doppelklick auf Zellen öffnet den DAG weiterhin.",
      provOffTitle: "ƒ-Herkunftssymbole an Zellen mit nachvollziehbarer Herkunft anzeigen.",
      downloadXlsx: "Arbeitsmappe laden", downloadXlsxTitle: "Das Regelbuch als Excel-Arbeitsmappe (.xlsx) herunterladen",
      gearShowAs: "Darstellung", gearLanguage: "Sprache",
      navSearch: "Tabellen und Felder filtern…",
      navDerivedOnly: "Nur abgeleitete",
      navNoMatch: "Keine Treffer.",
      groupOther: "Weitere",
      tabAll: "Alle",
      tabDerived: "Abgeleitet",
      tabStored: "Gespeichert",
      tabRelationships: "Beziehungen",
      viewGroups: "Gruppen",
      viewList: "A–Z",
      viewGraph: "Graph",
      tablesCount: (n) => `${n} Tabellen`,
      graphHint: "Jedes Kästchen ist eine Tabelle; eine Linie führt von einer Tabelle zu der Tabelle, auf die sie verweist. Zum Öffnen anklicken.",
      filterFields: "Felder filtern…",
      cellHintOn: "Symbol überfahren für eine Vorschau; Doppelklick öffnet die vollständige DAG-Seite",
      cellHintOff: "Doppelklick zeigt die Herkunft (ƒ-Symbole in der Kopfzeile umschalten)",
      ontologyTitle: "Die Ontologie",
      ontologyLead: "Jede Tabelle im Regelbuch. Klicken Sie auf eine Tabelle, um ihre Spalten zu sehen.",
      fieldsCount: (n) => `${n} Felder`, derivedCount: (n) => `${n} abgeleitet`, relCount: (n) => `${n} Bez.`,
      tableNotFound: "Tabelle nicht gefunden.", fieldNotFound: "Feld im Regelbuch nicht gefunden.",
      nullable: "NULL erlaubt",
      hopsFromGround: (n) => (n === 1 ? "1 Schritt von den Grunddaten" : `${n} Schritte von den Grunddaten`),
      relationshipPointer: "Beziehungszeiger", groundTruth: "Grunddaten",
      relPointerBody: (link) => `Dieses Feld ist ein Zeiger auf ${link}. Es wird aus nichts anderem abgeleitet — es ist die Verknüpfung, an der der Rest des DAG hängt.`,
      relPointerBodyNoLink: "Dieses Feld ist ein Zeiger auf eine andere Tabelle. Es wird aus nichts anderem abgeleitet — es ist die Verknüpfung, an der der Rest des DAG hängt.",
      groundTruthBody: (dt) => `Diese Zelle ist ein Blatt. Sie wird direkt in die Datenbank <em>geschrieben</em> (Datentyp <code>${dt}</code>) und aus nichts anderem berechnet. Alles andere weiter oben geht letztlich auf Werte wie diesen zurück.`,
      openTable: (t) => `${t} öffnen →`,
      ruleCardTitle: "Diese Zelle ist der Wert einer deklarativen Regel",
      oneLineFnCardTitle: "Diese Zelle ist der Wert einer einzeiligen Funktion",
      inRuleSpeak: "In Dokumentation", inEnglish: "Im Klartext", asFormula: "Als Formel",
      mechanicalFlag: "⚠︎ mechanisch", mechanicalTitle: "Inhaltlich korrekt, aber holprig formuliert — ein Hinweis für eine optionale Überarbeitung",
      noRuleRendered: (t) => `Für dieses ${t}-Feld wurde keine deklarative Regel erzeugt.`,
      obligations: "Verpflichtungen",
      ruleSpeakHintFormula: "Dies ist dieselbe Logik, die die Formel ausdrückt, als Geschäftsregel formuliert — von der gemeinsamen Dokumentation-Engine aus dem Regelbuch erzeugt.",
      ruleSpeakHintNoFormula: "Von der gemeinsamen Dokumentation-Engine aus dem Regelbuch erzeugt.",
      englishHint: "Jedes unterstrichene Feld ist selbst eine einzeilige Funktion. Klicken Sie darauf, um tiefer einzusteigen.",
      noFormulaSentence: (t) => `Dieses ${t} hat keine Tabellenformel, die sich als Satz lesen ließe — es ist eine Aggregation, strukturell über eine Beziehung definiert. Wechseln Sie zu <strong>Dokumentation</strong>, um die Definition zu sehen.`,
      formulaHint: "Jeder Chip ist selbst eine einzeilige Funktion. Klicken Sie auf einen Chip, um tiefer einzusteigen.",
      noFormulaAtAll: (t) => `Dieses ${t} hat keine Tabellenformel — es ist eine Aggregation, strukturell über eine Beziehung definiert. Wechseln Sie zu <strong>Dokumentation</strong>, um die Definition zu sehen.`,
      inputsTitle: "Eingaben", inputsLead: "Felder, die diese einzeilige Funktion aufruft.",
      groundTruthInline: (dt) => `Grunddaten · Datentyp ${dt}`,
      consumersTitle: "Verwender",
      consumersDirect: "direkt", consumersTransitive: "transitiv",
      consumersEmpty: "Nichts anderes im Regelbuch verweist bisher auf diese Zelle.",
      consumersLead: "Felder, deren einzeilige Funktion diese aufruft.",
      showTransitive: (n) => `${n} transitive Verwender anzeigen (weiter stromabwärts)`,
      leavesTitle: "Grunddaten an den Blättern",
      leavesLead: "Letztlich hängt der Wert dieser Zelle von diesen Rohwerten ab. Wird einer davon geändert und neu gebaut, wirkt sich das automatisch hier aus.",
      hoverGroundTruth: (dt) => `Grunddaten — direkt in die Datenbank geschrieben${dt ? ` (Datentyp <code>${dt}</code>)` : ""}.`,
      hoverRelPointer: "Beziehungszeiger — die Verknüpfung, an der der Rest des DAG hängt.",
      hoverNoRule: "Keine deklarative Regel erzeugt.",
      hoverNoFormula: "Keine Formel — eine strukturelle Aggregation. Siehe Dokumentation.",
      hoverInputs: "Eingaben",
      hoverHint: "Doppelklick auf die Zelle öffnet die vollständige DAG-Seite.",
      hoverOpenPage: "Vollständige Seite öffnen",
      // The witness (witness.js): one row's value and the values it came from.
      witnessTitle: "Nachvollzogen bis zu den Grunddaten",
      witnessLead: "Der Wert dieser Zelle für eine Zeile und jeder Wert, aus dem er entstanden ist, bis hinunter zu den gespeicherten Werten. Jeder wurde soeben hier vom generierten TypeScript dieses Projekts (rulebook-to-typescript) berechnet. Öffnen Sie eine Zeile, um zu sehen, woraus sie entstanden ist.",
      witnessRow: "Zeile",
      witnessForRow: (label) => `Für ${label}`,
      witnessComputing: "Diese Zeile wird mit dem generierten TypeScript berechnet…",
      witnessNoRows: (t) => `${t} enthält keine Zeilen, die sich nachvollziehen ließen.`,
      witnessFailed: "Der Nachweis konnte nicht berechnet werden.",
      witnessGroundTruth: "Grunddaten",
      witnessPointsTo: (t, label) => `verweist auf ${t} ${label}`,
      witnessPointsNowhere: (t) => `verweist auf keine Zeile in ${t}`,
      witnessBlank: "leer",
      witnessEmptyText: "leerer Text",
      witnessError: (m) => `konnte nicht berechnet werden: ${m}`,
      witnessStep: (n, of) => `Schritt ${n} von ${of} wurde angewendet.`,
      witnessNoStep: "Kein Schritt wurde angewendet, daher ist der Wert leer.",
      // The noun's gender is not known, so it follows a noun whose gender is.
      witnessStepMark: (noun) => (noun ? `auf diesen Datensatz angewendet (${noun})` : "auf diese Zeile angewendet"),
      witnessLookup: (key, value, t, label) => `Nachgeschlagen über ${key} = ${value}: gelesen aus ${t} ${label}.`,
      witnessLookupMiss: (key, value, t) => `Nachgeschlagen über ${key} = ${value}; dazu gibt es keine Zeile in ${t}.`,
      witnessOver: (op, n, t) => {
        const rows = `${n} ${n === 1 ? "passende Zeile" : "passenden Zeilen"} in ${t}`;
        return ({
          COUNTIFS: `Gezählt: ${n} ${n === 1 ? "Zeile" : "Zeilen"} in ${t} ${n === 1 ? "passt" : "passen"}.`,
          COUNT: `Gezählt: ${n} ${n === 1 ? "Zeile" : "Zeilen"} in ${t} ${n === 1 ? "passt" : "passen"}.`,
          SUM: `Summiert über die ${rows}.`,
          AVERAGE: `Durchschnitt über die ${rows}.`,
          MIN: `Der kleinste Wert über die ${rows}.`,
          MAX: `Der größte Wert über die ${rows}.`,
        }[op] || `Berechnet über die ${rows}.`);
      },
      witnessOverUnknown: "Berechnet über Zeilen, die diese Seite nicht auflisten kann.",
      witnessMoreRows: (n) => `… und ${n} weitere ${n === 1 ? "Zeile" : "Zeilen"}`,
      witnessSeenAbove: "bereits oben gezeigt",
      type_raw: "Grunddaten", type_calculated: "Berechnet", type_aggregation: "Aggregation", type_relationship: "Beziehung", type_lookup: "Nachschlagen",
    },
    es: {
      back: "← atrás", backTitle: "Volver un paso",
      home: "🏠 Inicio", homeTitle: "Salir del explicador — volver a la aplicación",
      tablesCrumb: "Tablas",
      narrRulespeak: "Documentación", narrEnglish: "Lenguaje claro", narrFormula: "Fórmula",
      narrRulespeakHint: "La regla de negocio declarativa (solo si / debe / prioridad)",
      narrEnglishHint: "La fórmula leída como una oración sencilla",
      narrFormulaHint: "La fórmula en bruto, estilo hoja de cálculo, con campos clicables",
      narrAriaLabel: "Cómo explicar esta regla",
      gearTitle: "Configuración — elige qué se muestra en la tarjeta emergente y en la página del campo",
      gearAriaLabel: "Configuración",
      gearPopTitle: "Mostrar en la ventana emergente y en la página",
      gearReset: "Volver a activar todo",
      elemDesc: "Descripción", elemDescHint: "La descripción del campo en lenguaje sencillo.",
      elemRulespeak: "Documentación", elemRulespeakHint: "La regla de negocio declarativa.",
      elemEnglish: "Lenguaje claro", elemEnglishHint: "La fórmula leída como una oración sencilla.",
      elemFormula: "Fórmula", elemFormulaHint: "La fórmula en bruto, estilo hoja de cálculo.",
      elemInputs: "Entradas", elemInputsHint: "Los campos a los que llama esta función de una línea.",
      elemConsumers: "Consumidores", elemConsumersHint: "Campos cuya función llama a esta.",
      provOn: "Procedencia · SÍ", provOff: "Procedencia · no",
      provOnTitle: "Ocultar los glifos ƒ de procedencia. El doble clic en las celdas sigue abriendo el DAG.",
      provOffTitle: "Mostrar los glifos ƒ de procedencia en las celdas con procedencia explorable.",
      downloadXlsx: "Descargar libro", downloadXlsxTitle: "Descargar el libro de reglas como libro de Excel (.xlsx)",
      gearShowAs: "Mostrar como", gearLanguage: "Idioma",
      navSearch: "Filtrar tablas y campos…",
      navDerivedOnly: "Solo derivados",
      navNoMatch: "Nada coincide.",
      groupOther: "Otras",
      tabAll: "Todos",
      tabDerived: "Derivados",
      tabStored: "Almacenados",
      tabRelationships: "Relaciones",
      viewGroups: "Grupos",
      viewList: "A–Z",
      viewGraph: "Grafo",
      tablesCount: (n) => `${n} tablas`,
      graphHint: "Cada recuadro es una tabla; una línea va de una tabla a la tabla a la que se refiere. Haz clic en una tabla para abrirla.",
      filterFields: "Filtrar campos…",
      cellHintOn: "Pasa el cursor sobre la insignia para ver un avance; doble clic para abrir la página DAG completa",
      cellHintOff: "Doble clic para ver la procedencia (activa los glifos ƒ en la cabecera)",
      ontologyTitle: "La ontología",
      ontologyLead: "Todas las tablas del libro de reglas. Haz clic en una tabla para ver sus columnas.",
      fieldsCount: (n) => `${n} campos`, derivedCount: (n) => `${n} derivados`, relCount: (n) => `${n} rel.`,
      tableNotFound: "Tabla no encontrada.", fieldNotFound: "Campo no encontrado en el libro de reglas.",
      nullable: "admite nulos",
      hopsFromGround: (n) => (n === 1 ? "a 1 salto de los datos base" : `a ${n} saltos de los datos base`),
      relationshipPointer: "Puntero de relación", groundTruth: "Datos base",
      relPointerBody: (link) => `Este campo es un puntero a ${link}. No se deriva de nada más: es el vínculo del que cuelga el resto del DAG.`,
      relPointerBodyNoLink: "Este campo es un puntero a otra tabla. No se deriva de nada más: es el vínculo del que cuelga el resto del DAG.",
      groundTruthBody: (dt) => `Esta celda es una hoja. Se <em>escribe</em> directamente en la base de datos (tipo de dato <code>${dt}</code>) y no se calcula a partir de nada más. Todo lo demás, aguas arriba, se remonta en última instancia a valores como este.`,
      openTable: (t) => `Abrir ${t} →`,
      ruleCardTitle: "Esta celda es el valor de una regla declarativa",
      oneLineFnCardTitle: "Esta celda es el valor de una función de una línea",
      inRuleSpeak: "En Documentación", inEnglish: "En lenguaje claro", asFormula: "Como fórmula",
      mechanicalFlag: "⚠︎ mecánico", mechanicalTitle: "Fiel pero con una redacción torpe — una señal para una revisión opcional",
      noRuleRendered: (t) => `No se generó ninguna regla declarativa para este campo ${t}.`,
      obligations: "Obligaciones",
      ruleSpeakHintFormula: "Es la misma lógica que codifica la fórmula, escrita como regla de negocio — generada a partir del libro de reglas por el motor compartido de Documentación.",
      ruleSpeakHintNoFormula: "Generada a partir del libro de reglas por el motor compartido de Documentación.",
      englishHint: "Cada campo subrayado es a su vez una función de una línea. Haz clic en cualquiera para profundizar.",
      noFormulaSentence: (t) => `Este ${t} no tiene una fórmula de hoja de cálculo que leer como oración: es un acumulado definido estructuralmente sobre una relación. Cambia a <strong>Documentación</strong> para ver cómo se define.`,
      formulaHint: "Cada chip es a su vez una función de una línea. Haz clic en cualquier chip para profundizar.",
      noFormulaAtAll: (t) => `Este ${t} no tiene fórmula de hoja de cálculo: es un acumulado definido estructuralmente sobre una relación. Cambia a <strong>Documentación</strong> para ver cómo se define.`,
      inputsTitle: "Entradas", inputsLead: "Campos a los que llama esta función de una línea.",
      groundTruthInline: (dt) => `Datos base · tipo de dato ${dt}`,
      consumersTitle: "Consumidores",
      consumersDirect: "directos", consumersTransitive: "transitivos",
      consumersEmpty: "Nada más en el libro de reglas hace referencia a esta celda todavía.",
      consumersLead: "Campos cuya función de una línea llama a esta.",
      showTransitive: (n) => `Mostrar ${n} consumidores transitivos (más abajo en la cadena)`,
      leavesTitle: "Datos base en las hojas",
      leavesLead: "En última instancia, el valor de esta celda depende de estos valores en bruto. Editar cualquiera de ellos y reconstruir se propaga aquí automáticamente.",
      hoverGroundTruth: (dt) => `Datos base — escritos directamente en la base de datos${dt ? ` (tipo de dato <code>${dt}</code>)` : ""}.`,
      hoverRelPointer: "Puntero de relación — el vínculo del que cuelga el resto del DAG.",
      hoverNoRule: "No se generó ninguna regla declarativa.",
      hoverNoFormula: "Sin fórmula — un acumulado estructural. Consulta Documentación.",
      hoverInputs: "Entradas",
      hoverHint: "Haz doble clic en la celda para abrir la página DAG completa.",
      hoverOpenPage: "Abrir la página completa",
      // The witness (witness.js): one row's value and the values it came from.
      witnessTitle: "Comprobado hasta los datos base",
      witnessLead: "El valor de esta celda para una fila y cada valor del que procede, hasta llegar a los almacenados. Cada uno se ha calculado aquí, ahora mismo, con el TypeScript generado de este proyecto (rulebook-to-typescript). Abre una línea para ver de qué se obtuvo.",
      witnessRow: "Fila",
      witnessForRow: (label) => `Para ${label}`,
      witnessComputing: "Calculando esta fila con el TypeScript generado…",
      witnessNoRows: (t) => `${t} no tiene filas que comprobar.`,
      witnessFailed: "No se pudo calcular la comprobación.",
      witnessGroundTruth: "datos base",
      witnessPointsTo: (t, label) => `apunta a ${t} ${label}`,
      witnessPointsNowhere: (t) => `no apunta a ninguna fila de ${t}`,
      witnessBlank: "en blanco",
      witnessEmptyText: "texto vacío",
      witnessError: (m) => `no se pudo calcular: ${m}`,
      witnessStep: (n, of) => `Se aplicó el paso ${n} de ${of}.`,
      witnessNoStep: "No se aplicó ningún paso, así que el valor queda en blanco.",
      // The noun's gender is not known, so it follows a noun whose gender is.
      witnessStepMark: (noun) => (noun ? `se aplicó a este registro (${noun})` : "se aplicó a esta fila"),
      witnessLookup: (key, value, t, label) => `Buscado mediante ${key} = ${value}: leído de ${t} ${label}.`,
      witnessLookupMiss: (key, value, t) => `Buscado mediante ${key} = ${value}, que no corresponde a ninguna fila de ${t}.`,
      witnessOver: (op, n, t) => {
        const rows = `${n} ${n === 1 ? "fila" : "filas"} de ${t} que ${n === 1 ? "coincide" : "coinciden"}`;
        return ({
          COUNTIFS: `Contado: ${n} ${n === 1 ? "fila" : "filas"} de ${t} ${n === 1 ? "coincide" : "coinciden"}.`,
          COUNT: `Contado: ${n} ${n === 1 ? "fila" : "filas"} de ${t} ${n === 1 ? "coincide" : "coinciden"}.`,
          SUM: `Sumado sobre ${rows}.`,
          AVERAGE: `Promedio sobre ${rows}.`,
          MIN: `El menor valor entre ${rows}.`,
          MAX: `El mayor valor entre ${rows}.`,
        }[op] || `Calculado sobre ${rows}.`);
      },
      witnessOverUnknown: "Calculado sobre filas que esta página no puede listar.",
      witnessMoreRows: (n) => `… y ${n} ${n === 1 ? "fila" : "filas"} más`,
      witnessSeenAbove: "ya se muestra arriba",
      type_raw: "Datos base", type_calculated: "Calculado", type_aggregation: "Agregación", type_relationship: "Relación", type_lookup: "Búsqueda",
    },
    fr: {
      back: "← retour", backTitle: "Revenir d’une étape",
      home: "🏠 Accueil", homeTitle: "Quitter l’explorateur — retour à l’application",
      tablesCrumb: "Tables",
      narrRulespeak: "Documentation", narrEnglish: "Langage clair", narrFormula: "Formule",
      narrRulespeakHint: "La règle métier déclarative (seulement si / doit / priorité)",
      narrEnglishHint: "La formule lue comme une phrase simple",
      narrFormulaHint: "La formule brute, façon tableur, avec des champs cliquables",
      narrAriaLabel: "Comment expliquer cette règle",
      gearTitle: "Paramètres — choisissez ce qui s’affiche dans la carte de survol et sur la page du champ",
      gearAriaLabel: "Paramètres",
      gearPopTitle: "Afficher dans la fenêtre et sur la page",
      gearReset: "Tout réactiver",
      elemDesc: "Description", elemDescHint: "La description du champ en langage courant.",
      elemRulespeak: "Documentation", elemRulespeakHint: "La règle métier déclarative.",
      elemEnglish: "Langage clair", elemEnglishHint: "La formule lue comme une phrase simple.",
      elemFormula: "Formule", elemFormulaHint: "La formule brute, façon tableur.",
      elemInputs: "Entrées", elemInputsHint: "Les champs appelés par cette fonction d’une ligne.",
      elemConsumers: "Consommateurs", elemConsumersHint: "Les champs dont la fonction appelle celle-ci.",
      provOn: "Provenance · ACTIVÉE", provOff: "Provenance · désactivée",
      provOnTitle: "Masquer les glyphes ƒ de provenance. Un double-clic sur les cellules ouvre toujours le DAG.",
      provOffTitle: "Afficher les glyphes ƒ de provenance sur les cellules dont la provenance est explorable.",
      downloadXlsx: "Télécharger le classeur", downloadXlsxTitle: "Télécharger le livre de règles au format classeur Excel (.xlsx)",
      gearShowAs: "Afficher en", gearLanguage: "Langue",
      navSearch: "Filtrer les tables et les champs…",
      navDerivedOnly: "Dérivés seulement",
      navNoMatch: "Aucun résultat.",
      groupOther: "Autres",
      tabAll: "Tous",
      tabDerived: "Dérivés",
      tabStored: "Stockés",
      tabRelationships: "Relations",
      viewGroups: "Groupes",
      viewList: "A–Z",
      viewGraph: "Graphe",
      tablesCount: (n) => `${n} ${n === 1 ? "table" : "tables"}`,
      graphHint: "Chaque case est une table ; un trait va d’une table vers la table à laquelle elle renvoie. Cliquez sur une table pour l’ouvrir.",
      filterFields: "Filtrer les champs…",
      cellHintOn: "Survolez le badge pour un aperçu ; double-cliquez pour ouvrir la page DAG complète",
      cellHintOff: "Double-cliquez pour voir la provenance (activez les glyphes ƒ dans l’en-tête)",
      ontologyTitle: "L’ontologie",
      ontologyLead: "Toutes les tables du livre de règles. Cliquez sur une table pour voir ses colonnes.",
      fieldsCount: (n) => `${n} champs`, derivedCount: (n) => `${n} dérivés`, relCount: (n) => `${n} rel.`,
      tableNotFound: "Table introuvable.", fieldNotFound: "Champ introuvable dans le livre de règles.",
      nullable: "NULL autorisé",
      hopsFromGround: (n) => (n === 1 ? "à 1 saut des données de base" : `à ${n} sauts des données de base`),
      relationshipPointer: "Pointeur de relation", groundTruth: "Données de base",
      relPointerBody: (link) => `Ce champ est un pointeur vers ${link}. Il n’est dérivé de rien d’autre — c’est le lien auquel se rattache le reste du DAG.`,
      relPointerBodyNoLink: "Ce champ est un pointeur vers une autre table. Il n’est dérivé de rien d’autre — c’est le lien auquel se rattache le reste du DAG.",
      groundTruthBody: (dt) => `Cette cellule est une feuille. Elle est <em>écrite</em> directement dans la base de données (type <code>${dt}</code>) et n’est calculée à partir de rien d’autre. Tout le reste, en amont, remonte finalement à des valeurs comme celle-ci.`,
      openTable: (t) => `Ouvrir ${t} →`,
      ruleCardTitle: "Cette cellule est la valeur d’une règle déclarative",
      oneLineFnCardTitle: "Cette cellule est la valeur d’une fonction d’une ligne",
      inRuleSpeak: "En Documentation", inEnglish: "En langage clair", asFormula: "Sous forme de formule",
      mechanicalFlag: "⚠︎ mécanique", mechanicalTitle: "Fidèle mais formulé maladroitement — un signal pour une reformulation facultative",
      noRuleRendered: (t) => `Aucune règle déclarative n’a été générée pour ce champ ${t}.`,
      obligations: "Obligations",
      ruleSpeakHintFormula: "C’est la même logique que celle de la formule, écrite comme une règle métier — générée à partir du livre de règles par le moteur Documentation partagé.",
      ruleSpeakHintNoFormula: "Générée à partir du livre de règles par le moteur Documentation partagé.",
      englishHint: "Chaque champ souligné est lui-même une fonction d’une ligne. Cliquez dessus pour approfondir.",
      noFormulaSentence: (t) => `Ce ${t} n’a pas de formule de tableur à lire comme une phrase — c’est un cumul défini structurellement sur une relation. Passez à <strong>Documentation</strong> pour voir sa définition.`,
      formulaHint: "Chaque puce est elle-même une fonction d’une ligne. Cliquez sur une puce pour approfondir.",
      noFormulaAtAll: (t) => `Ce ${t} n’a pas de formule de tableur — c’est un cumul défini structurellement sur une relation. Passez à <strong>Documentation</strong> pour voir sa définition.`,
      inputsTitle: "Entrées", inputsLead: "Champs appelés par cette fonction d’une ligne.",
      groundTruthInline: (dt) => `Données de base · type ${dt}`,
      consumersTitle: "Consommateurs",
      consumersDirect: "directs", consumersTransitive: "transitifs",
      consumersEmpty: "Rien d’autre dans le livre de règles ne fait encore référence à cette cellule.",
      consumersLead: "Champs dont la fonction d’une ligne appelle celle-ci.",
      showTransitive: (n) => `Afficher ${n} consommateurs transitifs (en aval de l’aval)`,
      leavesTitle: "Données de base aux feuilles",
      leavesLead: "En fin de compte, la valeur de cette cellule dépend de ces valeurs brutes. Modifier l’une d’elles puis reconstruire se propage ici automatiquement.",
      hoverGroundTruth: (dt) => `Données de base — écrites directement dans la base de données${dt ? ` (type <code>${dt}</code>)` : ""}.`,
      hoverRelPointer: "Pointeur de relation — le lien auquel se rattache le reste du DAG.",
      hoverNoRule: "Aucune règle déclarative générée.",
      hoverNoFormula: "Pas de formule — un cumul structurel. Voir Documentation.",
      hoverInputs: "Entrées",
      hoverHint: "Double-cliquez sur la cellule pour ouvrir la page DAG complète.",
      hoverOpenPage: "Ouvrir la page complète",
      // The witness (witness.js): one row's value and the values it came from.
      witnessTitle: "Vérifié jusqu’aux données de base",
      witnessLead: "La valeur de cette cellule pour une ligne, et chaque valeur dont elle provient, jusqu’aux valeurs enregistrées. Chacune vient d’être calculée ici par le TypeScript généré de ce projet (rulebook-to-typescript). Ouvrez une ligne pour voir de quoi elle est faite.",
      witnessRow: "Ligne",
      witnessForRow: (label) => `Pour ${label}`,
      witnessComputing: "Calcul de cette ligne avec le TypeScript généré…",
      witnessNoRows: (t) => `${t} ne contient aucune ligne à vérifier.`,
      witnessFailed: "La vérification n’a pas pu être calculée.",
      witnessGroundTruth: "données de base",
      witnessPointsTo: (t, label) => `pointe vers ${t} ${label}`,
      witnessPointsNowhere: (t) => `ne pointe vers aucune ligne de ${t}`,
      witnessBlank: "vide",
      witnessEmptyText: "texte vide",
      witnessError: (m) => `n’a pas pu être calculé : ${m}`,
      witnessStep: (n, of) => `Étape ${n} sur ${of} appliquée.`,
      witnessNoStep: "Aucune étape ne s’applique : la valeur est vide.",
      // The noun's gender is not known, so it follows a noun whose gender is.
      witnessStepMark: (noun) => (noun ? `s’applique à cet enregistrement (${noun})` : "s’applique à cette ligne"),
      witnessLookup: (key, value, t, label) => `Recherché via ${key} = ${value} : lu dans ${t} ${label}.`,
      witnessLookupMiss: (key, value, t) => `Recherché via ${key} = ${value}, qui ne correspond à aucune ligne de ${t}.`,
      witnessOver: (op, n, t) => {
        const rows = `${n} ${n < 2 ? "ligne" : "lignes"} de ${t} qui ${n < 2 ? "correspond" : "correspondent"}`;
        return ({
          COUNTIFS: `Compté : ${n} ${n < 2 ? "ligne" : "lignes"} de ${t} ${n < 2 ? "correspond" : "correspondent"}.`,
          COUNT: `Compté : ${n} ${n < 2 ? "ligne" : "lignes"} de ${t} ${n < 2 ? "correspond" : "correspondent"}.`,
          SUM: `Somme sur ${rows}.`,
          AVERAGE: `Moyenne sur ${rows}.`,
          MIN: `La plus petite valeur parmi ${rows}.`,
          MAX: `La plus grande valeur parmi ${rows}.`,
        }[op] || `Calculé sur ${rows}.`);
      },
      witnessOverUnknown: "Calculé sur des lignes que cette page ne peut pas lister.",
      witnessMoreRows: (n) => `… et ${n} ${n < 2 ? "autre ligne" : "autres lignes"}`,
      witnessSeenAbove: "déjà affiché plus haut",
      type_raw: "Données de base", type_calculated: "Calculé", type_aggregation: "Agrégation", type_relationship: "Relation", type_lookup: "Recherche",
    },
    pt: {
      back: "← voltar", backTitle: "Voltar um passo",
      home: "🏠 Início", homeTitle: "Sair do explicador — voltar ao aplicativo",
      tablesCrumb: "Tabelas",
      narrRulespeak: "Documentação", narrEnglish: "Linguagem clara", narrFormula: "Fórmula",
      narrRulespeakHint: "A regra de negócio declarativa (somente se / deve / prioridade)",
      narrEnglishHint: "A fórmula lida como uma frase simples",
      narrFormulaHint: "A fórmula bruta, estilo planilha, com campos clicáveis",
      narrAriaLabel: "Como explicar esta regra",
      gearTitle: "Configurações — escolha o que aparece no cartão flutuante e na página do campo",
      gearAriaLabel: "Configurações",
      gearPopTitle: "Mostrar no popup e na página",
      gearReset: "Reativar tudo",
      elemDesc: "Descrição", elemDescHint: "A descrição do campo em linguagem simples.",
      elemRulespeak: "Documentação", elemRulespeakHint: "A regra de negócio declarativa.",
      elemEnglish: "Linguagem clara", elemEnglishHint: "A fórmula lida como uma frase simples.",
      elemFormula: "Fórmula", elemFormulaHint: "A fórmula bruta, estilo planilha.",
      elemInputs: "Entradas", elemInputsHint: "Os campos que esta função de uma linha chama.",
      elemConsumers: "Consumidores", elemConsumersHint: "Campos cuja função chama esta.",
      provOn: "Proveniência · LIGADA", provOff: "Proveniência · desligada",
      provOnTitle: "Ocultar os glifos ƒ de proveniência. O clique duplo nas células continua abrindo o DAG.",
      provOffTitle: "Mostrar os glifos ƒ de proveniência nas células com proveniência explorável.",
      downloadXlsx: "Baixar planilha", downloadXlsxTitle: "Baixar o livro de regras como planilha do Excel (.xlsx)",
      gearShowAs: "Mostrar como", gearLanguage: "Idioma",
      navSearch: "Filtrar tabelas e campos…",
      navDerivedOnly: "Somente derivados",
      navNoMatch: "Nada corresponde.",
      groupOther: "Outras",
      tabAll: "Todos",
      tabDerived: "Derivados",
      tabStored: "Armazenados",
      tabRelationships: "Relações",
      viewGroups: "Grupos",
      viewList: "A–Z",
      viewGraph: "Grafo",
      tablesCount: (n) => `${n} tabelas`,
      graphHint: "Cada caixa é uma tabela; uma linha vai de uma tabela até a tabela a que ela se refere. Clique em uma tabela para abri-la.",
      filterFields: "Filtrar campos…",
      cellHintOn: "Passe o cursor sobre o selo para uma prévia; clique duas vezes para abrir a página DAG completa",
      cellHintOff: "Clique duas vezes para ver a proveniência (ative os glifos ƒ no cabeçalho)",
      ontologyTitle: "A ontologia",
      ontologyLead: "Todas as tabelas do livro de regras. Clique em uma tabela para ver suas colunas.",
      fieldsCount: (n) => `${n} campos`, derivedCount: (n) => `${n} derivados`, relCount: (n) => `${n} rel.`,
      tableNotFound: "Tabela não encontrada.", fieldNotFound: "Campo não encontrado no livro de regras.",
      nullable: "aceita nulo",
      hopsFromGround: (n) => (n === 1 ? "a 1 salto dos dados de base" : `a ${n} saltos dos dados de base`),
      relationshipPointer: "Ponteiro de relação", groundTruth: "Dados de base",
      relPointerBody: (link) => `Este campo é um ponteiro para ${link}. Não é derivado de nada mais — é o vínculo do qual o resto do DAG depende.`,
      relPointerBodyNoLink: "Este campo é um ponteiro para outra tabela. Não é derivado de nada mais — é o vínculo do qual o resto do DAG depende.",
      groundTruthBody: (dt) => `Esta célula é uma folha. Ela é <em>gravada</em> diretamente no banco de dados (tipo de dado <code>${dt}</code>) e não é calculada a partir de nada mais. Todo o resto, a montante, remonta afinal a valores como este.`,
      openTable: (t) => `Abrir ${t} →`,
      ruleCardTitle: "Esta célula é o valor de uma regra declarativa",
      oneLineFnCardTitle: "Esta célula é o valor de uma função de uma linha",
      inRuleSpeak: "Em Documentação", inEnglish: "Em linguagem clara", asFormula: "Como fórmula",
      mechanicalFlag: "⚠︎ mecânico", mechanicalTitle: "Fiel, mas com redação truncada — um sinal para uma revisão opcional",
      noRuleRendered: (t) => `Nenhuma regra declarativa foi gerada para este campo ${t}.`,
      obligations: "Obrigações",
      ruleSpeakHintFormula: "É a mesma lógica que a fórmula codifica, escrita como regra de negócio — gerada a partir do livro de regras pelo mecanismo compartilhado do Documentação.",
      ruleSpeakHintNoFormula: "Gerada a partir do livro de regras pelo mecanismo compartilhado do Documentação.",
      englishHint: "Cada campo sublinhado é, ele próprio, uma função de uma linha. Clique em qualquer um para detalhar.",
      noFormulaSentence: (t) => `Este ${t} não tem fórmula de planilha para ler como frase — é um agregado definido estruturalmente sobre uma relação. Mude para <strong>Documentação</strong> para ver como é definido.`,
      formulaHint: "Cada chip é, ele próprio, uma função de uma linha. Clique em qualquer chip para detalhar.",
      noFormulaAtAll: (t) => `Este ${t} não tem fórmula de planilha — é um agregado definido estruturalmente sobre uma relação. Mude para <strong>Documentação</strong> para ver como é definido.`,
      inputsTitle: "Entradas", inputsLead: "Campos que esta função de uma linha chama.",
      groundTruthInline: (dt) => `Dados de base · tipo de dado ${dt}`,
      consumersTitle: "Consumidores",
      consumersDirect: "diretos", consumersTransitive: "transitivos",
      consumersEmpty: "Nada mais no livro de regras faz referência a esta célula ainda.",
      consumersLead: "Campos cuja função de uma linha chama esta.",
      showTransitive: (n) => `Mostrar ${n} consumidores transitivos (mais abaixo na cadeia)`,
      leavesTitle: "Dados de base nas folhas",
      leavesLead: "Em última instância, o valor desta célula depende destes valores brutos. Editar qualquer um deles e reconstruir se propaga aqui automaticamente.",
      hoverGroundTruth: (dt) => `Dados de base — gravados diretamente no banco de dados${dt ? ` (tipo de dado <code>${dt}</code>)` : ""}.`,
      hoverRelPointer: "Ponteiro de relação — o vínculo do qual o resto do DAG depende.",
      hoverNoRule: "Nenhuma regra declarativa gerada.",
      hoverNoFormula: "Sem fórmula — um agregado estrutural. Veja Documentação.",
      hoverInputs: "Entradas",
      hoverHint: "Clique duas vezes na célula para abrir a página DAG completa.",
      hoverOpenPage: "Abrir a página completa",
      // The witness (witness.js): one row's value and the values it came from.
      witnessTitle: "Comprovado até os dados de base",
      witnessLead: "O valor desta célula para uma linha e cada valor de que ele veio, até os valores armazenados. Cada um foi calculado aqui, agora mesmo, pelo TypeScript gerado deste projeto (rulebook-to-typescript). Abra uma linha para ver de que ela foi feita.",
      witnessRow: "Linha",
      witnessForRow: (label) => `Para ${label}`,
      witnessComputing: "Calculando esta linha com o TypeScript gerado…",
      witnessNoRows: (t) => `${t} não tem linhas para comprovar.`,
      witnessFailed: "Não foi possível calcular a comprovação.",
      witnessGroundTruth: "dados de base",
      witnessPointsTo: (t, label) => `aponta para ${t} ${label}`,
      witnessPointsNowhere: (t) => `não aponta para nenhuma linha de ${t}`,
      witnessBlank: "em branco",
      witnessEmptyText: "texto vazio",
      witnessError: (m) => `não pôde ser calculado: ${m}`,
      witnessStep: (n, of) => `Etapa ${n} de ${of} aplicada.`,
      witnessNoStep: "Nenhuma etapa se aplicou, por isso o valor fica em branco.",
      // The noun's gender is not known, so it follows a noun whose gender is.
      witnessStepMark: (noun) => (noun ? `aplicada a este registro (${noun})` : "aplicada a esta linha"),
      witnessLookup: (key, value, t, label) => `Procurado por ${key} = ${value}: lido de ${t} ${label}.`,
      witnessLookupMiss: (key, value, t) => `Procurado por ${key} = ${value}, que não corresponde a nenhuma linha de ${t}.`,
      witnessOver: (op, n, t) => {
        const rows = `${n} ${n === 1 ? "linha" : "linhas"} de ${t} que ${n === 1 ? "corresponde" : "correspondem"}`;
        return ({
          COUNTIFS: `Contado: ${n} ${n === 1 ? "linha" : "linhas"} de ${t} ${n === 1 ? "corresponde" : "correspondem"}.`,
          COUNT: `Contado: ${n} ${n === 1 ? "linha" : "linhas"} de ${t} ${n === 1 ? "corresponde" : "correspondem"}.`,
          SUM: `Somado sobre ${rows}.`,
          AVERAGE: `Média sobre ${rows}.`,
          MIN: `O menor valor entre ${rows}.`,
          MAX: `O maior valor entre ${rows}.`,
        }[op] || `Calculado sobre ${rows}.`);
      },
      witnessOverUnknown: "Calculado sobre linhas que esta página não consegue listar.",
      witnessMoreRows: (n) => `… e mais ${n} ${n === 1 ? "linha" : "linhas"}`,
      witnessSeenAbove: "já mostrado acima",
      type_raw: "Dados de base", type_calculated: "Calculado", type_aggregation: "Agregação", type_relationship: "Relação", type_lookup: "Consulta",
    },
    it: {
      back: "← indietro", backTitle: "Torna indietro di un passo",
      home: "🏠 Home", homeTitle: "Esci dall’explainer — torna all’app",
      tablesCrumb: "Tabelle",
      narrRulespeak: "Documentazione", narrEnglish: "Linguaggio chiaro", narrFormula: "Formula",
      narrRulespeakHint: "La regola di business dichiarativa (solo se / deve / priorità)",
      narrEnglishHint: "La formula letta come una semplice frase",
      narrFormulaHint: "La formula grezza, in stile foglio di calcolo, con campi cliccabili",
      narrAriaLabel: "Come spiegare questa regola",
      gearTitle: "Impostazioni — scegli cosa mostrare nella scheda al passaggio del mouse e nella pagina del campo",
      gearAriaLabel: "Impostazioni",
      gearPopTitle: "Mostra nel popup e nella pagina",
      gearReset: "Riattiva tutto",
      elemDesc: "Descrizione", elemDescHint: "La descrizione del campo in linguaggio semplice.",
      elemRulespeak: "Documentazione", elemRulespeakHint: "La regola di business dichiarativa.",
      elemEnglish: "Linguaggio chiaro", elemEnglishHint: "La formula letta come una semplice frase.",
      elemFormula: "Formula", elemFormulaHint: "La formula grezza, in stile foglio di calcolo.",
      elemInputs: "Input", elemInputsHint: "I campi richiamati da questa funzione di una riga.",
      elemConsumers: "Utilizzatori", elemConsumersHint: "Campi la cui funzione richiama questa.",
      provOn: "Provenienza · ON", provOff: "Provenienza · off",
      provOnTitle: "Nascondi i glifi ƒ di provenienza. Il doppio clic sulle celle apre comunque il DAG.",
      provOffTitle: "Mostra i glifi ƒ di provenienza sulle celle con provenienza esplorabile.",
      downloadXlsx: "Scarica la cartella", downloadXlsxTitle: "Scarica il libro delle regole come cartella di lavoro Excel (.xlsx)",
      gearShowAs: "Mostra come", gearLanguage: "Lingua",
      navSearch: "Filtra tabelle e campi…",
      navDerivedOnly: "Solo derivati",
      navNoMatch: "Nessun risultato.",
      groupOther: "Altre",
      tabAll: "Tutti",
      tabDerived: "Derivati",
      tabStored: "Memorizzati",
      tabRelationships: "Relazioni",
      viewGroups: "Gruppi",
      viewList: "A–Z",
      viewGraph: "Grafo",
      tablesCount: (n) => `${n} tabelle`,
      graphHint: "Ogni riquadro è una tabella; una linea va da una tabella alla tabella a cui fa riferimento. Fai clic su una tabella per aprirla.",
      filterFields: "Filtra i campi…",
      cellHintOn: "Passa sopra il badge per un’anteprima; doppio clic per aprire la pagina DAG completa",
      cellHintOff: "Doppio clic per vedere la provenienza (attiva i glifi ƒ nell’intestazione)",
      ontologyTitle: "L’ontologia",
      ontologyLead: "Ogni tabella del libro delle regole. Fai clic su una tabella per vederne le colonne.",
      fieldsCount: (n) => `${n} campi`, derivedCount: (n) => `${n} derivati`, relCount: (n) => `${n} rel.`,
      tableNotFound: "Tabella non trovata.", fieldNotFound: "Campo non trovato nel libro delle regole.",
      nullable: "ammette null",
      hopsFromGround: (n) => (n === 1 ? "a 1 passaggio dai dati di base" : `a ${n} passaggi dai dati di base`),
      relationshipPointer: "Puntatore di relazione", groundTruth: "Dati di base",
      relPointerBody: (link) => `Questo campo è un puntatore a ${link}. Non deriva da nient’altro — è il collegamento a cui è appeso il resto del DAG.`,
      relPointerBodyNoLink: "Questo campo è un puntatore a un’altra tabella. Non deriva da nient’altro — è il collegamento a cui è appeso il resto del DAG.",
      groundTruthBody: (dt) => `Questa cella è una foglia. Viene <em>scritta</em> direttamente nel database (tipo di dato <code>${dt}</code>) e non è calcolata da nient’altro. Tutto il resto, a monte, risale in definitiva a valori come questo.`,
      openTable: (t) => `Apri ${t} →`,
      ruleCardTitle: "Questa cella è il valore di una regola dichiarativa",
      oneLineFnCardTitle: "Questa cella è il valore di una funzione di una riga",
      inRuleSpeak: "In Documentazione", inEnglish: "In linguaggio chiaro", asFormula: "Come formula",
      mechanicalFlag: "⚠︎ meccanico", mechanicalTitle: "Fedele ma formulato in modo goffo — un segnale per una revisione facoltativa",
      noRuleRendered: (t) => `Non è stata generata alcuna regola dichiarativa per questo campo ${t}.`,
      obligations: "Obblighi",
      ruleSpeakHintFormula: "È la stessa logica codificata dalla formula, scritta come regola di business — generata dal libro delle regole dal motore Documentazione condiviso.",
      ruleSpeakHintNoFormula: "Generata dal libro delle regole dal motore Documentazione condiviso.",
      englishHint: "Ogni campo sottolineato è a sua volta una funzione di una riga. Fai clic su uno qualsiasi per approfondire.",
      noFormulaSentence: (t) => `Questo ${t} non ha una formula da foglio di calcolo da leggere come frase — è un aggregato definito strutturalmente su una relazione. Passa a <strong>Documentazione</strong> per vedere come è definito.`,
      formulaHint: "Ogni chip è a sua volta una funzione di una riga. Fai clic su un chip per approfondire.",
      noFormulaAtAll: (t) => `Questo ${t} non ha una formula da foglio di calcolo — è un aggregato definito strutturalmente su una relazione. Passa a <strong>Documentazione</strong> per vedere come è definito.`,
      inputsTitle: "Input", inputsLead: "Campi richiamati da questa funzione di una riga.",
      groundTruthInline: (dt) => `Dati di base · tipo di dato ${dt}`,
      consumersTitle: "Utilizzatori",
      consumersDirect: "diretti", consumersTransitive: "transitivi",
      consumersEmpty: "Nient’altro nel libro delle regole fa ancora riferimento a questa cella.",
      consumersLead: "Campi la cui funzione di una riga richiama questa.",
      showTransitive: (n) => `Mostra ${n} utilizzatori transitivi (più a valle)`,
      leavesTitle: "Dati di base alle foglie",
      leavesLead: "In definitiva, il valore di questa cella dipende da questi valori grezzi. Modificarne uno e ricompilare si propaga qui automaticamente.",
      hoverGroundTruth: (dt) => `Dati di base — scritti direttamente nel database${dt ? ` (tipo di dato <code>${dt}</code>)` : ""}.`,
      hoverRelPointer: "Puntatore di relazione — il collegamento a cui è appeso il resto del DAG.",
      hoverNoRule: "Nessuna regola dichiarativa generata.",
      hoverNoFormula: "Nessuna formula — un aggregato strutturale. Vedi Documentazione.",
      hoverInputs: "Input",
      hoverHint: "Fai doppio clic sulla cella per aprire la pagina DAG completa.",
      hoverOpenPage: "Apri la pagina completa",
      // The witness (witness.js): one row's value and the values it came from.
      witnessTitle: "Verificato fino ai dati di base",
      witnessLead: "Il valore di questa cella per una riga e ogni valore da cui deriva, fino a quelli memorizzati. Ciascuno è stato calcolato qui, in questo momento, dal TypeScript generato di questo progetto (rulebook-to-typescript). Apri una riga per vedere da che cosa è stata ottenuta.",
      witnessRow: "Riga",
      witnessForRow: (label) => `Per ${label}`,
      witnessComputing: "Calcolo di questa riga con il TypeScript generato…",
      witnessNoRows: (t) => `${t} non ha righe da verificare.`,
      witnessFailed: "Non è stato possibile calcolare la verifica.",
      witnessGroundTruth: "dati di base",
      witnessPointsTo: (t, label) => `punta a ${t} ${label}`,
      witnessPointsNowhere: (t) => `non punta a nessuna riga di ${t}`,
      witnessBlank: "vuoto",
      witnessEmptyText: "testo vuoto",
      witnessError: (m) => `non è stato possibile calcolarlo: ${m}`,
      witnessStep: (n, of) => `Applicato il passo ${n} di ${of}.`,
      witnessNoStep: "Nessun passo è stato applicato, quindi il valore è vuoto.",
      // The noun's gender is not known, so it follows a noun whose gender is.
      witnessStepMark: (noun) => (noun ? `applicato a questo record (${noun})` : "applicato a questa riga"),
      witnessLookup: (key, value, t, label) => `Cercato tramite ${key} = ${value}: letto da ${t} ${label}.`,
      witnessLookupMiss: (key, value, t) => `Cercato tramite ${key} = ${value}, che non corrisponde a nessuna riga di ${t}.`,
      witnessOver: (op, n, t) => {
        const rows = `${n} ${n === 1 ? "riga" : "righe"} di ${t} che ${n === 1 ? "corrisponde" : "corrispondono"}`;
        return ({
          COUNTIFS: `Contato: ${n} ${n === 1 ? "riga" : "righe"} di ${t} ${n === 1 ? "corrisponde" : "corrispondono"}.`,
          COUNT: `Contato: ${n} ${n === 1 ? "riga" : "righe"} di ${t} ${n === 1 ? "corrisponde" : "corrispondono"}.`,
          SUM: `Sommato su ${rows}.`,
          AVERAGE: `Media su ${rows}.`,
          MIN: `Il valore più piccolo tra ${rows}.`,
          MAX: `Il valore più grande tra ${rows}.`,
        }[op] || `Calcolato su ${rows}.`);
      },
      witnessOverUnknown: "Calcolato su righe che questa pagina non può elencare.",
      witnessMoreRows: (n) => `… e ${n === 1 ? "un’altra riga" : `altre ${n} righe`}`,
      witnessSeenAbove: "già mostrato sopra",
      type_raw: "Dati di base", type_calculated: "Calcolato", type_aggregation: "Aggregazione", type_relationship: "Relazione", type_lookup: "Ricerca",
    },
    nl: {
      back: "← terug", backTitle: "Eén stap terug",
      home: "🏠 Start", homeTitle: "De explainer verlaten — terug naar de app",
      tablesCrumb: "Tabellen",
      narrRulespeak: "Documentatie", narrEnglish: "Klare taal", narrFormula: "Formule",
      narrRulespeakHint: "De declaratieve bedrijfsregel (alleen als / moet / prioriteit)",
      narrEnglishHint: "De formule gelezen als een eenvoudige zin",
      narrFormulaHint: "De ruwe spreadsheetformule, met aanklikbare velden",
      narrAriaLabel: "Hoe deze regel uit te leggen",
      gearTitle: "Instellingen — kies wat er in de hoverkaart en op de veldpagina verschijnt",
      gearAriaLabel: "Instellingen",
      gearPopTitle: "Tonen in popup en op pagina",
      gearReset: "Alles weer aanzetten",
      elemDesc: "Beschrijving", elemDescHint: "De beschrijving van het veld in gewone taal.",
      elemRulespeak: "Documentatie", elemRulespeakHint: "De declaratieve bedrijfsregel.",
      elemEnglish: "Klare taal", elemEnglishHint: "De formule gelezen als een eenvoudige zin.",
      elemFormula: "Formule", elemFormulaHint: "De ruwe spreadsheetformule.",
      elemInputs: "Invoer", elemInputsHint: "De velden die deze eenregelige functie aanroept.",
      elemConsumers: "Afnemers", elemConsumersHint: "Velden waarvan de functie deze aanroept.",
      provOn: "Herkomst · AAN", provOff: "Herkomst · uit",
      provOnTitle: "ƒ-herkomstsymbolen verbergen. Dubbelklikken op cellen opent de DAG nog steeds.",
      provOffTitle: "ƒ-herkomstsymbolen tonen op cellen met verkenbare herkomst.",
      downloadXlsx: "Werkmap downloaden", downloadXlsxTitle: "Het regelboek downloaden als Excel-werkmap (.xlsx)",
      gearShowAs: "Weergeven als", gearLanguage: "Taal",
      navSearch: "Tabellen en velden filteren…",
      navDerivedOnly: "Alleen afgeleide",
      navNoMatch: "Niets gevonden.",
      groupOther: "Overig",
      tabAll: "Alle",
      tabDerived: "Afgeleid",
      tabStored: "Opgeslagen",
      tabRelationships: "Relaties",
      viewGroups: "Groepen",
      viewList: "A–Z",
      viewGraph: "Graaf",
      tablesCount: (n) => `${n} tabellen`,
      graphHint: "Elk vak is een tabel; een lijn loopt van een tabel naar de tabel waarnaar hij verwijst. Klik op een tabel om hem te openen.",
      filterFields: "Velden filteren…",
      cellHintOn: "Beweeg over het symbool voor een voorbeeld; dubbelklik om de volledige DAG-pagina te openen",
      cellHintOff: "Dubbelklik om de herkomst te zien (ƒ-symbolen aan/uit in de kopbalk)",
      ontologyTitle: "De ontologie",
      ontologyLead: "Elke tabel in het regelboek. Klik op een tabel om de kolommen te zien.",
      fieldsCount: (n) => `${n} velden`, derivedCount: (n) => `${n} afgeleid`, relCount: (n) => `${n} rel.`,
      tableNotFound: "Tabel niet gevonden.", fieldNotFound: "Veld niet gevonden in het regelboek.",
      nullable: "NULL toegestaan",
      hopsFromGround: (n) => (n === 1 ? "1 stap van de basisgegevens" : `${n} stappen van de basisgegevens`),
      relationshipPointer: "Relatieverwijzing", groundTruth: "Basisgegevens",
      relPointerBody: (link) => `Dit veld is een verwijzing naar ${link}. Het is nergens anders van afgeleid — het is de koppeling waaraan de rest van de DAG hangt.`,
      relPointerBodyNoLink: "Dit veld is een verwijzing naar een andere tabel. Het is nergens anders van afgeleid — het is de koppeling waaraan de rest van de DAG hangt.",
      groundTruthBody: (dt) => `Deze cel is een blad. Ze wordt rechtstreeks naar de database <em>geschreven</em> (gegevenstype <code>${dt}</code>) en wordt nergens anders uit berekend. Al het andere stroomopwaarts gaat uiteindelijk terug op waarden zoals deze.`,
      openTable: (t) => `${t} openen →`,
      ruleCardTitle: "Deze cel is de waarde van één declaratieve regel",
      oneLineFnCardTitle: "Deze cel is de waarde van één eenregelige functie",
      inRuleSpeak: "In Documentatie", inEnglish: "In klare taal", asFormula: "Als formule",
      mechanicalFlag: "⚠︎ mechanisch", mechanicalTitle: "Correct maar stroef geformuleerd — een signaal voor een optionele herformulering",
      noRuleRendered: (t) => `Voor dit ${t}-veld is geen declaratieve regel gegenereerd.`,
      obligations: "Verplichtingen",
      ruleSpeakHintFormula: "Dit is dezelfde logica als in de formule, geschreven als bedrijfsregel — door de gedeelde Documentatie-engine uit het regelboek gegenereerd.",
      ruleSpeakHintNoFormula: "Door de gedeelde Documentatie-engine uit het regelboek gegenereerd.",
      englishHint: "Elk onderstreept veld is zelf een eenregelige functie. Klik erop om verder in te zoomen.",
      noFormulaSentence: (t) => `Deze ${t} heeft geen spreadsheetformule om als zin te lezen — het is een aggregatie die structureel over een relatie is gedefinieerd. Schakel over naar <strong>Documentatie</strong> om te zien hoe die is gedefinieerd.`,
      formulaHint: "Elke chip is zelf een eenregelige functie. Klik op een chip om verder in te zoomen.",
      noFormulaAtAll: (t) => `Deze ${t} heeft geen spreadsheetformule — het is een aggregatie die structureel over een relatie is gedefinieerd. Schakel over naar <strong>Documentatie</strong> om te zien hoe die is gedefinieerd.`,
      inputsTitle: "Invoer", inputsLead: "Velden die deze eenregelige functie aanroept.",
      groundTruthInline: (dt) => `Basisgegevens · gegevenstype ${dt}`,
      consumersTitle: "Afnemers",
      consumersDirect: "direct", consumersTransitive: "transitief",
      consumersEmpty: "Nog niets anders in het regelboek verwijst naar deze cel.",
      consumersLead: "Velden waarvan de eenregelige functie deze aanroept.",
      showTransitive: (n) => `${n} transitieve afnemers tonen (verder stroomafwaarts)`,
      leavesTitle: "Basisgegevens aan de bladeren",
      leavesLead: "Uiteindelijk hangt de waarde van deze cel af van deze ruwe waarden. Een ervan bewerken en opnieuw bouwen werkt hier automatisch door.",
      hoverGroundTruth: (dt) => `Basisgegevens — rechtstreeks naar de database geschreven${dt ? ` (gegevenstype <code>${dt}</code>)` : ""}.`,
      hoverRelPointer: "Relatieverwijzing — de koppeling waaraan de rest van de DAG hangt.",
      hoverNoRule: "Geen declaratieve regel gegenereerd.",
      hoverNoFormula: "Geen formule — een structurele aggregatie. Zie Documentatie.",
      hoverInputs: "Invoer",
      hoverHint: "Dubbelklik op de cel om de volledige DAG-pagina te openen.",
      hoverOpenPage: "Volledige pagina openen",
      // The witness (witness.js): one row's value and the values it came from.
      witnessTitle: "Nagerekend tot aan de basisgegevens",
      witnessLead: "De waarde van deze cel voor één rij, en elke waarde waaruit die is voortgekomen, tot aan de opgeslagen waarden. Elke waarde is zojuist hier berekend door de gegenereerde TypeScript van dit project (rulebook-to-typescript). Open een regel om te zien waaruit die is opgebouwd.",
      witnessRow: "Rij",
      witnessForRow: (label) => `Voor ${label}`,
      witnessComputing: "Deze rij wordt berekend met de gegenereerde TypeScript…",
      witnessNoRows: (t) => `${t} heeft geen rijen om na te rekenen.`,
      witnessFailed: "De narekening kon niet worden berekend.",
      witnessGroundTruth: "basisgegevens",
      witnessPointsTo: (t, label) => `verwijst naar ${t} ${label}`,
      witnessPointsNowhere: (t) => `verwijst naar geen enkele rij in ${t}`,
      witnessBlank: "leeg",
      witnessEmptyText: "lege tekst",
      witnessError: (m) => `kon niet worden berekend: ${m}`,
      witnessStep: (n, of) => `Stap ${n} van ${of} is toegepast.`,
      witnessNoStep: "Er is geen stap toegepast, dus de waarde is leeg.",
      // The noun's gender (de / het) is not known, so it follows a noun whose gender is.
      witnessStepMark: (noun) => (noun ? `toegepast op dit record (${noun})` : "toegepast op deze rij"),
      witnessLookup: (key, value, t, label) => `Opgezocht via ${key} = ${value}: gelezen uit ${t} ${label}.`,
      witnessLookupMiss: (key, value, t) => `Opgezocht via ${key} = ${value}, waar geen rij in ${t} bij hoort.`,
      witnessOver: (op, n, t) => {
        const rows = `${n} ${n === 1 ? "rij" : "rijen"} in ${t} die ${n === 1 ? "overeenkomt" : "overeenkomen"}`;
        return ({
          COUNTIFS: `Geteld: ${n} ${n === 1 ? "rij" : "rijen"} in ${t} ${n === 1 ? "komt" : "komen"} overeen.`,
          COUNT: `Geteld: ${n} ${n === 1 ? "rij" : "rijen"} in ${t} ${n === 1 ? "komt" : "komen"} overeen.`,
          SUM: `Opgeteld over de ${rows}.`,
          AVERAGE: `Gemiddelde over de ${rows}.`,
          MIN: `De kleinste waarde over de ${rows}.`,
          MAX: `De grootste waarde over de ${rows}.`,
        }[op] || `Berekend over de ${rows}.`);
      },
      witnessOverUnknown: "Berekend over rijen die deze pagina niet kan weergeven.",
      witnessMoreRows: (n) => `… en nog ${n} ${n === 1 ? "rij" : "rijen"}`,
      witnessSeenAbove: "hierboven al getoond",
      type_raw: "Basisgegevens", type_calculated: "Berekend", type_aggregation: "Aggregatie", type_relationship: "Relatie", type_lookup: "Opzoeking",
    },
    ru: {
      back: "← назад", backTitle: "Вернуться на шаг назад",
      home: "🏠 Главная", homeTitle: "Выйти из обозревателя — вернуться в приложение",
      tablesCrumb: "Таблицы",
      narrRulespeak: "Документация", narrEnglish: "Простыми словами", narrFormula: "Формула",
      narrRulespeakHint: "Декларативное бизнес-правило (только если / должен / приоритет)",
      narrEnglishHint: "Формула, прочитанная как простое предложение",
      narrFormulaHint: "Исходная формула в стиле электронной таблицы, с кликабельными полями",
      narrAriaLabel: "Как объяснить это правило",
      gearTitle: "Настройки — выберите, что показывать во всплывающей карточке и на странице поля",
      gearAriaLabel: "Настройки",
      gearPopTitle: "Показывать во всплывающем окне и на странице",
      gearReset: "Снова включить всё",
      elemDesc: "Описание", elemDescHint: "Описание поля простым языком.",
      elemRulespeak: "Документация", elemRulespeakHint: "Декларативное бизнес-правило.",
      elemEnglish: "Простыми словами", elemEnglishHint: "Формула, прочитанная как простое предложение.",
      elemFormula: "Формула", elemFormulaHint: "Исходная формула в стиле электронной таблицы.",
      elemInputs: "Входы", elemInputsHint: "Поля, которые вызывает эта однострочная функция.",
      elemConsumers: "Потребители", elemConsumersHint: "Поля, функция которых вызывает эту.",
      provOn: "Происхождение · ВКЛ", provOff: "Происхождение · выкл",
      provOnTitle: "Скрыть значки происхождения ƒ. Двойной щелчок по ячейкам по-прежнему открывает DAG.",
      provOffTitle: "Показывать значки происхождения ƒ на ячейках, происхождение которых можно изучить.",
      downloadXlsx: "Скачать книгу", downloadXlsxTitle: "Скачать свод правил как книгу Excel (.xlsx)",
      gearShowAs: "Показывать как", gearLanguage: "Язык",
      navSearch: "Фильтр таблиц и полей…",
      navDerivedOnly: "Только производные",
      navNoMatch: "Ничего не найдено.",
      groupOther: "Прочее",
      tabAll: "Все",
      tabDerived: "Производные",
      tabStored: "Хранимые",
      tabRelationships: "Связи",
      viewGroups: "Группы",
      viewList: "A–Z",
      viewGraph: "Граф",
      tablesCount: (n) => `${n} ${slavicCount(n, "таблица", "таблицы", "таблиц")}`,
      graphHint: "Каждый блок — таблица; линия идёт от таблицы к той таблице, на которую она ссылается. Нажмите на таблицу, чтобы открыть её.",
      filterFields: "Фильтр полей…",
      cellHintOn: "Наведите на значок для предпросмотра; двойной щелчок открывает полную страницу DAG",
      cellHintOff: "Двойной щелчок — посмотреть происхождение (значки ƒ включаются в заголовке)",
      ontologyTitle: "Онтология",
      ontologyLead: "Все таблицы свода правил. Щёлкните таблицу, чтобы увидеть её столбцы.",
      fieldsCount: (n) => `полей: ${n}`, derivedCount: (n) => `производных: ${n}`, relCount: (n) => `связей: ${n}`,
      tableNotFound: "Таблица не найдена.", fieldNotFound: "Поле не найдено в своде правил.",
      nullable: "допускает NULL",
      hopsFromGround: (n) => `шагов от исходных данных: ${n}`,
      relationshipPointer: "Указатель связи", groundTruth: "Исходные данные",
      relPointerBody: (link) => `Это поле — указатель на ${link}. Оно ни из чего не выводится — это связь, на которой держится остальной DAG.`,
      relPointerBodyNoLink: "Это поле — указатель на другую таблицу. Оно ни из чего не выводится — это связь, на которой держится остальной DAG.",
      groundTruthBody: (dt) => `Эта ячейка — лист. Она <em>записывается</em> прямо в базу данных (тип данных <code>${dt}</code>) и ни из чего не вычисляется. Всё остальное выше по цепочке в итоге сводится к таким значениям.`,
      openTable: (t) => `Открыть ${t} →`,
      ruleCardTitle: "Эта ячейка — значение одного декларативного правила",
      oneLineFnCardTitle: "Эта ячейка — значение одной однострочной функции",
      inRuleSpeak: "В Документация", inEnglish: "Простыми словами", asFormula: "В виде формулы",
      mechanicalFlag: "⚠︎ механически", mechanicalTitle: "Точно, но неуклюже сформулировано — отметка для необязательной правки",
      noRuleRendered: (t) => `Для этого поля типа ${t} декларативное правило не создано.`,
      obligations: "Обязательства",
      ruleSpeakHintFormula: "Это та же логика, что заложена в формуле, записанная как бизнес-правило — создано из свода правил общим движком Документация.",
      ruleSpeakHintNoFormula: "Создано из свода правил общим движком Документация.",
      englishHint: "Каждое подчёркнутое поле само является однострочной функцией. Щёлкните любое, чтобы перейти к нему.",
      noFormulaSentence: (t) => `У этого поля типа ${t} нет формулы, которую можно прочитать как предложение, — это свёртка, определённая структурно через связь. Переключитесь на <strong>Документация</strong>, чтобы увидеть её определение.`,
      formulaHint: "Каждый чип сам является однострочной функцией. Щёлкните любой чип, чтобы перейти к нему.",
      noFormulaAtAll: (t) => `У этого поля типа ${t} нет формулы — это свёртка, определённая структурно через связь. Переключитесь на <strong>Документация</strong>, чтобы увидеть её определение.`,
      inputsTitle: "Входы", inputsLead: "Поля, которые вызывает эта однострочная функция.",
      groundTruthInline: (dt) => `Исходные данные · тип данных ${dt}`,
      consumersTitle: "Потребители",
      consumersDirect: "прямых", consumersTransitive: "транзитивных",
      consumersEmpty: "Пока ничто в своде правил не ссылается на эту ячейку.",
      consumersLead: "Поля, однострочная функция которых вызывает эту.",
      showTransitive: (n) => `Показать транзитивных потребителей: ${n} (ниже по цепочке)`,
      leavesTitle: "Исходные данные в листьях",
      leavesLead: "В конечном счёте значение этой ячейки зависит от этих исходных значений. Если изменить любое из них и пересобрать, изменение автоматически дойдёт сюда.",
      hoverGroundTruth: (dt) => `Исходные данные — записываются прямо в базу данных${dt ? ` (тип данных <code>${dt}</code>)` : ""}.`,
      hoverRelPointer: "Указатель связи — связь, на которой держится остальной DAG.",
      hoverNoRule: "Декларативное правило не создано.",
      hoverNoFormula: "Нет формулы — структурная свёртка. См. Документация.",
      hoverInputs: "Входы",
      hoverHint: "Дважды щёлкните ячейку, чтобы открыть полную страницу DAG.",
      hoverOpenPage: "Открыть полную страницу",
      // The witness (witness.js): one row's value and the values it came from.
      witnessTitle: "Проверено до исходных данных",
      witnessLead: "Значение этой ячейки для одной строки и все значения, из которых оно получено, вплоть до сохранённых. Каждое только что вычислено здесь сгенерированным TypeScript этого проекта (rulebook-to-typescript). Раскройте строку, чтобы увидеть, из чего она получена.",
      witnessRow: "Строка",
      witnessForRow: (label) => `Для ${label}`,
      witnessComputing: "Строка вычисляется сгенерированным TypeScript…",
      witnessNoRows: (t) => `В ${t} нет строк для проверки.`,
      witnessFailed: "Проверку не удалось вычислить.",
      witnessGroundTruth: "исходные данные",
      witnessPointsTo: (t, label) => `указывает на ${t} ${label}`,
      witnessPointsNowhere: (t) => `не указывает ни на одну строку ${t}`,
      witnessBlank: "пусто",
      witnessEmptyText: "пустой текст",
      witnessError: (m) => `не удалось вычислить: ${m}`,
      witnessStep: (n, of) => `Применён шаг ${n} из ${of}.`,
      witnessNoStep: "Ни один шаг не применён, поэтому значение пустое.",
      // The noun's gender and case are not known, so it follows a noun whose are.
      witnessStepMark: (noun) => (noun ? `применено к этой записи (${noun})` : "применено к этой строке"),
      witnessLookup: (key, value, t, label) => `Найдено по ${key} = ${value}: прочитано из ${t} ${label}.`,
      witnessLookupMiss: (key, value, t) => `Поиск по ${key} = ${value}: такой строки в ${t} нет.`,
      // The count stands after the noun ("строкам … (всего 3)"), so the noun's case never depends on it.
      witnessOver: (op, n, t) => ({
        COUNTIFS: `Подсчитано: ${n} ${slavicCount(n, "строка", "строки", "строк")} ${t} ${n % 10 === 1 && n % 100 !== 11 ? "подходит" : "подходят"}.`,
        COUNT: `Подсчитано: ${n} ${slavicCount(n, "строка", "строки", "строк")} ${t} ${n % 10 === 1 && n % 100 !== 11 ? "подходит" : "подходят"}.`,
        SUM: `Сумма по подходящим строкам ${t} (всего ${n}).`,
        AVERAGE: `Среднее по подходящим строкам ${t} (всего ${n}).`,
        MIN: `Наименьшее значение среди подходящих строк ${t} (всего ${n}).`,
        MAX: `Наибольшее значение среди подходящих строк ${t} (всего ${n}).`,
      }[op] || `Вычислено по подходящим строкам ${t} (всего ${n}).`),
      witnessOverUnknown: "Вычислено по строкам, которые эта страница не может перечислить.",
      witnessMoreRows: (n) => `… и ещё ${n} ${slavicCount(n, "строка", "строки", "строк")}`,
      witnessSeenAbove: "уже показано выше",
      type_raw: "Исходные данные", type_calculated: "Вычисляемое", type_aggregation: "Агрегат", type_relationship: "Связь", type_lookup: "Подстановка",
    },
    zh: {
      back: "← 返回", backTitle: "后退一步",
      home: "🏠 首页", homeTitle: "离开解释器 — 返回应用",
      tablesCrumb: "表",
      narrRulespeak: "文档", narrEnglish: "通俗读法", narrFormula: "公式",
      narrRulespeakHint: "声明式业务规则（仅当 / 必须 / 优先级）",
      narrEnglishHint: "将公式读作一句简单的句子",
      narrFormulaHint: "原始的电子表格式公式，字段可点击",
      narrAriaLabel: "如何解释此规则",
      gearTitle: "设置 — 选择悬停卡片和字段页面中显示的内容",
      gearAriaLabel: "设置",
      gearPopTitle: "在弹窗和页面中显示",
      gearReset: "全部重新开启",
      elemDesc: "描述", elemDescHint: "字段的通俗描述。",
      elemRulespeak: "文档", elemRulespeakHint: "声明式业务规则。",
      elemEnglish: "通俗读法", elemEnglishHint: "将公式读作一句简单的句子。",
      elemFormula: "公式", elemFormulaHint: "原始的电子表格式公式。",
      elemInputs: "输入", elemInputsHint: "此单行函数调用的字段。",
      elemConsumers: "使用方", elemConsumersHint: "其函数调用此函数的字段。",
      provOn: "溯源 · 开", provOff: "溯源 · 关",
      provOnTitle: "隐藏 ƒ 溯源标记。双击单元格仍会打开 DAG。",
      provOffTitle: "在可追溯来源的单元格上显示 ƒ 溯源标记。",
      downloadXlsx: "下载工作簿", downloadXlsxTitle: "将规则手册下载为 Excel 工作簿（.xlsx）",
      gearShowAs: "显示方式", gearLanguage: "语言",
      navSearch: "筛选表和字段…",
      navDerivedOnly: "仅派生",
      navNoMatch: "没有匹配项。",
      groupOther: "其他",
      tabAll: "全部",
      tabDerived: "派生",
      tabStored: "存储",
      tabRelationships: "关系",
      viewGroups: "分组",
      viewList: "A–Z",
      viewGraph: "图",
      tablesCount: (n) => `${n} 个表`,
      graphHint: "每个方框是一张表；连线从一张表指向它所引用的表。点击表即可打开。",
      filterFields: "筛选字段…",
      cellHintOn: "悬停标记可预览；双击打开完整的 DAG 页面",
      cellHintOff: "双击查看溯源（在页头切换 ƒ 标记）",
      ontologyTitle: "本体",
      ontologyLead: "规则手册中的所有表。点击某个表查看其列。",
      fieldsCount: (n) => `${n} 个字段`, derivedCount: (n) => `${n} 个派生`, relCount: (n) => `${n} 个关系`,
      tableNotFound: "未找到该表。", fieldNotFound: "规则手册中未找到该字段。",
      nullable: "可为空",
      hopsFromGround: (n) => `距基础数据 ${n} 跳`,
      relationshipPointer: "关系指针", groundTruth: "基础数据",
      relPointerBody: (link) => `此字段是指向 ${link} 的指针。它不由其他任何内容派生——它是 DAG 其余部分所依附的链接。`,
      relPointerBodyNoLink: "此字段是指向另一个表的指针。它不由其他任何内容派生——它是 DAG 其余部分所依附的链接。",
      groundTruthBody: (dt) => `此单元格是叶节点。它直接<em>写入</em>数据库（数据类型 <code>${dt}</code>），不由其他任何内容计算得出。上游的一切最终都可追溯到这样的值。`,
      openTable: (t) => `打开 ${t} →`,
      ruleCardTitle: "此单元格是一条声明式规则的值",
      oneLineFnCardTitle: "此单元格是一个单行函数的值",
      inRuleSpeak: "文档 表述", inEnglish: "通俗表述", asFormula: "公式表述",
      mechanicalFlag: "⚠︎ 机械化", mechanicalTitle: "含义准确但措辞生硬 — 可选择性改写的标记",
      noRuleRendered: (t) => `未为此 ${t} 字段生成声明式规则。`,
      obligations: "义务",
      ruleSpeakHintFormula: "这是公式所表达的同一逻辑，以业务规则的形式写出 — 由共享的 文档 引擎根据规则手册生成。",
      ruleSpeakHintNoFormula: "由共享的 文档 引擎根据规则手册生成。",
      englishHint: "每个带下划线的字段本身都是一个单行函数。点击任意字段可深入查看。",
      noFormulaSentence: (t) => `此 ${t} 没有可读作句子的电子表格公式 — 它是通过关系结构化定义的汇总。切换到 <strong>文档</strong> 查看其定义。`,
      formulaHint: "每个标签本身都是一个单行函数。点击任意标签可深入查看。",
      noFormulaAtAll: (t) => `此 ${t} 没有电子表格公式 — 它是通过关系结构化定义的汇总。切换到 <strong>文档</strong> 查看其定义。`,
      inputsTitle: "输入", inputsLead: "此单行函数调用的字段。",
      groundTruthInline: (dt) => `基础数据 · 数据类型 ${dt}`,
      consumersTitle: "使用方",
      consumersDirect: "直接", consumersTransitive: "间接",
      consumersEmpty: "规则手册中目前没有其他内容引用此单元格。",
      consumersLead: "其单行函数调用此函数的字段。",
      showTransitive: (n) => `显示 ${n} 个间接使用方（下游的下游）`,
      leavesTitle: "叶节点处的基础数据",
      leavesLead: "归根结底，此单元格的值取决于这些原始值。编辑其中任何一个并重新构建，变化都会自动传播到这里。",
      hoverGroundTruth: (dt) => `基础数据 — 直接写入数据库${dt ? `（数据类型 <code>${dt}</code>）` : ""}。`,
      hoverRelPointer: "关系指针 — DAG 其余部分所依附的链接。",
      hoverNoRule: "未生成声明式规则。",
      hoverNoFormula: "无公式 — 结构化汇总。请参阅 文档。",
      hoverInputs: "输入",
      hoverHint: "双击单元格打开完整的 DAG 页面。",
      hoverOpenPage: "打开完整页面",
      // The witness (witness.js): one row's value and the values it came from.
      witnessTitle: "逐级验证至基础数据",
      witnessLead: "此单元格在某一行上的值，以及得出它的每一个值，一直追溯到存储的值。每个值都是刚刚在这里由本项目生成的 TypeScript（rulebook-to-typescript）计算出来的。展开任意一行，可查看它由哪些值得出。",
      witnessRow: "行",
      witnessForRow: (label) => `针对 ${label}`,
      witnessComputing: "正在用生成的 TypeScript 计算此行…",
      witnessNoRows: (t) => `${t} 没有可供验证的行。`,
      witnessFailed: "无法计算验证结果。",
      witnessGroundTruth: "基础数据",
      witnessPointsTo: (t, label) => `指向 ${t} 的 ${label}`,
      witnessPointsNowhere: (t) => `未指向 ${t} 中的任何行`,
      witnessBlank: "空",
      witnessEmptyText: "空文本",
      witnessError: (m) => `无法计算：${m}`,
      witnessStep: (n, of) => `共 ${of} 步，应用了第 ${n} 步。`,
      witnessNoStep: "没有任何步骤适用，因此值为空。",
      witnessStepMark: (noun) => (noun ? `适用于此 ${noun}` : "适用于此行"),
      witnessLookup: (key, value, t, label) => `通过 ${key} = ${value} 查找：读取自 ${t} 的 ${label}。`,
      witnessLookupMiss: (key, value, t) => `通过 ${key} = ${value} 查找，但 ${t} 中没有对应的行。`,
      witnessOver: (op, n, t) => ({
        COUNTIFS: `计数：${t} 中有 ${n} 行匹配。`,
        COUNT: `计数：${t} 中有 ${n} 行匹配。`,
        SUM: `对 ${t} 中匹配的 ${n} 行求和。`,
        AVERAGE: `对 ${t} 中匹配的 ${n} 行求平均值。`,
        MIN: `${t} 中匹配的 ${n} 行里的最小值。`,
        MAX: `${t} 中匹配的 ${n} 行里的最大值。`,
      }[op] || `根据 ${t} 中匹配的 ${n} 行计算。`),
      witnessOverUnknown: "根据本页面无法列出的行计算。",
      witnessMoreRows: (n) => `… 另有 ${n} 行`,
      witnessSeenAbove: "上方已显示",
      type_raw: "基础数据", type_calculated: "计算", type_aggregation: "聚合", type_relationship: "关系", type_lookup: "查找",
    },
    hi: {
      back: "← वापस", backTitle: "एक कदम पीछे जाएँ",
      home: "🏠 होम", homeTitle: "एक्सप्लेनर छोड़ें — ऐप पर वापस जाएँ",
      tablesCrumb: "तालिकाएँ",
      narrRulespeak: "दस्तावेज़ीकरण", narrEnglish: "सरल पठन", narrFormula: "सूत्र",
      narrRulespeakHint: "घोषणात्मक व्यावसायिक नियम (केवल यदि / अनिवार्य / प्राथमिकता)",
      narrEnglishHint: "सूत्र को एक सरल वाक्य के रूप में पढ़ा गया",
      narrFormulaHint: "स्प्रेडशीट-शैली का मूल सूत्र, क्लिक करने योग्य फ़ील्ड के साथ",
      narrAriaLabel: "इस नियम को कैसे समझाएँ",
      gearTitle: "सेटिंग्स — चुनें कि होवर कार्ड और फ़ील्ड पेज पर क्या दिखे",
      gearAriaLabel: "सेटिंग्स",
      gearPopTitle: "पॉपअप और पेज पर दिखाएँ",
      gearReset: "सब फिर से चालू करें",
      elemDesc: "विवरण", elemDescHint: "फ़ील्ड का सरल भाषा में विवरण।",
      elemRulespeak: "दस्तावेज़ीकरण", elemRulespeakHint: "घोषणात्मक व्यावसायिक नियम।",
      elemEnglish: "सरल पठन", elemEnglishHint: "सूत्र को एक सरल वाक्य के रूप में पढ़ा गया।",
      elemFormula: "सूत्र", elemFormulaHint: "स्प्रेडशीट-शैली का मूल सूत्र।",
      elemInputs: "इनपुट", elemInputsHint: "वे फ़ील्ड जिन्हें यह एक-पंक्ति फ़ंक्शन कॉल करता है।",
      elemConsumers: "उपभोक्ता", elemConsumersHint: "वे फ़ील्ड जिनका फ़ंक्शन इसे कॉल करता है।",
      provOn: "उत्पत्ति · चालू", provOff: "उत्पत्ति · बंद",
      provOnTitle: "ƒ उत्पत्ति चिह्न छिपाएँ। सेल पर डबल-क्लिक करने से DAG फिर भी खुलता है।",
      provOffTitle: "जिन सेल की उत्पत्ति देखी जा सकती है, उन पर ƒ उत्पत्ति चिह्न दिखाएँ।",
      downloadXlsx: "वर्कबुक डाउनलोड करें", downloadXlsxTitle: "नियम-पुस्तिका को Excel वर्कबुक (.xlsx) के रूप में डाउनलोड करें",
      gearShowAs: "ऐसे दिखाएँ", gearLanguage: "भाषा",
      navSearch: "तालिकाएँ और फ़ील्ड फ़िल्टर करें…",
      navDerivedOnly: "केवल व्युत्पन्न",
      navNoMatch: "कुछ भी मेल नहीं खाता।",
      groupOther: "अन्य",
      tabAll: "सभी",
      tabDerived: "व्युत्पन्न",
      tabStored: "संग्रहीत",
      tabRelationships: "संबंध",
      viewGroups: "समूह",
      viewList: "A–Z",
      viewGraph: "ग्राफ़",
      tablesCount: (n) => `${n} तालिकाएँ`,
      graphHint: "हर बॉक्स एक तालिका है; रेखा एक तालिका से उस तालिका की ओर जाती है जिसका वह संदर्भ देती है। खोलने के लिए तालिका पर क्लिक करें।",
      filterFields: "फ़ील्ड फ़िल्टर करें…",
      cellHintOn: "झलक के लिए बैज पर होवर करें; पूरा DAG पेज खोलने के लिए डबल-क्लिक करें",
      cellHintOff: "उत्पत्ति देखने के लिए डबल-क्लिक करें (हेडर में ƒ चिह्न चालू/बंद करें)",
      ontologyTitle: "ऑन्टोलॉजी",
      ontologyLead: "नियम-पुस्तिका की हर तालिका। किसी तालिका के कॉलम देखने के लिए उस पर क्लिक करें।",
      fieldsCount: (n) => `${n} फ़ील्ड`, derivedCount: (n) => `${n} व्युत्पन्न`, relCount: (n) => `${n} संबंध`,
      tableNotFound: "तालिका नहीं मिली।", fieldNotFound: "नियम-पुस्तिका में फ़ील्ड नहीं मिला।",
      nullable: "NULL स्वीकार्य",
      hopsFromGround: (n) => `मूल डेटा से ${n} चरण दूर`,
      relationshipPointer: "संबंध सूचक", groundTruth: "मूल डेटा",
      relPointerBody: (link) => `यह फ़ील्ड ${link} की ओर एक सूचक है। यह किसी और चीज़ से व्युत्पन्न नहीं है — यही वह कड़ी है जिस पर शेष DAG टिका है।`,
      relPointerBodyNoLink: "यह फ़ील्ड किसी अन्य तालिका की ओर एक सूचक है। यह किसी और चीज़ से व्युत्पन्न नहीं है — यही वह कड़ी है जिस पर शेष DAG टिका है।",
      groundTruthBody: (dt) => `यह सेल एक पत्ती है। इसे सीधे डेटाबेस में <em>लिखा</em> जाता है (डेटा प्रकार <code>${dt}</code>) और यह किसी और चीज़ से गणना नहीं किया जाता। ऊपर की हर चीज़ अंततः ऐसे ही मानों तक पहुँचती है।`,
      openTable: (t) => `${t} खोलें →`,
      ruleCardTitle: "यह सेल एक घोषणात्मक नियम का मान है",
      oneLineFnCardTitle: "यह सेल एक एक-पंक्ति फ़ंक्शन का मान है",
      inRuleSpeak: "दस्तावेज़ीकरण में", inEnglish: "सरल शब्दों में", asFormula: "सूत्र के रूप में",
      mechanicalFlag: "⚠︎ यांत्रिक", mechanicalTitle: "अर्थ सही है पर शब्द-रचना अटपटी है — वैकल्पिक पुनर्लेखन के लिए संकेत",
      noRuleRendered: (t) => `इस ${t} फ़ील्ड के लिए कोई घोषणात्मक नियम नहीं बनाया गया।`,
      obligations: "दायित्व",
      ruleSpeakHintFormula: "यह वही तर्क है जो सूत्र में है, व्यावसायिक नियम के रूप में लिखा गया — साझा दस्तावेज़ीकरण इंजन द्वारा नियम-पुस्तिका से बनाया गया।",
      ruleSpeakHintNoFormula: "साझा दस्तावेज़ीकरण इंजन द्वारा नियम-पुस्तिका से बनाया गया।",
      englishHint: "रेखांकित हर फ़ील्ड स्वयं एक एक-पंक्ति फ़ंक्शन है। विस्तार से देखने के लिए किसी पर भी क्लिक करें।",
      noFormulaSentence: (t) => `इस ${t} का कोई स्प्रेडशीट सूत्र नहीं है जिसे वाक्य की तरह पढ़ा जा सके — यह किसी संबंध पर संरचनात्मक रूप से परिभाषित एक समेकन है। इसकी परिभाषा देखने के लिए <strong>दस्तावेज़ीकरण</strong> पर जाएँ।`,
      formulaHint: "हर चिप स्वयं एक एक-पंक्ति फ़ंक्शन है। विस्तार से देखने के लिए किसी भी चिप पर क्लिक करें।",
      noFormulaAtAll: (t) => `इस ${t} का कोई स्प्रेडशीट सूत्र नहीं है — यह किसी संबंध पर संरचनात्मक रूप से परिभाषित एक समेकन है। इसकी परिभाषा देखने के लिए <strong>दस्तावेज़ीकरण</strong> पर जाएँ।`,
      inputsTitle: "इनपुट", inputsLead: "वे फ़ील्ड जिन्हें यह एक-पंक्ति फ़ंक्शन कॉल करता है।",
      groundTruthInline: (dt) => `मूल डेटा · डेटा प्रकार ${dt}`,
      consumersTitle: "उपभोक्ता",
      consumersDirect: "प्रत्यक्ष", consumersTransitive: "अप्रत्यक्ष",
      consumersEmpty: "नियम-पुस्तिका में अभी कोई और चीज़ इस सेल का संदर्भ नहीं देती।",
      consumersLead: "वे फ़ील्ड जिनका एक-पंक्ति फ़ंक्शन इसे कॉल करता है।",
      showTransitive: (n) => `${n} अप्रत्यक्ष उपभोक्ता दिखाएँ (आगे की शृंखला में)`,
      leavesTitle: "पत्तियों पर मूल डेटा",
      leavesLead: "अंततः इस सेल का मान इन मूल मानों पर निर्भर करता है। इनमें से किसी को बदलकर फिर से बिल्ड करने पर बदलाव अपने-आप यहाँ तक पहुँचता है।",
      hoverGroundTruth: (dt) => `मूल डेटा — सीधे डेटाबेस में लिखा गया${dt ? ` (डेटा प्रकार <code>${dt}</code>)` : ""}।`,
      hoverRelPointer: "संबंध सूचक — वह कड़ी जिस पर शेष DAG टिका है।",
      hoverNoRule: "कोई घोषणात्मक नियम नहीं बनाया गया।",
      hoverNoFormula: "कोई सूत्र नहीं — एक संरचनात्मक समेकन। दस्तावेज़ीकरण देखें।",
      hoverInputs: "इनपुट",
      hoverHint: "पूरा DAG पेज खोलने के लिए सेल पर डबल-क्लिक करें।",
      hoverOpenPage: "पूरा पेज खोलें",
      // The witness (witness.js): one row's value and the values it came from.
      witnessTitle: "मूल डेटा तक प्रमाणित",
      witnessLead: "एक पंक्ति के लिए इस सेल का मान, और वे सभी मान जिनसे यह बना है, संग्रहीत मानों तक। हर मान अभी-अभी यहीं इस प्रोजेक्ट के जनरेट किए गए TypeScript (rulebook-to-typescript) से निकाला गया है। कोई पंक्ति खोलकर देखें कि वह किन मानों से बनी है।",
      witnessRow: "पंक्ति",
      witnessForRow: (label) => `${label} के लिए`,
      witnessComputing: "जनरेट किए गए TypeScript से इस पंक्ति की गणना हो रही है…",
      witnessNoRows: (t) => `${t} में प्रमाणित करने के लिए कोई पंक्ति नहीं है।`,
      witnessFailed: "प्रमाण की गणना नहीं हो सकी।",
      witnessGroundTruth: "मूल डेटा",
      witnessPointsTo: (t, label) => `${t} के ${label} की ओर संकेत करता है`,
      witnessPointsNowhere: (t) => `${t} की किसी पंक्ति की ओर संकेत नहीं करता`,
      witnessBlank: "रिक्त",
      witnessEmptyText: "खाली पाठ",
      witnessError: (m) => `गणना नहीं हो सकी: ${m}`,
      witnessStep: (n, of) => `${of} में से चरण ${n} लागू हुआ।`,
      witnessNoStep: "कोई चरण लागू नहीं हुआ, इसलिए मान रिक्त है।",
      // The noun's gender is not known, so it follows a noun whose gender is.
      witnessStepMark: (noun) => (noun ? `इस रिकॉर्ड (${noun}) पर लागू` : "इस पंक्ति पर लागू"),
      witnessLookup: (key, value, t, label) => `${key} = ${value} के ज़रिए खोजा गया: ${t} के ${label} से पढ़ा गया।`,
      witnessLookupMiss: (key, value, t) => `${key} = ${value} के ज़रिए खोजा गया, पर ${t} में ऐसी कोई पंक्ति नहीं है।`,
      witnessOver: (op, n, t) => {
        const rows = `${t} की मेल खाने वाली ${n} ${n === 1 ? "पंक्ति" : "पंक्तियों"}`;
        return ({
          COUNTIFS: `गिनती: ${t} की ${n} ${n === 1 ? "पंक्ति मेल खाती है" : "पंक्तियाँ मेल खाती हैं"}।`,
          COUNT: `गिनती: ${t} की ${n} ${n === 1 ? "पंक्ति मेल खाती है" : "पंक्तियाँ मेल खाती हैं"}।`,
          SUM: `${rows} का योग।`,
          AVERAGE: `${rows} का औसत।`,
          MIN: `${rows} में सबसे छोटा मान।`,
          MAX: `${rows} में सबसे बड़ा मान।`,
        }[op] || `${rows} पर गणना की गई।`);
      },
      witnessOverUnknown: "उन पंक्तियों पर गणना की गई जिन्हें यह पेज सूचीबद्ध नहीं कर सकता।",
      witnessMoreRows: (n) => `… और ${n} ${n === 1 ? "पंक्ति" : "पंक्तियाँ"}`,
      witnessSeenAbove: "ऊपर पहले ही दिखाया गया",
      type_raw: "मूल डेटा", type_calculated: "गणना किया गया", type_aggregation: "समेकन", type_relationship: "संबंध", type_lookup: "लुकअप",
    },
    ar: {
      back: "→ رجوع", backTitle: "الرجوع خطوة واحدة",
      home: "🏠 الرئيسية", homeTitle: "مغادرة المُفسِّر والعودة إلى التطبيق",
      tablesCrumb: "الجداول",
      narrRulespeak: "التوثيق", narrEnglish: "القراءة المبسّطة", narrFormula: "الصيغة",
      narrRulespeakHint: "قاعدة العمل التصريحية (فقط إذا / يجب أن / الأولوية)",
      narrEnglishHint: "الصيغة مقروءةً في جملة بسيطة",
      narrFormulaHint: "الصيغة الخام بأسلوب الجداول الإلكترونية، مع حقول قابلة للنقر",
      narrAriaLabel: "طريقة شرح هذه القاعدة",
      gearTitle: "الإعدادات — اختر ما يظهر في بطاقة التمرير وفي صفحة الحقل",
      gearAriaLabel: "الإعدادات",
      gearPopTitle: "ما يُعرض في النافذة المنبثقة وفي الصفحة",
      gearReset: "إعادة تفعيل الكل",
      elemDesc: "الوصف", elemDescHint: "وصف الحقل بلغة مبسّطة.",
      elemRulespeak: "التوثيق", elemRulespeakHint: "قاعدة العمل التصريحية.",
      elemEnglish: "القراءة المبسّطة", elemEnglishHint: "الصيغة مقروءةً في جملة بسيطة.",
      elemFormula: "الصيغة", elemFormulaHint: "الصيغة الخام بأسلوب الجداول الإلكترونية.",
      elemInputs: "المدخلات", elemInputsHint: "الحقول التي تستدعيها هذه الدالة ذات السطر الواحد.",
      elemConsumers: "المستهلِكات", elemConsumersHint: "الحقول التي تستدعي دالتُها هذا الحقل.",
      provOn: "المصدر · مفعّل", provOff: "المصدر · معطّل",
      provOnTitle: "إخفاء رموز ƒ الخاصة بالمصدر. النقر المزدوج على الخلايا يفتح DAG كما هو.",
      provOffTitle: "إظهار رموز ƒ على الخلايا التي يمكن استكشاف مصدرها.",
      downloadXlsx: "تنزيل المصنّف", downloadXlsxTitle: "تنزيل دفتر القواعد في صورة مصنّف Excel (.xlsx)",
      gearShowAs: "العرض", gearLanguage: "اللغة",
      navSearch: "تصفية الجداول والحقول…",
      navDerivedOnly: "المشتقة فقط",
      navNoMatch: "لا توجد نتائج.",
      groupOther: "أخرى",
      tabAll: "الكل",
      tabDerived: "مشتقة",
      tabStored: "مخزَّنة",
      tabRelationships: "العلاقات",
      viewGroups: "المجموعات",
      viewList: "A–Z",
      viewGraph: "الرسم",
      tablesCount: (n) => `${n} جدول`,
      graphHint: "كل مربع جدول؛ وخط يمتد من جدول إلى الجدول الذي يشير إليه. انقر على جدول لفتحه.",
      filterFields: "تصفية الحقول…",
      cellHintOn: "مرّر المؤشر على الشارة للمعاينة؛ وانقر نقرًا مزدوجًا لفتح صفحة DAG كاملة",
      cellHintOff: "انقر نقرًا مزدوجًا لعرض المصدر (بدّل رموز ƒ من الترويسة)",
      ontologyTitle: "الأنطولوجيا",
      ontologyLead: "كل جدول في دفتر القواعد. انقر جدولًا لعرض أعمدته.",
      fieldsCount: (n) => `الحقول: ${n}`, derivedCount: (n) => `المشتقة: ${n}`, relCount: (n) => `العلاقات: ${n}`,
      tableNotFound: "الجدول غير موجود.", fieldNotFound: "الحقل غير موجود في دفتر القواعد.",
      nullable: "يقبل القيمة الفارغة",
      hopsFromGround: (n) => (n === 1 ? "قفزة واحدة عن البيانات الأصلية" : `عدد القفزات عن البيانات الأصلية: ${n}`),
      relationshipPointer: "مؤشّر علاقة", groundTruth: "بيانات أصلية",
      relPointerBody: (link) => `هذا الحقل مؤشّر إلى الجدول ${link}. وهو لا يُشتق من أي شيء آخر؛ فهو الرابط الذي يتعلّق به سائر DAG.`,
      relPointerBodyNoLink: "هذا الحقل مؤشّر إلى جدول آخر. وهو لا يُشتق من أي شيء آخر؛ فهو الرابط الذي يتعلّق به سائر DAG.",
      groundTruthBody: (dt) => `هذه الخلية طرف نهائي. قيمتها <em>مكتوبة</em> مباشرةً في قاعدة البيانات (نوع البيانات <code>${dt}</code>) ولا تُحسب من أي شيء آخر. وكل ما يسبقها يعود في النهاية إلى قيم كهذه.`,
      openTable: (t) => `فتح الجدول «${t}» ←`,
      ruleCardTitle: "هذه الخلية هي قيمة قاعدة تصريحية واحدة",
      oneLineFnCardTitle: "هذه الخلية هي قيمة دالة واحدة ذات سطر واحد",
      inRuleSpeak: "بترميز التوثيق", inEnglish: "بلغة مبسّطة", asFormula: "في صورة صيغة",
      mechanicalFlag: "⚠︎ آلي", mechanicalTitle: "صياغة أمينة لكنها متكلّفة — إشارة إلى إعادة صياغة اختيارية",
      noRuleRendered: (t) => `لم تُنشأ قاعدة تصريحية لهذا الحقل (نوعه: ${t}).`,
      obligations: "الالتزامات",
      ruleSpeakHintFormula: "هذا هو المنطق نفسه الذي تُرمّزه الصيغة، مكتوبًا في صورة قاعدة عمل — استخرجه محرّك التوثيق المشترك من دفتر القواعد.",
      ruleSpeakHintNoFormula: "استخرجه محرّك التوثيق المشترك من دفتر القواعد.",
      englishHint: "كل حقل مسطّر هو بدوره دالة ذات سطر واحد. انقر أيًّا منها للتعمّق.",
      noFormulaSentence: (t) => `ليست لهذا الحقل (نوعه: ${t}) صيغة جدولية تُقرأ جملةً — فهو تجميع معرّف بنيويًا على علاقة. انتقل إلى <strong>التوثيق</strong> لترى كيف يُعرَّف.`,
      formulaHint: "كل شارة هي بدورها دالة ذات سطر واحد. انقر أي شارة للتعمّق.",
      noFormulaAtAll: (t) => `ليست لهذا الحقل (نوعه: ${t}) صيغة جدولية — فهو تجميع معرّف بنيويًا على علاقة. انتقل إلى <strong>التوثيق</strong> لترى كيف يُعرَّف.`,
      inputsTitle: "المدخلات", inputsLead: "الحقول التي تستدعيها هذه الدالة ذات السطر الواحد.",
      groundTruthInline: (dt) => `بيانات أصلية · نوع البيانات ${dt}`,
      consumersTitle: "المستهلِكات",
      consumersDirect: "مباشرة", consumersTransitive: "غير مباشرة",
      consumersEmpty: "لا يوجد في دفتر القواعد ما يشير إلى هذه الخلية حتى الآن.",
      consumersLead: "الحقول التي تستدعي دالتُها ذات السطر الواحد هذا الحقل.",
      showTransitive: (n) => `عرض المستهلِكات غير المباشرة (ما يلي المستهلِكات)، وعددها ${n}`,
      leavesTitle: "البيانات الأصلية عند الأطراف",
      leavesLead: "تعتمد قيمة هذه الخلية في النهاية على هذه القيم الخام. تحرير أي منها ثم إعادة البناء ينتقل أثره إلى هنا تلقائيًا.",
      hoverGroundTruth: (dt) => `بيانات أصلية — مكتوبة مباشرةً في قاعدة البيانات${dt ? ` (نوع البيانات <code>${dt}</code>)` : ""}.`,
      hoverRelPointer: "مؤشّر علاقة — الرابط الذي يتعلّق به سائر DAG.",
      hoverNoRule: "لم تُنشأ قاعدة تصريحية.",
      hoverNoFormula: "لا صيغة — تجميع بنيوي. انظر التوثيق.",
      hoverInputs: "المدخلات",
      hoverHint: "انقر الخلية نقرًا مزدوجًا لفتح صفحة DAG كاملة.",
      hoverOpenPage: "فتح الصفحة كاملة",
      // The witness (witness.js): one row's value and the values it came from.
      witnessTitle: "الشاهد، وصولًا إلى البيانات الأصلية",
      witnessLead: "قيمة هذه الخلية لسجل واحد، وكل القيم التي نشأت منها، وصولًا إلى القيم المخزّنة. حُسبت كل قيمة هنا الآن بواسطة شيفرة TypeScript المولَّدة لهذا المشروع (rulebook-to-typescript). افتح أي سطر لترى ما تكوّنت منه.",
      witnessRow: "السجل",
      witnessForRow: (label) => `بالنسبة إلى السجل «${label}»`,
      witnessComputing: "جارٍ حساب هذا السجل بشيفرة TypeScript المولَّدة…",
      witnessNoRows: (t) => `لا توجد سجلات في الجدول «${t}» يمكن إظهار شاهد عليها.`,
      witnessFailed: "تعذّر حساب الشاهد.",
      witnessGroundTruth: "بيانات أصلية",
      witnessPointsTo: (t, label) => `يشير إلى السجل «${label}» في الجدول «${t}»`,
      witnessPointsNowhere: (t) => `لا يشير إلى أي سجل في الجدول «${t}»`,
      witnessBlank: "فارغ",
      witnessEmptyText: "نص فارغ",
      witnessError: (m) => `تعذّر حسابه: ${m}`,
      witnessStep: (n, of) => `طُبّقت الخطوة ${n} من ${of}.`,
      witnessNoStep: "لم تُطبَّق أي خطوة، فالقيمة فارغة.",
      // `noun` is what one row of the table is called ("case"); "row" only when the
      // graph does not say (rowNoun).
      witnessStepMark: (noun) => (noun ? `طُبّقت على هذا السجل (${noun})` : "طُبّقت على هذا السجل"),
      witnessLookup: (key, value, t, label) => `بُحث عبر ${key} = ${value}؛ والقيمة مقروءة من السجل «${label}» في الجدول «${t}».`,
      witnessLookupMiss: (key, value, t) => `بُحث عبر ${key} = ${value}، وهذه القيمة لا تطابق أي سجل في الجدول «${t}».`,
      witnessOver: (op, n, t) => ({
        COUNTIFS: `العدّ: عدد السجلات المطابقة في الجدول «${t}» هو ${n}.`,
        COUNT: `العدّ: عدد السجلات المطابقة في الجدول «${t}» هو ${n}.`,
        SUM: `جُمعت القيم عبر السجلات المطابقة في الجدول «${t}»، وعددها ${n}.`,
        AVERAGE: `حُسب المتوسط عبر السجلات المطابقة في الجدول «${t}»، وعددها ${n}.`,
        MIN: `الأصغر بين السجلات المطابقة في الجدول «${t}»، وعددها ${n}.`,
        MAX: `الأكبر بين السجلات المطابقة في الجدول «${t}»، وعددها ${n}.`,
      }[op] || `حُسبت عبر السجلات المطابقة في الجدول «${t}»، وعددها ${n}.`),
      witnessOverUnknown: "حُسبت عبر سجلات لا تستطيع هذه الصفحة سردها.",
      witnessMoreRows: (n) => `… وسجلات أخرى، عددها ${n}`,
      witnessSeenAbove: "ظاهر أعلاه",
      type_raw: "بيانات أصلية", type_calculated: "محسوب", type_aggregation: "تجميع", type_relationship: "علاقة", type_lookup: "بحث",
    },
    id: {
      back: "← kembali", backTitle: "Kembali satu langkah",
      home: "🏠 Beranda", homeTitle: "Tinggalkan penjelas — kembali ke aplikasi",
      tablesCrumb: "Tabel",
      narrRulespeak: "Dokumentasi", narrEnglish: "Pembacaan sederhana", narrFormula: "Rumus",
      narrRulespeakHint: "Aturan bisnis deklaratif (hanya jika / harus / prioritas)",
      narrEnglishHint: "Rumus yang dibacakan sebagai kalimat sederhana",
      narrFormulaHint: "Rumus mentah bergaya lembar kerja, dengan kolom yang dapat diklik",
      narrAriaLabel: "Cara menjelaskan aturan ini",
      gearTitle: "Pengaturan — pilih apa yang tampil di kartu melayang dan di halaman kolom",
      gearAriaLabel: "Pengaturan",
      gearPopTitle: "Tampilkan di kartu melayang dan di halaman",
      gearReset: "Aktifkan kembali semuanya",
      elemDesc: "Deskripsi", elemDescHint: "Deskripsi kolom dalam bahasa sehari-hari.",
      elemRulespeak: "Dokumentasi", elemRulespeakHint: "Aturan bisnis deklaratif.",
      elemEnglish: "Pembacaan sederhana", elemEnglishHint: "Rumus yang dibacakan sebagai kalimat sederhana.",
      elemFormula: "Rumus", elemFormulaHint: "Rumus mentah bergaya lembar kerja.",
      elemInputs: "Masukan", elemInputsHint: "Kolom yang dipanggil oleh fungsi satu baris ini.",
      elemConsumers: "Pemakai", elemConsumersHint: "Kolom yang fungsinya memanggil kolom ini.",
      provOn: "Asal-usul · AKTIF", provOff: "Asal-usul · nonaktif",
      provOnTitle: "Sembunyikan tanda asal-usul ƒ. Klik ganda pada sel tetap membuka DAG.",
      provOffTitle: "Tampilkan tanda asal-usul ƒ pada sel yang asal-usulnya dapat ditelusuri.",
      downloadXlsx: "Unduh buku kerja", downloadXlsxTitle: "Unduh buku aturan sebagai buku kerja Excel (.xlsx)",
      gearShowAs: "Tampilkan sebagai", gearLanguage: "Bahasa",
      navSearch: "Saring tabel dan kolom…",
      navDerivedOnly: "Hanya turunan",
      navNoMatch: "Tidak ada yang cocok.",
      groupOther: "Lainnya",
      tabAll: "Semua",
      tabDerived: "Turunan",
      tabStored: "Tersimpan",
      tabRelationships: "Relasi",
      viewGroups: "Grup",
      viewList: "A–Z",
      viewGraph: "Grafik",
      tablesCount: (n) => `${n} tabel`,
      graphHint: "Setiap kotak adalah tabel; garis mengarah dari satu tabel ke tabel yang dirujuknya. Klik tabel untuk membukanya.",
      filterFields: "Saring kolom…",
      cellHintOn:"Arahkan kursor ke tanda untuk melihat sekilas; klik ganda untuk membuka halaman DAG lengkap",
      cellHintOff: "Klik ganda untuk melihat asal-usul (tanda ƒ diatur di bagian kepala)",
      ontologyTitle: "Ontologi",
      ontologyLead: "Semua tabel dalam buku aturan. Klik sebuah tabel untuk melihat kolom-kolomnya.",
      fieldsCount: (n) => `${n} kolom`, derivedCount: (n) => `${n} turunan`, relCount: (n) => `${n} relasi`,
      tableNotFound: "Tabel tidak ditemukan.", fieldNotFound: "Kolom tidak ditemukan dalam buku aturan.",
      nullable: "boleh kosong",
      hopsFromGround: (n) => `${n} lompatan dari fakta dasar`,
      relationshipPointer: "Penunjuk relasi", groundTruth: "Fakta dasar",
      relPointerBody: (link) => `Kolom ini adalah penunjuk ke ${link}. Nilainya tidak diturunkan dari apa pun — inilah tautan tempat seluruh bagian lain DAG bergantung.`,
      relPointerBodyNoLink: "Kolom ini adalah penunjuk ke tabel lain. Nilainya tidak diturunkan dari apa pun — inilah tautan tempat seluruh bagian lain DAG bergantung.",
      groundTruthBody: (dt) => `Sel ini adalah simpul daun. Nilainya <em>ditulis</em> langsung ke basis data (tipe data <code>${dt}</code>) dan tidak dihitung dari apa pun. Semua nilai lain di hulu pada akhirnya bersumber dari nilai seperti ini.`,
      openTable: (t) => `Buka ${t} →`,
      ruleCardTitle: "Sel ini adalah nilai dari satu aturan deklaratif",
      oneLineFnCardTitle: "Sel ini adalah nilai dari satu fungsi satu baris",
      inRuleSpeak: "Dalam Dokumentasi", inEnglish: "Dalam kata-kata sederhana", asFormula: "Sebagai rumus",
      mechanicalFlag: "⚠︎ mekanis", mechanicalTitle: "Rumusan yang setia tetapi kaku — isyarat untuk penyuntingan ulang yang opsional",
      noRuleRendered: (t) => `Tidak ada aturan deklaratif yang disusun untuk kolom berjenis ${t} ini.`,
      obligations: "Kewajiban",
      ruleSpeakHintFormula: "Ini logika yang sama dengan yang dikandung rumus, ditulis sebagai aturan bisnis — disusun dari buku aturan oleh mesin Dokumentasi bersama.",
      ruleSpeakHintNoFormula: "Disusun dari buku aturan oleh mesin Dokumentasi bersama.",
      englishHint: "Setiap kolom yang bergaris bawah juga merupakan fungsi satu baris. Klik salah satunya untuk menelusuri lebih dalam.",
      noFormulaSentence: (t) => `Kolom berjenis ${t} ini tidak memiliki rumus lembar kerja untuk dibacakan sebagai kalimat — kolom ini adalah agregasi yang didefinisikan secara struktural di atas sebuah relasi. Beralihlah ke <strong>Dokumentasi</strong> untuk melihat definisinya.`,
      formulaHint: "Setiap label juga merupakan fungsi satu baris. Klik salah satunya untuk menelusuri lebih dalam.",
      noFormulaAtAll: (t) => `Kolom berjenis ${t} ini tidak memiliki rumus lembar kerja — kolom ini adalah agregasi yang didefinisikan secara struktural di atas sebuah relasi. Beralihlah ke <strong>Dokumentasi</strong> untuk melihat definisinya.`,
      inputsTitle: "Masukan", inputsLead: "Kolom yang dipanggil oleh fungsi satu baris ini.",
      groundTruthInline: (dt) => `Fakta dasar · tipe data ${dt}`,
      consumersTitle: "Pemakai",
      consumersDirect: "langsung", consumersTransitive: "transitif",
      consumersEmpty: "Belum ada bagian lain dalam buku aturan yang merujuk pada sel ini.",
      consumersLead: "Kolom yang fungsi satu barisnya memanggil kolom ini.",
      showTransitive: (n) => `Tampilkan ${n} pemakai transitif (hilir dari hilir)`,
      leavesTitle: "Fakta dasar di simpul daun",
      leavesLead: "Pada akhirnya, nilai sel ini bergantung pada nilai-nilai mentah berikut. Jika salah satunya diubah lalu dibangun ulang, perubahannya sampai ke sini dengan sendirinya.",
      hoverGroundTruth: (dt) => `Fakta dasar — ditulis langsung ke basis data${dt ? ` (tipe data <code>${dt}</code>)` : ""}.`,
      hoverRelPointer: "Penunjuk relasi — tautan tempat seluruh bagian lain DAG bergantung.",
      hoverNoRule: "Tidak ada aturan deklaratif yang disusun.",
      hoverNoFormula: "Tanpa rumus — agregasi struktural. Lihat Dokumentasi.",
      hoverInputs: "Masukan",
      hoverHint: "Klik ganda pada sel untuk membuka halaman DAG lengkap.",
      hoverOpenPage: "Buka halaman lengkap",
      // The witness (witness.js): one row's value and the values it came from.
      witnessTitle: "Dibuktikan, hingga ke fakta dasar",
      witnessLead: "Nilai sel ini untuk satu baris, beserta setiap nilai yang menjadi asalnya, hingga ke nilai yang tersimpan. Semuanya dihitung di sini, baru saja, oleh TypeScript yang dihasilkan untuk proyek ini (rulebook-to-typescript). Buka sebuah baris untuk melihat dari nilai apa saja baris itu dibentuk.",
      witnessRow: "Baris",
      witnessForRow: (label) => `Untuk ${label}`,
      witnessComputing: "Menghitung baris ini dengan TypeScript yang dihasilkan…",
      witnessNoRows: (t) => `Tabel ${t} tidak memiliki baris untuk dibuktikan.`,
      witnessFailed: "Bukti tidak dapat dihitung.",
      witnessGroundTruth: "fakta dasar",
      // The row's label comes first and the table follows a fixed head noun, so
      // neither term needs a particle of its own.
      witnessPointsTo: (t, label) => `menunjuk ke ${label} pada tabel ${t}`,
      witnessPointsNowhere: (t) => `tidak menunjuk ke baris mana pun pada tabel ${t}`,
      witnessBlank: "kosong",
      witnessEmptyText: "teks kosong",
      witnessError: (m) => `tidak dapat dihitung: ${m}`,
      witnessStep: (n, of) => `Langkah ${n} dari ${of} yang berlaku.`,
      witnessNoStep: "Tidak ada langkah yang berlaku, sehingga nilainya kosong.",
      // `noun` follows a fixed Indonesian head noun ("entri ini (customer)").
      witnessStepMark: (noun) => (noun ? `berlaku untuk entri ini (${noun})` : "berlaku untuk baris ini"),
      witnessLookup: (key, value, t, label) => `Dicari melalui ${key} = ${value}: dibaca dari ${label} pada tabel ${t}.`,
      witnessLookupMiss: (key, value, t) => `Dicari melalui ${key} = ${value}, tetapi tidak ada baris yang sesuai pada tabel ${t}.`,
      // Indonesian has no plural, so one wording serves every count.
      witnessOver: (op, n, t) => ({
        COUNTIFS: `Dicacah: ${n} baris yang cocok pada tabel ${t}.`,
        COUNT: `Dicacah: ${n} baris yang cocok pada tabel ${t}.`,
        SUM: `Total dari ${n} baris yang cocok pada tabel ${t}.`,
        AVERAGE: `Rata-rata dari ${n} baris yang cocok pada tabel ${t}.`,
        MIN: `Nilai terkecil dari ${n} baris yang cocok pada tabel ${t}.`,
        MAX: `Nilai terbesar dari ${n} baris yang cocok pada tabel ${t}.`,
      }[op] || `Dihitung dari ${n} baris yang cocok pada tabel ${t}.`),
      witnessOverUnknown: "Dihitung dari baris-baris yang tidak dapat didaftar oleh halaman ini.",
      witnessMoreRows: (n) => `… dan ${n} baris lainnya`,
      witnessSeenAbove: "sudah ditampilkan di atas",
      type_raw: "Fakta dasar", type_calculated: "Terhitung", type_aggregation: "Agregasi", type_relationship: "Relasi", type_lookup: "Pencarian",
    },
    ko: {
      back: "← 뒤로", backTitle: "한 단계 뒤로 이동",
      home: "🏠 홈", homeTitle: "익스플레이너를 나가 앱으로 돌아가기",
      tablesCrumb: "테이블",
      narrRulespeak: "문서", narrEnglish: "풀어 읽기", narrFormula: "수식",
      narrRulespeakHint: "선언적 비즈니스 규칙(경우에만 / 해야 합니다 / 우선순위)",
      narrEnglishHint: "수식을 평이한 문장으로 읽은 것",
      narrFormulaHint: "클릭할 수 있는 필드가 포함된, 스프레드시트 형식의 원본 수식",
      narrAriaLabel: "이 규칙을 설명하는 방식",
      gearTitle: "설정 — 호버 카드와 필드 페이지에 표시할 항목 선택",
      gearAriaLabel: "설정",
      gearPopTitle: "팝업과 페이지에 표시할 항목",
      gearReset: "모두 켜기로 되돌리기",
      elemDesc: "설명", elemDescHint: "필드에 대한 평이한 설명입니다.",
      elemRulespeak: "문서", elemRulespeakHint: "선언적 비즈니스 규칙입니다.",
      elemEnglish: "풀어 읽기", elemEnglishHint: "수식을 평이한 문장으로 소리 내어 읽은 것입니다.",
      elemFormula: "수식", elemFormulaHint: "스프레드시트 형식의 원본 수식입니다.",
      elemInputs: "입력", elemInputsHint: "이 한 줄 함수가 호출하는 필드입니다.",
      elemConsumers: "사용처", elemConsumersHint: "함수에서 이 필드를 호출하는 필드입니다.",
      provOn: "출처 표시 · 켜짐", provOff: "출처 표시 · 꺼짐",
      provOnTitle: "ƒ 출처 표시를 숨깁니다. 셀을 더블클릭하면 여전히 DAG가 열립니다.",
      provOffTitle: "출처를 살펴볼 수 있는 셀에 ƒ 출처 표시를 나타냅니다.",
      downloadXlsx: "통합 문서 내려받기", downloadXlsxTitle: "룰북을 Excel 통합 문서(.xlsx)로 내려받기",
      gearShowAs: "표시 방식", gearLanguage: "언어",
      navSearch: "테이블과 필드 필터…",
      navDerivedOnly: "파생만",
      navNoMatch: "일치하는 항목이 없습니다.",
      groupOther: "기타",
      tabAll: "전체",
      tabDerived: "파생",
      tabStored: "저장",
      tabRelationships: "관계",
      viewGroups: "그룹",
      viewList: "A–Z",
      viewGraph: "그래프",
      tablesCount: (n) => `테이블 ${n}개`,
      graphHint: "각 상자는 테이블이며, 선은 한 테이블에서 그 테이블이 참조하는 테이블로 향합니다. 테이블을 누르면 열립니다.",
      filterFields: "필드 필터…",
      cellHintOn: "배지에 마우스를 올리면 미리 볼 수 있고, 더블클릭하면 전체 DAG 페이지가 열립니다",
      cellHintOff: "더블클릭하면 출처를 볼 수 있습니다(머리글에서 ƒ 표시를 켜고 끌 수 있음)",
      ontologyTitle: "온톨로지",
      ontologyLead: "룰북의 모든 테이블입니다. 테이블을 클릭하면 열을 볼 수 있습니다.",
      fieldsCount: (n) => `필드 ${n}개`, derivedCount: (n) => `계산 ${n}개`, relCount: (n) => `관계 ${n}개`,
      tableNotFound: "테이블을 찾을 수 없습니다.", fieldNotFound: "룰북에서 해당 필드를 찾을 수 없습니다.",
      nullable: "널 허용",
      hopsFromGround: (n) => `원천 데이터에서 ${n}홉`,
      relationshipPointer: "관계 포인터", groundTruth: "원천 데이터",
      relPointerBody: (link) => `이 필드는 ${link} 테이블을 가리키는 포인터입니다. 다른 값에서 파생되지 않으며, DAG의 나머지 부분이 이 링크에 연결됩니다.`,
      relPointerBodyNoLink: "이 필드는 다른 테이블을 가리키는 포인터입니다. 다른 값에서 파생되지 않으며, DAG의 나머지 부분이 이 링크에 연결됩니다.",
      groundTruthBody: (dt) => `이 셀은 말단입니다. 데이터베이스에 직접 <em>기록되는</em> 값(데이터 유형 <code>${dt}</code>)이며, 다른 값에서 계산되지 않습니다. 상위의 모든 값은 결국 이와 같은 값으로 거슬러 올라갑니다.`,
      openTable: (t) => `${t} 테이블 열기 →`,
      ruleCardTitle: "이 셀은 하나의 선언적 규칙의 값입니다",
      oneLineFnCardTitle: "이 셀은 하나의 한 줄 함수의 값입니다",
      inRuleSpeak: "문서 표현", inEnglish: "쉬운 말로", asFormula: "수식으로",
      mechanicalFlag: "⚠︎ 기계적 표현", mechanicalTitle: "정확하지만 어색한 표현 — 원하면 문장을 다듬어도 좋다는 표시",
      noRuleRendered: (t) => `이 필드(${t})에는 선언적 규칙이 생성되지 않았습니다.`,
      obligations: "의무 규칙",
      ruleSpeakHintFormula: "수식이 담고 있는 것과 같은 논리를 비즈니스 규칙으로 풀어 쓴 것입니다. 공용 문서 엔진이 룰북에서 생성했습니다.",
      ruleSpeakHintNoFormula: "공용 문서 엔진이 룰북에서 생성했습니다.",
      englishHint: "밑줄 그어진 각 필드도 그 자체로 한 줄 함수입니다. 클릭하면 자세히 볼 수 있습니다.",
      noFormulaSentence: (t) => `이 필드(${t})에는 문장으로 읽을 스프레드시트 수식이 없습니다. 관계를 바탕으로 구조적으로 정의된 집계입니다. 정의를 보려면 <strong>문서</strong>로 전환하십시오.`,
      formulaHint: "각 칩도 그 자체로 한 줄 함수입니다. 칩을 클릭하면 자세히 볼 수 있습니다.",
      noFormulaAtAll: (t) => `이 필드(${t})에는 스프레드시트 수식이 없습니다. 관계를 바탕으로 구조적으로 정의된 집계입니다. 정의를 보려면 <strong>문서</strong>로 전환하십시오.`,
      inputsTitle: "입력", inputsLead: "이 한 줄 함수가 호출하는 필드입니다.",
      groundTruthInline: (dt) => `원천 데이터 · 데이터 유형 ${dt}`,
      consumersTitle: "사용처",
      consumersDirect: "직접", consumersTransitive: "간접",
      consumersEmpty: "룰북에서 이 셀을 참조하는 항목은 아직 없습니다.",
      consumersLead: "한 줄 함수에서 이 필드를 호출하는 필드입니다.",
      showTransitive: (n) => `간접 사용처 ${n}개 표시(사용처의 사용처)`,
      leavesTitle: "말단의 원천 데이터",
      leavesLead: "이 셀의 값은 결국 아래의 원천 값에 의존합니다. 이 값 중 하나를 수정하고 다시 빌드하면 이 셀까지 자동으로 반영됩니다.",
      hoverGroundTruth: (dt) => `원천 데이터 — 데이터베이스에 직접 기록됩니다${dt ? `(데이터 유형 <code>${dt}</code>)` : ""}.`,
      hoverRelPointer: "관계 포인터 — DAG의 나머지 부분이 이 링크에 연결됩니다.",
      hoverNoRule: "생성된 선언적 규칙이 없습니다.",
      hoverNoFormula: "수식 없음 — 구조적 집계입니다. 문서를 참조하십시오.",
      hoverInputs: "입력",
      hoverHint: "셀을 더블클릭하면 전체 DAG 페이지가 열립니다.",
      hoverOpenPage: "전체 페이지 열기",
      // The witness (witness.js): one row's value and the values it came from.
      witnessTitle: "원천 데이터까지 확인한 증명",
      witnessLead: "한 레코드에 대한 이 셀의 값과, 그 값의 바탕이 된 모든 값을 저장된 값까지 거슬러 올라가며 보여 줍니다. 모든 값은 이 프로젝트에서 생성한 TypeScript(rulebook-to-typescript)가 방금 이 자리에서 계산한 것입니다. 항목을 열면 그 값이 무엇으로 만들어졌는지 볼 수 있습니다.",
      witnessRow: "레코드",
      witnessForRow: (label) => `${label} 레코드의 경우`,
      witnessComputing: "생성된 TypeScript로 이 레코드를 계산하는 중…",
      witnessNoRows: (t) => `${t} 테이블에는 증명할 레코드가 없습니다.`,
      witnessFailed: "증명을 계산하지 못했습니다.",
      witnessGroundTruth: "원천 데이터",
      witnessPointsTo: (t, label) => `${t} 테이블의 ${label} 레코드를 가리킵니다`,
      witnessPointsNowhere: (t) => `${t} 테이블의 어떤 레코드도 가리키지 않습니다`,
      witnessBlank: "비어 있음",
      witnessEmptyText: "빈 텍스트",
      witnessError: (m) => `계산할 수 없습니다: ${m}`,
      witnessStep: (n, of) => `${of}단계 중 ${n}단계를 적용했습니다.`,
      witnessNoStep: "적용된 단계가 없어 값이 비어 있습니다.",
      // `noun` is what one row of the table is called ("case"); "row" only when the
      // graph does not say (rowNoun).
      witnessStepMark: (noun) => (noun ? `이 레코드(${noun})에 적용` : "이 레코드에 적용"),
      witnessLookup: (key, value, t, label) => `${key} = ${value} 조건으로 조회: ${t} 테이블의 ${label} 레코드에서 읽었습니다.`,
      witnessLookupMiss: (key, value, t) => `${key} = ${value} 조건으로 조회했지만 ${t} 테이블에 해당하는 레코드가 없습니다.`,
      witnessOver: (op, n, t) => ({
        COUNTIFS: `개수: 조건에 맞는 ${t} 레코드는 ${n}건입니다.`,
        COUNT: `개수: 조건에 맞는 ${t} 레코드는 ${n}건입니다.`,
        SUM: `조건에 맞는 ${t} 레코드 ${n}건을 합산했습니다.`,
        AVERAGE: `조건에 맞는 ${t} 레코드 ${n}건의 평균입니다.`,
        MIN: `조건에 맞는 ${t} 레코드 ${n}건 중 최솟값입니다.`,
        MAX: `조건에 맞는 ${t} 레코드 ${n}건 중 최댓값입니다.`,
      }[op] || `조건에 맞는 ${t} 레코드 ${n}건으로 계산했습니다.`),
      witnessOverUnknown: "이 페이지에서 나열할 수 없는 레코드로 계산했습니다.",
      witnessMoreRows: (n) => `… 외 ${n}건`,
      witnessSeenAbove: "위에 이미 표시됨",
      type_raw: "원천 데이터", type_calculated: "계산", type_aggregation: "집계", type_relationship: "관계", type_lookup: "조회",
    },
    tr: {
      back: "← geri", backTitle: "Bir adım geri dön",
      home: "🏠 Ana sayfa", homeTitle: "Açıklayıcıdan çık — uygulamaya geri dön",
      tablesCrumb: "Tablolar",
      narrRulespeak: "Dokümantasyon", narrEnglish: "Yalın okuma", narrFormula: "Formül",
      narrRulespeakHint: "Bildirimsel iş kuralı (yalnızca … ise / zorundadır / öncelik)",
      narrEnglishHint: "Formülün yalın bir cümle olarak okunuşu",
      narrFormulaHint: "Tıklanabilir alanlarla birlikte ham, elektronik tablo tarzı formül",
      narrAriaLabel: "Bu kuralın nasıl açıklanacağı",
      gearTitle: "Ayarlar — üzerine gelince açılan kartta ve alan sayfasında nelerin gösterileceğini seçin",
      gearAriaLabel: "Ayarlar",
      gearPopTitle: "Açılır pencerede ve sayfada göster",
      gearReset: "Tümünü açık hale getir",
      elemDesc: "Açıklama", elemDescHint: "Alanın yalın dildeki açıklaması.",
      elemRulespeak: "Dokümantasyon", elemRulespeakHint: "Bildirimsel iş kuralı.",
      elemEnglish: "Yalın okuma", elemEnglishHint: "Formülün yalın bir cümle olarak okunuşu.",
      elemFormula: "Formül", elemFormulaHint: "Ham, elektronik tablo tarzı formül.",
      elemInputs: "Girdiler", elemInputsHint: "Bu tek satırlık işlevin çağırdığı alanlar.",
      elemConsumers: "Tüketiciler", elemConsumersHint: "İşlevi bu alanı çağıran alanlar.",
      provOn: "Köken · AÇIK", provOff: "Köken · kapalı",
      provOnTitle: "ƒ köken simgelerini gizle. Hücrelere çift tıklamak yine de DAG sayfasını açar.",
      provOffTitle: "İncelenebilir kökeni olan hücrelerde ƒ köken simgelerini göster.",
      downloadXlsx: "Çalışma kitabını indir", downloadXlsxTitle: "Kural kitabını Excel çalışma kitabı (.xlsx) olarak indir",
      gearShowAs: "Şöyle göster", gearLanguage: "Dil",
      navSearch: "Tabloları ve alanları süz…",
      navDerivedOnly: "Yalnızca türetilmiş",
      navNoMatch: "Eşleşen yok.",
      groupOther: "Diğer",
      tabAll: "Tümü",
      tabDerived: "Türetilmiş",
      tabStored: "Saklanan",
      tabRelationships: "İlişkiler",
      viewGroups: "Gruplar",
      viewList: "A–Z",
      viewGraph: "Grafik",
      tablesCount: (n) => `${n} tablo`,
      graphHint: "Her kutu bir tablodur; bir çizgi, bir tablodan başvurduğu tabloya uzanır. Açmak için bir tabloya tıklayın.",
      filterFields: "Alanları süz…",
      cellHintOn: "Önizlemek için rozetin üzerine gelin; tam DAG sayfasını açmak için çift tıklayın",
      cellHintOff: "Kökeni görmek için çift tıklayın (ƒ simgelerini başlıktan açıp kapatabilirsiniz)",
      ontologyTitle: "Ontoloji",
      ontologyLead: "Kural kitabındaki tüm tablolar. Sütunlarını görmek için bir tabloya tıklayın.",
      fieldsCount: (n) => `${n} alan`, derivedCount: (n) => `${n} türetilmiş`, relCount: (n) => `${n} ilişki`,
      tableNotFound: "Tablo bulunamadı.", fieldNotFound: "Alan kural kitabında bulunamadı.",
      nullable: "boş olabilir",
      hopsFromGround: (n) => (n === 1 ? "Temel veriden 1 adım uzakta" : `Temel veriden ${n} adım uzakta`),
      relationshipPointer: "İlişki referansı", groundTruth: "Temel veri",
      relPointerBody: (link) => `Bu alan, ${link} tablosuna giden bir referanstır. Başka hiçbir şeyden türetilmez — DAG yapısının geri kalanının bağlandığı bağlantıdır.`,
      relPointerBodyNoLink: "Bu alan, başka bir tabloya giden bir referanstır. Başka hiçbir şeyden türetilmez — DAG yapısının geri kalanının bağlandığı bağlantıdır.",
      groundTruthBody: (dt) => `Bu hücre bir yapraktır. Doğrudan veritabanına <em>yazılır</em> (veri türü <code>${dt}</code>) ve başka hiçbir şeyden hesaplanmaz. Yukarı akıştaki diğer her şey eninde sonunda bunun gibi değerlere dayanır.`,
      openTable: (t) => `${t} tablosunu aç →`,
      ruleCardTitle: "Bu hücre, bir bildirimsel kuralın değeridir",
      oneLineFnCardTitle: "Bu hücre, tek satırlık bir işlevin değeridir",
      inRuleSpeak: "Dokümantasyon ile", inEnglish: "Yalın sözlerle", asFormula: "Formül olarak",
      mechanicalFlag: "⚠︎ mekanik", mechanicalTitle: "Aslına sadık ama hantal ifade — isteğe bağlı bir yeniden yazım turu için işaret",
      noRuleRendered: (t) => `“${t}” tablosundaki bu alan için bildirimsel kural üretilmedi.`,
      obligations: "Yükümlülükler",
      ruleSpeakHintFormula: "Formülün kodladığı mantığın aynısı, iş kuralı olarak yazılmış hâliyle — kural kitabından ortak Dokümantasyon motoru tarafından üretilir.",
      ruleSpeakHintNoFormula: "Kural kitabından ortak Dokümantasyon motoru tarafından üretilir.",
      englishHint: "Altı çizili her alan kendi başına tek satırlık bir işlevdir. İncelemek için herhangi birine tıklayın.",
      noFormulaSentence: (t) => `“${t}” tablosundaki bu alanın cümle olarak okunabilecek bir elektronik tablo formülü yoktur — bir ilişki üzerinden yapısal olarak tanımlanmış bir özettir. Nasıl tanımlandığını görmek için <strong>Dokümantasyon</strong> görünümüne geçin.`,
      formulaHint: "Her etiket kendi başına tek satırlık bir işlevdir. İncelemek için herhangi bir etikete tıklayın.",
      noFormulaAtAll: (t) => `“${t}” tablosundaki bu alanın elektronik tablo formülü yoktur — bir ilişki üzerinden yapısal olarak tanımlanmış bir özettir. Nasıl tanımlandığını görmek için <strong>Dokümantasyon</strong> görünümüne geçin.`,
      inputsTitle: "Girdiler", inputsLead: "Bu tek satırlık işlevin çağırdığı alanlar.",
      groundTruthInline: (dt) => `Temel veri · veri türü ${dt}`,
      consumersTitle: "Tüketiciler",
      consumersDirect: "doğrudan", consumersTransitive: "dolaylı",
      consumersEmpty: "Kural kitabındaki hiçbir şey henüz bu hücreye başvurmuyor.",
      consumersLead: "Tek satırlık işlevi bu alanı çağıran alanlar.",
      showTransitive: (n) => `${n} dolaylı tüketiciyi göster (tüketicilerin tüketicileri)`,
      leavesTitle: "Yapraklardaki temel veri",
      leavesLead: "Bu hücrenin değeri eninde sonunda bu ham değerlere bağlıdır. Bunlardan herhangi birini düzenleyip yeniden derlemek, sonucu buraya otomatik olarak yansıtır.",
      hoverGroundTruth: (dt) => `Temel veri — doğrudan veritabanına yazılır${dt ? ` (veri türü <code>${dt}</code>)` : ""}.`,
      hoverRelPointer: "İlişki referansı — DAG yapısının geri kalanının bağlandığı bağlantı.",
      hoverNoRule: "Bildirimsel kural üretilmedi.",
      hoverNoFormula: "Formül yok — yapısal bir özet. Bkz. Dokümantasyon.",
      hoverInputs: "Girdiler",
      hoverHint: "Tam DAG sayfasını açmak için hücreye çift tıklayın.",
      hoverOpenPage: "Tam sayfayı aç",
      // The witness (witness.js): one row's value and the values it came from.
      witnessTitle: "Tanıklık edilmiş, temel veriye kadar",
      witnessLead: "Bu hücrenin tek bir satır için değeri ve geldiği her değer, saklanan değerlere kadar. Her biri burada, az önce, bu projenin üretilmiş TypeScript kodu (rulebook-to-typescript) tarafından hesaplandı. Neyden oluştuğunu görmek için bir satırı açın.",
      witnessRow: "Satır",
      witnessForRow: (label) => `“${label}” kaydı için`,
      witnessComputing: "Bu satır, üretilmiş TypeScript ile hesaplanıyor…",
      witnessNoRows: (t) => `“${t}” tablosunda tanıklık edilecek satır yok.`,
      witnessFailed: "Tanıklık hesaplanamadı.",
      witnessGroundTruth: "temel veri",
      witnessPointsTo: (t, label) => `“${t}” tablosundaki “${label}” kaydına işaret eder`,
      witnessPointsNowhere: (t) => `“${t}” tablosunda hiçbir satıra işaret etmez`,
      witnessBlank: "boş",
      witnessEmptyText: "boş metin",
      witnessError: (m) => `hesaplanamadı: ${m}`,
      witnessStep: (n, of) => `Adım ${n} / ${of} uygulandı.`,
      witnessNoStep: "Hiçbir adım uygulanmadı; bu yüzden değer boş.",
      // `noun` is what one row of the table is called ("case"); "row" only when the
      // graph does not say (rowNoun).
      witnessStepMark: (noun) => (noun ? `bu “${noun}” kaydına uygulandı` : "bu satıra uygulandı"),
      witnessLookup: (key, value, t, label) => `“${key}” = ${value} üzerinden aranıp “${t}” tablosundaki “${label}” kaydından okundu.`,
      witnessLookupMiss: (key, value, t) => `“${key}” = ${value} üzerinden arandı; bu değer “${t}” tablosunda hiçbir satıra işaret etmiyor.`,
      witnessOver: (op, n, t) => ({
        COUNTIFS: `Sayıldı: “${t}” tablosunda ${n} satır eşleşiyor.`,
        COUNT: `Sayıldı: “${t}” tablosunda ${n} satır eşleşiyor.`,
        SUM: `“${t}” tablosunda eşleşen ${n} satır üzerinden toplandı.`,
        AVERAGE: `“${t}” tablosunda eşleşen ${n} satır üzerinden ortalaması alındı.`,
        MIN: `“${t}” tablosunda eşleşen ${n} satır arasındaki en küçük değer.`,
        MAX: `“${t}” tablosunda eşleşen ${n} satır arasındaki en büyük değer.`,
      }[op] || `“${t}” tablosunda eşleşen ${n} satır üzerinden hesaplandı.`),
      witnessOverUnknown: "Bu sayfanın listeleyemediği satırlar üzerinden hesaplandı.",
      witnessMoreRows: (n) => `… ve ${n} satır daha`,
      witnessSeenAbove: "yukarıda zaten gösterildi",
      type_raw: "Temel veri", type_calculated: "Hesaplanan", type_aggregation: "Özet", type_relationship: "İlişki", type_lookup: "Arama",
    },
    vi: {
      back: "← quay lại", backTitle: "Quay lại một bước",
      home: "🏠 Trang chủ", homeTitle: "Rời trang giải thích — quay lại ứng dụng",
      tablesCrumb: "Bảng",
      narrRulespeak: "Tài liệu", narrEnglish: "Cách đọc thông thường", narrFormula: "Công thức",
      narrRulespeakHint: "Quy tắc nghiệp vụ dạng khai báo (chỉ khi / phải / thứ tự ưu tiên)",
      narrEnglishHint: "Công thức được đọc thành một câu thông thường",
      narrFormulaHint: "Công thức gốc kiểu bảng tính, kèm các trường bấm được",
      narrAriaLabel: "Cách giải thích quy tắc này",
      gearTitle: "Cài đặt — chọn nội dung hiển thị trong thẻ xem nhanh và trên trang của trường",
      gearAriaLabel: "Cài đặt",
      gearPopTitle: "Hiển thị trong thẻ xem nhanh và trên trang",
      gearReset: "Bật lại tất cả",
      elemDesc: "Mô tả", elemDescHint: "Mô tả của trường bằng lời thông thường.",
      elemRulespeak: "Tài liệu", elemRulespeakHint: "Quy tắc nghiệp vụ dạng khai báo.",
      elemEnglish: "Cách đọc thông thường", elemEnglishHint: "Công thức được đọc thành một câu thông thường.",
      elemFormula: "Công thức", elemFormulaHint: "Công thức gốc kiểu bảng tính.",
      elemInputs: "Đầu vào", elemInputsHint: "Các trường mà hàm một dòng này gọi đến.",
      elemConsumers: "Nơi sử dụng", elemConsumersHint: "Các trường có hàm gọi đến trường này.",
      provOn: "Nguồn gốc · BẬT", provOff: "Nguồn gốc · tắt",
      provOnTitle: "Ẩn các ký hiệu nguồn gốc ƒ. Bấm đúp vào ô vẫn mở được DAG.",
      provOffTitle: "Hiện ký hiệu nguồn gốc ƒ trên những ô có nguồn gốc xem được.",
      downloadXlsx: "Tải sổ làm việc", downloadXlsxTitle: "Tải bộ quy tắc về dưới dạng sổ làm việc Excel (.xlsx)",
      gearShowAs: "Hiển thị dạng", gearLanguage: "Ngôn ngữ",
      navSearch: "Lọc bảng và trường…",
      navDerivedOnly: "Chỉ trường suy ra",
      navNoMatch: "Không có kết quả.",
      groupOther: "Khác",
      tabAll: "Tất cả",
      tabDerived: "Suy ra",
      tabStored: "Lưu trữ",
      tabRelationships: "Quan hệ",
      viewGroups: "Nhóm",
      viewList: "A–Z",
      viewGraph: "Đồ thị",
      tablesCount: (n) => `${n} bảng`,
      graphHint: "Mỗi ô là một bảng; một đường nối từ bảng này tới bảng mà nó tham chiếu. Nhấp vào một bảng để mở.",
      filterFields: "Lọc trường…",
      cellHintOn: "Rê chuột lên ký hiệu để xem nhanh; bấm đúp để mở trang DAG đầy đủ",
      cellHintOff: "Bấm đúp để xem nguồn gốc (bật hoặc tắt ký hiệu ƒ ở phần đầu trang)",
      ontologyTitle: "Bản thể luận",
      ontologyLead: "Mọi bảng trong bộ quy tắc. Bấm vào một bảng để xem các cột của bảng đó.",
      fieldsCount: (n) => `${n} trường`, derivedCount: (n) => `${n} dẫn xuất`, relCount: (n) => `${n} quan hệ`,
      tableNotFound: "Không tìm thấy bảng.", fieldNotFound: "Không tìm thấy trường trong bộ quy tắc.",
      nullable: "cho phép trống",
      hopsFromGround: (n) => `cách dữ liệu gốc ${n} bước`,
      relationshipPointer: "Con trỏ quan hệ", groundTruth: "Dữ liệu gốc",
      relPointerBody: (link) => `Trường này là một con trỏ tới ${link}. Nó không được suy ra từ bất kỳ giá trị nào khác — đây là mối liên kết mà phần còn lại của DAG bám vào.`,
      relPointerBodyNoLink: "Trường này là một con trỏ tới một bảng khác. Nó không được suy ra từ bất kỳ giá trị nào khác — đây là mối liên kết mà phần còn lại của DAG bám vào.",
      groundTruthBody: (dt) => `Ô này là một nút lá. Giá trị của nó được <em>ghi</em> trực tiếp vào cơ sở dữ liệu (kiểu dữ liệu <code>${dt}</code>) và không được tính từ bất kỳ giá trị nào khác. Mọi giá trị khác ở phía trên rốt cuộc đều truy ngược về những giá trị như thế này.`,
      openTable: (t) => `Mở bảng ${t} →`,
      ruleCardTitle: "Ô này là giá trị của một quy tắc khai báo",
      oneLineFnCardTitle: "Ô này là giá trị của một hàm một dòng",
      inRuleSpeak: "Theo Tài liệu", inEnglish: "Bằng lời thông thường", asFormula: "Dưới dạng công thức",
      mechanicalFlag: "⚠︎ máy móc", mechanicalTitle: "Cách diễn đạt trung thành nhưng còn vụng — gợi ý cho một lượt biên tập lại (không bắt buộc)",
      // `t` is the field kind as the rulebook spells it ("lookup"), so it follows the head noun "loại".
      noRuleRendered: (t) => `Không có quy tắc khai báo nào được sinh ra cho trường thuộc loại ${t} này.`,
      obligations: "Nghĩa vụ",
      ruleSpeakHintFormula: "Đây chính là logic mà công thức thể hiện, được viết thành một quy tắc nghiệp vụ — do bộ máy Tài liệu dùng chung sinh ra từ bộ quy tắc.",
      ruleSpeakHintNoFormula: "Do bộ máy Tài liệu dùng chung sinh ra từ bộ quy tắc.",
      englishHint: "Mỗi trường được gạch chân cũng là một hàm một dòng. Bấm vào trường bất kỳ để xem chi tiết.",
      noFormulaSentence: (t) => `Trường thuộc loại ${t} này không có công thức bảng tính nào để đọc thành câu — đây là một phép tổng hợp được định nghĩa theo cấu trúc trên một quan hệ. Chuyển sang <strong>Tài liệu</strong> để xem định nghĩa của nó.`,
      formulaHint: "Mỗi thẻ cũng là một hàm một dòng. Bấm vào thẻ bất kỳ để xem chi tiết.",
      noFormulaAtAll: (t) => `Trường thuộc loại ${t} này không có công thức bảng tính — đây là một phép tổng hợp được định nghĩa theo cấu trúc trên một quan hệ. Chuyển sang <strong>Tài liệu</strong> để xem định nghĩa của nó.`,
      inputsTitle: "Đầu vào", inputsLead: "Các trường mà hàm một dòng này gọi đến.",
      groundTruthInline: (dt) => `Dữ liệu gốc · kiểu dữ liệu ${dt}`,
      consumersTitle: "Nơi sử dụng",
      consumersDirect: "trực tiếp", consumersTransitive: "bắc cầu",
      consumersEmpty: "Chưa có nơi nào khác trong bộ quy tắc tham chiếu đến ô này.",
      consumersLead: "Các trường có hàm một dòng gọi đến trường này.",
      showTransitive: (n) => `Hiện ${n} nơi sử dụng bắc cầu (nằm sau các nơi sử dụng trực tiếp)`,
      leavesTitle: "Dữ liệu gốc ở các nút lá",
      leavesLead: "Rốt cuộc, giá trị của ô này phụ thuộc vào các giá trị gốc sau đây. Khi sửa bất kỳ giá trị nào trong số đó rồi build lại, thay đổi sẽ tự động lan đến đây.",
      hoverGroundTruth: (dt) => `Dữ liệu gốc — được ghi trực tiếp vào cơ sở dữ liệu${dt ? ` (kiểu dữ liệu <code>${dt}</code>)` : ""}.`,
      hoverRelPointer: "Con trỏ quan hệ — mối liên kết mà phần còn lại của DAG bám vào.",
      hoverNoRule: "Không có quy tắc khai báo nào được sinh ra.",
      hoverNoFormula: "Không có công thức — đây là một phép tổng hợp theo cấu trúc. Xem Tài liệu.",
      hoverInputs: "Đầu vào",
      hoverHint: "Bấm đúp vào ô để mở trang DAG đầy đủ.",
      hoverOpenPage: "Mở trang đầy đủ",
      // The witness (witness.js): one row's value and the values it came from.
      // Vietnamese nouns do not change with a count, and every variable term follows a
      // fixed head noun (bảng, bản ghi, trường), so no wording depends on `n` or on the term.
      witnessTitle: "Đã kiểm chứng đến tận dữ liệu gốc",
      witnessLead: "Giá trị của ô này đối với một bản ghi, cùng mọi giá trị đã tạo nên nó, truy đến tận các giá trị được lưu trữ. Mỗi giá trị đều vừa được tính ngay tại đây bằng mã TypeScript được sinh ra của dự án này (rulebook-to-typescript). Mở một dòng để xem nó được tạo nên từ những gì.",
      witnessRow: "Bản ghi",
      witnessForRow: (label) => `Đối với bản ghi ${label}`,
      witnessComputing: "Đang tính bản ghi này bằng mã TypeScript được sinh ra…",
      witnessNoRows: (t) => `Bảng ${t} không có bản ghi nào để kiểm chứng.`,
      witnessFailed: "Không tính được phần kiểm chứng.",
      witnessGroundTruth: "dữ liệu gốc",
      witnessPointsTo: (t, label) => `trỏ tới bản ghi ${label} của bảng ${t}`,
      witnessPointsNowhere: (t) => `không trỏ tới bản ghi nào của bảng ${t}`,
      witnessBlank: "trống",
      witnessEmptyText: "chuỗi rỗng",
      witnessError: (m) => `không tính được: ${m}`,
      witnessStep: (n, of) => `Đã áp dụng bước ${n} trong tổng số ${of} bước.`,
      witnessNoStep: "Không bước nào được áp dụng, nên giá trị để trống.",
      // `noun` is the bare subject term of the rule sentences ("Mỗi customer …",
      // "customer đó"); "bản ghi" only when the graph does not say.
      witnessStepMark: (noun) => `áp dụng cho ${noun || "bản ghi"} này`,
      witnessLookup: (key, value, t, label) => `Tra cứu qua trường “${key}” = ${value}: đọc từ bản ghi ${label} của bảng ${t}.`,
      witnessLookupMiss: (key, value, t) => `Tra cứu qua trường “${key}” = ${value}, nhưng bảng ${t} không có bản ghi nào như vậy.`,
      witnessOver: (op, n, t) => ({
        COUNTIFS: `Đã đếm: bảng ${t} có ${n} bản ghi khớp.`,
        COUNT: `Đã đếm: bảng ${t} có ${n} bản ghi khớp.`,
        SUM: `Tính tổng trên ${n} bản ghi khớp của bảng ${t}.`,
        AVERAGE: `Lấy trung bình trên ${n} bản ghi khớp của bảng ${t}.`,
        MIN: `Giá trị nhỏ nhất trên ${n} bản ghi khớp của bảng ${t}.`,
        MAX: `Giá trị lớn nhất trên ${n} bản ghi khớp của bảng ${t}.`,
      }[op] || `Được tính trên ${n} bản ghi khớp của bảng ${t}.`),
      witnessOverUnknown: "Được tính trên những bản ghi mà trang này không liệt kê được.",
      witnessMoreRows: (n) => `… và ${n} bản ghi khác`,
      witnessSeenAbove: "đã hiển thị ở trên",
      type_raw: "Dữ liệu gốc", type_calculated: "Được tính", type_aggregation: "Tổng hợp", type_relationship: "Quan hệ", type_lookup: "Tra cứu",
    },
  };
  function tt(key, ...args) {
    const dict = CHROME[lang] || CHROME.en;
    const entry = key in dict ? dict[key] : CHROME.en[key];
    return typeof entry === "function" ? entry(...args) : entry;
  }
  // What ONE row of a table is called: the rules' own noun for it ("case" for Cases,
  // the word in "The case's case status is ..."), so a sentence about a row never has
  // to say "this row" (owner, 2026-11-22). Taken in the language `key`'s words are
  // written in, which is English wherever this language has no words for it yet.
  // ── Language ────────────────────────────────────────────────────────────
  //  The languages a page can be in are the ones the build baked (-p languages=...): the
  //  page's own texts exist in all of them, but the rules are only in the baked ones, and a
  //  page must not say German around English rules. The choice is remembered, and an
  //  address's ?lang= wins for the page it opens.
  const LANG_KEY = "effortless-explainer-lang";
  const NATIVE = {
    en: "English", ja: "日本語", de: "Deutsch", es: "Español", fr: "Français", pt: "Português", it: "Italiano", nl: "Nederlands",
    ru: "Русский", zh: "中文", hi: "हिन्दी", ar: "العربية", id: "Bahasa Indonesia", ko: "한국어", tr: "Türkçe", vi: "Tiếng Việt",
  };
  function availableLanguages() {
    const baked = Object.keys((global.__EFFORTLESS_EXPLAINER__ || {}).rulespeakByLang || {});
    const have = Object.keys(CHROME).filter((c) => baked.includes(c));
    return have.length ? have : ["en"];
  }
  function preferredLanguage() {
    const ok = (c) => c && CHROME[c] && availableLanguages().includes(c);
    let asked = null;
    try { asked = (/[?&]lang=([A-Za-z-]+)/.exec(window.location.search || "") || [])[1] || null; } catch { /* no address */ }
    if (ok(asked)) return asked;
    let kept = null;
    try { kept = localStorage.getItem(LANG_KEY); } catch { /* no storage */ }
    return ok(kept) ? kept : "en";
  }
  function setLanguage(code) {
    if (!CHROME[code] || !availableLanguages().includes(code)) {
      throw new Error(`EffortlessExplainer.setLanguage: "${code}" is not baked into this page. It has: ${availableLanguages().join(", ")}. Build with -p languages=all (or a list) to offer more.`);
    }
    lang = code;
    try { localStorage.setItem(LANG_KEY, code); } catch { /* no storage: this page only */ }
    routing = null; ensureRouting();      // its links to the explainer's own page carry the language
    // The page is ours to dress only when it is the explainer's own (it has the outlet); a
    // host's page keeps its own <html lang>.
    if (document.getElementById && document.getElementById("effortless-explainer-outlet")) {
      document.documentElement.lang = code;
      document.documentElement.dir = RTL.has(code) ? "rtl" : "ltr";
      try {
        const u = new URL(window.location.href); u.searchParams.set("lang", code);
        window.history.replaceState(null, "", u.toString());
      } catch { /* no address to carry it */ }
    }
    window.dispatchEvent(new Event(PREFS_EVENT));   // the page, the corner control: say it again
  }

  function rowNoun(table, key) {
    if (!table) return null;
    const said = CHROME[lang] && key in CHROME[lang] ? lang : "en";
    return R().ruleSpeakNoun(table, said);
  }

  // Languages written right to left. Everything the explainer draws carries its own
  // lang and dir, so the host page's language and direction do not decide them; the
  // stylesheet is direction-neutral (inline-start / -end) and keeps formulas left to right.
  const RTL = new Set(["ar"]);
  function speaks(el) {
    if (el && el.setAttribute) { el.setAttribute("lang", lang); el.setAttribute("dir", RTL.has(lang) ? "rtl" : "ltr"); }
    return el;
  }
  // The arrow that points onward, the way the language reads.
  const onward = () => (RTL.has(lang) ? "←" : "→");

  let config = { mode: "hash", basePath: "/rulebook-explainer-dag" };
  // Where this file was loaded from (read now: currentScript is only set while it runs).
  const SELF_BASE = (function () {
    try { return (document.currentScript && document.currentScript.src || "").replace(/\/[^/]*$/, ""); } catch { return ""; }
  })();
  let routing = null;
  const CLOSE_DELAY_MS = 220;
  const CARD_W = 360;
  const SAFETY = 15;
  const GAP = 8;
  const MARGIN = 12;
  const EST_CARD_H = 300;
  let hoverCloseTimer = null;
  let activeHoverCard = null;

  // The page currently on screen, so a prefs change (narration / gear / glyphs)
  // can re-render IT — host-agnostic. Outlet/hash hosts and framework hosts that
  // render into their own container (e.g. the Angular DagFieldComponent's <div>)
  // both work, because we replay the exact last render call rather than relying
  // on a global outlet or the URL hash.
  let lastRender = null; // { kind: "index"|"table"|"field", container, table, field }
  function rerenderCurrent() {
    if (!lastRender || !lastRender.container || !lastRender.container.isConnected) return false;
    const { kind, container, table, field } = lastRender;
    if (kind === "field") renderFieldPage(container, table, field);
    else if (kind === "table") renderTablePage(container, table);
    else renderTablesIndex(container);
    return true;
  }

  function esc(s) {
    const d = document.createElement("div");
    d.textContent = s == null ? "" : String(s);
    return d.innerHTML;
  }
  function humanizeField(name) { return R().humanizeField(name); }

  // Guarantee `routing` exists before any renderer runs. A host that calls a
  // render method (e.g. renderFieldPage) BEFORE init() — common with framework
  // routers that mount a component on hard-load before the explainer's own
  // init has run — would otherwise hit a null `routing` and render nothing (the
  // "works only after navigating away and back" bug). We lazily build a default
  // hash-routing so the first paint always works; a later init() with custom
  // routing replaces it.
  // The folder this explainer is served from: where its own pages are.
  function ownBase() {
    const named = config && config.basePath && config.basePath !== "/rulebook-explainer-dag" ? config.basePath : null;
    return String(named || SELF_BASE || (config && config.basePath) || "/rulebook-explainer-dag").replace(/\/$/, "");
  }
  function buildRouting() {
    return global.EffortlessExplainerRouting.createRouting({
      mode: (config && config.mode) || "hash",
      lang,
      ownBase: ownBase(),
      ownPage: config && config.ownPage,
      hashPrefix: config && config.hashPrefix,
      fieldPath: config && config.fieldPath,
      tablePath: config && config.tablePath,
      indexPath: config && config.indexPath,
      homePath: config && config.homePath,
      htmxFieldUrl: config && config.htmxFieldUrl,
      routing: config && config.routing,
    });
  }
  function ensureRouting() {
    if (routing) return routing;
    routing = buildRouting();
    return routing;
  }

  // ════════════════════════════════════════════════════════════════════════
  //  PREFS — single source of truth (localStorage + a custom event), read
  //  synchronously so the first paint is already correct. Mirrors React
  //  lib/dagPrefs.ts plus the doc-element gating the gear controls.
  // ════════════════════════════════════════════════════════════════════════
  const PREFS_EVENT = "dag-prefs-changed";
  // Kept as the original key so existing hosts (e.g. the demo) stay compatible.
  const GLYPHS_KEY = "effortless-explainer-glyphs-on";
  const MODE_KEY = "dag.narrationMode";
  const ELEM_KEY = "dag.docElements";

  const NARRATION_MODES = ["rulespeak", "english", "formula"];
  const DEFAULT_MODE = "rulespeak";

  // The independent doc-element toggles the gear exposes. `key` drives the
  // page container class (show-<key>) and the hover-card section visibility.
  const DOC_ELEMENTS = [
    { key: "desc",      labelKey: "elemDesc",      hintKey: "elemDescHint" },
    { key: "rulespeak", labelKey: "elemRulespeak", hintKey: "elemRulespeakHint" },
    { key: "english",   labelKey: "elemEnglish",   hintKey: "elemEnglishHint" },
    { key: "formula",   labelKey: "elemFormula",   hintKey: "elemFormulaHint" },
    { key: "inputs",    labelKey: "elemInputs",    hintKey: "elemInputsHint" },
    { key: "consumers", labelKey: "elemConsumers", hintKey: "elemConsumersHint" },
  ];
  // Inputs defaults OFF: the RuleSpeak®/English/Formula card right above the
  // field page's Inputs section already names and links every one of those
  // same fields, so showing them again by default is pure repetition. The
  // gear still offers it for whoever wants the grid view.
  const DEFAULT_ELEMS = { desc: true, rulespeak: true, english: true, formula: true, inputs: false, consumers: true };

  const prefs = (function () {
    // The person's own choice once they have made one; until then what the app
    // asked for (init({ provenance: "off" })), and ON when it asked for nothing.
    function glyphsOn() {
      const chosen = localStorage.getItem(GLYPHS_KEY);
      if (chosen === "true" || chosen === "false") return chosen === "true";
      return config.provenance !== "off";
    }
    function setGlyphs(on) {
      localStorage.setItem(GLYPHS_KEY, on ? "true" : "false");
      fire();
    }
    function narrationMode() {
      const v = localStorage.getItem(MODE_KEY);
      return v && NARRATION_MODES.includes(v) ? v : DEFAULT_MODE;
    }
    function setNarrationMode(mode) {
      if (!NARRATION_MODES.includes(mode)) return;
      localStorage.setItem(MODE_KEY, mode);
      fire();
    }
    function docElements() {
      try {
        const raw = JSON.parse(localStorage.getItem(ELEM_KEY) || "null");
        if (raw && typeof raw === "object") return Object.assign({}, DEFAULT_ELEMS, raw);
      } catch { /* fall through */ }
      return Object.assign({}, DEFAULT_ELEMS);
    }
    function setDocElement(key, on) {
      const next = docElements();
      next[key] = !!on;
      localStorage.setItem(ELEM_KEY, JSON.stringify(next));
      fire();
    }
    function resetDocElements() {
      localStorage.removeItem(ELEM_KEY);
      fire();
    }
    // The space-separated `show-<key>` classes for the page container, computed
    // synchronously so the field page paints with the right sections up front.
    function docElementClasses() {
      const e = docElements();
      return DOC_ELEMENTS.filter((d) => e[d.key]).map((d) => `show-${d.key}`).join(" ");
    }
    function fire() { window.dispatchEvent(new Event(PREFS_EVENT)); }
    function subscribe(cb) {
      window.addEventListener(PREFS_EVENT, cb);
      window.addEventListener("storage", cb);
      return () => {
        window.removeEventListener(PREFS_EVENT, cb);
        window.removeEventListener("storage", cb);
      };
    }
    return {
      glyphsOn, setGlyphs, narrationMode, setNarrationMode,
      docElements, setDocElement, resetDocElements, docElementClasses, subscribe,
    };
  })();

  // ════════════════════════════════════════════════════════════════════════
  //  SHARED RENDER HELPERS
  // ════════════════════════════════════════════════════════════════════════

  function ruleSpeakFor(table, field) { return R().ruleSpeakForField(table, field, lang); }
  // A formula read as a plain sentence, in the page's language: the engine's reading,
  // baked at build time for every language but English, with that language's own
  // sentence end. English (and a language the build did not bake) is read by the
  // page itself (dag-resolver.js renderEnglish).
  function plainReading(table, field, formula) {
    const baked = lang !== "en" ? R().plainReadingForField(table, field, lang) : null;
    if (baked) return { text: baked, end: R().ruleSpeakConnectors(lang).sentenceEnd || "." };
    const english = R().englishForFormula(formula);
    return english ? { text: english, end: "." } : null;
  }

  function renderFieldLink(table, field, label, className) {
    const a = document.createElement("a");
    a.href = routing.fieldHref(table, field);
    a.className = className || "dag-link";
    a.textContent = label;
    a.addEventListener("click", (e) => {
      if (e.metaKey || e.ctrlKey || e.shiftKey) return;
      e.preventDefault();
      routing.navigate(table, field);
    });
    return a;
  }
  function renderTableLink(table, label, className) {
    const a = document.createElement("a");
    a.href = routing.tableHref(table);
    a.className = className || "dag-link";
    a.textContent = label;
    a.addEventListener("click", (e) => {
      if (e.metaKey || e.ctrlKey || e.shiftKey) return;
      e.preventDefault();
      routing.navigateTable(table);
    });
    return a;
  }

  // ── Markdown emphasis (**kw**, *em*) → HTML, with field-ref linkification.
  // Mirrors React lib/rulespeak.ts (renderRuleMarkdown / linkifyText / renderRuleRich).
  function indexOfWord(hay, needle) {
    const lc = hay.toLowerCase(), target = needle.toLowerCase();
    let from = 0;
    while (from <= lc.length - target.length) {
      const idx = lc.indexOf(target, from);
      if (idx < 0) return -1;
      const before = idx === 0 ? "" : lc[idx - 1];
      const after = idx + target.length >= lc.length ? "" : lc[idx + target.length];
      if ((!before || !/[a-z0-9]/i.test(before)) && (!after || !/[a-z0-9]/i.test(after))) return idx;
      from = idx + 1;
    }
    return -1;
  }
  // Returns HTML string. refs become clickable fc-inline spans.
  function linkifyHtml(text, refs) {
    if (!text) return "";
    const sorted = (refs || []).filter((r) => r.label).slice().sort((a, b) => b.label.length - a.label.length);
    if (!sorted.length) return esc(text);
    let out = "", rest = text;
    while (true) {
      let bestIdx = -1, bestRef = null;
      for (const r of sorted) {
        const idx = indexOfWord(rest, r.label);
        if (idx >= 0 && (bestIdx === -1 || idx < bestIdx)) { bestIdx = idx; bestRef = r; if (idx === 0) break; }
      }
      if (bestIdx === -1 || !bestRef) { out += esc(rest); break; }
      if (bestIdx > 0) out += esc(rest.slice(0, bestIdx));
      const matched = rest.slice(bestIdx, bestIdx + bestRef.label.length);
      const tone = bestRef.tone || "calc";
      out += `<a class="fc-inline fc-inline-${tone}" href="${esc(refHref(bestRef.table, bestRef.field))}" data-ref-table="${esc(bestRef.table)}" data-ref-field="${esc(bestRef.field)}">${esc(matched)}</a>`;
      rest = rest.slice(bestIdx + bestRef.label.length);
    }
    return out;
  }
  // Bold/italic markdown + ref linkification inside the plain segments.
  function renderRuleRichHtml(md, refs) {
    if (!md) return "";
    const re = /\*\*([^*]+)\*\*|\*([^*]+)\*/g;
    let out = "", last = 0, m;
    while ((m = re.exec(md)) !== null) {
      if (m.index > last) out += linkifyHtml(md.slice(last, m.index), refs);
      if (m[1] !== undefined) out += `<strong class="dag-rs-kw">${esc(m[1])}</strong>`;
      else if (m[2] !== undefined) out += `<em>${esc(m[2])}</em>`;
      last = re.lastIndex;
    }
    if (last < md.length) out += linkifyHtml(md.slice(last), refs);
    return out;
  }
  // Tag each rulespeak ref with the referenced field's tone so links are color-coded.
  function tonedRefs(rs) {
    if (!rs || !rs.refs) return [];
    return rs.refs.map((r) => {
      const f = R().fieldOf(R().rulebook(), r.table, r.field);
      return Object.assign({}, r, { tone: R().typeTone((f && f.type) || "raw") });
    });
  }

  // ── Formula AST → HTML (clickable chips). Mirrors lib/renderJsx.tsx. ─────────
  const BINOP_LABEL = { "+": "+", "-": "−", "*": "×", "/": "÷", "&": "&", "=": "=", "<>": "≠", "<": "<", ">": ">", "<=": "≤", ">=": "≥" };
  function fmtNum(n) {
    if (Number.isInteger(n) && Math.abs(n) >= 1000) return n.toLocaleString("en-US");
    return String(n);
  }
  // Real href for a ref link (so chips/pills/inline refs behave like links —
  // cursor, focus, cmd/middle-click open-in-new-tab — and don't go dead if a JS
  // handler ever fails to bind). The click handler still intercepts for SPA nav.
  function refHref(table, field) {
    try { return (routing && routing.fieldHref) ? routing.fieldHref(table, field) : `#/dag/${encodeURIComponent(table)}/${encodeURIComponent(field)}`; }
    catch { return `#/dag/${encodeURIComponent(table)}/${encodeURIComponent(field)}`; }
  }
  function tableHrefSafe(table) {
    try { return (routing && routing.tableHref) ? routing.tableHref(table) : `#/dag/${encodeURIComponent(table)}`; }
    catch { return `#/dag/${encodeURIComponent(table)}`; }
  }
  function chipHtml(table, field, pageTable) {
    const node = R().fieldOf(R().rulebook(), table, field);
    const type = (node && node.type) || "raw";
    const tone = R().typeTone(type);
    const glyph = R().typeGlyph(type);
    const showTable = table && table !== pageTable;
    return `<a class="fc fc-${tone} fc-chip" href="${esc(refHref(table, field))}" data-ref-table="${esc(table)}" data-ref-field="${esc(field)}">` +
      `<span class="fc-glyph">${esc(glyph)}</span>` +
      (showTable ? `<span class="fc-table">${esc(table)}.</span>` : "") +
      `<span class="fc-name">${esc(humanizeField(field))}</span></a>`;
  }
  function formulaNodeHtml(node, pageTable, top) {
    switch (node.kind) {
      case "ref": return chipHtml(node.table || pageTable, node.field, pageTable);
      case "num": return `<span class="fx-num">${esc(fmtNum(node.value))}</span>`;
      case "str": return `<span class="fx-str">“${esc(node.value)}”</span>`;
      case "bool": return `<span class="fx-bool">${node.value ? "TRUE" : "FALSE"}</span>`;
      case "unary": return `−${formulaNodeHtml(node.arg, pageTable, false)}`;
      case "binop": {
        const inner = `${formulaNodeHtml(node.left, pageTable, false)} <span class="fx-op">${esc(BINOP_LABEL[node.op] || node.op)}</span> ${formulaNodeHtml(node.right, pageTable, false)}`;
        return top ? inner : `(${inner})`;
      }
      case "call": {
        const args = node.args.map((a, i) =>
          (i > 0 ? `<span class="fx-comma">,&nbsp;</span>` : "") + formulaNodeHtml(a, pageTable, false)).join("");
        return `<span class="fx-fn">${esc(node.fn.toLowerCase())}</span><span class="fx-paren">(</span>${args}<span class="fx-paren">)</span>`;
      }
    }
    return "";
  }
  function formulaHtml(formula, pageTable) {
    const ast = R().tryParseFormula(formula);
    if (!ast) return `<code class="fx-raw">${esc(formula)}</code>`;
    return `<span class="fx"><span class="fx-eq">= </span>${formulaNodeHtml(ast, pageTable, true)}</span>`;
  }

  // ── RuleTree (priority ladders / AND-OR) → HTML. Mirrors components/RuleTree.tsx.
  function ruleTreeHtml(node, refs, step, table) {
    // Connector words come from the language pack (baked per language into
    // embedded-graph.js) — the same words the RuleSpeak® document uses.
    const cx = R().ruleSpeakConnectors(lang);
    function group(n) {
      const label = `${n.kind === "all" ? cx.allOf : cx.anyOf}:`;
      const items = (n.children || []).map((ch) => `<li class="dag-rt-child">${cond(ch)}</li>`).join("");
      return `<span class="dag-rt-group dag-rt-${n.kind}"><span class="dag-rt-group-label">${esc(label)}</span><ul class="dag-rt-children">${items}</ul></span>`;
    }
    function cond(n) {
      if (n.kind === "leaf") return `<span class="dag-rt-leaf">${linkifyHtml(n.text || "", refs)}</span>`;
      return group(n);
    }
    function val(text) { return `<strong class="dag-rt-val">${linkifyHtml(text || "", refs)}</strong>`; }
    function render(n) {
      if (n.kind === "priority") {
        let html = "";
        if (n.headline) html += `<p class="dag-rt-headline">${linkifyHtml(n.headline, refs)}</p>`;
        if (n.cases && n.cases.length) {
          html += "<ol class=\"dag-rt-cases\">";
          // `step` (the witness): the step that applied to the row being witnessed is
          // marked, the ones before it (tried first, did not hold) and after it (never
          // reached) are dimmed. Only when the outline has the engine's count of steps.
          const marks = n === node && step && step.steps === n.cases.length && step.index != null;
          n.cases.forEach((c, i) => {
            const isElse = c.when == null;
            const mark = !marks ? "" : i === step.index ? " dag-rt-fired" : " dag-rt-passed";
            const badge = marks && i === step.index ? `<span class="dag-rt-fired-mark">✓ ${esc(tt("witnessStepMark", rowNoun(table, "witnessStepMark")))}</span>` : "";
            // Word order mirrors HtmlDocRenderer (value, then the connector) — the
            // packs' connector words were written for that position.
            if (isElse) html += `<li class="dag-rt-case dag-rt-else${mark}"><span class="dag-rt-case-val">${val(c.value)}</span><span class="dag-rt-if"> ${esc(cx.otherwise)}</span>${badge}</li>`;
            else html += `<li class="dag-rt-case${mark}"><span class="dag-rt-case-val">${val(c.value)}</span><span class="dag-rt-if"> ${esc(cx.if)} </span>${cond(c.when)}${badge}</li>`;
          });
          html += "</ol>";
        }
        if (n.children && n.children.length) {
          html += `<div class="dag-rt-cond-block">${n.children.map((ch) => `<div>${cond(ch)}</div>`).join("")}</div>`;
        }
        return `<div class="dag-rt-priority">${html}</div>`;
      }
      if (n.kind === "all" || n.kind === "any") return group(n);
      return `<span class="dag-rt-leaf">${linkifyHtml(n.text || "", refs)}</span>`;
    }
    return `<div class="dag-rt">${render(node)}</div>`;
  }

  // Wire data-ref-table/field links (chips + inline refs + rule-tree refs) to navigation.
  function wireRefLinks(root) {
    root.querySelectorAll("[data-ref-table][data-ref-field]").forEach((a) => {
      a.addEventListener("click", (e) => {
        if (e.metaKey || e.ctrlKey || e.shiftKey) return;
        e.preventDefault();
        routing.navigate(a.getAttribute("data-ref-table"), a.getAttribute("data-ref-field"));
      });
    });
  }

  function typeBadgeHtml(type, size) {
    const tone = R().typeTone(type);
    const label = tt("type_" + type) || R().typeLabel(type);
    return `<span class="tb tb-${tone} tb-${size || "md"}"><span class="tb-glyph">${esc(R().typeGlyph(type))}</span><span class="tb-label">${esc(label)}</span></span>`;
  }

  // ════════════════════════════════════════════════════════════════════════
  //  WITNESS: one row's value and the values it came from, down to the stored
  //  ones. witness.js computes them with the project's generated TypeScript;
  //  this part only asks and draws. Off until the host names the folder that
  //  holds rulebook-to-typescript's output: init({ typescript: "/…" }).
  // ════════════════════════════════════════════════════════════════════════
  const W = () => global.EffortlessWitness;
  const witnessOn = () => !!config.typescript;

  // A build that writes rulebook-to-typescript's output INTO this explainer's own
  // folder, as rulebook-explainer-dag/typescript/ (the rulebook editor does, on
  // every build), turns the witness on with nothing said: no option, no proxy rule,
  // because that folder is already served wherever this file is. Asked once, with
  // one small request; a host that names a folder, or says typescript: false, is
  // not asked at all.
  let ownTypescript = null;   // null = not asked yet | "asking" | "none" | the folder
  function findOwnTypescript() {
    if (ownTypescript !== null || typeof fetch !== "function") return;
    const folder = `${(SELF_BASE || config.basePath || "/rulebook-explainer-dag").replace(/\/$/, "")}/typescript`;
    ownTypescript = "asking";
    fetch(`${folder}/erb_sdk.ts`, { method: "HEAD", headers: { Accept: "text/plain, */*;q=0.1" } })
      // A dev server that answers every unknown address with the app's own page says 200 too: that is not it.
      .then((res) => !!res && res.ok && !/text\/html/i.test((res.headers && res.headers.get && res.headers.get("content-type")) || ""))
      .catch(() => false)
      .then((found) => {
        ownTypescript = found ? folder : "none";
        if (!found || config.typescript != null) return;
        config.typescript = folder;
        if (W()) configureWitness();
        rerenderCurrent();
      });
  }
  let witnessScript = null;
  let witnessWaiters = [];

  // witness.js sits beside this file and is fetched on first use, so a host that
  // pasted the original four script tags needs no fifth.
  function loadWitnessScript() {
    if (W()) return Promise.resolve();
    if (!witnessScript) {
      witnessScript = new Promise((resolve, reject) => {
        const el = document.createElement("script");
        el.src = `${(config.basePath && config.basePath !== "/rulebook-explainer-dag" ? config.basePath : SELF_BASE || config.basePath || "/rulebook-explainer-dag").replace(/\/$/, "")}/witness.js`;
        el.onload = () => resolve();
        el.onerror = () => reject(new Error(`${el.src} could not be loaded`));
        document.head.appendChild(el);
      });
    }
    return witnessScript;
  }
  function configureWitness() {
    if (!W()) return;
    W().configure({ typescript: config.typescript || null, basePath: SELF_BASE || config.basePath || "/rulebook-explainer-dag" });
    if (config.rows) { W().setRows(config.rows); }
  }
  // "ready" | "loading" | "error" | "off". Asking starts the load; `then` runs once when it ends.
  function witnessState(then) {
    if (!witnessOn()) return "off";
    const w = W();
    if (!w && witnessLoadError) return "error";     // witness.js itself would not load: say so, do not ask again
    const st = w ? w.state() : "idle";
    if (st === "ready" || st === "error") return st;
    if (then) witnessWaiters.push(then);
    if (witnessLoading) return "loading";
    witnessLoading = true;
    loadWitnessScript()
      .then(() => { configureWitness(); return W().ready(); })
      .catch((e) => { if (!W()) witnessLoadError = (e && e.message) || String(e); /* else W().error() holds it */ })
      .then(() => {
        witnessLoading = false;
        const run = witnessWaiters; witnessWaiters = [];
        run.forEach((fn) => { try { fn(); } catch { /* a popup that has gone */ } });
      });
    return "loading";
  }
  let witnessLoading = false;
  let witnessLoadError = null;
  const witnessErrorText = () => (W() && W().error()) || witnessLoadError || "";

  // The row being witnessed, per table: the one the cell named (data-er-row), then
  // the one last picked, then the table's first. Kept for the tab, so following a
  // link to another field of the same table stays on the same row.
  const TYPESCRIPT_KEY = "dag.typescript";
  const ROWS_KEY = "dag.witnessRows";
  let rowsMemo = {};
  function witnessRows() {
    try { return Object.assign(rowsMemo, JSON.parse(sessionStorage.getItem(ROWS_KEY) || "{}")); } catch { return rowsMemo; }
  }
  function setWitnessRow(table, rowId) {
    if (!table || rowId == null || rowId === "") return;
    rowsMemo = witnessRows();
    rowsMemo[table] = String(rowId);
    try { sessionStorage.setItem(ROWS_KEY, JSON.stringify(rowsMemo)); } catch { /* kept in memory */ }
  }
  function witnessRowFor(table) {
    const rows = W().rowsOf(table);
    const want = witnessRows()[table];
    if (want != null && rows.some((r) => r.id === String(want))) return String(want);
    return rows.length ? rows[0].id : null;
  }

  function valueHtml(v) {
    if (v === null || v === undefined) return `<span class="dag-w-val dag-w-blank">${esc(tt("witnessBlank"))}</span>`;
    if (v === "") return `<span class="dag-w-val dag-w-blank">${esc(tt("witnessEmptyText"))}</span>`;
    if (typeof v === "string") return `<span class="dag-w-val dag-w-str">“${esc(v)}”</span>`;
    if (typeof v === "number") return `<span class="dag-w-val dag-w-num">${esc(fmtNum(v))}</span>`;
    return `<span class="dag-w-val dag-w-bool">${esc(String(v))}</span>`;
  }
  const MAX_COUNTED = 25;

  // What a derived cell was made from, in words, then each of those cells.
  // `depth` = how many levels below this one are drawn open now; deeper ones are
  // drawn when their line is opened (wireWitness). `trail` stops a cycle.
  function witnessBodyHtml(node, depth, trail, isRoot) {
    let why = "";
    if (node.error) why += `<p class="dag-w-why dag-w-err">${esc(tt("witnessError", node.error))}</p>`;
    if (node.via) {
      const key = esc(humanizeField(node.via.keyField || ""));
      why += `<p class="dag-w-why">${node.via.id != null
        ? tt("witnessLookup", key, valueHtml(node.via.keyValue), esc(node.via.table), `<strong>${esc(node.via.label)}</strong>`)
        : tt("witnessLookupMiss", key, valueHtml(node.via.keyValue), esc(node.via.table || ""))}</p>`;
    } else if (node.over) {
      why += `<p class="dag-w-why">${esc(node.over.rows == null ? tt("witnessOverUnknown") : tt("witnessOver", node.over.op, node.over.rows, node.over.table || ""))}</p>`;
    } else if (node.type !== "raw" && node.type !== "relationship") {
      // A nested cell shows its own rule (with the step that applied to this row);
      // the root's rule is already on the page or in the popup, just above.
      const rs = ruleSpeakFor(node.table, node.field);
      if (!isRoot && rs && rs.rule) why += `<div class="dag-w-why dag-w-rule">${ruleHtml(rs, tonedRefs(rs), node.step)}</div>`;
      // In words too, wherever the rule above is not an outline with the step marked
      // (the root's rule is drawn elsewhere; a SWITCH reads as one sentence).
      if (node.step && (isRoot || !(rs && rs.structure))) {
        why += `<p class="dag-w-why">${esc(node.step.index == null ? tt("witnessNoStep") : tt("witnessStep", node.step.index + 1, node.step.steps))}</p>`;
      }
    }
    let kids = node.children.map((c) => witnessCellHtml(c, depth - 1, trail, node)).join("");
    if (node.counted) {
      const shown = node.counted.slice(0, MAX_COUNTED);
      kids += shown.map((c) => c.field
        ? witnessCellHtml({ table: c.table, field: c.field, rowId: c.id }, depth - 1, trail, node)
        : `<div class="dag-w dag-w-rowonly"><span class="dag-w-line"><span class="dag-w-of">${esc(c.table)}</span> <strong>${esc(c.label)}</strong></span></div>`).join("");
      if (node.counted.length > shown.length) kids += `<div class="dag-w dag-w-more">${esc(tt("witnessMoreRows", node.counted.length - shown.length))}</div>`;
    }
    return why + (kids ? `<div class="dag-w-kids">${kids}</div>` : "");
  }

  // One cell of the witness: name = value, and (when opened) what it came from.
  function witnessCellHtml(cell, depth, trail, parent, isRoot) {
    const node = W().witness(cell.table, cell.field, cell.rowId);
    if (!node) return "";
    if (node.missing) return `<div class="dag-w dag-w-err">${esc(node.missing)}</div>`;
    const tone = R().typeTone(node.type);
    const key = `${node.table}.${node.field}#${node.rowId}`;
    const elsewhere = !parent || parent.table !== node.table || parent.rowId !== node.rowId;
    let note = "";
    if (node.groundTruth) {
      note = `<span class="dag-w-note dag-w-gt">${esc(tt("witnessGroundTruth"))}</span>`;
      if (node.pointsTo) {
        note += `<span class="dag-w-note">${node.pointsTo.label != null
          ? tt("witnessPointsTo", esc(node.pointsTo.table), `<strong>${esc(node.pointsTo.label)}</strong>`)
          : esc(tt("witnessPointsNowhere", node.pointsTo.table))}</span>`;
      }
    }
    const line =
      `<span class="dag-w-glyph dag-w-glyph-${tone}">${esc(R().typeGlyph(node.type))}</span>` +
      (elsewhere && !isRoot ? `<span class="dag-w-of">${esc(node.table)} <strong>${esc(node.rowLabel)}</strong>:</span> ` : "") +
      `<a class="dag-w-name fc-inline fc-inline-${tone}" href="${esc(refHref(node.table, node.field))}" data-ref-table="${esc(node.table)}" data-ref-field="${esc(node.field)}" data-w-row="${esc(node.rowId)}">${esc(humanizeField(node.field))}</a>` +
      `<span class="dag-w-eq">=</span>${valueHtml(node.value)}${note}`;
    const attrs = `data-w-table="${esc(node.table)}" data-w-field="${esc(node.field)}" data-w-id="${esc(node.rowId)}"`;
    const expandable = !node.groundTruth && (node.children.length || node.counted || node.via || node.over || node.error);
    if (!expandable) return `<div class="dag-w dag-w-leaf dag-w-${tone}" ${attrs}><span class="dag-w-line">${line}</span></div>`;
    if (trail.includes(key)) {
      return `<div class="dag-w dag-w-leaf dag-w-${tone}" ${attrs}><span class="dag-w-line">${line}<span class="dag-w-note">${esc(tt("witnessSeenAbove"))}</span></span></div>`;
    }
    const open = depth > 0;
    const body = open ? witnessBodyHtml(node, depth, trail.concat(key), !!isRoot) : "";
    return `<details class="dag-w dag-w-${tone}" ${attrs}${open ? " open" : ` data-w-lazy="${esc(trail.concat(key).join("|"))}"`}${isRoot ? ' data-w-root="1"' : ""}>` +
      `<summary class="dag-w-line">${line}</summary><div class="dag-w-body">${body}</div></details>`;
  }

  // The witness of one cell as HTML: the tree when the engine has answered, else
  // what it is waiting for or why it failed. depth = levels drawn open.
  function witnessHtml(table, field, rowId, opts) {
    const depth = (opts && opts.depth) || 1;
    const st = witnessState();
    if (st === "off") return "";
    if (st === "loading") return `<div class="dag-w-wait">${esc(tt("witnessComputing"))}</div>`;
    if (st === "error") return `<div class="dag-w-fail"><strong>${esc(tt("witnessFailed"))}</strong> ${esc(witnessErrorText())}</div>`;
    const id = rowId != null && rowId !== "" ? String(rowId) : witnessRowFor(table);
    if (id == null) return `<div class="dag-w-wait">${esc(tt("witnessNoRows", table))}</div>`;
    return `<div class="dag-w-tree">${witnessCellHtml({ table, field, rowId: id }, depth, [], null, true)}</div>`;
  }

  // Draw a closed line's content the first time it is opened, and make a witness
  // link carry its row along (the page it opens shows that row).
  function wireWitness(root) {
    root.querySelectorAll("details[data-w-lazy]").forEach((d) => {
      if (d.dataset.wWired) return;
      d.dataset.wWired = "1";
      d.addEventListener("toggle", () => {
        if (!d.open || !d.hasAttribute("data-w-lazy")) return;
        const trail = d.getAttribute("data-w-lazy").split("|").filter(Boolean);
        d.removeAttribute("data-w-lazy");
        const node = W().witness(d.dataset.wTable, d.dataset.wField, d.dataset.wId);
        const body = d.querySelector(".dag-w-body");
        if (!node || node.missing || !body) return;
        body.innerHTML = witnessBodyHtml(node, 1, trail, false);
        wireRefLinks(body);
        wireWitness(body);
      });
    });
    root.querySelectorAll("a[data-w-row]").forEach((a) => {
      if (a.dataset.wWired) return;
      a.dataset.wWired = "1";
      a.addEventListener("click", () => setWitnessRow(a.getAttribute("data-ref-table"), a.getAttribute("data-w-row")), true);
    });
  }

  // The step of a priority rule that applied to the row being witnessed, for the
  // rule shown on the page / in the popup; null when there is none to mark.
  function stepFor(table, field, rowId) {
    if (!witnessOn() || !W() || W().state() !== "ready") return null;
    const id = rowId != null && rowId !== "" ? String(rowId) : witnessRowFor(table);
    return id == null ? null : W().firedStep(table, field, id);
  }

  // The picker that chooses which row is witnessed. Its label says what a row of
  // this table IS, in the noun the rules use for it ("Measure"); "Row" only when
  // the graph has no noun for the table.
  function witnessPickerHtml(table) {
    const rows = W().rowsOf(table);
    if (!rows.length) return "";
    const current = witnessRowFor(table);
    const label = rowNoun(table, "witnessRow") || tt("witnessRow");
    return `<label class="dag-w-picker"><span class="dag-w-picker-label">${esc(label)}</span>` +
      `<select class="dag-w-select">${rows.map((r) => `<option value="${esc(r.id)}"${r.id === current ? " selected" : ""}>${esc(r.label)}</option>`).join("")}</select></label>`;
  }
  function renderWitnessSection(container, dag) {
    const sec = document.createElement("section");
    sec.className = "dag-section dag-witness";
    const st = witnessState(() => { if (sec.isConnected) rerenderCurrent() || renderFromHash(); });
    const picker = st === "ready" ? witnessPickerHtml(dag.table) : "";
    sec.innerHTML = `<h2 class="dag-section-title">▼ ${esc(tt("witnessTitle"))}</h2>` +
      `<p class="dag-section-lead muted">${esc(tt("witnessLead"))}</p>` + picker +
      witnessHtml(dag.table, dag.field, null, { depth: 2 });
    const select = sec.querySelector(".dag-w-select");
    if (select) select.addEventListener("change", () => { setWitnessRow(dag.table, select.value); rerenderCurrent() || renderFromHash(); });
    wireWitness(sec);
    container.appendChild(sec);
  }

  // ════════════════════════════════════════════════════════════════════════
  //  HEADER (back / home / crumbs + narration slider + gear)
  // ════════════════════════════════════════════════════════════════════════
  function renderHeader(crumbs) {
    const bar = document.createElement("div");
    bar.className = "dag-topbar";

    const left = document.createElement("div");
    left.className = "dag-nav-left";
    const back = document.createElement("button");
    back.type = "button"; back.className = "dag-back"; back.textContent = tt("back");
    back.title = tt("backTitle"); back.onclick = () => routing.onBack();
    left.appendChild(back);
    const home = document.createElement("button");
    home.type = "button"; home.className = "dag-home"; home.textContent = tt("home");
    home.title = tt("homeTitle"); home.onclick = () => routing.onHome();
    left.appendChild(home);

    if (crumbs && crumbs.length) {
      const trail = document.createElement("span");
      trail.className = "dag-crumb-trail";
      const sep0 = document.createElement("span"); sep0.className = "dag-crumb-sep"; sep0.textContent = "·";
      trail.appendChild(sep0);
      crumbs.forEach((c, i) => {
        if (i > 0) { const s = document.createElement("span"); s.className = "dag-crumb-sep"; s.textContent = "›"; trail.appendChild(s); }
        if (typeof c === "string") {
          const span = document.createElement("span"); span.className = "dag-crumb-text"; span.textContent = c; trail.appendChild(span);
        } else if (c.index) {
          const a = document.createElement("a"); a.className = "dag-crumb-index-link"; a.href = indexPath();
          a.textContent = c.label || tt("tablesCrumb");
          a.onclick = (e) => { if (e.metaKey || e.ctrlKey) return; e.preventDefault(); routing.navigateTable(""); };
          trail.appendChild(a);
        } else if (c.table) {
          trail.appendChild(renderTableLink(c.table, c.table, "dag-crumb-table-link"));
        }
      });
      left.appendChild(trail);
    }
    bar.appendChild(left);

    const right = document.createElement("div");
    right.className = "dag-nav-right";
    right.appendChild(renderNarrationSlider());
    right.appendChild(renderGear());
    bar.appendChild(right);
    return bar;
  }

  // The global 3-way RuleSpeak®·English·Formula segmented control. Built fresh
  // per render (not a module-level constant) so it always reflects the
  // current `lang`.
  // The slider offers the two readings a person switches between; Formula, and everything
  // else, is in the gear (narrationOptions(true) is all three).
  function narrationOptions(all) {
    const opts = [
      { value: "rulespeak", label: tt("narrRulespeak"), hint: tt("narrRulespeakHint") },
      { value: "english",   label: tt("narrEnglish"),   hint: tt("narrEnglishHint") },
      { value: "formula",   label: tt("narrFormula"),   hint: tt("narrFormulaHint") },
    ];
    return all ? opts : opts.filter((o) => o.value !== "formula");
  }
  function renderNarrationSlider() {
    const seg = document.createElement("div");
    seg.className = "dag-narr-seg"; seg.setAttribute("role", "radiogroup");
    seg.setAttribute("aria-label", tt("narrAriaLabel"));
    const mode = prefs.narrationMode();
    for (const o of narrationOptions()) {
      const b = document.createElement("button");
      b.type = "button"; b.setAttribute("role", "radio");
      b.setAttribute("aria-checked", String(mode === o.value));
      b.className = `dag-narr-seg-opt${mode === o.value ? " is-active" : ""} dag-narr-${o.value}`;
      b.title = o.hint; b.textContent = o.label;
      b.onclick = () => prefs.setNarrationMode(o.value);
      seg.appendChild(b);
    }
    return seg;
  }

  // The settings gear: a ⚙︎ button opening a popup of independent doc-element
  // checkboxes. Controls which sections show in the hover card AND the field page.
  function renderGear() {
    const wrap = document.createElement("div");
    wrap.className = "dag-gear-wrap";
    const btn = document.createElement("button");
    btn.type = "button"; btn.className = "dag-gear"; btn.textContent = "⚙︎";
    btn.title = tt("gearTitle");
    btn.setAttribute("aria-expanded", "false");
    btn.setAttribute("aria-label", tt("gearAriaLabel"));
    wrap.appendChild(btn);

    let pop = null;
    function close() {
      if (pop) { pop.remove(); pop = null; }
      btn.setAttribute("aria-expanded", "false");
      document.removeEventListener("click", onDocClick, true);
    }
    function onDocClick(e) { if (!wrap.contains(e.target)) close(); }
    function open() {
      pop = document.createElement("div");
      pop.className = "dag-gear-pop";
      const title = document.createElement("div");
      title.className = "dag-gear-pop-title"; title.textContent = tt("gearAriaLabel");
      pop.appendChild(title);

      // Show as: Docs / Simple / Formula (the slider has the first two).
      const showTitle = document.createElement("div");
      showTitle.className = "dag-gear-section"; showTitle.textContent = tt("gearShowAs");
      pop.appendChild(showTitle);
      const mode = prefs.narrationMode();
      for (const o of narrationOptions(true)) {
        const label = document.createElement("label");
        label.className = "dag-gear-opt";
        const r = document.createElement("input");
        r.type = "radio"; r.name = "dag-narration"; r.checked = mode === o.value;
        r.onchange = () => prefs.setNarrationMode(o.value);
        const lab = document.createElement("span");
        lab.className = "dag-gear-opt-label"; lab.textContent = o.label;
        const hint = document.createElement("span");
        hint.className = "dag-gear-opt-hint"; hint.textContent = o.hint;
        label.appendChild(r); label.appendChild(lab); label.appendChild(hint);
        pop.appendChild(label);
      }

      // Language: the ones this page was built with, each in its own name.
      const langs = availableLanguages();
      if (langs.length > 1) {
        const langTitle = document.createElement("div");
        langTitle.className = "dag-gear-section"; langTitle.textContent = tt("gearLanguage");
        pop.appendChild(langTitle);
        const sel = document.createElement("select");
        sel.className = "dag-gear-lang"; sel.setAttribute("aria-label", tt("gearLanguage"));
        for (const code of langs) {
          const o = document.createElement("option");
          o.value = code; o.textContent = NATIVE[code] || code;
          if (code === lang) o.selected = true;
          sel.appendChild(o);
        }
        sel.onchange = () => setLanguage(sel.value);
        pop.appendChild(sel);
      }

      const elemTitle = document.createElement("div");
      elemTitle.className = "dag-gear-section"; elemTitle.textContent = tt("gearPopTitle");
      pop.appendChild(elemTitle);
      const elems = prefs.docElements();
      for (const d of DOC_ELEMENTS) {
        const label = document.createElement("label");
        label.className = "dag-gear-opt";
        const cb = document.createElement("input");
        cb.type = "checkbox"; cb.checked = !!elems[d.key];
        cb.onchange = () => prefs.setDocElement(d.key, cb.checked);
        const lab = document.createElement("span");
        lab.className = "dag-gear-opt-label"; lab.textContent = tt(d.labelKey);
        const hint = document.createElement("span");
        hint.className = "dag-gear-opt-hint"; hint.textContent = tt(d.hintKey);
        label.appendChild(cb); label.appendChild(lab); label.appendChild(hint);
        pop.appendChild(label);
      }
      const reset = document.createElement("button");
      reset.type = "button"; reset.className = "dag-gear-reset"; reset.textContent = tt("gearReset");
      reset.onclick = () => { prefs.resetDocElements(); };
      pop.appendChild(reset);
      wrap.appendChild(pop);
      btn.setAttribute("aria-expanded", "true");
      // Defer so the opening click doesn't immediately close it.
      setTimeout(() => document.addEventListener("click", onDocClick, true), 0);
    }
    btn.onclick = (e) => { e.stopPropagation(); if (pop) close(); else open(); };
    return wrap;
  }

  function indexPath() {
    return global.EffortlessExplainerRouting.indexPath(routing.cfg);
  }

  // ════════════════════════════════════════════════════════════════════════
  //  PAGES
  // ════════════════════════════════════════════════════════════════════════
  function renderTablesIndex(container) {
    ensureRouting();
    lastRender = { kind: "index", container };
    container.innerHTML = "";
    speaks(container).className = "dag-page " + prefs.docElementClasses();
    container.appendChild(renderHeader([{ index: true }]));

    const hero = document.createElement("header");
    hero.className = "dag-hero dag-hero-calc";
    hero.innerHTML = `<h1 class="dag-title">${esc(tt("ontologyTitle"))}</h1>` +
      `<p class="dag-subtitle"><span class="dag-subtitle-human">${esc(tt("ontologyLead"))}</span></p>`;
    container.appendChild(hero);

    // Views over the tables (groups, A-Z, the graph) and a filter that narrows all of them.
    const view = NAV.view || "groups";
    const tools = document.createElement("div");
    tools.innerHTML = indexViewsHtml(view);
    const bar = tools.firstChild;
    container.appendChild(bar);
    const body = document.createElement("div");
    body.className = "dag-index-body";
    body.innerHTML = indexBodyHtml(view);
    container.appendChild(body);
    bar.querySelectorAll("[data-view]").forEach((t) => t.addEventListener("click", () => { NAV.view = t.getAttribute("data-view"); renderTablesIndex(container); }));
    const q = bar.querySelector(".dag-index-q");
    q.addEventListener("input", () => {
      navFilter(q.value);
      body.innerHTML = indexBodyHtml(NAV.view || "groups");
      wireLinks(body);
      const nav = container.querySelector(".dag-nav");
      if (nav) { const nq = nav.querySelector(".dag-nav-q"); if (nq) nq.value = NAV.q; refreshNavBody(nav, {}); }
    });
    wireLinks(container);
    withShell(container, {});
  }

  function renderTablePage(container, table) {
    ensureRouting();
    lastRender = { kind: "table", container, table };
    container.innerHTML = "";
    speaks(container).className = "dag-page " + prefs.docElementClasses();
    container.appendChild(renderHeader([{ index: true }, { table }]));
    if (!R().tableExists(table)) {
      const d = document.createElement("div"); d.className = "muted"; d.textContent = tt("tableNotFound");
      container.appendChild(d); return;
    }
    const mode = prefs.narrationMode();
    const hero = document.createElement("header");
    hero.className = "dag-hero dag-hero-rel";
    const desc = R().tableDescription(table);
    hero.innerHTML = `<h1 class="dag-title">${esc(table)}</h1>` +
      `<p class="dag-subtitle"><span class="dag-subtitle-human">${esc(humanizeField(table))}</span></p>` +
      (desc ? `<p class="dag-description">${esc(desc)}</p>` : "");
    container.appendChild(hero);

    const tabsHost = document.createElement("div");
    tabsHost.innerHTML = fieldTabsHtml(table);
    container.appendChild(tabsHost.firstChild);
    const list = document.createElement("div");
    list.className = "dag-col-list";
    for (const col of R().tableColumns(table)) {
      const row = renderFieldLink(table, col.field, "", `dag-col dag-col-${R().typeTone(col.type)}`);
      row.setAttribute("data-kind", fieldKind(col));
      row.setAttribute("data-name", (col.field + " " + humanizeField(col.field)).toLowerCase());
      let explain = "";
      if (col.type !== "raw" && col.type !== "relationship") {
        if (mode === "rulespeak") {
          const rs = ruleSpeakFor(table, col.field);
          if (rs && rs.rule) explain = renderRuleRichHtml(rs.rule, tonedRefs(rs));
        }
        if (!explain) {
          const plain = plainReading(table, col.field, col.formula);
          if (plain) explain = esc(plain.text + plain.end);
        }
      }
      if (!explain && col.description) explain = esc(col.description);
      row.innerHTML =
        `<span class="dag-col-head">${typeBadgeHtml(col.type, "sm")}` +
        `<span class="dag-col-name">${esc(col.field)}</span>` +
        `<span class="dag-col-human">${esc(humanizeField(col.field))}</span>` +
        (col.nullable ? `<span class="dag-pill dag-pill-muted">${esc(tt("nullable"))}</span>` : "") + `</span>` +
        (explain ? `<span class="dag-col-explain">${explain}</span>` : "") +
        (col.type === "relationship" && col.relatedTo
          ? `<span class="dag-col-rel">${onward()} <a class="dag-col-rel-link" href="${esc(tableHrefSafe(col.relatedTo))}" data-table-link="${esc(col.relatedTo)}">${esc(col.relatedTo)}</a></span>` : "");
      list.appendChild(row);
    }
    container.appendChild(list);
    // Relationship → related-table links inside column rows.
    container.querySelectorAll("[data-table-link]").forEach((a) => {
      a.addEventListener("click", (e) => {
        if (e.metaKey || e.ctrlKey) return; e.preventDefault();
        e.stopPropagation();
        routing.navigateTable(a.getAttribute("data-table-link"));
      });
    });
    wireRefLinks(container);
    wireFieldTabs(container);
    withShell(container, { table });
  }

  function renderFieldPage(container, table, field) {
    ensureRouting();
    lastRender = { kind: "field", container, table, field };
    container.innerHTML = "";
    speaks(container).className = "dag-page " + prefs.docElementClasses();
    const dag = R().resolveDag(table, field);
    if (!dag) {
      container.appendChild(renderHeader([{ index: true }, { table }, field]));
      const d = document.createElement("div"); d.className = "muted"; d.textContent = tt("fieldNotFound");
      container.appendChild(d); return;
    }
    const mode = prefs.narrationMode();
    const elems = prefs.docElements();
    const tone = R().typeTone(dag.type);
    const rs = ruleSpeakFor(table, field);
    const refs = tonedRefs(rs);

    container.appendChild(renderHeader([{ index: true }, { table: dag.table }, dag.field]));

    // Hero (with the owning table as a link).
    const hero = document.createElement("header");
    hero.className = `dag-hero dag-hero-${tone}`;
    let metaPills = typeBadgeHtml(dag.type, "md");
    if (dag.depth > 0) metaPills += `<span class="dag-pill dag-pill-depth">${esc(tt("hopsFromGround", dag.depth))}</span>`;
    metaPills += `<span class="dag-pill">${esc(dag.datatype)}</span>`;
    if (dag.nullable) metaPills += `<span class="dag-pill dag-pill-muted">${esc(tt("nullable"))}</span>`;
    hero.innerHTML =
      `<div class="dag-hero-meta">${metaPills}</div>` +
      `<h1 class="dag-title">${esc(dag.field)}</h1>` +
      `<p class="dag-subtitle"><a class="dag-subtitle-table dag-subtitle-table-link" href="${esc(tableHrefSafe(dag.table))}" data-table-link="${esc(dag.table)}">${esc(dag.table)}</a> · <span class="dag-subtitle-human">${esc(humanizeField(dag.field))}</span></p>` +
      (dag.description ? `<p class="dag-description elem elem-desc">${esc(dag.description)}</p>` : "");
    container.appendChild(hero);

    // The rule card.
    if (dag.type === "raw" || dag.type === "relationship") {
      container.appendChild(renderGroundTruthCard(dag));
    } else if (mode === "rulespeak") {
      container.appendChild(renderRuleSpeakCard(dag, rs, refs, stepFor(table, field, null)));
    } else if (mode === "english") {
      container.appendChild(renderEnglishCard(dag, refs));
    } else {
      container.appendChild(renderFormulaCard(dag));
    }

    // The witness: this cell for one row, down to the stored values (witness.js).
    if (witnessOn()) renderWitnessSection(container, dag);

    if (elems.inputs && dag.upstream.length) container.appendChild(renderInputsSection(dag.upstream, mode));
    container.appendChild(renderConsumersSection(dag.downstream, dag.consumerTransitive));
    if (dag.leaves.length) container.appendChild(renderLeavesSection(dag.leaves));

    // Wire all navigation.
    container.querySelectorAll("[data-table-link]").forEach((a) => {
      a.addEventListener("click", (e) => { if (e.metaKey || e.ctrlKey) return; e.preventDefault(); routing.navigateTable(a.getAttribute("data-table-link")); });
    });
    wireRefLinks(container);
    withShell(container, { table: dag.table, field: dag.field });
  }

  function renderGroundTruthCard(dag) {
    const sec = document.createElement("section");
    sec.className = "dag-card dag-gt";
    const isRel = dag.type === "relationship";
    let body;
    if (isRel) {
      body = dag.relatedTo
        ? tt("relPointerBody", `<a class="dag-gt-rel-link" href="${esc(tableHrefSafe(dag.relatedTo))}" data-table-link="${esc(dag.relatedTo)}">${esc(dag.relatedTo)}</a>`)
        : tt("relPointerBodyNoLink");
    } else {
      body = tt("groundTruthBody", esc(dag.datatype));
    }
    sec.innerHTML =
      `<div class="dag-gt-glyph">${esc(R().typeGlyph(dag.type))}</div>` +
      `<div><h3 class="dag-gt-title">${esc(isRel ? tt("relationshipPointer") : tt("groundTruth"))}</h3>` +
      `<p class="dag-gt-body">${body}</p>` +
      (isRel && dag.relatedTo ? `<p class="dag-gt-rel-cta"><a class="dag-gt-rel-btn" href="${esc(tableHrefSafe(dag.relatedTo))}" data-table-link="${esc(dag.relatedTo)}">${esc(tt("openTable", dag.relatedTo))}</a></p>` : "") +
      `</div>`;
    return sec;
  }

  // Some packs end their own sentences (ja "。", hi "।"); add a period only when
  // the rendered rule does not already end in sentence-final punctuation.
  function ruleSentenceHtml(rule, refs) {
    const end = /[.。।!?！？]$/.test(String(rule).replace(/[*_\s]+$/, "")) ? "" : ".";
    return renderRuleRichHtml(rule, refs) + end;
  }

  // A field's rule as HTML: an outline (numbered steps, nested AND/OR lists) for a
  // priority ladder or a rule of several conditions, one sentence otherwise. The
  // field's page and the popup over a cell both show exactly this.
  function ruleHtml(rs, refs, step) {
    return rs.structure ? ruleTreeHtml(rs.structure, refs, step, rs.table) : ruleSentenceHtml(rs.rule, refs);
  }
  function ruleBodyHtml(table, field) {
    ensureRouting();
    const rs = ruleSpeakFor(table, field);
    return rs && rs.rule ? ruleHtml(rs, tonedRefs(rs)) : "";
  }

  function renderRuleSpeakCard(dag, rs, refs, step) {
    const sec = document.createElement("section");
    sec.className = "dag-card dag-rulespeak elem elem-rulespeak";
    let inner = `<h3 class="dag-card-title">${esc(tt("ruleCardTitle"))}</h3>`;
    if (rs && rs.rule) {
      const mech = rs.mechanical ? ` <span class="dag-rs-mech" title="${esc(tt("mechanicalTitle"))}"> ${esc(tt("mechanicalFlag"))}</span>` : "";
      const ruleBody = rs.structure
        ? `<div class="dag-english-text">${ruleHtml(rs, refs, step)}</div>`
        : `<p class="dag-english-text">${ruleHtml(rs, refs)}</p>`;
      inner += `<div class="dag-english dag-rs-rule"><span class="dag-english-label">${esc(tt("inRuleSpeak"))}${mech}</span>${ruleBody}</div>`;
    } else {
      inner += `<p class="dag-english-text muted">${esc(tt("noRuleRendered", dag.type))}</p>`;
    }
    const obls = R().obligationsForField(dag.table, dag.field, lang);
    if (obls.length) {
      inner += `<div class="dag-rs-obligations"><span class="dag-english-label">${esc(tt("obligations"))}</span><ul class="dag-rs-obl-list">` +
        obls.map((o) => `<li class="dag-rs-obl dag-rs-obl-${o.severity || "hard"}">${renderRuleRichHtml(o.markdown, refs)}</li>`).join("") +
        `</ul></div>`;
    }
    inner += `<p class="dag-formula-hint muted">${esc(dag.formula ? tt("ruleSpeakHintFormula") : tt("ruleSpeakHintNoFormula"))}</p>`;
    sec.innerHTML = inner;
    return sec;
  }

  function renderEnglishCard(dag, refs) {
    const sec = document.createElement("section");
    sec.className = "dag-card dag-formula elem elem-english";
    const plain = plainReading(dag.table, dag.field, dag.formula);
    let inner = `<h3 class="dag-card-title">${esc(tt("oneLineFnCardTitle"))}</h3>`;
    if (plain) {
      inner += `<div class="dag-english"><span class="dag-english-label">${esc(tt("inEnglish"))}</span><p class="dag-english-text">${linkifyHtml(plain.text, refs)}${esc(plain.end)}</p></div>` +
        `<p class="dag-formula-hint muted">${esc(tt("englishHint"))}</p>`;
    } else {
      inner += `<p class="dag-english-text muted">${tt("noFormulaSentence", esc(dag.type))}</p>`;
    }
    sec.innerHTML = inner;
    return sec;
  }

  function renderFormulaCard(dag) {
    const sec = document.createElement("section");
    sec.className = "dag-card dag-formula elem elem-formula";
    let inner = `<h3 class="dag-card-title">${esc(tt("oneLineFnCardTitle"))}</h3>`;
    if (dag.formula) {
      inner += `<div class="dag-formula-syntax"><span class="dag-formula-label">${esc(tt("asFormula"))}</span><div class="dag-formula-code">${formulaHtml(dag.formula, dag.table)}</div></div>` +
        `<p class="dag-formula-hint muted">${esc(tt("formulaHint"))}</p>`;
    } else {
      inner += `<p class="dag-english-text muted">${tt("noFormulaAtAll", esc(dag.type))}</p>`;
    }
    sec.innerHTML = inner;
    return sec;
  }

  function renderInputsSection(upstream, mode) {
    const sec = document.createElement("section");
    sec.className = "dag-section elem elem-inputs";
    sec.innerHTML = `<h2 class="dag-section-title">▼ ${esc(tt("inputsTitle"))} <span class="dag-count">${upstream.length}</span></h2>` +
      `<p class="dag-section-lead muted">${esc(tt("inputsLead"))}</p>`;
    const grid = document.createElement("div");
    grid.className = "dag-input-grid";
    for (const u of upstream) {
      const tone = R().typeTone(u.type);
      const card = renderFieldLink(u.table, u.field, "", `dag-input dag-input-${tone}`);
      let explain = "";
      if (mode === "rulespeak") {
        const rs = ruleSpeakFor(u.table, u.field);
        if (rs && rs.rule) explain = `<span class="dag-input-english dag-input-rulespeak">${renderRuleRichHtml(rs.rule, tonedRefs(rs))}</span>`;
      }
      if (!explain) {
        const plain = plainReading(u.table, u.field, u.formula);
        if (plain) explain = `<span class="dag-input-english">${esc(plain.text + plain.end)}</span>`;
      }
      if (!explain && u.description) explain = `<span class="dag-input-desc">${esc(u.description)}</span>`;
      if (!explain && u.type === "raw") explain = `<span class="dag-input-desc muted">${esc(tt("groundTruthInline", u.datatype))}</span>`;
      card.innerHTML =
        `<span class="dag-input-head">${typeBadgeHtml(u.type, "sm")}` +
        `<span class="dag-input-name">${esc(u.field)}</span>` +
        `<span class="dag-input-table">${esc(u.table)}</span></span>` + explain;
      grid.appendChild(card);
    }
    sec.appendChild(grid);
    return sec;
  }

  function pillHtml(table, field) {
    const node = R().fieldOf(R().rulebook(), table, field);
    const type = (node && node.type) || "raw";
    const tone = R().typeTone(type);
    return `<a class="fc fc-${tone} fc-pill" href="${esc(refHref(table, field))}" data-ref-table="${esc(table)}" data-ref-field="${esc(field)}">` +
      `<span class="fc-glyph">${esc(R().typeGlyph(type))}</span>` +
      `<span class="fc-table">${esc(table)}.</span>` +
      `<span class="fc-name">${esc(humanizeField(field))}</span></a>`;
  }

  function renderConsumersSection(direct, transitive) {
    const sec = document.createElement("section");
    sec.className = "dag-section elem elem-consumers";
    const transitiveOnly = (transitive || []).filter((t) => !direct.some((d) => d.table === t.table && d.field === t.field));
    if (!direct.length && !transitiveOnly.length) {
      sec.innerHTML = `<h2 class="dag-section-title">▼ ${esc(tt("consumersTitle"))} <span class="dag-count">0</span></h2>` +
        `<p class="muted">${esc(tt("consumersEmpty"))}</p>`;
      return sec;
    }
    let inner = `<h2 class="dag-section-title">▼ ${esc(tt("consumersTitle"))} <span class="dag-count">${direct.length} ${esc(tt("consumersDirect"))}</span>` +
      (transitiveOnly.length ? `<span class="dag-count dag-count-muted">+${transitiveOnly.length} ${esc(tt("consumersTransitive"))}</span>` : "") + `</h2>` +
      `<p class="dag-section-lead muted">${esc(tt("consumersLead"))}</p>`;
    if (direct.length) {
      inner += `<div class="dag-pill-row">${direct.map((d) => pillHtml(d.table, d.field)).join("")}</div>`;
    }
    if (transitiveOnly.length) {
      inner += `<details class="dag-transitive"><summary>${esc(tt("showTransitive", transitiveOnly.length))}</summary>` +
        `<div class="dag-pill-row dag-pill-row-faded">${transitiveOnly.map((t) => pillHtml(t.table, t.field)).join("")}</div></details>`;
    }
    sec.innerHTML = inner;
    return sec;
  }

  function renderLeavesSection(leaves) {
    const sec = document.createElement("section");
    sec.className = "dag-section dag-section-leaves";
    sec.innerHTML = `<h2 class="dag-section-title">▼ ${esc(tt("leavesTitle"))} <span class="dag-count">${leaves.length}</span></h2>` +
      `<p class="dag-section-lead muted">${esc(tt("leavesLead"))}</p>` +
      `<div class="dag-pill-row">${leaves.map((l) => pillHtml(l.table, l.field)).join("")}</div>`;
    return sec;
  }

  // ════════════════════════════════════════════════════════════════════════
  //  MODAL (routing mode === "modal")
  // ════════════════════════════════════════════════════════════════════════
  function ensureModal() {
    let modal = document.getElementById("effortless-explainer-modal");
    if (modal) return modal;
    modal = document.createElement("div");
    modal.id = "effortless-explainer-modal";
    speaks(modal).className = "dag-modal hidden";
    modal.innerHTML =
      '<div class="dag-modal-backdrop"></div><div class="dag-modal-panel"><button type="button" class="dag-modal-close">×</button><div id="effortless-explainer-modal-body"></div></div>';
    document.body.appendChild(modal);
    modal.querySelector(".dag-modal-backdrop").onclick = closeModal;
    modal.querySelector(".dag-modal-close").onclick = closeModal;
    return modal;
  }
  // What the modal has shown since it opened, so its Back button steps back
  // through that and never through the host page's history.
  let modalTrail = [];
  function drawModal(view) {
    const modal = ensureModal();
    const body = modal.querySelector("#effortless-explainer-modal-body");
    if (view.table && view.field) renderFieldPage(body, view.table, view.field);
    else if (view.table) renderTablePage(body, view.table);
    else renderTablesIndex(body);
    modal.classList.remove("hidden");
  }
  function openModal(table, field) {
    const view = { table: table || null, field: field || null };
    const last = modalTrail[modalTrail.length - 1];
    if (!last || last.table !== view.table || last.field !== view.field) modalTrail.push(view);
    drawModal(view);
  }
  function modalBack() {
    modalTrail.pop();
    if (modalTrail.length) drawModal(modalTrail[modalTrail.length - 1]);
    else closeModal();
  }
  function closeModal() {
    modalTrail = [];
    const modal = document.getElementById("effortless-explainer-modal");
    if (modal) modal.classList.add("hidden");
  }

  // ════════════════════════════════════════════════════════════════════════
  //  HOVER MICRO-PAGE over cells (sticky; dialect follows narration + gear)
  // ════════════════════════════════════════════════════════════════════════
  function cancelHoverClose() {
    if (hoverCloseTimer !== null) { clearTimeout(hoverCloseTimer); hoverCloseTimer = null; }
  }
  function removeHoverCard() {
    if (activeHoverCard) { activeHoverCard.remove(); activeHoverCard = null; }
  }
  function scheduleHoverClose() {
    cancelHoverClose();
    hoverCloseTimer = setTimeout(() => { removeHoverCard(); hoverCloseTimer = null; }, CLOSE_DELAY_MS);
  }
  function positionHoverCard(shell, anchor) {
    const vw = window.innerWidth, vh = window.innerHeight;
    const cardW = Math.min(CARD_W, vw - 2 * MARGIN);
    const cardLeft = Math.min(Math.max(anchor.right - cardW, MARGIN), vw - cardW - MARGIN);
    const spaceBelow = vh - anchor.bottom - GAP - MARGIN;
    const spaceAbove = anchor.top - GAP - MARGIN;
    const placeBelow = spaceBelow >= EST_CARD_H || spaceBelow >= spaceAbove;
    const maxH = Math.max(120, placeBelow ? spaceBelow : spaceAbove);
    let cardTop = placeBelow ? anchor.bottom + GAP : anchor.top - GAP - Math.min(EST_CARD_H, maxH);
    cardTop = Math.min(Math.max(cardTop, MARGIN), vh - MARGIN - Math.min(EST_CARD_H, maxH));
    shell.style.position = "fixed";
    shell.style.left = `${cardLeft - SAFETY}px`;
    shell.style.top = `${cardTop - SAFETY}px`;
    shell.style.setProperty("--dag-card-maxh", `${Math.round(maxH)}px`);
  }

  function hoverDialectHtml(dag, mode, elems, rowId) {
    // Raw / relationship cells have no rule to narrate.
    if (dag.type === "raw") {
      return `<span class="dag-hovercard-gt">${tt("hoverGroundTruth", dag.datatype ? esc(dag.datatype) : "")}</span>`;
    }
    if (dag.type === "relationship") {
      return `<span class="dag-hovercard-gt">${esc(tt("hoverRelPointer"))}</span>`;
    }
    if (mode === "rulespeak" && elems.rulespeak) {
      const rs = ruleSpeakFor(dag.table, dag.field);
      // The same rule body the field's page shows (ruleHtml): a priority ladder is
      // numbered steps here too, never one run-on sentence.
      const body = rs && rs.rule
        ? (rs.structure
          ? `<div class="dag-hovercard-english-text dag-hovercard-outline">${ruleHtml(rs, tonedRefs(rs), rowId == null ? null : stepFor(dag.table, dag.field, rowId))}</div>`
          : `<span class="dag-hovercard-english-text">${ruleHtml(rs, tonedRefs(rs))}</span>`)
        : `<span class="dag-hovercard-english-text dag-hovercard-muted">${esc(tt("hoverNoRule"))}</span>`;
      return `<span class="dag-hovercard-english"><span class="dag-hovercard-label">${esc(tt("inRuleSpeak"))}</span>${body}</span>`;
    }
    if (mode === "english" && elems.english) {
      const rs = ruleSpeakFor(dag.table, dag.field);
      const plain = plainReading(dag.table, dag.field, dag.formula);
      const body = plain
        ? `<span class="dag-hovercard-english-text">${linkifyHtml(plain.text, tonedRefs(rs))}${esc(plain.end)}</span>`
        : `<span class="dag-hovercard-english-text dag-hovercard-muted">${esc(tt("hoverNoFormula"))}</span>`;
      return `<span class="dag-hovercard-english"><span class="dag-hovercard-label">${esc(tt("inEnglish"))}</span>${body}</span>`;
    }
    if (mode === "formula" && elems.formula) {
      const body = dag.formula
        ? `<span class="dag-hovercard-formula-code">${formulaHtml(dag.formula, dag.table)}</span>`
        : `<span class="dag-hovercard-english-text dag-hovercard-muted">${esc(tt("hoverNoFormula"))}</span>`;
      return `<span class="dag-hovercard-formula"><span class="dag-hovercard-label">${esc(tt("asFormula"))}</span>${body}</span>`;
    }
    return ""; // current dialect disabled in settings → show nothing for it
  }

  // The popup's whole content, as HTML text ("" for a field the rulebook does not have).
  // `rowId` (the cell's data-er-row) adds the witness of that row, when the host
  // named the generated TypeScript: the value, the step that applied, and what it
  // came from, each line opening down to the stored values.
  function hoverCardHtml(table, field, rowId) {
    ensureRouting();
    const dag = R().resolveDag(table, field);
    if (!dag) return "";
    const tone = R().typeTone(dag.type);
    const mode = prefs.narrationMode();
    const elems = prefs.docElements();

    // Every popup links to the page of the field it is about: its name in the head
    // and "Open the full page" at the foot. Until 2026-11-22 the only links in a
    // popup were to the OTHER fields its rule names, so a raw field's popup, or a
    // rule that names nothing, had no way into the docs but a double-click.
    const selfLink = (className, labelHtml) =>
      `<a class="${className}" href="${esc(refHref(table, field))}" data-ref-table="${esc(table)}" data-ref-field="${esc(field)}">${labelHtml}</a>`;

    let inputsHtml = "";
    if (elems.inputs && dag.upstream.length) {
      inputsHtml = `<span class="dag-hovercard-inputs"><span class="dag-hovercard-label">${esc(tt("hoverInputs"))} <span class="dag-hovercard-count">${dag.upstream.length}</span></span>` +
        `<span class="dag-hovercard-input-row">${dag.upstream.map((u) => pillHtml(u.table, u.field)).join("")}</span></span>`;
    }

    return `<span class="dag-hovercard dag-hovercard-${tone}" role="dialog">` +
      `<span class="dag-hovercard-head">${typeBadgeHtml(dag.type, "sm")}` +
      `<span class="dag-hovercard-where"><a class="dag-hovercard-table" href="${esc(tableHrefSafe(table))}" data-table-link="${esc(table)}">${esc(table)}</a>` +
      `<span class="dag-hovercard-dot">·</span>${selfLink("dag-hovercard-field", esc(humanizeField(field)))}</span></span>` +
      (elems.desc && dag.description ? `<span class="dag-hovercard-desc">${esc(dag.description)}</span>` : "") +
      hoverDialectHtml(dag, mode, elems, rowId) +
      (rowId != null && rowId !== "" && witnessOn()
        ? `<span class="dag-hovercard-witness"><span class="dag-hovercard-label">${esc(tt("witnessTitle"))}</span>${witnessHtml(table, field, rowId, { depth: 1 })}</span>`
        : "") +
      inputsHtml +
      `<span class="dag-hovercard-foot">${selfLink("dag-hovercard-open", `${esc(tt("hoverOpenPage"))} ${onward()}`)}` +
      `<span class="dag-hovercard-hint">${esc(tt("hoverHint"))}</span></span>` +
      `</span>`;
  }

  function openHoverCard(table, field, anchor, rowId) {
    cancelHoverClose();
    removeHoverCard();
    if (!anchor) return;
    const html = hoverCardHtml(table, field, rowId);
    if (!html) return;

    const shell = document.createElement("span");
    speaks(shell).className = "dag-hovercard-shell dag-hovercard-shell-fixed";
    shell.addEventListener("mouseenter", cancelHoverClose);
    shell.addEventListener("mouseleave", scheduleHoverClose);
    shell.innerHTML = html;
    // The witness is computed on first use: draw the popup again when it answers, if it is still this popup.
    if (rowId != null && rowId !== "" && witnessState(() => { if (activeHoverCard === shell) openHoverCard(table, field, anchor, rowId); }) === "loading") { /* waits */ }
    // Any link out of a cell's popup opens that page on the cell's row.
    if (rowId != null && rowId !== "") shell.querySelectorAll("a[href]").forEach((a) => a.addEventListener("click", () => setWitnessRow(table, rowId), true));
    wireWitness(shell);

    wireRefLinks(shell);
    shell.querySelectorAll("[data-table-link]").forEach((a) => {
      a.addEventListener("click", (e) => {
        if (e.metaKey || e.ctrlKey || e.shiftKey) return;
        e.preventDefault();
        routing.navigateTable(a.getAttribute("data-table-link"));
      });
    });
    // A link in the popup leaves the page the cell is on (or re-renders it): take the popup with it.
    shell.querySelectorAll("a[href]").forEach((a) => a.addEventListener("click", (e) => {
      if (!(e.metaKey || e.ctrlKey || e.shiftKey)) removeHoverCard();
    }));
    positionHoverCard(shell, anchor);
    document.body.appendChild(shell);
    activeHoverCard = shell;
  }

  function attachHoverToGlyph(glyphEl, table, field, rowOf) {
    glyphEl.addEventListener("mouseenter", () => openHoverCard(table, field, glyphEl.getBoundingClientRect(), rowOf ? rowOf() : null));
    glyphEl.addEventListener("mouseleave", scheduleHoverClose);
  }

  // ════════════════════════════════════════════════════════════════════════
  //  INLINE CELL ENHANCEMENT (data-er-dag="Table.Field")
  // ════════════════════════════════════════════════════════════════════════
  function dagType(table, field) {
    const f = R().fieldOf(R().rulebook(), table, field);
    return (f && f.type) || "calculated";
  }
  function enhanceCells(root) {
    ensureRouting();
    root = root || document;
    const glyphsOn = prefs.glyphsOn();
    root.querySelectorAll("[data-er-dag]").forEach((el) => {
      if (el.dataset.erEnhanced) return;
      const spec = el.dataset.erDag;
      if (!spec || !spec.includes(".")) return;
      const [table, field] = spec.split(".", 2);
      if (!R().isDerivedField(table, field)) return;
      el.dataset.erEnhanced = "1";
      el.classList.add("dag-cell");
      if (!glyphsOn) el.classList.add("dag-cell-quiet");
      el.title = glyphsOn ? tt("cellHintOn") : tt("cellHintOff");

      const tone = R().typeTone(dagType(table, field));
      const badge = document.createElement("span");
      badge.className = "dag-cell-badge-wrap";
      const link = renderFieldLink(table, field, "", `dag-cell-fx dag-cell-fx-${tone}`);
      const glyph = document.createElement("span");
      glyph.className = "dag-cell-fx-glyph";
      glyph.textContent = R().typeGlyph(dagType(table, field));
      link.appendChild(glyph);
      // data-er-row="<the row's key>" says which row this cell shows; read when used,
      // so a host that fills it in after the first paint is still heard.
      const rowOf = () => el.getAttribute("data-er-row");
      attachHoverToGlyph(glyph, table, field, rowOf);
      link.addEventListener("click", () => setWitnessRow(table, rowOf()), true);
      el.addEventListener("dblclick", () => setWitnessRow(table, rowOf()), true);
      badge.appendChild(link);
      el.appendChild(badge);

      el.addEventListener("dblclick", (e) => {
        if (e.target.closest("a, button, input, select, textarea")) return;
        e.preventDefault();
        routing.navigate(table, field);
      });
    });
  }

  // ════════════════════════════════════════════════════════════════════════
  //  NAVIGATION: a left nav on every page, grouped tables, a filter, a table's
  //  fields in tabs, and the rulebook as a graph.
  //
  //  Big rulebooks were unnavigable: the index was one long page of cards, and
  //  getting from one table to another meant scrolling back past all of them. The
  //  nav is always on screen (sticky, scrolling by itself), so any table or field is
  //  one click away from any page. Every builder below returns HTML text; the page
  //  only wires what that text says (a click, a typed filter).
  // ════════════════════════════════════════════════════════════════════════
  const NAV = { q: "", derivedOnly: false, scroll: 0, view: null, tab: "all", fieldQ: "", closed: {} };
  // Below this many tables there is nothing to group: one list.
  const GROUP_FROM = 9;
  const NAV_MIN_WIDTH = 900;

  let modelMemo = null;
  function graphModel() {
    if (modelMemo) return modelMemo;
    const tables = R().listTables();
    const known = new Set(tables.map((t) => t.table));
    const edges = [];
    const fields = {};
    for (const t of tables) {
      const cols = R().tableColumns(t.table);
      fields[t.table] = cols;
      for (const c of cols) {
        if (c.type === "relationship" && c.relatedTo && c.relatedTo !== t.table && known.has(c.relatedTo)) {
          edges.push({ from: t.table, to: c.relatedTo, field: c.field });
        }
      }
    }
    modelMemo = { tables, edges, fields, info: new Map(tables.map((t) => [t.table, t])) };
    return modelMemo;
  }

  // "CorpusDomainRuns" -> "Corpus"; "ERBTables" -> "ERB"; "order_lines" -> "order".
  function leadingWord(name) {
    const head = String(name).replace(/^[^A-Za-z0-9]+/, "").split(/[\s_-]+/)[0];
    const m = /^(?:[A-Z]+(?=[A-Z][a-z])|[A-Z][a-z0-9]*|[A-Z]+[0-9]*|[a-z0-9]+)/.exec(head);
    return m ? m[0] : head;
  }

  // Tables that share a leading word are a family; a table with none joins the family
  // most of its neighbours are in; the rest are "Other". Small rulebooks are one list.
  function groupTables(tables, edges) {
    const names = tables.map((t) => t.table).sort((a, b) => a.localeCompare(b));
    if (names.length < GROUP_FROM) return [{ key: "", label: null, tables: names }];
    // Matched by the singular ("Substrate" and "Substrates" are one family); the
    // family is called by the shortest spelling its members use.
    const singular = (w) => (w.length > 3 ? w.replace(/ies$/i, "y").replace(/s$/i, "") : w);
    const byWord = new Map();
    for (const n of names) {
      const w = leadingWord(n), key = singular(w).toLowerCase();
      if (!byWord.has(key)) byWord.set(key, { words: [], members: [] });
      byWord.get(key).words.push(w);
      byWord.get(key).members.push(n);
    }
    const family = new Map();   // table -> its family's key
    const label = new Map();    // family key -> what it is called
    for (const [key, f] of byWord) {
      if (f.members.length < 2 || key.length < 3) continue;
      label.set(key, f.words.slice().sort((a, b) => a.length - b.length || a.localeCompare(b))[0]);
      for (const n of f.members) family.set(n, key);
    }
    if (!family.size) return [{ key: "", label: null, tables: names }];
    const neighbours = new Map(names.map((n) => [n, []]));
    for (const e of edges) { if (neighbours.has(e.from) && neighbours.has(e.to)) { neighbours.get(e.from).push(e.to); neighbours.get(e.to).push(e.from); } }
    const groups = new Map();
    const put = (w, n) => { if (!groups.has(w)) groups.set(w, []); groups.get(w).push(n); };
    const strays = [];
    for (const n of names) { if (family.has(n)) put(family.get(n), n); else strays.push(n); }
    for (const n of strays) {
      const tally = new Map();
      for (const m of neighbours.get(n)) if (family.has(m)) tally.set(family.get(m), (tally.get(family.get(m)) || 0) + 1);
      let best = null, bestN = 0;
      for (const [w, c] of [...tally].sort((a, b) => a[0].localeCompare(b[0]))) if (c > bestN) { best = w; bestN = c; }
      put(best || "", n);
    }
    const ordered = [...groups.keys()].filter((w) => w !== "").map((w) => ({ key: w, label: label.get(w), tables: groups.get(w) }))
      .sort((a, b) => a.label.localeCompare(b.label));
    if (groups.has("")) ordered.push({ key: "", label: tt("groupOther"), tables: groups.get("") });
    return ordered;
  }

  const fieldKind = (col) =>
    col.type === "relationship" ? "relationships" : (col.type === "raw" ? "stored" : "derived");

  function navMatches(table, cols) {
    const q = NAV.q.trim().toLowerCase();
    const m = graphModel();
    const info = m.info.get(table);
    if (NAV.derivedOnly && !(info && info.derivedCount > 0)) return null;
    if (!q) return { fields: [] };
    if (table.toLowerCase().includes(q) || humanizeField(table).includes(q)) return { fields: [] };
    const hit = cols.filter((c) => c.field.toLowerCase().includes(q) || humanizeField(c.field).includes(q));
    return hit.length ? { fields: hit } : null;
  }

  function navTableHtml(table, ctx) {
    const m = graphModel();
    const cols = m.fields[table] || [];
    const hit = navMatches(table, cols);
    if (!hit) return "";
    const current = ctx && ctx.table === table;
    const info = m.info.get(table);
    let html = `<a class="dag-nav-table${current ? " is-current" : ""}" href="${esc(tableHrefSafe(table))}" data-table-link="${esc(table)}" title="${esc(humanizeField(table))}">` +
      `<span class="dag-nav-name">${esc(table)}</span>` +
      (info && info.derivedCount ? `<span class="dag-nav-badge" title="${esc(tt("derivedCount", info.derivedCount))}">ƒ ${info.derivedCount}</span>` : "") + `</a>`;
    const shown = current ? cols : hit.fields.slice(0, 8);
    if (shown.length) {
      html += `<ul class="dag-nav-fields">` + shown.map((c) => {
        const here = current && ctx.field === c.field;
        return `<li><a class="dag-nav-field dag-nav-field-${R().typeTone(c.type)}${here ? " is-current" : ""}" href="${esc(refHref(table, c.field))}" data-ref-table="${esc(table)}" data-ref-field="${esc(c.field)}">` +
          `<span class="dag-nav-glyph">${esc(R().typeGlyph(c.type))}</span><span class="dag-nav-fname">${esc(c.field)}</span></a></li>`;
      }).join("") + `</ul>`;
    }
    return `<div class="dag-nav-row">${html}</div>`;
  }

  function navGroupsHtml(groups, ctx) {
    const filtering = !!(NAV.q.trim() || NAV.derivedOnly);
    const body = groups.map((g) => {
      const rows = g.tables.map((t) => navTableHtml(t, ctx)).filter(Boolean);
      if (!rows.length) return "";
      if (g.label == null) return rows.join("");
      const holdsCurrent = !!(ctx && ctx.table && g.tables.includes(ctx.table));
      const closed = NAV.closed[g.key] === true;
      const open = filtering || holdsCurrent || (groups.length <= 3 && !closed) || (NAV.closed[g.key] === false);
      return `<details class="dag-nav-group${holdsCurrent ? " has-current" : ""}" data-group="${esc(g.key)}"${open ? " open" : ""}>` +
        `<summary class="dag-nav-group-title"><span class="dag-nav-group-name">${esc(g.label)}</span><span class="dag-nav-group-n">${rows.length}</span></summary>${rows.join("")}</details>`;
    }).join("");
    return body || `<p class="dag-nav-empty">${esc(tt("navNoMatch"))}</p>`;
  }

  function navHtml(ctx) {
    const m = graphModel();
    const groups = groupTables(m.tables, m.edges);
    return `<div class="dag-nav-head">` +
      `<a class="dag-nav-index${ctx && ctx.table ? "" : " is-current"}" href="${esc(indexPath())}" data-table-link="">▦ ${esc(tt("tablesCrumb"))}<span class="dag-nav-group-n">${m.tables.length}</span></a>` +
      `<input type="search" class="dag-nav-q" placeholder="${esc(tt("navSearch"))}" aria-label="${esc(tt("navSearch"))}" value="${esc(NAV.q)}">` +
      `<label class="dag-nav-chk"><input type="checkbox" class="dag-nav-derived"${NAV.derivedOnly ? " checked" : ""}> ${esc(tt("navDerivedOnly"))}</label></div>` +
      `<div class="dag-nav-body">${navGroupsHtml(groups, ctx || {})}</div>`;
  }

  // The filter is shared by the nav and the index, so it is set in one place.
  function navFilter(q, opts) {
    NAV.q = q == null ? "" : String(q);
    if (opts && "derivedOnly" in opts) NAV.derivedOnly = !!opts.derivedOnly;
  }

  function wireLinks(root) {
    root.querySelectorAll("[data-table-link]").forEach((a) => {
      a.addEventListener("click", (e) => { if (e.metaKey || e.ctrlKey || e.shiftKey) return; e.preventDefault(); routing.navigateTable(a.getAttribute("data-table-link")); });
    });
    wireRefLinks(root);
  }

  function wireNav(nav, ctx) {
    const body = () => nav.querySelector(".dag-nav-body");
    const redraw = () => {
      const keep = body().scrollTop;
      body().innerHTML = navGroupsHtml(groupTables(graphModel().tables, graphModel().edges), ctx);
      body().scrollTop = keep;
      wireLinks(body()); wireGroups(nav);
    };
    const q = nav.querySelector(".dag-nav-q");
    q.addEventListener("input", () => { navFilter(q.value); redraw(); });
    nav.querySelector(".dag-nav-derived").addEventListener("change", (e) => { NAV.derivedOnly = e.target.checked; redraw(); });
    wireLinks(nav); wireGroups(nav);
    const b = body();
    b.scrollTop = NAV.scroll;
    b.addEventListener("scroll", () => { NAV.scroll = b.scrollTop; });
    const cur = nav.querySelector(".is-current.dag-nav-field, .dag-nav-table.is-current");
    if (cur && cur.scrollIntoView && NAV.scroll === 0) cur.scrollIntoView({ block: "center" });
  }
  function refreshNavBody(nav, ctx) {
    const body = nav.querySelector(".dag-nav-body");
    body.innerHTML = navGroupsHtml(groupTables(graphModel().tables, graphModel().edges), ctx);
    wireLinks(body); wireGroups(nav);
  }
  function wireGroups(root) {
    root.querySelectorAll("details.dag-nav-group").forEach((d) => d.addEventListener("toggle", () => { NAV.closed[d.getAttribute("data-group")] = !d.open; }));
  }

  // The page and its nav side by side. Only for the explainer's own outlet (its own
  // page, or a hash-mode host's): a modal or a framework's own container is narrow
  // and not ours to lay out.
  function shellWanted(container) {
    if (config.nav === false) return false;
    if (config.nav === true) return true;
    // A host that gives the explainer a container of its own (a React route) says so
    // on the container: data-explainer-nav.
    if (container.getAttribute && container.getAttribute("data-explainer-nav") === "true") return true;
    return container.id === "effortless-explainer-outlet";
  }
  function withShell(container, ctx) {
    if (!shellWanted(container)) return;
    const main = document.createElement("div");
    main.className = "dag-main";
    while (container.firstChild) main.appendChild(container.firstChild);
    const nav = document.createElement("nav");
    nav.className = "dag-nav";
    nav.setAttribute("aria-label", tt("tablesCrumb"));
    nav.innerHTML = navHtml(ctx);
    container.classList.add("dag-has-nav");
    container.appendChild(nav);
    container.appendChild(main);
    wireNav(nav, ctx);
  }

  // ── a table's fields in tabs ────────────────────────────────────────────
  function fieldTabsHtml(table) {
    const cols = graphModel().fields[table] || [];
    const n = (kind) => cols.filter((c) => kind === "all" || fieldKind(c) === kind).length;
    const tab = (kind, key) => {
      const count = n(kind);
      if (kind !== "all" && !count) return "";
      return `<button type="button" role="tab" class="dag-tab${NAV.tab === kind ? " is-active" : ""}" data-tab="${kind}" aria-selected="${NAV.tab === kind}">${esc(tt(key))}<span class="dag-tab-n">${count}</span></button>`;
    };
    return `<div class="dag-tabs" role="tablist">${tab("all", "tabAll")}${tab("derived", "tabDerived")}${tab("stored", "tabStored")}${tab("relationships", "tabRelationships")}` +
      `<input type="search" class="dag-tab-q" placeholder="${esc(tt("filterFields"))}" aria-label="${esc(tt("filterFields"))}" value="${esc(NAV.fieldQ)}"></div>`;
  }
  function wireFieldTabs(container) {
    const rows = Array.from(container.querySelectorAll(".dag-col-list > [data-kind]"));
    const tabs = container.querySelector(".dag-tabs");
    if (!tabs) return;
    const apply = () => {
      const q = NAV.fieldQ.trim().toLowerCase();
      rows.forEach((r) => {
        const kindOk = NAV.tab === "all" || r.getAttribute("data-kind") === NAV.tab;
        const textOk = !q || (r.getAttribute("data-name") || "").includes(q);
        r.hidden = !(kindOk && textOk);
      });
      tabs.querySelectorAll(".dag-tab").forEach((t) => {
        const on = t.getAttribute("data-tab") === NAV.tab;
        t.classList.toggle("is-active", on); t.setAttribute("aria-selected", String(on));
      });
    };
    tabs.querySelectorAll(".dag-tab").forEach((t) => t.addEventListener("click", () => { NAV.tab = t.getAttribute("data-tab"); apply(); }));
    const q = tabs.querySelector(".dag-tab-q");
    q.addEventListener("input", () => { NAV.fieldQ = q.value; apply(); });
    apply();
  }

  // ── the rulebook as a graph ─────────────────────────────────────────────
  // Boxes in columns: what a table points at is to its left, so a table's columns
  // read as how far it is from the tables nothing else feeds.
  function graphSvg(tables, edges, opts) {
    const names = tables.map((t) => t.table);
    const set = new Set(names);
    const info = new Map(tables.map((t) => [t.table, t]));
    const out = new Map(names.map((n) => [n, []]));
    const into = new Map(names.map((n) => [n, []]));
    const live = edges.filter((e) => e.from !== e.to && set.has(e.from) && set.has(e.to));
    for (const e of live) { out.get(e.from).push(e.to); into.get(e.to).push(e.from); }
    const depth = new Map();
    const visiting = new Set();
    const depthOf = (n) => {
      if (depth.has(n)) return depth.get(n);
      if (visiting.has(n)) return 0;           // a cycle: stop here, do not loop
      visiting.add(n);
      let d = 0;
      for (const t of out.get(n)) d = Math.max(d, 1 + depthOf(t));
      visiting.delete(n);
      depth.set(n, d);
      return d;
    };
    names.forEach(depthOf);
    const maxD = Math.max(0, ...names.map((n) => depth.get(n)));
    // Column 0 = what points at nothing; a table sits one column right of the furthest
    // table it points at, so every line runs from right to left.
    const byDepth = Array.from({ length: maxD + 1 }, () => []);
    for (const n of names.slice().sort((a, b) => a.localeCompare(b))) byDepth[depth.get(n)].push(n);
    const layers = byDepth.filter((l) => l.length);   // a cycle can leave a depth empty: no blank column
    const BW = 164, BH = 30, GX = 96, GY = 14;
    const pos = new Map();
    layers.forEach((layer, li) => {
      if (li > 0) {   // one pass: sit near the neighbours already placed
        const bary = (n) => { const ps = [...out.get(n), ...into.get(n)].filter((x) => pos.has(x)).map((x) => pos.get(x).row); return ps.length ? ps.reduce((a, b) => a + b, 0) / ps.length : 1e9; };
        layer.sort((a, b) => bary(a) - bary(b) || a.localeCompare(b));
      }
      layer.forEach((n, ri) => pos.set(n, { col: li, row: ri, x: li * (BW + GX), y: ri * (BH + GY) }));
    });
    const rows = Math.max(1, ...layers.map((l) => l.length));
    const W = layers.length * (BW + GX) - GX, H = rows * (BH + GY) - GY;
    const edgeSvg = live.map((e) => {
      const a = pos.get(e.from), b = pos.get(e.to);
      const x1 = a.x, y1 = a.y + BH / 2, x2 = b.x + BW, y2 = b.y + BH / 2, dx = Math.max(24, (x1 - x2) / 2);
      return `<path class="dag-gedge" d="M${x1},${y1} C${x1 - dx},${y1} ${x2 + dx},${y2} ${x2},${y2}" marker-end="url(#dag-garrow)"><title>${esc(e.from)}.${esc(e.field || e.to)} → ${esc(e.to)}</title></path>`;
    }).join("");
    const nodeSvg = names.map((n) => {
      const p = pos.get(n), t = info.get(n);
      const label = n.length > 22 ? n.slice(0, 21) + "…" : n;
      return `<g class="dag-gnode${t && t.derivedCount ? " has-derived" : ""}${opts && opts.current === n ? " is-current" : ""}" data-table-link="${esc(n)}" data-x="${p.x}" transform="translate(${p.x},${p.y})">` +
        `<a href="${esc(tableHrefSafe(n))}" data-table-link="${esc(n)}" data-x="${p.x}"><rect width="${BW}" height="${BH}" rx="6"/><text x="${BW / 2}" y="${BH / 2 + 4}" text-anchor="middle">${esc(label)}</text><title>${esc(n)}</title></a></g>`;
    }).join("");
    return `<svg class="dag-graph" viewBox="-4 -4 ${W + 8} ${H + 8}" width="${W + 8}" height="${H + 8}" role="img" aria-label="${esc(tt("viewGraph"))}">` +
      `<defs><marker id="dag-garrow" viewBox="0 0 8 8" refX="7" refY="4" markerWidth="7" markerHeight="7" orient="auto-start-reverse"><path d="M0,0 L8,4 L0,8 z" class="dag-garrow"/></marker></defs>` +
      edgeSvg + nodeSvg + `</svg>`;
  }

  function indexViewsHtml(view) {
    const v = (key, label) => `<button type="button" role="tab" class="dag-tab${view === key ? " is-active" : ""}" data-view="${key}" aria-selected="${view === key}">${esc(tt(label))}</button>`;
    return `<div class="dag-tabs dag-views" role="tablist">${v("groups", "viewGroups")}${v("list", "viewList")}${v("graph", "viewGraph")}` +
      `<input type="search" class="dag-tab-q dag-index-q" placeholder="${esc(tt("navSearch"))}" aria-label="${esc(tt("navSearch"))}" value="${esc(NAV.q)}"></div>`;
  }

  function indexCardHtml(t) {
    return `<a class="dag-table-card" href="${esc(tableHrefSafe(t.table))}" data-table-link="${esc(t.table)}">` +
      `<span class="dag-table-card-name">${esc(t.table)}</span>` +
      `<span class="dag-table-card-human">${esc(humanizeField(t.table))}</span>` +
      `<span class="dag-table-card-meta">${esc(tt("fieldsCount", t.fieldCount))} · ${esc(tt("derivedCount", t.derivedCount))} · ${esc(tt("relCount", t.relationshipCount))}</span>` +
      (t.description ? `<span class="dag-table-card-desc">${esc(t.description)}</span>` : "") + `</a>`;
  }
  function indexBodyHtml(view) {
    const m = graphModel();
    const keep = (t) => navMatches(t.table, m.fields[t.table] || []);
    const shown = m.tables.filter(keep);
    if (!shown.length) return `<p class="dag-nav-empty">${esc(tt("navNoMatch"))}</p>`;
    const shownSet = new Set(shown.map((t) => t.table));
    const groups = groupTables(shown, m.edges.filter((e) => shownSet.has(e.from) && shownSet.has(e.to)));
    if (view === "list") {
      return `<table class="dag-idx-list"><tbody>` + shown.slice().sort((a, b) => a.table.localeCompare(b.table)).map((t) =>
        `<tr><td><a href="${esc(tableHrefSafe(t.table))}" data-table-link="${esc(t.table)}">${esc(t.table)}</a></td>` +
        `<td class="dag-idx-n">${t.fieldCount}</td><td class="dag-idx-n">ƒ ${t.derivedCount}</td><td class="dag-idx-n">⇢ ${t.relationshipCount}</td>` +
        `<td class="dag-idx-desc">${esc(t.description || "")}</td></tr>`).join("") + `</tbody></table>`;
    }
    const sectionOf = (g, inner) => g.label == null ? inner :
      `<details class="dag-idx-group" open><summary class="dag-idx-group-title"><span>${esc(g.label)}</span><span class="dag-nav-group-n">${esc(tt("tablesCount", g.tables.length))}</span></summary>${inner}</details>`;
    const byName = new Map(shown.map((t) => [t.table, t]));
    if (view === "graph") {
      return `<p class="dag-graph-hint muted">${esc(tt("graphHint"))}</p>` + groups.map((g) =>
        sectionOf(g, `<div class="dag-graph-wrap">${graphSvg(g.tables.map((n) => byName.get(n)), m.edges)}</div>`)).join("");
    }
    return groups.map((g) => sectionOf(g, `<div class="dag-table-grid">${g.tables.map((n) => indexCardHtml(byName.get(n))).join("")}</div>`)).join("");
  }

  // ════════════════════════════════════════════════════════════════════════
  //  ROUTING DISPATCH
  // ════════════════════════════════════════════════════════════════════════
  function parseHashRoute() {
    const h = (window.location.hash || "").replace(/^#/, "");
    const m = h.match(/^\/dag\/([^/]+)(?:\/([^/]+))?/);
    if (!m) return null;
    return { table: decodeURIComponent(m[1]), field: m[2] ? decodeURIComponent(m[2]) : null };
  }
  function parsePathRoute() {
    const path = window.location.pathname;
    const m = path.match(/\/dag\/?([^/]*)\/?([^/]*)/);
    if (!m) return null;
    return { table: m[1] ? decodeURIComponent(m[1]) : null, field: m[2] ? decodeURIComponent(m[2]) : null };
  }
  function renderFromHash() {
    const outlet = document.getElementById("effortless-explainer-outlet");
    if (!outlet) return;
    const route = routing.cfg.mode === "path" ? parsePathRoute() : parseHashRoute();
    if (!route || (!route.table && !route.field)) { renderTablesIndex(outlet); return; }
    if (route.table && route.field) renderFieldPage(outlet, route.table, route.field);
    else if (route.table) renderTablePage(outlet, route.table);
    else renderTablesIndex(outlet);
  }

  // ════════════════════════════════════════════════════════════════════════
  //  CORNER CONTROL: provenance on/off, and the workbook download
  //
  //  Two icons in the corner the app gives it; the pointer over it (or keyboard
  //  focus, or a tap) opens it to what each does. It stays where it is put:
  //    init({ corner: "bottom-right" })        the explainer pins it to that corner
  //    init({ mountToggle: "#my-element" })    the app's own element holds it
  //  (the old pill's CSS pinned it to the top right of the window either way).
  //    init({ xlsx: "/files/my-rulebook.xlsx" })   adds the download
  //    init({ provenance: "off" })                 starts with the ƒ badges off
  // ════════════════════════════════════════════════════════════════════════
  const CORNERS = ["top-left", "top-right", "bottom-left", "bottom-right"];
  const DOWNLOAD_ICON =
    '<svg viewBox="0 0 16 16" width="14" height="14" aria-hidden="true" focusable="false">' +
    '<path d="M8 1.75v8M4.75 6.75 8 10l3.25-3.25M2.5 12v1.5a.75.75 0 0 0 .75.75h9.5a.75.75 0 0 0 .75-.75V12" ' +
    'fill="none" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/></svg>';
  function setGlyphsOn(on) {
    prefs.setGlyphs(on);
    document.querySelectorAll(".dag-cell").forEach((el) => el.classList.toggle("dag-cell-quiet", !on));
  }
  function cornerHtml() {
    const on = prefs.glyphsOn();
    const place = config.corner ? config.corner.split("-") : null;
    const cls = "dag-corner" + (place ? ` dag-corner-${place[0]} dag-corner-${place[1]} dag-corner-fixed` : "");
    const download = config.xlsx
      ? `<a class="dag-corner-item dag-corner-download" href="${esc(config.xlsx)}" download title="${esc(tt("downloadXlsxTitle"))}">` +
        `<span class="dag-corner-icon dag-corner-icon-download">${DOWNLOAD_ICON}</span>` +
        `<span class="dag-corner-label">${esc(tt("downloadXlsx"))}</span></a>`
      : "";
    const toggle =
      `<button type="button" class="dag-corner-item dag-toggle ${on ? "is-on" : "is-off"}" aria-pressed="${on}" title="${esc(on ? tt("provOnTitle") : tt("provOffTitle"))}">` +
      `<span class="dag-corner-icon dag-toggle-glyph">ƒ</span>` +
      `<span class="dag-corner-label">${esc(on ? tt("provOn") : tt("provOff"))}<span class="dag-toggle-knob" aria-hidden="true"></span></span></button>`;
    return `<div class="${cls}">${download}${toggle}</div>`;
  }
  function mountCorner(container) {
    const holder = speaks(document.createElement("div"));
    holder.className = "dag-corner-holder";
    let openedByTap = false;
    const draw = () => {
      holder.innerHTML = cornerHtml();
      const control = holder.querySelector(".dag-corner");
      if (!control) return;
      if (openedByTap) control.classList.add("is-open");
      const btn = control.querySelector(".dag-toggle");
      if (btn) btn.onclick = () => setGlyphsOn(!prefs.glyphsOn());
      // A finger has no hover: the first tap opens the control, the next one acts.
      control.addEventListener("pointerdown", (e) => {
        if (e.pointerType !== "touch" || openedByTap) return;
        openedByTap = true;
        control.classList.add("is-open");
        const swallow = (c) => { c.preventDefault(); c.stopPropagation(); };
        control.addEventListener("click", swallow, { capture: true, once: true });
      });
    };
    draw();
    prefs.subscribe(draw);
    document.addEventListener("pointerdown", (e) => {
      if (!openedByTap || holder.contains(e.target)) return;
      openedByTap = false;
      const control = holder.querySelector(".dag-corner");
      if (control) control.classList.remove("is-open");
    });
    container.appendChild(holder);
    return holder;
  }
  // The name hosts already call. `container` is the app's own element.
  function mountToggle(container) {
    return mountCorner(container || document.body);
  }

  // ════════════════════════════════════════════════════════════════════════
  //  INIT
  // ════════════════════════════════════════════════════════════════════════
  let _outletWired = false;
  let _prefsWired = false;
  function init(opts) {
    config = Object.assign({}, config, opts || {});
    if (config.corner != null && !CORNERS.includes(config.corner)) {
      throw new Error(`EffortlessExplainer.init: corner ${JSON.stringify(config.corner)} is not a corner. Use one of: ${CORNERS.join(", ")}.`);
    }
    if (config.provenance != null && config.provenance !== "on" && config.provenance !== "off") {
      throw new Error(`EffortlessExplainer.init: provenance ${JSON.stringify(config.provenance)} is not a starting value. Use one of: on, off.`);
    }
    lang = CHROME[config.lang] ? config.lang : preferredLanguage();
    routing = buildRouting();

    // The witness: the folder holding rulebook-to-typescript's output (and, for an
    // app's live rows instead of the rulebook's, `rows`). Nothing is fetched until
    // a witness is first shown.
    // The folder is remembered for this site, so the tool's own pages/*.html (which
    // call init themselves, in path mode) witness too; typescript: false forgets it.
    try {
      if (config.typescript) localStorage.setItem(TYPESCRIPT_KEY, String(config.typescript));
      else if (config.typescript === false) localStorage.removeItem(TYPESCRIPT_KEY);
      else config.typescript = localStorage.getItem(TYPESCRIPT_KEY) || null;
    } catch { /* no storage: the option alone decides */ }
    if (W()) configureWitness();
    // No folder named: look in this explainer's own folder (below).
    if (config.typescript == null) findOwnTypescript();

    if (config.enhance !== false) enhanceCells(document);
    if (config.mountToggle) {
      const mount = typeof config.mountToggle === "string" ? document.querySelector(config.mountToggle) : config.mountToggle;
      if (mount) mountCorner(mount);
      else console.error(`EffortlessExplainer.init: mountToggle ${JSON.stringify(config.mountToggle)} matches no element on this page, so the provenance control is not shown.`);
    } else if (config.corner) {
      mountCorner(document.body);
    }

    // Re-render the page on screen when a pref changes (narration mode, gear
    // doc-element toggles, glyphs) so the choice takes effect live. Wired ONCE
    // and host-agnostic: outlet/hash hosts re-render from the hash, while hosts
    // that render into their own container (frameworks) re-render that same
    // container via lastRender. Without this, a framework host's narration
    // slider / gear would appear dead because nothing re-rendered.
    if (!_prefsWired) {
      prefs.subscribe(() => {
        if (document.getElementById("effortless-explainer-outlet")) renderFromHash();
        else rerenderCurrent();
      });
      _prefsWired = true;
    }

    if (document.getElementById("effortless-explainer-outlet")) {
      renderFromHash();
      if (!_outletWired) {
        window.addEventListener("hashchange", renderFromHash);
        _outletWired = true;
      }
    }

    return { routing, enhanceCells, renderFromHash, openModal, closeModal, modalBack, setGlyphsOn, rerenderCurrent };
  }

  global.EffortlessExplainer = {
    init,
    enhanceCells,
    renderTablesIndex,
    renderTablePage,
    renderFieldPage,
    renderFromHash,
    openModal,
    closeModal,
    modalBack,
    setGlyphsOn,
    mountToggle,
    cornerHtml,
    renderGear, renderNarrationSlider, availableLanguages, preferredLanguage, setLanguage, language: () => lang,
    shellWanted, groupTables, navHtml, navGroupsHtml, navFilter, fieldTabsHtml, fieldKind, graphSvg, indexViewsHtml, indexBodyHtml,
    rerenderCurrent,
    hoverCardHtml,
    ruleBodyHtml,
    witnessHtml,
    witnessPickerHtml,
    // Resolves when the witness has been computed (rejects with why it could not be).
    witnessReady: () => loadWitnessScript().then(() => { configureWitness(); return W().ready(); }),
    // An app's live rows in place of the rulebook's own: { Table: [row, …] }.
    setRows: (tables) => { config.rows = tables || null; if (W()) { W().setRows(config.rows); } rerenderCurrent(); },
    setWitnessRow,
    prefs,
    // The languages this page's own text is written in, and the names of the texts
    // each has (the tests hold every language to English's list).
    languages: () => Object.keys(CHROME),
    textsOf: (code) => (CHROME[code] ? Object.assign({}, CHROME[code]) : null),
    rightToLeft: (code) => RTL.has(code),
  };
})(typeof window !== "undefined" ? window : globalThis);
