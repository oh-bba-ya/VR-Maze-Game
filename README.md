VR-Maze-Game
===============================

개요
------------------------
#### 프로젝트 환경
개발 환경 : Unity , Oculus VR
요약 : Oculus VR 을 사용하여 Maze를 탈출하는 게임입니다.

#### 기획
소프트웨어 융합 개론 수업 Term 프로젝트로 제작했던 미로 탈출 게임을 VR로 구현해보자


#### 요구 사항
1. maze generator 
2. Enemy AI
3. Weapon , Shield
4. VR , Inventory


## 클래스
### Cell.cs

1. ShowWalls
~~~
    public void ShowWalls()
    {
        forwardWall.SetActive(isForwardWall);
        BackWall.SetActive(isBackWall);
        leftWall.SetActive(isLeftWall);
        rightWall.SetActive(isRightWall);
    }
~~~

2. CheckAllWall
~~~
    /// <summary>
    /// 모든 4면이 활성화 되어 있는지 체크.
    /// </summary>
    /// <returns></returns>
    public bool CheckAllWall()
    {
        return isForwardWall && isBackWall && isRightWall && isLeftWall;
    }
~~~

### Maze generator.cs
#### 설명
1. 게임 실행시 자동적으로 미로가 생성되며 미로 형태는 랜덤으로 생성됩니다.

#### Code
**1. BatchCells**
~~~
/// <summary>
/// 입력받은 Width , height 만큼 Cell 생성.
/// </summary>
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
            // Maze 부모 객체 위치에 기반해서 생성.  
            _cell.transform.localPosition = new Vector3(x * 5, 0, y * 5);                     
  
            cellMap[x, y] = _cell;  
                
        }  
    }  
}  
~~~
  
**2. MakeMaze :**
~~~
    /// <summary>
    /// startCell 상하좌우 이웃 Cell들을 배열로 입력받아 배열 범위만큼의 랜덤하게 선택하여 Cell을 연결한다.
    /// </summary>
    /// <param name="startCell"> 입력받은 Cell 시작 </param>
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
            // Stack을 사용하여 놓친 Cell들이 없도록 한다.
            if(cellHisttoryStack.Count > 0)
            {
                Cell lastCell = cellHisttoryStack.Pop();
                MakeMaze(lastCell);
            }
        }
    }
~~~
  
  **3. GetNeighborCells**
~~~
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
 ~~~
  
**4. ConnectCells**
~~~
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
        if (dir.y == 1)         
        {
            // 벽 비활성화
            c0.isBackWall = false;
            c1.isForwardWall = false;
        }

        // left
        if (dir.x == 1)         
        {
            // 벽 비활성화
            c0.isLeftWall = false;
            c1.isRightWall = false;
        }

        // right
        if (dir.x == -1)         
        {
            // 벽 비활성화
            c0.isRightWall = false;
            c1.isLeftWall = false;
        }

        c0.ShowWalls();
        c1.ShowWalls();
    }                                
~~~
------------------------
### Gun.cs

**1.  Fire**
~~~
    /// <summary>
    /// 발사 처리를 시도하는 함수.  
    /// </summary>
    public void Fire()
    {
        // 총이 준비된 상태 AND 현재 시간 >= 마지막 발사 시점 + 연사 간격
        if(m_CurrentState == State.Ready && Time.time >= m_LastFireTime + m_TimeBetFire)
        {
            m_LastFireTime = Time.time;             // 마지막으로 총은 쏜 시점이 현재 시점으로 갱신

            Shot();
            UpdateUI();
        }

        if(m_CurrentAmmo == 0)
        {
            Reload();
        }
    }
~~~

