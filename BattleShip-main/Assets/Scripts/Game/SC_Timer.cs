using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Timer : MonoBehaviour
{
    public float originalTimerRemaining = 60;
    private float timeRemaining;
    public bool timerIsRunning = false;
    public TextMesh timeText;
    // Start is called before the first frame update
    void Start()
    {
        // Starts the timer automatically
        timerIsRunning = true;
        timeRemaining = originalTimerRemaining;
        timeText = GetComponent<TextMesh>();
    }
    public void resetTimer()
    {
        timeRemaining = originalTimerRemaining;
        timerIsRunning = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;   
            }
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        gameObject.GetComponent<TextMesh>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
