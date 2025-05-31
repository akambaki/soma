#!/bin/bash

# SOMA Platform - GitHub Issues Creation Script
# This script creates GitHub issues for all epics and stories defined in EPICS_AND_STORIES.md

set -e

echo "🚀 Creating GitHub issues for SOMA Platform epics and stories..."

# Check if gh CLI is available and authenticated
if ! command -v gh &> /dev/null; then
    echo "❌ Error: GitHub CLI (gh) is not installed"
    exit 1
fi

if ! gh auth status &> /dev/null; then
    echo "❌ Error: Not authenticated with GitHub CLI. Run 'gh auth login' first."
    exit 1
fi

# Create Epic issues first
echo "📋 Creating Epic issues..."

# Epic 1: User Platform Epic
gh issue create \
    --title "Epic: User Platform - Core user-facing features (105 pts)" \
    --body "**Epic Description:** Provide end-users with comprehensive platform access including registration, wallet management, payments, and support capabilities.

## Features Included:
- Registration, Login & KYC
- Wallet Management
- Fiat On/Off-Ramp
- Payments & Transfers
- Notifications
- Support

## Story Points Total: 105

## Stories:
- User Registration and Authentication (8 pts)
- KYC Document Management (13 pts)
- XRPL Wallet Operations (13 pts)
- Balance and Transaction History (8 pts)
- Fiat Deposit and Withdrawal (21 pts)
- XRPL Asset Management (13 pts)
- Fund Transfers (13 pts)
- QR Code and Address-Based Transfers (8 pts)
- Multi-Channel Notifications (8 pts)
- User Support System (13 pts)

## Implementation Priority: Phase 1 (Foundation)

This epic is part of the core foundation and should be implemented in Phase 1 along with XRPL Blockchain Epic core features." \
    --label "epic,user-platform,phase-1" \
    --assignee akambaki

# Epic 2: Staff Operations Epic
gh issue create \
    --title "Epic: Staff Operations - Internal tools and monitoring (68 pts)" \
    --body "**Epic Description:** Provide staff members with tools to manage users, monitor transactions, handle support, and generate reports.

## Features Included:
- User Management
- Transaction Monitoring
- Support Tools
- Reporting

## Story Points Total: 68

## Stories:
- User Account Management (13 pts)
- KYC Approval and Fraud Detection (21 pts)
- Transaction Overview Dashboard (13 pts)
- Manual Transaction Overrides (13 pts)
- Support Ticket Management (13 pts)
- Compliance and Transaction Reports (8 pts)

## Implementation Priority: Phase 2 (Operations)

This epic provides essential operational tools for staff to manage the platform effectively." \
    --label "epic,staff-operations,phase-2" \
    --assignee akambaki

# Epic 3: Administrative Management Epic
gh issue create \
    --title "Epic: Administrative Management - System administration and configuration (60 pts)" \
    --body "**Epic Description:** Provide administrators with comprehensive system management capabilities including configuration, user roles, compliance, and system monitoring.

## Features Included:
- System Configuration
- Role & Permission Management
- Audit & Compliance
- Dashboard

## Story Points Total: 60

## Stories:
- Payment Provider Management (13 pts)
- Transaction Limits and Fee Configuration (13 pts)
- Staff Management System (13 pts)
- Audit Trail System (13 pts)
- Access Logs and Regulatory Tools (8 pts)
- Administrative Dashboard (13 pts)

## Implementation Priority: Phase 2 (Operations)

Essential for system administration and compliance management." \
    --label "epic,admin-management,phase-2" \
    --assignee akambaki

# Epic 4: Partner Integration Epic
gh issue create \
    --title "Epic: Partner Integration - APIs and external integrations (42 pts)" \
    --body "**Epic Description:** Enable partner integrations through APIs, dashboards, and event systems to extend platform capabilities.

## Features Included:
- API & Dashboard
- Webhook/Event Subscriptions

## Story Points Total: 42

## Stories:
- Partner API Management (13 pts)
- Partner Analytics Dashboard (8 pts)
- Event Subscription System (13 pts)
- Sandbox Environment (8 pts)

## Implementation Priority: Phase 4 (Integration)

Enables third-party integrations and partner ecosystem." \
    --label "epic,partner-integration,phase-4" \
    --assignee akambaki

# Epic 5: XRPL Blockchain Epic
gh issue create \
    --title "Epic: XRPL Blockchain - Core blockchain integration (122 pts)" \
    --body "**Epic Description:** Implement comprehensive XRPL blockchain integration for wallet operations, transaction handling, asset management, and node operations.

