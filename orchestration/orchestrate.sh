#!/bin/bash
# =============================================================================
# ORCHESTRATE.SH
# =============================================================================
# Central orchestration for ERB execution substrates.
# Handles: Airtable sync, running tests, viewing results, cleaning.
# =============================================================================

set -e
set -o pipefail  # CRITICAL: Catch failures in piped commands (e.g., bash script | tee)

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
SUBSTRATES_DIR="$PROJECT_ROOT/execution-substrates"
RULEBOOK_EXAMPLES_DIR="$PROJECT_ROOT/rulebook-examples"
ACTIVE_DOMAIN_FILE="$SCRIPT_DIR/active-domain.txt"

# =============================================================================
# COLORS
# =============================================================================
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
MAGENTA='\033[0;35m'
CYAN='\033[0;36m'
WHITE='\033[1;37m'
BOLD='\033[1m'
DIM='\033[2m'
NC='\033[0m' # No Color

# Substrate colors (cycle through for visual distinction)
SUBSTRATE_COLORS=(
    '\033[38;5;214m'  # Orange
    '\033[38;5;118m'  # Bright green
    '\033[38;5;147m'  # Light purple
    '\033[38;5;81m'   # Sky blue
    '\033[38;5;219m'  # Pink
    '\033[38;5;228m'  # Light yellow
    '\033[38;5;123m'  # Aqua
    '\033[38;5;183m'  # Lavender
    '\033[38;5;203m'  # Coral
    '\033[38;5;157m'  # Mint
    '\033[38;5;208m'  # Dark orange
    '\033[38;5;120m'  # Light green
)

# =============================================================================
# PARSE ARGUMENTS
# =============================================================================
CI_MODE=false
DOCKER_MODE=false
while [[ $# -gt 0 ]]; do
    case $1 in
        --ci)
            CI_MODE=true
            shift
            ;;
        --docker)
            DOCKER_MODE=true
            CI_MODE=true  # Docker mode implies CI mode (non-interactive)
            shift
            ;;
        *)
            shift
            ;;
    esac
done

# Also check environment variable for Docker mode
if [ "${ERB_DOCKER_MODE:-}" = "true" ]; then
    DOCKER_MODE=true
    CI_MODE=true
fi

# =============================================================================
# TOOL DETECTION
# =============================================================================
if [ -z "$SSOTME_AVAILABLE" ]; then
    if command -v effortless &> /dev/null; then
        SSOTME_AVAILABLE=true
    else
        SSOTME_AVAILABLE=false
    fi
fi

if command -v psql &> /dev/null; then
    POSTGRES_AVAILABLE=true
else
    POSTGRES_AVAILABLE=false
fi

# =============================================================================
# HELPER FUNCTIONS
# =============================================================================
get_active_domain() {
    if [ -f "$ACTIVE_DOMAIN_FILE" ]; then
        cat "$ACTIVE_DOMAIN_FILE" | tr -d '[:space:]'
    else
        echo "customer-fullname"
    fi
}

set_active_domain() {
    echo "$1" > "$ACTIVE_DOMAIN_FILE"
}

get_domain_rulebook_path() {
    local domain="${1:-$(get_active_domain)}"
    echo "$RULEBOOK_EXAMPLES_DIR/$domain/effortless-rulebook/effortless-rulebook.json"
}

get_project_name() {
    local domain
    domain=$(get_active_domain)
    local effortless_json="$RULEBOOK_EXAMPLES_DIR/$domain/effortless.json"
    if [ -f "$effortless_json" ]; then
        python3 -c "
import json
with open('$effortless_json', 'r') as f:
    config = json.load(f)
print(config.get('Name', '$domain'))
"
    else
        echo "$domain"
    fi
}

list_domains() {
    for d in "$RULEBOOK_EXAMPLES_DIR"/*/; do
        [ -d "$d" ] || continue
        basename "$d"
    done
}

# Canonical substrate ordering — computation substrates grouped roughly from
# "most feature-complete" to "spreadsheet/binary/exotic". Any substrate not
# listed here falls through to the end in alphabetical order.
SUBSTRATE_ORDER=(
    english
    python
    golang
    owl
    uml
    xlsx
    binary
    cobol
    csv
    yaml
    explain-dag
    # Effortless-licensed substrates render LAST.
    effortless-postgres
    effortless-xlsx
    effortless-entity-framework
)

get_valid_substrates() {
    # Returns valid substrates in SUBSTRATE_ORDER, with unlisted ones appended
    # alphabetically. "Valid" = has an inject script OR a take-test script.
    local -a discovered=()
    for dir in "$SUBSTRATES_DIR"/*/; do
        [ -d "$dir" ] || continue
        local name
        name=$(basename "$dir")
        [[ "$name" == .* ]] && continue
        if [ -f "$dir/inject-substrate.sh" ] || \
           [ -f "$dir/inject-into-${name}.py" ] || \
           [ -f "$dir/take-test.py" ] || \
           [ -f "$dir/take-test.sh" ]; then
            discovered+=("$name")
        fi
    done

    local -a ordered=()
    # 1. Append names from SUBSTRATE_ORDER that are actually present on disk
    local wanted present
    for wanted in "${SUBSTRATE_ORDER[@]}"; do
        for present in "${discovered[@]}"; do
            if [ "$wanted" = "$present" ]; then
                ordered+=("$wanted")
                break
            fi
        done
    done
    # 2. Append any discovered substrates not in SUBSTRATE_ORDER (alphabetical)
    local name known=0
    for name in $(printf '%s\n' "${discovered[@]}" | sort); do
        known=0
        for wanted in "${SUBSTRATE_ORDER[@]}"; do
            if [ "$wanted" = "$name" ]; then known=1; break; fi
        done
        [ $known -eq 0 ] && ordered+=("$name")
    done

    echo "${ordered[@]}"
}

