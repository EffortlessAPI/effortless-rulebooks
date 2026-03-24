#!/usr/bin/env python3
"""
Populate ACME Corporation Airtable with sample data.

Creates:
- 3 Employees: 1 Manager (Sarah Chen), 2 Non-Managers (Mike Johnson, Priya Patel)
- 3 Projects demonstrating different states:
  1. Internal project, approved (no manager approval needed)
  2. Client-facing project, pending approval (requires manager approval)
  3. Compliance project, approved by manager

Usage:
  python populate-acme-corp.py           # Dry run (preview changes)
  python populate-acme-corp.py --execute # Actually make changes
"""

import os
import sys
import time
import argparse
import requests
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

# Parse arguments
parser = argparse.ArgumentParser(description="Populate ACME Corporation with sample data")
parser.add_argument("--execute", action="store_true", help="Actually execute changes (default is dry-run)")
args = parser.parse_args()

DRY_RUN = not args.execute

# Configuration - ACME Corporation base
BASE_ID = "appzkcmBFPWFGBtRo"
API_KEY = os.getenv("AIRTABLE_API_KEY")

if not API_KEY:
    print("ERROR: AIRTABLE_API_KEY not found in environment")
    sys.exit(1)

HEADERS = {
    "Authorization": f"Bearer {API_KEY}",
    "Content-Type": "application/json"
}

BASE_URL = f"https://api.airtable.com/v0/{BASE_ID}"

# Rate limiting helper
def rate_limit():
    time.sleep(0.25)  # Airtable rate limit: 5 requests/sec

def get_all_records(table_name):
    """Fetch all records from a table"""
    records = []
    offset = None

    while True:
        url = f"{BASE_URL}/{table_name}"
        params = {}
        if offset:
            params["offset"] = offset

        response = requests.get(url, headers=HEADERS, params=params)
        rate_limit()

        if response.status_code != 200:
            print(f"  ERROR fetching {table_name}: {response.status_code} - {response.text}")
            return []

        data = response.json()
        records.extend(data.get("records", []))

        offset = data.get("offset")
        if not offset:
            break

    return records

def delete_records(table_name, record_ids):
    """Delete records in batches of 10 (Airtable limit)"""
    if DRY_RUN:
        print(f"  [DRY RUN] Would delete {len(record_ids)} records")
        return

    for i in range(0, len(record_ids), 10):
        batch = record_ids[i:i+10]
        url = f"{BASE_URL}/{table_name}"
        params = {"records[]": batch}

        response = requests.delete(url, headers=HEADERS, params=params)
        rate_limit()

        if response.status_code != 200:
            print(f"  ERROR deleting from {table_name}: {response.status_code} - {response.text}")
        else:
            print(f"  Deleted {len(batch)} records")

def create_records(table_name, records):
    """Create records in batches of 10 (Airtable limit). Returns created record IDs."""
    created_ids = []

    if DRY_RUN:
        print(f"  [DRY RUN] Would create {len(records)} records:")
        for r in records:
            name = r.get("Name") or r.get("DisplayName") or str(r)[:50]
            print(f"    - {name}")
        # Return fake IDs for dry run
        return [f"fake_id_{i}" for i in range(len(records))]

    for i in range(0, len(records), 10):
        batch = records[i:i+10]
        url = f"{BASE_URL}/{table_name}"
        payload = {"records": [{"fields": r} for r in batch]}

        response = requests.post(url, headers=HEADERS, json=payload)
        rate_limit()

        if response.status_code != 200:
            print(f"  ERROR creating in {table_name}: {response.status_code} - {response.text}")
        else:
            result = response.json()
            for rec in result.get("records", []):
                created_ids.append(rec["id"])
            print(f"  Created {len(batch)} records")

    return created_ids

def update_record(table_name, record_id, fields):
    """Update a single record"""
    if DRY_RUN:
        print(f"  [DRY RUN] Would update {record_id}")
        return True

    url = f"{BASE_URL}/{table_name}/{record_id}"
    payload = {"fields": fields}

    response = requests.patch(url, headers=HEADERS, json=payload)
    rate_limit()

    if response.status_code != 200:
        print(f"  ERROR updating {record_id}: {response.status_code} - {response.text}")
        return False
    return True

def find_record_id(table_name, field_name, value):
    """Find a record ID by field value"""
    records = get_all_records(table_name)
    for r in records:
        fields = r.get("fields", {})
        if fields.get(field_name) == value:
            return r["id"]
    return None

# ============================================================================
# Sample Data - demonstrating different use cases
# ============================================================================

# Roles already exist in schema - we'll just look them up
# - ProjectManager (IsManager: true)
# - Developer (IsManager: false)
# - Analyst (IsManager: false)
# - Director (IsManager: true)

