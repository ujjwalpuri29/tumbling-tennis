using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }
    [SerializeField]
    private TMP_Text scoreTxt;
    [SerializeField]
    private TMP_Text winnerTxt;
    [SerializeField]
    private GameObject winnerPanel;
    [SerializeField]
    private int playerOne = 0;
    [SerializeField]
    private int playerTwo = 0;
    [SerializeField]
    private int winScore = 10;
    [SerializeField]
    private float returnDelay = 5f;
    private bool gameEnded = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        UpdateScore();
        if (winnerPanel != null)
        {
            winnerPanel.SetActive(false);
        }
        else if (winnerTxt != null)
        {
            winnerTxt.gameObject.SetActive(false);
        }
    }
    public bool AddScore(Player player)
    {
        if (gameEnded)
            return true;
        if (player == Player.playerOne)
        {
            playerOne++;
        }
        else
        {
            playerTwo++;
        }
        UpdateScore();
        if (playerOne >= winScore)
        {
            EndGame(Player.playerOne);
            return true;
        }
        if (playerTwo >= winScore)
        {
            EndGame(Player.playerTwo);
            return true;
        }
        return false;
    }
    private void EndGame(Player winner)
    {
        gameEnded = true;

        if (scoreTxt != null)
        {
            scoreTxt.gameObject.SetActive(false);
        }

        if (winnerPanel != null)
        {
            winnerPanel.SetActive(true);
        }

        if (winnerTxt != null)
        {
            winnerTxt.gameObject.SetActive(true);
            if (winner == Player.playerOne)
            {
                winnerTxt.text = $"Player 1 Wins!\nFinal Score: {playerOne} : {playerTwo}";
            }
            else
            {
                winnerTxt.text = $"Player 2 Wins!\nFinal Score: {playerOne} : {playerTwo}";
            }
        }
        Time.timeScale = 0f;
        StartCoroutine(ReturnToStartPage());
    }
    private IEnumerator ReturnToStartPage()
    {
        yield return new WaitForSecondsRealtime(returnDelay);
        Time.timeScale = 1f;
        if (ModeChoose.Instance != null)
        {
            ModeChoose.Instance.BackToStart();
        }
        SceneManager.LoadScene("StartPage");
    }
    public void ResetScore()
    {
        playerOne = 0;
        playerTwo = 0;
        gameEnded = false;
        if (scoreTxt != null)
        {
            scoreTxt.gameObject.SetActive(true);
        }
        UpdateScore();
        if (winnerPanel != null)
        {
            winnerPanel.SetActive(false);
        }
        else if (winnerTxt != null)
        {
            winnerTxt.gameObject.SetActive(false);
        }
        Time.timeScale = 1f;
    }
    private void UpdateScore()
    {
        if (scoreTxt == null)
        {
            return;
        }
        scoreTxt.text = $"{playerOne} : {playerTwo}";
    }
}