## Features Included:
- XRPL Wallet Operations
- Transaction Handling
- Asset Management
- Webhooks/Callbacks
- Fee & Reserve Management
- XRPL Node Management

## Story Points Total: 122

## Stories:
- Platform Wallet Management (21 pts)
- Secure Key Management (21 pts)
- XRPL Transaction Processing (21 pts)
- Ledger Monitoring (13 pts)
- Multi-Token Support (13 pts)
- Compliance Features (13 pts)
- XRPL Event Listening (13 pts)
- Reserve and Fee Monitoring (8 pts)
- Node Operations (21 pts)

## Implementation Priority: Phase 1 (Foundation)

Core blockchain functionality essential for platform operation." \
    --label "epic,xrpl-blockchain,phase-1" \
    --assignee akambaki

# Epic 6: Development Infrastructure Epic
gh issue create \
    --title "Epic: Development Infrastructure - CI/CD and development practices (21 pts)" \
    --body "**Epic Description:** Implement agile development practices and automated CI/CD pipelines to ensure quality and efficient development processes.

## Features Included:
- CI/CD Pipelines

## Story Points Total: 21

## Stories:
- Automated Testing and Deployment (21 pts)

## Implementation Priority: Phase 4 (Integration)

Essential for maintaining code quality and deployment efficiency." \
    --label "epic,dev-infrastructure,phase-4" \
    --assignee akambaki

# Epic 7: Platform Operations Epic
gh issue create \
    --title "Epic: Platform Operations - Infrastructure and monitoring (115 pts)" \
    --body "**Epic Description:** Establish robust deployment, monitoring, and infrastructure management to ensure platform reliability and scalability.

## Features Included:
- Environments
- Infrastructure as Code
- Containerization
- Monitoring & Alerts
- Secrets Management
- Backup & Disaster Recovery

## Story Points Total: 115

## Stories:
- Environment Management (13 pts)
- Infrastructure Automation (21 pts)
- Container Orchestration (21 pts)
- Comprehensive Monitoring (21 pts)
- Alerting and Health Checks (13 pts)
- Secure Secrets Handling (13 pts)
- Automated Backup System (13 pts)

## Implementation Priority: Phase 3 (Scaling)

Critical infrastructure for scalable and reliable platform operation." \
    --label "epic,platform-ops,phase-3" \
    --assignee akambaki

# Epic 8: Security & Compliance Epic
gh issue create \
    --title "Epic: Security & Compliance - Security measures and regulatory compliance (60 pts)" \
    --body "**Epic Description:** Implement comprehensive security measures and compliance frameworks to protect user data and meet regulatory requirements.

## Features Included:
- Data Encryption
- Penetration Testing
- Compliance
- Incident Response

## Story Points Total: 60

## Stories:
- Comprehensive Encryption (13 pts)
- Security Assessment Program (13 pts)
- Regulatory Compliance Framework (21 pts)
- Security Incident Management (13 pts)

## Implementation Priority: Phase 3 (Scaling)

Essential security and compliance requirements." \
    --label "epic,security-compliance,phase-3" \
    --assignee akambaki

echo "✅ Epic issues created successfully!"

# Now create individual story issues
echo "📝 Creating Story issues..."

# User Platform Epic Stories
gh issue create \
    --title "Story: User Registration and Authentication (8 pts)" \
    --body "**As a** new user  
**I want to** register using email/phone/OAuth and set up 2FA  
**So that** I can securely access the platform

## Acceptance Criteria:
- [ ] User can register with email or phone number
- [ ] OAuth registration supported (Google, Apple, etc.)
- [ ] Two-factor authentication setup required
- [ ] Email/SMS verification for account activation
- [ ] Password strength requirements enforced

**Story Points:** 8  
**Epic:** User Platform Epic  
**Feature:** Registration, Login & KYC" \
    --label "story,user-platform,registration,8-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: KYC Document Management (13 pts)" \
    --body "**As a** registered user  
**I want to** upload and track my KYC documents  
**So that** I can become verified and access full platform features

## Acceptance Criteria:
- [ ] Document upload interface (passport, ID, proof of address)
- [ ] Real-time verification status tracking
- [ ] Secure document storage and encryption
- [ ] Integration with KYC verification service
- [ ] Notification system for status updates

**Story Points:** 13  
**Epic:** User Platform Epic  
**Feature:** Registration, Login & KYC" \
    --label "story,user-platform,kyc,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: XRPL Wallet Operations (13 pts)" \
    --body "**As a** verified user  
