VR-Maze-Game
===============================

개요
------------------------
#### 프로젝트 환경
개발 환경 : Unity , Oculus VR  
요약 : Oculus VR 을 사용하여 Maze를 탈출하는 게임입니다.

#### 기획
소프트웨어 융합 개론 수업 Term 프로젝트로 제작했던 미로 탈출 게임을 VR로 구현해보았습니다.


#### 요구 사항
1. maze generator 
2. Enemy AI
3. Weapon , Shield
4. VR , Inventory

#### 구성 요소
### Level
1. 게임 실행마다 미로의 구조가 바뀝니다. 출입문을 열고 미로에 입장할 수 있습니다.

<img src="https://user-images.githubusercontent.com/49023743/131736408-27ab6b36-77fb-4b51-812f-b232ed5571e7.PNG" width = "700" height = "500">

<img src="https://user-images.githubusercontent.com/49023743/131736405-ecc2480c-03b5-4219-a8f3-fdec697200de.PNG" width = "700" height = "500">

<img src="https://user-images.githubusercontent.com/49023743/131744368-41feaa65-e33a-40ab-8d67-8c54a33d614e.mp4">


### Player
1. 오른쪽 컨트롤러의 엄지스틱을 사용하여 화면을 회전시킬 수 있습니다.
<img src="https://user-images.githubusercontent.com/49023743/131713105-bc3acfbc-a6d0-4660-8756-f21598824148.mp4">

2. 컨트롤러를 통해 텔레포트와 기본이동을 할 수 있습니다.
<img src="https://user-images.githubusercontent.com/49023743/131715537-812a5e8b-096d-4cde-ac2b-cc2b3272f233.mp4">

3. Trigger 버튼을 통해 무기를 사용할 수 있습니다. 사용하는 무기들은 오른쪽 허벅지, 왼쪽 허벅지 , 왼쪽 어깨 뒤에 인벤토리가 존재하며 무기들을 저장하고 가져올 수 있습니다.
<img src="https://user-images.githubusercontent.com/49023743/131731976-da523144-3c86-411b-b223-f683a5c58a3e.mp4">

<img src="https://user-images.githubusercontent.com/49023743/131731983-752bef6e-df3b-4a0a-9dd7-7bb03ffef8d9.mp4">

<img src="https://user-images.githubusercontent.com/49023743/131732890-cea9c189-d916-470d-9db2-7862896d5440.mp4">


### Enemy

<img src="https://user-images.githubusercontent.com/49023743/131748346-a7f1d996-cec4-4d1f-8113-1e6500f8411a.mp4">


<img src="https://user-images.githubusercontent.com/49023743/131748351-f91600e1-9342-44ae-be47-e5976c932983.mp4">


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
**1. InterFace 스크립트로서 Damaged를 입는 추상 클래스로서 사용된다.**
~~~
void OnDamage(float damageAmount);
~~~

### EnemySpawn.cs
Object pooling을 사용하여 Enemy Prefabs 할당하고 파괴하는 대신, 필요할 때만 Queue에서 GetQueue()함수를  사용하고 , Player Gun에 의해 사라진 개체는 InsertQueue() 함수를 사용하여 Enqueue하였다.

**1. Queue Initialize**
~~~
    void Start()
    {
        instance = this;

        for(int i = 0; i < m_MaxMonster; i++)
        {
            GameObject t_Object = Instantiate(m_MonsterPrefab, this.gameObject.transform);
            m_MonsterQueue.Enqueue(t_Object);
            t_Object.SetActive(false);
        }


        StartCoroutine(CoroutineEnemySpawn());
    }
~~~

**2. Object Pooling**
~~~
    public void InsertQueue(GameObject p_Object)
    {
        m_MonsterQueue.Enqueue(p_Object);
        p_Object.SetActive(false);
    }

    public GameObject GetQueue()
    {
        GameObject t_Object = m_MonsterQueue.Dequeue();
        t_Object.SetActive(true);

        return t_Object;
    }
~~~

**3. CoroutineEnemySpawn()**
~~~
    /// <summary>
    /// 코루틴을 통한 적 생성.
    /// </summary>
    /// <returns></returns>
    IEnumerator CoroutineEnemySpawn()
    {
        while (!isGameOver)
        {
            if (m_CurMonster < m_MaxMonster)
            {
                yield return new WaitForSeconds(m_SpanwTime);
                int idx = Random.Range(0, 5);
                Transform pos = m_SpawnPoints[idx].transform;
                Debug.Log("name : " + pos.name);
                GameObject t_Object = GetQueue();
                t_Object.transform.position = pos.position;
                ++m_CurMonster;
                Debug.Log("Create");
            }
            else
            {
                yield return null;
            }
        }


    }
~~~


### Gun.cs
Player의 Weapon

**1. Fire**
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

**4. Update UI**
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

**5. ReLoad**
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

### Grenade.cs
Player의 Weapon 수류탄

**1. CookGrenade**
~~~
    /// <summary>
    /// 수류탄 Set (카운트 다운 시작) , Trigger 버튼 클릭 후 나타남.
    /// </summary>
    public void CookGrenade()
    {
        // 이미 수류탄이 Cooking 이라면 , 처리를 종료
        if (m_Cooking)
        {
            return;
        }

        m_Cooking = true;
        m_GrenadeText.text = "Cooking";
        // 지연 시간뒤에 Explode를 실행.
        Invoke("Explode",m_TimeToExploade);

    }
~~~

