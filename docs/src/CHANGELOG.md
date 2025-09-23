# Initial Setup and Documentation

## Document Info
Changes from [2025-09-13] - [2025-09-23]
|         |         |
| ------- | ------- |
| Author  | Griffin |
| Project | Eddy    |
|         |         |

## Project README
A comprehensive README file has been created to provide an overview of the project, its purpose, and usage instructions. This is crucial for onboarding new contributors and users.

## Additional Documentation - *mdBook*
An `mdBook` project has been created to provide detailed documentation for the application. This includes multiple chapters that guide users through application concepts and implementation details. This documentation is a **work in progress**, and not all ideas are expected to be implemented. Care should be taken to avoid adding additional documentation before committing to a plan to implement a working feature or improvement.

## Blazor UI Project Setup
- Documentation has been added to outline common conventions for Blazor and C# development. This includes best practices for code organization, naming standards, and code conventions.
- A secure application plan has also been documented, detailing strategies for implementing security measures such as authentication, authorization, and data protection.
- The "Eddy" class library is reusable across different parts of the application. This separation of concerns improves modularity and simplifies future enhancements.
- A minimal, barebones Blazor UI project has been set up to serve as a starting point for further development. This provides a clean foundation without unnecessary dependencies or configurations.

## Task Timer
- Basic Pomodoro task timer features were implemented in `TaskTimer`, enabling users to start, pause, and reset tasks.
- A dedicated task timer page and corresponding component were created, allowing users to interact with the timer through a user-friendly interface.
- Tests were added to ensure that the break time functionality works as expected, verifying its reliability and accuracy.
- Playwright tests were added to automate testing across different browsers and ensure consistent behavior.

## Features Tested
1. Timer Operations
- Timer initialization and cancellation handling.
- Addition of a Cancel method to handle timer cancellation.
- Toggle pause functionality with accurate tracking of elapsed time.

2. Event Handling
- Triggering of TimeElapsed events upon completion.
- Testing cancellation toggling pause to ensure correct behavior.
- Proper handling of cancellation in TaskTimer and its associated tests.

3. Playwright (Integration Testing)
- Ensure main page for `TaskTimer.razor` is loaded completely.
- Test the state of the page after the Start button is pressed.
