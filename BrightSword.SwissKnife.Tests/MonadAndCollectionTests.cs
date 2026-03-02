using BrightSword.SwissKnife;

namespace BrightSword.SwissKnife.Tests;

#region MonadExtensions Tests

public class MonadExtensions_MaybeWithFunc
{
    [Fact]
    public void Returns_default_when_input_is_null()
    {
        string? input = null;
        var result = input.Maybe(s => s.Length);
        Assert.Equal(0, result);
    }

    [Fact]
    public void Returns_custom_default_when_input_is_null()
    {
        string? input = null;
        var result = input.Maybe(s => s.Length, -1);
        Assert.Equal(-1, result);
    }

    [Fact]
    public void Calls_func_when_input_is_non_null()
    {
        var input = "hello";
        var result = input.Maybe(s => s.Length);
        Assert.Equal(5, result);
    }

    [Fact]
    public void Returns_reference_type_default_when_null()
    {
        string? input = null;
        var result = input.Maybe(s => s.ToUpper());
        Assert.Null(result);
    }

    [Fact]
    public void Returns_func_result_for_reference_type()
    {
        var input = "hello";
        var result = input.Maybe(s => s.ToUpper());
        Assert.Equal("HELLO", result);
    }
}

public class MonadExtensions_MaybeWithAction
{
    [Fact]
    public void Skips_action_when_input_is_null()
    {
        string? input = null;
        var executed = false;
        input.Maybe(_ => executed = true);
        Assert.False(executed);
    }

    [Fact]
    public void Executes_action_when_input_is_non_null()
    {
        var input = "hello";
        var captured = "";
        input.Maybe(s => captured = s);
        Assert.Equal("hello", captured);
    }

    [Fact]
    public void Returns_original_input_when_non_null()
    {
        var input = "hello";
        var result = input.Maybe(_ => { });
        Assert.Same(input, result);
    }

    [Fact]
    public void Returns_null_when_input_is_null()
    {
        string? input = null;
        var result = input.Maybe(_ => { });
        Assert.Null(result);
    }
}

public class MonadExtensions_When
{
    [Fact]
    public void Func_overload_calls_func_when_predicate_true()
    {
        var input = "hello";
        var result = input.When(s => s.Length > 3, s => s.ToUpper());
        Assert.Equal("HELLO", result);
    }

    [Fact]
    public void Func_overload_returns_default_when_predicate_false()
    {
        var input = "hi";
        var result = input.When(s => s.Length > 3, s => s.ToUpper());
        Assert.Null(result);
    }

    [Fact]
    public void Func_overload_returns_custom_default_when_predicate_false()
    {
        var input = "hi";
        var result = input.When(s => s.Length > 3, s => s.Length, -1);
        Assert.Equal(-1, result);
    }

    [Fact]
    public void Func_overload_returns_default_when_null()
    {
        string? input = null;
        var result = input.When(s => s.Length > 3, s => s.ToUpper());
        Assert.Null(result);
    }

    [Fact]
    public void Action_overload_executes_when_predicate_true()
    {
        var input = "hello";
        var executed = false;
        input.When(s => s.Length > 3, _ => executed = true);
        Assert.True(executed);
    }

    [Fact]
    public void Action_overload_skips_when_predicate_false()
    {
        var input = "hi";
        var executed = false;
        input.When(s => s.Length > 3, _ => executed = true);
        Assert.False(executed);
    }

    [Fact]
    public void Action_overload_returns_original_input()
    {
        var input = "hello";
        var result = input.When(s => s.Length > 3, _ => { });
        Assert.Same(input, result);
    }

    [Fact]
    public void Action_overload_returns_null_when_null()
    {
        string? input = null;
        var result = input.When(s => true, _ => { });
        Assert.Null(result);
    }
}

public class MonadExtensions_Unless
{
    [Fact]
    public void Func_overload_calls_func_when_predicate_false()
    {
        var input = "hello";
        var result = input.Unless(s => s.Length < 3, s => s.ToUpper());
        Assert.Equal("HELLO", result);
    }

    [Fact]
    public void Func_overload_returns_default_when_predicate_true()
    {
        var input = "hello";
        var result = input.Unless(s => s.Length > 3, s => s.ToUpper());
        Assert.Null(result);
    }

    [Fact]
    public void Func_overload_returns_custom_default_when_predicate_true()
    {
        var input = "hello";
        var result = input.Unless(s => s.Length > 3, s => s.Length, -1);
        Assert.Equal(-1, result);
    }

