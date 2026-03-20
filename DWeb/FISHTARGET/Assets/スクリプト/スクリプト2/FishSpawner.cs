using UnityEngine;
using System.Collections.Generic;

public class FishSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject blackFishPrefab;      // 黒い魚（ハズレ）
    public GameObject[] normalFishPrefabs; // 当たり魚のリスト（重複なし用）

    [Header("Spawn Area")]
    public Vector2 spawnRangeX = new Vector2(-2, 2);
    public Vector2 spawnRangeY = new Vector2(-4, 4);

    void Start()
    {
        SpawnLevelFish();
    }

    void SpawnLevelFish()
    {
        int level = GameDirector.currentLevel;

        // ★ 黒い魚を1匹配置
        SpawnFish(blackFishPrefab);

        // ★ レベルに応じた数の当たり魚を配置（レベル1なら1匹、合計2匹）
        // 重複を避けるためにリストをシャッフルして選ぶ
        List<GameObject> fishPool = new List<GameObject>(normalFishPrefabs);
        ShuffleList(fishPool);

        for (int i = 0; i < level; i++)
        {
            if (i < fishPool.Count)
            {
                SpawnFish(fishPool[i]);
            }
        }
    }

    void SpawnFish(GameObject prefab)
    {
        Vector3 pos = new Vector3(
            Random.Range(spawnRangeX.x, spawnRangeX.y),
            Random.Range(spawnRangeY.x, spawnRangeY.y),
            0
        );
        Instantiate(prefab, pos, Quaternion.identity);
    }

    // リストをシャッフルするアルゴリズム
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