**2. Explode**
~~~
    /// <summary>
    /// 실제 폭발 처리를 하는 부분.
    /// </summary>
    private void Explode()
    {
        // 입력한 position 기준으로 입력한 Radius만큼 반지름을 가진 구를 그린 후 거기에 겹치는 모든 충돌체들을 가져온다.
        // m_TargetLayer을 입력함으로써 설정한 Layer에 해당하는 충돌체들을 가져온다.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius,m_TargetLayer);


        // 감지된 충돌체들중 IDamageable을 가지고 있다면 데미지를 실제로 준다.
        for(int i = 0; i < colliders.Length; i++)
        {
            IDamageable target = colliders[i].GetComponent<IDamageable>();

            if(target != null)
            {
                target.OnDamage(m_Damage);
            }
        }

        // 파티클 효과를 생성 재생
        ParticleSystem explosionEffect = Instantiate(m_ExplosionEffectPrefab, transform.position, transform.rotation);
        explosionEffect.Play();

        // 폭발 소리 재생.
        m_ExplosionAudioSource.clip = m_ExplosionClip;
        m_ExplosionAudioSource.Play();

        // ParticleSystem.main.duration 파티클이 가지고 있는 지속 유지시간. 후 파괴
        Destroy(explosionEffect.gameObject, explosionEffect.main.duration);

        // 수류탄 자시 자신을 파괴
        Destroy(gameObject);
    }
~~~


### EnemyAi.cs , MoveAgent.cs 
Enemy의 Navmesh를 이용한 움직임 구현

**1. Enemy State 관리 (Trace , Patrol, Attack , Die)**
~~~
    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());
    }

    IEnumerator Action()
    {
        // 적 캐릭터가 사망할 때까지 무한 루프.
        while (!m_IsDie)
        {
            yield return m_WaitSecond;

            switch (state)
            {
                case State.PATROL:
                    // 총알 발사 정지
                    m_EnemyFire.isFire = false;
                    m_MoveAgent.patrolling = true;
                    m_Animator.SetBool(hashMove, true);
                    break;

                case State.TRACE:
                    m_EnemyFire.isFire = false;
                    m_MoveAgent.TraceTarget = m_PlayerTr.position;
                    m_Animator.SetBool(hashMove, false);
                    m_Animator.SetBool(hashRun, true);
                    break;

                case State.ATTACK:
                    m_MoveAgent.Stop();
                    m_Animator.SetBool(hashMove, false);
                    m_Animator.SetBool(hashRun, false);
                    if (m_EnemyFire.isFire == false) m_EnemyFire.isFire = true;
                    break;

                case State.DIE:
                    m_MoveAgent.Stop();
                    m_EnemyFire.isFire = false;
                    break;
            }
        }
    }

    IEnumerator CheckState()
    {
        while (!m_IsDie)
        {
            if (state == State.DIE) yield break;

            float dis = Vector3.Distance(m_PlayerTr.position, m_EnemyTr.position);

            if (dis <= m_AttackDis)
            {
                RaycastHit hit;
                Debug.DrawRay(m_FireTransform.position, m_FireTransform.forward * m_AttackDis, Color.blue, 0.3f);
                
                if(Physics.Raycast(m_FireTransform.position,m_FireTransform.forward, out hit, m_AttackDis))
                {
                    state = State.ATTACK;

                }          
            }
            else if (dis <= m_TraceDis)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }

            yield return m_WaitSecond;
        }
    }

~~~


**2. Property**
~~~
    // Patrolling 프로퍼티 정의
    public bool patrolling
    {
        get { return m_Patolling; }
        set
        {
            m_Patolling = value;
            if (m_Patolling)
            {
                m_Agent.speed = m_PatrollerSpeed;
                // 순찰 상태의 회전계수
                m_Damping = 1.0f;
                MoveWayPoint();
            }
        }
    }
    
        public Vector3 TraceTarget
    {
        get { return m_TraceTarget; }
        set
        {
            m_TraceTarget = value;
            m_Agent.speed = m_TraceSpeed;
            // 추적 상태의 회전 계수.
            m_Damping = 7.0f;
            TraceTargetFunction(m_TraceTarget);
        }
    }

    public float speed
    {
        get { return m_Agent.velocity.magnitude;  }
    }




    void TraceTargetFunction(Vector3 pos)
    {
        if (m_Agent.isPathStale) return;

        m_Agent.destination = pos;
        m_Agent.isStopped = false;
    }
~~~


## Epilogue

**1. 프로젝트를 진행하면서 어려웠 던 점**  
VR 프로젝트를 혼자 진행하면서 기획부터 동영상 편집을 통해 게임 소개 영상까지 모든걸 혼자하려다 보니 전체적인 프로젝트 퀄리티가 떨어졌습니다.  
처음 아이디어를 기획하며 문서를 작성하고 프로젝트 개발을 시작하게 되는 과정이 혼자서 진행하다 보니 문서화를 하지 않고 그때 그때 넣고 싶은 요소들을 추가하다 보니 프로젝트의 방향성이 점점 멀어지게 되며 프로젝트가 의도를 알 수 없게 되었습니다.
또한 Oculus를 사용한 프로젝트를 하다보니 Unity 와 Oculus를 연동하는걸 시작으로 모든것이 튜토리얼 영상을 보고 제작하며 필요한 기능들을 단순히 익히며 하나둘씩 저의 프로젝트에 적용하면서 많은 시간이 소요되었습니다.  

**2. 깨달은 점**  
혼자서 프로젝트를 진행하게 된다면 떠오르는 아이디어를 추가 할 수 있습니다. 하지만 이것은 프로젝트의 방향성을 쉽게 잃게 될 수 있다는 것을 알게 되었습니다. 프로젝트의 방향성을 잃어버리지 않기 위해 기획 즉 문서화된 프로젝트가 존재해야 한다는 것을 알 수 있었습니다.