# =============================================================================
# MENU DISPLAY
# =============================================================================
show_menu() {
    PROJECT_NAME=$(get_project_name)
    ACTIVE_DOMAIN=$(get_active_domain)

    echo ""
    echo -e "${BOLD}${CYAN}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BOLD}${CYAN}║${NC}          ${BOLD}${WHITE}EXECUTION SUBSTRATE ORCHESTRATOR${NC}                  ${BOLD}${CYAN}║${NC}"
    echo -e "${BOLD}${CYAN}╚════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "  Project:  ${WHITE}$PROJECT_NAME${NC}"
    echo -e "  Domain:   ${CYAN}$ACTIVE_DOMAIN${NC}"
    echo ""

    # Get list of valid substrates for the menu
    SUBSTRATES=$(get_valid_substrates)
    SUBSTRATES_ARRAY=($SUBSTRATES)
    TOTAL_SUBSTRATES=${#SUBSTRATES_ARRAY[@]}

    echo -e "${BOLD}${WHITE}Select an option:${NC}"
    echo ""
    echo -e "  ${GREEN}[A]${NC} Run ${BOLD}ALL${NC} substrates ($TOTAL_SUBSTRATES total) ${DIM}(default)${NC}"
    echo -e "  ${MAGENTA}[V]${NC} ${BOLD}VIEW RESULTS${NC} - Generate and open HTML report"
    echo -e "  ${YELLOW}[O]${NC} ${BOLD}SELECT ONTOLOGY${NC} - Switch active rulebook-examples domain"
    echo -e "  ${BLUE}[I]${NC} ${BOLD}IMPORT FROM AIRTABLE${NC} - Pull a base by ID into rulebook-examples"
    echo ""

    echo -e "  ${DIM}────────────────────────────────────────${NC}"
    echo -e "  ${YELLOW}Or select a specific substrate:${NC}"
    echo ""

    # Display substrates with numbers (already filtered by get_valid_substrates)
    INDEX=1
    for substrate in $SUBSTRATES; do
        if [ $INDEX -lt 10 ]; then
            echo -e "  ${CYAN}[0$INDEX]${NC} $substrate"
        else
            echo -e "  ${CYAN}[$INDEX]${NC} $substrate"
        fi
        INDEX=$((INDEX + 1))
    done

    # Dev-Ops / utilities
    echo -e "  ${DIM}────────────────────────────────────────${NC}"
    echo -e "  ${RED}[C]${NC} ${BOLD}CLEAN${NC} all generated files"
    echo -e "  ${YELLOW}[D]${NC} ${BOLD}DEV-OPS${NC} menu (PostgreSQL init, Effortless setup)"

    echo ""

    echo -e "  [${RED}Q${NC}] Quit"
    echo ""

}

# =============================================================================
# ACTION FUNCTIONS
# =============================================================================
action_select_domain() {
    CURRENT_DOMAIN=$(get_active_domain)

    echo ""
    echo -e "${BOLD}${CYAN}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BOLD}${CYAN}║${NC}              ${BOLD}${WHITE}SELECT ONTOLOGY${NC}                               ${BOLD}${CYAN}║${NC}"
    echo -e "${BOLD}${CYAN}╚════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "  Active: ${GREEN}${CURRENT_DOMAIN}${NC}"
    echo ""

    DOMAINS_ARRAY=()
    INDEX=1
    while IFS= read -r domain; do
        DOMAINS_ARRAY+=("$domain")
        if [ "$domain" = "$CURRENT_DOMAIN" ]; then
            echo -e "  ${GREEN}[$INDEX]${NC} ${GREEN}${domain}${NC} ${DIM}(active)${NC}"
        else
            echo -e "  ${CYAN}[$INDEX]${NC} ${domain}"
        fi
        INDEX=$((INDEX + 1))
    done < <(list_domains)

    DOMAINS_COUNT=${#DOMAINS_ARRAY[@]}

    echo ""
    echo -e "  ${RED}[Q]${NC} Cancel"
    echo ""

    read -p "  Select domain [1-$DOMAINS_COUNT, Q]: " DOMAIN_CHOICE

    case $DOMAIN_CHOICE in
        [Qq]|"")
            echo -e "  ${DIM}Cancelled - no changes made${NC}"
            echo ""
            return
            ;;
        [0-9]|[0-9][0-9])
            if [ "$DOMAIN_CHOICE" -ge 1 ] && [ "$DOMAIN_CHOICE" -le "$DOMAINS_COUNT" ]; then
                NEW_DOMAIN="${DOMAINS_ARRAY[$((DOMAIN_CHOICE - 1))]}"
                if [ "$NEW_DOMAIN" = "$CURRENT_DOMAIN" ]; then
                    echo -e "  ${DIM}Already using this domain${NC}"
                    echo ""
                    read -p "Press Enter to continue..."
                    return
                fi
                RULEBOOK="$(get_domain_rulebook_path "$NEW_DOMAIN")"
                if [ ! -f "$RULEBOOK" ]; then
                    echo -e "${RED}No rulebook found at $RULEBOOK${NC}"
                    read -p "Press Enter to continue..."
                    return
                fi
                set_active_domain "$NEW_DOMAIN"
                echo ""
                echo -e "${BOLD}${GREEN}Switched to: ${WHITE}${NEW_DOMAIN}${NC}"
                echo -e "${DIM}Run [A] to rebuild substrates against the new rulebook.${NC}"
            else
                echo -e "${RED}Invalid selection: $DOMAIN_CHOICE${NC}"
            fi
            ;;
        *)
            echo -e "${RED}Invalid option: $DOMAIN_CHOICE${NC}"
            ;;
    esac
    echo ""
    read -p "Press Enter to continue..."
}

action_import_from_airtable() {
    if [ "$SSOTME_AVAILABLE" != true ]; then
        echo ""
        echo -e "${RED}Effortless CLI is not installed.${NC}"
        echo -e "Importing from Airtable requires the Effortless CLI."
        echo -e "Visit ${CYAN}https://www.effortlessapi.com${NC} for installation instructions."
        echo ""
        read -p "Press Enter to continue..."
        return
    fi

    echo ""
    echo -e "${BOLD}${CYAN}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BOLD}${CYAN}║${NC}              ${BOLD}${WHITE}IMPORT FROM AIRTABLE${NC}                          ${BOLD}${CYAN}║${NC}"
    echo -e "${BOLD}${CYAN}╚════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "  This pulls a base from Airtable and creates a new local rulebook-example."
    echo -e "  After import the base becomes a standalone local ontology."
    echo ""

    read -p "  Enter Airtable base ID (e.g., appXXXXX) or [Q] to cancel: " BASE_ID

    case $BASE_ID in
        [Qq]|"")
            echo -e "  ${DIM}Cancelled${NC}"
            echo ""
            return
            ;;
    esac

    if [[ ! "$BASE_ID" =~ ^app[A-Za-z0-9]+ ]]; then
        echo -e "${RED}Invalid Base ID format. Airtable Base IDs start with 'app'.${NC}"
        read -p "Press Enter to continue..."
        return
    fi

    # Fetch the base name from Airtable via base-manager
    echo ""
    echo -e "${YELLOW}Fetching base name from Airtable...${NC}"
    BASE_NAME=$(python3 "$SCRIPT_DIR/base-manager.py" get-name "$BASE_ID" 2>/dev/null || echo "")

    if [ -z "$BASE_NAME" ]; then
        echo -e "${RED}Could not fetch base name from Airtable. Check your API key and base ID.${NC}"
        read -p "Press Enter to continue..."
        return
    fi

    # Derive a safe folder name from the base name
    DOMAIN_NAME=$(echo "$BASE_NAME" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-z0-9]/-/g' | sed 's/--*/-/g' | sed 's/^-//;s/-$//')

    DOMAIN_DIR="$RULEBOOK_EXAMPLES_DIR/$DOMAIN_NAME"
    RULEBOOK_DIR_NEW="$DOMAIN_DIR/effortless-rulebook"

    if [ -d "$DOMAIN_DIR" ]; then
        echo -e "${YELLOW}Domain folder already exists: ${WHITE}$DOMAIN_NAME${NC}"
        read -p "  Overwrite rulebook? [Y/n]: " OVERWRITE
        if [[ "$OVERWRITE" =~ ^[Nn]$ ]]; then
            echo -e "  ${DIM}Cancelled${NC}"
            read -p "Press Enter to continue..."
            return
        fi
    fi

    mkdir -p "$RULEBOOK_DIR_NEW"

    # Pull the rulebook from Airtable into the new domain folder
    echo ""
    echo -e "${YELLOW}Pulling rulebook from Airtable into ${WHITE}rulebook-examples/$DOMAIN_NAME/${NC}..."
    cd "$RULEBOOK_DIR_NEW"
    if ! effortless airtabletorulebook -o effortless-rulebook.json -account airtable -p "view=Grid view"; then
        echo -e "${RED}Failed to pull rulebook from Airtable.${NC}"
        read -p "Press Enter to continue..."
        return
    fi

    # Write effortless.json for this domain (with baseId + airtabletorulebook enabled)
    python3 -c "
