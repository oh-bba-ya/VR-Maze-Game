using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Flags]
public enum WallState
{
    // 0000 -> NO Walls
    // 1111 -> Left,Right,UP,Down
    LEFT = 1, // 0001
    RIGHT = 2, // 0010
    UP = 3,     // 0100
    DOWN = 4, // 1000
}

public static class MazeGenerator 
{
    //public static WallState[,] Generate(int width, int height)
    //{
    //    WallState[,] maze = new WallState[width, height];
        
    //    for(int i=0;)

    //    return maze;
    //}

}
