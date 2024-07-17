using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReactiveProperty<T>: IObservable<T>, IDisposable
{
    T value { get; set; }
}
public class ReactiveProperty<T> : IReactiveProperty<T>, IReadOnlyReactiveProperty<T>
{
    public T value {
        get => _value; set {
            _value = value;
            IObserver<T>[] toExecute = new IObserver<T>[_observers.Count];
            _observers.CopyTo(toExecute);
            foreach (var observer in toExecute) {
                observer.OnNext(value);
            }
        }
    }
    private T _value;
    private List<IObserver<T>> _observers = new List<IObserver<T>>();
    
    public void Dispose() {
        _observers.ForEach(x => x.OnCompleted());
        _observers.Clear();
    }

    public IDisposable Subscribe(IObserver<T> observer) {
        var unsubscriber = DisposableHelpers.Basic(() => _observers?.Remove(observer));
        //if (!value.Equals(default))
        observer.OnNext(value);
        _observers.Add(observer);
        return unsubscriber;
    }
}

public interface IReadOnlyReactiveProperty<T> : IObservable<T>, IDisposable
{
    T value { get; }
}