    [Fact]
    public void Func_overload_returns_default_when_null()
    {
        string? input = null;
        var result = input.Unless(s => true, s => s.ToUpper());
        Assert.Null(result);
    }

    [Fact]
    public void Action_overload_executes_when_predicate_false()
    {
        var input = "hello";
        var executed = false;
        input.Unless(s => s.Length < 3, _ => executed = true);
        Assert.True(executed);
    }

    [Fact]
    public void Action_overload_skips_when_predicate_true()
    {
        var input = "hello";
        var executed = false;
        input.Unless(s => s.Length > 3, _ => executed = true);
        Assert.False(executed);
    }

    [Fact]
    public void Action_overload_returns_original_input()
    {
        var input = "hello";
        var result = input.Unless(s => false, _ => { });
        Assert.Same(input, result);
    }

    [Fact]
    public void Action_overload_returns_null_when_null()
    {
        string? input = null;
        var result = input.Unless(s => true, _ => { });
        Assert.Null(result);
    }
}

#endregion

#region ExceptionMonad Tests

public class ExceptionMonad_MakeSafe
{
    [Fact]
    public void MakeSafe_FuncT_captures_result()
    {
        Func<int> func = () => 42;
        var safe = func.MakeSafe();
        var result = safe();
        Assert.False(result.IsException);
        Assert.Equal(42, result.GetValueOrDefault());
    }

    [Fact]
    public void MakeSafe_FuncT_captures_exception()
    {
        Func<int> func = () => throw new InvalidOperationException("boom");
        var safe = func.MakeSafe();
        var result = safe();
        Assert.True(result.IsException);
        Assert.IsType<InvalidOperationException>(result.GetException());
    }

    [Fact]
    public void MakeSafe_FuncTR_captures_result()
    {
        Func<string, int> func = s => s.Length;
        var safe = func.MakeSafe();
        ExceptionMonad<string> input = "hello";
        var result = safe(input);
        Assert.False(result.IsException);
        Assert.Equal(5, result.GetValueOrDefault());
    }

    [Fact]
    public void MakeSafe_FuncTR_captures_exception()
    {
        Func<string, int> func = _ => throw new ArgumentException("bad");
        var safe = func.MakeSafe();
        ExceptionMonad<string> input = "hello";
        var result = safe(input);
        Assert.True(result.IsException);
        Assert.IsType<ArgumentException>(result.GetException());
    }

    [Fact]
    public void MakeSafe_FuncTR_propagates_exception_from_input()
    {
        Func<string, int> func = s => s.Length;
        var safe = func.MakeSafe();
        ExceptionMonad<string> input = new InvalidOperationException("prior");
        var result = safe(input);
        Assert.True(result.IsException);
        Assert.IsType<InvalidOperationException>(result.GetException());
    }

    [Fact]
    public void MakeSafe_FuncTIntR_captures_result()
    {
        Func<string, int, string> func = (s, i) => $"{s}_{i}";
        var safe = func.MakeSafe();
        ExceptionMonad<string> input = "item";
        var result = safe(input, 3);
        Assert.False(result.IsException);
        Assert.Equal("item_3", result.GetValueOrDefault());
    }

    [Fact]
    public void MakeSafe_FuncTIntR_captures_exception()
    {
        Func<string, int, string> func = (_, _) => throw new ArgumentException("bad");
        var safe = func.MakeSafe();
        ExceptionMonad<string> input = "item";
        var result = safe(input, 0);
        Assert.True(result.IsException);
    }

    [Fact]
    public void MakeSafe_FuncTIntR_propagates_exception_from_input()
    {
        Func<string, int, string> func = (s, i) => $"{s}_{i}";
        var safe = func.MakeSafe();
        ExceptionMonad<string> input = new InvalidOperationException("prior");
        var result = safe(input, 0);
        Assert.True(result.IsException);
        Assert.IsType<InvalidOperationException>(result.GetException());
    }

    [Fact]
    public void MakeSafe_FuncIEnumerableR_captures_results()
    {
        Func<IEnumerable<int>> func = () => new[] { 1, 2, 3 };
        var safe = func.MakeSafe();
        var results = safe().ToList();
        Assert.Equal(3, results.Count);
        Assert.All(results, r => Assert.False(r.IsException));
    }

