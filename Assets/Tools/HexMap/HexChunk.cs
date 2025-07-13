using UnityEditor;
using UnityEngine;

public class HexChunk : MonoBehaviour
{
    public void GenerateChunk(int chunkSizeX, int chunkSizeZ, Vector2Int startCoords, GameObject tilePrefab)
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Tile Prefab is missing.");
            return;
        }

        // Clean any previous children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        for (int z = 0; z < chunkSizeZ; z++)
        {
            for (int x = 0; x < chunkSizeX; x++)
            {
                int cellX = startCoords.x + x;
                int cellZ = startCoords.y + z;

                float xPos = (cellX + cellZ * 0.5f - cellZ / 2) * (HexMetrics.innerRadius * 2f);
                float zPos = cellZ * (HexMetrics.outerRadius * 1.5f);
                Vector3 position = new Vector3(xPos, 0, zPos);

                GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab);
                tile.transform.SetParent(transform);
                tile.transform.localPosition = position;
                tile.name = $"Hex_{cellX}_{cellZ}";
            }
        }
    }
}
