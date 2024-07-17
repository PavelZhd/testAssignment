using UnityEngine;
using System;

public delegate EnemyViewModel EnemyViewModelFactory(int enemyId);
public class EnemyViewModel : IViewModel
{
    public IObservable<Vector2> position { get; }
    public IObservable<int> hp { get; }
    public IObservable<int> hpMax { get; }

    public EnemyViewModel(
        IReadOnlyModel readOnlyModel,
        int enemyIndex
        )
    {
        position = readOnlyModel.changed.Select(_ => readOnlyModel.getEnemy(enemyIndex)).Where(x => x.hpRemaining > 0).Select(x => x.position);
        hp = readOnlyModel.changed.Select(_ => readOnlyModel.getEnemy(enemyIndex)).Where(x => x.hpRemaining > 0).Select(x => x.hpRemaining).DistinctUntilChanged();
        hpMax = readOnlyModel.changed.Select(_ => readOnlyModel.getEnemy(enemyIndex)).Where(x => x.hpRemaining > 0).Select(x => x.hpMax).DistinctUntilChanged();
    }
    public void Dispose() { }
}