    [Fact]
    public void MakeSafe_FuncIEnumerableR_captures_exception()
    {
        Func<IEnumerable<int>> func = () => throw new InvalidOperationException("boom");
        var safe = func.MakeSafe();
        var results = safe().ToList();
        Assert.Single(results);
        Assert.True(results[0].IsException);
    }

    [Fact]
    public void MakeSafe_FuncTIEnumerableR_captures_results()
    {
        Func<string, IEnumerable<char>> func = s => s.ToCharArray();
        var safe = func.MakeSafe();
        ExceptionMonad<string> input = "ab";
        var results = safe(input).ToList();
        Assert.Equal(2, results.Count);
        Assert.Equal('a', results[0].GetValueOrDefault());
        Assert.Equal('b', results[1].GetValueOrDefault());
    }

    [Fact]
    public void MakeSafe_FuncTIEnumerableR_propagates_exception_from_input()
    {
        Func<string, IEnumerable<char>> func = s => s.ToCharArray();
        var safe = func.MakeSafe();
        ExceptionMonad<string> input = new InvalidOperationException("prior");
        var results = safe(input).ToList();
        Assert.Single(results);
        Assert.True(results[0].IsException);
    }

    [Fact]
    public void MakeSafe_Action_does_not_throw()
    {
        Action action = () => throw new InvalidOperationException("boom");
        var safe = action.MakeSafe();
        var ex = Record.Exception(() => safe());
        Assert.Null(ex);
    }

    [Fact]
    public void MakeSafe_Action_executes_normally()
    {
        var executed = false;
        Action action = () => executed = true;
        var safe = action.MakeSafe();
        safe();
        Assert.True(executed);
    }

    [Fact]
    public void MakeSafe_ActionT_executes_normally()
    {
        var captured = "";
        Action<string> action = s => captured = s;
        var safe = action.MakeSafe();
        ExceptionMonad<string> input = "hello";
        safe(input);
        Assert.Equal("hello", captured);
    }

    [Fact]
    public void MakeSafe_ActionT_skips_when_exception()
    {
        var executed = false;
        Action<string> action = _ => executed = true;
        var safe = action.MakeSafe();
        ExceptionMonad<string> input = new InvalidOperationException("prior");
        safe(input);
        Assert.False(executed);
    }

    [Fact]
    public void MakeSafe_ActionT_swallows_exception_from_action()
    {
        Action<string> action = _ => throw new InvalidOperationException("boom");
        var safe = action.MakeSafe();
        ExceptionMonad<string> input = "hello";
        var ex = Record.Exception(() => safe(input));
        Assert.Null(ex);
    }
}

public class ExceptionMonad_SafeCall
{
    [Fact]
    public void SafeCall_FuncT_Action_executes_action_on_success()
    {
        Func<int> func = () => 42;
        var captured = 0;
        func.SafeCall(v => captured = v);
        Assert.Equal(42, captured);
    }

    [Fact]
    public void SafeCall_FuncT_Action_skips_action_on_exception()
    {
        Func<int> func = () => throw new InvalidOperationException("boom");
        var executed = false;
        func.SafeCall(_ => executed = true);
        Assert.False(executed);
    }

    [Fact]
    public void SafeCall_FuncT_FuncTR_chains_on_success()
    {
        Func<string> first = () => "hello";
        Func<string, int> second = s => s.Length;
        var result = first.SafeCall(second);
        Assert.False(result.IsException);
        Assert.Equal(5, result.GetValueOrDefault());
    }

    [Fact]
    public void SafeCall_FuncT_FuncTR_captures_first_exception()
    {
        Func<string> first = () => throw new InvalidOperationException("boom");
        Func<string, int> second = s => s.Length;
        var result = first.SafeCall(second);
        Assert.True(result.IsException);
    }

    [Fact]
    public void SafeCall_FuncT_FuncTR_captures_second_exception()
    {
        Func<string> first = () => "hello";
        Func<string, int> second = _ => throw new ArgumentException("bad");
        var result = first.SafeCall(second);
        Assert.True(result.IsException);
    }
}

public class ExceptionMonad_SelectAndSelectMany
{
    [Fact]
    public void Select_FuncTR_maps_successful_values()
    {
        var items = new List<ExceptionMonad<int>> { 1, 2, 3 };
        var results = items.Select((int x) => x * 2).ToList();
        Assert.Equal(3, results.Count);
        Assert.Equal(2, results[0].GetValueOrDefault());
        Assert.Equal(4, results[1].GetValueOrDefault());
        Assert.Equal(6, results[2].GetValueOrDefault());
    }

