# Contributing to BrightSword

Thank you for your interest in contributing to BrightSword! This document provides guidelines and information for contributors.

## Code of Conduct

We are committed to providing a welcoming and inspiring community for all. Please be respectful and constructive in your interactions.

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check existing issues to avoid duplicates.

**Good bug reports include:**
- Clear, descriptive title
- Steps to reproduce the issue
- Expected vs actual behavior
- Environment details (.NET version, OS, etc.)
- Code samples or error messages
- Screenshots if applicable

### Suggesting Enhancements

Enhancement suggestions are welcome! Please provide:
- Clear use case and motivation
- Detailed description of the proposed functionality
- Examples of how it would be used
- Potential alternatives considered

### Pull Requests

1. **Fork the repository** and create your branch from `main`:
   ```bash
   git checkout -b feature/amazing-feature
   ```

2. **Make your changes**:
   - Follow the coding standards (see below)
   - Add tests for new functionality
   - Update documentation as needed

3. **Test your changes**:
   ```bash
   ./build.ps1 -Target Test
   ```

4. **Build and verify**:
   ```bash
   ./build.ps1 -Target CI
   ```

5. **Commit your changes**:
   - Use clear commit messages
   - Follow conventional commit format (see below)

6. **Push** to your fork and **create a Pull Request**

## Development Setup

### Prerequisites

- .NET 10 SDK or later
- Git
- A code editor (Visual Studio, VS Code, Rider)

### Clone and Build

```bash
# Clone your fork
git clone https://github.com/YOUR_USERNAME/BrightSword.git
cd BrightSword

# Add upstream remote
git remote add upstream https://github.com/brightsword/BrightSword.git

# Build
./build.ps1
```

## Coding Standards

### C# Style Guidelines

- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful names for variables, methods, and classes
- Keep methods small and focused
- Avoid deep nesting (max 3-4 levels)
- Use nullable reference types (`#nullable enable`)

### Naming Conventions

- **Classes**: PascalCase (e.g., `ActionBuilder`)
- **Methods**: PascalCase (e.g., `BuildAction`)
- **Properties**: PascalCase (e.g., `InstanceParameter`)
- **Parameters**: camelCase (e.g., `propertyInfo`)
- **Private fields**: _camelCase (e.g., `_cachedDelegate`)
- **Constants**: PascalCase (e.g., `DefaultTimeout`)

### Code Organization

```csharp
// Namespaces - alphabetically
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrightSword.PackageName
{
    /// <summary>
    /// XML documentation for class
    /// </summary>
    public class MyClass
    {
        // Private fields
        private readonly IService _service;
        
        // Constructors
        public MyClass(IService service)
        {
            _service = service;
        }
        
        // Public properties
        public string Name { get; set; }
        
        // Public methods
        public void DoSomething()
        {
            // Implementation
        }
        
        // Private methods
        private void HelperMethod()
        {
            // Implementation
        }
    }
}
```

### XML Documentation

All public APIs must have XML documentation:

```csharp
/// <summary>
/// Brief description of what the method does.
/// </summary>
/// <param name="input">Description of the input parameter.</param>
/// <returns>Description of what is returned.</returns>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="input"/> is null.
/// </exception>
/// <example>
/// <code>
/// var result = MyMethod("example");
/// Console.WriteLine(result);
/// </code>
/// </example>
/// <remarks>
/// Additional notes about usage or behavior.
/// </remarks>
public string MyMethod(string input)
{
    ArgumentNullException.ThrowIfNull(input);
    return input.ToUpper();
}
```

## Testing Guidelines

### Writing Tests

- Use descriptive test method names: `MethodName_Scenario_ExpectedBehavior`
- Follow Arrange-Act-Assert pattern
- Test both happy paths and edge cases
- Use meaningful test data
- Keep tests focused and independent

### Test Organization

```csharp
[TestClass]
public class MyClassTests
{
    [TestMethod]
    public void DoSomething_WithValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var myClass = new MyClass();
        var input = "test";
        
        // Act
        var result = myClass.DoSomething(input);
        
        // Assert
        Assert.AreEqual("expected", result);
    }
    
    [TestMethod]
    public void DoSomething_WithNullInput_ThrowsArgumentNullException()
    {
        // Arrange
        var myClass = new MyClass();
        
        // Act & Assert
        var exception = new Action(() => myClass.DoSomething(null))
            .ExpectException<ArgumentNullException>();
        
        Assert.IsNotNull(exception);
    }
}
```

