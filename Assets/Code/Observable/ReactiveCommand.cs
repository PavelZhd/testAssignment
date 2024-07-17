using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReactiveCommand<T> : IObservable<T>, IDisposable
{
    IObservable<bool> isInteractive { get; }
    void Trigger(T value);
}

public class ReactiveCommand<T>: IReactiveCommand<T>
{
    public IObservable<bool> isInteractive { get; }
    
    private List<IObserver<T>> _observers = new List<IObserver<T>>();
    public ReactiveCommand() : this(Observable.Return(true)) { }
    public ReactiveCommand(IObservable<bool> interactivityFlag) {
        this.isInteractive = interactivityFlag ?? Observable.Return(true);
    }

    public void Trigger(T value) {
        _observers.ForEach(x => x.OnNext(value));
    }

    public IDisposable Subscribe(IObserver<T> observer) {
        _observers.Add(observer);
        return DisposableHelpers.Basic(() => _observers.Remove(observer));
    }

    public void Dispose() {
        _observers.ForEach(x => x.OnCompleted());
        _observers.Clear();
    }
}

public static class ReactiveCommandBindind
{
    public static void Bind<T>(this UnityEngine.UI.Button button, ReactiveCommand<T> command, DisposableCollection disposableCollection, ReactiveProperty<T> valueSource = null) {
        command.isInteractive
            .Subscribe(x => button.interactable = x)
            .AddTo(disposableCollection);

        UnityEngine.Events.UnityAction buttonListener = () => {
            command.Trigger(valueSource == null ? default : valueSource.value);
        };

        button.onClick.AddListener(buttonListener);
        DisposableHelpers
            .Basic(() => button.onClick.RemoveListener(buttonListener))
            .AddTo(disposableCollection);

    }
}