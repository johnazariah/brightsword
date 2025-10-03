# BrightSword.Feber

BrightSword.Feber is an expression-based utility that composes per-property operations into compiled delegates for high performance. The key idea is to build Expression trees once and compile them into delegates (Action/Func) which are cached for repeated use.

Why use Feber
- Performance: compiled delegates run much faster than repeated reflection.
- Productivity: avoid writing repetitive property-by-property boilerplate.
- Reusability: compose complex operations from small building blocks.

Key Concepts
- ActionBuilder / FunctionBuilder - builders that compose `Expression` blocks and compile them into cached delegates.
- OperationExpressions - small Expression fragments representing per-property behavior.
- Caching pattern - builders cache compiled delegates with null-coalescing assignment (`_cached ??= Build()`).

Common Usage
- Create a builder that produces `Action<T>` to copy, validate or print properties.
- Call `Compile` or use the provided `Build` helper to obtain the delegate.
- Invoke the delegate repeatedly in performance-critical code.

Guidelines
- Preserve order of expressions when refactoring; expression order may affect semantics.
- Avoid capturing mutable external state in the built expression unless intentional.
- Ensure you understand parameter expressions used (e.g. `InstanceParameterExpression`) before changing builder signatures.

See the `BrightSword.Feber/Samples` folder for examples.