# 3 Employees: 1 manager, 2 non-managers
EMPLOYEES = [
    {
        "Name": "Sarah Chen",
        "EmailAddress": "sarah.chen@acmecorp.example",
        "PhoneNumber": "555-0101"
        # Role: ProjectManager (linked later)
    },
    {
        "Name": "Mike Johnson",
        "EmailAddress": "mike.johnson@acmecorp.example",
        "PhoneNumber": "555-0102"
        # Role: Developer (linked later)
    },
    {
        "Name": "Priya Patel",
        "EmailAddress": "priya.patel@acmecorp.example",
        "PhoneNumber": "555-0103"
        # Role: Analyst (linked later)
    }
]

# 3 Projects demonstrating different states
PROJECTS = [
    {
        "Name": "Internal Tools Dashboard",
        "Description": "Build internal analytics dashboard for team productivity metrics",
        "DueDate": "2026-06-15",
        "IsApproved": True
        # ProjectType: Internal (no manager approval needed - auto-approved)
    },
    {
        "Name": "Customer Portal Redesign",
        "Description": "Complete redesign of customer-facing portal with modern UX",
        "DueDate": "2026-08-01",
        "IsApproved": False
        # ProjectType: ClientFacing (requires manager approval - PENDING)
    },
    {
        "Name": "GDPR Compliance Audit",
        "Description": "Annual compliance audit and documentation update",
        "DueDate": "2026-04-30",
        "IsApproved": True
        # ProjectType: Compliance (requires approval - APPROVED by manager)
    }
]

# Customers - keeping existing ones but adding context
CUSTOMERS = [
    {
        "Customer": "CUST0001",
        "FirstName": "Larry",
        "LastName": "Smith",
        "EmailAddress": "larry.smith@email.com"
    },
    {
        "Customer": "CUST0002",
        "FirstName": "John",
        "LastName": "Doe",
        "EmailAddress": "john.doe@email.com"
    },
    {
        "Customer": "CUST0003",
        "FirstName": "Emily",
        "LastName": "Jones",
        "EmailAddress": "emily.jones@email.com"
    }
]

