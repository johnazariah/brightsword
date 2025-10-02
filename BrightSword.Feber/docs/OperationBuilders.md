# OperationBuilders

Purpose
- A collection of small builder helpers to create common operation expression fragments used by the library (e.g., null checks, assignment operations, branching).

Key APIs
- BuildNullCheck(ParameterExpression p)
- BuildAssignment(Expression left, Expression right)

Usage
```csharp
var check = OperationBuilders.BuildNullCheck(instanceParam);
```

Notes
- Operation builders are low-level building blocks used by the ActionBuilder/FunctionBuilder flow.
