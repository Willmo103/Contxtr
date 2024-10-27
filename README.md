# Contxtr
*Context-aware document processing and analysis toolkit*

## Overview
Contxtr is an evolution of my previous project [RepoScribe](https://github.com/yourusername/RepoScribe), rebuilt from the ground up with improved architecture and expanded capabilities. While RepoScribe began as a simple tool to flatten codebases into markdown files, Contxtr aims to be a comprehensive toolkit for processing, analyzing, and relating various types of content using AI.

## Background & Journey
As a junior developer, I created RepoScribe to solve a specific problem: documenting codebases in a readable format. While successful in its core functionality (and still used in production!), the project taught me valuable lessons about software architecture and design:

- **Lesson 1:** Started with tight coupling and hard-coded dependencies
- **Lesson 2:** Mixed different architectural patterns inconsistently
- **Lesson 3:** Lacked proper separation of concerns
- **Lesson 4:** Configuration management was scattered and inconsistent

Rather than continue building on a shaky foundation, I decided to rebuild from scratch, applying these lessons learned and implementing proper software engineering practices.

## Core Features (MVP)
- Document flattening with improved architecture
- Content versioning and relationship tracking
- Structured output with metadata
- Extensible processor pipeline

## Technology Stack
### Core Solution
- .NET 8.0
- C# 12

### Required Packages
Contxtr.Core:
- Microsoft.Extensions.Configuration.Abstractions
- Microsoft.Extensions.DependencyInjection.Abstractions
- Microsoft.Extensions.Logging.Abstractions

Contxtr.Infrastructure:
- Microsoft.Extensions.Configuration.Json
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Logging
- Serilog
- Serilog.Sinks.Console
- Serilog.Sinks.File

Contxtr.CLI:
- System.CommandLine

### Future Language Extensions
- Python (for AI/ML capabilities)
- TypeScript (for web interface)

## Project Structure
```
Contxtr/
├── src/
│   ├── Contxtr.Core/             # Core domain models and interfaces
│   ├── Contxtr.Infrastructure/   # Implementation of core interfaces
│   └── Contxtr.CLI/             # Command line interface
├── tests/
│   ├── Contxtr.Core.Tests/
│   └── Contxtr.Infrastructure.Tests/
└── docs/
    ├── DEVLOG.md                # Development progress log
    └── architecture/            # Architecture decision records
```

## Installation
```bash
# Clone the repository
git clone https://github.com/yourusername/Contxtr.git

# Navigate to the project directory
cd Contxtr

# Build the solution
dotnet build

# Run the CLI
cd src/Contxtr.CLI
dotnet run -- --help
```

## Usage
```bash
# Flatten a codebase
contxtr flatten --input ./my-project --output ./documentation.md

# Process with versioning
contxtr process --input ./my-project --track-versions

# Additional commands coming soon...
```

## Development Status
This project is actively under development. Check [DEVLOG.md](./docs/DEVLOG.md) for the latest updates and progress.

## Contributing
While this project serves as a portfolio piece demonstrating my growth as a developer, contributions and feedback are welcome! Please feel free to:

- Open issues with suggestions
- Submit pull requests
- Provide feedback on architecture decisions
- Share ideas for future features

## License
MIT

## Acknowledgments
Special thanks to the users of RepoScribe who provided valuable feedback and use cases that helped shape this evolution.