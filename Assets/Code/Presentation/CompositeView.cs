using UnityEngine;

public class CompositeView : MonoBehaviour, IView
{
    public void Bind(IViewModel viewModel, ICompositeDisposable disposable, SubviewBinder binder) {
        foreach (var bind in GetComponents<IBind>()) {
            bind.Bind(viewModel, binder);
        }
    }
}
