using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public bool DevMode;

    public Wave[] Waves;
    public Enemy Enemy;

    LivingEntity PlayerEntity;
    Transform PlayerTransform;

    Wave CurrentWave;
    int CurrentWaveNumber;

    MapGenerator MapGen;

    int RemainingEnemies;
    int RemainingAliveEnemies;
    float NextSpawnTime;

    float CampingCheckRate = 2;
    float NextCampCheckTime;
    float CampThresholdDistance = 1.5f;
    Vector3 LastCampPosition;
    bool IsCamping;

    public event System.Action<int> OnNewWave;

    bool Active;

	// Use this for initialization
	void Start () {

        Active = true;
        MapGen = FindObjectOfType<MapGenerator>();
        PlayerEntity = FindObjectOfType<Player>();
        PlayerTransform = PlayerEntity.transform;
        NextCampCheckTime = CampingCheckRate + Time.time;
        LastCampPosition = PlayerTransform.position;
        NextWave();
        PlayerEntity.OnDeath += OnPlayerDeath;
	}

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            if (Time.time > NextCampCheckTime)
            {
                NextCampCheckTime = Time.time + CampingCheckRate;

                IsCamping = Vector3.Distance(PlayerTransform.position, LastCampPosition) < CampThresholdDistance;
                LastCampPosition = PlayerTransform.position;
            }

            if ((RemainingEnemies > 0 || CurrentWave.Infinite) && Time.time > NextSpawnTime)
            {
                RemainingEnemies--;
                NextSpawnTime = Time.time + CurrentWave.TimeBetweenSpawns;

                StartCoroutine("SpawnEnemy");
            }
        }

        if(DevMode)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                foreach(Enemy CurrentEnemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(CurrentEnemy.gameObject);
                }
                NextWave();
            }
        }
    }
    IEnumerator SpawnEnemy()
    {

        Transform SpawnTile = MapGen.GetRandomOpenTile();
        if(IsCamping)
        {
            SpawnTile = MapGen.GetTileFromPosition(PlayerTransform.position);
        }
        float SpawnDelay = 1;
        float TileFlashSpeed = 4;
        Material TileMaterial = SpawnTile.GetComponent<Renderer>().material;
        Color OriginalTileColour = Color.white;
        Color FlashColour = Color.red;
        float SpawnTimer = 0;

        while(SpawnTimer < SpawnDelay)
        {
            TileMaterial.color = Color.Lerp(OriginalTileColour, FlashColour, Mathf.PingPong(SpawnTimer * TileFlashSpeed, 1));
            SpawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy SpawnedEnemy = Instantiate(Enemy, SpawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        SpawnedEnemy.OnDeath += OnEnemyDeath;
        SpawnedEnemy.SetProperties(CurrentWave.MoveSpeed, CurrentWave.HitsToKillPlayer, CurrentWave.EnemyHealth, CurrentWave.EnemyColor);
    }

    void OnEnemyDeath()
    {
        RemainingAliveEnemies--;

        if(RemainingAliveEnemies == 0)
        {
            NextWave();
        }
    }

    void OnPlayerDeath()
    {
        Active = false;

    }

    void NextWave()
    {
        if(CurrentWaveNumber > 0)
        {
            AudioManager.INSTANCE.Play2DSound("LevelComplete");
        }
        CurrentWaveNumber++;
        if(CurrentWaveNumber - 1 < Waves.Length)
        {
            CurrentWave = Waves[CurrentWaveNumber - 1];
            RemainingEnemies = CurrentWave.EnemyCount;
            RemainingAliveEnemies = RemainingEnemies;
        }

        if(OnNewWave != null)
        {
            OnNewWave(CurrentWaveNumber);
        }

        ResetPlayerPosition();
    }

    void ResetPlayerPosition()
    {
        PlayerTransform.position = MapGen.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    [System.Serializable]
    public class Wave
    {
        public bool Infinite;
        public int EnemyCount;
        public float TimeBetweenSpawns;
        public float MoveSpeed;
        public int HitsToKillPlayer;
        public int EnemyHealth;
        public Color EnemyColor;
    }
}
