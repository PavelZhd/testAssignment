using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SubviewBinder
{
    private Dictionary<System.Type, GameObject> prefabs = new Dictionary<System.Type, GameObject>();
    public IDisposable Populate(IViewModel viewModel, Transform parent) {
        var disposable = new DisposableCollection();
        var Tt = viewModel.GetType();
        if (!prefabs.TryGetValue(Tt, out GameObject view)) {
            string expectedName = $"Prefabs/{Tt.Name.Replace("ViewModel", "View")}";
            view = Resources.LoadAll<GameObject>(expectedName).FirstOrDefault();
            if (view == null) {
                throw new System.Exception($"Error binding {Tt.Name}. PRefab with expected name '{expectedName}' not found.");
            }
            prefabs.Add(Tt, view);
        }
        var newView = GameObject.Instantiate(view, parent);
        foreach (var addedview in newView.GetComponents<IView>())
            addedview.Bind(viewModel, disposable, this);
        return disposable;
    }
}
