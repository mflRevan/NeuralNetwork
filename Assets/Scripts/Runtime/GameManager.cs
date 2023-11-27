using System.Collections;
using System.Collections.Generic;
using Default;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private LayerMask wallMask;
    [SerializeField] private NeuralNetworkData data;

    public LayerMask WallMask => wallMask;


    private void Awake()
    {
        Instance = this;
    }
}
