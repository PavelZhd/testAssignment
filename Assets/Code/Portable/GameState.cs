using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public EnemyState[] enemies;
    public PlayerState player;
    public ProjectileState[] projectiles;
    public GameOutcome outcome;
}

public enum GameOutcome { inProgress, Victory, Defeat }

public delegate GameState InitializeGameState(LevelData levelData);
public delegate GameState AdvanceGameState(GameState inpu, LevelData levelData, float timeStep, Vector2 movementInput);
public delegate void PresentGameState(GameState input);

public struct EnemyState
{
    public int hpRemaining;
    public int hpMax;
    public Vector2 position;
    public Vector2 speed;
}

public struct PlayerState
{
    public int hpRemaining;
    public int enemySpawnsRemaining;
    public int hpMax;
    public Vector2 position;
    public float reloadCD;
    public float spawnCD;
}
public struct ProjectileState
{
    public float lifetime;
    public Vector2 position;
    public Vector2 velocity;
}
