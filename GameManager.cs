using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum gameStatus {
    next, play, gameover, win
}

public class GameManager : Singleton<GameManager> {

    
	// public static GameManager instance = null;
    
    [SerializeField] private int totalWaves = 15;
    [SerializeField] private Text totalMoneyLbl;
    [SerializeField] private Text currentWaveLbl;
    [SerializeField] private Text playBtnLbl;
    [SerializeField] private Button playBtn;
    [SerializeField] private Text totalEscapedLbl;
	[SerializeField] private GameObject spawnPoint;
	[SerializeField] private GameObject[] enemies;
	[SerializeField] private int totalEnemies = 3;
	[SerializeField] private int enemiesPerSpawn;

    private int spawnNum = 0;
    private int waveNumber = 0;
    private int totalMoney = 10;
    private int totalEscaped = 0;
    private int roundEscaped = 0;
    private int totalKilled = 0;
    // private int whichEnemiesToSpawn = 0;
    private gameStatus currentState = gameStatus.play;
    private AudioSource audioSource;

    public List<Enemy> EnemyList = new List<Enemy>();

    const float spawnDelay = 0.65f;

    public int TotalEscaped {
        get {
            return totalEscaped;
        }
        set {
            totalEscaped = value;
        }
    }
    public int RoundEscaped {
        get {
            return roundEscaped;
        }
        set {
            roundEscaped = value;
        }
    }
    public int TotalKilled {
        get {
            return totalKilled;
        }
        set {
            totalKilled = value;
        }
    }
    public int TotalMoney {
        get {
            return totalMoney;
        }
        set {
            totalMoney = value;
            totalMoneyLbl.text = totalMoney.ToString();
        }
    } 

    public AudioSource AudioSource {
        get {
            return audioSource;
        }
    }

	void Start () {
        playBtn.gameObject.SetActive(false);
        ShowMenu();
        audioSource = GetComponent<AudioSource>();
	}
    
    void Update () {
        
    }

    IEnumerator Spawn(){
        if (waveNumber <= 1){
            spawnNum = 0;
        }
        else if (waveNumber >= 2 && waveNumber < 4){
            spawnNum = Random.Range(0,2);
        }
        else if (waveNumber >= 4 ){
            spawnNum = Random.Range(0,3);
        }
        if (enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies) {
            for (int i = 0; i < enemiesPerSpawn; i++){
                if (EnemyList.Count < totalEnemies) {
                    GameObject newEnemy = Instantiate(enemies[spawnNum]) as GameObject;
                    newEnemy.transform.position = spawnPoint.transform.position;  
                }
            }
            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(Spawn());
        }

    }

    public void RegisterEnemy(Enemy enemy){
        EnemyList.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy){
        EnemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    public void DestroyAllEnemies(){
        foreach(Enemy enemy in EnemyList){
            Destroy(enemy.gameObject);
        }

        EnemyList.Clear();
    }

    public void AddMoney(int amount){
        TotalMoney += amount;
    }

    public void SubtractMoney(int amount){
        TotalMoney -= amount;
    }
    
    public void IsWaveOver(){
        totalEscapedLbl.text = "Escaped " + TotalEscaped + "/10";
        if ((RoundEscaped + TotalKilled) == totalEnemies) {
            SetCurrentGameState();
            ShowMenu();
        }
    }

    public void SetCurrentGameState(){
        if (TotalEscaped >= 10) {
            currentState = gameStatus.gameover;
            waveNumber = 0;
        }
        else if ((waveNumber == 0 && (TotalKilled + RoundEscaped) == 0)){
            currentState = gameStatus.play;
        }
        else if (waveNumber >= totalWaves){
            currentState = gameStatus.win;
        }
        else {
            currentState = gameStatus.next;
        }
    }

    public void ShowMenu(){
        switch (currentState) {
            case gameStatus.gameover:
                playBtnLbl.text = "Play Again!";
                AudioSource.PlayOneShot(SoundManager.Instance.Gameover);
                break;

            case gameStatus.next:
                playBtnLbl.text = "Next Wave";
                break;

            case gameStatus.play:
                playBtnLbl.text = "Play";
                break;

            case gameStatus.win:
                playBtnLbl.text = "Play";
                break;
        }
        playBtn.gameObject.SetActive(true);
    }

    public void playBtnPressed(){
        switch(currentState) {
            case gameStatus.next:
                waveNumber += 1;
                totalEnemies += waveNumber;
                break;
            
            default:
                totalEnemies = 3;
                TotalEscaped = 0;
                TotalMoney = 10;
                TowerManager.Instance.DestroyAllTower();
                TowerManager.Instance.RenameTagsBuildSites();
                totalMoneyLbl.text = TotalMoney.ToString();
                totalEscapedLbl.text = "Escaped " + totalEscaped + "/10";
                audioSource.PlayOneShot(SoundManager.Instance.Newgame);
                break;
        }
        DestroyAllEnemies();
        TotalKilled = 0;
        RoundEscaped = 0;
        currentWaveLbl.text = "Wave " + (waveNumber + 1);
        StartCoroutine(Spawn());
        playBtn.gameObject.SetActive(false);
    }
}
