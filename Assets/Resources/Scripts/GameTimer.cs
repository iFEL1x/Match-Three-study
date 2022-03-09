using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [SerializeField] private float timer;

    private void Awake()
    {
        Instance = this;
    }

    public float tTimer
    {
        get { return timer; }
        set { timer = value; }
    }


    private void Update()
    {
        if (timer > 0)
        {
            GUIManager.Instance.Timer = (int)(timer -= Time.deltaTime);
        }
    }
}