**I want to** create, link, and recover XRPL wallets  
**So that** I can manage my digital assets securely

## Acceptance Criteria:
- [ ] XRPL wallet creation with secure key generation
- [ ] Wallet linking for existing XRPL addresses
- [ ] Wallet recovery using seed phrases
- [ ] Wallet backup and security warnings
- [ ] Multi-wallet support for users

**Story Points:** 13  
**Epic:** User Platform Epic  
**Feature:** Wallet Management" \
    --label "story,user-platform,wallet,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Balance and Transaction History (8 pts)" \
    --body "**As a** user with a wallet  
**I want to** view my balances and transaction history  
**So that** I can track my fiat and crypto holdings

## Acceptance Criteria:
- [ ] Real-time balance display (fiat and crypto)
- [ ] Comprehensive transaction history with filters
- [ ] Transaction details and status information
- [ ] Export functionality for transaction records
- [ ] Multi-currency balance tracking

**Story Points:** 8  
**Epic:** User Platform Epic  
**Feature:** Wallet Management" \
    --label "story,user-platform,wallet,8-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Fiat Deposit and Withdrawal (21 pts)" \
    --body "**As a** verified user  
**I want to** deposit and withdraw fiat currency  
**So that** I can fund my account and cash out

## Acceptance Criteria:
- [ ] Bank transfer integration for deposits/withdrawals
- [ ] PSP (Payment Service Provider) integration
- [ ] Instant notification system for transactions
- [ ] Transaction limits and fee display
- [ ] Multiple fiat currency support

**Story Points:** 21  
**Epic:** User Platform Epic  
**Feature:** Fiat On/Off-Ramp" \
    --label "story,user-platform,fiat,21-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: XRPL Asset Management (13 pts)" \
    --body "**As a** user  
**I want to** deposit and withdraw XRP and other XRPL tokens  
**So that** I can manage my cryptocurrency holdings

## Acceptance Criteria:
- [ ] XRP deposit/withdrawal functionality
- [ ] Support for issued XRPL tokens
- [ ] Address validation and QR code support
- [ ] Transaction fee estimation
- [ ] Real-time transaction tracking

**Story Points:** 13  
**Epic:** User Platform Epic  
**Feature:** Fiat On/Off-Ramp" \
    --label "story,user-platform,xrpl,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Fund Transfers (13 pts)" \
    --body "**As a** user  
**I want to** send and receive funds (fiat or XRPL tokens)  
**So that** I can make payments and transfers

## Acceptance Criteria:
- [ ] Send/receive functionality for fiat and crypto
- [ ] Contact management for frequent recipients
- [ ] Transaction confirmation workflow
- [ ] Fee calculation and display
- [ ] Transaction receipt generation

**Story Points:** 13  
**Epic:** User Platform Epic  
**Feature:** Payments & Transfers" \
    --label "story,user-platform,transfers,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: QR Code and Address-Based Transfers (8 pts)" \
    --body "**As a** user  
**I want to** use QR codes or addresses for transfers  
**So that** I can easily send funds without manual entry

## Acceptance Criteria:
- [ ] QR code generation for receiving funds
- [ ] QR code scanning for sending funds
- [ ] Address book management
- [ ] Address validation and verification
- [ ] Share functionality for payment requests

**Story Points:** 8  
**Epic:** User Platform Epic  
**Feature:** Payments & Transfers" \
    --label "story,user-platform,qr-code,8-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Multi-Channel Notifications (8 pts)" \
    --body "**As a** user  
**I want to** receive notifications via email, SMS, and in-app  
**So that** I stay informed about transactions and security events

## Acceptance Criteria:
- [ ] Email notification system
- [ ] SMS notification integration
- [ ] In-app notification center
- [ ] Notification preferences management
- [ ] Security alert system

**Story Points:** 8  
**Epic:** User Platform Epic  
**Feature:** Notifications" \
    --label "story,user-platform,notifications,8-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: User Support System (13 pts)" \
    --body "**As a** user  
**I want to** access help through chat, FAQ, and support tickets  
**So that** I can get assistance when needed

## Acceptance Criteria:
- [ ] In-app chat system
- [ ] Comprehensive FAQ section
- [ ] Support ticket submission and tracking
- [ ] Knowledge base search functionality
- [ ] Escalation system for complex issues

