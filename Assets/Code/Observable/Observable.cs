using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Observable
{
    public static IObservable<T> Return<T>(T value) {
        return new DummyObservable<T>(value);
    }


    private class DummyObservable<T> : IObservable<T>
    {
        public T value { get; }
        public DummyObservable(T value) {
            this.value = value;
        }
        public IDisposable Subscribe(IObserver<T> observer) {
            observer.OnNext(value);
            observer.OnCompleted();
            return new DummyDisposable();
        }
    }


    

    private class DummyDisposable : IDisposable
    {
        public void Dispose() { }
    }
}


public static class ObserverHelper
{
    public class DummyObserver<T> : IObserver<T>
    {
        private Action<T> _listener;

        public DummyObserver(Action<T> listener) {
            this._listener = listener;
        }

        void IObserver<T>.OnCompleted() {
            _listener = null;
        }

        void IObserver<T>.OnError(Exception error) {
            _listener = null;
            throw error;
        }

        void IObserver<T>.OnNext(T value) {
            _listener?.Invoke(value);
        }
    }

    public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> listener) {
        return observable.Subscribe(new DummyObserver<T>(listener));
    }
}

public class Unit {
    public override bool Equals(object obj) {
        return false;
    }
    private Unit() { }

    public static Unit identity => new Unit();
}