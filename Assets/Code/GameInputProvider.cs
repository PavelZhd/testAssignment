using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputProvider : MonoBehaviour, IGameInputSource
{
    public Vector2 movementInput { get; private set; }

    public event Action<float> onAdvanceTime;

    // Update is called once per frame
    void Update()
    {
        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        onAdvanceTime?.Invoke(Time.deltaTime);
    }
}