**Story Points:** 13  
**Epic:** User Platform Epic  
**Feature:** Support" \
    --label "story,user-platform,support,13-pts" \
    --assignee akambaki

# Staff Operations Epic Stories
gh issue create \
    --title "Story: User Account Management (13 pts)" \
    --body "**As a** staff member  
**I want to** view and manage user accounts and KYC statuses  
**So that** I can assist users and maintain platform integrity

## Acceptance Criteria:
- [ ] User search and filtering capabilities
- [ ] Account status overview and management
- [ ] KYC status tracking and history
- [ ] User activity monitoring
- [ ] Account suspension/activation controls

**Story Points:** 13  
**Epic:** Staff Operations Epic  
**Feature:** User Management" \
    --label "story,staff-operations,user-management,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: KYC Approval and Fraud Detection (21 pts)" \
    --body "**As a** compliance staff member  
**I want to** approve/reject KYC applications and flag suspicious activity  
**So that** I can ensure regulatory compliance and platform security

## Acceptance Criteria:
- [ ] KYC document review interface
- [ ] Approval/rejection workflow with comments
- [ ] Suspicious activity flagging system
- [ ] Risk scoring and alerts
- [ ] Audit trail for all actions

**Story Points:** 21  
**Epic:** Staff Operations Epic  
**Feature:** User Management" \
    --label "story,staff-operations,kyc-approval,21-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Transaction Overview Dashboard (13 pts)" \
    --body "**As a** staff member  
**I want to** monitor inflow/outflow and transaction statuses  
**So that** I can ensure proper platform operation

## Acceptance Criteria:
- [ ] Real-time transaction monitoring dashboard
- [ ] Transaction status filtering (pending, failed, suspicious)
- [ ] Transaction volume analytics
- [ ] Alert system for unusual patterns
- [ ] Export functionality for reports

**Story Points:** 13  
**Epic:** Staff Operations Epic  
**Feature:** Transaction Monitoring" \
    --label "story,staff-operations,transaction-monitoring,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Manual Transaction Overrides (13 pts)" \
    --body "**As a** operations staff member  
**I want to** manually override failed settlements  
**So that** I can resolve transaction issues

## Acceptance Criteria:
- [ ] Failed transaction identification system
- [ ] Manual override capabilities with approval workflow
- [ ] Override reason documentation
- [ ] Impact assessment tools
- [ ] Notification system for affected users

**Story Points:** 13  
**Epic:** Staff Operations Epic  
**Feature:** Transaction Monitoring" \
    --label "story,staff-operations,manual-overrides,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Support Ticket Management (13 pts)" \
    --body "**As a** support staff member  
**I want to** access and respond to user support tickets  
**So that** I can provide timely customer assistance

## Acceptance Criteria:
- [ ] Support ticket dashboard and queue management
- [ ] Ticket assignment and escalation system
- [ ] Response templates and knowledge base
- [ ] Internal notes and collaboration tools
- [ ] Performance metrics and reporting

**Story Points:** 13  
**Epic:** Staff Operations Epic  
**Feature:** Support Tools" \
    --label "story,staff-operations,support-tickets,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Compliance and Transaction Reports (8 pts)" \
    --body "**As a** staff member  
**I want to** generate exportable reports for transactions, KYC, and compliance  
**So that** I can meet regulatory requirements and business needs

## Acceptance Criteria:
- [ ] Report generation interface with filters
- [ ] CSV and PDF export capabilities
- [ ] Scheduled report generation
- [ ] Report templates for common requirements
- [ ] Access control for sensitive reports

**Story Points:** 8  
**Epic:** Staff Operations Epic  
**Feature:** Reporting" \
    --label "story,staff-operations,reporting,8-pts" \
    --assignee akambaki

# Administrative Management Epic Stories
gh issue create \
    --title "Story: Payment Provider Management (13 pts)" \
    --body "**As an** administrator  
**I want to** manage PSPs, banks, and XRPL settings  
**So that** I can configure payment processing and blockchain integration

## Acceptance Criteria:
- [ ] PSP configuration interface
- [ ] Bank integration settings management
- [ ] XRPL network configuration
- [ ] API key and credential management
- [ ] Connection testing and validation

**Story Points:** 13  
**Epic:** Administrative Management Epic  
**Feature:** System Configuration" \
    --label "story,admin-management,payment-providers,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Transaction Limits and Fee Configuration (13 pts)" \
    --body "**As an** administrator  
**I want to** configure transaction limits, fees, and compliance rules  
**So that** I can control platform operations and ensure compliance

