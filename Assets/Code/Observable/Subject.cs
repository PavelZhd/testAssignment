using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subject<T> : IReadOnlyReactiveProperty<T>, IObserver<T>
{
    List<IObserver<T>> _observers = new List<IObserver<T>>();

    public Subject(T defaultValue = default) {
        value = defaultValue;
    }

    public T value { get; private set; }

    public void Dispose() {
        OnCompleted();
    }

    public void OnCompleted() {
        foreach (var observer in _observers) {
            observer.OnCompleted();
        }
        _observers.Clear();
    }

    public void OnError(Exception error) {
        foreach (var observer in _observers) {
            observer.OnError(error);
        }
        _observers.Clear();
        throw error;
    }

    public void OnNext(T value) {
        /*if (this.value != null && !this.value.Equals(value) && this.value is IDisposable disposable) {
            disposable.Dispose();
        }*/
        this.value = value;
        foreach (var observer in _observers) {
            observer.OnNext(value);
        }
    }

    public IDisposable Subscribe(IObserver<T> observer) {
        _observers.Add(observer);
        //if (!(value == null || value.Equals(default))) {
            observer.OnNext(value);
        //}
        return DisposableHelpers.Basic(() => {
            _observers?.Remove(observer); 
        });
    }
}