## Commit Message Format

We follow the [Conventional Commits](https://www.conventionalcommits.org/) specification:

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

- **feat**: New feature
- **fix**: Bug fix
- **docs**: Documentation changes
- **style**: Code style changes (formatting, etc.)
- **refactor**: Code refactoring
- **test**: Adding or updating tests
- **chore**: Build process or auxiliary tool changes

### Examples

```
feat(swissknife): add new string extension methods

Add ToTitleCase and ToKebabCase extension methods for strings.
These methods provide convenient string transformations.

Closes #123
```

```
fix(feber): resolve null reference in FunctionBuilder

The FunctionBuilder was throwing NullReferenceException when
processing null property values. Added null checks and tests.

Fixes #456
```

## Versioning

We use [Semantic Versioning](https://semver.org/):

- **MAJOR** version for incompatible API changes
- **MINOR** version for new functionality (backwards-compatible)
- **PATCH** version for bug fixes (backwards-compatible)

### Updating Versions

Use the provided script to increment versions:

```powershell
# Patch version (1.0.0 -> 1.0.1)
./increment-version.ps1 -Package BrightSword.SwissKnife

# Minor version (1.0.0 -> 1.1.0)
./increment-version.ps1 -Package BrightSword.Feber -Component Minor

# Major version (1.0.0 -> 2.0.0)
./increment-version.ps1 -Package BrightSword.Squid -Component Major
```

## Documentation

### Code Documentation

- Add XML comments to all public APIs
- Include `<summary>`, `<param>`, `<returns>`, `<exception>` tags
- Provide `<example>` blocks with code samples
- Use `<remarks>` for additional context

### Package Documentation

Each package has a `docs/` folder. When adding features:

1. Update or create relevant `.md` files
2. Include code examples
3. Explain use cases and best practices
4. Update the package's README.md

### Monorepo Documentation

Update the top-level `docs/` folder for:

- Build process changes
- CI/CD updates
- Architecture decisions
- Contributing guidelines (this file)

## Pull Request Process

1. **Update your branch** with latest `main`:
   ```bash
   git fetch upstream
   git rebase upstream/main
   ```

2. **Ensure all checks pass**:
   - Build succeeds
   - All tests pass
   - Code coverage is maintained or improved

3. **Update documentation**:
   - XML comments for new APIs
   - Package docs for new features
   - Update CHANGELOG if applicable

4. **Create the Pull Request**:
   - Clear title following commit message format
   - Detailed description of changes
   - Reference related issues
   - Screenshots for UI changes (if applicable)

5. **Address review feedback**:
   - Respond to comments
   - Make requested changes
   - Push updates to your branch

6. **Merge**:
   - Maintainers will merge once approved
   - PR will be squash-merged with a clean commit message

## Review Process

### What We Look For

- **Code Quality**: Clean, readable, maintainable code
- **Tests**: Adequate test coverage with meaningful tests
- **Documentation**: Complete and accurate documentation
- **Compatibility**: No breaking changes without justification
- **Performance**: No significant performance regressions

### Review Timeline

- Initial review within 3-5 business days
- Follow-up reviews within 1-2 business days
- Larger PRs may take longer

## Release Process

Releases are handled by maintainers:

1. Version increment via script
2. Update CHANGELOG
3. Create release tag
4. GitHub Actions builds and publishes to NuGet.org
5. GitHub Release with release notes

## Getting Help

- **Documentation**: Check the `docs/` folder
- **Issues**: Search existing GitHub issues
- **Discussions**: Use GitHub Discussions for questions
- **Contact**: Reach out to maintainers

## Recognition

Contributors will be recognized in:
- Repository contributor list
- Release notes for significant contributions
- Project documentation for major features

## License

By contributing, you agree that your contributions will be licensed under the **Creative Commons Legal Code (CC0 1.0 Universal)**.

This means your contributions:
- Can be used commercially
- Can be modified and distributed
- Require attribution to BrightSword Technologies Pte Ltd
- Do not require sharing derivative works under the same license

See the [LICENSE](../LICENSE) file for full details.

---

Thank you for contributing to BrightSword! ??
