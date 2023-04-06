using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : ScriptableObject
{
    public int score;
    public int live;

    public List<int> pellets;
    public Vector3 pacmanPos;
    public bool hadData = false;

    public GameData()
    {
        
    }
}
