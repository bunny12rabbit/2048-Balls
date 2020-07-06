using System;
using UnityEngine;

public class CoroutineTicker : MonoBehaviour
{
    public static CoroutineTicker Instance { get; private set; }

    private void Awake()
    {
        InitializeInstance();
    }

    private void InitializeInstance()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}