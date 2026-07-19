PYTHON ?= python3
RULEBOOK := effortless-rulebook/procedural-knowledge-ontology-rulebook.json

.PHONY: validate generate test clean

validate:
	$(PYTHON) tools/pko_rulebook_tool.py validate -i $(RULEBOOK)

generate: validate
	$(PYTHON) tools/pko_rulebook_tool.py natural-language-docs -i $(RULEBOOK) -o generated/natural-language-docs.md
	$(PYTHON) tools/pko_rulebook_tool.py financial-sops -i $(RULEBOOK) -o generated/financial-sops.md
	$(PYTHON) tools/pko_rulebook_tool.py text-email-policies -i $(RULEBOOK) -o generated/text-email-policies.md
	$(PYTHON) tools/pko_rulebook_tool.py governance -i $(RULEBOOK) -o generated/governance.md
	$(PYTHON) tools/pko_rulebook_tool.py validation-report -i $(RULEBOOK) -o generated/validation-report.md

test:
	$(PYTHON) -m unittest discover -s tests -v

clean:
	rm -f generated/*.md generated/*-pko-rulebook.json