    [Fact]
    public void Select_FuncTR_propagates_exception()
    {
        var ex = new InvalidOperationException("boom");
        var items = new List<ExceptionMonad<int>> { 1, ex };
        var results = items.Select((int x) => x * 2).ToList();
        Assert.False(results[0].IsException);
        Assert.True(results[1].IsException);
    }

    [Fact]
    public void Select_FuncTIntR_maps_with_index()
    {
        var items = new List<ExceptionMonad<string>> { "a", "b" };
        var results = items.Select((string s, int i) => $"{s}{i}").ToList();
        Assert.Equal("a0", results[0].GetValueOrDefault());
        Assert.Equal("b1", results[1].GetValueOrDefault());
    }

    [Fact]
    public void Select_FuncTIntR_propagates_exception()
    {
        var ex = new InvalidOperationException("boom");
        var items = new List<ExceptionMonad<string>> { "a", ex };
        var results = items.Select((string s, int i) => $"{s}{i}").ToList();
        Assert.False(results[0].IsException);
        Assert.True(results[1].IsException);
    }

    [Fact]
    public void SelectMany_flattens_results()
    {
        var items = new List<ExceptionMonad<string>> { "ab", "cd" };
        var results = items.SelectMany((string s) => s.ToCharArray().Select(c => (int)c)).ToList();
        Assert.Equal(4, results.Count);
        Assert.All(results, r => Assert.False(r.IsException));
    }

    [Fact]
    public void SelectMany_propagates_exception()
    {
        var ex = new InvalidOperationException("boom");
        var items = new List<ExceptionMonad<string>> { "ab", ex };
        var results = items.SelectMany((string s) => s.ToCharArray().Select(c => (int)c)).ToList();
        Assert.Equal(3, results.Count);
        Assert.False(results[0].IsException);
        Assert.False(results[1].IsException);
        Assert.True(results[2].IsException);
    }
}

public class ExceptionMonadT_Tests
{
    [Fact]
    public void Implicit_conversion_from_value()
    {
        ExceptionMonad<int> monad = 42;
        Assert.False(monad.IsException);
        Assert.Equal(42, monad.GetValueOrDefault());
    }

    [Fact]
    public void Implicit_conversion_from_exception()
    {
        var ex = new InvalidOperationException("boom");
        ExceptionMonad<int> monad = ex;
        Assert.True(monad.IsException);
        Assert.Same(ex, monad.GetException());
    }

    [Fact]
    public void IsException_false_for_value()
    {
        ExceptionMonad<string> monad = "hello";
        Assert.False(monad.IsException);
    }

    [Fact]
    public void IsException_true_for_exception()
    {
        ExceptionMonad<string> monad = new ArgumentException("bad");
        Assert.True(monad.IsException);
    }

    [Fact]
    public void Bind_FuncTU_transforms_value()
    {
        ExceptionMonad<string> monad = "hello";
        var result = monad.Bind(s => s.Length);
        Assert.False(result.IsException);
        Assert.Equal(5, result.GetValueOrDefault());
    }

    [Fact]
    public void Bind_FuncTU_propagates_exception()
    {
        ExceptionMonad<string> monad = new InvalidOperationException("boom");
        var result = monad.Bind(s => s.Length);
        Assert.True(result.IsException);
    }

    [Fact]
    public void Bind_FuncTU_captures_new_exception()
    {
        ExceptionMonad<string> monad = "hello";
        var result = monad.Bind<int>(_ => throw new ArgumentException("bad"));
        Assert.True(result.IsException);
        Assert.IsType<ArgumentException>(result.GetException());
    }

    [Fact]
    public void Bind_FuncTIntU_transforms_with_index()
    {
        ExceptionMonad<string> monad = "item";
        var result = monad.Bind((s, i) => $"{s}_{i}", 5);
        Assert.False(result.IsException);
        Assert.Equal("item_5", result.GetValueOrDefault());
    }

    [Fact]
    public void Bind_FuncTIntU_propagates_exception()
    {
        ExceptionMonad<string> monad = new InvalidOperationException("boom");
        var result = monad.Bind((s, i) => $"{s}_{i}", 5);
        Assert.True(result.IsException);
    }

    [Fact]
    public void Bind_Action_executes_on_success()
    {
        ExceptionMonad<int> monad = 42;
        var captured = 0;
        monad.Bind(v => captured = v);
        Assert.Equal(42, captured);
    }

