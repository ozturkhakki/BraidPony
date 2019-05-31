using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupcakeController : MonoBehaviour
{
    public static CupcakeController Instance { get; private set; }


    public GameObject CupcakePrefab;
    private List<GameObject> Cupcakes, CupcakesNotUsed;
    public float CupcakeMovementSpeed = 1.592f;
    private float[] lastTimeCupcakeSpawned = {0f, 0f, 0f};
    
    public AudioSource audioSource;
    public int spawnRandomness = 100;

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
    
    void Start()
    {
        Cupcakes = new List<GameObject>();
        CupcakesNotUsed = new List<GameObject>();
    }
    
    void FixedUpdate()
    {
        // if health is at zero, dont spawn cupcakes, dont move cupcakes
        if (Player.Instance.Health <= 0) return;

        // spawn cupcake with a probability
        int randomShouldSpawn = Random.Range(0, 100);

        if (randomShouldSpawn < 4)
        {
            List<int> positions = new List<int>() { 0, 1, 2 };

            for (int i = 0; i < 3; i++)
            {
                int randomPos = Random.Range(0, positions.Count - 1);
                if (Time.time > lastTimeCupcakeSpawned[positions[randomPos]] + 0.5f)
                {
                    SpawnCupcake(positions[randomPos]);
                    break;
                }
                else
                {
                    positions.RemoveAt(randomPos);
                }
            }
        }

        // move existing cupcakes
        for (int i = Cupcakes.Count - 1; i >= 0; i--)
        {
            // move cupcake
            Cupcakes[i].transform.position -= new Vector3(0f, CupcakeMovementSpeed / 50f, 0f);

            // check if passed by the limit
            if (Cupcakes[i].transform.position.y < -5.3f)
            {
                GameObject temp = Cupcakes[i];
                CupcakesNotUsed.Add(temp);
                temp.SetActive(false);
                Cupcakes.RemoveAt(i);

                Player.Instance.CupcakeMissed();
            }
        }
    }

    void Update()
    {
        if (Player.Instance.Health <= 0) return;

        for (int i = Cupcakes.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < 5; j++)
            {
                if (PlayerInput.Instance.PTouch[j].isPlayerTouching == false)
                {
                    break; // Since there wont be any more touches
                }

                BoxCollider2D collider = Cupcakes[i].GetComponent<BoxCollider2D>();
                if (collider.OverlapPoint(PlayerInput.Instance.PTouch[j].touchWorldPositionBegin))
                {
                    GameObject temp = Cupcakes[i];
                    Player.Instance.CupcakeCatched(temp.transform.position.y);

                    CupcakesNotUsed.Add(temp);
                    temp.SetActive(false);
                    Cupcakes.RemoveAt(i);
                }
            }
        }
    }

    public void SpawnCupcake(int at)
    {
        GameObject temp;
        if (CupcakesNotUsed.Count == 0)
        {
            temp = Instantiate(CupcakePrefab);
        }
        else
        {
            temp = CupcakesNotUsed[CupcakesNotUsed.Count - 1];
            CupcakesNotUsed.RemoveAt(CupcakesNotUsed.Count - 1);
        }

        temp.transform.position = new Vector3(
            (at - 1) * 1.5f,
            transform.position.y,
            -1
        );

        temp.SetActive(true);
        Cupcakes.Add(temp);
        lastTimeCupcakeSpawned[at] = Time.time;
    }

    public void ResetCupcakes()
    {
        if (Cupcakes != null)
        {
            foreach (GameObject obj in Cupcakes)
            {
                Destroy(obj);
            }

            Cupcakes.Clear();
        }
        if (CupcakesNotUsed != null)
        {
            foreach (GameObject obj in CupcakesNotUsed)
            {
                Destroy(obj);
            }

            CupcakesNotUsed.Clear();
        }
    }
}
