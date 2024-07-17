using System;
using System.Linq;

public delegate GameUIViewModel GameUIViewModelFactory();
public class GameUIViewModel : IViewModel
{
    
    public IObservable<int> EnemiesRemaining { get; }
    public IObservable<int> PlayerHpRemaining { get;  }
    public IObservable<bool> IsDefeatVisible { get; }
    public IObservable<bool> IsVictoryVisible { get; }
    public IReactiveCommand<Unit> restartComand { get; }
    private DisposableCollection _disposable;
    public GameUIViewModel(IReadOnlyModel readOnlyModel, RestartGame restart) {
        EnemiesRemaining = readOnlyModel.changed.Select(_ => readOnlyModel.getActiveEnemies.Count() + readOnlyModel.playerState.enemySpawnsRemaining);
        PlayerHpRemaining = readOnlyModel.changed.Select(_ => readOnlyModel.playerState.hpRemaining).DistinctUntilChanged();
        IsDefeatVisible = readOnlyModel.changed.Select(_ => readOnlyModel.gameOutcome == GameOutcome.Defeat);
        IsVictoryVisible = readOnlyModel.changed.Select(_ => readOnlyModel.gameOutcome == GameOutcome.Victory);
        _disposable = new DisposableCollection();
        restartComand = new ReactiveCommand<Unit>().AddTo(_disposable);
        restartComand.Subscribe(_ => restart()).AddTo(_disposable);
    }

    public void Dispose() { _disposable.Dispose(); }
}
