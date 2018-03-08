using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Pathfinding {

    public static Tile[] FindPath(Tile startTile, Tile targetTile, out int cost) {

        foreach(Tile t in GridManager.grid) {
            t.ResetCosts();
        }

        cost = 0;
        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        openSet.Add(startTile);

        while(openSet.Count > 0) {
            Tile currentTile = openSet[0];
            for(int i = 1; i < openSet.Count; i++) {
                if(openSet[i].fCost < currentTile.fCost || openSet[i].fCost == currentTile.fCost && openSet[i].hCost < currentTile.hCost) {
                    currentTile = openSet[i];
                }
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            if(currentTile == targetTile) {
                return RetracePath(startTile, targetTile, out cost);
            }

            foreach(Tile n in GetNeighbors(currentTile)) {

                if(n.isAvailableToPathfinding == false) {
                    continue;
                }
                if(n.isEmpty == false) {
                    continue;
                }
                if(closedSet.Contains(n) == true) {
                    continue;
                }

                int weight = n.baseWeight;

                int newMovementCostToNeighbor = currentTile.gCost + (int)(Tile.Distance(currentTile, n) * BattleManager.distanceCoefficient * weight);

                if(newMovementCostToNeighbor < n.gCost || !openSet.Contains(n)) {

                    n.gCost = newMovementCostToNeighbor;
                    n.hCost = Tile.Distance(n, targetTile) * BattleManager.distanceCoefficient;
                    n.parent = currentTile;

                    if(!openSet.Contains(n))
                        openSet.Add(n);
                }
            }
        }

        return null;
    }

    public static Tile[] FindPath(Tile startTile, Tile targetTile, out int cost, GridInfo gridInfo) {

        foreach(Tile t in GridManager.grid) {
            t.ResetCosts();
        }

        cost = 0;
        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        openSet.Add(startTile);

        while(openSet.Count > 0) {
            Tile currentTile = openSet[0];
            for(int i = 1; i < openSet.Count; i++) {
                if(openSet[i].fCost < currentTile.fCost || openSet[i].fCost == currentTile.fCost && openSet[i].hCost < currentTile.hCost) {
                    currentTile = openSet[i];
                }
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            if(currentTile == targetTile) {
                return RetracePath(startTile, targetTile, out cost);
            }

            foreach(Tile n in GetNeighbors(currentTile)) {

                if(gridInfo.unreachableTiles != null) {
                    if(gridInfo.unreachableTiles.Contains(n)) {
                        continue;
                    }
                }

                if(n.isEmpty == false) {
                    continue;
                }
                if(closedSet.Contains(n) == true) {
                    continue;
                }

                int weight = n.baseWeight;

                int newMovementCostToNeighbor = currentTile.gCost + (int)(Tile.Distance(currentTile, n) * BattleManager.distanceCoefficient * weight);

                if(newMovementCostToNeighbor < n.gCost || !openSet.Contains(n)) {

                    n.gCost = newMovementCostToNeighbor;
                    n.hCost = Tile.Distance(n, targetTile) * BattleManager.distanceCoefficient;
                    n.parent = currentTile;

                    if(!openSet.Contains(n))
                        openSet.Add(n);
                }
            }
        }

        return null;
    }

    static Tile[] RetracePath(Tile startTile, Tile endTile, out int cost) {

        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        cost = 0;

        while(currentTile != startTile) {
            cost += currentTile.baseWeight;
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }

        path.Reverse();

        return path.ToArray();
    }

    public static List<Tile> GetRange(Tile tile, int range) {

        List<Tile> listA = new List<Tile>();
        List<Tile> listB = new List<Tile>();

        foreach(Tile t in GridManager.grid) {
            if(Tile.AxisDistance(tile, t, Hex.Axis.X) >= -range && Tile.AxisDistance(tile, t, Hex.Axis.X) <= range) {
                listA.Add(t);
            }
        }

        foreach(Tile t in listA) {
            if(Tile.AxisDistance(tile, t, Hex.Axis.Y) >= -range && Tile.AxisDistance(tile, t, Hex.Axis.Y) <= range) {
                listB.Add(t);
            }
        }

        listA.Clear();

        foreach(Tile t in listB) {
            if(Tile.AxisDistance(tile, t, Hex.Axis.Z) >= -range && Tile.AxisDistance(tile, t, Hex.Axis.Z) <= range) {
                listA.Add(t);
            }
        }

        return listA;
    }

    public class GridInfo {

        public List<Tile> unreachableTiles;
        public List<Tile> unPathableTiles;
        public bool allowEntitiesToBeTargeted;
        public bool useMovementPenalty;
        public bool useLineOfSight;
        public bool cleanse;
        public bool useThisIsCool;
        public BattleEntity bEntity;
    }

    public struct ThisIsCool {
        public List<Tile> outerTiles;
    }

    public static List<Tile> GetRangeObstacles(Tile startTile, int range, GridInfo gridInfo, out ThisIsCool cool) {

        List<TileRange> openSet = new List<TileRange>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        HashSet<Tile> finalSet = new HashSet<Tile>();
        cool.outerTiles = new List<Tile>();

        if(gridInfo == null) {
            gridInfo = new GridInfo();
            gridInfo.cleanse = false;
            gridInfo.useLineOfSight = false;
            gridInfo.useMovementPenalty = false;
            gridInfo.useThisIsCool = false;
            gridInfo.allowEntitiesToBeTargeted = false;
        }

        foreach(Tile t in GridManager.grid) {
            t.isAvailableToPathfinding = true;
            t.isClickableForInput = true;
        }

        openSet.Add(new TileRange(startTile, range));

        while(openSet.Count > 0) {

            TileRange currentTile = openSet[0];

            openSet.Remove(currentTile);
            finalSet.Add(currentTile.tile);
            cool.outerTiles.Add(currentTile.tile);

            foreach(Tile n in GetNeighbors(currentTile.tile)) {

                if(finalSet.Contains(n)) {
                    continue;
                }

                if(gridInfo.unreachableTiles != null) {
                    if(gridInfo.unreachableTiles.Contains(n) == true) {
                        continue;
                    }
                }

                int comparerWeight = BattleManager.distanceCoefficient;
                if(gridInfo.useMovementPenalty == true) {
                    comparerWeight = n.baseWeight;
                }

                if(currentTile.rangeLeft >= comparerWeight && ((n.isEmpty || gridInfo.allowEntitiesToBeTargeted) || gridInfo.useThisIsCool)) {

                    if(cool.outerTiles.Contains(currentTile.tile)) {
                        cool.outerTiles.Remove(currentTile.tile);
                    }

                    if(gridInfo.cleanse) {
                        if(gridInfo.unPathableTiles.Contains(n)) {
                            finalSet.Add(n);
                        }
                        else if(closedSet.Contains(n) == false) {
                            openSet.Add(new TileRange(n, currentTile.rangeLeft - comparerWeight));
                            closedSet.Add(n);
                        }
                    }
                    else if(closedSet.Contains(n) == false) {
                        openSet.Add(new TileRange(n, currentTile.rangeLeft - comparerWeight));
                        closedSet.Add(n);
                    }
                    else {
                        finalSet.Add(n);
                    }
                }
            }
        }

        List<Tile> result = new List<Tile>();

        foreach(Tile t in GridManager.grid) {
            if(finalSet.Contains(t))
                result.Add(t);
            else {
                t.isAvailableToPathfinding = false;
                t.isClickableForInput = false;
            }
        }

        result.Remove(startTile);

        return result;
    }
    /*
    public static Tile[] GetPathForWaypointedEntity(BattleEntity entity, bool currentlyHittingATile, Tile tileHit, out int cost) {

        Tile[] path;
        Tile[] pathA;
        Tile[] pathB;

        path = Pathfinding.FindPath(entity.tile, entity.waypoints[0], out cost);

        if(path == null) {
            // Entity cannot trace its path to its first waypoint.
            return null;
        }

        for(int i = 0; i < entity.waypoints.Count - 1; i++) {
            pathA = path;
            pathB = FindPath(entity.waypoints[i], entity.waypoints[i + 1], out cost);
            if(pathB == null) {
                return null;
            }
            path = new Tile[pathA.Length + pathB.Length];
            pathA.CopyTo(path, 0);
            pathB.CopyTo(path, pathA.Length);
        }

        if(currentlyHittingATile == true) {
            pathA = path;
            pathB = FindPath(entity.waypoints[entity.waypoints.Count - 1], tileHit, out cost);
            if(pathB == null) {
                return null;
            }
            path = new Tile[pathA.Length + pathB.Length];
            pathA.CopyTo(path, 0);
            pathB.CopyTo(path, pathA.Length);
        }

        return path;
    }*/

    public static List<Tile> GetNeighbors(Tile tile) {

        List<Tile> neighbors = new List<Tile>();

        AddNeighbor(ref neighbors, GetDirectionCube(tile, Hex.CubeDirections.Left));
        AddNeighbor(ref neighbors, GetDirectionCube(tile, Hex.CubeDirections.TopLeft));
        AddNeighbor(ref neighbors, GetDirectionCube(tile, Hex.CubeDirections.Top));
        AddNeighbor(ref neighbors, GetDirectionCube(tile, Hex.CubeDirections.TopRight));
        AddNeighbor(ref neighbors, GetDirectionCube(tile, Hex.CubeDirections.Right));
        AddNeighbor(ref neighbors, GetDirectionCube(tile, Hex.CubeDirections.BottomRight));
        AddNeighbor(ref neighbors, GetDirectionCube(tile, Hex.CubeDirections.Bottom));
        AddNeighbor(ref neighbors, GetDirectionCube(tile, Hex.CubeDirections.BottomLeft));

        return neighbors;
    }

    static void AddNeighbor(ref List<Tile> neighbors, Tile tile) {

        if(neighbors.Contains(tile) == false && tile != null)
            neighbors.Add(tile);
    }

    static Tile GetDirectionCube(Tile tile, Hex.CubeDirections dir) {

        Hex.OffsetPairs offsetPairs = Hex.GetNeighborFromDir(GridManager.hex, dir, new Hex.OffsetPairs(tile.position.x, tile.position.y));

        if((offsetPairs.row == tile.position.x && offsetPairs.col == tile.position.y) || offsetPairs.col == -1 || offsetPairs.row == -1)
            return null;

        offsetPairs.row = Mathf.Clamp(offsetPairs.row, 0, GridManager.grid.GetLength(0) - 1);
        offsetPairs.col = Mathf.Clamp(offsetPairs.col, 0, GridManager.grid.GetLength(1) - 1);

        return GridManager.grid[offsetPairs.row, offsetPairs.col];
    }

    struct TileRange {
        public int rangeLeft;
        public Tile tile;

        public TileRange(Tile tile, int rangeLeft) {
            this.tile = tile;
            this.rangeLeft = rangeLeft;
        }
    }
}