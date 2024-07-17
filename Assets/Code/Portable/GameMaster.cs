using System.Collections;
using System.Collections.Generic;

public class GameMaster: System.IDisposable
{
    private GameState _currentState;
    private LevelData _levelData;
    private readonly InitializeGameState _initializer;
    private readonly IGameInputSource _inputSource;
    private readonly ILevelConfig _configSource;
    private readonly AdvanceGameState _advancer;
    private readonly PresentGameState _presenter;
    public GameMaster(
        InitializeGameState initializer,
        ILevelConfig configSource,
        AdvanceGameState advancer,
        PresentGameState presenter,
        IGameInputSource inputSource
        ) {
        _initializer = initializer;
        _inputSource = inputSource;
        _configSource = configSource;
        _advancer = advancer;
        _presenter = presenter;
        Restart();
        inputSource.onAdvanceTime += AdvanceAndRePresent;
        
    }

    public void Restart() {
        _levelData = _configSource.getLevelData();
        _currentState = _initializer(_levelData);
        _presenter(_currentState);
    }

    void AdvanceAndRePresent(float deltaTime) {
        _currentState = _advancer(_currentState, _levelData, deltaTime, _inputSource.movementInput);
        _presenter(_currentState);
    }

    public void Dispose() {
        _inputSource.onAdvanceTime -= AdvanceAndRePresent;
    }
}

public delegate void RestartGame();