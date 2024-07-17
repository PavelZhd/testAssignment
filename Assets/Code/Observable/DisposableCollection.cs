using System;
using System.Collections.Generic;


public interface ICompositeDisposable: IDisposable
{
    void Add(IDisposable disposable);
}
public class DisposableCollection: ICompositeDisposable
{
    private List<IDisposable> _subDisposables = new List<IDisposable>();
    public void Add(IDisposable disposable) { _subDisposables.Add(disposable); }

    public virtual void Dispose() {
        foreach (var i in _subDisposables) {
            if (i != null)
                i.Dispose();
        }
    }
}

public static class DisposableHelpers
{
    public static IDisposable Basic(Action onDisposed) {
        return new DummyDisposable(onDisposed);
    }

    private class DummyDisposable: IDisposable
    {
        private Action _onDisposed;
        public DummyDisposable(Action onDisposed) {
            _onDisposed = onDisposed;
        }

        public void Dispose() {
            _onDisposed?.Invoke();
        }
    }
}

public static class DisposableExtensions
{
    public static T AddTo<T>(this T item, ICompositeDisposable collection) where T:IDisposable {
        collection.Add(item);
        return item;
    }

    public static T SetTo<T>(this T item, SerialDisposable collection) where T : IDisposable {
        collection.current = item;
        return item;
    }
}