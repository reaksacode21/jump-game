//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerScript : MonoBehaviour
//{
//    public Rigidbody2D playerBody;

//    public float moveSpeed = 2f;
//    public float normalPush = 10f;
//    public float extraPush = 14f;

//    private bool initialPush = false;
//    private int pushCount;
//    private bool playerDied = false;

//    void Awake()
//    {
//        playerBody = GetComponent<Rigidbody2D>();

//    }

//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void FixedUpdate()
//    {
//        Move();
//    }

//    void Move()
//    {
//        if (playerDied)
//            return;

//        if(Input.GetAxisRaw("Horizontal") > 0)
//        {
//            playerBody.velocity = new Vector2(moveSpeed, playerBody.velocity.y);
//        } else if (Input.GetAxisRaw("Horizontal") < 0)
//        {
//            playerBody.velocity = new Vector2(-moveSpeed, playerBody.velocity.y);
//        }

//    } // player movement

//    void OnTriggerEnter2D(Collider2D target)
//    {
//        if (playerDied)
//            return;

//        if (target.tag == "ExtraPush")
//        {
//            if (!initialPush)
//            {
//                initialPush = true;

//                playerBody.velocity = new Vector2(playerBody.velocity.x, 18f);

//                target.gameObject.SetActive(false);

//                SoundManager.instance.JumpSoundFX();

//                // Exit on trigger enter because of initial push
//                return;

//            }
//        }

//        if(target.tag == "NormalPush")
//        {
//            playerBody.velocity = new Vector2(playerBody.velocity.x, normalPush);

//            target.gameObject.SetActive(false);

//            pushCount++;

//            SoundManager.instance.JumpSoundFX();

//        }

//        if(target.tag == "ExtraPush")
//        {
//            playerBody.velocity = new Vector2(playerBody.velocity.x, extraPush);

//            target.gameObject.SetActive(false);

//            pushCount++;

//            SoundManager.instance.JumpSoundFX();

//        }

//        if(pushCount == 2)
//        {
//            pushCount = 0;
//            PlatformSpawner.instance.SpawnPlatforms();
//        }

//        if(target.tag == "FallDown" || target.tag == "Bird")
//        {
//            playerDied = true;

//            SoundManager.instance.GameOverSoundFX();

//            GameManager.instance.RestartGame();

//        }

//    } // On trigger enter

//} // Class

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D[] playerBodies;
     Text scoreText;
     Text highScoreText;
    public Button nextButton;
    public Button backButton;

    public float moveSpeed = 2f;
    public float normalPush = 10f;
    public float extraPush = 14f;

    private int activePlayerIndex = 0;
    private bool initialPush = false;
    private int pushCount;
    private bool playerDied = false;
    private int score = 0;
    private int highScore = 1;

    void Awake()
    {
        playerBodies = new Rigidbody2D[4];
        for (int i = 0; i < playerBodies.Length; i++)
            playerBodies[i] = transform.GetChild(i).GetComponent<Rigidbody2D>();

        nextButton.onClick.AddListener(NextPlayer);
        backButton.onClick.AddListener(PreviousPlayer);
    }

    void Start()
    {
        UpdateScoreText();
        LoadHighScore();
        SetActivePlayer(activePlayerIndex);
    }

    void Update()
    {
        if (playerDied)
            return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.position.x < Screen.width / 2)
            {
                playerBodies[activePlayerIndex].velocity = new Vector2(-moveSpeed, playerBodies[activePlayerIndex].velocity.y);
            }
            else if (touch.position.x > Screen.width / 2)
            {
                playerBodies[activePlayerIndex].velocity = new Vector2(moveSpeed, playerBodies[activePlayerIndex].velocity.y);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        if (playerDied)
            return;

        if (target.tag == "ExtraPush")
        {
            if (!initialPush)
            {
                initialPush = true;
                playerBodies[activePlayerIndex].velocity = new Vector2(playerBodies[activePlayerIndex].velocity.x, 18f);
                target.gameObject.SetActive(false);
                return;
            }
        }

        if (target.tag == "NormalPush" || target.tag == "ExtraPush")
        {
            float pushForce = target.tag == "NormalPush" ? normalPush : extraPush;
            playerBodies[activePlayerIndex].velocity = new Vector2(playerBodies[activePlayerIndex].velocity.x, pushForce);
            target.gameObject.SetActive(false);
            pushCount++;
        }

        if (target.tag == "coin")
        {
            target.gameObject.SetActive(false);
        }

        if (pushCount == 2)
        {
            pushCount = 0;
            PlatformSpawner.instance.SpawnPlatforms();
        }

        if (target.tag == "FallDown" || target.tag == "Bird")
        {
            playerDied = true;
            SaveHighScore();
            GameManager.instance.RestartGame();
        }

        UpdateScore();
        UpdateScoreText();
    }

    void UpdateScore()
    {
        score++;
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore;
    }

    void SaveHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            highScoreText.text = "High Score: " + highScore;
        }
    }

   public  void NextPlayer()
    {
        activePlayerIndex++;
        if (activePlayerIndex >= playerBodies.Length)
            activePlayerIndex = 0;

        SetActivePlayer(activePlayerIndex);
    }

    public void PreviousPlayer()
    {
        activePlayerIndex--;
        if (activePlayerIndex < 0)
            activePlayerIndex = playerBodies.Length - 1;

        SetActivePlayer(activePlayerIndex);
    }

    void SetActivePlayer(int index)
    {
        for (int i = 0; i < playerBodies.Length; i++)
        {
            if (i == index)
                playerBodies[i].gameObject.SetActive(true);
            else
                playerBodies[i].gameObject.SetActive(false);
        }
    }
}




























