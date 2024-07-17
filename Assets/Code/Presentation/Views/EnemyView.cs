using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : View<EnemyViewModel>
{
    protected override void Bind(EnemyViewModel viewModel, ICompositeDisposable disposable, SubviewBinder subviewBinder) {
        viewModel.position.Subscribe(pos => transform.position = pos).AddTo(disposable);
    }
}
