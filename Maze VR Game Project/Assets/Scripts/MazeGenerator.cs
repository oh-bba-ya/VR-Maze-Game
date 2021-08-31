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


    // ���� �ִ� ���
    

    // Start is called before the first frame update
    void Start()
    {
        BatchCells();
        MakeMaze(cellMap[0,0]);

        // ó���� ���� �Ա� �ⱸ �����.
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
                _cell.transform.localPosition = new Vector3(x * 5, 0, y * 5);                                   // Maze �θ� ��ü ��ġ�� ����ؼ� ����.

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

            // ����Լ�
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
    /// �ֺ�(���� cell �� �����¿�)�� Ž���ϴ� �Լ�.
    /// </summary>
    /// <param name="cell"> ���� </param>
    /// <returns></returns>
    private Cell[] GetNeighborCells(Cell cell)
    {
        List<Cell> retCellList = new List<Cell>();
        Vector2Int index = cell.index;

        // forward
        // ���� 5 x 5 ���� �����Ѵٸ� index[i,j] =  �� 0~4 ������ ���������� �Էµȴ�. , �׷��Ƿ� �ε��� ������ Cell =  4�϶� �ֺ� Cell�� �����ϱ� ���ؼ��� + 1�ؼ� ó�� ������ 5 x 5 �� Height = 5 ���� �۾ƾ� �������� �����Ѵٰ� �� �� �ִ�.
        if(index.y +1 < height)
        {
            // �� �������� �Է¹��� cell �� �տ� �ִ�  cell�� ����.
            Cell neighbor = cellMap[index.x, index.y + 1];
            if (neighbor.CheckAllWall())
            {
                retCellList.Add(neighbor);
            }
            
        }

        // back
        if (index.y - 1 >= 0)
        {
            // �� �������� �Է¹��� cell �� �տ� �ִ�  cell�� ����.
            Cell neighbor = cellMap[index.x, index.y - 1];
            if (neighbor.CheckAllWall())
            {
                retCellList.Add(neighbor);
            }

        }

        // left
        if (index.x -  1 >= 0)
        {
            // �� �������� �Է¹��� cell �� �տ� �ִ�  cell�� ����.
            Cell neighbor = cellMap[index.x -1, index.y];
            if (neighbor.CheckAllWall())
            {
                retCellList.Add(neighbor);
            }

        }

        // right
        if (index.x + 1 < width)
        {
            // �� �������� �Է¹��� cell �� �տ� �ִ�  cell�� ����.
            Cell neighbor = cellMap[index.x +1, index.y];
            if (neighbor.CheckAllWall())
            {
                retCellList.Add(neighbor);
            }

        }

        return retCellList.ToArray();

    }

    /// <summary>
    /// �湮�� cell �� �ֺ��� �̿��� cell �� ���� ����
    /// </summary>
    /// <param name="c0"></param>
    /// <param name="c1"></param>
    private void ConnectCells(Cell c0, Cell c1)
    {
        // ���� � �������� �̾��� �ִ��� Ȯ��.
        Vector2Int dir = c0.index - c1.index;

        // forward
        if(dir.y == -1)         // ex) c0 = (0,0) , c1 = (0,1) �϶� c1.index - c0.index = (0,-1)�� �ȴ�. �� c0 forward ���⿡ �̿��� c1�� �����Ѵ�.
        {
            // �� ��Ȱ��ȭ
            c0.isForwardWall = false;                   
            c1.isBackWall = false;
        }

        // back
        if (dir.y == 1)         // ex) c0 = (0,0) , c1 = (0,1) �϶� c1.index - c0.index = (0,-1)�� �ȴ�. �� c0 forward ���⿡ �̿��� c1�� �����Ѵ�.
        {
            // �� ��Ȱ��ȭ
            c0.isBackWall = false;
            c1.isForwardWall = false;
        }

        // left
        if (dir.x == 1)         // ex) c0 = (0,0) , c1 = (0,1) �϶� c1.index - c0.index = (0,-1)�� �ȴ�. �� c0 forward ���⿡ �̿��� c1�� �����Ѵ�.
        {
            // �� ��Ȱ��ȭ
            c0.isLeftWall = false;
            c1.isRightWall = false;
        }

        // right
        if (dir.x == -1)         // ex) c0 = (0,0) , c1 = (0,1) �϶� c1.index - c0.index = (0,-1)�� �ȴ�. �� c0 forward ���⿡ �̿��� c1�� �����Ѵ�.
        {
            // �� ��Ȱ��ȭ
            c0.isRightWall = false;
            c1.isLeftWall = false;
        }

        c0.ShowWalls();
        c1.ShowWalls();
    }

}