    [Fact]
    public void Bind_Action_skips_on_exception()
    {
        ExceptionMonad<int> monad = new InvalidOperationException("boom");
        var executed = false;
        monad.Bind(_ => executed = true);
        Assert.False(executed);
    }

    [Fact]
    public void BindMany_returns_results_on_success()
    {
        ExceptionMonad<string> monad = "abc";
        var results = monad.BindMany(s => s.ToCharArray().Select(c => c.ToString())).ToList();
        Assert.Equal(3, results.Count);
        Assert.All(results, r => Assert.False(r.IsException));
    }

    [Fact]
    public void BindMany_propagates_exception()
    {
        ExceptionMonad<string> monad = new InvalidOperationException("boom");
        var results = monad.BindMany(s => s.ToCharArray().Select(c => c.ToString())).ToList();
        Assert.Single(results);
        Assert.True(results[0].IsException);
    }

    [Fact]
    public void Do_executes_action_on_success()
    {
        ExceptionMonad<int> monad = 42;
        var captured = 0;
        monad.Do(v => captured = v);
        Assert.Equal(42, captured);
    }

    [Fact]
    public void Do_throws_on_exception()
    {
        var original = new InvalidOperationException("boom");
        ExceptionMonad<int> monad = original;
        var thrown = Assert.Throws<InvalidOperationException>(() => monad.Do(_ => { }));
        Assert.Same(original, thrown);
    }

    [Fact]
    public void GetValueOrDefault_returns_value_on_success()
    {
        ExceptionMonad<int> monad = 42;
        Assert.Equal(42, monad.GetValueOrDefault());
    }

    [Fact]
    public void GetValueOrDefault_returns_default_on_exception()
    {
        ExceptionMonad<int> monad = new InvalidOperationException("boom");
        Assert.Equal(0, monad.GetValueOrDefault());
    }

    [Fact]
    public void GetValueOrDefault_returns_custom_default_on_exception()
    {
        ExceptionMonad<int> monad = new InvalidOperationException("boom");
        Assert.Equal(-1, monad.GetValueOrDefault(-1));
    }

    [Fact]
    public void GetException_returns_null_on_success()
    {
        ExceptionMonad<int> monad = 42;
        Assert.Null(monad.GetException());
    }

    [Fact]
    public void GetException_returns_exception_on_failure()
    {
        var ex = new ArgumentException("bad");
        ExceptionMonad<int> monad = ex;
        Assert.Same(ex, monad.GetException());
    }
}

public class ExceptionMonadT_Unit
{
    [Fact]
    public void Unit_FuncT_success_path()
    {
        var safe = ExceptionMonad<int>.Unit(() => 42);
        var result = safe();
        Assert.False(result.IsException);
        Assert.Equal(42, result.GetValueOrDefault());
    }

    [Fact]
    public void Unit_FuncT_exception_path()
    {
        Func<int> throwing = () => throw new InvalidOperationException("boom");
        var safe = ExceptionMonad<int>.Unit(throwing);
        var result = safe();
        Assert.True(result.IsException);
    }

    [Fact]
    public void Unit_Action_success_path()
    {
        var executed = false;
        var safe = ExceptionMonad<object>.Unit(() => executed = true);
        safe();
        Assert.True(executed);
    }

    [Fact]
    public void Unit_Action_exception_path()
    {
        Action action = () => throw new InvalidOperationException("boom");
        var safe = ExceptionMonad<object>.Unit(action);
        var ex = Record.Exception(() => safe());
        Assert.Null(ex);
    }

    [Fact]
    public void Unit_ActionT_success_path()
    {
        var captured = "";
        var safe = ExceptionMonad<string>.Unit<string>(s => captured = s);
        ExceptionMonad<string> input = "hello";
        safe(input);
        Assert.Equal("hello", captured);
    }

    [Fact]
    public void Unit_ActionT_exception_input_path()
    {
        var executed = false;
        var safe = ExceptionMonad<string>.Unit<string>(_ => executed = true);
        ExceptionMonad<string> input = new InvalidOperationException("prior");
        safe(input);
        Assert.False(executed);
    }

    [Fact]
    public void Unit_FuncIEnumerableR_success_path()
    {
        var safe = ExceptionMonad<int>.Unit<int>(() => new[] { 1, 2, 3 }.AsEnumerable());
        var results = safe().ToList();
        Assert.Equal(3, results.Count);
        Assert.All(results, r => Assert.False(r.IsException));
    }

