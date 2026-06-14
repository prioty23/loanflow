# LoanFlow Architecture

## Overview

LoanFlow is organized as a deployable modular monolith with clear project boundaries.

## Projects

### Domain

Contains business entities, value objects, domain events, and core rules with no infrastructure dependencies.

### Application

Contains use cases, orchestration logic, abstractions, and authorization constants. Depends only on `LoanFlow.Domain`.

### Infrastructure

Contains persistence, ASP.NET Core Identity, EF Core, SQL Server integration, and seed data. Depends on `LoanFlow.Application` and `LoanFlow.Domain`.

### Web

Contains the Blazor UI, pages, components, and application startup. Depends on `LoanFlow.Application` and `LoanFlow.Infrastructure`.

### UnitTests

Contains unit tests for application and domain behavior. Depends on `LoanFlow.Application` and `LoanFlow.Domain`.

## Dependency Rules

- `LoanFlow.Domain` has no project references.
- `LoanFlow.Application` references `LoanFlow.Domain`.
- `LoanFlow.Infrastructure` references `LoanFlow.Application` and `LoanFlow.Domain`.
- `LoanFlow.Web` references `LoanFlow.Application` and `LoanFlow.Infrastructure`.
- `LoanFlow.UnitTests` references `LoanFlow.Application` and `LoanFlow.Domain`.
