# SOMA Platform - GitHub Issues Creation

This directory contains a script to automatically create GitHub issues for all the epics and stories defined in `EPICS_AND_STORIES.md`.

## Prerequisites

1. **GitHub CLI installed**: Make sure you have the GitHub CLI (`gh`) installed on your system
2. **Authentication**: You must be authenticated with GitHub CLI. Run `gh auth login` if you haven't already
3. **Repository access**: You need write access to the `akambaki/soma` repository

## Usage

Simply run the script from the repository root:

```bash
./create_issues.sh
```

## What the script creates

The script will create **55 GitHub issues** in total:

### Epic Issues (8 issues)
- **User Platform Epic** - Core user-facing features (105 pts)
- **Staff Operations Epic** - Internal tools and monitoring (68 pts)  
- **Administrative Management Epic** - System administration (60 pts)
- **Partner Integration Epic** - APIs and external integrations (42 pts)
- **XRPL Blockchain Epic** - Core blockchain integration (122 pts)
- **Development Infrastructure Epic** - CI/CD and development practices (21 pts)
- **Platform Operations Epic** - Infrastructure and monitoring (115 pts)
- **Security & Compliance Epic** - Security and regulatory compliance (60 pts)

### Story Issues (47 issues)
Individual user stories with detailed acceptance criteria, organized by epic and feature.

## Issue Organization

All issues are created with appropriate labels for easy filtering and organization:

- **Epic labels**: `epic`, `[epic-name]`, `[phase-number]`
- **Story labels**: `story`, `[epic-name]`, `[feature-name]`, `[story-points]`

## Story Points

Total estimated effort: **593 story points** across all features.

## Implementation Phases

Issues are labeled by implementation phase:
- **Phase 1 (Foundation)**: User Platform + XRPL Blockchain core features
- **Phase 2 (Operations)**: Staff Operations + Administrative Management
- **Phase 3 (Scaling)**: Platform Operations + Security & Compliance  
- **Phase 4 (Integration)**: Partner Integration + Development Infrastructure

## After Running the Script

Once the issues are created, you can:

1. **Use GitHub Projects** to organize issues into sprints
2. **Filter by labels** to view specific epics, features, or story point ranges
3. **Track progress** using GitHub's issue tracking features
4. **Create milestones** for each implementation phase
5. **Assign issues** to team members as needed

The issues are structured to support agile development practices with clear acceptance criteria and effort estimation.