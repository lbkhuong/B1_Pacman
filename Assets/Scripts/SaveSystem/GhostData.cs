using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostData
{
    private Vector3 ghost_pos;
    private Vector2 ghost_current_direction;
    private int ghost_behaviour;

    public GhostData() { }
    public GhostData(Vector3 ghost_pos, Vector2 ghost_current_direction, int ghost_behavior)
    {
        this.Ghost_pos = ghost_pos;
        this.Ghost_current_direction = ghost_current_direction;
        this.Ghost_behaviour = ghost_behaviour;
    }

    public Vector3 Ghost_pos { get => ghost_pos; set => ghost_pos = value; }
    public Vector2 Ghost_current_direction { get => ghost_current_direction; set => ghost_current_direction = value; }
    public int Ghost_behaviour { get => ghost_behaviour; set => ghost_behaviour = value; }
}