## Acceptance Criteria:
- [ ] Transaction limit configuration by user tier
- [ ] Fee structure management for different operations
- [ ] Compliance rule engine configuration
- [ ] Geographic restrictions management
- [ ] A/B testing capabilities for fee structures

**Story Points:** 13  
**Epic:** Administrative Management Epic  
**Feature:** System Configuration" \
    --label "story,admin-management,limits-fees,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Staff Management System (13 pts)" \
    --body "**As an** administrator  
**I want to** invite and manage staff with defined roles  
**So that** I can control access to platform functions

## Acceptance Criteria:
- [ ] Staff invitation and onboarding system
- [ ] Role definition and permission management
- [ ] Role templates (admin, support, compliance, etc.)
- [ ] Access level configuration
- [ ] Staff activity monitoring

**Story Points:** 13  
**Epic:** Administrative Management Epic  
**Feature:** Role & Permission Management" \
    --label "story,admin-management,staff-management,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Audit Trail System (13 pts)" \
    --body "**As an** administrator  
**I want to** maintain full audit trails for sensitive operations  
**So that** I can ensure accountability and regulatory compliance

## Acceptance Criteria:
- [ ] Comprehensive logging of all sensitive actions
- [ ] Immutable audit trail storage
- [ ] Search and filtering capabilities
- [ ] Automated compliance reporting
- [ ] Integration with external audit systems

**Story Points:** 13  
**Epic:** Administrative Management Epic  
**Feature:** Audit & Compliance" \
    --label "story,admin-management,audit-trail,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Access Logs and Regulatory Tools (8 pts)" \
    --body "**As an** administrator  
**I want to** access logs and regulatory export tools  
**So that** I can meet compliance requirements and security standards

## Acceptance Criteria:
- [ ] Comprehensive access logging system
- [ ] Regulatory report generation
- [ ] Log retention and archival
- [ ] Data export for regulatory authorities
- [ ] Privacy-compliant log management

**Story Points:** 8  
**Epic:** Administrative Management Epic  
**Feature:** Audit & Compliance" \
    --label "story,admin-management,access-logs,8-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Administrative Dashboard (13 pts)" \
    --body "**As an** administrator  
**I want to** view real-time system health and business metrics  
**So that** I can monitor platform performance and growth

## Acceptance Criteria:
- [ ] Real-time system health monitoring
- [ ] Transaction volume and revenue metrics
- [ ] User growth and engagement analytics
- [ ] Performance alerts and notifications
- [ ] Customizable dashboard widgets

**Story Points:** 13  
**Epic:** Administrative Management Epic  
**Feature:** Dashboard" \
    --label "story,admin-management,dashboard,13-pts" \
    --assignee akambaki

# Partner Integration Epic Stories
gh issue create \
    --title "Story: Partner API Management (13 pts)" \
    --body "**As a** partner  
**I want to** access API keys and documentation  
**So that** I can integrate with the platform

## Acceptance Criteria:
- [ ] API key generation and management
- [ ] Comprehensive API documentation
- [ ] Rate limiting and usage monitoring
- [ ] API versioning support
- [ ] Developer portal with examples

**Story Points:** 13  
**Epic:** Partner Integration Epic  
**Feature:** API & Dashboard" \
    --label "story,partner-integration,api-management,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Partner Analytics Dashboard (8 pts)" \
    --body "**As a** partner  
**I want to** view my transaction stats and settlements  
**So that** I can monitor my integration performance

## Acceptance Criteria:
- [ ] Partner-specific transaction analytics
- [ ] Settlement tracking and reporting
- [ ] Revenue share calculations
- [ ] Performance metrics dashboard
- [ ] Historical data access

**Story Points:** 8  
**Epic:** Partner Integration Epic  
**Feature:** API & Dashboard" \
    --label "story,partner-integration,analytics,8-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Event Subscription System (13 pts)" \
    --body "**As a** partner  
**I want to** subscribe to platform events  
**So that** I can receive real-time updates for my integration

## Acceptance Criteria:
- [ ] Event subscription management interface
- [ ] Support for deposits, withdrawals, KYC status events
- [ ] Reliable webhook delivery system
- [ ] Event filtering and customization
- [ ] Retry logic and error handling

**Story Points:** 13  
**Epic:** Partner Integration Epic  
**Feature:** Webhook/Event Subscriptions" \
    --label "story,partner-integration,webhooks,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Sandbox Environment (8 pts)" \
    --body "**As a** partner  
