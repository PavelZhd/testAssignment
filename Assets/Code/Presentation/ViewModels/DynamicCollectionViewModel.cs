using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DynamicCollectionViewModel<ItemViewMode> : IViewModel 
    where ItemViewMode: IViewModel
{
    public IObservable<ItemViewMode> modelActivated => _modelActivated;
    public IObservable<ItemViewMode> modelDeactivated => _modelDeactivated;

    private Subject<ItemViewMode> _modelActivated;
    private Subject<ItemViewMode> _modelDeactivated;

    private DisposableCollection _disposable = new DisposableCollection();
    private HashSet<int> currentlyActive = new HashSet<int>();
    private HashSet<int> temporatySet = new HashSet<int>();
    private Dictionary<int, ItemViewMode> _createdViewModels = new Dictionary<int, ItemViewMode>();
    public DynamicCollectionViewModel(
        IObservable<Unit> changed,
        Func<IEnumerable<int>> activeSelector,
        Func<int, ItemViewMode> itemFactory
        ) {
        _modelActivated = new Subject<ItemViewMode>().AddTo(_disposable);
        _modelDeactivated = new Subject<ItemViewMode>().AddTo(_disposable);

        changed.Subscribe(_=> {
            temporatySet.Clear();
            foreach (int item in currentlyActive)
                temporatySet.Add(item);
            currentlyActive.Clear();
            foreach (int item in activeSelector()) {
                currentlyActive.Add(item);
                if (temporatySet.Contains(item)) {
                    temporatySet.Remove(item);
                }
                else {
                    if (!_createdViewModels.TryGetValue(item, out ItemViewMode model)) {
                        model = itemFactory(item);
                        _createdViewModels.Add(item, model);
                    }
                    _modelActivated.OnNext(model);
                }
            }
            foreach (int item in temporatySet) {
                if (_createdViewModels.TryGetValue(item, out ItemViewMode model)) {
                    _modelDeactivated.OnNext(model);
                }
            }
        }).AddTo(_disposable);
    }

    public void Dispose() {
        _disposable.Dispose();
    }
}
