using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;
    public List<Pellet> listPellet;
    public Tilemap pelletsTilemap;
    public Text titleText;
    public Text scoreText;
    public Text livesText;
    public Text highScoreText;
    public int ghostMultiplier { get; private set; } = 1;
    public int score { get; private set; }
    public int highScore { get; private set; }
    public int lives { get; private set; }
    
    public static List<int> listfood;
    private void Start()
    {
        SoundManager.Instance.PlaySound(SoundManager.PlayList.Pacman_StartSound);
        for(int i=-25; i<=24; i++)
        {
            for(int j=-16; j<=12; j++)
            {
                if (pelletsTilemap.GetInstantiatedObject(new Vector3Int(i, j, 0))!=null)
                    listPellet.Add(pelletsTilemap.GetInstantiatedObject(new Vector3Int(i, j, 0)).GetComponent<Pellet>());
            }
        }
        if (!LoadGame())
        NewGame();
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    private void Update()
    {
        if (lives <= 0 && Input.anyKeyDown) {
            NewGame();
        }
        if(highScore < score)
        {
            highScore = score;
        }
    }

    private void NewGame()
    {
        SoundManager.Instance.PlaySound(SoundManager.PlayList.Pacman_StartSound);
        SetScore(0);
        SetLives(3);
        NewRound();
    }
    public void SaveGame()
    {
        GameData gameData = new GameData();
        gameData.Score = score;
        gameData.HighScore = highScore;
        gameData.Lives = lives;
        gameData.Pacman_pos = this.pacman.transform.position;
        List<int> foodlists = new List<int>();
        foreach (var pellet in listPellet)
        {
            if (pellet.gameObject.activeSelf == false) foodlists.Add(0);
            else foodlists.Add(1);
        }
        gameData.Pelletlist = foodlists;
        

        List<GhostData> ghostDatas = new List<GhostData>();
        for (int i = 0; i < ghosts.Length; i++)
        {
            GhostData ghostData = new GhostData();
            ghostData.Ghost_pos= ghosts[i].transform.position;
            ghostData.Ghost_current_direction = ghosts[i].movement.direction;
            ghostData.Ghost_behaviour = (int)ghosts[i].behaviour;
            ghostDatas.Add(ghostData);
        }
        SAVEGAME.Instance.Save(gameData, ghostDatas);
    }
    public bool LoadGame()
    {
        GameData gameData = SAVEGAME.Instance.LoadGameData();
        List<GhostData> ghostDatas = SAVEGAME.Instance.LoadGhostData();
        if (gameData.HadData)
        {
            this.score = gameData.Score;
            this.highScore = gameData.HighScore;
            this.lives = gameData.Lives;
            SetScore(score);
            SetHighScore(highScore, score);
            this.pacman.transform.position = gameData.Pacman_pos;
            
            for (int i = 0; i < listPellet.Count; i++)
            {
                if (gameData.Pelletlist[i] == 0) listPellet[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < ghosts.Length; i++)
            {
                ghosts[i].transform.position = ghostDatas[i].Ghost_pos;
                ghosts[i].movement.SetDirection(ghostDatas[i].Ghost_current_direction);
                ghosts[i].behaviour = (Ghost.Behaviour)ghostDatas[i].Ghost_behaviour;
            }
            return true;
        }
        return false;
    }

    private void NewRound()
    {
        SoundManager.Instance.PlaySound(SoundManager.PlayList.Pacman_StartSound);
        titleText.text = "";
        foreach (Transform pellet in pellets) {
            pellet.gameObject.SetActive(true);
        }

        ResetState();
    }

    private void ResetState()
    {
        SoundManager.Instance.PlaySound(SoundManager.PlayList.Pacman_StartSound);
        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].ResetState();
        }

        pacman.ResetState();
    }

    private void GameOver()
    {
        titleText.text = "GAME OVER";

        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].gameObject.SetActive(false);
        }
        
        pacman.gameObject.SetActive(false);
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = "x" + lives.ToString();
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString().PadLeft(2, '0');
    }

    private void SetHighScore(int highScore, int score)
    {
        this.highScore = highScore;
        this.score = score;
        if (score > highScore)
        {
            highScore = score;
        }
        highScoreText.text = highScore.ToString().PadLeft(2, '0');
    }

    public void PacmanEaten()
    {
        SoundManager.Instance.StopMusic();
        SoundManager.Instance.PlaySound(SoundManager.PlayList.Pacman_Death);

        pacman.DeathSequence();

        SetLives(lives - 1);

        if (lives > 0) {
            Invoke(nameof(ResetState), 3f);
        } else {
            GameOver();
        }
    }

    public void GhostEaten(Ghost ghost)
    {
        SoundManager.Instance.PlaySound(SoundManager.PlayList.Pacman_EatGhost);
        int points = ghost.points * ghostMultiplier;
        SetScore(score + points);
        SetHighScore(highScore, score);
        ghostMultiplier++;
    }

    public void PelletEaten(Pellet pellet)
    {
        SoundManager.Instance.PlaySound(SoundManager.PlayList.Pacman_Chomp);
        pellet.gameObject.SetActive(false);

        SetScore(score + pellet.points);
        SetHighScore(highScore, score);
        if (!HasRemainingPellets())
        {
            SoundManager.Instance.StopMusic();
            pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 3f);
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        CancelInvoke(nameof(ResetGhostMultiplier));
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf) {
                return true;
            }
        }

        return false;
    }

    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }

}
