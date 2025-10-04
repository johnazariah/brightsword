This monorepo currently contains the following projects, and associated test projects:

* BrightSword.Crucible
* BrightSword.Feber
* BrightSword.SwissKnife
* BrightSword.Squid

Each of these listed projects generates a nuget package with its own version number.

The following tasks need to be done:

* Develop a coherent build strategy that builds and packages each individual package and MSBuild scripts to implement the strategy
* A sensible versioning strategy that maintains existing version numbers of each package in an appropriate location, and tools to automatically increment the version numbers at the appropriate time
* A CI/CD pipeline that supports branch development, a protected main which only accepts PRs, and revs/publishes a specified package and all other packages that depend on it automatically
* Consistent, thorough XML documentation of the classes and methods in each project with examples, remarks and usage guidelines
* Consistent documentation in the `docs` folder of each project which should be kept in sync with code changes
* A top level `docs` folder with information about the monorepo itself, the components in the monorepo, and guidelines for building and contributing to the monorepo
* Extraction of class and method documentation to GitHub Pages, in addition to the documents in the project-specific `docs` folders
