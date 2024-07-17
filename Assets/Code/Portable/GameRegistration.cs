using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameRegistration
{
    public static void RegisterGame(Scope gameScope) {
        gameScope.RegisterConstructor<GameMaster>(c => 
            new GameMaster(
                    c.Resolve<InitializeGameState>(),
                    c.Resolve<ILevelConfig>(),
                    c.Resolve<AdvanceGameState>(),
                    c.Resolve<PresentGameState>(),
                    c.Resolve<IGameInputSource>()
                )
            );
        gameScope.RegisterConstructor<RestartGame>(c => {
            var master = c.Resolve<GameMaster>();
            return () => { 
                master.Restart(); 
            };
        });
        gameScope.RegisterConstructor<InitializeGameState>(c => {
            return (levelData) => {
                GameState initialState = new GameState();
                initialState.enemies = new EnemyState[50]; //Precache enemy states to use and reuse as needed so we don't need to constantly allocate/free memory;
                for (int i = 0; i < initialState.enemies.Length; i++) {
                    initialState.enemies[i] = new EnemyState() {
                        hpMax = levelData.EnemyHP,
                        hpRemaining = 0,
                    };
                }
                initialState.player = new PlayerState() {
                    hpMax = levelData.PlayerHP,
                    hpRemaining = levelData.PlayerHP,
                    position = levelData.PlayerMovementBounds.center,
                    enemySpawnsRemaining = levelData.NumberOfSpawns, 
                    spawnCD = 3,
                    reloadCD = 3 };
                initialState.projectiles = new ProjectileState[100];
                for (int i = 0; i < initialState.projectiles.Length; i++) {
                    initialState.projectiles[i] = new ProjectileState() {
                        lifetime = 0,
                    };
                }
                return initialState;
            };
        });

        gameScope.RegisterConstructor<AdvanceGameState>(c => {
            var moveEnemies = c.Resolve<MoveEnemies>();
            var moveProjectiles = c.Resolve<MoveProjectiles>();
            var movePlayer = c.Resolve<MovePlayer>();
            var checkCooldowns = c.Resolve<CheckCooldowns>();
            var checkEndgame = c.Resolve <CheckEndgame>();
            return (stateIn, levelData, timeStep, movementInput) => {
                //UnityEngine.Debug.LogWarning($"Updating game state: {stateIn.outcome} {stateIn.player.spawnCD} {timeStep}");
                if (stateIn.outcome == GameOutcome.Defeat || stateIn.outcome == GameOutcome.Victory)
                    return stateIn; // Game already over. No need to modify state;
                var newState = new GameState();
                newState.enemies = new EnemyState[stateIn.enemies.Length];
                for (int i = 0; i < stateIn.enemies.Length; i++) {
                    newState.enemies[i] = stateIn.enemies[i];
                }
                newState.player = stateIn.player;
                newState.projectiles = new ProjectileState[stateIn.projectiles.Length];
                for (int i = 0; i < stateIn.projectiles.Length; i++) {
                    newState.projectiles[i] = stateIn.projectiles[i];
                }

                moveEnemies(newState, timeStep);
                moveProjectiles(newState, timeStep, levelData);
                movePlayer(newState, timeStep, levelData, movementInput);
                checkCooldowns(newState, levelData, timeStep);
                checkEndgame(newState);
                return newState;
            };
        });

        gameScope.RegisterConstructor<MoveEnemies>(c => {
            var hitPlayer = c.Resolve<HitPlayer>();
            return (state, deltaTime) => {
                for (int i = 0; i < state.enemies.Length; i++) {
                    if (state.enemies[i].hpRemaining <= 0)
                        continue; // Enemy inactive. No need to move it;
                    var curEnemy = state.enemies[i];
                    if (curEnemy.position.y <= 0) {
                        hitPlayer(state, i);
                        curEnemy.hpRemaining = -1; //Hit Delivered. No need to keep the enemy alive;
                    }
                    else {
                        curEnemy.position += curEnemy.speed * deltaTime;
                    }
                    state.enemies[i] = curEnemy;
                }
            };
        });
        gameScope.RegisterConstructor<MoveProjectiles>(c=> {
            var detectHit = c.Resolve <DetectHit>();
            var hitEnemy = c.Resolve<HitEnemy>();
            return (state, deltaTime, levelData) => {
                for (int i = 0; i < state.projectiles.Length; i++) {
                    if (state.projectiles[i].lifetime <= 0)
                        continue; //Projectile inactive. Ignore it;
                    var curProjectile = state.projectiles[i];
                    var pos1 = curProjectile.position;
                    var travelTime = Mathf.Min(deltaTime, curProjectile.lifetime);
                    curProjectile.position += curProjectile.velocity * travelTime;
                    var pos2 = curProjectile.position;                    
                    for (int j = 0; j < state.enemies.Length; j++) {
                        if (state.enemies[j].hpRemaining <= 0)
                            continue;
                        if (detectHit(pos1, pos2, state.enemies[j].position, levelData.ProjectileHitWidth)) {
                            hitEnemy(state, j, i, levelData);
                            curProjectile.lifetime = -1; // Projectile hit the enemy. Its role fulfilled;
                            break;
                        }                       
                    }
                    curProjectile.lifetime -= deltaTime;
                    state.projectiles[i] = curProjectile;
                }
            };
        });
        gameScope.RegisterConstructor<MovePlayer>(c => (state, deltaTime, levelData, inputData) => {
            state.player.position += inputData * levelData.PlayerMovementSpeed * deltaTime;
            if (!levelData.PlayerMovementBounds.Contains(state.player.position))
                state.player.position = levelData.PlayerMovementBounds.ClosestPoint(state.player.position);

        });
        gameScope.RegisterConstructor<CheckCooldowns>(c=> {
            var spawnEnemy = c.Resolve<SpawnEnemy>();
            var shootAtEnemy = c.Resolve<ShootAtEnemy>();
            var selectTarget = c.Resolve<SelectTarget>();
            return (state, levelData, deltaTime) => {
                while (state.player.spawnCD <= 0 && state.player.enemySpawnsRemaining > 0) {
                    spawnEnemy(state, levelData, -state.player.spawnCD);
                    state.player.spawnCD += Random.Range(levelData.SpawnCD_min, levelData.SpawnCD_max);
                    state.player.enemySpawnsRemaining--;
                }
                state.player.spawnCD -= deltaTime;

                int targetId = selectTarget(state, levelData);
                
                if (targetId == -1) {// No target in range. No overfire possible. Cap cooldown at 0;
                    if (state.player.reloadCD >= deltaTime) {
                        state.player.reloadCD -= deltaTime;
                    }
                    else {
                        state.player.reloadCD = 0;
                    }
                }
                else {
                    while (state.player.reloadCD <= 0) {
                        shootAtEnemy(state, targetId, levelData, -state.player.reloadCD);
                        state.player.reloadCD += levelData.ShotCD;
                    }
                    state.player.reloadCD -= deltaTime;
                }
            };
        });
        gameScope.RegisterConstructor<CheckEndgame>(c => {
            return (state) => {                
                if (state.player.hpRemaining <=0) {
                    state.outcome = GameOutcome.Defeat;
                    return;
                }
                if (state.player.enemySpawnsRemaining <=0) {
                    bool haveAlive = false;
                    for (int i = 0; i < state.enemies.Length; i++) {
                        if (state.enemies[i].hpRemaining > 0) {
                            haveAlive = true; // Found alive enemy. No chance of Victory;
                            break;
                        }
                    }
                    if (!haveAlive) 
                        state.outcome = GameOutcome.Victory;
                }
            };
        });
        gameScope.RegisterInstance<GameModel>(new GameModel());
        gameScope.RegisterConstructor<PresentGameState>(c=> {
            var model = c.Resolve<GameModel>();
            return model.UpdateState;
        });
        gameScope.RegisterConstructor<IReadOnlyModel>(c => {
            var model = c.Resolve<GameModel>();
            return model;
        });
        gameScope.RegisterConstructor<HitPlayer>(c=> 
        (state, enemyId) => {
            state.player.hpRemaining--;
        });
        gameScope.RegisterConstructor<DetectHit>(c=> 
            (p1, p2, target, width) => {
                var dir = p2 - p1;
                var orth = new Vector2(dir.y, -dir.x).normalized;
                var c1 = p1 - orth * width;
                if (Vector2.Dot(target - c1, orth) < 0)
                    return false;
                var c2 = p2 - orth * width;
                if (Vector2.Dot(target - c2, dir) > 0)
                    return false;
                var c3 = p2 + orth * width;
                if (Vector2.Dot(target - c3, orth) > 0)
                    return false;
                var c4 = p1 + orth * width;
                if (Vector2.Dot(target - c4, dir) < 0)
                    return false;
                return true;
            }
        );
        gameScope.RegisterConstructor<HitEnemy>(c=>
            (state, enemyId, projectileId, levelData) => {
                var enemy = state.enemies[enemyId];
                enemy.hpRemaining -= levelData.ProjectileDamage;
                state.enemies[enemyId] = enemy;
            }
        );
        gameScope.RegisterConstructor<SpawnEnemy>(c =>
            (state, levelData, timeOffset) => {
                int positionFound = -1;
                for (int i = 0; i < state.enemies.Length; i++) {
                    if (state.enemies[i].hpRemaining > 0)
                        continue;
                    positionFound = i;
                    break;                    
                }
                if (positionFound == -1) {
                    var oldArray = state.enemies;
                    state.enemies = new EnemyState[oldArray.Length + 50];
                    for (int i = 0; i < oldArray.Length; i++) {
                        state.enemies[i] = oldArray[i];
                    }
                    positionFound = oldArray.Length;
                    for (int i = oldArray.Length + 1; i < state.enemies.Length; i++) {
                        state.enemies[i] = new EnemyState() {
                            hpMax = levelData.EnemyHP,
                            hpRemaining = 0,
                        };
                    }
                    
                }
                var speed = Vector2.down * Random.Range(levelData.Speed_min, levelData.Speed_max);
                var position = levelData.SpawnPositions[Random.Range(0, levelData.SpawnPositions.Length)] + speed * timeOffset;
                state.enemies[positionFound] = new EnemyState() { 
                    hpMax = levelData.EnemyHP, 
                    hpRemaining = levelData.EnemyHP, 
                    position = position,
                    speed = speed
                };
            }
        );
        gameScope.RegisterConstructor<SelectTarget>(c=>
            (state, levelData) => {
                int closesTargetId = -1;
                float closesDist = float.MaxValue;
                for (int i = 0; i < state.enemies.Length; i++) {
                    if (state.enemies[i].hpRemaining <= 0)
                        continue;
                    var dist = Vector2.SqrMagnitude(state.enemies[i].position - state.player.position);
                    if (closesDist> dist) {
                        closesDist = dist;
                        closesTargetId = i;
                    }
                }
                if (closesDist > levelData.TargetRangeSqr)
                    closesTargetId = -1; //Closes target beyond range. Ignoring it;
                return closesTargetId;
            }
        );
        gameScope.RegisterConstructor<ShootAtEnemy>(c=>
            (GameState state, int enemyId, LevelData levelData, float timeOffset) => {
                var enemy = state.enemies[enemyId];
                var velocity = (enemy.position - state.player.position).normalized * levelData.ProjectileSpeed;
                var position = state.player.position + velocity * timeOffset;

                int projectileIndex = -1;
                for (int i = 0; i < state.projectiles.Length; i++) {
                    if (state.projectiles[i].lifetime > 0)
                        continue;
                    projectileIndex = i;
                    break;
                }
                if (projectileIndex == -1) {
                    //All projectile model slots active. Need more slots;
                    var oldProjectiles = state.projectiles;
                    state.projectiles = new ProjectileState[oldProjectiles.Length + 100];

                    for (int i = 0; i < oldProjectiles.Length; i++) {
                        state.projectiles[i] = oldProjectiles[i];
                    }
                    projectileIndex = oldProjectiles.Length;
                    for (int i = oldProjectiles.Length+1; i < state.projectiles.Length; i++) {
                        state.projectiles[i] = new ProjectileState() {
                            lifetime = 0,
                        };
                    }
                }
                state.projectiles[projectileIndex] = new ProjectileState() {
                    lifetime = levelData.ProjectileLifetime - timeOffset,
                    position = position,
                    velocity = velocity,
                };
            }
        );
    }
}
