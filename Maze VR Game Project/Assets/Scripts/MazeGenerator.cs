using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 5;
    public int height = 5;

    public Cell cellPrefab;

    private Cell[,] cellMap;
    private Stack<Cell> cellHisttoryStack;


    // 갈수 있는 경로
    

    // Start is called before the first frame update
    void Start()
    {
        BatchCells();
        MakeMaze(cellMap[0,0]);

        // 처음과 끝에 입구 출구 만들기.
        cellMap[0, 0].isLeftWall = false;
        cellMap[width - 1, height - 1].isRightWall = false;
        cellMap[0, 0].ShowWalls();
        cellMap[width - 1, height - 1].ShowWalls();
    }

    private void BatchCells()
    {
        cellMap = new Cell[width, height];
        cellHisttoryStack = new Stack<Cell>();
  

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Cell _cell = Instantiate<Cell>(cellPrefab, this.transform);
                _cell.index = new Vector2Int(x, y);
                _cell.name = "cell_" + x + "_" + y;
                _cell.transform.localPosition = new Vector3(x * 5, 0, y * 5);                                   // Maze 부모 객체 위치에 기반해서 생성.

                cellMap[x, y] = _cell;
                
            }
        }
    }

    private void MakeMaze(Cell startCell)
    {
        Cell[] neighbors = GetNeighborCells(startCell);
        if(neighbors.Length > 0)
        {
            Cell nextCell = neighbors[Random.Range(0, neighbors.Length)];
            ConnectCells(startCell, nextCell);
            cellHisttoryStack.Push(nextCell);

            // 재귀함수
            MakeMaze(nextCell);
        }
        else
        {
            if(cellHisttoryStack.Count > 0)
            {
                Cell lastCell = cellHisttoryStack.Pop();
                MakeMaze(lastCell);
            }
        }
    }


    /// <summary>
    /// 주변(기준 cell 의 상하좌우)을 탐색하는 함수.
    /// </summary>
    /// <param name="cell"> 기준 </param>
    /// <returns></returns>
    private Cell[] GetNeighborCells(Cell cell)
    {
        List<Cell> retCellList = new List<Cell>();
        Vector2Int index = cell.index;

        // forward
        // 만약 5 x 5 맵을 생성한다면 index[i,j] =  에 0~4 범위가 순차적으로 입력된다. , 그러므로 인덱스 마지막 Cell =  4일때 주변 Cell이 존재하기 위해서는 + 1해서 처음 설정한 5 x 5 즉 Height = 5 보다 작아야 범위내에 존재한다고 볼 수 있다.
        if(index.y +1 < height)
        {
            // 즉 기준으로 입력받은 cell 의 앞에 있는  cell을 뜻함.
            Cell neighbor = cellMap[index.x, index.y + 1];
            if (neighbor.CheckAllWall())
            {
                retCellList.Add(neighbor);
            }
            
        }

        // back
        if (index.y - 1 >= 0)
        {
            // 즉 기준으로 입력받은 cell 의 앞에 있는  cell을 뜻함.
            Cell neighbor = cellMap[index.x, index.y - 1];
            if (neighbor.CheckAllWall())
            {
                retCellList.Add(neighbor);
            }

        }

        // left
        if (index.x -  1 >= 0)
        {
            // 즉 기준으로 입력받은 cell 의 앞에 있는  cell을 뜻함.
            Cell neighbor = cellMap[index.x -1, index.y];
            if (neighbor.CheckAllWall())
            {
                retCellList.Add(neighbor);
            }

        }

        // right
        if (index.x + 1 < width)
        {
            // 즉 기준으로 입력받은 cell 의 앞에 있는  cell을 뜻함.
            Cell neighbor = cellMap[index.x +1, index.y];
            if (neighbor.CheckAllWall())
            {
                retCellList.Add(neighbor);
            }

        }

        return retCellList.ToArray();

    }

    /// <summary>
    /// 방문한 cell 과 주변과 이웃한 cell 의 벽을 삭제
    /// </summary>
    /// <param name="c0"></param>
    /// <param name="c1"></param>
    private void ConnectCells(Cell c0, Cell c1)
    {
        // 서로 어떤 방향으로 이어져 있는지 확인.
        Vector2Int dir = c0.index - c1.index;

        // forward
        if(dir.y == -1)         // ex) c0 = (0,0) , c1 = (0,1) 일때 c1.index - c0.index = (0,-1)이 된다. 즉 c0 forward 방향에 이웃한 c1이 존재한다.
        {
            // 벽 비활성화
            c0.isForwardWall = false;                   
            c1.isBackWall = false;
        }

        // back
        if (dir.y == 1)         // ex) c0 = (0,0) , c1 = (0,1) 일때 c1.index - c0.index = (0,-1)이 된다. 즉 c0 forward 방향에 이웃한 c1이 존재한다.
        {
            // 벽 비활성화
            c0.isBackWall = false;
            c1.isForwardWall = false;
        }

        // left
        if (dir.x == 1)         // ex) c0 = (0,0) , c1 = (0,1) 일때 c1.index - c0.index = (0,-1)이 된다. 즉 c0 forward 방향에 이웃한 c1이 존재한다.
        {
            // 벽 비활성화
            c0.isLeftWall = false;
            c1.isRightWall = false;
        }

        // right
        if (dir.x == -1)         // ex) c0 = (0,0) , c1 = (0,1) 일때 c1.index - c0.index = (0,-1)이 된다. 즉 c0 forward 방향에 이웃한 c1이 존재한다.
        {
            // 벽 비활성화
            c0.isRightWall = false;
            c1.isLeftWall = false;
        }

        c0.ShowWalls();
        c1.ShowWalls();
    }

}
