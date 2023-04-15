using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    private int score;
    private int highScore;
    private int lives;
    private Vector3 pacman_pos;
    private List<int> pelletlist;
    private bool hadData = false;

    public GameData() { }
    public GameData(int score, int highScore, int lives, Vector3 pacman_pos, List<int> pelletlist, bool hadData)
    {
        this.score = score;
        this.highScore = highScore;
        this.lives = lives;
        this.pacman_pos = pacman_pos;
        this.pelletlist = pelletlist;
        this.hadData = hadData;
    }

    public int Score { get => score; set => score = value; }
    public int HighScore { get => highScore; set => highScore = value; }
    public int Lives { get => lives; set => lives = value; }
    public Vector3 Pacman_pos { get => pacman_pos; set => pacman_pos = value; }
    public List<int> Pelletlist { get => pelletlist; set => pelletlist = value; }
    public bool HadData { get => hadData; set => hadData = value; }
}
