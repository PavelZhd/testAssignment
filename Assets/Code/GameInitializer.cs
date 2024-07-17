using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] GameInputProvider gameInput;
    [SerializeField] LevelConfig configSource;
    [SerializeField] Transform rootView;
    void Start()
    {
        var scope = new Scope("Game");
        var subviewBinder = new SubviewBinder();
        scope.RegisterInstance<ILevelConfig>(configSource);
        scope.RegisterInstance<IGameInputSource>(gameInput);
        configSource.gameObject.SetActive(false);
        GameRegistration.RegisterGame(scope);
        var viewModelScope = new Scope("ViewModels");
        viewModelScope.RegisterInstance<IReadOnlyModel>(scope.Resolve<IReadOnlyModel>());
        viewModelScope.RegisterInstance<RestartGame>(scope.Resolve<RestartGame>());
        GameViewModelRegistrations.RegisterViewmodels(viewModelScope);
        subviewBinder.Populate(viewModelScope.Resolve<GameViewModelFactory>()(), rootView);
    }

}
