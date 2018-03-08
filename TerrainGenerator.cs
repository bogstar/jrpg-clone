using UnityEngine;
using System.Collections;

public static class TerrainGenerator {

    public static Tile[,] GenerateTerrain(TerrainData data, Transform gridT) {

        Hex hex = data.hex;

        Tile[,] grid = new Tile[data.gridX, data.gridY];

        int x;
        int y;

        for(y = 0; y < data.gridY; y++) {
            for(x = 0; x < data.gridX; x++) {

                GameObject tileGO = (GameObject)GameObject.Instantiate(GridManager.instance.tilePrefab, gridT.Find("Tiles"));
                tileGO.name = x + " " + y;
                Tile tile = tileGO.GetComponent<Tile>();

                float cx;
                float cy;

                if(hex.orientation == Hex.HexagonOrientation.PointyTopped) {

                    cx = x * (hex.width + data.offset);
                    cy = y * (hex.height * .75f + data.offset);

                    if(y % 2 != 0) {
                        if(hex.oddity == Hex.HexagonOddity.Odd)
                            cx += hex.width * .5f + data.offset;
                        else
                            cx -= hex.width * .5f + data.offset;
                    }
                }
                else {

                    tile.graphics.transform.localEulerAngles = new Vector3(tile.graphics.transform.localEulerAngles.x, 30f, tile.graphics.transform.localEulerAngles.z);

                    cx = x * (hex.width * .75f + data.offset);
                    cy = y * (hex.height + data.offset);

                    if(x % 2 != 0) {
                        if(hex.oddity == Hex.HexagonOddity.Odd)
                            cy += hex.height * .5f + data.offset;
                        else
                            cy -= hex.height * .5f + data.offset;
                    }
                }

                tileGO.transform.localPosition = new Vector3(cx, 0f, cy);
                grid[x, y] = tile.GetComponent<Tile>();
                grid[x, y] = tile;
                grid[x, y].SetEmpty();
                grid[x, y].position = new Tile.TilePosition(x, y);
                grid[x, y].text.text = "";
            }
        }

        return grid;
    }

    public static void SpawnRocks(int amount, Tile[,] grid, Transform gridT, GameObject rockPrefab) {

        int tries = 1000;
        int rocksSpawned = 0;
        while(rocksSpawned < amount) {
            int x = Random.Range(0, grid.GetLength(0) - 1);
            int y = Random.Range(0, grid.GetLength(1) - 1);
            if(grid[x, y].entityOnTile == false) {
                SpawnRock(grid, gridT, new Tile.TilePosition(x, y), rockPrefab);
                rocksSpawned++;
            }
            tries--;
            if(tries <= 0)
                break;
        }
    }

    public static void SpawnWater(int amount, Tile[,] grid) {

        int tries = 1000;
        int waterSpawned = 0;
        while(waterSpawned < amount) {
            int x = Random.Range(0, grid.GetLength(0) - 1);
            int y = Random.Range(0, grid.GetLength(1) - 1);
            if(grid[x, y].entityPresent == Tile.EntityType.None && grid[x, y].tileModifier == Tile.TileModifier.None) {
                waterSpawned++;
                grid[x, y].SetTileModifier(Tile.TileModifier.Water, (int)(grid[x, y].baseWeight * GridManager.instance.waterWeightCoeff));
            }
            tries--;
            if(tries <= 0)
                break;
        }
    }

    public static void SpawnGrass(int amount, Tile[,] grid) {

        int tries = 1000;
        int grassSpawned = 0;
        while(grassSpawned < amount) {
            int x = Random.Range(0, grid.GetLength(0) - 1);
            int y = Random.Range(0, grid.GetLength(1) - 1);
            if(grid[x, y].entityPresent == Tile.EntityType.None && grid[x, y].tileModifier == Tile.TileModifier.None) {
                grassSpawned++;
                grid[x, y].SetTileModifier(Tile.TileModifier.Grass, (int)(grid[x, y].baseWeight * GridManager.instance.grassWeightCoeff));
            }
            tries--;
            if(tries <= 0)
                break;
        }
    }

    public static void SpawnRock(Tile[,] grid, Transform gridT, Tile.TilePosition pos, GameObject rockPrefab) {

        grid[pos.x, pos.y].SpawnRockOnTile(rockPrefab, gridT.Find("Rocks"), .5f);
        GridManager.instance.ColorTile(grid[pos.x, pos.y], GridManager.TileColorType.Unmovable);
    }

    public enum HexagonOrientation {
        FlatTopped, PointyTopped
    }

    public enum HexagonOddity {
        // odd - uvuceno, even - ispupceno
        Odd, Even
    }

    public struct TerrainData {
        public int gridX;
        public int gridY;
        public float offset;
        public Hex hex;

        public TerrainData(int gridX, int gridY, float offset, Hex hex) {
            this.gridX = gridX;
            this.gridY = gridY;
            this.offset = offset;
            this.hex = hex;
        }
    }
}