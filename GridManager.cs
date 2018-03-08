using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class GridManager : MonoBehaviour {

    public static GridManager instance;

    public float hexagonHeight;
    public Hex.HexagonOrientation hexagonOrientation;
    public Hex.HexagonOddity hexagonOddity;
    [Range(1f, float.MaxValue)]
    public float waterWeightCoeff;
    public float grassWeightCoeff;
    public float spaceBetween = 0f;

    public Color traversableColor;
    public Color neutralColor;
    public Color unwalkableColor;
    public Color currentPathColor;
    public Color grassColor;
    public Color grassTraversableColor;
    public Color grassPathColor;
    public Color hillColor;
    public Color hillTraversableColor;
    public Color hillPathColor;
    public Color meleeRangeColor;
    public Color intimidationRangeColor;

    public GameObject tilePrefab;
    public GameObject rockPrefab;
    
    public static Tile[,] grid { get; private set; }
    public static Hex hex { get; private set; }
    

    void Awake() {

        if(instance == null)
            instance = this;
    }

    public void GenerateGrid(BattleData battleData) {

        hex = new Hex(hexagonHeight * .5f, hexagonOrientation, hexagonOddity);

        TerrainGenerator.TerrainData terData = new TerrainGenerator.TerrainData(battleData.gridSizeX, battleData.gridSizeY, spaceBetween, hex);

        grid = TerrainGenerator.GenerateTerrain(terData, this.transform);
        TerrainGenerator.SpawnRocks(battleData.numberOfRocks, grid, this.transform, rockPrefab);
        TerrainGenerator.SpawnWater(battleData.numberOfHills, grid);
        TerrainGenerator.SpawnGrass(battleData.numberOfPaths, grid);

        foreach(Tile t in grid) {
            t.text.text = (t.baseWeight).ToString();
        }

        ResetGridColor();
    }

    public List<BattleEntity> SpawnPlayers(List<Entity> players) {

        int x = 4;

        List<BattleEntity> pl = new List<BattleEntity>();

        foreach(Entity e in players) {

            pl.Add(SpawnEntity(x, 1, e, Tile.EntityType.Player));
            x++;
        }

        return pl;
    }

    public List<BattleEntity> SpawnEnemies(string[] enemies) {

        int x = 4;

        List<BattleEntity> en = new List<BattleEntity>();

        foreach(string s in enemies) {

            Entity e = EntityLibrary.instance.GetEntity(s);

            en.Add(SpawnEntity(x, 12, e, Tile.EntityType.Enemy));
            x++;
        }

        return en;
    }

    public void DisplayRangeWithTargetableEntities(BattleEntity bEntity, RangeType rangeType, Relationship relationship) {

        ResetGridColor();

        List<Tile> range = new List<Tile>();
        Dictionary<Tile, TileColorType> color = new Dictionary<Tile, TileColorType>();

        switch(rangeType) {
            case RangeType.Melee:
                range = bEntity.tilesInMeleeRange;
                break;
            case RangeType.Intimidation:
                range = bEntity.tilesInIntimidationRange;
                break;
            case RangeType.Movement:
                range = bEntity.tilesInMovementRange;
                break;
            case RangeType.Any:
                range.AddRange(bEntity.tilesInMovementRange);
                range.AddRange(bEntity.tilesInMeleeRange);
                range.AddRange(bEntity.tilesInIntimidationRange);
                range = range.Distinct().ToList();
                break;
        }

        foreach(Tile t in range) {
            if(t.entityOnTile.CompareEntityRelationShip(bEntity, relationship)) {

            }
        }

    }

    public void DisplayRelativeRangeAndPath(BattleEntity bEntity, RangeType rangeType) {

        ResetGridColor();

        List<Tile> range = new List<Tile>();
        Tile[] path;

        Dictionary<Tile, TileColorType> color = new Dictionary<Tile, TileColorType>();

        switch(rangeType) {
            case RangeType.Melee:
                range = bEntity.tilesInMeleeRange;
                break;
            case RangeType.Intimidation:
                range = bEntity.tilesInIntimidationRange;
                break;
            case RangeType.Movement:
                range = bEntity.tilesInMovementRange;
                break;
            case RangeType.Any:
                range.AddRange(bEntity.tilesInMovementRange);
                range.AddRange(bEntity.tilesInMeleeRange);
                range.AddRange(bEntity.tilesInIntimidationRange);
                range = range.Distinct().ToList();
                break;
        }

        path = bEntity.path;

        foreach(Tile t in range) {

            if(t.IsInEnemysMeleeRange(bEntity) == true) {
                color.Add(t, TileColorType.MeleeRange);
            }
            else if(t.IsInEnemysOuterIntimidationRing(bEntity) == true) {
                color.Add(t, TileColorType.IntimidationRange);
            }
            else if(t.IsInEnemysIntimidationRange(bEntity) == true) {
                color.Add(t, TileColorType.IntimidationRange);
            }
            else {
                t.isClickableForInput = true;
                color.Add(t, TileColorType.Movable);
            }

            t.isClickableForInput = true;
            if(t.IsInEnemysOuterIntimidationRing(bEntity) == true) {
                t.isClickableForInput = true;
            }
        }

        DisplayRangeAndPath(color, path, true);
    }

    public void DisplayRange(BattleEntity bEntity, RangeType rangeType) {

        ResetGridColor();

        HashSet<Tile> range = new HashSet<Tile>();
        Dictionary<Tile, TileColorType> color = new Dictionary<Tile, TileColorType>();

        foreach(Tile t in bEntity.tilesInMeleeRange) {
            range.Add(t);
            color.Add(t, TileColorType.MeleeRange);
        }

        foreach(Tile t in bEntity.tilesInIntimidationRange) {
            if(range.Contains(t) == false) {
                range.Add(t);
                color.Add(t, TileColorType.IntimidationRange);
            }
        }

        foreach(Tile t in bEntity.tilesInMovementRange) {
            if(range.Contains(t) == false) {
                range.Add(t);
                color.Add(t, TileColorType.Movable);
            }
        }

        DisplayRangeAndPath(color, null, false);
    }

    void DisplayRangeAndPath(Dictionary<Tile, TileColorType> range, Tile[] path, bool relative) {

        if(range != null) {
            ColorRange(range);
        }

        if(path != null) {
            ColorPath(path);
        }
    }

    void ColorRange(Dictionary<Tile, TileColorType> range) {

        foreach(KeyValuePair<Tile, TileColorType> t in range) {

            if(t.Value == TileColorType.Movable) {
                ColorTile(t.Key, TileColorType.Movable);
            }
            else if(t.Value == TileColorType.IntimidationRange) {
                ColorTile(t.Key, TileColorType.IntimidationRange);
            }
            else if(t.Value == TileColorType.MeleeRange) {
                ColorTile(t.Key, TileColorType.MeleeRange);
            }
        }
    }

    void ColorPath(Tile[] path) {

        if(path.Length == 0)
            return;

        for(int i = 0; i < path.Length; i++) {
            if(i != path.Length - 1) {
                ColorTile(path[i], TileColorType.CurrentPath);
            }
            else {
                ColorTile(path[i], TileColorType.CurrentPathDestination);
            }
        }
    }

   public void ResetGridColor() {

        foreach(Tile t in grid) {

            if(t.tileModifier == Tile.TileModifier.Water) {
                ColorTile(t, TileColorType.Hill);
            }
            else if(t.tileModifier == Tile.TileModifier.Grass) {
                ColorTile(t, TileColorType.Grass);
            }
            else {
                ColorTile(t, TileColorType.Unmovable);
            }
        }
    }

    public BattleEntity SpawnEntity(int x, int y, Entity entity, Tile.EntityType type) {

        Tile tile = grid[x, y];
        return SpawnEntity(tile, entity, type);
    }

    public BattleEntity SpawnEntity(Tile tile, Entity entity, Tile.EntityType type) {

        BattleEntity bEntity = tile.SpawnBattleEntityOnTile(null, type);

        bEntity.Initialize(entity, BattleManager.distanceCoefficient, tile, type);

        return bEntity;
    }

    public void ColorTile(Tile tile, TileColorType colorType) {

        if(tile.entityPresent == Tile.EntityType.None) {

            switch(colorType) {
                case TileColorType.Movable:
                    tile.graphics.GetComponent<Renderer>().material.color = traversableColor;
                    break;
                case TileColorType.Unmovable:
                    tile.graphics.GetComponent<Renderer>().material.color = neutralColor;
                    break;
                case TileColorType.CurrentPath:
                    tile.graphics.GetComponent<Renderer>().material.color = currentPathColor;
                    break;
                case TileColorType.Hill:
                    tile.graphics.GetComponent<Renderer>().material.color = hillColor;
                    break;
                case TileColorType.HillTraversable:
                    tile.graphics.GetComponent<Renderer>().material.color = hillTraversableColor;
                    break;
                case TileColorType.HillPath:
                    tile.graphics.GetComponent<Renderer>().material.color = hillPathColor;
                    break;
                case TileColorType.Grass:
                    tile.graphics.GetComponent<Renderer>().material.color = grassColor;
                    break;
                case TileColorType.GrassPath:
                    tile.graphics.GetComponent<Renderer>().material.color = grassPathColor;
                    break;
                case TileColorType.GrassTraversable:
                    tile.graphics.GetComponent<Renderer>().material.color = grassTraversableColor;
                    break;
                case TileColorType.CurrentPathDestination:
                    tile.graphics.GetComponent<Renderer>().material.color = currentPathColor;
                    break;
                case TileColorType.MeleeRange:
                    tile.graphics.GetComponent<Renderer>().material.color = meleeRangeColor;
                    break;
                case TileColorType.IntimidationRange:
                    tile.graphics.GetComponent<Renderer>().material.color = intimidationRangeColor;
                    break;
            }
        }
        else if(tile.entityPresent == Tile.EntityType.Rock) {
            tile.graphics.GetComponent<Renderer>().material.color = unwalkableColor;
        }
        else {
            tile.graphics.GetComponent<Renderer>().material.color = neutralColor;
        }
    }

    public enum TileColorType {
        CurrentPath, Movable, Unmovable, Hill, HillTraversable, HillPath, Grass, GrassTraversable,
        GrassPath, CurrentPathDestination, MeleeRange, IntimidationRange
    }
}