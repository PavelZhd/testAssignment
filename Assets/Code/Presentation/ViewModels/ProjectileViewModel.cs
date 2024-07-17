using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate ProjectileViewModel ProjectileViewModelFactory(int projectileId);
public class ProjectileViewModel : IViewModel
{
    public IObservable<Vector2> position { get; }
    public IObservable<Vector2> velocity { get; }

    public ProjectileViewModel (IReadOnlyModel readOnlyModel, int projectileId) {
        position = readOnlyModel.changed.Select(_ => readOnlyModel.getProjectile(projectileId).position).DistinctUntilChanged();
        velocity = readOnlyModel.changed.Select(_ => readOnlyModel.getProjectile(projectileId).velocity).DistinctUntilChanged();
    }

    public void Dispose() { }
}
