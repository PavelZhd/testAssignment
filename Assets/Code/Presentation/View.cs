using UnityEngine;

public abstract class View<ViewModel>: MonoBehaviour, IView where ViewModel: IViewModel
{
    protected abstract void Bind(ViewModel viewModel, ICompositeDisposable disposable, SubviewBinder subviewBinder);
    public void Bind(IViewModel viewModel, ICompositeDisposable disposable, SubviewBinder subviewBinder) {
        if (viewModel is ViewModel typedVM) {
            Bind(typedVM, disposable, subviewBinder);
        }
    }
}
