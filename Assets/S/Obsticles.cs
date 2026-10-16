using System.Collections.Generic;
using UnityEngine;

public class Obsticles : MonoBehaviour
{
    public static readonly List<Obsticles> ActiveObstacles = new List<Obsticles>();
    private static int nextEntityId = 1;

    public float minSize = 0.5f;
    public float maxSize = 2.0f;

    public float minSpeed = 50.0f;
    public float maxSpeed = 150.0f;

    public float MaxSpinSpeed = 10f;

    [SerializeField] private AudioClip obstacleCollisionSound;

    private Rigidbody2D rg;
    private int entityId;

    private void Awake()
    {
        entityId = nextEntityId++;
    }

    private void OnEnable()
    {
        if (!ActiveObstacles.Contains(this))
        {
            ActiveObstacles.Add(this);
        }
    }

    private void OnDisable()
    {
        ActiveObstacles.Remove(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       float randomSize = Random.Range(minSize, maxSize);
       transform.localScale = new Vector3(randomSize, randomSize, 1);

       float randomSpeed = Random.Range(minSpeed, maxSpeed);
       Vector2 randomDirection = Random.insideUnitCircle.normalized;

       rg = GetComponent<Rigidbody2D>();
        rg.AddForce(randomDirection * randomSpeed);



        float randomTorque = Random.Range(-MaxSpinSpeed, MaxSpinSpeed);
        rg.AddTorque(randomTorque);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Obsticles"))
        {
            return;
        }

        Obsticles otherObstacle = collision.gameObject.GetComponent<Obsticles>();
        if (otherObstacle == null)
        {
            return;
        }

        if (entityId > otherObstacle.entityId)
        {
            return;
        }

        if (GameAudio.Instance != null)
        {
            GameAudio.Instance.PlaySfx(obstacleCollisionSound, transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
