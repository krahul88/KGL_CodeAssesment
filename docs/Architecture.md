# Application Architecture

## 1. Project layers

The application is split into four layers. Each layer has a separate responsibility.

- **API:** Exposes the HTTP endpoints, accepts requests, performs basic request validation, and returns responses.
- **Application:** Coordinates the evaluation use case. It prepares the order data and coordinates the audit operation.
- **Domain:** Contains the DTOs, rules, evaluation context, and condition-evaluation logic.
- **Infrastructure:** Handles SQL Server data access and caching.

## 2. Order evaluation flow

When an order is submitted, the application follows this sequence:

1. Convert the incoming request into an `OrderEvaluationContext`.
2. Load the active rulesets and sort them by priority.
3. Check the conditions configured at the ruleset level. All conditions in a group must match (`AND`).
4. When a ruleset matches, check its rules in sequence order.
5. Return the `ProductionPlant` from the first rule that matches.
6. Stop after the first matching ruleset, even if none of its rules match.
7. Save the evaluation result and the condition-level trace.


