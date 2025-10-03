using System;

namespace BrightSword.SwissKnife
{
    /// <summary>
    /// Wraps an instance and a dispose action, providing a simple <see cref="IDisposable"/> implementation.
    /// </summary>
    /// <typeparam name="T">The type of the wrapped instance.</typeparam>
    /// <remarks>
    /// Useful for managing resources or cleanup actions in a scoped/disposable pattern.
    /// </remarks>
    /// <example>
    /// <code>
    /// var resource = new Disposable<MyResource>(myResource, r => r.Cleanup());
    /// using (resource) { /* use resource.Instance */ }
    /// // Cleanup is called automatically on Dispose
    /// </code>
    /// </example>
    [Obsolete("Use using declarations or System.Threading.CancellationTokenSource, or implement IDisposable directly. Disposable<T> is superseded by modern C# patterns.")]
    public sealed class Disposable<T>(T instance, Action<T> dispose) : IDisposable
    {
        private readonly Action<T> _dispose = dispose ?? (_ => { });

    /// <summary>
    /// Gets the wrapped instance.
    /// </summary>
    public T Instance { get; } = instance;

    /// <summary>
    /// Invokes the dispose action and suppresses finalization.
    /// </summary>
    public void Dispose()
    {
        _dispose(Instance);
        GC.SuppressFinalize(this);
    }
}
}
