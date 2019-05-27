using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnBomb : MonoBehaviour
{
    [SerializeField]
    private Tilemap tileMap;
    [SerializeField]
    private GameObject bombPrefab;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = tileMap.WorldToCell(worldMousePos);
            // print(cell);
            Vector3 pos = tileMap.GetCellCenterWorld(cell);

            Instantiate(bombPrefab, pos, Quaternion.identity);
        }
    }
}
