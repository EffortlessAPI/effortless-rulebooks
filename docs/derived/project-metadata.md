<!-- GENERATED FILE — DO NOT EDIT. -->
<!-- Source: effortless-platform/effortless-rulebook/effortless-rulebook.json (table: `ProjectMetadata`) -->
<!-- Regenerate with: cd effortless-platform && effortless build -->
<!-- Generated: 2026-05-25T22:56:16Z -->

# Project Metadata

Project overview

## Effortlessly Invariant Rulesbooks

- **ProjectId**: erb-001
- **Name**: Effortlessly Invariant Rulesbooks
- **Purpose**: Host a catalog of independent Effortless projects under rulebook-examples/, each a self-contained rulebook that fully captures one domain and selects the subset of platform substrates it needs. A rulebook alone is sufficient for any frontier LLM to answer any question about the domain or produce a faithful implementation in any language / platform.
- **Architecture**: rulebook-examples/<project>/ — N independent Effortless projects (acme-llc, star-trek, jessica-advanced, jessica-basic, customer-fullname, acme-corporation, is-everything-a-language, effortless-rulesbooks, …). Each project's rulebook JSON is its own hub / durable SSoT; each picks the subset of the platform's 15 substrates it needs (acme-llc exercises all 15; most use 3+). Builds are convergent: downstream artifacts mirror the current rulebook (additions appear, removals disappear). The platform itself (effortless-platform/) is one such project, describing ERB.
- **EntryPoint**: ./start.sh
- **PortalUrl**: http://localhost:7777
- **ProxyUrl**: http://localhost:4242
- **RepositoryRoot**: effortlessly-invariant-rulesbooks/

