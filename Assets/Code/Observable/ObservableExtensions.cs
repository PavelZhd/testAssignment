using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ObservableExtensions
{
    public static IObservable<TRes> Select<TSrc, TRes>(this IObservable<TSrc> source, Func<TSrc, TRes> selector) {
        return new ObservableSelector<TSrc, TRes>(selector, source);
    }

    private class ObservableSelector<TSrc, TRes>: IObservable<TRes>
    {
        private Func<TSrc, TRes> _selector;
        private IObservable<TSrc> _source;

        public ObservableSelector(Func<TSrc, TRes> selector, IObservable<TSrc> source) {
            this._selector = selector;
            this._source = source;
        }

        public IDisposable Subscribe(IObserver<TRes> observer) {
            return _source.Subscribe(new ModifiedObserver<TSrc,TRes>(_selector, observer));
        }
    }

    private class ModifiedObserver<TSrc, TRes>: IObserver<TSrc>
    {
        private readonly Func<TSrc, TRes> _selector;
        private readonly IObserver<TRes> _observer;

        public ModifiedObserver(Func<TSrc, TRes> selector, IObserver<TRes> observer) {
            this._selector = selector;
            this._observer = observer;
        }

        
        public void OnCompleted() {
            _observer.OnCompleted();
        }

        public void OnError(Exception error) {
            _observer.OnError(error);
        }

        public void OnNext(TSrc value) {
            _observer.OnNext(_selector(value));
        }
    }

    public static IObservable<T> Where<T>(this IObservable<T> source, Func<T,bool> predicate) {
        return new ObservableFilter<T>(predicate, source);
    }
    private class ObservableFilter<T>: IObservable<T>
    {
        private readonly Func<T, bool> _predicate;
        private readonly IObservable<T> _source;

        public ObservableFilter(Func<T,bool> predicate, IObservable<T> source) {
            this._predicate = predicate;
            this._source = source;
        }

        public IDisposable Subscribe(IObserver<T> observer) {
            return _source.Subscribe(new FilteredObserver<T>(_predicate, observer));
        }
    }
    private class FilteredObserver<T> : IObserver<T>
    {
        private readonly Func<T, bool> _predicate;
        private readonly IObserver<T> _target;

        public FilteredObserver(Func<T, bool> predicate, IObserver<T> target) {
            this._predicate = predicate;
            this._target = target;
        }

        public void OnCompleted() {
            _target.OnCompleted();
        }

        public void OnError(Exception error) {
            _target.OnError(error);
        }

        public void OnNext(T value) {
            if (_predicate?.Invoke(value)??true) {
                _target.OnNext(value);
            }
        }
    }

    public static IObservable<T> DistinctUntilChanged<T>(this IObservable<T> source) {
        T curValue = default;
        return new ObservableFilter<T>(x => { 
            if (!curValue.Equals(x)) {
                curValue = x;
                return true;
            }
            return false;
        }, source);
    }

    /*public static IObservable<TRes> CombileLatest<TSrc1, TSrc2, TRes>(this IObservable<TSrc1> source1, TSrc2 source2, Func<TSrc1, TSrc2, TRes> selector) {

    }

    public static IObservable<TRes> CombineLatest<TSrc, TRes>(this IEnumerable<IObservable<TSrc>> sources, Func<IEnumerable<TSrc>,TRes> selector) {

    }*/
}

public interface IDisposableObservable<T>: IDisposable, IObservable<T> { }

