using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleEntity : MonoBehaviour {

    public Entity entity;
    public Tile tile;
    public Tile.TilePosition position {
        get {
            return tile.position;
        }
    }

    public int movementRange;
    public float modelHeight;

    public bool upperBodyAvailable;
    public bool lowerBodyAvailable;

    public List<Tile> tilesInMovementRange = new List<Tile>();
    public List<Tile> tilesInMeleeRange = new List<Tile>();
    public List<Tile> tilesInIntimidationRange = new List<Tile>();

    public List<BattleEntity> bEntitiesInIntimidationRange = new List<BattleEntity>();
    public List<BattleEntity> alliesSharingIntimidationRange = new List<BattleEntity>();

    public Tile[] path;

    public Tile.EntityType entityType;


    public void Initialize(Entity entity, int distCoeff, Tile tile, Tile.EntityType type) {

        this.entity = entity;
        GameObject model;

        if(entity == null)
            return;

        if(entity.graphics != null) {
            model = entity.graphics;
            modelHeight = entity.height;
        }
        else {
            Debug.Log("Character \"" + entity.id + "\" has no graphics. Using Dummy Character as graphics.");
            model = BattleManager.instance.dummyCharacterGraphics;
            modelHeight = 1f;
        }

        transform.name = entity.name;
        transform.position = tile.tileCenter.position + Vector3.up * modelHeight / 2f;

        GameObject graphics = (GameObject)GameObject.Instantiate(model, transform);
        graphics.name = "Graphics";
        graphics.transform.localPosition = Vector3.zero;

        this.tile = tile;

        entityType = type;

        SetRangeToMax();
        CalculateRanges();
    }

    public bool IsInOuterIntimidationRing(Tile tile) {
        foreach(Tile t in tilesInIntimidationRange) {
            if(Tile.Distance(t, tile) == entity.stats.intimidationRange) {
                return true;
            }
        }
        return false;
    }

    public bool CompareEntityRelationShip(BattleEntity entity, Relationship relationship) {

        if(relationship == Relationship.Any) {
            return true;
        }
        if(relationship == Relationship.Ally && ((entity.entityType == Tile.EntityType.Player && entityType == Tile.EntityType.Player) || (entity.entityType == Tile.EntityType.Enemy && entityType == Tile.EntityType.Enemy))) {
            return true;
        }
        if(relationship == Relationship.Enemy && ((entity.entityType == Tile.EntityType.Player && entityType == Tile.EntityType.Enemy) || (entity.entityType == Tile.EntityType.Enemy && entityType == Tile.EntityType.Player))) {
            return true;
        }
        return false;
    }

    public void ProcessTick() {

        // Apply Status effects ticks


        


    }

    public List<BattleEntity> GetEntitiesInRange(BattleEntity entity, RangeType rangeType, Relationship relationship) {
        List<BattleEntity> result = new List<BattleEntity>();

        if(rangeType == RangeType.Movement) {
            foreach(Tile t in tilesInMovementRange) {
                if(t.entityPresent != Tile.EntityType.None) {
                    if(relationship == Relationship.Enemy && ((t.entityPresent == Tile.EntityType.Player && entity.entityType == Tile.EntityType.Enemy) || (t.entityPresent == Tile.EntityType.Enemy && entity.entityType == Tile.EntityType.Player))) {
                        result.Add(t.entityOnTile);
                    }
                    else if(relationship == Relationship.Ally && ((t.entityPresent == Tile.EntityType.Player && entity.entityType == Tile.EntityType.Player) || (t.entityPresent == Tile.EntityType.Enemy && entity.entityType == Tile.EntityType.Enemy))) {
                        result.Add(t.entityOnTile);
                    }
                    else if(relationship == Relationship.Any) {
                        result.Add(t.entityOnTile);
                    }
                }
            }
        }
        else if(rangeType == RangeType.Intimidation) {
            foreach(Tile t in tilesInIntimidationRange) {
                if(t.entityPresent != Tile.EntityType.None) {
                    if(relationship == Relationship.Enemy && ((t.entityPresent == Tile.EntityType.Player && entity.entityType == Tile.EntityType.Enemy) || (t.entityPresent == Tile.EntityType.Enemy && entity.entityType == Tile.EntityType.Player))) {
                        result.Add(t.entityOnTile);
                    }
                    else if(relationship == Relationship.Ally && ((t.entityPresent == Tile.EntityType.Player && entity.entityType == Tile.EntityType.Player) || (t.entityPresent == Tile.EntityType.Enemy && entity.entityType == Tile.EntityType.Enemy))) {
                        result.Add(t.entityOnTile);
                    }
                    else if(relationship == Relationship.Any) {
                        result.Add(t.entityOnTile);
                    }
                }
            }
        }
        else if(rangeType == RangeType.Melee) {
            foreach(Tile t in tilesInMeleeRange) {
                if(t.entityPresent != Tile.EntityType.None) {
                    if(relationship == Relationship.Enemy && ((t.entityPresent == Tile.EntityType.Player && entity.entityType == Tile.EntityType.Enemy) || (t.entityPresent == Tile.EntityType.Enemy && entity.entityType == Tile.EntityType.Player))) {
                        result.Add(t.entityOnTile);
                    }
                    else if(relationship == Relationship.Ally && ((t.entityPresent == Tile.EntityType.Player && entity.entityType == Tile.EntityType.Player) || (t.entityPresent == Tile.EntityType.Enemy && entity.entityType == Tile.EntityType.Enemy))) {
                        result.Add(t.entityOnTile);
                    }
                    else if(relationship == Relationship.Any) {
                        result.Add(t.entityOnTile);
                    }
                }
            }
        }
        return result;
    }

    public void CalculateRanges() {

        foreach(Tile t in GridManager.grid) {

            if(t.bEntityConnection.ContainsKey(this)) {
                t.bEntityConnection.Remove(this);
            }
        }

        Pathfinding.ThisIsCool cool1 = new Pathfinding.ThisIsCool();
        Pathfinding.ThisIsCool cool2 = new Pathfinding.ThisIsCool();
        Pathfinding.ThisIsCool cool3 = new Pathfinding.ThisIsCool();
        Pathfinding.GridInfo gridInfo = new Pathfinding.GridInfo();
        gridInfo.cleanse = false;
        gridInfo.useMovementPenalty = true;
        gridInfo.useThisIsCool = true;
        gridInfo.allowEntitiesToBeTargeted = true;
        tilesInMovementRange = Pathfinding.GetRangeObstacles(tile, movementRange, gridInfo, out cool1);
        gridInfo.useMovementPenalty = false;
        tilesInIntimidationRange = Pathfinding.GetRangeObstacles(tile, entity.stats.intimidationRange * BattleManager.distanceCoefficient, gridInfo, out cool2);
        tilesInMeleeRange = Pathfinding.GetRangeObstacles(tile, entity.stats.meleeRange * BattleManager.distanceCoefficient, gridInfo, out cool3);

        foreach(Tile t in GridManager.grid) {
            t.bEntityConnection.Add(this, new Tile.BattleEntityConnection());

            if(tilesInMovementRange.Contains(t)) {
                t.bEntityConnection[this].inEntitiesMovementRange = true;
            }
            if(tilesInIntimidationRange.Contains(t)) {
                t.bEntityConnection[this].inEntitiesIntimidationRange = true;
            }
            if(tilesInMeleeRange.Contains(t)) {
                t.bEntityConnection[this].inEntitiesMeleeRange = true;
            }
            if(cool2.outerTiles.Contains(t)) {
                t.bEntityConnection[this].inEntitiesIntimidationRangeOuter = true;
            }
            if(cool3.outerTiles.Contains(t)) {
                t.bEntityConnection[this].inEntitiesMeleeRangeOuter = true;
            }
        }
    }

    public void CalculateReachableAreas() {

        Pathfinding.GridInfo gridInfo = new Pathfinding.GridInfo();
        Pathfinding.ThisIsCool cool1 = new Pathfinding.ThisIsCool();
        gridInfo = new Pathfinding.GridInfo();
        gridInfo.useLineOfSight = false;
        gridInfo.useMovementPenalty = true;
        gridInfo.unreachableTiles = new List<Tile>();
        gridInfo.unPathableTiles = new List<Tile>();

        // Set everything to be unreachable for this entity.
        foreach(Tile t in GridManager.grid) {
            gridInfo.unreachableTiles.Add(t);
            gridInfo.unPathableTiles.Add(t);
        }

        // Cycle through all tiles and set each one that isn't in enemy's intimidation range
        // and intimidation ring to reachable and solve for pathing.
        foreach(Tile t in GridManager.grid) {
            if(t.IsInEnemysIntimidationRange(this) == false) {
                gridInfo.unreachableTiles.Remove(t);
                gridInfo.unPathableTiles.Remove(t);
            }
            else if(t.IsInEnemysOuterIntimidationRing(this) == true) {
                gridInfo.unreachableTiles.Remove(t);
            }
        }

        // Cycle through all of this battle entity's battle entities in range.
        foreach(BattleEntity be in bEntitiesInIntimidationRange) {

            List<Tile> alliedIntimidationTiles = new List<Tile>();
            foreach(BattleEntity ally in be.alliesSharingIntimidationRange) {
                alliedIntimidationTiles.AddRange(ally.tilesInIntimidationRange);
            }
            alliedIntimidationTiles.AddRange(be.tilesInIntimidationRange);

            foreach(Tile t in alliedIntimidationTiles) {
                    
                bool flag = false;

                foreach(BattleEntity other in t.GetEntitiesInRange(be, RangeType.Intimidation, Relationship.Ally)) {
                    if(be != other) {
                        if(!be.alliesSharingIntimidationRange.Contains(other) && !bEntitiesInIntimidationRange.Contains(other)) {
                            flag = true;
                        }
                    }
                }

                if(flag == false) {
                    gridInfo.unreachableTiles.Remove(t);
                    gridInfo.unPathableTiles.Remove(t);
                }
            }
        }

        gridInfo.cleanse = true;
        gridInfo.bEntity = this;
        tilesInMovementRange = Pathfinding.GetRangeObstacles(this.tile, movementRange, gridInfo, out cool1);

        foreach(Tile t in GridManager.grid) {
            if(gridInfo.unreachableTiles.Contains(t)) {
                t.isClickableForInput = false;
            }
            if(gridInfo.unPathableTiles.Contains(t)) {
                t.isAvailableToPathfinding = false;
            }
        }

        foreach(Tile t in tilesInMovementRange) {
            Tile.BattleEntityConnection bec = new Tile.BattleEntityConnection();
            bec.inEntitiesMovementRange = true;
            if(!t.bEntityConnection.ContainsKey(this)) {
                t.bEntityConnection.Add(this, new Tile.BattleEntityConnection());
            }
            else {
                t.bEntityConnection[this].inEntitiesMovementRange = true;
            }
        }
    }

    public delegate void CharacterFinishedMovingCallback(BattleEntity entity);

    public void MoveCharacter(Tile[] path, CharacterFinishedMovingCallback Callback) {

        tile.DestroyEntityOnTile();
        StartCoroutine(MoveChar(path, Callback));
    }

    void OnArrivedToDestination(Tile destination) {

        destination.entityOnTile = this;
        destination.entityPresent = entityType;
        tile = destination;
    }

    public void RecalculateEnemiesInIntimidationRange() {
        bEntitiesInIntimidationRange.Clear();

        if(tile.IsInEnemysIntimidationRange(this)) {
            foreach(BattleEntity be in tile.GetEntitiesInRange(this, RangeType.Intimidation, Relationship.Enemy)) {
                bEntitiesInIntimidationRange.Add(be);
            }
        }
    }

    bool movedToTile = false;

    IEnumerator MoveChar(Tile[] path, CharacterFinishedMovingCallback Callback) {

        for(int i = 0; i < path.Length; i++) {
            StartCoroutine(MoveToTile(path[i]));
            movedToTile = false;
            while(movedToTile == false) {
                yield return null;
            }
        }

        OnArrivedToDestination(path[path.Length - 1]);
        Callback(this);
    }

    Vector3 target;

    IEnumerator MoveToTile(Tile destination) {

        Vector3 actualDestination = destination.tileCenter.position + Vector3.up * .5f;

        float rotSpeed = .125f;
        float moveSpeed = .25f;

        float start = Time.time;
        float elapsed = 0f;
        float coefficient = 0f;

        Quaternion startingRot = transform.rotation;
        target = destination.tileCenter.position - transform.position;
        Quaternion targ = Quaternion.LookRotation(Vector3.Scale(target, new Vector3(1, 0, 1)));
        if(Quaternion.Angle(startingRot, targ) <= 5f)
            coefficient = 1.1f;

        while(coefficient < 1f) {
            elapsed = Time.time - start;
            coefficient = elapsed / rotSpeed;
            transform.rotation = Quaternion.Slerp(startingRot, targ, coefficient);
            yield return null;
        }

        start = Time.time;
        elapsed = 0f;
        coefficient = 0f;

        Vector3 startingPos = transform.position;

        while(coefficient < 1f) {
            elapsed = Time.time - start;
            coefficient = elapsed / moveSpeed;
            transform.position = Vector3.Lerp(startingPos, actualDestination, coefficient);
            yield return null;
        }

        movedToTile = true;
    }

    public void SetRangeToMax() {

        movementRange = entity.stats.movementRange * BattleManager.distanceCoefficient;
    }

    public void ModifyRange(int amount) {

        movementRange += amount;
        movementRange = Mathf.Clamp(movementRange, 0, int.MaxValue);
    }
}