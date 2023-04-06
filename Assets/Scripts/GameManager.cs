using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;
    public List<Pellet> listPellet;

    public Text titleText;
    public Text scoreText;
    public Text livesText;

    public int ghostMultiplier { get; private set; } = 1;
    public int score { get; private set; }

    public int highScore { get; private set; }
    public int lives { get; private set; }

    public bool gameRunning = false;
    
    private void Start()
    {
        SoundManager.Instance.PlaySound(SoundManager.PlayList.Pacman_StartSound);
        if(!LoadGame())
            NewGame();
        gameRunning = true;
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
    }

    private void NewGame()
    {
        SoundManager.Instance.PlaySound(SoundManager.PlayList.Pacman_StartSound);
        SetScore(0);
        SetLives(3);
        NewRound();
    }
    public void ContinueGame()
    {
        scoreText.text = score.ToString();
        gameRunning = true;
        SetLives(lives);
        foreach (var ghost in ghosts)
        {
            ghost.gameObject.SetActive(true);
        }
    }
    public void SaveGame()
    {
        GameData gameData = new GameData();
        gameData.score = score;
        gameData.live = lives;
        gameData.pacmanPos = this.pacman.transform.position;
        List<int> foodlists = new List<int>();
        foreach (var pellet in listPellet)
        {
            if (pellet.gameObject.activeSelf == false) foodlists.Add(0);
            else foodlists.Add(1);
        }
        gameData.pellets = foodlists;
        SaveSystem.Instance.SaveGame(gameData);

        List<GhostData> ghostDatas = new List<GhostData>();
        for (int i = 0; i < ghosts.Length; i++)
        {
            GhostData data = new GhostData();
            data.ghostsPos = ghosts[i].transform.position;
            data.CurrentDirection = ghosts[i].movement.direction;
            data.ghostsBehaviour = (int)ghosts[i].behaviour;
            ghostDatas.Add(data);
        }
        SaveSystem.Instance.SaveList(ghostDatas);
    }

    public bool LoadGame()
    {
        GameData gameData = SaveSystem.Instance.LoadGameData();
        List<GhostData> ghostDatas = SaveSystem.Instance.LoadGhostList(ghosts.Length);
        if (gameData.hadData)
        {
            this.lives = gameData.live;
            this.score = gameData.score;
            for (int i = 0; i < listPellet.Count; i++)
            {
                if (gameData.pellets[i] == 0) listPellet[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < ghosts.Length; i++)
            {
                ghosts[i].transform.position = ghostDatas[i].ghostsPos;
                ghosts[i].movement.SetDirection(ghostDatas[i].CurrentDirection);
                ghosts[i].behaviour = (Ghost.Behaviour)ghostDatas[i].ghostsBehaviour;
            }
            this.pacman.transform.position = gameData.pacmanPos;
            return true;
        }
        return false;
    }

    private void NewRound()
    {
        SoundManager.Instance.PlaySound(SoundManager.PlayList.Pacman_StartSound);

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

        ghostMultiplier++;
    }

    public void PelletEaten(Pellet pellet)
    {
        SoundManager.Instance.PlaySound(SoundManager.PlayList.Pacman_Chomp);
        pellet.gameObject.SetActive(false);

        SetScore(score + pellet.points);

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
