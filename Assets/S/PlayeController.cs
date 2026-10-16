using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayeController : MonoBehaviour
{
    public float thrustForce = 1f;
    Rigidbody2D rb;

    public float maxspeed = 5f;

    public GameObject bosster;
    public GameObject explosion;

    private float elaspedTime = 0f;

    public float scoreMultiplier = 1f;
    private float score = 0f;
    private float highScore = 0f;

    private const string HighScoreKey = "HighScore";

    [SerializeField] private PanelRenderer panel;
    [SerializeField] private VisualElementReference<Label> LableReference;
    [SerializeField] private VisualElementReference<Label> HighScoreLableReference;
    [SerializeField] private VisualElementReference<Button> RestartButtonReference;

    [SerializeField] private AudioClip boundaryHitSound;
    [SerializeField] private AudioClip playerDestroyedSound;
    [SerializeField] private AudioClip proximityBeepSound;
    [SerializeField] private string boundaryTag = "Boundary";
    [SerializeField] private float warningDistance = 4f;
    [SerializeField] private float warningBeepInterval = 0.4f;
    [SerializeField] [Range(0f, 1f)] private float proximityBeepVolume = 0.5f;

    private Label scoreText;
    private Label highScoreText;
    private Button restartButton;
    private float nextBeepTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        highScore = PlayerPrefs.GetFloat(HighScoreKey, 0f);
    }

    // Update is called once per frame
    void Update()
    {



        elaspedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elaspedTime * scoreMultiplier);


        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }

        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString("0");
        }

        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore.ToString("0");
        }

        if (Mouse.current.leftButton.isPressed)
        {
            //  Vector3 mousePos =Camera.main.ScreenToWorldPoint(Mouse.current.position.Value());
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 direction = (mousePos - transform.position).normalized;
            transform.up = direction;
            rb.AddForce(direction * thrustForce);
        }

        if (Mouse.current.leftButton.isPressed)
        {
            bosster.SetActive(true);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            bosster.SetActive(false);
        }

        UpdateProximityBeep();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(boundaryTag))
        {
            if (GameAudio.Instance != null)
            {
                GameAudio.Instance.PlaySfx(boundaryHitSound, transform.position);
            }

            return;
        }

        if (collision.gameObject.CompareTag("Obsticles"))
        {
            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetFloat(HighScoreKey, highScore);
                PlayerPrefs.Save();
            }

            if (GameAudio.Instance != null)
            {
                GameAudio.Instance.PlaySfx(playerDestroyedSound, transform.position);
            }

            if (explosion != null)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    private void UpdateProximityBeep()
    {
        if (Time.time < nextBeepTime)
        {
            return;
        }

        float nearestDistance = GetNearestObstacleDistance();

        if (nearestDistance > warningDistance)
        {
            return;
        }

        float volume = Mathf.Clamp01(1f - (nearestDistance / warningDistance));

        if (GameAudio.Instance != null)
        {
            GameAudio.Instance.PlaySfx(proximityBeepSound, transform.position, volume * proximityBeepVolume);
        }

        nextBeepTime = Time.time + warningBeepInterval;
    }

    private float GetNearestObstacleDistance()
    {
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < Obsticles.ActiveObstacles.Count; i++)
        {
            Obsticles obstacle = Obsticles.ActiveObstacles[i];

            if (obstacle == null)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, obstacle.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
            }
        }

        return nearestDistance;
    }

    private static void RestartGame()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Awake()
    {
        panel.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnDestroy()
    {
        panel.UnregisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer panel, VisualElement root, int version)
    {
        LableReference.RegisterReferenceResolvedCallback(HandleLableReferenceCallback);
        HighScoreLableReference.RegisterReferenceResolvedCallback(HandleHighScoreLableReferenceCallback);
        RestartButtonReference.RegisterReferenceResolvedCallback(HandleRestartButtonReferenceCallback);
    }

    private void HandleLableReferenceCallback(Label label)
    {
        scoreText = label;
    }

    private void HandleHighScoreLableReferenceCallback(Label label)
    {
        highScoreText = label;
    }

    private void HandleRestartButtonReferenceCallback(Button button)
    {
        if (restartButton != null)
        {
            restartButton.clicked -= RestartGame;
        }

        restartButton = button;
        restartButton.clicked += RestartGame;
    }
}
