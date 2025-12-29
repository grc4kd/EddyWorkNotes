# README

## GitHub Actions status indicators

[![CodeQL](https://github.com/grc4kd/EddyWorkNotes/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/grc4kd/EddyWorkNotes/actions/workflows/github-code-scanning/codeql)

## Installation

To install `Eddy`, follow these steps:

1. Clone the repository from GitHub:

    ```bash
    git clone https://github.com/grc4kd/EddyWorkNotes.git
    ```

2. Navigate to the project directory:

    ```bash
    cd EddyWorkNotes
    ```

3. Install dependencies using NuGet Package Manager or .NET CLI:
    - Using NuGet Package Manager:
      Right-click on the `ui` project in Solution Explorer, select "Manage NuGet Packages", and install the required packages.
    - Using .NET CLI:
      Run the following command to restore the necessary dependencies:

      ```bash
      dotnet restore
      ```

4. Set up environment variables for database connection details using `dotnet user-secrets`:

    ```bash
    dotnet user-secrets --project ui/ set "ConnectionStrings:EddyWorkNotes" "Host=localhost:5432;Database=postgres;Username=postgres;Password=dbpassword1!"
    ```

5. Run the application:
   - Using Visual Studio or Visual Studio Code, run the project as usual.
   - Alternatively, you can use the .NET CLI to start the application:

    ```bash
     dotnet run --project ui/
     ```

6. Access the application in your web browser at `http://localhost:<port_number>`, where `<port_number>` is the
port number specified in the `ui/Properties/launchSettings.json` file. The default web app URL is `http://localhost:5085`

## Quick Start (Getting Started)

Eddy helps you track your skill development and maintain motivation while coding. Here's how to get started:

**1. Starting a New Task:**

- **Record Initial Work Notes:**  Log anything you need to get started. For example, I like to include things like:
  - Names, URLs, and metadata for systems involved with a new process. This doesn't normally include fine details or
  software architecture concerns, but is more about the servers and networks involved in the current problem.
  - Details required to replicate bugs and issues in development. Steps that reproduce the bug. A general but useful
  description of where to find the bug and what output to test to confirm it.
  - How much time I expect to spend on the problem. The type of problems I hope to track with this software should be
  broken down into achievable, time-bound tasks. I tend to spend around 30 minutes to 2 hours on any given problem.
  Some problems take longer than others.

**Essentially:** Track your work, get feedback, and use that feedback to improve and set new goals.

## Prerequisites

- This application requires a Postgres database server to store and retreive persistent data for users.
  - This project assumes a postgres server instance is configured at `localhost`.
- The postgres database connection can be changed for a development build in runtime app settings, under the directory `ui\appsettings.json`.
- An easy way to manage app secrets outside of `appsettings.json` is to use the `dotnet user-secrets` utility. The
app expects a database connection string to configured with the key `ConnectionStrings:EddyWorkNotes`. A
configured user secret at the project level has been tested and overrides the configuration from `appsettings.json`.

## Running the application

The application can be started as normal, with `dotnet run` or `dotnet run --project ui` depending on the working
directory. You can also use the debuggers of Visual Studio and Visual Studio Code to start the app with full support.
Unit tests are available to run with `dotnet test` or the IDE unit test explorer tools, respectively. The `ui` app
is all that has to run for the interactive server to come online. It handles data object-relational database mapping
through EF Core, an asynchronous timer interface written in Blazor and JavaScript, and a basic report view using the
built-in QuickGrid component.

## Project Structure

There are additional C# projects that support the `ui` project, managed with a solution file in the top-level directory.

These projects are as follows:

- `app` The backend services that host background functions and timers outside of the Blazor / web client context.
- `DataEntities` A project for mappings from the database layer to the application layer, handled by EF Core using
metadata from C# record types and property attribute annotations, such as:

    ```csharp
    [property: JsonPropertyName("description")] 
    public string? Description { get; set; } = Description;
    ```

  - This Description class member is a field assigned during the record's class constructor. It stores string data and
    maps to the property name "description" in lower-case when serialized or deserialized from another .NET type.
    Finally, this data type is publically accessible through the member accessor `WorkNote.Description`. You can get and
    set the value like any regular C# object property from other classes.

- `PlaywrightTests` This was created using the .NET library for Playwright automated UI testing. I've used it so far to
implement basic smoke testing through a web browser client during `dotnet test`.

- `test` This project contains unit and integration tests written in Xunit during project development. The goal for
coverage is to test anything beneath the UI layer here that depends on project / client code. All of these tests should
be run continuously through the development process and, therefore, they should be very lightweight and speedy.

And of course, you can read even more documentation inside of the [docs](./docs/) folder, but this wraps up the
high-level overview.
