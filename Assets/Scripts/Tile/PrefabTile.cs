using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Tiles/PrefabTile")]
public class PrefabTile : Tile
{
    public GameObject prefab;  // ����� Prefab
    private static Dictionary<Vector3Int, GameObject> instantiatedPrefabs = new Dictionary<Vector3Int, GameObject>();

    // ��д Tile �ķ�����ʵ�� Prefab ����Ϊ
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);

        // ������λ���Ƿ��Ѿ�ʵ������ Prefab�����������ʵ����
        if (instantiatedPrefabs.ContainsKey(position))
        {
            return;  // �Ѿ�ʵ��������������
        }

        // ͨ�� Tilemap ��������䷽��
        Tilemap map = tilemap.GetComponent<Tilemap>();
        if (map != null)
        {
            Vector3 worldPosition = map.CellToWorld(position);  // ʹ�� Tilemap �� CellToWorld

            // ʵ���� Prefab �����浽�ֵ���
            GameObject instance = GameObject.Instantiate(prefab, worldPosition, Quaternion.identity);
            instantiatedPrefabs[position] = instance;  // ��¼��ʵ����λ�úͶ�Ӧ�� Prefab
        }
        else
        {
            Debug.LogError("Tilemap does not have Tilemap component!");
        }
    }
}
