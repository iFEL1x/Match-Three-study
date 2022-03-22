/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

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
