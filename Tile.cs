using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

    public Transform tileCenter;
    public BattleEntity entityOnTile;
    public Text text;

    public GameObject graphics;

    public TilePosition position;

    public int gCost;
    public int hCost;
    public Tile parent;

    public int baseWeight;

    public int fCost {
        get {
            return gCost + hCost;
        }
    }
    public bool isClickableForInput;
    public bool isAvailableToPathfinding;

    public Dictionary<BattleEntity, BattleEntityConnection> bEntityConnection = new Dictionary<BattleEntity, BattleEntityConnection>();

    public bool isEmpty {
        get {
            if(entityPresent == EntityType.None)
                return true;
            return false;
        }
    }

    [System.Serializable]
    public class BattleEntityConnection {
        public bool isClickableForInput;
        public bool isAvailableToPathfinding;
        public bool inEntitiesMovementRange;
        public bool inEntitiesIntimidationRange;
        public bool inEntitiesIntimidationRangeOuter;
        public bool inEntitiesMeleeRange;
        public bool inEntitiesMeleeRangeOuter;
        public List<BattleEntity> alliesSharingIntimidationRange;

        public BattleEntityConnection() {
            this.isClickableForInput = false;
            this.isAvailableToPathfinding = false;
            this.inEntitiesMovementRange = false;
            this.inEntitiesIntimidationRange = false;
            this.inEntitiesMeleeRange = false;
            this.inEntitiesIntimidationRangeOuter = false;
            this.inEntitiesMeleeRangeOuter = false;
            alliesSharingIntimidationRange = new List<BattleEntity>();
        }
    }

    public EntityType entityPresent;
    public TileModifier tileModifier;


    public List<BattleEntity> GetEntitiesInRange(BattleEntity entity, RangeType rangeType, Relationship relationship) {
        List<BattleEntity> bes = new List<BattleEntity>();
        foreach(KeyValuePair<BattleEntity, BattleEntityConnection> bec in bEntityConnection) {
            if(rangeType == RangeType.Movement && bec.Value.inEntitiesMovementRange) {
                if(relationship == Relationship.Ally && ((entity.entityType == EntityType.Player && bec.Key.entityType == EntityType.Player) || (entity.entityType == EntityType.Enemy && bec.Key.entityType == EntityType.Enemy))) {
                    bes.Add(bec.Key);
                }
                else if(relationship == Relationship.Enemy && ((entity.entityType == EntityType.Player && bec.Key.entityType == EntityType.Enemy) || (entity.entityType == EntityType.Enemy && bec.Key.entityType == EntityType.Player))) {
                    bes.Add(bec.Key);
                }
                else if(relationship == Relationship.Any) {
                    bes.Add(bec.Key);
                }
            }
            else if(rangeType == RangeType.Intimidation && bec.Value.inEntitiesIntimidationRange) {
                if(relationship == Relationship.Ally && ((entity.entityType == EntityType.Player && bec.Key.entityType == EntityType.Player) || (entity.entityType == EntityType.Enemy && bec.Key.entityType == EntityType.Enemy))) { 
                    bes.Add(bec.Key);
                }
                else if(relationship == Relationship.Enemy && ((entity.entityType == EntityType.Player && bec.Key.entityType == EntityType.Enemy) || (entity.entityType == EntityType.Enemy && bec.Key.entityType == EntityType.Player))) {
                    bes.Add(bec.Key);
                }
                else if(relationship == Relationship.Any) {
                    bes.Add(bec.Key);
                }
            }
            else if(rangeType == RangeType.OuterIntimidation && bec.Value.inEntitiesIntimidationRangeOuter) {
                if(relationship == Relationship.Ally && ((entity.entityType == EntityType.Player && bec.Key.entityType == EntityType.Player) || (entity.entityType == EntityType.Enemy && bec.Key.entityType == EntityType.Enemy))) { 
                bes.Add(bec.Key);
                }
                else if(relationship == Relationship.Enemy && ((entity.entityType == EntityType.Player && bec.Key.entityType == EntityType.Enemy) || (entity.entityType == EntityType.Enemy && bec.Key.entityType == EntityType.Player))) {
                    bes.Add(bec.Key);
                }
                else if(relationship == Relationship.Any) {
                    bes.Add(bec.Key);
                }
            }
            else if(rangeType == RangeType.Melee && bec.Value.inEntitiesMeleeRange) {
                if(relationship == Relationship.Ally && ((entity.entityType == EntityType.Player && bec.Key.entityType == EntityType.Player) || (entity.entityType == EntityType.Enemy && bec.Key.entityType == EntityType.Enemy))) {
                    bes.Add(bec.Key);
                }
                else if(relationship == Relationship.Enemy && ((entity.entityType == EntityType.Player && bec.Key.entityType == EntityType.Enemy) || (entity.entityType == EntityType.Enemy && bec.Key.entityType == EntityType.Player))) {
                    bes.Add(bec.Key);
                }
                else {
                    bes.Add(bec.Key);
                }
            }
            else if(rangeType == RangeType.OuterMelee && bec.Value.inEntitiesMeleeRangeOuter) {
                if(relationship == Relationship.Ally && (entity.entityType == EntityType.Player && bec.Key.entityType == EntityType.Player) || (entity.entityType == EntityType.Enemy && bec.Key.entityType == EntityType.Enemy)) {
                    bes.Add(bec.Key);
                }
                else if(relationship == Relationship.Enemy && (entity.entityType == EntityType.Player && bec.Key.entityType == EntityType.Enemy) || (entity.entityType == EntityType.Enemy && bec.Key.entityType == EntityType.Player)) {
                    bes.Add(bec.Key);
                }
                else if(relationship == Relationship.Any) {
                    bes.Add(bec.Key);
                }
            }
        }

        return bes;
    }
    
    public int GetNumberOfEntitesInIntimidationRange() {
        int i = 0;
        foreach(KeyValuePair<BattleEntity, BattleEntityConnection> bec in bEntityConnection) {
            if(bec.Value.inEntitiesIntimidationRange) {
                i++;
            }
        }
        return i;
    }

    public List<BattleEntity> GetEntitiesInIntimidationRange() {
        List<BattleEntity> bes = new List<BattleEntity>();
        foreach(KeyValuePair<BattleEntity, BattleEntityConnection> bec in bEntityConnection) {
            if(bec.Value.inEntitiesIntimidationRange) {
                bes.Add(bec.Key);
            }
        }
        return bes;
    }

    public bool IsInEnemysIntimidationRange(BattleEntity entity) {

        foreach(KeyValuePair<BattleEntity, BattleEntityConnection> bec in bEntityConnection) {
            if(bec.Value.inEntitiesIntimidationRange) {
                if(bec.Key.entityType == EntityType.Player && entity.entityType == EntityType.Enemy) {
                    return true;
                }
                if(bec.Key.entityType == EntityType.Enemy && entity.entityType == EntityType.Player) {
                    return true;
                }
            }
        }
        return false;
        /*
        foreach(BattleEntity be in inEntitiesIntimidationRange) {
            if(entity.entityType == EntityType.Player && be.entityType == EntityType.Enemy)
                return true;
            if(entity.entityType == EntityType.Enemy && be.entityType == EntityType.Player)
                return true;
        }
        return false;*/
    }

    public bool IsInAllysIntimidationRange(BattleEntity entity) {

        foreach(KeyValuePair<BattleEntity, BattleEntityConnection> bec in bEntityConnection) {
            if(bec.Value.inEntitiesIntimidationRange) {
                if(bec.Key.entityType == EntityType.Player && entity.entityType == EntityType.Player) {
                    return true;
                }
                if(bec.Key.entityType == EntityType.Enemy && entity.entityType == EntityType.Enemy) {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsInAllysOuterIntimidationRing(BattleEntity entity) {

        foreach(KeyValuePair<BattleEntity, BattleEntityConnection> bec in bEntityConnection) {
            if(bec.Value.inEntitiesIntimidationRangeOuter) {
                if(bec.Key.entityType == EntityType.Player && entity.entityType == EntityType.Player) {
                    return true;
                }
                if(bec.Key.entityType == EntityType.Enemy && entity.entityType == EntityType.Enemy) {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsInEnemysOuterIntimidationRing(BattleEntity entity) {

        foreach(KeyValuePair<BattleEntity, BattleEntityConnection> bec in bEntityConnection) {
            if(bec.Value.inEntitiesIntimidationRangeOuter) {
                if(bec.Key.entityType == EntityType.Player && entity.entityType == EntityType.Enemy) {
                    return true;
                }
                if(bec.Key.entityType == EntityType.Enemy && entity.entityType == EntityType.Player) {
                    return true;
                }
            }
        }
        return false;

        /*
        bool flag = true;
        if(inEntitiesIntimidationRange.Count == 0) {
            return false;
        }
        foreach(BattleEntity be in inEntitiesIntimidationRange) {

            if(entity == EntityType.Enemy && be.entityType == EntityType.Player) {

                if(Tile.Distance(be.tile, this) == be.entity.stats.intimidationRange)
                    flag &= true;
                else
                    flag &= false;
            }
            else if(entity == EntityType.Player && be.entityType == EntityType.Enemy) {
                
                if(Tile.Distance(be.tile, this) == be.entity.stats.intimidationRange)
                    flag &= true;
                else
                    flag &= false;
            }
            else
                flag &= false;
        }
        return flag;*/
    }

    public bool IsInEnemysMeleeRange(BattleEntity entity) {

        foreach(KeyValuePair<BattleEntity, BattleEntityConnection> bec in bEntityConnection) {
            if(bec.Value.inEntitiesMeleeRange) {
                if(bec.Key.entityType == EntityType.Player && entity.entityType == EntityType.Enemy) {
                    return true;
                }
                if(bec.Key.entityType == EntityType.Enemy && entity.entityType == EntityType.Player) {
                    return true;
                }
            }
        }
        return false;

        /*
        foreach(BattleEntity be in inEntitiesMeleeRange) {
            if(entity == EntityType.Player && be.entityType == EntityType.Enemy)
                return true;
            if(entity == EntityType.Enemy && be.entityType == EntityType.Player)
                return true;
        }
        return false;*/
    }

    public void SetTileModifier(TileModifier modifier, int weight) {

        this.baseWeight = weight;
        if(modifier == TileModifier.Water) {
            graphics.GetComponent<MeshFilter>().mesh = JRPG.TerrainLibrary.instance.GetTerrain("hill").tileModel.GetComponent<MeshFilter>().sharedMesh;
            tileCenter.localPosition = new Vector3(tileCenter.localPosition.x, .23f, tileCenter.localPosition.z);
            text.transform.parent.localPosition = new Vector3(0f, 0.23f, 0f);
        }
        else {
            graphics.GetComponent<MeshFilter>().mesh = JRPG.TerrainLibrary.instance.GetTerrain("path").tileModel.GetComponent<MeshFilter>().sharedMesh;
            tileCenter.localPosition = new Vector3(tileCenter.localPosition.x, .02f, tileCenter.localPosition.z);
            text.transform.parent.localPosition = new Vector3(0f, 0.02f, 0f);
        } 
            
        tileModifier = modifier;
    }

    public void SetEmpty(int emptyWeight) {

        baseWeight = emptyWeight;
        tileModifier = TileModifier.None;
        entityPresent = EntityType.None;
    }

    public void SetEmpty() {

        SetEmpty(BattleManager.distanceCoefficient);
    }

    public BattleEntity SpawnBattleEntityOnTile(Transform parent, EntityType type) {

        if(entityOnTile == null) {

            GameObject newEntity = new GameObject();

            if(parent != null)
                newEntity.transform.SetParent(parent);

            entityOnTile = newEntity.AddComponent<BattleEntity>();
            entityPresent = type;
            return entityOnTile;
        }
        return null;
    }

    public void SpawnRockOnTile(GameObject rockPrefab, Transform parent, float height) {

        if(entityOnTile == null) {
            GameObject newEntity;
            if(parent != null)
                newEntity = (GameObject)GameObject.Instantiate(rockPrefab, parent);
            else
                newEntity = (GameObject)GameObject.Instantiate(rockPrefab);
            newEntity.transform.position = tileCenter.position + Vector3.up * height / 2f;
            entityPresent = EntityType.Rock;
            baseWeight = int.MaxValue;
        }
    }

    public void DestroyEntityOnTile() {

        if(entityOnTile != null) {
            entityOnTile = null;
            entityPresent = EntityType.None;
        }
    }

    public void ResetCosts() {

        gCost = 0;
        hCost = 0;
    }

    public static int Distance(Tile a, Tile b) {

        Hex.CubePairs ca = Hex.OffsetToCube(GridManager.hex, a.position.x, a.position.y);
        Hex.CubePairs cb = Hex.OffsetToCube(GridManager.hex, b.position.x, b.position.y);

        return Mathf.Max(Mathf.Abs(ca.x - cb.x), Mathf.Abs(ca.y - cb.y), Mathf.Abs(ca.z - cb.z));
    }

    public static int AxisDistance(Tile a, Tile b, Hex.Axis axis) {

        Hex.CubePairs ca = Hex.OffsetToCube(GridManager.hex, a.position.x, a.position.y);
        Hex.CubePairs cb = Hex.OffsetToCube(GridManager.hex, b.position.x, b.position.y);

        switch(axis) {
            case Hex.Axis.X:
                return ca.x - cb.x;
            case Hex.Axis.Y:
                return ca.y - cb.y;
            case Hex.Axis.Z:
                return ca.z - cb.z;
        }

        return 0;
    }

    public enum EntityType {
        None, Rock, Player, Enemy
    }

    public enum TileModifier {
        None, Water, Grass
    }

    [System.Serializable]
    public struct TilePosition {
        public int x;
        public int y;

        public TilePosition(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }
}

public enum RangeType {
    Movement, Intimidation, Melee, OuterIntimidation, OuterMelee, Any
};

public enum Relationship {
    Ally, Enemy, Any
};