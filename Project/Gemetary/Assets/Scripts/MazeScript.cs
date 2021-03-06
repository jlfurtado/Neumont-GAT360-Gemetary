﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeScript : MonoBehaviour {
    public GameObject ChaseEnemyPrefab;
    public GameObject MazeSectorPrefab;
    public GameObject PillarPrefab;
    public GameObject FencePrefab;
    public GameObject GemPrefab;
    public GameObject FloorPrefab;
    public GameObject PowerupPrefab;
    public GameObject FollowEnemyPrefab;
    public GameObject DepthEnemyPrefab;
    public GameObject RestorerPrefab;
    public GameObject BombPrefab;
    public GameObject LoadingFogPrefab;
    public Material[] GemColors;
    public Material[] FloorColors;
    public int MaxEnemiesPerSection;
    public int TombstonesPerSection;

    public int SectionSize;
    public float SquareSize;
    public int RenderDistance;

    private int genTiles;
    private int numSections;
    private int totalTiles;
    private float mazeSize;
    private Dictionary<string, MazeSectionGenerator> generatedMazes = new Dictionary<string, MazeSectionGenerator>(10000);
    private GameObject[] pillarPool;
    private GameObject[] fencePool;
    private GameObject[] fogPool;
    private GameObject[] restorerPool;
    private EatForPoints[] gemPool;
    private Enemy[] enemyPool;
    private Renderer[] floorPool;
    private Powerup[] powerupPool;
    private Bomb[] bombPool;

    public GameObject FogHolder { get; private set; }
    public GameObject PillarHolder { get; private set; }
    public GameObject FenceHolder { get; private set; }
    public GameObject GemHolder { get; private set; }
    public GameObject FloorHolder { get; private set; }
    public GameObject PowerupHolder { get; private set; }
    public GameObject EnemyHolder { get; private set; }
    public GameObject RestorerHolder { get; private set; }
    public GameObject BombHolder { get; private set; }
    private IVec2 lastSection;
    private System.Random rand = new System.Random();
    private int sideLength;
    private int maxEnemies;
    private int numBombs;


    private string ToKey(int x, int z)
    {
        return string.Concat(Strings.KEY_PREFIX, x, Strings.KEY_MIDDLE, z);
    }

    private string ToKey(IVec2 pos)
    {
        return ToKey(pos.x, pos.z);
    }

    private GameObject Parent(GameObject obj, GameObject parent)
    {
        obj.transform.parent = parent.transform;
        return obj;
    }

    // Use this for initialization
    void Awake ()
    {
        if (ScoreManager.IsHardcore()) { MaxEnemiesPerSection *= 2; }

        MazeSectionGenerator.Size = SectionSize;
        MazeSectionGenerator.SquareSize = SquareSize;
        MazeSectionGenerator.BombsPerSection = TombstonesPerSection;

        sideLength = 1 + 2 * RenderDistance;
        numSections = sideLength * sideLength;
        genTiles = SectionSize * SectionSize;
        totalTiles = genTiles * numSections;
        mazeSize = SectionSize * SquareSize;

        Parent(FloorHolder = new GameObject(), this.gameObject).name = Strings.FLOOR_HOLDER_NAME;
        Parent(PowerupHolder = new GameObject(), this.gameObject).name = Strings.POWERUP_HOLDER_NAME;
        Parent(FenceHolder = new GameObject(), this.gameObject).name = Strings.FENCE_HOLDER_NAME;
        Parent(PillarHolder = new GameObject(), this.gameObject).name = Strings.PILLAR_HOLDER_NAME;
        Parent(GemHolder = new GameObject(), this.gameObject).name = Strings.GEM_HOLDER_NAME;
        Parent(EnemyHolder = new GameObject(), this.gameObject).name = Strings.ENEMY_HOLDER_NAME;
        Parent(RestorerHolder = new GameObject(), this.gameObject).name = Strings.RESTORE_HOLDER_NAME;
        Parent(BombHolder = new GameObject(), this.gameObject).name = Strings.BOMB_HOLDER_NAME;
        Parent(FogHolder = new GameObject(), this.gameObject).name = Strings.FOG_HOLDER_NAME;

        maxEnemies = numSections * MaxEnemiesPerSection;
        enemyPool = new Enemy[maxEnemies];
        for (int i = 0; i < maxEnemies; ++i)
        {
            GameObject obj = i % 3 == 0 ? FollowEnemyPrefab : i % 3 == 1 ? DepthEnemyPrefab : ChaseEnemyPrefab;
            Parent((enemyPool[i] = Instantiate(obj).GetComponent<Enemy>()).gameObject, EnemyHolder);
        }

        numBombs = numSections * TombstonesPerSection;
        bombPool = new Bomb[numBombs];
        for (int i = 0; i < numBombs; ++i)
        {
            Parent((bombPool[i] = Instantiate(BombPrefab).GetComponent<Bomb>()).gameObject, BombHolder);
        }

        restorerPool = new GameObject[numSections];
        floorPool = new Renderer[numSections];
        powerupPool = new Powerup[numSections];
        fogPool = new GameObject[numSections];
        for (int i = 0; i < numSections; ++i)
        {
            Parent((powerupPool[i] = Instantiate(PowerupPrefab).GetComponent<Powerup>()).gameObject, PowerupHolder);
            Parent((floorPool[i] = Instantiate(FloorPrefab).GetComponent<Renderer>()).gameObject, FloorHolder);
            Parent(fogPool[i] = Instantiate(LoadingFogPrefab), FogHolder);
            Parent(restorerPool[i] = Instantiate(RestorerPrefab), RestorerHolder);
        }

        fencePool = new GameObject[totalTiles];
        pillarPool = new GameObject[totalTiles];
        gemPool = new EatForPoints[totalTiles];
        FenceHolder.transform.position = Vector3.down * 10.0f;
        PillarHolder.transform.position = Vector3.down * 10.0f;
        GemHolder.transform.position = Vector3.down * 10.0f;
        for (int i = 0; i < totalTiles; ++i)
        {
            Parent(fencePool[i] = Instantiate(FencePrefab), FenceHolder).name = Strings.FENCE_NAME;
            Parent(pillarPool[i] = Instantiate(PillarPrefab), PillarHolder).name = Strings.PILLAR_NAME;
            Parent((gemPool[i] = Instantiate(GemPrefab).GetComponent<EatForPoints>()).gameObject, GemHolder).name = Strings.GEM_NAME;
        }
	}

    void Start()
    {
        for (int i = 0; i < maxEnemies; ++i)
        {
            enemyPool[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < numBombs; ++i)
        {
            bombPool[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < numSections; ++i)
        {
            floorPool[i].gameObject.SetActive(false);
            powerupPool[i].gameObject.SetActive(false);
            restorerPool[i].SetActive(false);
            fogPool[i].SetActive(false);
        }

        for (int i = 0; i < totalTiles; ++i)
        {
            fencePool[i].SetActive(false);
            pillarPool[i].SetActive(false);
            gemPool[i].gameObject.SetActive(false);
        }

        int halfSide = (sideLength - 1) / 2;
        for (int i = -halfSide; i <= halfSide; ++i)
        {
            for (int j = -halfSide; j <= halfSide; ++j)
            {
                Enable(i, j, true);
            }
        }

        lastSection = new IVec2(0, 0);

        GenerateAround(Vector3.zero);
    }

    public void EatAt(IVec2 mazeLoc, IVec2 sectionLoc)
    {
        generatedMazes[ToKey(mazeLoc)].EatAt(sectionLoc.x, sectionLoc.z);
    }

    public void EatAt(Vector3 pos)
    {
        int halfSize = SectionSize / 2;
        float mhs = mazeSize / 2;
        IVec2 mazeLoc = new IVec2((int)Mathf.Floor((pos.x + mhs) / mazeSize), (int)Mathf.Floor((pos.z + mhs) / mazeSize));
        IVec2 sectionLoc = new IVec2((int)Mathf.Floor((pos.x - (mazeLoc.x * mazeSize)) / SquareSize) + (halfSize), (int)Mathf.Floor((pos.z - (mazeLoc.z * mazeSize)) / SquareSize) + (halfSize));
        EatAt(mazeLoc, sectionLoc);
    }

    public MazeSectionGenerator SectionAt(Vector3 pos)
    {
        float mhs = mazeSize / 2;
        IVec2 mazeLoc = new IVec2((int)Mathf.Floor((pos.x + mhs) / mazeSize), (int)Mathf.Floor((pos.z + mhs) / mazeSize));
        return generatedMazes[ToKey(mazeLoc)];
    }

    public IVec2 SectionLocFor(Vector3 pos)
    {
        int halfSize = SectionSize / 2;
        float mhs = mazeSize / 2;
        IVec2 mazeLoc = new IVec2((int)Mathf.Floor((pos.x + mhs) / mazeSize), (int)Mathf.Floor((pos.z + mhs) / mazeSize));
        return new IVec2((int)Mathf.Floor((pos.x - (mazeLoc.x * mazeSize)) / SquareSize) + (halfSize), (int)Mathf.Floor((pos.z - (mazeLoc.z * mazeSize)) / SquareSize) + (halfSize));
    }

    private int Mod(int n, int m)
    {
        return ((n % m) + m) % m;
    }

    private int MagicMath(int x, int z)
    {
        return Mod(x, sideLength) * sideLength + Mod(z, sideLength);
    }

    public void GenerateAround(Vector3 position)
    {
        GenerateNear(position);
    }

    private void GenerateNear(Vector3 position)
    {
        IVec2 currentSection = new IVec2((int)(Mathf.Floor((position.x + (mazeSize / 2)) / mazeSize)),
                                        (int)(Mathf.Floor((position.z + (mazeSize / 2)) / mazeSize)));

        if (!lastSection.Equals(currentSection))
        {
            if (currentSection.x > lastSection.x)
            {
                // move all the old left ones to the new right
                for (int i = 0; i < sideLength; ++i)
                {
                    Disable(lastSection.x - RenderDistance, i + lastSection.z - RenderDistance);
                    Enable(currentSection.x + RenderDistance, i + currentSection.z - RenderDistance, false);
                }
            }
            else if (currentSection.x < lastSection.x)
            {
                // move all the old right ones to the new left
                for (int i = 0; i < sideLength; ++i)
                {
                    Disable(lastSection.x + RenderDistance, i + lastSection.z - RenderDistance);
                    Enable(currentSection.x - RenderDistance, i + currentSection.z - RenderDistance, false);
                }
            }

            if (currentSection.z > lastSection.z)
            {
                // move all the top old ones to the new bottom
                for (int i = 0; i < sideLength; ++i)
                {
                    Disable(lastSection.x - RenderDistance + i, lastSection.z - RenderDistance);
                    Enable(currentSection.x - RenderDistance + i, currentSection.z + RenderDistance, false);
                }
            }
            else if (currentSection.z < lastSection.z)
            {
                // move all the bottom old ones to the new top
                for (int i = 0; i < sideLength; ++i)
                {
                    Disable(lastSection.x - RenderDistance + i, lastSection.z + RenderDistance);
                    Enable(currentSection.x - RenderDistance + i, currentSection.z - RenderDistance, false);
                }
            }
        }

        lastSection = currentSection;
    }

    private void Disable(int x, int z)
    {
        string mazeKey = ToKey(x, z);
        int idx = MagicMath(x, z);
        if (!generatedMazes.ContainsKey(mazeKey)) { return; }

        generatedMazes[mazeKey].gameObject.SetActive(false);

        for (int k = 0; k < genTiles; ++k)
        {
            int w = idx * genTiles + k;
            Parent(fencePool[w], PillarHolder);
            Parent(pillarPool[w], FenceHolder);
            Parent(gemPool[w].gameObject, GemHolder);
        }
    }

    private void Enable(int x, int z, bool sync)
    {
        string mazeKey = ToKey(x, z);
        int idx = MagicMath(x, z);
        bool newMaze = !generatedMazes.ContainsKey(mazeKey);
        if (newMaze) { generatedMazes.Add(mazeKey, GenerateMazeSection(x, z)); }
        
        MazeSectionGenerator gen = generatedMazes[mazeKey];
        gen.gameObject.SetActive(true);
        gen.gameObject.transform.localPosition = new Vector3(x * mazeSize, 0.0f, z * mazeSize);
        if (newMaze) { gen.GenerateMaze(rand.Next()); }
        gen.CalcDiff(new IVec2(x, z));
        SetMazeParams(gen, idx);

        gen.RedoGeometry(x, z, sync);
    }

    private void SetMazeParams(MazeSectionGenerator gen, int idx)
    {
        gen.FloorPool.start = idx;
        gen.FloorPool.count = 1;

        gen.RestorerPool.start = idx;
        gen.RestorerPool.count = 1;

        gen.FogPool.start = idx;
        gen.FogPool.count = 1;

        gen.PowerupPool.start = idx;
        gen.PowerupPool.count = 1;

        gen.PillarPool.start = idx * genTiles;
        gen.PillarPool.count = genTiles;

        gen.FencePool.start = idx * genTiles;
        gen.FencePool.count = genTiles;

        gen.GemPool.start = idx * genTiles;
        gen.GemPool.count = genTiles;

        gen.EnemyPool.start = idx * MaxEnemiesPerSection;
        gen.EnemyPool.count = Mathf.Clamp(Mathf.FloorToInt(Mathf.Pow(gen.Difficulty(), 1.2f)), 1, MaxEnemiesPerSection); // TODO TWEAK SCALING

        gen.BombPool.start = idx * TombstonesPerSection;
        gen.BombPool.count = TombstonesPerSection;
    }

    private MazeSectionGenerator GenerateMazeSection(int x, int z)
    {    
        GameObject obj = Parent(Instantiate(MazeSectorPrefab), this.gameObject);
        obj.name = string.Concat(Strings.MAZE_SECTION_NAME_PREFIX, x, Strings.COMMA, z, Strings.MAZE_SECTION_NAME_POSTFIX);
        MazeSectionGenerator gen = obj.GetComponent<MazeSectionGenerator>();
        gen.RestorerPool = new RefArray<GameObject>(restorerPool, 0, 0);
        gen.FloorPool = new RefArray<Renderer>(floorPool, 0, 0);
        gen.PowerupPool = new RefArray<Powerup>(powerupPool, 0, 0);
        gen.PillarPool = new RefArray<GameObject>(pillarPool, 0, 0);
        gen.FencePool = new RefArray<GameObject>(fencePool, 0, 0);
        gen.GemPool = new RefArray<EatForPoints>(gemPool, 0, 0);
        gen.EnemyPool = new RefArray<Enemy>(enemyPool, 0, 0);
        gen.BombPool = new RefArray<Bomb>(bombPool, 0, 0);
        gen.FogPool = new RefArray<GameObject>(fogPool, 0, 0);
        gen.GemMat = GemColors[Mod(x - z, GemColors.Length)];
        gen.FloorMat = FloorColors[Mod(x - z, FloorColors.Length)];
        return gen;
    }
}
