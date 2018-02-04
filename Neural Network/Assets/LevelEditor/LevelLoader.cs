using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour {

    public int width = 100;
    public int height = 100;
    public GameObject tilePrefab;
    public float tileSize = 1;
    [HideInInspector] public string SaveName = "";

    GameObject[] tiles;


	// Use this for initialization
	void Start () {
        generateFullTileMap();
    }
	
	// Update is called once per frame
	void Update () {
        CheckForDrawing();
    }

    //generate a completely filled tile map
    public void generateFullTileMap() {
        clearMap();
        List<GameObject> tiles = new List<GameObject>();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                GameObject obj = Instantiate(tilePrefab);
                tiles.Add(obj);
                obj.transform.position = new Vector3(x * tileSize, 0, y * tileSize);
                obj.transform.SetParent(transform);
            }
        }
        this.tiles = tiles.ToArray();
    }

    //check if anyone is drawing and remove the blocks
    private void CheckForDrawing() {
        if (Input.GetMouseButton(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f)) {
                if (hit.transform.tag == "Editable") {
                    Destroy(hit.transform.gameObject);
                }
            }
        }
    }

    //saves the map to disk
    public void SaveMap() {
        string map = "";
        foreach (GameObject obj in tiles) {
            if (obj != null) {
                map += "1,";
            } else {
                map += "0,";
            }
        }

        FileManager fm = new FileManager();
        fm.SaveData(map, SaveName);
    }

    //loads a map by name
    public void loadMap() {
        string mapString = new FileManager().LoadData(SaveName);
        string[] map = mapString.Split(',');
        int x = 0;
        int y = 0;

        clearMap();
        List<GameObject> tiles = new List<GameObject>();
        foreach (string tile in map) {
            if (tile == "1") {
                GameObject obj = Instantiate(tilePrefab);
                tiles.Add(obj);
                obj.transform.position = new Vector3(x * tileSize, 0, y * tileSize);
                obj.transform.SetParent(transform);
            }
                x++;
            if (x % width == 0) {
                y++;
                x = 0;
            }
        }
        this.tiles = tiles.ToArray();
    }

    //clears the map
    private void clearMap() {
        if (tiles != null) {
            foreach (GameObject obj in tiles) {
                if (obj != null)
                    Destroy(obj);
            }
        }
    }

    //set save file name
    public void SetSaveName(string SaveName) {
        this.SaveName = SaveName;
    }
}
