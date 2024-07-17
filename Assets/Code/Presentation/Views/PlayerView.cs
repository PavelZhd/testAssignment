using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : View<PlayerViewModel>
{
    protected override void Bind(PlayerViewModel viewModel, ICompositeDisposable disposable, SubviewBinder subviewBinder) {
        viewModel.position.Subscribe(pos => transform.position = pos).AddTo(disposable);
    }
}