**I want to** test and simulate events in a sandbox  
**So that** I can develop and test my integration safely

## Acceptance Criteria:
- [ ] Isolated sandbox environment
- [ ] Event simulation tools
- [ ] Test data generation
- [ ] Integration testing capabilities
- [ ] Documentation and examples

**Story Points:** 8  
**Epic:** Partner Integration Epic  
**Feature:** Webhook/Event Subscriptions" \
    --label "story,partner-integration,sandbox,8-pts" \
    --assignee akambaki

# XRPL Blockchain Epic Stories
gh issue create \
    --title "Story: Platform Wallet Management (21 pts)" \
    --body "**As a** system  
**I want to** create, import, and recover XRPL wallets  
**So that** I can manage user and platform wallets securely

## Acceptance Criteria:
- [ ] Automated wallet generation for new users
- [ ] Wallet import functionality for existing addresses
- [ ] Secure wallet recovery mechanisms
- [ ] Hot/cold wallet segregation
- [ ] Multi-signature wallet support

**Story Points:** 21  
**Epic:** XRPL Blockchain Epic  
**Feature:** XRPL Wallet Operations" \
    --label "story,xrpl-blockchain,wallet-management,21-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Secure Key Management (21 pts)" \
    --body "**As a** system administrator  
**I want to** implement secure key management  
**So that** I can protect user funds and platform assets

## Acceptance Criteria:
- [ ] HSM integration for key storage
- [ ] Encrypted storage solutions
- [ ] Key rotation procedures
- [ ] Access control and audit trails
- [ ] Disaster recovery for keys

**Story Points:** 21  
**Epic:** XRPL Blockchain Epic  
**Feature:** XRPL Wallet Operations" \
    --label "story,xrpl-blockchain,key-management,21-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: XRPL Transaction Processing (21 pts)" \
    --body "**As a** system  
**I want to** send payments, set up trustlines, and submit transactions  
**So that** I can process user requests on the XRPL

## Acceptance Criteria:
- [ ] Payment transaction creation and submission
- [ ] Trustline setup and management
- [ ] Transaction signing and validation
- [ ] Error handling and retry logic
- [ ] Transaction status tracking

**Story Points:** 21  
**Epic:** XRPL Blockchain Epic  
**Feature:** Transaction Handling" \
    --label "story,xrpl-blockchain,transaction-processing,21-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Ledger Monitoring (13 pts)" \
    --body "**As a** system  
**I want to** monitor the XRPL ledger in real-time  
**So that** I can track incoming and outgoing transactions

## Acceptance Criteria:
- [ ] Real-time ledger subscription
- [ ] Transaction detection and parsing
- [ ] Balance update processing
- [ ] Event triggering for transaction updates
- [ ] Performance optimization for high volume

**Story Points:** 13  
**Epic:** XRPL Blockchain Epic  
**Feature:** Transaction Handling" \
    --label "story,xrpl-blockchain,ledger-monitoring,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Multi-Token Support (13 pts)" \
    --body "**As a** system  
**I want to** support multiple XRPL tokens  
**So that** I can handle XRP and issued tokens

## Acceptance Criteria:
- [ ] XRP native token support
- [ ] Issued token integration
- [ ] Token metadata management
- [ ] Balance tracking for all supported tokens
- [ ] Token discovery and validation

**Story Points:** 13  
**Epic:** XRPL Blockchain Epic  
**Feature:** Asset Management" \
    --label "story,xrpl-blockchain,multi-token,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Compliance Features (13 pts)" \
    --body "**As a** compliance officer  
**I want to** implement freeze and blacklist features  
**So that** I can meet regulatory requirements

## Acceptance Criteria:
- [ ] Account freeze functionality
- [ ] Blacklist management system
- [ ] Compliance rule enforcement
- [ ] Regulatory reporting capabilities
- [ ] Audit trail for compliance actions

**Story Points:** 13  
**Epic:** XRPL Blockchain Epic  
**Feature:** Asset Management" \
    --label "story,xrpl-blockchain,compliance,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: XRPL Event Listening (13 pts)" \
    --body "**As a** system  
**I want to** listen for XRPL events  
**So that** I can respond to blockchain changes in real-time

## Acceptance Criteria:
- [ ] Event subscription system
- [ ] Transaction event processing
- [ ] Account change detection
- [ ] Reliable event delivery
- [ ] Event replay capabilities

**Story Points:** 13  
**Epic:** XRPL Blockchain Epic  
**Feature:** Webhooks/Callbacks" \
    --label "story,xrpl-blockchain,event-listening,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Reserve and Fee Monitoring (8 pts)" \
    --body "**As a** system  
