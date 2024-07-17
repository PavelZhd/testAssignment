using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope
{
    public Scope(string label) {
        this._Label = label;
    }

    private Dictionary<System.Type, object> instances = new Dictionary<System.Type, object>();
    private Dictionary<System.Type, System.Func<Scope, object>> constructors = new Dictionary<System.Type, System.Func<Scope, object>>();

    private HashSet<System.Type> _inProgressResolves = new HashSet<System.Type>();
    private readonly string _Label;

    public T Resolve<T>() {        
        System.Type Tt = typeof(T);
        if (_inProgressResolves.Contains(Tt)) {
            throw new System.Exception($"({_Label}) Error resolving type {Tt.FullName}. Attempted to call resolve from resolver. Potential circular dependency.");
        }
        _inProgressResolves.Add(Tt);
        object result = ResolveInner(Tt);
        _inProgressResolves.Remove(Tt);
        if (result is null)
            throw new System.Exception($"({_Label}) Error resolving type {Tt.FullName}. Unable to resolve type. Check for missing registration");
        if (result is T typed)
            return typed;
        else
            throw new System.Exception($"({_Label}) Error resolving type {Tt.FullName}. Stored instance was of incorrect type: {result.GetType().FullName}");
    }

    private object ResolveInner(System.Type Tt) {
        if (instances.TryGetValue(Tt, out object result)) {
            return result;
        }
        else if (constructors.TryGetValue(Tt, out System.Func<Scope, object> constructor)) {
            object newInstance = constructor(this);
            instances.Add(Tt, newInstance);
            return newInstance;
        }
        else {
            throw new System.Exception($"({_Label}) Error resolving type {Tt.FullName}. Neither instance, nor constructor registerred. Check for missing registration.");
        }
    }

    public void RegisterInstance<T>(T instance) {
        System.Type Tt = typeof(T);
        if (instances.ContainsKey(Tt) || constructors.ContainsKey(Tt)) {
            throw new System.Exception($"({_Label}) Error registerring type {Tt.FullName}. Detected double registration within scope.");
        }
        instances.Add(Tt, instance);
    }
    public void RegisterConstructor<T>(System.Func<Scope,T> constructor) {
        System.Type Tt = typeof(T);
        if (instances.ContainsKey(Tt) || constructors.ContainsKey(Tt)) {
            throw new System.Exception($"({_Label}) Error registerring type {Tt.FullName}. Detected double registration within scope.");
        }
        constructors.Add(Tt, scope => constructor(scope));
    }
}
