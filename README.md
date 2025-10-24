![Status](https://github.com/javiertuya/selema/actions/workflows/test.yml/badge.svg)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=my%3Aselema&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=my%3Aselema)
[![Maven Central](https://img.shields.io/maven-central/v/io.github.javiertuya/selema)](https://central.sonatype.com/artifact/io.github.javiertuya/selema)
[![Nuget](https://img.shields.io/nuget/v/Selema)](https://www.nuget.org/packages/Selema/)

# Selema - Selenium Test Lifecycle Manager

A cross-platform, multi-framework Selenium Test Lifecycle Manager for Java and .NET.
It automates WebDriver instantiation and configuration, provides a unified HTML test log,
useful debugging information, and integrates with CI platforms and remote browser services.

- Support:
  - Platforms: Java (>=11) and .NET (netstandard 2.0)
  - Test frameworks: JUnit 4, JUnit 5, NUnit 3–4, MSTest 2–3
  - CI/CD environments: Jenkins, GitHub Actions
  - Browser services with video recording:
    - Selenoid
    - Selenium Docker Dynamic Grid
    - Selenium Docker Preloaded Containers
- Features:
  - Transparent download, creation and disposal of Selenium WebDriver instances
  - Configurable driver lifetime (per test, per class)
  - Configurable strategy for resolving driver versions
  - Unified HTML test log
  - Automatic screenshot capture on test failure
  - Video recording with failure timestamping
  - Watermark showing the test name and status
  - Visual diffs for comparing large strings (Visual Assert)
  - Flaky test handling (retry failed tests) across supported test frameworks

## Breaking changes (v4.0.0)

- This release adds several browser services and moves them to their own namespace. The Selenoid browser service has been renamed/moved to `giis.selema.services.browser.SelenoidBrowserService`.
- .NET core packages (MSTest, NUnit and Selenium) are declared as private assets (like java provided) to prevent propagation to the client. The client must declare the chosen packages and the versions for each framework.

# Getting started

On Java, include the `selema` dependency as shown on
[Maven Central](https://search.maven.org/artifact/io.github.javiertuya/selema).
On .NET, include the `Selema` package in your project as shown on
[NuGet](https://www.nuget.org/packages/Selema/).

Selema is organized around two main components:
A `Lifecycle*` controller class that detects test lifecycle events
and a `SeleManager` that performs the required actions.

## Basic usage

To use Selema in your tests you only need to:
- Instantiate a `SeleManager` object (`sm` in the examples).
- Configure the test class with a `Lifecycle*` annotation or rule (depending on the test framework).

The appropriate Selenium WebDriver will be instantiated and closed before and after test execution
and is accessible to tests using the `driver()` method (Java) or the `Driver` property (.NET).

Expand/collapse the items below for instructions and examples for each supported platform.
[Full source of examples can be found here](samples):

<details open><summary><strong>JUnit 5</strong></summary>

(1) Extend the test class with `@ExtendWith(LifecycleJunit5.class)` and (2) declare a static `SeleManager` instance:

```java
@ExtendWith(LifecycleJunit5.class)
public class TestSampleJunit5 {
	private static SeleManager sm=new SeleManager().setBrowser("chrome");
	@Test
	public void testFailMethod() {
		sm.driver().get("https://en.wikipedia.org/");
		assertEquals("XXXX Wikipedia, the free encyclopedia", sm.driver().getTitle());
	}
}
```

</details>

<details><summary><strong>JUnit 4</strong></summary>

Declare (1) a static instance `sm` of `SeleManager`
and (2) a `LifecycleJunit4Test` rule receiving `sm` as argument

```java
public class TestSampleJunit4 {
	private static SeleManager sm=new SeleManager().setBrowser("chrome");
	@Rule public LifecycleJunit4Test tw = new LifecycleJunit4Test(sm);
	@Test
	public void testFailMethod() {
		sm.driver().get("https://en.wikipedia.org/");
		assertEquals("XXXX Wikipedia, the free encyclopedia", sm.driver().getTitle());
	}
}
```

NOTE: If SeleManager is confgured to manage one driver per class (see below) 
an additional rule must be declared just after SeleManager instantiation:

```java
	@ClassRule public static LifecycleJunit4Class cw = new LifecycleJunit4Class(sm);
```

</details>

<details><summary><strong>NUnit 3</strong></summary>

Declare (1) an instance `sm` of `SeleManager`
and (2) a decorate the test class with the `[LifecycleNunit3]` annotation:

```C#
    [LifecycleNunit3]
    public class TestFailedExample
    {
        protected SeleManager sm = new SeleManager().SetBrowser("chrome");
        [Test]
        public void TestFailMethod()
        {
            sm.Driver.Url = "https://en.wikipedia.org/";
            Assert.AreEqual("XXXX Wikipedia, the free encyclopedia", sm.Driver.Title);
        }
    }
```

</details>

<details><summary><strong>MSTest 3</strong></summary>

Declare (1) a static object `sm` of `SeleManager`, (2) instantiate `sm` in the class constructor
and (3) inherit the test class from `LifecycleMstest2`. 
See comments in the example for additional explanations:

```C#
    [TestClass]
    public class TestFailedExample : LifecycleMstest2
    {
        protected static SeleManager sm;
        public TestFailedExample()
        {
            sm = LifecycleMstest2.GetManager(sm).SetBrowser("edge");
        }
        //Note that due to the extensibility restrictions of MSTest, the test class must inherit the Lifecycle class.
        //Additionally the instantiation and configuration is made in the class constructor,
        //Method GetManager ensures that there is a unique sm instance in the class and is bound to the Lifecycle class.
        //Instantiation can be done in ClassInitialize as shown in the full example

        [TestMethod]
        public void TestFailMethod2()
        {
            sm.Driver.Url = "https://en.wikipedia.org/";
            Assert.AreEqual("XXXX Wikipedia, the free encyclopedia", sm.Driver.Title);
        }
    }
```

</details>

## Log files

Test execution produces an HTML log file named `selema-log.html` in the `target` folder (Java) or the `reports` folder (.NET). The log contains links to screenshots captured on failures, diff files and videos. Log entries are also written to the configured application logger (slf4j on Java, NLog on .NET).

![log-example](docs/log-file-example.png "Diff example")

# Basic configuration

Basic configuration is performed using setter methods on the SeleManager instance. All `set*` methods use a fluent style and can be chained in a single statement.

NOTE: The sections below use Java syntax. Unless otherwise stated, on .NET the packages, classes and methods have the same names but follow .NET naming conventions (capitalized).

## Driver management

- **Browser**: `setBrowser(String browser)` sets the WebDriver for the specified browser ("chrome", "firefox", "edge", "safari", "opera"). Default: chrome.
- **Remote driver**: `setDriverUrl(String driverUrl)` configures a RemoteWebDriver instead of a local one. The URL must point to the remote browser server. An overloaded `setDriverUrl(String driverUrl, int warmUpPeriod)` is useful when tests start immediately after the browser server is launched and the service needs a short warm-up period; during the warm-up period unsuccessful connections are retried.
- **Versions**: `setDriverVersion(String versionStrategy)` changes the driver version resolution strategy (see next section).
- **Modes of operation**: By default, Selema starts a WebDriver before each test and quits it after each test. You can change this behavior:
  - `setManageAtClass()`: Start a WebDriver before the first test in a class and quit it after all tests in the class.
  - `setManageNone()`: Do not start/quit WebDriver automatically. Useful to test scenarios where the user closes the browser. In this mode you can call `createDriver()` and `quitDriver(WebDriver driver)` manually. Use `hasDriver()` to check whether a driver has been created (accessing a non-created driver throws an exception).
  - `setManagedAtTest()`: Restore the default per-test behavior.
- **WebDriver capabilities (options)**: `setOptions(Map<String,Object> options)` adds specific capabilities to the WebDriver before creation.
- **WebDriver arguments**: `setArguments(String[] arguments)` adds execution arguments (for example `setArguments(new String[] {"--headless"})`).
- **Browser-specific options instance**: `setOptionsInstance(Capabilities optionsInstance)` provides a browser-specific options instance; capabilities from `setOptions` and `setArguments` are also applied.

NOTE: Since Selenium v4.9.0 standard capabilities no longer require vendor prefixes. When using `setOptionsInstance`, values provided via `setOptions` and `setArguments` are applied as well.

## Driver versions

By default Selema relies on third-party WebDriver managers
([Java](https://github.com/bonigarcia/webdrivermanager) and
[.NET](https://github.com/rosolko/WebDriverManager.Net))
to obtain a driver version that best matches the installed browser.

This suits most scenarios, but sometimes you need more control or compatibility fixes. Use `setDriverVersion` to change the strategy:
- `setDriverVersion(DriverVersion.LATEST_AVAILABLE)`: Use the latest available driver version regardless of browser version.
- `setDriverVersion(DriverVersion.SELENIUM_MANAGER)`: Disable third-party driver managers and rely on SeleniumManager (introduced in Selenium 4.6). Note: SeleniumManager is still considered beta.
- `setDriverVersion(DriverVersion.MATCH_BROWSER)`: Restore the default behavior (match the driver to the browser).
- `setDriverVersion(<VERSION_NUMBER>)`: Force a specific driver version.

## Log file location

The default report folder and log file name can be overridden by passing a `SelemaConfig` when constructing `SeleManager`. `SelemaConfig` supports these fluent methods:

- `setReportSubdir(String subdir)`: Change the reports folder name (relative to project root). Default: `target` (Java) and `reports` (.NET).
- `setProjectRoot(String root)`: Change the project root. Default: `.` (Java) and `../../../..` (.NET) — the latter assumes the test project is directly under the solution folder.
- `setName(String name)`: Change the log name. Useful to separate logs into different files.

Example:
`new SeleManager(new SelemaConfig().setReportSubdir("target/site").setName("custom"))`
produces reports in `target/site` and a log file named `selema-custom-log.html`.

## Delegated configurations

You can delegate configuration to a user-supplied object that implements an interface with a `configure` method:

- `setManagerDelegate(IManagerConfigDelegate configDelegate)`: The delegate executes configuration actions at manager instantiation. Useful for shared or complex configuration.
- `setDriverDelegate(IDriverConfigDelegate driverConfig)`: The delegate executes configuration actions after each driver is created.

# Advanced configuration and services

Most actions performed by SeleManager are implemented as services injected via overloaded `add(...)` methods. Some services are attached by default; others are optional and can be added with `add(<service-instance>)`. All `add` and service configuration methods use a fluent style.

## Remote browser servers and services

Setting the remote browser URL with `setDriverUrl()` is enough for Selema to create a `RemoteWebDriver` that connects to that URL. However, to use features such as video recording or VNC, add a specialized browser service to the SeleManager using `add(IBrowserService)`. `IBrowserService` exposes `setVideo()` and `setVnc()` to enable video recording and VNC (if supported).

Recorded videos are linked from the Selema log and uniquely identified by test class, test name, and CI context. The CI context includes branch and build number (Jenkins) or workflow name, job name, and run id (GitHub Actions). When a test fails, the log includes an approximate failure timestamp and a watermark is displayed in the browser for a few seconds. This is especially helpful when Selema manages drivers at the class level.

The concrete `IBrowserService` to inject depends on the kind of remote browser server, which falls into two main categories:

- **Dynamic browser containers**: automatically create and dispose browser and video recorder containers.
- **Preloaded browser containers**: use pre-created browser containers paired with a sidecar video recorder.

The supported remote browser platforms and their corresponding services are described below and illustrated with examples.

### Dynamic browser containers

- [Selenoid](https://aerokube.com/selenoid/latest/): Add `new SelenoidBrowserService()`. Default URL: `http://localhost:4444/wd/hub`. Use `setBrowserCapability(String key, Object value)` to configure Selenoid-specific capabilities.
- [Selenium Dynamic Grid](https://github.com/SeleniumHQ/docker-selenium): Add `new DynamicGridBrowserService()`. Default URL: `http://localhost:4444`. A `.toml` configuration file is required; see the docker-selenium project for details.

### Preloaded browser containers

Dynamic containers are flexible and easy to configure, but they have drawbacks: Selenoid is no longer maintained as of December 2024, and Selenium Dynamic Grid can introduce significant overhead because of starting and stopping containers for every Selenium session.

A compromise between ease of configuration and performance is to use preloaded containers. Before running tests, start a standalone browser container (docker-selenium) paired with a sidecar video recorder. Current video recorder implementations record continuously, producing very large video files that can become unmanageable. To address this, Selema manages recording by starting and stopping the recorder for each test/class and copying each video to the destination folder. This approach can be much faster than a dynamic grid [#37](https://github.com/javiertuya/selema/issues/37).

To use preloaded browser containers, instantiate `RemoteBrowserService()` and configure the video management with `setVideo(IVideoController)` by providing a video controller implementation. Two variants are supported:

- **Local**: If tests and browsers share Docker and file access, use `VideoControllerLocal(String label, String sourceFile, String targetFolder)`. Arguments: a label to identify containers when running in parallel, the temporary video filename recorded, and the folder where each video will be copied when the session ends.
- **Remote**: If tests do not have access to Docker or run on a different VM, use `VideoControllerRemote(String label, String controllerUrl, String targetFolder)`. The video controller is a tiny REST server that runs in the browsers' VM and is accesible at the `controllerUrl` endpoint. Multiple tests can share the same controller.

The `./video-controller` folder contains the video controller source (`app` folder) and parameterized Docker Compose files to start/stop preloaded browser containers and the video controller server. Params such as port or paths can be configured via environment variables (see `server.js`). Examples follow.

### Browser severs and services examples

<details><summary><strong>Selenoid</strong></summary>

```bash
# Creates the required browser configuration file and required folders that will be mapped in the containers
mkdir -p target/browsers
echo '{"chrome": {"default": "latest", "versions": {"latest": {"image":"selenoid/chrome:latest","port":"4444","tmpfs": {"/tmp":"size=512m"} } } } }' > target/browsers/browsers.json
mkdir -p target/video

# Downloads images and lauch selenoid container (project root is $GITHUB_WORKSPACE)
docker pull selenoid/chrome:latest
docker pull selenoid/video-recorder:latest-release
docker run -d --name selenoid -p 4444:4444 \
  -v /var/run/docker.sock:/var/run/docker.sock -v $GITHUB_WORKSPACE/target/browsers:/etc/selenoid/:ro \
  -v $GITHUB_WORKSPACE/target/site/video/:/opt/selenoid/video/ -e OVERRIDE_VIDEO_OUTPUT_DIR=$GITHUB_WORKSPACE/target/video/ \
  aerokube/selenoid:latest-release
```

Instanciate the Selema manager as follows:
```java
SeleManager sm = new SeleManager().setDriverUrl("http://localhost:4444/wd/hub")
		.add(new SelenoidBrowserService().setVideo());
```

</details>


<details><summary><strong>Selenium Dynamic Grid</strong></summary>

Assume recordings are placed in `/assets` and the required grid configuration file is `/grid.toml` (see [docker-selenium](https://github.com/SeleniumHQ/docker-selenium) for examples):


```bash
mkdir -p /assets
sudo chown 1200:1201 /assets
docker run -d --name selenium --shm-size="2g" -p 4444:4444 \
  -v /grid.toml:/opt/selenium/docker.toml \
  -v /assets:/opt/selenium/assets \
  -v /var/run/docker.sock:/var/run/docker.sock \
  selenium/standalone-docker:4.35.0-20250909
```

Instanciate the Selema manager as follows:
```java
SeleManager sm = new SeleManager().setDriverUrl("http://localhost:4444")
		.add(new DynamicGridBrowserService().setVideo());
```

</details>


<details><summary><strong>Preloaded browser containers (local)</strong></summary>

Assuming the repository root is the current folder, a Docker network `grid` exists, the tests have access to the `grid` network, and the temporary folder for recordings is `/tmp/videos`. To create a pair labeled `chrome`:

```bash
mkdir -p /tmp/videos
LABEL=chrome NETWORK=grid FOLDER=/tmp/videos docker compose -f ./video-controller/docker-compose-preload.yml up -d 
```

Container names use the label with prefixes `selenium-node-` and `selenium-video-`. AIf they share a network with the tests, access them by container name and internal port. Additional environment variables can configure exported ports (e.g., `PORTS=4444:4444`) or the user that runs the containers (e.g., `USER=$UID`).

Instanciate the Selema manager as follows:
```java
SeleManager sm = new SeleManager().setDriverUrl("http://selenium-node-chrome:4444")
		.add(new RemoteBrowserService()
				.setVideo(new VideoControllerLocal("chrome", "/tmp/videos/chrome.mp4", "./target"))
		);
```

</details>


<details open><summary><strong>Preloaded browser containers (remote)</strong></summary>

Assuming the repository root is the current folder, a Docker network `grid` exists, and the temporary folder for recordings is `/tmp/videos`. To start the video controller:

```bash
mkdir -p /tmp/videos
NETWORK=grid FOLDER=/tmp/videos PORTS=4449:4449 docker compose -f ./video-controller/docker-compose-controller.yml up -d --build
# Alternative for debugging: start video recorder server (same port) without a container
# cd video-controller/app
# npm install && npm start
```

To start a pair of browser and recorder labeled `chrome`:
```bash
LABEL=chrome NETWORK=grid FOLDER=/tmp/videos docker compose -f ./video-controller/docker-compose-preload.yml up -d 
```

Container names use the label with prefixes `selenium-node-` and `selenium-video-`. If they share a network with the tests, access them by container name and internal port. Additional environment variables can configure exported ports (e.g., `PORTS=4444:4444`) or the user that runs the containers (e.g., `USER=$UID`).

Instanciate the Selema manager as follows:
```java
SeleManager sm = new SeleManager().setDriverUrl("http://selenium-node-chrome:4444")
		.add(new RemoteBrowserService()
				.setVideo(new VideoControllerRemote("chrome", "http://localhost:4449/selema-video-controller", "./target"))
		);
```

</details>


## Watermark service

Displays text in the browser's top-left corner showing the test name and its status.
Attach it with `add(new WatermarkService())`. Customize the service with:

- `setDelayOnFailure(int seconds)`: After a test failure, wait the given number of seconds to allow interactive inspection or to capture video.
- `setBackground(String color)`: Set a background color to improve visibility (default: no background).

Watermarks are inserted automatically after a test failure. You can also insert a watermark manually with `watermark()` (for example, after navigating to a URL) or write arbitrary text using `watermarkText(String value)`.

## JavaScript coverage service (JSCover)

Attach JSCover integration with `add(JsCoverService.getInstance(<instrumented-root-url>))` to collect JavaScript coverage:

- Coverage is stored in the browser's local storage while running instrumented code.
- When a new driver session starts, previously recorded coverage is restored to local storage (except on the very first run).
- Before closing a driver, coverage is saved from local storage to a file (`jscover.json`).

Notes:

- Instantiate the service as a singleton using `JsCoverService.getInstance`.
- JavaScript must be pre-instrumented with JSCover; `<instrumented-root-url>` is the root for instrumented files.
- Add the provided helper HTML file `jscoverage-restore-local-storage.html` (java/src/main/resources/...) to the instrumented root to allow restoring coverage between sessions.
- Tested with JSCover 2.0.6 in [File Mode](http://tntim96.github.io/JSCover/manual/manual.xml#fileMode).

## Predefined services

- **CI/CD services** (`ICiService`): SeleManager detects known CI platforms and attaches the appropriate service. Use `getCiService()` to access:
  - `isLocal()`: true when running outside recognized CI platforms.
  - `getName()`: platform name (e.g., local, jenkins, github).
  - `getJobId()`: unique job identifier including build number.
  - `getJobName()`: job identifier without the build number.
- **Screenshot service**: Captures screenshots and links them in the log. Tests can call `screenshot(String fileName)` on SeleManager.
- **Visual Assert service**: Compares large strings and writes an HTML diff accessible from the log. Use `visualAssertEquals(...)` on SeleManager. See the [Visual Assert documentation](https://github.com/javiertuya/visual-assert) for details.
- **Soft Assert service**: A soft-assert variant of Visual Assert:
  - `softAssertEquals(...)` records failures and generates diffs without throwing immediately.
  - Call `softAssertAll()` at the end of a test to throw an exception if any soft assertions failed.
  - Call `softAssertClear()` at the start of a test to reset recorded soft-assert messages.

# Handling flaky tests

A common mitigation for flaky tests is to retry the test until it passes or a maximum number of retries is reached. Selema provides helpers for retries; examples are in the samples folder.

- **JUnit 5**: Add the rerunner-jupiter dependency and annotate tests with `@RepeatedIfExceptionsTest(repeats = <repetitions>)`. Do NOT also annotate with `@Test`.
- **JUnit 4**: Use Selema's custom rule. After declaring the manager rules, declare a `RepeatedTestRule` instance and annotate tests with `@RepeatedIfExceptionsTest(repeats = <repetitions>)`. You MUST still use `@Test`.
- **NUnit 3–4**: Use the built-in retry attribute: `[Retry(<repetitions>)]`.
- **MSTest 2–3**: Use Selema's custom attribute: replace `[TestMethod]` with `[RetryTestMethod(<repetitions>)]` for tests that should be retried.
