# AGENTS.md

This file provides guidance to coding agents (Claude Code, GitHub Copilot, etc.) when working with code in this repository.

Selema (Selenium Test Lifecycle Manager) is a cross-platform, multi-framework library, published to Maven Central (`io.github.javiertuya:selema`) and NuGet (`Selema`). It manages the WebDriver lifecycle, produces a unified HTML test log, captures screenshots/video on failure, and integrates with CI and remote browser services.

## Critical: the .NET code is auto-generated from Java

**Java is the single source of truth.** The `net/` C# code is produced from the Java sources by [JavaToCSharp](https://github.com/paulirwin/JavaToCSharp) plus heavy post-processing rules in `net/build.xml`. Any file whose header reads `THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT` (everything under `net/**/Translated/`) must never be hand-edited — changes are overwritten on the next conversion.

- To change library behavior: edit the Java sources under `java/src/main/java/giis/selema`, then regenerate C# and rebuild:
  ```bash
  cd net
  dotnet tool install JavaToCSharpCli --global   # once
  ant convert
  dotnet build
  ```
- `net/build.xml` (`convert` target) both runs the converter and applies hundreds of `replacefilter` token substitutions that bridge Java↔C# idioms (e.g. `WebDriver`→`IWebDriver`, `.getText()`→`.Text`) and synthesize the framework variants: JUnit4 tests are translated to **NUnit3**, then copied and transformed into **MSTest4**. When you touch Java that has no clean C# equivalent, you usually add/adjust a rule here rather than editing generated `.cs`.
- `java-mapping.yml` holds type/name mappings passed to the converter.
- CI runs `ant convert && dotnet build` on every push to catch Java changes that break the .NET build.

## Build and test

The two platforms are built independently. Run Maven from `java/` and dotnet from `net/`.

Java (Maven, single module):
```bash
cd java
mvn test                              # build + run tests
mvn test -Dtest=TestLifecycle4        # single test class
mvn test -Dtest='!**/selema/video/Test*'   # default CI scope (skip video-controller mock tests)
```
- Surefire is configured with `testFailureIgnore=true`, so `mvn test` exits 0 even with failures; inspect `target/surefire-reports` and the HTML log at `target/site/junit-noframes/junit-noframes.html`.
- JaCoCo coverage is attached automatically; report at `target/site/jacoco/`.
- Java 17 is the default toolchain (JUnit 6). The `jdk11-junit5` Maven profile activates automatically on JDK < 17, dropping to source 11 and JUnit 5 for compatibility. Published snapshots are built with Java 11.

.NET (`net/Selema.sln`, netstandard2.0 library + net test projects, SDK 10):
```bash
cd net
dotnet test                                                        # all
dotnet test --filter FullyQualifiedName~Test4giis.Selema.Mstest4   # subset
dotnet test --filter FullyQualifiedName!~Test4giis.Selema.Video    # default CI scope
```

## Test execution modes

Tests exercise several remote-browser topologies selected by a `selema.properties` file written at runtime (see `.github/workflows/test.yml`): `headless`, `grid` (Selenium Dynamic Grid), `preload-local` / `preload-remote` (preloaded standalone container + video sidecar), and `vcmock-local` / `vcmock-remote` (video-controller mock). The grid/preload modes require Docker and the `selenium/standalone-*` / `selenium/video` images pinned in the workflow `env:` block. Locally, `headless` needs only a browser; the container modes need Docker and the `video-controller` compose files.

## Architecture

Selema is organized around two collaborating concepts, mirrored in every supported framework:
- **`Lifecycle*`** controller (annotation/rule/base class per framework) detects test lifecycle events (before/after test, before/after class) and forwards them to the manager.
- **`SeleManager`** (`giis.selema.manager`) performs the work: it resolves and creates/disposes the WebDriver, and delegates most behavior to **services** injected via fluent `add(...)` calls.

Source layout (`java/src/main/java/giis/selema`):
- `manager/` — `SeleManager`, `SelemaConfig`, driver factory, driver-version strategy.
- `framework/junit4`, `framework/junit5` — the `Lifecycle*` adapters. NUnit3/4 and MSTest4 adapters live only on the generated .NET side.
- `services/` + `services/impl` + `services/browser` — pluggable services: CI detection (`ICiService`, Jenkins/GitHub), screenshots, watermark, JSCover, visual/soft assert, and the browser services (`SelenoidBrowserService`, `DynamicGridBrowserService`, `RemoteBrowserService`) with video controllers (`VideoControllerLocal`, `VideoControllerRemote`).
- `portable/selenium` — thin portability layer shared with the `portable-java`/`visual-assert` companion libraries.

`video-controller/` is a small **Node.js** REST server (`app/`, its own `Dockerfile`) plus parameterized Docker Compose files (`docker-compose-preload.yml`, `docker-compose-controller.yml`) used by preloaded-container modes to start/stop recording per test and copy each video to the reports folder. `wait-container-ready.sh` blocks until a container logs a readiness marker.

`samples/` holds the runnable per-framework usage examples referenced from the README (`samples-selema-junit4/junit5/mstest/nunit`); they have a dedicated `samples.yml` workflow.

Test output: a unified `selema-log.html` is written to `target/` (Java) or `reports/` (.NET), linking screenshots, diffs, and videos.

## Conventions

- Public API names are identical across platforms except for casing: Java `sm.driver()` / `setBrowser(...)` ↔ .NET `sm.Driver` / `SetBrowser(...)`. Keep this parity when adding API.
- `.github/workflows/test.yml` uses an `if:` guard on the test job to avoid running twice for local-branch PRs (runs the PR, not the duplicate push) while still allowing forked-repo and dependabot PRs — preserve it when editing the workflow.
