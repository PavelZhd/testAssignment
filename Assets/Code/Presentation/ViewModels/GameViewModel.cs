using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate GameViewModel GameViewModelFactory();
public class GameViewModel: IViewModel
{
    public GameUIViewModel UIviewModel { get; }
    public PlayerViewModel playerViewModel { get; }
    public EnemyCollectionViewModel enemyCollection { get; }
    public ProjectileCollectionViewModel projectileCollection { get; }
    public GameViewModel(
        GameUIViewModelFactory uIViewModelFactory,
        PlayerViewModelFactory playerViewModelFactory,
        EnemyCollectionViewModelFactory enemyCollectionViewModelFactory,
        ProjectileCollectionViewModelFactory projectileCollectionViewModelFactory

        ) {
        UIviewModel = uIViewModelFactory();
        playerViewModel = playerViewModelFactory();
        enemyCollection = enemyCollectionViewModelFactory();
        projectileCollection = projectileCollectionViewModelFactory();
    }

    public void Dispose() {
        UIviewModel.Dispose();
        playerViewModel.Dispose();
        enemyCollection.Dispose();
        projectileCollection.Dispose();
    }
}

