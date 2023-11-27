using System;
using UnityEngine;

public class TickSystem : MonoBehaviour
{
    public static Action OnTick;
    public static int Tick { get; private set; }

    private float tickTimer;

    private static float tickTimerMax;


    private void Start()
    {
        Tick = 0;
        SetTickTimerMax(0.05f);
    }

    public static void SetTickTimerMax(float timerMax)
    {
        tickTimerMax = timerMax;
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickTimerMax)
        {
            tickTimer -= tickTimerMax;
            Tick++;
            OnTick?.Invoke();
        }
    }
}
