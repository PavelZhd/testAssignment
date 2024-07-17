using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialDisposable : IDisposable
{
    public IDisposable current {
        get => _current;

        set {
            if (_current != null)
                _current.Dispose();
            _current = value;
        }
    }

    private IDisposable _current;

    public void Dispose() {
        if (_current != null)
            _current.Dispose();
    }
}
