using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAVEGAME : MonoBehaviour
{
    public static SAVEGAME Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    public GameData gameData;
    public GhostData ghostData;

    public void Save(GameData gameData, List<GhostData> ghostDatas)
    {
        SaveSystem.SetInt("score", gameData.Score);
        SaveSystem.SetInt("highScore", gameData.HighScore);
        SaveSystem.SetInt("lives", gameData.Lives);
        SaveSystem.SetVector3("pacman_pos", gameData.Pacman_pos);
        SaveSystem.SetInt("pellet_count", gameData.Pelletlist.Count);
        gameData.HadData = true;
        SaveSystem.SetBool("hadData", gameData.HadData);
        for(int i=0; i<gameData.Pelletlist.Count; i++)
        {
            SaveSystem.SetInt("pellet[" + i + "]", gameData.Pelletlist[i]);
        }
        SaveSystem.SetInt("ghost_count", ghostDatas.Count);
        for(int j=0; j<ghostDatas.Count; j++)
        {
            SaveSystem.SetVector3("ghost[" + j + "]_pos", ghostDatas[j].Ghost_pos);
            SaveSystem.SetVector2("ghost[" + j + "]_current_direction", ghostDatas[j].Ghost_current_direction);
            SaveSystem.SetInt("ghost[" + j + "]_behaviour", ghostDatas[j].Ghost_behaviour);
        }
    }

    public GameData LoadGameData()
    {
        GameData gameData = new GameData();
        List<int> listTemp = new List<int>();
        gameData.Score = SaveSystem.GetInt("score");
        gameData.HighScore = SaveSystem.GetInt("highScore");
        gameData.Lives = SaveSystem.GetInt("lives");
        gameData.Pacman_pos = SaveSystem.GetVector3("pacman_pos");
        int pellet_count = SaveSystem.GetInt("pellet_count");
        gameData.HadData = SaveSystem.GetBool("hadData");
        for(int i=0; i < pellet_count; i++)
        {
            int temp = SaveSystem.GetInt("pellet[" + i + "]");
            listTemp.Add(temp);
        }
        gameData.Pelletlist = listTemp;
        return gameData;
    }

    public List<GhostData> LoadGhostData()
    {
        List<GhostData> ghostDatas = new List<GhostData>();
        int ghost_count = SaveSystem.GetInt("ghost_count");
        for(int i = 0; i < ghost_count; i++)
        {
            GhostData ghostData = new GhostData();
            ghostData.Ghost_pos = SaveSystem.GetVector3("ghost[" + i + "]_pos");
            ghostData.Ghost_current_direction = SaveSystem.GetVector2("ghost[" + i + "]_current_direction");
            ghostData.Ghost_behaviour = SaveSystem.GetInt("ghost["+ i +"]_behaviour");
            ghostDatas.Add(ghostData);
        }
        return ghostDatas;
    }
}
