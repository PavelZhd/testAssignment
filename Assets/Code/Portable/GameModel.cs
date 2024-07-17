using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameModel: IReadOnlyModel
{
    public PlayerState playerState => state.player;
    public EnemyState getEnemy(int i) => state.enemies[i];
    public IEnumerable<int> getActiveEnemies { get {
            for (int i = 0; i < state.enemies.Length; i++) {
                if (state.enemies[i].hpRemaining > 0)
                    yield return i;
            }
        } 
    }
    public int enemyCount => state.enemies.Length;
    public ProjectileState getProjectile(int i) => state.projectiles[i];
    public IEnumerable<int> getActiveProjectiles {
        get {
            for (int i = 0; i < state.projectiles.Length; i++) {
                if (state.projectiles[i].lifetime > 0)
                    yield return i;
            }
        }
    }
    public int projectileCount => state.projectiles.Length;
    private ReactiveProperty<Unit> _changed = new ReactiveProperty<Unit>();
    public IObservable<Unit> changed => _changed;
    public GameOutcome gameOutcome => state.outcome;
    public void UpdateState(GameState state) {
        this.state = state;
        _changed.value = Unit.identity;
    }

    private GameState state;
}

public interface IReadOnlyModel
{
    PlayerState playerState { get; }
    EnemyState getEnemy(int i);
    IEnumerable<int> getActiveEnemies { get; }
    int enemyCount { get; }
    ProjectileState getProjectile(int i);
    IEnumerable<int> getActiveProjectiles { get; }
    int projectileCount { get; }
    GameOutcome gameOutcome { get; }
    IObservable<Unit> changed { get; }
}