**2. Shot**
~~~
    /// <summary>
    /// 실제 발사 처리를 하는 부분.
    /// </summary>
    public void Shot()
    {
        RaycastHit hit;         // 레이 캐스트 정보를 저장하는, 충돌 정보 컨테이너

        // 총을 쏴서 총알이 맞은 곳 : 총구 위치 + 총구 위치 앞쪽 방향 * 사정 거리
        Vector3 hitPosition = m_FireTransform.position + m_FireTransform.forward * m_FireDistance;

        // 레이캐스트(시작지점,방향, 충돌 정보 컨테이너, 사정거리)
        if(Physics.Raycast(m_FireTransform.position, m_FireTransform.forward, out hit, m_FireDistance))             // 어떠한 물체와 충돌하게 되면 true값 리턴.
        {
            // 상대방의 컴포넌트 IDamageable을 가져온다. 없다면 null값이 들어간다.
            IDamageable target = hit.collider.GetComponent<IDamageable>();

            // 상대방이 IDamageable이 존재한다면.
            if (target != null)
            {
                target.OnDamage(m_Damage);
            }

            
        }

        // 발사 이펙트 재생 시작
        StartCoroutine(ShotEffect(hitPosition));

        m_CurrentAmmo--;

        if(m_CurrentAmmo <= 0)
        {
            m_CurrentState = State.Empty;
        }

    }
~~~

**3. ShotEffect**
~~~
    /// <summary>
    /// 발사 이펙트를 재생하고 총알 궤적을 잠시 그렸다가 끄는 함수.
    /// </summary>
    /// <param name="hitPosition"></param>
    /// <returns></returns>
    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        m_Animator.SetTrigger("Fire");          // Fire 트리거를 당김.

        // 총알 궤적 렌더러를 켬
        m_BulletLineRenderer.enabled = true;

        // 선분의 첫번째 점은 총구의 위치.
        m_BulletLineRenderer.SetPosition(0, m_FireTransform.position);

        // 선분의 두번쨰 점 위치는 충돌한 곳.
        m_BulletLineRenderer.SetPosition(1, hitPosition);

        // 총구 화염 이펙트를 재생
        m_MuzzleFlashEffect.Play();

        // 현재 들어가 있는 소리가 발사 소리가 아니라면.
        if(m_GunAudioPlayer.clip != m_ShotClip)
        {
            m_GunAudioPlayer.clip = m_ShotClip;         // 총 발사 소리 넣기.
        }
        

        // 총격 소리 재생
        m_GunAudioPlayer.Play();

        yield return new WaitForSeconds(0.07f);         // 처리를 '잠시' 쉬는 시간.

        // 0.07초 후 실행됌
        m_BulletLineRenderer.enabled = false;

    }
~~~

**4. UpdateUI**
~~~
    /// <summary>
    /// 총의 탄약 UI에 남은 탄약을 갱신해준다.
    /// </summary>
    private void UpdateUI()
    {
        if(m_CurrentState == State.Empty)
        {
            m_AmmoText.text = "Empty";
        }
        else if(m_CurrentState == State.Reloading)
        {
            m_AmmoText.text = "Reloading";
        }
        else
        {
            m_AmmoText.text = m_CurrentAmmo.ToString();
        }
    }
~~~

**5. Reload**
~~~
    /// <summary>
    /// 재장전을 시도.
    /// </summary>
    public void Reload()
    {
        if(m_CurrentState != State.Reloading )
        {
            StartCoroutine(ReloadRoutine());
        }
    }
~~~

**6. ReloadRoutine**
~~~
    /// <summary>
    /// 실제 재장전 처리가 진행되는 곳.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReloadRoutine()
    {
        m_Animator.SetTrigger("Reloading");            // Reload 파라미터 트리거를 당김.
        m_CurrentState = State.Reloading;

        m_GunAudioPlayer.clip = m_ReloadClip;           // 재장전 소리 삽입.

        m_GunAudioPlayer.Play();            // 소리 재생.

        UpdateUI();

        yield return new WaitForSeconds(m_ReloadTime);              // 재장전 시간 만큼 쉰다.

        m_CurrentAmmo = m_MaxAmmo;         // 탄약 최대 충전.
        m_CurrentState = State.Ready;
        UpdateUI();

    }
~~~

### IDamageable.cs
1. InterFace 스크립트로서 Damaged를 입는 추상 클래스로서 사용된다.
~~~
void OnDamage(float damageAmount);
~~~
