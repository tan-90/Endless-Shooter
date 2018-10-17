using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUi : MonoBehaviour {

    public Image FadeQuad;
    public RectTransform WaveBanner;
    public Text WaveTitle;
    public Text WaveEnemyCount;
    public GameObject GameOverUI;

    Spawner SceneSpawner;

    void Awake()
    {
        SceneSpawner = FindObjectOfType<Spawner>();
        SceneSpawner.OnNewWave += OnNewWave;
    }

	// Use this for initialization
	void Start () {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        GameOverUI.SetActive(true);
    }

    void OnNewWave(int WaveNumber)
    {
        string[] Numbers = { "Infinite", "One", "Two", "Tree", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen", "Twenty" };
        WaveTitle.text = "- Wave " + Numbers[WaveNumber] + " -";
        WaveEnemyCount.text = "Enemies: " + ((SceneSpawner.Waves[WaveNumber - 1].EnemyCount == -1) ? "Infinite" : SceneSpawner.Waves[WaveNumber - 1].EnemyCount.ToString());

        StopCoroutine("AnimateWaveBanner");
        StartCoroutine("AnimateWaveBanner");
    }

    IEnumerator AnimateWaveBanner()
    {
        // -470 30
        float Percentage = 0;
        float Speed = 3f;
        float Delay = 1.5f;
        int Direction = 1;

        float EndDelayTime = Time.time + 1 / Speed + Delay;

        while(Percentage >= 0)
        {
            Percentage += Time.deltaTime * Speed * Direction;
            if(Percentage >= 1)
            {
                Percentage = 1;
                if(Time.time > EndDelayTime)
                {
                    Direction = -1;
                }
            }

            WaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-470, 200, Percentage);
            yield return null;
        }


    }

    IEnumerator Fade(Color From, Color To, float time)
    {

        float Speed = 1 / time;
        float Percentage = 0;
        while(Percentage < 1)
        {
            Percentage += Time.deltaTime * Speed;
            FadeQuad.color = Color.Lerp(From, To, Percentage);
            yield return null;
        }
    }

    public void StartNewGame()
    {
        Application.LoadLevel("Game");
    }
}
