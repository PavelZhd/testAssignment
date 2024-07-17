using UnityEngine;

public interface IGameInputSource {
    event System.Action<float> onAdvanceTime;
    Vector2 movementInput { get; }
}