import json
config = {
    'Name': '$BASE_NAME',
    'Description': 'Imported from Airtable base $BASE_ID',
    'Version': '1.0',
    'ProjectSettings': [
        {'Name': 'baseId', 'Value': '$BASE_ID', 'Description': 'Airtable base ID (used for re-import only)'}
    ],
    'Transpilers': [
        {
            'Name': 'airtabletorulebook',
            'RelativePath': '/effortless-rulebook',
            'CommandLine': 'airtable-to-rulebook -o effortless-rulebook.json -account airtable -p \"view=Grid view\"',
            'Enabled': True,
            'Description': 'Re-import from Airtable (optional; edit effortless-rulebook.json directly otherwise)'
        },
        {
            'Name': 'rulebooktoairtable',
            'RelativePath': '/effortless-rulebook/push-to-airtable',
            'CommandLine': 'rulebook-to-airtable -i ../effortless-rulebook.json -account airtable -w 300000',
            'Enabled': False,
            'Description': 'Reverse-sync: push rulebook changes back to Airtable (optional)'
        }
    ]
}
with open('$DOMAIN_DIR/effortless.json', 'w') as f:
    json.dump(config, f, indent=2)
print('Written effortless.json')
"

    # Switch active domain to the new import
    set_active_domain "$DOMAIN_NAME"

    echo ""
    echo -e "${BOLD}${GREEN}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BOLD}${GREEN}║${NC}              ${BOLD}${WHITE}IMPORT COMPLETE${NC}                               ${BOLD}${GREEN}║${NC}"
    echo -e "${BOLD}${GREEN}╚════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "  Domain:   ${WHITE}$DOMAIN_NAME${NC}"
    echo -e "  Location: ${WHITE}rulebook-examples/$DOMAIN_NAME/${NC}"
    echo ""
    echo -e "  ${DIM}This is now a standalone local rulebook. Re-import is optional.${NC}"
    echo -e "  ${DIM}Run [A] to build and test substrates against the new rulebook.${NC}"
    echo ""
    read -p "Press Enter to continue..."
}

action_view_results() {
    echo ""
    echo -e "${BOLD}${MAGENTA}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BOLD}${MAGENTA}║${NC}              ${BOLD}${WHITE}GENERATING HTML REPORT${NC}                       ${BOLD}${MAGENTA}║${NC}"
    echo -e "${BOLD}${MAGENTA}╚════════════════════════════════════════════════════════════╝${NC}"
    echo ""

    # Note: We don't regenerate individual substrate reports here because:
    # 1. They were already generated during the test run
    # 2. Regenerating would overwrite meaningful logs with stale data (e.g., for skipped tests)
    # The orchestration report reads from the existing substrate-report.html files

    # Generate main orchestration report
    python3 "$SCRIPT_DIR/generate-report.py"
    echo ""
    echo -e "${BOLD}${GREEN}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BOLD}${GREEN}║${NC}              ${BOLD}${WHITE}REPORT GENERATED${NC}                              ${BOLD}${GREEN}║${NC}"
    echo -e "${BOLD}${GREEN}╚════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    read -p "Press Enter to continue..."
}