**I want to** monitor and manage base reserves and transaction fees  
**So that** I can optimize costs and ensure transaction success

## Acceptance Criteria:
- [ ] Base reserve monitoring
- [ ] Transaction fee calculation
- [ ] Fee optimization algorithms
- [ ] Reserve management for platform accounts
- [ ] Cost reporting and analytics

**Story Points:** 8  
**Epic:** XRPL Blockchain Epic  
**Feature:** Fee & Reserve Management" \
    --label "story,xrpl-blockchain,fees-reserves,8-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Node Operations (21 pts)" \
    --body "**As a** system administrator  
**I want to** operate and monitor XRPL nodes  
**So that** I can ensure platform reliability and independence

## Acceptance Criteria:
- [ ] XRPL node deployment and configuration
- [ ] Node health monitoring
- [ ] Failover and redundancy setup
- [ ] Performance optimization
- [ ] Maintenance and upgrade procedures

**Story Points:** 21  
**Epic:** XRPL Blockchain Epic  
**Feature:** XRPL Node Management" \
    --label "story,xrpl-blockchain,node-operations,21-pts" \
    --assignee akambaki

# Development Infrastructure Epic Stories
gh issue create \
    --title "Story: Automated Testing and Deployment (21 pts)" \
    --body "**As a** development team  
**I want to** have automated testing, linting, builds, and deployments  
**So that** I can ensure code quality and rapid deployment

## Acceptance Criteria:
- [ ] Automated unit and integration testing
- [ ] Code linting and quality checks
- [ ] Automated build processes
- [ ] Multi-environment deployment automation
- [ ] Rollback capabilities

**Story Points:** 21  
**Epic:** Development Infrastructure Epic  
**Feature:** CI/CD Pipelines" \
    --label "story,dev-infrastructure,ci-cd,21-pts" \
    --assignee akambaki

# Platform Operations Epic Stories
gh issue create \
    --title "Story: Environment Management (13 pts)" \
    --body "**As a** DevOps engineer  
**I want to** maintain separate staging, production, and sandbox environments  
**So that** I can ensure proper testing and deployment processes

## Acceptance Criteria:
- [ ] Isolated environment configurations
- [ ] Environment-specific data and settings
- [ ] Promotion pipelines between environments
- [ ] Environment health monitoring
- [ ] Data refresh and anonymization for non-prod

**Story Points:** 13  
**Epic:** Platform Operations Epic  
**Feature:** Environments" \
    --label "story,platform-ops,environments,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Infrastructure Automation (21 pts)" \
    --body "**As a** DevOps engineer  
**I want to** use Infrastructure as Code tools  
**So that** I can ensure reproducible and manageable infrastructure

## Acceptance Criteria:
- [ ] Terraform/Ansible implementation
- [ ] Version-controlled infrastructure definitions
- [ ] Automated infrastructure provisioning
- [ ] Configuration drift detection
- [ ] Disaster recovery automation

**Story Points:** 21  
**Epic:** Platform Operations Epic  
**Feature:** Infrastructure as Code" \
    --label "story,platform-ops,infrastructure-automation,21-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Container Orchestration (21 pts)" \
    --body "**As a** DevOps engineer  
**I want to** implement Docker and Kubernetes/ECS  
**So that** I can ensure scalable and portable application deployment

## Acceptance Criteria:
- [ ] Docker containerization for all services
- [ ] Kubernetes/ECS orchestration setup
- [ ] Auto-scaling configuration
- [ ] Service mesh implementation
- [ ] Container security best practices

**Story Points:** 21  
**Epic:** Platform Operations Epic  
**Feature:** Containerization" \
    --label "story,platform-ops,containerization,21-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Comprehensive Monitoring (21 pts)" \
    --body "**As a** operations team  
**I want to** implement metrics, logging, and error tracking  
**So that** I can maintain platform health and performance

## Acceptance Criteria:
- [ ] Prometheus metrics collection
- [ ] Grafana dashboard implementation
- [ ] ELK/Loki logging stack
- [ ] Sentry error tracking
- [ ] Custom business metrics

**Story Points:** 21  
**Epic:** Platform Operations Epic  
**Feature:** Monitoring & Alerts" \
    --label "story,platform-ops,monitoring,21-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Alerting and Health Checks (13 pts)" \
    --body "**As a** operations team  