def main():
    print("=" * 60)
    print("Populate ACME Corporation with Sample Data")
    print("=" * 60)
    print(f"Base ID: {BASE_ID}")
    if DRY_RUN:
        print()
        print("*** DRY RUN MODE - No changes will be made ***")
        print("Run with --execute to apply changes")
    print()

    # Phase 1: Clean existing data (except lookup tables)
    print("PHASE 1: Cleaning existing data (Employees, Projects, Customers)")
    print("-" * 40)

    for table_name in ["Employees", "Projects", "Customers"]:
        print(f"\n{table_name}:")
        records = get_all_records(table_name)
        if records:
            record_ids = [r["id"] for r in records]
            print(f"  Found {len(record_ids)} records to delete")
            delete_records(table_name, record_ids)
        else:
            print("  No records found")

    # Phase 2: Look up Roles and TypesOfProject IDs
    print()
    print("PHASE 2: Looking up reference data")
    print("-" * 40)

    print("\nRoles:")
    project_mgr_id = find_record_id("Roles", "Name", "ProjectManager")
    developer_id = find_record_id("Roles", "Name", "Developer")
    analyst_id = find_record_id("Roles", "Name", "Analyst")
    print(f"  ProjectManager: {project_mgr_id or 'NOT FOUND'}")
    print(f"  Developer: {developer_id or 'NOT FOUND'}")
    print(f"  Analyst: {analyst_id or 'NOT FOUND'}")

    print("\nTypesOfProject:")
    internal_id = find_record_id("TypesOfProject", "Name", "Internal")
    clientfacing_id = find_record_id("TypesOfProject", "Name", "ClientFacing")
    compliance_id = find_record_id("TypesOfProject", "Name", "Compliance")
    print(f"  Internal: {internal_id or 'NOT FOUND'}")
    print(f"  ClientFacing: {clientfacing_id or 'NOT FOUND'}")
    print(f"  Compliance: {compliance_id or 'NOT FOUND'}")

    # Phase 3: Create Customers
    print()
    print("PHASE 3: Creating Customers")
    print("-" * 40)
    customer_ids = create_records("Customers", CUSTOMERS)

    # Phase 4: Create Employees with Role links
    print()
    print("PHASE 4: Creating Employees")
    print("-" * 40)

    # Add Role links to employees
    employees_with_roles = []
    role_assignments = [project_mgr_id, developer_id, analyst_id]

    for i, emp in enumerate(EMPLOYEES):
        emp_copy = emp.copy()
        if role_assignments[i]:
            emp_copy["Role"] = [role_assignments[i]]
        employees_with_roles.append(emp_copy)

    employee_ids = create_records("Employees", employees_with_roles)

    # Map employee names to IDs for later use
    employee_map = {}
    if not DRY_RUN:
        emp_records = get_all_records("Employees")
        for rec in emp_records:
            name = rec.get("fields", {}).get("Name")
            if name:
                employee_map[name] = rec["id"]
    else:
        employee_map = {
            "Sarah Chen": "fake_sarah",
            "Mike Johnson": "fake_mike",
            "Priya Patel": "fake_priya"
        }

    sarah_id = employee_map.get("Sarah Chen")
    print(f"\n  Sarah Chen (Manager) ID: {sarah_id}")

    # Phase 5: Create Projects with relationships
    print()
    print("PHASE 5: Creating Projects")
    print("-" * 40)

    projects_with_links = []

    # Project 1: Internal Tools Dashboard - auto-approved, no ApprovedBy needed
    proj1 = PROJECTS[0].copy()
    if internal_id:
        proj1["ProjectType"] = [internal_id]
    projects_with_links.append(proj1)

    # Project 2: Customer Portal Redesign - pending approval
    proj2 = PROJECTS[1].copy()
    if clientfacing_id:
        proj2["ProjectType"] = [clientfacing_id]
    projects_with_links.append(proj2)

    # Project 3: GDPR Compliance - approved by Sarah Chen (manager)
    proj3 = PROJECTS[2].copy()
    if compliance_id:
        proj3["ProjectType"] = [compliance_id]
    if sarah_id:
        proj3["ApprovedBy"] = [sarah_id]
    projects_with_links.append(proj3)

    project_ids = create_records("Projects", projects_with_links)

    # Phase 6: Link employees to projects
    print()
    print("PHASE 6: Linking Employees to Projects")
    print("-" * 40)

    if not DRY_RUN:
        proj_records = get_all_records("Projects")
        proj_map = {}
        for rec in proj_records:
            name = rec.get("fields", {}).get("Name")
            if name:
                proj_map[name] = rec["id"]

        # Sarah manages all projects
        sarah_id_real = employee_map.get("Sarah Chen")
        if sarah_id_real:
            all_proj_ids = list(proj_map.values())
            update_record("Employees", sarah_id_real, {"Projects": all_proj_ids})
            print(f"  Linked Sarah Chen to all {len(all_proj_ids)} projects")

        # Mike works on Internal Tools Dashboard
        mike_id = employee_map.get("Mike Johnson")
        internal_proj_id = proj_map.get("Internal Tools Dashboard")
        if mike_id and internal_proj_id:
            update_record("Employees", mike_id, {"Projects": [internal_proj_id]})
            print("  Linked Mike Johnson to Internal Tools Dashboard")

        # Priya works on Customer Portal and GDPR
        priya_id = employee_map.get("Priya Patel")
        portal_id = proj_map.get("Customer Portal Redesign")
        gdpr_id = proj_map.get("GDPR Compliance Audit")
        if priya_id:
            priya_projs = [p for p in [portal_id, gdpr_id] if p]
            if priya_projs:
                update_record("Employees", priya_id, {"Projects": priya_projs})
                print(f"  Linked Priya Patel to {len(priya_projs)} projects")
    else:
        print("  [DRY RUN] Would link employees to projects")

    # Summary
    print()
    print("=" * 60)
    if DRY_RUN:
        print("DRY RUN COMPLETE - No changes were made")
        print()
        print("To apply these changes, run:")
        print("  python populate-acme-corp.py --execute")
    else:
        print("Population complete!")
    print()
    print("Summary of sample data:")
    print()
    print("EMPLOYEES (3 total):")
    print("  1. Sarah Chen - Project Manager (IsManager: true)")
    print("  2. Mike Johnson - Developer (IsManager: false)")
    print("  3. Priya Patel - Analyst (IsManager: false)")
    print()
    print("PROJECTS (3 total, demonstrating different states):")
    print("  1. Internal Tools Dashboard")
    print("     - Type: Internal (RequiresManagerApproval: false)")
    print("     - Status: Approved (auto-approved, no manager needed)")
    print("     - Due: 2026-06-15")
    print()
    print("  2. Customer Portal Redesign")
    print("     - Type: ClientFacing (RequiresManagerApproval: true)")
    print("     - Status: PENDING APPROVAL")
    print("     - Due: 2026-08-01")
    print()
    print("  3. GDPR Compliance Audit")
    print("     - Type: Compliance (RequiresManagerApproval: true)")
    print("     - Status: Approved by Sarah Chen (manager)")
    print("     - Due: 2026-04-30")
    print()
    print("CUSTOMERS (3 total):")
    print("  - Larry Smith, John Doe, Emily Jones")
    print("=" * 60)

if __name__ == "__main__":
    main()
