using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class DynamicCollectionView<CollectionViewModel, ItemViewModel> : View<CollectionViewModel>
    where CollectionViewModel : DynamicCollectionViewModel<ItemViewModel>
    where ItemViewModel : IViewModel
{
    [SerializeField] private Transform container;
    [SerializeField] private GameObject template;

    private Stack<GameObject> _stashedObjects = new Stack<GameObject>();
    public Dictionary<IViewModel, IDisposable> _unbinders = new Dictionary<IViewModel, IDisposable>();
    protected override void Bind(CollectionViewModel viewModel, ICompositeDisposable disposable, SubviewBinder subviewBinder) {
        viewModel.modelActivated.Subscribe(viewModel => {
            if (viewModel == null)
                return;
            if (!_stashedObjects.TryPop(out var instance)) {
                instance = GameObject.Instantiate(template, container);
            }
            var unbinder = new DisposableCollection();
            foreach (var viewComponent in instance.GetComponents<IView>()) {
                viewComponent.Bind(viewModel, unbinder, subviewBinder);
            }
            instance.SetActive(true);
            Disposable.Create(() => { 
                instance.gameObject.SetActive(false);
                _stashedObjects.Push(instance);
            }).AddTo(unbinder);
            _unbinders.Add(viewModel, unbinder);

        }).AddTo(disposable);

        viewModel.modelDeactivated.Subscribe(viewModel => {
            if (viewModel == null)
                return; 
            if (_unbinders.TryGetValue(viewModel, out IDisposable unbinder)) {
                unbinder.Dispose();
                _unbinders.Remove(viewModel);
            }
        }).AddTo(disposable);
    }
}
