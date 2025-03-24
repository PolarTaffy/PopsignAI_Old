using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using LitJson;

public class RandomVideoSelector : MonoBehaviour
{
	public string[] colors = new string[] {"blue", "green", "red", "violet", "yellow"};
	public string curtVideoName;
	public string reviewWord;
	public string folderName;
	public VideoPlayer videoPlayer;
	public int curtVideoIndex;
	public int frameCount;
	private JsonData levelData;

	void Start()
	{
		ReadJsonFromTXT();
		RandomizeVideo();
	}

	//POPSign Read Json file(The connection between colors and videos)
	void ReadJsonFromTXT()
	{
		int currentLevel = PlayerPrefs.GetInt("OpenLevel");

		//For Random Levels (BK)
		int randomizeLevels = PlayerPrefs.GetInt("RandomizeLevel", 0);
		//For Random Levels (BK)

		TextAsset textReader;
		if(randomizeLevels == 0)
        {
			//Original code
			textReader = Resources.Load("VideoConnection/" + "level" + currentLevel) as TextAsset;
		}
        else
        {
			string path = Application.persistentDataPath + "/level" + currentLevel + ".txt";
			string levelText;
			using(StreamReader reader = new StreamReader(path))
            {
				levelText = reader.ReadToEnd();
            }

			textReader = new TextAsset(levelText);
		}
		levelData = JsonMapper.ToObject(textReader.text);
	}

	public void RandomizeVideo()
	{
		string color = colors[UnityEngine.Random.Range(0, colors.Length)];
		reviewWord = levelData[color + "fileName"] + "";
		folderName = levelData[color + "folderName"] + "";
		string frameNumber = levelData[color + "frameNumber"] + "";
		string imageName = levelData [color + "ImageName"] + "";

		if (reviewWord != "" && folderName != "" && frameNumber != "" && imageName != "")
		{
			Debug.Log(frameNumber);
			Debug.Log(reviewWord);
			Debug.Log(folderName);
			Debug.Log(imageName);
			Debug.Log(color);
			#if UNITY_EDITOR
			videoPlayer.url = Application.dataPath + "/StreamingAssets/" + folderName +".mp4";
			#elif UNITY_ANDROID
					videoPlayer.url = "jar:file://" + Application.dataPath + "!/assets/"+ folderName +".mp4";
			#elif UNITY_IOS
					videoPlayer.url = Application.dataPath + "/Raw/" + folderName + ".mp4";
			#endif
			videoPlayer.Prepare();
			videoPlayer.Play();
		}
	}
}
