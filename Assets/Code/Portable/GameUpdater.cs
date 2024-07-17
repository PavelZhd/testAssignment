using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void MoveEnemies(GameState state, float deltaTime);
public delegate void MoveProjectiles(GameState state, float deltaTime, LevelData levelData);
public delegate void MovePlayer(GameState state, float deltaTime, LevelData levelData, Vector2 inputData);
public delegate bool DetectHit(Vector2 p1, Vector2 p2, Vector2 target, float hitWidth);
public delegate void HitEnemy(GameState state, int enemyId, int projectileId, LevelData levelData);
public delegate void HitPlayer(GameState state, int enemyId);
public delegate void CheckCooldowns(GameState state, LevelData levelData, float deltaTime);
public delegate void SpawnEnemy(GameState state, LevelData levelData, float timeOffset);
public delegate void ShootAtEnemy(GameState state, int enemyId, LevelData levelData, float timeOffset);
public delegate int SelectTarget(GameState state, LevelData levelData);
public delegate void CheckEndgame(GameState state);