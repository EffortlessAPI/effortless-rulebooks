#!/bin/bash
#
# Power Laws & Fractals - Project Launcher
#
# Just run: ./start.sh
#

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
RED='\033[0;31m'
NC='\033[0m' # No Color
BOLD='\033[1m'
DIM='\033[2m'

show_menu() {
    clear
    echo ""
    echo -e "${BOLD}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BOLD}║${NC}     🔺 ${CYAN}POWER LAWS & FRACTALS${NC} - Veritasium Edition        ${BOLD}║${NC}"
    echo -e "${BOLD}╚════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "  ${BOLD}Run Tests:${NC}"
    echo -e "  ${GREEN}1)${NC}  🧪  Run ALL Platform Tests   ${YELLOW}(+ opens report)${NC}"
    echo -e "  ${GREEN}2)${NC}  🐍  Python Only"
    echo -e "  ${GREEN}3)${NC}  🐹  Go Only"
    echo -e "  ${GREEN}4)${NC}  🐘  PostgreSQL Only          ${DIM}(requires Docker)${NC}"
    echo ""
    echo -e "  ${BOLD}View:${NC}"
    echo -e "  ${GREEN}5)${NC}  📊  View Results Report      ${YELLOW}(opens in browser)${NC}"
    echo ""
    echo -e "  ${BOLD}Utilities:${NC}"
    echo -e "  ${MAGENTA}g)${NC}  🔄  Regenerate Test Data     ${DIM}(CANONICAL Python, 6dp)${NC}"
    echo -e "  ${MAGENTA}s)${NC}  📄  View SSoT JSON"
    echo -e "  ${MAGENTA}r)${NC}  📖  View README"
    echo -e "  ${MAGENTA}j)${NC}  📓  Jupyter Notebook"
    echo ""
    echo -e "  ${RED}q)${NC}  ❌  Quit"
    echo ""
    echo -e "${BOLD}────────────────────────────────────────────────────────────────${NC}"
    echo -n "  Pick an option: "
}

open_report() {
    # Generate the comprehensive HTML report and open it
    python3 "$SCRIPT_DIR/visualizer/generate_report.py"
    
    if [[ "$OSTYPE" == "darwin"* ]]; then
        open "$SCRIPT_DIR/visualizer/report.html"
    elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
        xdg-open "$SCRIPT_DIR/visualizer/report.html" 2>/dev/null || echo "Open visualizer/report.html in your browser"
    else
        echo "Open visualizer/report.html in your browser"
    fi
}

run_all_tests() {
    echo ""
    echo -e "${CYAN}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${CYAN}║  Running ALL Platform Tests                                ║${NC}"
    echo -e "${CYAN}╚════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "${DIM}Using existing test-input.json and answer-key.json${NC}"
    echo -e "${DIM}(Use 'g' to regenerate test data from SSoT)${NC}"
    echo ""
    python3 "$SCRIPT_DIR/orchestrator.py" --all
    echo ""
    echo -e "${CYAN}Opening results report in browser...${NC}"
    open_report
    echo ""
    echo -e "${YELLOW}Press Enter to continue...${NC}"
    read
}

run_python() {
    echo ""
    echo -e "${CYAN}Running Python tests...${NC}"
    echo ""
    python3 "$SCRIPT_DIR/python/run-tests.py"
    echo ""
    echo -e "${YELLOW}Press Enter to continue...${NC}"
    read
}

run_golang() {
    echo ""
    echo -e "${CYAN}Running Go tests...${NC}"
    echo ""
    cd "$SCRIPT_DIR/golang"
    if command -v go &> /dev/null; then
        go run .
    else
        echo -e "${YELLOW}Go not found. Install Go to run this test.${NC}"
        echo -e "See: ${BLUE}golang/README.md${NC}"
    fi
    cd "$SCRIPT_DIR"
    echo ""
    echo -e "${YELLOW}Press Enter to continue...${NC}"
    read
}

