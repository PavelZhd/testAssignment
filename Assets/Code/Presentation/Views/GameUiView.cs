using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUiView : View<GameUIViewModel>
{
    [SerializeField] private TMPro.TMP_Text enemyCounter;
    [SerializeField] private TMPro.TMP_Text HPCounter;

    [SerializeField] private GameObject loseDialog;
    [SerializeField] private GameObject winDialog;
    [SerializeField] private UnityEngine.UI.Button[] restartButtons;

    protected override void Bind(GameUIViewModel viewModel, ICompositeDisposable disposable, SubviewBinder subviewBinder) {
        viewModel.EnemiesRemaining.Subscribe(x=> enemyCounter.text = x.ToString()).AddTo(disposable);
        viewModel.PlayerHpRemaining.Subscribe(x => HPCounter.text = x.ToString()).AddTo(disposable);

        viewModel.IsDefeatVisible.Subscribe(x=> loseDialog.SetActive(x)).AddTo(disposable);
        viewModel.IsVictoryVisible.Subscribe(x => winDialog.SetActive(x)).AddTo(disposable);

        void callRestart() {
            viewModel.restartComand.Trigger(Unit.identity);
        }
        foreach (var button in restartButtons) {
            button.onClick.AddListener(callRestart);
            Disposable.Create(() => button.onClick.RemoveListener(callRestart)).AddTo(disposable);
        }
    }
}
