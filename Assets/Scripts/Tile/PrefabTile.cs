using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Tiles/PrefabTile")]
public class PrefabTile : Tile
{
    public GameObject prefab;  // 这里放 Prefab
    private static Dictionary<Vector3Int, GameObject> instantiatedPrefabs = new Dictionary<Vector3Int, GameObject>();

    // 覆写 Tile 的方法来实现 Prefab 的行为
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);

        // 检查这个位置是否已经实例化了 Prefab，如果有则不再实例化
        if (instantiatedPrefabs.ContainsKey(position))
        {
            return;  // 已经实例化，不再生成
        }

        // 通过 Tilemap 组件访问其方法
        Tilemap map = tilemap.GetComponent<Tilemap>();
        if (map != null)
        {
            Vector3 worldPosition = map.CellToWorld(position);  // 使用 Tilemap 的 CellToWorld

            // 实例化 Prefab 并保存到字典中
            GameObject instance = GameObject.Instantiate(prefab, worldPosition, Quaternion.identity);
            instantiatedPrefabs[position] = instance;  // 记录已实例化位置和对应的 Prefab
        }
        else
        {
            Debug.LogError("Tilemap does not have Tilemap component!");
        }
    }
}