action_clean() {
    SUBSTRATES=$(ls -d "$SUBSTRATES_DIR"/*/ 2>/dev/null | xargs -n1 basename)

    echo ""
    echo -e "${BOLD}${RED}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BOLD}${RED}║${NC}              ${BOLD}${WHITE}CLEANING ALL SUBSTRATES${NC}                       ${BOLD}${RED}║${NC}"
    echo -e "${BOLD}${RED}╚════════════════════════════════════════════════════════════╝${NC}"
    echo ""

    for substrate in $SUBSTRATES; do
        substrate_dir="$SUBSTRATES_DIR/$substrate"
        echo -e "${YELLOW}Cleaning ${substrate}...${NC}"

        # Try different clean methods in order of preference
        if [ -f "$substrate_dir/inject-into-${substrate}.py" ]; then
            # Most substrates have inject-into-*.py with --clean
            (cd "$substrate_dir" && python3 "inject-into-${substrate}.py" --clean 2>/dev/null) || true
        elif [ -f "$substrate_dir/clean.py" ]; then
            # YAML has a separate clean.py
            (cd "$substrate_dir" && python3 clean.py --clean 2>/dev/null) || true
        else
            echo -e "  ${DIM}No clean script found${NC}"
        fi
    done

    echo ""
    echo -e "${BOLD}${GREEN}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BOLD}${GREEN}║${NC}              ${BOLD}${WHITE}CLEAN COMPLETE${NC}                                ${BOLD}${GREEN}║${NC}"
    echo -e "${BOLD}${GREEN}╚════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    read -p "Press Enter to continue..."
}

# =============================================================================
# DEV-OPS ACTIONS
# =============================================================================
action_devops_menu() {
    while true; do
        echo ""
        echo -e "${BOLD}${CYAN}╔════════════════════════════════════════════════════════════╗${NC}"
        echo -e "${BOLD}${CYAN}║${NC}                    ${BOLD}${WHITE}DEV-OPS MENU${NC}                           ${BOLD}${CYAN}║${NC}"
        echo -e "${BOLD}${CYAN}╚════════════════════════════════════════════════════════════╝${NC}"
        echo ""

        # PostgreSQL
        if $POSTGRES_AVAILABLE; then
            echo -e "  [${CYAN}I${NC}] Initialize PostgreSQL Database"
        else
            echo -e "  ${DIM}[I] Initialize PostgreSQL (not installed)${NC}"
        fi

        # Effortless CLI setup
        if [ "$SSOTME_AVAILABLE" = true ]; then
            echo -e "  ${DIM}[S] Effortless Setup (already installed)${NC}"
        else
            echo -e "  [${CYAN}S${NC}] Effortless Setup Instructions"
        fi

        echo ""
        echo -e "  ${DIM}────────────────────────────────────────${NC}"
        echo -e "  ${BOLD}Tool Status:${NC}"
        if [ "$SSOTME_AVAILABLE" = true ]; then
            echo -e "    Effortless: ${GREEN}Available${NC}"
        else
            echo -e "    Effortless: ${YELLOW}Not installed${NC} ${DIM}(Airtable sync disabled)${NC}"
        fi

        if $POSTGRES_AVAILABLE; then
            echo -e "    PostgreSQL: ${GREEN}Available${NC}"
        else
            echo -e "    PostgreSQL: ${YELLOW}Not installed${NC} ${DIM}(DB init disabled)${NC}"
        fi
        echo ""

        echo -e "  [${RED}Q${NC}] Back to main menu"
        echo ""

        read -p "Enter choice [I/S/Q]: " devops_choice

        case $devops_choice in
            [Ii])
                if $POSTGRES_AVAILABLE; then
                    action_init_postgres
                else
                    echo ""
                    echo -e "${RED}PostgreSQL is not installed.${NC}"
                    read -p "Press Enter to continue..."
                fi
                ;;
            [Ss])
                action_setup_effortless
                ;;
            [Qq]|"")
                return
                ;;
            *)
                echo ""
                echo -e "${RED}Invalid option: $devops_choice${NC}"
                sleep 1
                ;;
        esac
    done
}

action_setup_effortless() {
    echo ""
    echo -e "${BOLD}${CYAN}Effortless CLI Installation Instructions${NC}"
    echo ""
    echo -e "The Effortless CLI is required for:"
    echo -e "  ${DIM}-${NC} Pulling data from Airtable"
    echo -e "  ${DIM}-${NC} Regenerating code from rulebook changes"
    echo ""
    echo -e "${YELLOW}To install Effortless:${NC}"
    echo ""
    echo -e "  1. Visit: ${CYAN}https://www.effortlessapi.com${NC}"
    echo -e "  2. Follow the installation instructions for your platform"
    echo -e "  3. Run ${WHITE}effortless --version${NC} to verify installation"
    echo ""
    echo -e "${DIM}Note: You can still run substrate tests without Effortless using existing files.${NC}"
    echo ""
    read -p "Press Enter to continue..."
}

action_init_postgres() {
    if ! $POSTGRES_AVAILABLE; then
        echo ""
        echo -e "${RED}PostgreSQL (psql) is not installed or not in PATH.${NC}"
        echo ""
        echo -e "${YELLOW}To install PostgreSQL:${NC}"
        echo -e "  macOS:  ${WHITE}brew install postgresql${NC}"
        echo -e "  Ubuntu: ${WHITE}sudo apt install postgresql${NC}"
        echo ""
        read -p "Press Enter to continue..."
        return
    fi

    echo ""
    echo -e "${BOLD}${CYAN}Initialize PostgreSQL Database${NC}"
    echo ""

    local init_script="$PROJECT_ROOT/licensed-effortless-tools/postgres/init-db.sh"

    if [ ! -f "$init_script" ]; then
        echo -e "${RED}Error: init-db.sh not found at $init_script${NC}"
        read -p "Press Enter to continue..."
        return
    fi

    echo -e "${YELLOW}This will:${NC}"
    echo -e "  1. Drop existing tables (if any)"
    echo -e "  2. Create tables, functions, and views"
    echo -e "  3. Insert seed data"
    echo ""

    read -p "Proceed? [Y/n]: " confirm
    if [[ "$confirm" =~ ^[Nn]$ ]]; then
        echo "Cancelled."
        return
    fi

    echo ""
    bash "$init_script"

    echo ""
    read -p "Press Enter to continue..."
}

# =============================================================================
# RUN SUBSTRATES (extracted as function for reuse)
# =============================================================================
run_substrates() {
    local RUN_SINGLE="$1"

    # Get list of valid substrates (those with inject or test scripts)
    SUBSTRATES=$(get_valid_substrates)
    SUBSTRATES_ARRAY=($SUBSTRATES)
    TOTAL_SUBSTRATES=${#SUBSTRATES_ARRAY[@]}

    # -----------------------------------------------------------------------------
    # Step 1: Generate answer key and blank test from database
    # -----------------------------------------------------------------------------
echo -e "${BOLD}${BLUE}┌──────────────────────────────────────────────────────────────┐${NC}"
echo -e "${BOLD}${BLUE}│${NC} ${BOLD}${WHITE}STEP 1:${NC} ${YELLOW}Generating answer key and blank test...${NC}              ${BOLD}${BLUE}│${NC}"
echo -e "${BOLD}${BLUE}└──────────────────────────────────────────────────────────────┘${NC}"
python3 -c "
import sys
sys.path.insert(0, '$SCRIPT_DIR')
from importlib.util import spec_from_loader, module_from_spec
from importlib.machinery import SourceFileLoader

# Load test-orchestrator module
spec = spec_from_loader('test_orchestrator', SourceFileLoader('test_orchestrator', '$SCRIPT_DIR/test-orchestrator.py'))
test_orch = module_from_spec(spec)
spec.loader.exec_module(test_orch)

# Load rulebook (no database connection needed - answer keys come from rulebook seed data)
rulebook = test_orch.load_rulebook()

# Run steps 1 and 2 (new generic functions)
all_answer_keys = test_orch.generate_all_answer_keys(rulebook)
test_orch.generate_all_blank_tests(all_answer_keys, rulebook)
"
echo ""

# -----------------------------------------------------------------------------
# Step 2: Run inject-substrate.sh for each substrate
# -----------------------------------------------------------------------------
echo -e "${BOLD}${BLUE}┌──────────────────────────────────────────────────────────────┐${NC}"
echo -e "${BOLD}${BLUE}│${NC} ${BOLD}${WHITE}STEP 2:${NC} ${YELLOW}Running inject + test for each substrate...${NC}         ${BOLD}${BLUE}│${NC}"
echo -e "${BOLD}${BLUE}└──────────────────────────────────────────────────────────────┘${NC}"
echo ""

# Determine which substrates to process. Airtable is included; STEP 0 already
# ran its inject, so it falls through to the test-only branch (take-test.sh),
# which copies the fresh answer-keys into its test-answers/ for grading.
if [ -n "$RUN_SINGLE" ]; then
    SUBSTRATES_TO_RUN="$RUN_SINGLE"
    TOTAL_TO_RUN=1
else
    SUBSTRATES_TO_RUN="$SUBSTRATES"
    TOTAL_TO_RUN=$TOTAL_SUBSTRATES
fi

# -----------------------------------------------------------------------------
# CONSOLIDATED ENGLISH PROMPT: Ask ONCE before running ALL substrates
# When running a single substrate, don't ask - user explicitly chose it
# -----------------------------------------------------------------------------
export ENGLISH_SKIP_LLM="false"

# Show English warning when: running ALL substrates OR explicitly running english
# Skip warning in CI mode or non-interactive shells
if ! $CI_MODE && [[ -t 0 ]]; then
    ENGLISH_DIR="$SUBSTRATES_DIR/english"
    # Show if: (running all AND english exists) OR (running only english)
    if [ -d "$ENGLISH_DIR" ] && { [ -z "$RUN_SINGLE" ] || [ "$RUN_SINGLE" = "english" ]; }; then
        # Calculate time estimate based on rulebook size
        ESTIMATE=$(python3 -c "
import sys
sys.path.insert(0, '$SCRIPT_DIR')
from shared import load_rulebook, estimate_llm_time
import os
os.chdir('$PROJECT_ROOT/execution-substrates/english')
rb = load_rulebook()
print(estimate_llm_time(rb))
" 2>/dev/null || echo "?:??")

        echo ""
        echo -e "${BOLD}${MAGENTA}┌──────────────────────────────────────────────────────────────┐${NC}"
        echo -e "${BOLD}${MAGENTA}│${NC} ${BOLD}English Substrate Warning${NC}                                    ${BOLD}${MAGENTA}│${NC}"
        echo -e "${BOLD}${MAGENTA}└──────────────────────────────────────────────────────────────┘${NC}"
        echo -e "  The English substrate uses LLM calls."
        echo -e "  Estimated time: ${YELLOW}${ESTIMATE}${NC} (based on rulebook size)"
        echo ""
        read -p "  Run English substrate? [Y/n] " english_response
        if [[ "$english_response" =~ ^[Nn]$ ]]; then
            export ENGLISH_SKIP_LLM="true"
            echo -e "  ${DIM}English will use cached results${NC}"
        else
            echo -e "  ${GREEN}English will run (may take a while)${NC}"
        fi
        echo ""
    fi
fi

INJECT_RESULTS=""
COLOR_INDEX=0
CURRENT=0

# Array to store failed substrates (outputs stored in temp files)
FAILED_SUBSTRATES=""
FAILED_OUTPUTS_DIR=$(mktemp -d)
trap "rm -rf $FAILED_OUTPUTS_DIR" EXIT

for substrate in $SUBSTRATES_TO_RUN; do
    substrate_dir="$SUBSTRATES_DIR/$substrate"
    inject_script="$substrate_dir/inject-substrate.sh"
    CURRENT=$((CURRENT + 1))

    # Get color for this substrate
    COLOR="${SUBSTRATE_COLORS[$COLOR_INDEX]}"
    COLOR_INDEX=$(( (COLOR_INDEX + 1) % ${#SUBSTRATE_COLORS[@]} ))

    if [ -f "$inject_script" ]; then
        substrate_upper=$(echo "$substrate" | tr '[:lower:]' '[:upper:]')
        echo -e "${COLOR}╔══════════════════════════════════════════════════════════════╗${NC}"
        echo -e "${COLOR}║${NC} ${BOLD}[$CURRENT/$TOTAL_TO_RUN]${NC} ${COLOR}▶ ${BOLD}${substrate_upper}${NC}"
        echo -e "${COLOR}╚══════════════════════════════════════════════════════════════╝${NC}"

        # Backup/restore mechanism to preserve successful results on failure
        test_answers_dir="$substrate_dir/test-answers"
        test_answers_backup="$substrate_dir/test-answers.bak"

        # Backup existing test-answers before clearing (if they exist and have files)
        if [ -d "$test_answers_dir" ] && [ "$(ls -A "$test_answers_dir" 2>/dev/null)" ]; then
            echo -e "  ${DIM}Backing up previous test-answers...${NC}"
            rm -rf "$test_answers_backup"
            cp -r "$test_answers_dir" "$test_answers_backup"
        fi

        # Clear test-answers for fresh run
        if [ -d "$test_answers_dir" ]; then
            rm -rf "$test_answers_dir"
        fi
        mkdir -p "$test_answers_dir"

        # Run script with real-time output AND capture for error reporting
        # Use tee to show output live while also saving to temp file
        # CRITICAL: Use || true to prevent set -e from exiting, then capture PIPESTATUS
        INJECT_TEMP_FILE=$(mktemp)
        START_TIME=$(python3 -c "import time; print(time.time())")
        # Run the script; with pipefail set, pipeline returns first non-zero exit code
        # The '|| true' prevents set -e from exiting, while PIPESTATUS still captures the real exit code
        bash "$inject_script" 2>&1 | tee "$INJECT_TEMP_FILE" || true
        INJECT_EXIT_CODE=${PIPESTATUS[0]}  # Capture IMMEDIATELY - must be first command after pipeline
        END_TIME=$(python3 -c "import time; print(time.time())")
        ELAPSED_TIME=$(python3 -c "print($END_TIME - $START_TIME)")
        INJECT_OUTPUT=$(cat "$INJECT_TEMP_FILE")
        rm -f "$INJECT_TEMP_FILE"

        # Check for SUBSTRATE_SKIPPED_BUT_GRADE signal (preserve timing from last run)
        PRESERVE_TIMING=false
        if echo "$INJECT_OUTPUT" | grep -q "SUBSTRATE_SKIPPED_BUT_GRADE"; then
            PRESERVE_TIMING=true
            echo -e "  ${YELLOW}○${NC} ${substrate}: ${YELLOW}Using previous answers (timing preserved)${NC}"
        fi

        if [ $INJECT_EXIT_CODE -eq 0 ]; then
            if ! $PRESERVE_TIMING; then
                INJECT_RESULTS="$INJECT_RESULTS$substrate:OK\n"
                echo -e "  ${GREEN}✓${NC} ${substrate}: ${GREEN}${BOLD}OK${NC}"
            fi
            # Success: delete backup (new results are good)
            rm -rf "$test_answers_backup"
        else
            INJECT_RESULTS="$INJECT_RESULTS$substrate:FAILED\n"
            echo -e "  ${RED}✗${NC} ${substrate}: ${RED}${BOLD}FAILED TO EXECUTE${NC}"
            # Store failure information
            FAILED_SUBSTRATES="$FAILED_SUBSTRATES $substrate"
            echo "$INJECT_OUTPUT" > "$FAILED_OUTPUTS_DIR/$substrate.txt"
            # Failure: restore backup to preserve previous successful results
            if [ -d "$test_answers_backup" ]; then
                echo -e "  ${YELLOW}↩${NC} Restoring previous test-answers from backup..."
                rm -rf "$test_answers_dir"
                mv "$test_answers_backup" "$test_answers_dir"
            fi

            # ═══════════════════════════════════════════════════════════════
            # FAIL LOUDLY: Pause and ask user if they want to continue
            # ═══════════════════════════════════════════════════════════════
            if ! $CI_MODE; then
                echo ""
                echo -e "${RED}╔════════════════════════════════════════════════════════════════╗${NC}"
                echo -e "${RED}║${NC}     ${BOLD}${RED}⚠️  SUBSTRATE FAILED: ${substrate_upper}${NC}                          ${RED}║${NC}"
                echo -e "${RED}╠════════════════════════════════════════════════════════════════╣${NC}"
                echo -e "${RED}║${NC} ${DIM}Last 10 lines of output:${NC}                                       ${RED}║${NC}"
                echo -e "${RED}╟────────────────────────────────────────────────────────────────╢${NC}"
                tail -10 "$FAILED_OUTPUTS_DIR/$substrate.txt" | while IFS= read -r line; do
                    # Truncate long lines and format
                    truncated="${line:0:60}"
                    printf "${RED}║${NC} %-60s ${RED}║${NC}\n" "$truncated"
                done
                echo -e "${RED}╠════════════════════════════════════════════════════════════════╣${NC}"
                echo -e "${RED}║${NC}  ${YELLOW}[C]${NC} Continue with remaining substrates                        ${RED}║${NC}"
                echo -e "${RED}║${NC}  ${RED}[S]${NC} Stop orchestration now                                    ${RED}║${NC}"
                echo -e "${RED}╚════════════════════════════════════════════════════════════════╝${NC}"
                echo ""
                read -p "  Choice [C/S]: " FAILURE_CHOICE
                case $FAILURE_CHOICE in
                    [Ss])
                        echo ""
                        echo -e "${RED}${BOLD}Orchestration stopped by user after failure.${NC}"
                        echo -e "Run ${WHITE}./orchestrate.sh${NC} to retry."
                        echo ""
                        # Still grade and save what we have before exiting
                        return 1
                        ;;
                    *)
                        echo ""
                        echo -e "${YELLOW}Continuing with remaining substrates...${NC}"
                        echo ""
                        ;;
                esac
            fi
        fi

        # Grade this substrate immediately
        python3 -c "
import sys
import json
import os
import glob
sys.path.insert(0, '$SCRIPT_DIR')
from importlib.util import spec_from_loader, module_from_spec
from importlib.machinery import SourceFileLoader

spec = spec_from_loader('test_orchestrator', SourceFileLoader('test_orchestrator', '$SCRIPT_DIR/test-orchestrator.py'))
test_orch = module_from_spec(spec)
spec.loader.exec_module(test_orch)

# Load all answer keys
all_answer_keys = {}
for entity_file in glob.glob(os.path.join(test_orch.ANSWER_KEYS_DIR, '*.json')):
    entity = os.path.basename(entity_file).replace('.json', '')
    with open(entity_file, 'r') as f:
        all_answer_keys[entity] = json.load(f)

# Load rulebook for grading
rulebook = test_orch.load_rulebook()

substrate = '$substrate'
inject_exit_code = $INJECT_EXIT_CODE
elapsed_seconds = $ELAPSED_TIME
preserve_timing = True if '$PRESERVE_TIMING' == 'true' else False

# Grade substrate (new generic function)
if inject_exit_code != 0:
    grades = test_orch.grade_substrate(substrate, all_answer_keys, rulebook)
    grades['error'] = 'FAILED TO EXECUTE (inject-substrate.sh returned non-zero)'
    grades['execution_failed'] = True
    error_msg = 'inject-substrate.sh returned non-zero exit code'
else:
    grades = test_orch.grade_substrate(substrate, all_answer_keys, rulebook)
    error_msg = None

# Add timing information (use previous timing if preserve_timing is True)
if preserve_timing:
    # Load previous timing from metadata
    metadata = test_orch.load_run_metadata(substrate)
    prev_run = metadata.get('last_successful_run') or metadata.get('last_run')
    if prev_run and 'duration_seconds' in prev_run:
        grades['elapsed_seconds'] = prev_run['duration_seconds']
        grades['timing_preserved'] = True
    else:
        grades['elapsed_seconds'] = elapsed_seconds
else:
    grades['elapsed_seconds'] = elapsed_seconds

# Update run metadata (tracks success/failure history)
success = inject_exit_code == 0
test_orch.update_run_metadata(substrate, grades, success, error_msg, preserve_timing=preserve_timing)

test_orch.generate_substrate_report(substrate, grades, rulebook)
test_orch.print_substrate_test_summary(substrate, grades, rulebook)

# Generate per-substrate HTML report using the substrate's custom script
# SKIP if preserve_timing=True (test was skipped) - keeps previous meaningful log intact
import subprocess
substrate_dir = os.path.join(test_orch.SUBSTRATES_DIR, substrate)
custom_script = os.path.join(substrate_dir, 'create-substrate-report.sh')
if os.path.exists(custom_script) and not preserve_timing:
    subprocess.run(['bash', 'create-substrate-report.sh'], cwd=substrate_dir, capture_output=True)

# Save grades to temp file for final summary
import pickle
grades_file = os.path.join(test_orch.SUBSTRATES_DIR, substrate, '.grades.pkl')
with open(grades_file, 'wb') as f:
    pickle.dump(grades, f)

# Also write score to a simple file for bash to check
score_file = os.path.join(test_orch.SUBSTRATES_DIR, substrate, '.score')
score = grades.get('score', -1)
with open(score_file, 'w') as f:
    f.write(str(score))
"
        # Check for 0% score and pause if so (similar to execution failure)
        score_file="$SUBSTRATES_DIR/$substrate/.score"
        if [ -f "$score_file" ]; then
            SCORE=$(cat "$score_file")
            rm -f "$score_file"  # Clean up

            # Check if score is 0 (using bc for float comparison)
            if echo "$SCORE == 0" | bc -l | grep -q 1; then
                # Only pause if execution itself succeeded (0% test score is the issue)
                if [ $INJECT_EXIT_CODE -eq 0 ] && ! $CI_MODE; then
                    echo ""
                    echo -e "${YELLOW}╔════════════════════════════════════════════════════════════════╗${NC}"
                    echo -e "${YELLOW}║${NC}     ${BOLD}${YELLOW}⚠️  TEST SCORE 0%: ${substrate_upper}${NC}                             ${YELLOW}║${NC}"
                    echo -e "${YELLOW}╠════════════════════════════════════════════════════════════════╣${NC}"
                    echo -e "${YELLOW}║${NC} ${DIM}Execution succeeded but all tests failed.${NC}                      ${YELLOW}║${NC}"
                    echo -e "${YELLOW}║${NC} ${DIM}This usually means test-answers are missing or stale.${NC}          ${YELLOW}║${NC}"
                    echo -e "${YELLOW}╠════════════════════════════════════════════════════════════════╣${NC}"
                    echo -e "${YELLOW}║${NC}  ${GREEN}[C]${NC} Continue with remaining substrates                        ${YELLOW}║${NC}"
                    echo -e "${YELLOW}║${NC}  ${RED}[S]${NC} Stop orchestration now                                    ${YELLOW}║${NC}"
                    echo -e "${YELLOW}╚════════════════════════════════════════════════════════════════╝${NC}"
                    echo ""
                    read -p "  Choice [C/S]: " SCORE_CHOICE
                    case $SCORE_CHOICE in
                        [Ss])
                            echo ""
                            echo -e "${YELLOW}${BOLD}Orchestration stopped by user after 0% score.${NC}"
                            echo -e "Run ${WHITE}./orchestrate.sh${NC} to retry."
                            echo ""
                            return 1
                            ;;
                        *)
                            echo ""
                            echo -e "${YELLOW}Continuing with remaining substrates...${NC}"
                            echo ""
                            ;;
                    esac
                fi
            fi
        fi

        # Add vertical spacing after each substrate for visual isolation
        printf '\n%.0s' {1..10}
    elif [ -f "$substrate_dir/take-test.sh" ] || [ -f "$substrate_dir/take-test.py" ]; then
        # Test-only substrate (no inject script, but has take-test)
        substrate_upper=$(echo "$substrate" | tr '[:lower:]' '[:upper:]')
        echo -e "${COLOR}╔══════════════════════════════════════════════════════════════╗${NC}"
        echo -e "${COLOR}║${NC} ${BOLD}[$CURRENT/$TOTAL_TO_RUN]${NC} ${COLOR}▶ ${BOLD}${substrate_upper}${NC} ${DIM}(test-only)${NC}"
        echo -e "${COLOR}╚══════════════════════════════════════════════════════════════╝${NC}"

        # Setup test-answers directory
        test_answers_dir="$substrate_dir/test-answers"
        test_answers_backup="$substrate_dir/test-answers.bak"
        if [ -d "$test_answers_dir" ] && [ "$(ls -A "$test_answers_dir" 2>/dev/null)" ]; then
            rm -rf "$test_answers_backup"
            cp -r "$test_answers_dir" "$test_answers_backup"
        fi
        rm -rf "$test_answers_dir"
        mkdir -p "$test_answers_dir"

        # Run take-test script
        INJECT_TEMP_FILE=$(mktemp)
        START_TIME=$(python3 -c "import time; print(time.time())")
        if [ -f "$substrate_dir/take-test.sh" ]; then
            bash "$substrate_dir/take-test.sh" 2>&1 | tee "$INJECT_TEMP_FILE" || true
        else
            (cd "$substrate_dir" && python3 take-test.py) 2>&1 | tee "$INJECT_TEMP_FILE" || true
        fi
        INJECT_EXIT_CODE=${PIPESTATUS[0]}
        END_TIME=$(python3 -c "import time; print(time.time())")
        ELAPSED_TIME=$(python3 -c "print($END_TIME - $START_TIME)")
        INJECT_OUTPUT=$(cat "$INJECT_TEMP_FILE")
        rm -f "$INJECT_TEMP_FILE"

        PRESERVE_TIMING=false
        if [ $INJECT_EXIT_CODE -eq 0 ]; then
            INJECT_RESULTS="$INJECT_RESULTS$substrate:OK\n"
            echo -e "  ${GREEN}✓${NC} ${substrate}: ${GREEN}${BOLD}OK${NC}"
            rm -rf "$test_answers_backup"
        else
            INJECT_RESULTS="$INJECT_RESULTS$substrate:FAILED\n"
            echo -e "  ${RED}✗${NC} ${substrate}: ${RED}${BOLD}FAILED${NC}"
            FAILED_SUBSTRATES="$FAILED_SUBSTRATES $substrate"
            echo "$INJECT_OUTPUT" > "$FAILED_OUTPUTS_DIR/$substrate.txt"
            if [ -d "$test_answers_backup" ]; then
                rm -rf "$test_answers_dir"
                mv "$test_answers_backup" "$test_answers_dir"
            fi
        fi

        # Grade this substrate
        python3 -c "
import sys
import json
import os
import glob
sys.path.insert(0, '$SCRIPT_DIR')
from importlib.util import spec_from_loader, module_from_spec
from importlib.machinery import SourceFileLoader

spec = spec_from_loader('test_orchestrator', SourceFileLoader('test_orchestrator', '$SCRIPT_DIR/test-orchestrator.py'))
test_orch = module_from_spec(spec)
spec.loader.exec_module(test_orch)

all_answer_keys = {}
for entity_file in glob.glob(os.path.join(test_orch.ANSWER_KEYS_DIR, '*.json')):
    entity = os.path.basename(entity_file).replace('.json', '')
    with open(entity_file, 'r') as f:
        all_answer_keys[entity] = json.load(f)

rulebook = test_orch.load_rulebook()
substrate = '$substrate'
inject_exit_code = $INJECT_EXIT_CODE
elapsed_seconds = $ELAPSED_TIME

if inject_exit_code != 0:
    grades = test_orch.grade_substrate(substrate, all_answer_keys, rulebook)
    grades['error'] = 'FAILED TO EXECUTE'
    grades['execution_failed'] = True
else:
    grades = test_orch.grade_substrate(substrate, all_answer_keys, rulebook)

grades['elapsed_seconds'] = elapsed_seconds
test_orch.update_run_metadata(substrate, grades, inject_exit_code == 0, None, preserve_timing=False)
test_orch.generate_substrate_report(substrate, grades, rulebook)
test_orch.print_substrate_test_summary(substrate, grades, rulebook)

import pickle
grades_file = os.path.join(test_orch.SUBSTRATES_DIR, substrate, '.grades.pkl')
with open(grades_file, 'wb') as f:
    pickle.dump(grades, f)

score_file = os.path.join(test_orch.SUBSTRATES_DIR, substrate, '.score')
with open(score_file, 'w') as f:
    f.write(str(grades.get('score', -1)))
"
        printf '\n%.0s' {1..10}
    else
        echo -e "  ${YELLOW}○${NC} ${substrate}: ${DIM}SKIPPED (no inject or test script)${NC}"
        INJECT_RESULTS="$INJECT_RESULTS$substrate:SKIPPED\n"
    fi
done

# -----------------------------------------------------------------------------
# Step 3: Generate summary report
# -----------------------------------------------------------------------------
# Breathing room before summary
printf '\n%.0s' {1..5}
echo -e "${BOLD}${BLUE}┌──────────────────────────────────────────────────────────────┐${NC}"
echo -e "${BOLD}${BLUE}│${NC} ${BOLD}${WHITE}STEP 3:${NC} ${YELLOW}Generating summary report...${NC}                         ${BOLD}${BLUE}│${NC}"
echo -e "${BOLD}${BLUE}└──────────────────────────────────────────────────────────────┘${NC}"
python3 -c "
import sys
import json
import os
import pickle
sys.path.insert(0, '$SCRIPT_DIR')
from importlib.util import spec_from_loader, module_from_spec
from importlib.machinery import SourceFileLoader

spec = spec_from_loader('test_orchestrator', SourceFileLoader('test_orchestrator', '$SCRIPT_DIR/test-orchestrator.py'))
test_orch = module_from_spec(spec)
spec.loader.exec_module(test_orch)

# Load rulebook for reporting
rulebook = test_orch.load_rulebook()

# Collect grades from temp files
run_single = '$RUN_SINGLE'
if run_single:
    substrates = [run_single]
else:
    substrates = test_orch.get_substrates()

all_grades = {}
for substrate in substrates:
    grades_file = os.path.join(test_orch.SUBSTRATES_DIR, substrate, '.grades.pkl')
    if os.path.exists(grades_file):
        with open(grades_file, 'rb') as f:
            all_grades[substrate] = pickle.load(f)
        os.remove(grades_file)  # Clean up

# Generate summary report and print final table
if run_single:
    # For single substrate, just print the summary table (no full report)
    test_orch.print_final_summary_table(all_grades, rulebook)
else:
    test_orch.generate_summary_report(all_grades, rulebook)
    test_orch.print_final_summary_table(all_grades, rulebook)
"
echo ""

# -----------------------------------------------------------------------------
# Step 4: Cleanup timing-only changes before generating report
# -----------------------------------------------------------------------------
# Revert files where ONLY duration_seconds changed (no real test result changes)
# This prevents noise in git history from timing variations
python3 -c "
import sys
sys.path.insert(0, '$SCRIPT_DIR')
from importlib.util import spec_from_loader, module_from_spec
from importlib.machinery import SourceFileLoader

spec = spec_from_loader('test_orchestrator', SourceFileLoader('test_orchestrator', '$SCRIPT_DIR/test-orchestrator.py'))
test_orch = module_from_spec(spec)
spec.loader.exec_module(test_orch)

test_orch.cleanup_unchanged_files()
"

# -----------------------------------------------------------------------------
# Step 5: Generate HTML Report
# -----------------------------------------------------------------------------
echo -e "${BOLD}${BLUE}┌──────────────────────────────────────────────────────────────┐${NC}"
echo -e "${BOLD}${BLUE}│${NC} ${BOLD}${WHITE}STEP 5:${NC} ${YELLOW}Generating HTML report...${NC}                            ${BOLD}${BLUE}│${NC}"
echo -e "${BOLD}${BLUE}└──────────────────────────────────────────────────────────────┘${NC}"
python3 "$SCRIPT_DIR/generate-report.py"
echo ""

# -----------------------------------------------------------------------------
# Step 6: Show Failed Substrates Summary (if any)
# -----------------------------------------------------------------------------
# Trim leading space from FAILED_SUBSTRATES
FAILED_SUBSTRATES=$(echo "$FAILED_SUBSTRATES" | xargs)
FAILED_COUNT=$(echo "$FAILED_SUBSTRATES" | wc -w | tr -d ' ')

if [ -n "$FAILED_SUBSTRATES" ]; then
    printf '\n%.0s' {1..3}
    echo -e "${BOLD}${RED}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BOLD}${RED}║${NC}           ${BOLD}${WHITE}⚠️  FAILED TO EXECUTE ($FAILED_COUNT substrates)${NC}              ${BOLD}${RED}║${NC}"
    echo -e "${BOLD}${RED}╚════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    
    for failed_substrate in $FAILED_SUBSTRATES; do
        failed_upper=$(echo "$failed_substrate" | tr '[:lower:]' '[:upper:]')
        echo -e "${RED}┌──────────────────────────────────────────────────────────────┐${NC}"
        echo -e "${RED}│${NC} ${BOLD}${RED}✗ ${failed_upper}${NC} ${DIM}(FAILED TO EXECUTE)${NC}"
        echo -e "${RED}├──────────────────────────────────────────────────────────────┤${NC}"
        
        # Show the captured output (last 20 lines to keep it manageable)
        echo -e "${DIM}Output (last 20 lines):${NC}"
        if [ -f "$FAILED_OUTPUTS_DIR/$failed_substrate.txt" ]; then
            tail -20 "$FAILED_OUTPUTS_DIR/$failed_substrate.txt" | while IFS= read -r line; do
                echo -e "  ${DIM}│${NC} $line"
            done
        fi
        
        echo -e "${RED}└──────────────────────────────────────────────────────────────┘${NC}"
        echo ""
    done
    
    # List all failed substrates on one line for easy copy/paste
    echo -e "${RED}${BOLD}Failed substrates:${NC} $FAILED_SUBSTRATES"
    echo ""
fi

# -----------------------------------------------------------------------------
# Summary
# -----------------------------------------------------------------------------
if [ -n "$FAILED_SUBSTRATES" ]; then
    echo -e "${BOLD}${YELLOW}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BOLD}${YELLOW}║${NC}         ${BOLD}${WHITE}ORCHESTRATION COMPLETE (WITH FAILURES)${NC}            ${BOLD}${YELLOW}║${NC}"
    echo -e "${BOLD}${YELLOW}╚════════════════════════════════════════════════════════════╝${NC}"
else
    echo -e "${BOLD}${GREEN}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BOLD}${GREEN}║${NC}              ${BOLD}${WHITE}ORCHESTRATION COMPLETE${NC}                       ${BOLD}${GREEN}║${NC}"
    echo -e "${BOLD}${GREEN}╚════════════════════════════════════════════════════════════╝${NC}"
fi
echo ""
echo -e "${CYAN}Results written to:${NC}"
if [ -n "$RUN_SINGLE" ]; then
    echo -e "  ${DIM}•${NC} Per-substrate: ${WHITE}execution-substrates/$RUN_SINGLE/test-results.md${NC}"
else
    echo -e "  ${DIM}•${NC} Per-substrate: ${WHITE}execution-substrates/*/test-results.md${NC}"
    echo -e "  ${DIM}•${NC} Summary:       ${WHITE}orchestration/all-tests-results.md${NC}"
fi
echo -e "  ${DIM}•${NC} HTML Report:   ${WHITE}orchestration/orchestration-report.html${NC}"
echo ""

# Open browser (skip in CI mode)
if ! $CI_MODE; then
    echo -e "${CYAN}Opening HTML report in browser...${NC}"
    open "$SCRIPT_DIR/orchestration-report.html"
    echo ""
fi

# Return failure status (don't exit, let caller handle)
if [ -n "$FAILED_SUBSTRATES" ]; then
    echo -e "${RED}${BOLD}⚠️  $FAILED_COUNT substrate(s) failed to execute: $FAILED_SUBSTRATES${NC}"
    return 1
fi
return 0
}

# =============================================================================
# MAIN LOOP
# =============================================================================

# DOCKER/CI MODE: Run all substrates non-interactively and exit
if $CI_MODE; then
    echo ""
    echo -e "${BOLD}${CYAN}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BOLD}${CYAN}║${NC}          ${BOLD}${WHITE}EXECUTION SUBSTRATE ORCHESTRATOR${NC}                  ${BOLD}${CYAN}║${NC}"
    echo -e "${BOLD}${CYAN}╚════════════════════════════════════════════════════════════╝${NC}"
    echo ""

    if $DOCKER_MODE; then
        echo -e "${BOLD}Running in Docker mode - using cached data...${NC}"
        echo ""

        # Step 0: Set up from cache (use repo cache if no user cache)
        echo -e "${BLUE}Setting up from cache...${NC}"
        python3 "$SCRIPT_DIR/cache-manager.py" setup-offline 2>/dev/null || {
            echo -e "${YELLOW}Cache manager not available, trying rulebook-cache...${NC}"
            python3 "$SCRIPT_DIR/rulebook-cache.py" sync --offline --non-interactive || true
        }
        echo ""
    else
        echo -e "${BOLD}Running in CI mode - executing all substrates...${NC}"
    fi

    run_substrates ""
    EXIT_CODE=$?

    # In Docker mode, print a summary of where to find results
    if $DOCKER_MODE; then
        echo ""
        echo -e "${BOLD}${GREEN}════════════════════════════════════════════════════════════════${NC}"
        echo -e "${BOLD}${GREEN}  Docker execution complete!${NC}"
        echo -e "${BOLD}${GREEN}════════════════════════════════════════════════════════════════${NC}"
        echo ""
        echo -e "  ${BOLD}Reports generated:${NC}"
        echo -e "    • orchestration/orchestration-report.html"
        echo -e "    • orchestration/all-tests-results.md"
        echo -e "    • execution-substrates/*/substrate-report.html"
        echo ""
        echo -e "  ${DIM}(Reports are in your mounted volume - accessible from host)${NC}"
        echo ""
    fi

    exit $EXIT_CODE
fi

# Interactive menu loop
while true; do
    show_menu

    # Get substrates for numbered selection (must match show_menu)
    SUBSTRATES=$(get_valid_substrates)
    SUBSTRATES_ARRAY=($SUBSTRATES)
    TOTAL_SUBSTRATES=${#SUBSTRATES_ARRAY[@]}

    read -p "Enter choice [A, V, O, I, C, D, 1-$TOTAL_SUBSTRATES, Q] (default: A): " USER_CHOICE

    # Default to 'A' if user just presses Enter
    if [ -z "$USER_CHOICE" ]; then
        USER_CHOICE="A"
    fi

    case $USER_CHOICE in
        [Aa])
            echo ""
            echo -e "${GREEN}Running ALL substrates...${NC}"
            run_substrates ""
            ;;
        [Vv])
            action_view_results
            ;;
        [Oo])
            action_select_domain
            ;;
        [Ii])
            action_import_from_airtable
            ;;
        [Cc])
            action_clean
            ;;
        [Dd])
            action_devops_menu
            ;;
        [Qq])
            echo ""
            exit 0
            ;;
        [0-9]|[0-9][0-9])
            if [ "$USER_CHOICE" -ge 1 ] && [ "$USER_CHOICE" -le "$TOTAL_SUBSTRATES" ]; then
                RUN_SINGLE="${SUBSTRATES_ARRAY[$((USER_CHOICE - 1))]}"
                echo ""
                echo -e "${GREEN}Running single substrate: ${BOLD}$RUN_SINGLE${NC}"
                run_substrates "$RUN_SINGLE"
            else
                echo ""
                echo -e "${RED}Invalid substrate number: $USER_CHOICE${NC}"
                sleep 1
            fi
            ;;
        *)
            echo ""
            echo -e "${RED}Invalid option: $USER_CHOICE${NC}"
            sleep 1
            ;;
    esac
done
