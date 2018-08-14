using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRunner : MonoBehaviour
{

    public static float timeMultiplier = 600;
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
    }
	
	void Update ()
    {
        UpdateGameTime();
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
        timeString += ", Day " + day.ToString();

        int hour = Mathf.FloorToInt(now / secondsPerHour);
        now -= hour * secondsPerHour;
        timeString += ", " + hour.ToString().PadLeft(2, '0');

        int minute = Mathf.FloorToInt(now / secondsPerMinute);
        now -= minute * secondsPerMinute;
        timeString += ":" + minute.ToString().PadLeft(2, '0');

        timeString += ":" + Mathf.FloorToInt(now).ToString().PadLeft(2, '0');
        return timeString;
    }
	
	
	public static string GetTimeUntilPMUs(float remainingPMUs)
	{
		float remainingSeconds = remainingPMUs * 60.0F;
		string timeString = "";
		
		int month = Mathf.FloorToInt(remainingSeconds / secondsPerMonth);
        remainingSeconds -= month * secondsPerMonth;
		if (month > 0)
			timeString += month.ToString() + " Months, ";

        int week  = Mathf.FloorToInt(remainingSeconds / secondsPerWeek);
        remainingSeconds -= week * secondsPerWeek;
		if (week > 0)
			timeString += week.ToString() + " Weeks, ";

        int day = Mathf.FloorToInt(remainingSeconds / secondsPerDay);
        remainingSeconds -= day * secondsPerDay;
        if (day > 0)
			timeString += day.ToString() + " Days, ";

        int hour = Mathf.FloorToInt(remainingSeconds / secondsPerHour);
        remainingSeconds -= hour * secondsPerHour;
        timeString += hour.ToString().PadLeft(2, '0');

        int minute = Mathf.FloorToInt(remainingSeconds / secondsPerMinute);
        remainingSeconds -= minute * secondsPerMinute;
        timeString += ":" + minute.ToString().PadLeft(2, '0');

        timeString += ":" + Mathf.FloorToInt(remainingSeconds).ToString().PadLeft(2, '0');
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