using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelConfig : MonoBehaviour, ILevelConfig
{
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Camera camera;
    [SerializeField] [Range(1, 999)] int EnemyCountMin = 20;
    [SerializeField] [Range(1, 999)] int EnemyCountMax = 50;
    [SerializeField] [Range(0.001f, 2f)] float SpawnCDMin = 0.25f;
    [SerializeField] [Range(0.001f, 2f)] float SpawnCDMax = 0.5f;
    [SerializeField] [Range(0.001f, 9f)] float Speed_min = 1f;
    [SerializeField] [Range(0.001f, 9f)] float Speed_max = 5f;
    [SerializeField] [Range(1, 99)] int EnemyHP = 4;
    [SerializeField] [Range(0.001f, 9f)] float TargetRange = 5f;
    [SerializeField] [Range(0.001f, 99f)] float ShotPerSecond = 5f;
    [SerializeField] [Range(1, 99)] int ProjectileDamage = 1;
    [SerializeField] [Range(0.001f, 99f)] float ProjectileSpeed = 3;

    [SerializeField] [Range(1, 999)] int PlayerHP = 10;
    [SerializeField] [Range(0.001f, 99f)] float PlayerMoveSpeed = 2;
    [SerializeField] [Range(0.001f, 0.5f)] float ProjectileHitWidth = 0.1f;

    public LevelData getLevelData() {
        Vector3 LowLeftCorner = camera.ViewportToWorldPoint(Vector3.zero);
        Vector3 LowRight = camera.ViewportToWorldPoint(Vector3.right);
        Vector3 Center = new Vector3((LowLeftCorner.x+LowRight.x)/2, LowLeftCorner.y / 2, 0);
        Vector3 Extents = (Center - LowLeftCorner) * 2 - new Vector3(0.5f,0.5f,0);
        return new LevelData(
            spawnPoints.Select(x => (Vector2)x.position).ToArray(),
            Random.Range(EnemyCountMin, EnemyCountMax),
            SpawnCDMin,
            SpawnCDMax,
            Speed_min,
            Speed_max, 
            EnemyHP, 
            TargetRange, 
            ShotPerSecond, 
            ProjectileDamage,
            ProjectileSpeed,
            PlayerHP,
            PlayerMoveSpeed,
            new Bounds(Center, Extents),
            ProjectileHitWidth
            );
    }

    private void OnValidate() {
        if (EnemyCountMax < EnemyCountMin)
            EnemyCountMax = EnemyCountMin;
        if (SpawnCDMax < SpawnCDMin)
            SpawnCDMax = SpawnCDMin;
    }
}

public interface ILevelConfig
{
    LevelData getLevelData();
}

public struct LevelData
{
    public readonly Vector2[] SpawnPositions;
    public readonly int NumberOfSpawns;
    public readonly float SpawnCD_min;
    public readonly float SpawnCD_max;
    public readonly float Speed_min;
    public readonly float Speed_max;
    public readonly int EnemyHP;
    public readonly float TargetRangeSqr;
    public readonly float ShotCD;
    public readonly int ProjectileDamage;
    public readonly float ProjectileSpeed;

    public readonly int PlayerHP;
    public readonly float PlayerMovementSpeed;
    public readonly Bounds PlayerMovementBounds;
    public readonly float ProjectileHitWidth;
    public readonly float ProjectileLifetime;

    public LevelData(
        Vector2[] spawnPositions,
        int numberOfSpawns,
        float spawnCD_min,
        float spawnCD_max, 
        float speed_min, 
        float speed_max, 
        int enemyHP, 
        float targetRange, 
        float shotPerSecond, 
        int projectileDamage, 
        float projectileSpeed,
        int playerHP,
        float playerMovementSpeed,
        Bounds playerMovementBounds,
        float projectileHitWidth)
    {
        this.SpawnPositions = spawnPositions;
        this.NumberOfSpawns = numberOfSpawns;
        this.SpawnCD_min = spawnCD_min;
        this.SpawnCD_max = spawnCD_max;
        this.Speed_min = speed_min;
        this.Speed_max = speed_max;
        this.EnemyHP = enemyHP;
        this.TargetRangeSqr = targetRange * targetRange;
        this.ShotCD = 1 / shotPerSecond;
        this.ProjectileDamage = projectileDamage;
        this.ProjectileSpeed = projectileSpeed;
        this.PlayerHP = playerHP;
        this.PlayerMovementSpeed = playerMovementSpeed;
        this.PlayerMovementBounds = playerMovementBounds;
        this.ProjectileHitWidth = projectileHitWidth;
        this.ProjectileLifetime = targetRange / projectileSpeed;
    }
}