    [Fact]
    public void Unit_FuncIEnumerableR_exception_path()
    {
        Func<IEnumerable<int>> func = () => throw new InvalidOperationException("boom");
        var safe = ExceptionMonad<int>.Unit(func);
        var results = safe().ToList();
        Assert.Single(results);
        Assert.True(results[0].IsException);
    }

    [Fact]
    public void Unit_FuncTR_success_path()
    {
        var safe = ExceptionMonad<int>.Unit<string, int>(s => s.Length);
        ExceptionMonad<string> input = "hello";
        var result = safe(input);
        Assert.Equal(5, result.GetValueOrDefault());
    }

    [Fact]
    public void Unit_FuncTR_exception_in_input()
    {
        var safe = ExceptionMonad<int>.Unit<string, int>(s => s.Length);
        ExceptionMonad<string> input = new InvalidOperationException("prior");
        var result = safe(input);
        Assert.True(result.IsException);
    }

    [Fact]
    public void Unit_FuncTIntR_success_path()
    {
        var safe = ExceptionMonad<string>.Unit<string, string>((s, i) => $"{s}_{i}");
        ExceptionMonad<string> input = "item";
        var result = safe(input, 7);
        Assert.Equal("item_7", result.GetValueOrDefault());
    }

    [Fact]
    public void Unit_FuncTIntR_exception_in_input()
    {
        var safe = ExceptionMonad<string>.Unit<string, string>((s, i) => $"{s}_{i}");
        ExceptionMonad<string> input = new InvalidOperationException("prior");
        var result = safe(input, 0);
        Assert.True(result.IsException);
    }

    [Fact]
    public void Unit_FuncTIEnumerableR_success_path()
    {
        var safe = ExceptionMonad<char>.Unit<string, char>(s => s.ToCharArray());
        ExceptionMonad<string> input = "ab";
        var results = safe(input).ToList();
        Assert.Equal(2, results.Count);
        Assert.Equal('a', results[0].GetValueOrDefault());
    }

    [Fact]
    public void Unit_FuncTIEnumerableR_exception_in_input()
    {
        var safe = ExceptionMonad<char>.Unit<string, char>(s => s.ToCharArray());
        ExceptionMonad<string> input = new InvalidOperationException("prior");
        var results = safe(input).ToList();
        Assert.Single(results);
        Assert.True(results[0].IsException);
    }
}

#endregion

#region EnumerableExtensions Tests

public class EnumerableExtensions_AllUnique
{
    [Fact]
    public void Returns_true_for_unique_collection()
    {
        var items = new[] { 1, 2, 3, 4, 5 };
        Assert.True(items.AllUnique());
    }

    [Fact]
    public void Returns_false_for_non_unique_collection()
    {
        var items = new[] { 1, 2, 3, 2, 5 };
        Assert.False(items.AllUnique());
    }

    [Fact]
    public void Returns_true_for_empty_collection()
    {
        var items = Array.Empty<int>();
        Assert.True(items.AllUnique());
    }

    [Fact]
    public void Returns_true_for_single_element()
    {
        var items = new[] { 42 };
        Assert.True(items.AllUnique());
    }

    [Fact]
    public void Returns_false_for_all_duplicates()
    {
        var items = new[] { 1, 1, 1 };
        Assert.False(items.AllUnique());
    }

    [Fact]
    public void Works_with_strings()
    {
        var items = new[] { "a", "b", "c" };
        Assert.True(items.AllUnique());
    }

    [Fact]
    public void Detects_duplicate_strings()
    {
        var items = new[] { "a", "b", "a" };
        Assert.False(items.AllUnique());
    }
}

public class EnumerableExtensions_SortedListIsUnique
{
    [Fact]
    public void Returns_true_for_unique_sorted_list()
    {
        IList<int> items = new List<int> { 1, 2, 3, 4, 5 };
        Assert.True(items.SortedListIsUnique());
    }

    [Fact]
    public void Returns_false_for_duplicate_sorted_list()
    {
        IList<int> items = new List<int> { 1, 2, 2, 4, 5 };
        Assert.False(items.SortedListIsUnique());
    }

    [Fact]
    public void Returns_true_for_single_element()
    {
        IList<int> items = new List<int> { 42 };
        Assert.True(items.SortedListIsUnique());
    }

