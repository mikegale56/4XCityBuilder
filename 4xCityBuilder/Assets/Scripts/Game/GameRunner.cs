using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRunner : MonoBehaviour
{

    public static float timeMultiplier = 60;
    public static float gameTime = 0;
    private static float lastWallClockTime = 0;

    private static float secondsPerMonth  = 60 * 60 * 24 * 7 * 4;
    private static float secondsPerWeek   = 60 * 60 * 24 * 7;
    private static float secondsPerDay    = 60 * 60 * 24;
    private static float secondsPerHour   = 60 * 60;
    private static float secondsPerMinute = 60;

    void Start ()
    {
        lastWallClockTime = Time.time;
        InvokeRepeating("UpdateGameTime", 1.0F, 0.5F);
    }
	
	void Update ()
    {
        GetTimeString();
    }

    public static string GetTimeString()
    {


        // Month, Week, Day, HH:MM:SS
        float now = gameTime;
        string timeString = "";
        int month = Mathf.FloorToInt(now / secondsPerMonth);
        now -= month * secondsPerMonth;
        month++;
        timeString += "Month " + month.ToString();

        int week  = Mathf.FloorToInt(now / secondsPerWeek);
        now -= week * secondsPerWeek;
        week++;
        timeString += ", Week " + week.ToString();

        int day = Mathf.FloorToInt(now / secondsPerDay);
        now -= day * secondsPerDay;
        day++;
        timeString += ", Day " + 1+day.ToString();

        int hour = Mathf.FloorToInt(now / secondsPerHour);
        now -= hour * secondsPerHour;
        timeString += ", " + hour.ToString().PadLeft(2, '0');

        int minute = Mathf.FloorToInt(now / secondsPerMinute);
        now -= minute * secondsPerMinute;
        timeString += ":" + minute.ToString().PadLeft(2, '0');

        timeString += ":" + Mathf.FloorToInt(now).ToString().PadLeft(2, '0'); ;

        //Debug.Log(gameTime.ToString() + " => " + timeString);
        return timeString;
    }

    void UpdateGameTime()
    {
        float time = Time.time;
        float deltaT = time - lastWallClockTime;
        gameTime += deltaT * timeMultiplier;
        lastWallClockTime = time;
    }

}
