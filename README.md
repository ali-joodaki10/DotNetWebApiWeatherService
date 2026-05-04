About This Project
WeatherCollector.Api is a robust, resilient weather data collection API built for long-term autonomous operation (20+ years) under unreliable conditions. It stores raw JSON responses from an external weather service to ensure future compatibility and simplicity.

Architecture & Layering
The project follows a clean, layered architecture to promote maintainability, testability, and separation of concerns:


WeatherCollector.Api
│
├── Controllers         // HTTP request handling (API endpoints)
├── Services            // Business logic and orchestration
├── ExternalServices    // External API communication
├── Repositories        // Data access layer (EF Core + Dapper)
├── Entites             // Domain and persistence models
├── Data                // DbContext and EF migrations
├── BackgroundJobs      // Hosted services for periodic tasks
├── Middleware          // Cross-cutting concerns (e.g. timeout handling)

Key Patterns & Technologies
Repository Pattern:

Interface: IWeatherRepository

Implementation uses EF Core for standard CRUD and Dapper for high-performance bulk deletion (cleanup).

HttpClientFactory with Polly:

Resilience policies applied include retry with exponential backoff, timeout, and circuit breaker patterns to handle flaky network/external API issues gracefully.

Middleware-based Global Timeout:

Implements a 5-second end-to-end request timeout via custom middleware that propagates cancellation tokens and returns 504 Gateway Timeout on timeouts.

Fallback Strategy:

If fetching fresh data fails, the API falls back to returning the latest cached weather record, enabling graceful degradation.

Background Data Retention Worker:

Runs hourly cleanup jobs to keep database size manageable, deleting old records efficiently with Dapper and SQL Common Table Expression (CTE).

Data Storage Design
The WeatherRecord entity stores:

Id: Auto-increment Identity
CreatedAt: UTC timestamp of data collection
RawJson: Full raw JSON string from the external API
Storing raw JSON prioritizes forward compatibility without fragile deserialization, supporting future schema changes and replay.
