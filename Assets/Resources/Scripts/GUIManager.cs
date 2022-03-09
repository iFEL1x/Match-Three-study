using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public static GUIManager Instance;

    [SerializeField] private Text yourScoreTxt;
    [SerializeField] private Text highScoreTxt;
    [SerializeField] private Text scoreTxt;
    [SerializeField] private Text moveCounterTxt;
    [SerializeField] private Text timerTxt;

    [SerializeField] private int moveCounter;

    private int score;
    private int timer;

    public GameObject gameOverPanel;



    public int Score //Накопленные очки за раунд.
    {
        get { return score; }
        set
        {
            score = value;
            scoreTxt.text = "SCORE: " + score.ToString();
        }
    }

    public int Timer //Ограничение времени на раунд.
    {
        get { return timer; }
        set
        {
            timer = value;

            if(timer > 0)
            {
                timerTxt.gameObject.SetActive(true);
            }
            if (timer <= 0)
            {
                timer = 0;
                StartCoroutine(WaitForShifting());
            }

            timerTxt.text = "Timer: " + timer.ToString();
        }
    }

    public int MoveCounter //Колличество допустимых ходов в раунде.
    {
        get { return moveCounter; }
        set
        {
            moveCounter = value;
            if(moveCounter <= 0)
            {
                moveCounter = 0;
                StartCoroutine(WaitForShifting());
            }

            moveCounterTxt.text = "Move: " + moveCounter.ToString();
        }
    }



    private void Awake()
    {
        Instance = this;
        
        if(moveCounter == 0)
        {
            moveCounter = 20;
        }
        moveCounterTxt.text = "Move: " + moveCounter.ToString();

        if( Timer <= 0)
        {
            timerTxt.gameObject.SetActive(false);
        }

        if (PlayerPrefs.HasKey("Player_HightScore"))
        {
            highScoreTxt.text = PlayerPrefs.GetString("Player_HightScore");
        }
    }
    public void SaveHightScore()
    {
        PlayerPrefs.SetString("Player_HightScore", highScoreTxt.text);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        yourScoreTxt.text = score.ToString();

        if (int.Parse(yourScoreTxt.text) > int.Parse(highScoreTxt.text) || int.Parse(highScoreTxt.text) == 0)
        {
            highScoreTxt.text = score.ToString();
            SaveHightScore();
        }

        BoardManager.Instance.gameObject.SetActive(false);
        GameTimer.Instance.tTimer = 0;
    }

    private IEnumerator WaitForShifting()
    {
        yield return new WaitUntil(() => !BoardManager.Instance.IsShifting);
        yield return new WaitForSeconds(.5f);
        GameOver();
    }

    public void OnClickMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OnClickRestart()
    {
        SceneManager.LoadScene(1);
    }
}
