using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

public class LevelDataManager : MonoBehaviour
{
    public static LevelDataManager instance; // Singleton instance

    public List<LevelData> levels;
    public int selectedLevel;
    public bool isLevelSelected = false;
    public bool hasAchievedHighestScore = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Preserve the data across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }

        // Load the initial levels from StreamingAssets
        LoadLevels(Application.streamingAssetsPath + "/Levels", false);
        StartCoroutine(DownloadLevels());
    }
    public void OnLevelSelected(int levelNumber)
    {
        selectedLevel = levelNumber;
        isLevelSelected = true;
        hasAchievedHighestScore = false;
    }

    public LevelData GetCurrentLevel()
    {
        return levels[selectedLevel - 1];
    }
    private void LoadLevels(string folderPath, bool isLoadedFromPersistent)
    {
        int levelCounts = CountFilesWithoutExtensions(folderPath);
        string levelType = "RM_A";

        for (int i = 0; i < levelCounts; i++)
        {
            int levelNum = i + 1;
            if (isLoadedFromPersistent && i < 5)
            {
                levelNum += 10;
            }
            else if (isLoadedFromPersistent)
            {
                levelNum -= 5;
                levelType = "RM_B";
            }
            LevelData levelData = ParseLevelData(Path.Combine(folderPath, levelType + levelNum));
            levels.Add(levelData);
        }
    }
    public static LevelData ParseLevelData(string filePath)
    {
        LevelData levelData = new LevelData();
        StreamReader reader = new StreamReader(filePath);
        string line;

        while ((line = reader.ReadLine()) != null)
        {
            string[] tokens = line.Split(':');

            if (tokens.Length == 2)
            {
                switch (tokens[0])
                {
                    case "level_number":
                        levelData.levelNumber = int.Parse(tokens[1]);
                        break;
                    case "grid_width":
                        levelData.gridWidth = int.Parse(tokens[1]);
                        break;
                    case "grid_height":
                        levelData.gridHeight = int.Parse(tokens[1]);
                        break;
                    case "move_count":
                        levelData.moveCount = int.Parse(tokens[1]);
                        break;
                    case "grid":
                        levelData.gridData = GetItemColors(tokens[1]);
                        break;
                }
            }
        }

        reader.Close();
        return levelData;
    }

    private static List<char> GetItemColors(string itemColors)
    {
        List<char> itemColorsList = new List<char>();

        string itemColorsString = itemColors.Trim();
        string[] lettersArray = itemColorsString.Split(',');

        foreach (string letter in lettersArray)
        {
            if (letter.Length > 0)
            {
                char c = letter[0];
                itemColorsList.Add(c);
            }
        }

        return itemColorsList;
    }

    public static int CountFilesWithoutExtensions(string folderPath)
    {
        int fileCount = 0;

        DirectoryInfo directory = new DirectoryInfo(folderPath);
        foreach (FileInfo fileInfo in directory.GetFiles())
        {
            if (fileInfo.Extension.Length == 0)
            {
                fileCount++;
            }
        }

        return fileCount;
    }

    public bool IsPrevLevelUnlocked(int levelNumber)
    {
        int previousLevelScore = PlayerPrefs.GetInt((levelNumber - 1).ToString());

        if (previousLevelScore != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetCompletedLevel()
    {
        if (hasAchievedHighestScore)
        {
            if(PlayerPrefs.GetInt("maxAchievedLevel") < selectedLevel){
                PlayerPrefs.SetInt("maxAchievedLevel", selectedLevel);
            }
            return selectedLevel;
        }
        else
        {
            return 0;
        }
    }

    private IEnumerator DownloadLevels()
    {
        string baseURL = "https://row-match.s3.amazonaws.com/levels/";
        for (int i = 11; i <= 25; i++)
        {
            string fileName, levelURL;
            if (i < 16)
            {
                fileName = "RM_A" + i;
            }
            else
            {
                fileName = "RM_B" + (i - 15);
            }

            levelURL = baseURL + fileName;

            DirectoryInfo d = new DirectoryInfo(Application.persistentDataPath);
            FileInfo[] files = d.GetFiles();
            bool check = true;

            // Check if the level file is already downloaded
            foreach (var item in files)
            {
                if(item.Name.Equals(fileName)){
                    check = false;
                }
            }

            if (check)
            {
                using UnityWebRequest webRequest = UnityWebRequest.Get(levelURL);
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    // Deserialize the downloaded JSON data into a LevelData object
                    string savePath = Application.persistentDataPath + "/" + fileName;
                    File.WriteAllText(savePath, webRequest.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("Level download error: " + webRequest.error);
                }
            }
        }
        LoadLevels(Application.persistentDataPath, true);
    }
}