    [Fact]
    public void Returns_true_for_empty_list()
    {
        IList<int> items = new List<int>();
        Assert.True(items.SortedListIsUnique());
    }

    [Fact]
    public void Returns_false_for_adjacent_duplicates_at_end()
    {
        IList<int> items = new List<int> { 1, 2, 3, 3 };
        Assert.False(items.SortedListIsUnique());
    }

    [Fact]
    public void Returns_false_for_adjacent_duplicates_at_start()
    {
        IList<int> items = new List<int> { 1, 1, 2, 3 };
        Assert.False(items.SortedListIsUnique());
    }

    [Fact]
    public void Returns_true_for_two_distinct_elements()
    {
        IList<int> items = new List<int> { 1, 2 };
        Assert.True(items.SortedListIsUnique());
    }

    [Fact]
    public void Returns_false_for_two_equal_elements()
    {
        IList<int> items = new List<int> { 5, 5 };
        Assert.False(items.SortedListIsUnique());
    }
}

public class EnumerableExtensions_LastButOne
{
    [Fact]
    public void Returns_default_for_null()
    {
        IEnumerable<int>? items = null;
        Assert.Equal(0, items.LastButOne());
    }

    [Fact]
    public void Returns_default_for_empty()
    {
        var items = Array.Empty<int>();
        Assert.Equal(0, items.LastButOne());
    }

    [Fact]
    public void Returns_default_for_single_element()
    {
        var items = new[] { 42 };
        Assert.Equal(0, items.LastButOne());
    }

    [Fact]
    public void Returns_second_to_last_for_two_elements()
    {
        var items = new[] { 10, 20 };
        Assert.Equal(10, items.LastButOne());
    }

    [Fact]
    public void Returns_second_to_last_for_multiple_elements()
    {
        var items = new[] { 1, 2, 3, 4, 5 };
        Assert.Equal(4, items.LastButOne());
    }

    [Fact]
    public void Returns_null_for_null_reference_type_collection()
    {
        IEnumerable<string>? items = null;
        Assert.Null(items.LastButOne());
    }

    [Fact]
    public void Returns_null_for_single_string()
    {
        var items = new[] { "only" };
        Assert.Null(items.LastButOne());
    }

    [Fact]
    public void Returns_second_to_last_string()
    {
        var items = new[] { "a", "b", "c" };
        Assert.Equal("b", items.LastButOne());
    }
}

public class EnumerableExtensions_SequenceEqual
{
    [Fact]
    public void Returns_true_for_matching_sequences()
    {
        var source = new[] { 1, 2, 3 };
        IList<int> other = new List<int> { 1, 2, 3 };
        Assert.True(source.SequenceEqual(other));
    }

    [Fact]
    public void Returns_false_for_non_matching_sequences()
    {
        var source = new[] { 1, 2, 3 };
        IList<int> other = new List<int> { 1, 9, 3 };
        Assert.False(source.SequenceEqual(other));
    }

    [Fact]
    public void Returns_true_for_empty_sequences()
    {
        var source = Array.Empty<int>();
        IList<int> other = new List<int>();
        Assert.True(source.SequenceEqual(other));
    }

    [Fact]
    public void Returns_true_for_single_matching_element()
    {
        var source = new[] { 42 };
        IList<int> other = new List<int> { 42 };
        Assert.True(source.SequenceEqual(other));
    }

    [Fact]
    public void Returns_false_for_single_non_matching_element()
    {
        var source = new[] { 42 };
        IList<int> other = new List<int> { 99 };
        Assert.False(source.SequenceEqual(other));
    }

    [Fact]
    public void Works_with_strings()
    {
        var source = new[] { "hello", "world" };
        IList<string> other = new List<string> { "hello", "world" };
        Assert.True(source.SequenceEqual(other));
    }
}

public class EnumerableExtensions_Batch
{
    [Fact]
    public void Batches_exact_multiples()
    {
        var items = new[] { 1, 2, 3, 4, 5, 6 };
        var batches = items.Batch(3).ToList();
        Assert.Equal(2, batches.Count);
        Assert.Equal(new[] { 1, 2, 3 }, batches[0]);
        Assert.Equal(new[] { 4, 5, 6 }, batches[1]);
    }

    [Fact]
    public void Batches_with_remainder()
    {
        var items = new[] { 1, 2, 3, 4, 5 };
        var batches = items.Batch(3).ToList();
        Assert.Equal(2, batches.Count);
        Assert.Equal(new[] { 1, 2, 3 }, batches[0]);
        Assert.Equal(new[] { 4, 5 }, batches[1]);
    }

