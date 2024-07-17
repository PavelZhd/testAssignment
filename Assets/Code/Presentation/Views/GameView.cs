using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameView : View<GameViewModel>
{
    [SerializeField] private Transform UIroot;
    protected override void Bind(GameViewModel viewModel, ICompositeDisposable disposable, SubviewBinder subviewBinder) {
        subviewBinder.Populate(viewModel.UIviewModel, UIroot).AddTo(disposable);
        subviewBinder.Populate(viewModel.playerViewModel, transform).AddTo(disposable);
        subviewBinder.Populate(viewModel.enemyCollection, transform).AddTo(disposable);
        subviewBinder.Populate(viewModel.projectileCollection, transform).AddTo(disposable);
    }
}
