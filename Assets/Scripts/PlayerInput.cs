using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }

    public struct PlayerTouch
    {
        public bool isPlayerTouching;
        public Vector2 touchWorldPosition;
        public Vector2 touchWorldPositionBegin;
    }


    private Camera mainCamera;
    public PlayerTouch[] PTouch;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        PTouch = new PlayerTouch[5];
        for (int i = 0; i < 5; ++i)
        {
            PTouch[i] = new PlayerTouch();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            for (int i = 0; i < 5; i++)
            {
                PTouch[i].isPlayerTouching = Input.touchCount > i;
                if (PTouch[i].isPlayerTouching)
                {
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                    {
                        PTouch[i].touchWorldPositionBegin = mainCamera.ScreenToWorldPoint(Input.GetTouch(i).position);
                    }

                    PTouch[i].touchWorldPosition = mainCamera.ScreenToWorldPoint(Input.GetTouch(i).position);
                }
            }
        }
        else
        {
            PTouch[0].isPlayerTouching = Input.GetMouseButton(0);
            if (PTouch[0].isPlayerTouching)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    PTouch[0].touchWorldPositionBegin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                }

                PTouch[0].touchWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            }
        }
    }
}