using System;
using UnityEngine;

public delegate PlayerViewModel PlayerViewModelFactory();
public class PlayerViewModel : IViewModel
{
    public IObservable<Vector2> position { get; }
  
    public PlayerViewModel(IReadOnlyModel readonlyModel) {
        position = readonlyModel.changed.Select(_ => readonlyModel.playerState.position);
    }

    public void Dispose() {
        
    }
}