    [Fact]
    public void Single_element_produces_single_batch()
    {
        var items = new[] { 42 };
        var batches = items.Batch(5).ToList();
        Assert.Single(batches);
        Assert.Equal(new[] { 42 }, batches[0]);
    }

    [Fact]
    public void Empty_collection_produces_single_empty_batch()
    {
        var items = Array.Empty<int>();
        var batches = items.Batch(5).ToList();
        Assert.Single(batches);
        Assert.Empty(batches[0]);
    }

    [Fact]
    public void Batch_size_one_produces_individual_batches()
    {
        var items = new[] { 1, 2, 3 };
        var batches = items.Batch(1).ToList();
        Assert.Equal(3, batches.Count);
        Assert.Equal(new[] { 1 }, batches[0]);
        Assert.Equal(new[] { 2 }, batches[1]);
        Assert.Equal(new[] { 3 }, batches[2]);
    }

    [Fact]
    public void Batch_size_larger_than_collection_produces_single_batch()
    {
        var items = new[] { 1, 2, 3 };
        var batches = items.Batch(100).ToList();
        Assert.Single(batches);
        Assert.Equal(new[] { 1, 2, 3 }, batches[0]);
    }

    [Fact]
    public void Default_batch_size_is_100()
    {
        var items = Enumerable.Range(1, 250).ToList();
        var batches = items.Batch().ToList();
        Assert.Equal(3, batches.Count);
        Assert.Equal(100, batches[0].Count());
        Assert.Equal(100, batches[1].Count());
        Assert.Equal(50, batches[2].Count());
    }
}

#endregion

#region Disposable<T> Tests

public class DisposableT_Tests
{
    private class FakeResource
    {
        public bool WasDisposed { get; set; }
        public string Name { get; set; } = "";
    }

    [Fact]
    public void Constructor_with_instance_sets_instance_immediately()
    {
        var resource = new FakeResource { Name = "test" };
        using var disposable = new Disposable<FakeResource>(resource, null);
        Assert.Same(resource, disposable.Instance);
    }

    [Fact]
    public void Constructor_with_factory_creates_instance_lazily()
    {
        var created = false;
        var factory = () =>
        {
            created = true;
            return new FakeResource { Name = "lazy" };
        };
        using var disposable = new Disposable<FakeResource>(factory, null);
        Assert.False(created);
        var instance = disposable.Instance;
        Assert.True(created);
        Assert.Equal("lazy", instance.Name);
    }

    [Fact]
    public void Instance_returns_same_object_on_multiple_accesses()
    {
        var callCount = 0;
        var factory = () =>
        {
            callCount++;
            return new FakeResource { Name = "singleton" };
        };
        using var disposable = new Disposable<FakeResource>(factory, null);
        var first = disposable.Instance;
        var second = disposable.Instance;
        Assert.Same(first, second);
        Assert.Equal(1, callCount);
    }

    [Fact]
    public void Dispose_calls_dispose_action()
    {
        var resource = new FakeResource();
        var disposable = new Disposable<FakeResource>(resource, r => r.WasDisposed = true);
        disposable.Dispose();
        Assert.True(resource.WasDisposed);
    }

    [Fact]
    public void Dispose_nullifies_instance()
    {
        var resource = new FakeResource();
        var disposable = new Disposable<FakeResource>(resource, _ => { });
        Assert.Same(resource, disposable.Instance);
        disposable.Dispose();
        Assert.Null(disposable.Instance);
    }

    [Fact]
    public void Dispose_with_null_action_does_not_throw()
    {
        var resource = new FakeResource();
        var disposable = new Disposable<FakeResource>(resource, null);
        var ex = Record.Exception(() => disposable.Dispose());
        Assert.Null(ex);
    }

    [Fact]
    public void Constructor_with_null_factory_returns_null_instance()
    {
        using var disposable = new Disposable<FakeResource>((Func<FakeResource>?)null, null);
        Assert.Null(disposable.Instance);
    }

    [Fact]
    public void Using_pattern_calls_dispose()
    {
        var disposed = false;
        var resource = new FakeResource();
        using (var disposable = new Disposable<FakeResource>(resource, _ => disposed = true))
        {
            _ = disposable.Instance;
        }
        Assert.True(disposed);
    }
}

#endregion