run_postgres() {
    echo ""
    echo -e "${CYAN}Running PostgreSQL tests...${NC}"
    echo ""
    if command -v psql &> /dev/null; then
        python3 "$SCRIPT_DIR/postgres/run-tests.py"
    else
        echo -e "${YELLOW}psql not found. Install PostgreSQL client tools.${NC}"
        echo -e "On macOS: ${BLUE}brew install libpq${NC}"
        echo -e "See: ${BLUE}postgres/README.md${NC}"
    fi
    echo ""
    echo -e "${YELLOW}Press Enter to continue...${NC}"
    read
}

view_report() {
    echo ""
    echo -e "${CYAN}Generating and opening results report...${NC}"
    echo ""
    # Generate and open HTML
    open_report
    echo ""
    echo -e "${GREEN}Report opened in browser!${NC}"
    echo ""
    echo -e "${YELLOW}Press Enter to continue...${NC}"
    read
}

regenerate_data() {
    echo ""
    echo -e "${CYAN}╔════════════════════════════════════════════════════════════╗${NC}"
    echo -e "${CYAN}║  Regenerating CANONICAL Test Data from SSoT                ║${NC}"
    echo -e "${CYAN}╚════════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "${YELLOW}This Python script generates:${NC}"
    echo -e "  • ${GREEN}test-input.json${NC}  - iterations 4-7 with raw facts only"
    echo -e "  • ${GREEN}answer-key.json${NC}  - ALL 8 iterations with computed values (6dp)"
    echo -e "  • ${GREEN}base-data.json${NC}   - iterations 0-3 for platform init"
    echo ""
    echo -e "${MAGENTA}All numeric values are rounded to 6 decimal places.${NC}"
    echo -e "${MAGENTA}These files are CANONICAL - all platforms must match them exactly.${NC}"
    echo ""
    python3 "$SCRIPT_DIR/generate-test-data.py"
    echo ""
    echo -e "${YELLOW}Press Enter to continue...${NC}"
    read
}

run_jupyter() {
    echo ""
    echo -e "${CYAN}Starting Jupyter Notebook...${NC}"
    echo ""
    cd "$SCRIPT_DIR/jupyter"
    if command -v jupyter &> /dev/null; then
        jupyter notebook power-laws-and-fractals.ipynb
    else
        echo -e "${YELLOW}Jupyter not found. Install with: pip install jupyter${NC}"
        echo -e "Then run: ${BLUE}jupyter notebook jupyter/power-laws-and-fractals.ipynb${NC}"
    fi
    cd "$SCRIPT_DIR"
    echo ""
    echo -e "${YELLOW}Press Enter to continue...${NC}"
    read
}

view_ssot() {
    echo ""
    echo -e "${CYAN}Source of Truth (first 50 lines):${NC}"
    echo ""
    head -50 "$SCRIPT_DIR/ssot/ERB_veritasium-power-laws-and-fractals.json"
    echo ""
    echo -e "${YELLOW}Full file: ssot/ERB_veritasium-power-laws-and-fractals.json${NC}"
    echo ""
    echo -e "${YELLOW}Press Enter to continue...${NC}"
    read
}

view_readme() {
    echo ""
    if command -v less &> /dev/null; then
        less "$SCRIPT_DIR/README.md"
    else
        cat "$SCRIPT_DIR/README.md"
    fi
    echo ""
    echo -e "${YELLOW}Press Enter to continue...${NC}"
    read
}

# Main loop
while true; do
    show_menu
    read -r choice
    
    case $choice in
        1) run_all_tests ;;
        2) run_python ;;
        3) run_golang ;;
        4) run_postgres ;;
        5) view_report ;;
        g|G) regenerate_data ;;
        s|S) view_ssot ;;
        r|R) view_readme ;;
        j|J) run_jupyter ;;
        q|Q) 
            echo ""
            echo -e "${GREEN}Goodbye! 🔺${NC}"
            echo ""
            exit 0 
            ;;
        *) 
            echo -e "${YELLOW}Invalid option. Try again.${NC}"
            sleep 1
            ;;
    esac
done
