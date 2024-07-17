using System.Collections;
using System;
using UnityEngine;

public static class Disposable 
{
    public static IDisposable Create(Action onDispose) {
        return new DummyDisposable(onDispose);
    }
    private class DummyDisposable : IDisposable
    {
        

        private bool isDisposed;
        private Action onDispose;
        public DummyDisposable(Action onDispose) {
            this.onDispose = onDispose;
            this.isDisposed = false;
        }

        public void Dispose() {
            if (!isDisposed) {
                isDisposed = true;
                onDispose?.Invoke();                
            }
        }
    }
}
