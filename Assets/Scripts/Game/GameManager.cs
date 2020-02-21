using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public List<GameObject> targets;
    private float spawnRate = 1.0f;
    private int score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI playerNameText;
    public GameObject titleScreen;
    public GameObject gameOverScreen;
    public bool isGameActive;
    private string age;

    public TextMeshProUGUI playersOnlineText;

    private Player player;
    private Player playerOnline;

    public GameObject leftColumn;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        playerOnline = FindObjectOfType<Player>();
        playerNameText.text = player.Name;
        scoreText.text = score.ToString();
        StartCoroutine(getPlayersOnline());
    }

    public IEnumerator getPlayersOnline()
    {
        //getAge();
        //playersOnlineText.text += player.Name + "("+age+")";
       
            Player player = FindObjectOfType<Player>();
            UnityWebRequest httpClient = new UnityWebRequest(player.HttpServerAddress + "/api/Player/Online", "GET");

            httpClient.SetRequestHeader("Authorization", "bearer " + player.Token);
            httpClient.SetRequestHeader("Accept", "application/json");

            httpClient.downloadHandler = new DownloadHandlerBuffer();

            yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Helper > GetPlayerInfo: " + httpClient.error);
        }
        else
        {

            string jsonResponse = httpClient.downloadHandler.text;

            string response = "{\"players\":" + jsonResponse + "}";
            ListPlayerOnline players = JsonUtility.FromJson<ListPlayerOnline>(response);
        
        foreach (PlayerOnline str in players.players)
        {
                    getAge();
                    playersOnlineText.text += playerOnline.Name + "(" + age + ")\n" + playerOnline.ConnectedSince;
          
            
        }
        }

    }

    private void getAge()
    {
        DateTime birthday = playerOnline.BirthDay;
        DateTime today = DateTime.Now;
        TimeSpan difference = today - birthday;
        age = (difference.Days / 365).ToString();

    }

    public void StartGame(int difficulty)
    {
        isGameActive = true;
        score = 0;
        UpdateScore(0);
        titleScreen.gameObject.SetActive(false);
        spawnRate /= difficulty;
        StartCoroutine(SpawnTarget());
    }

    IEnumerator SpawnTarget()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);
            int randomIndex = UnityEngine.Random.Range(0, 4);
            Instantiate(targets[randomIndex]);
        }
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        gameOverScreen.gameObject.SetActive(true);
        isGameActive = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