**I want to** implement uptime monitoring and alerting  
**So that** I can respond quickly to issues

## Acceptance Criteria:
- [ ] Uptime and health check monitoring
- [ ] PagerDuty/Slack alert integration
- [ ] Alert escalation procedures
- [ ] SLA monitoring and reporting
- [ ] Automated remediation for common issues

**Story Points:** 13  
**Epic:** Platform Operations Epic  
**Feature:** Monitoring & Alerts" \
    --label "story,platform-ops,alerting,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Secure Secrets Handling (13 pts)" \
    --body "**As a** security team  
**I want to** implement secure secrets management  
**So that** I can protect sensitive configuration and credentials

## Acceptance Criteria:
- [ ] HashiCorp Vault or AWS Secrets Manager
- [ ] Automated secret rotation
- [ ] Access control and audit trails
- [ ] Integration with deployment pipelines
- [ ] Secret scanning and leak detection

**Story Points:** 13  
**Epic:** Platform Operations Epic  
**Feature:** Secrets Management" \
    --label "story,platform-ops,secrets,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Automated Backup System (13 pts)" \
    --body "**As a** operations team  
**I want to** implement automated backups  
**So that** I can ensure data protection and recovery capabilities

## Acceptance Criteria:
- [ ] Automated database backups
- [ ] Configuration and code backups
- [ ] Wallet key backup procedures
- [ ] Backup testing and validation
- [ ] Disaster recovery runbooks

**Story Points:** 13  
**Epic:** Platform Operations Epic  
**Feature:** Backup & Disaster Recovery" \
    --label "story,platform-ops,backup,13-pts" \
    --assignee akambaki

# Security & Compliance Epic Stories
gh issue create \
    --title "Story: Comprehensive Encryption (13 pts)" \
    --body "**As a** security team  
**I want to** encrypt data at rest and in transit  
**So that** I can protect sensitive user and platform data

## Acceptance Criteria:
- [ ] Database encryption at rest
- [ ] TLS/SSL for data in transit
- [ ] Application-level encryption for PII
- [ ] Key management for encryption
- [ ] Performance optimization for encrypted data

**Story Points:** 13  
**Epic:** Security & Compliance Epic  
**Feature:** Data Encryption" \
    --label "story,security-compliance,encryption,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Security Assessment Program (13 pts)" \
    --body "**As a** security team  
**I want to** conduct regular vulnerability scans and penetration testing  
**So that** I can identify and address security weaknesses

## Acceptance Criteria:
- [ ] Automated vulnerability scanning
- [ ] Regular penetration testing schedule
- [ ] Bug bounty program implementation
- [ ] Security finding remediation tracking
- [ ] Security assessment reporting

**Story Points:** 13  
**Epic:** Security & Compliance Epic  
**Feature:** Penetration Testing" \
    --label "story,security-compliance,security-assessment,13-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Regulatory Compliance Framework (21 pts)" \
    --body "**As a** compliance team  
**I want to** implement GDPR, AML, and KYC compliance  
**So that** I can meet regulatory requirements

## Acceptance Criteria:
- [ ] GDPR compliance implementation
- [ ] AML monitoring and reporting
- [ ] KYC procedure enforcement
- [ ] Regulatory reporting automation
- [ ] Privacy by design principles

**Story Points:** 21  
**Epic:** Security & Compliance Epic  
**Feature:** Compliance" \
    --label "story,security-compliance,regulatory,21-pts" \
    --assignee akambaki

gh issue create \
    --title "Story: Security Incident Management (13 pts)" \
    --body "**As a** security team  
**I want to** implement incident response procedures  
**So that** I can handle security events effectively

## Acceptance Criteria:
- [ ] Incident response plan and procedures
- [ ] Security event logging and monitoring
- [ ] Incident classification and escalation
- [ ] Forensic analysis capabilities
- [ ] Post-incident review and improvement

**Story Points:** 13  
**Epic:** Security & Compliance Epic  
**Feature:** Incident Response" \
    --label "story,security-compliance,incident-response,13-pts" \
    --assignee akambaki

echo "🎉 All issues created successfully!"
echo ""
echo "📊 Summary:"
echo "- 8 Epic issues created"
echo "- 47 Story issues created"
echo "- Total Story Points: 593"
echo ""
echo "🔗 Issues are organized with proper labels for:"
echo "- Epic tracking"
echo "- Feature grouping"
echo "- Story point estimation"
echo "- Implementation phases"
echo ""
echo "✅ Ready for agile project management and sprint planning!"