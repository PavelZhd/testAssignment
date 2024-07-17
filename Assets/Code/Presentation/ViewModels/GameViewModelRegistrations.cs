using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameViewModelRegistrations
{
    public static void RegisterViewmodels(Scope viewModelScope) {
        viewModelScope.RegisterConstructor<GameViewModelFactory>(c =>
            ()=>new GameViewModel(
                c.Resolve<GameUIViewModelFactory>(),
                c.Resolve<PlayerViewModelFactory>(),
                c.Resolve<EnemyCollectionViewModelFactory>(),
                c.Resolve<ProjectileCollectionViewModelFactory>())
        );

        viewModelScope.RegisterConstructor<GameUIViewModelFactory>(c =>
            () => new GameUIViewModel(
                c.Resolve<IReadOnlyModel>(),
                c.Resolve<RestartGame>()
            )
        );
        viewModelScope.RegisterConstructor<PlayerViewModelFactory>(c =>
            () => new PlayerViewModel(c.Resolve<IReadOnlyModel>())
        );
        viewModelScope.RegisterConstructor<EnemyCollectionViewModelFactory>(c=>
            ()=>new EnemyCollectionViewModel(
                c.Resolve<IReadOnlyModel>(),
                c.Resolve<EnemyViewModelFactory>())
        );
        viewModelScope.RegisterConstructor<ProjectileCollectionViewModelFactory>(c =>
            () => new ProjectileCollectionViewModel(
                c.Resolve<IReadOnlyModel>(),
                c.Resolve<ProjectileViewModelFactory>())
        );
        viewModelScope.RegisterConstructor<EnemyViewModelFactory>(c=>
            id=>new EnemyViewModel(c.Resolve<IReadOnlyModel>(), id)
        );
        viewModelScope.RegisterConstructor<ProjectileViewModelFactory>(c =>
            id => new ProjectileViewModel(c.Resolve<IReadOnlyModel>(), id)
        );
    }
}
