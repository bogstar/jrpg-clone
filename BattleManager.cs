using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour {


    public static readonly int distanceCoefficient = 100;

    public static BattleManager instance;

    public GameObject entityInfoWindow;

    public GameObject dummyCharacterGraphics;

    public List<BattleEntity> players {
        get {
            List<BattleEntity> result = new List<BattleEntity>();
            foreach(BattleEntity be in battleEntities) {
                if(be.entityType == Tile.EntityType.Player) {
                    result.Add(be);
                }
            }
            return result;
        }
    }
    public List<BattleEntity> enemies {
        get {
            List<BattleEntity> result = new List<BattleEntity>();
            foreach(BattleEntity be in battleEntities) {
                if(be.entityType == Tile.EntityType.Enemy) {
                    result.Add(be);
                }
            }
            return result;
        }
    }
    public List<BattleEntity> battleEntities;

    BattleEntity entityOnTurn;
    int entityOnTurnIndex;


    void Awake() {

        if(instance == null)
            instance = this;
        else
            GameObject.DestroyImmediate(this);
    }

    void Update() {

        GetInput();
    }

    public void StartBattle(BattleData battleData, List<Entity> players) {

        GridManager.instance.GenerateGrid(battleData);

        battleEntities.AddRange(GridManager.instance.SpawnEnemies(battleData.enemies));

        foreach(BattleEntity be in enemies) {
            be.entity.stats.health = be.entity.stats.maxHealth;
            be.entity.stats.mana = be.entity.stats.maxMana;
        }

        battleEntities.AddRange(GridManager.instance.SpawnPlayers(players));

        currentMenuSelection = 0;
        MenuButtonPressed(0);

        inputPhase = InputPhase.NoInput;

        entityOnTurnIndex = enemies.Count;
        StartTurnForEntity(battleEntities[entityOnTurnIndex]);
    }

    string lastHit;
    bool update;
    bool holdingShift;
    bool mouseClickAllowed;
    int cost;


    void GetInput() {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        bool currentlyHittingATile = false;

        Tile tileHit = null;
        string currentTileName = "";

        Physics.Raycast(ray, out hit);

        if(hit.transform != null) {
            if(hit.transform.parent.GetComponent<Tile>() != null) {
                currentlyHittingATile = true;
                tileHit = hit.transform.parent.GetComponent<Tile>();
                currentTileName = tileHit.name;
            }
            else {
                currentlyHittingATile = false;
                tileHit = null;
                currentTileName = "";
            }
        }
        else {
            currentlyHittingATile = false;
            tileHit = null;
            currentTileName = "";
        }

        if(lastHit != currentTileName) {
            lastHit = currentTileName;
            update = true;
        }

        if(inputPhase == InputPhase.NoInput || entityOnTurn.lowerBodyAvailable == false) {
            return;
        }

        if(inputPhase == InputPhase.Move || inputPhase == InputPhase.Battle) {
            GetMouseInput(tileHit);
        }

        if(update == true) {

            bool isClickable = false;
            mouseClickAllowed = false;

            if(currentlyHittingATile) {
                isClickable = tileHit.isClickableForInput;
            }

            if(characterMoving == false) {

                if(inputPhase == InputPhase.Looking) {

                    if(currentlyHittingATile == true) {
                        if(tileHit.entityPresent != Tile.EntityType.None && tileHit.entityPresent != Tile.EntityType.Rock) {
                            GridManager.instance.DisplayRange(tileHit.entityOnTile, RangeType.Any);
                            DisplayEntityInfoWindow(true, tileHit.entityOnTile);
                        }
                        else {
                            DisplayEntityInfoWindow(false, null);
                            GridManager.instance.ResetGridColor();
                        }
                    }
                    else {
                        DisplayEntityInfoWindow(false, null);
                        GridManager.instance.ResetGridColor();
                    }
                }
                else if(inputPhase == InputPhase.Battle) {

                    DisplayEntityInfoWindow(true, entityOnTurn);

                    List<BattleEntity> enemiesInMeleeRange = entityOnTurn.GetEntitiesInRange(entityOnTurn, RangeType.Melee, Relationship.Enemy);

                    GridManager.instance.DisplayRange(entityOnTurn, RangeType.Melee);
                }
                else if(inputPhase == InputPhase.Move) {

                    DisplayEntityInfoWindow(true, entityOnTurn);

                    if(currentlyHittingATile == true && isClickable == false) {
                        GridManager.instance.DisplayRelativeRangeAndPath(entityOnTurn, RangeType.Any);
                    }

                    else if(currentlyHittingATile == true && isClickable == true) {
                        mouseClickAllowed = true;

                        Pathfinding.GridInfo gridInfo = new Pathfinding.GridInfo();
                        gridInfo.unreachableTiles = new List<Tile>();

                        foreach(Tile t in GridManager.grid) {
                            if(!t.isAvailableToPathfinding) {
                                gridInfo.unreachableTiles.Add(t);
                            }
                        }
                        gridInfo.unreachableTiles.Remove(tileHit);
                        entityOnTurn.path = Pathfinding.FindPath(entityOnTurn.tile, tileHit, out cost, gridInfo);

                        GridManager.instance.DisplayRelativeRangeAndPath(entityOnTurn, RangeType.Any);
                    }

                    else {

                        GridManager.instance.DisplayRelativeRangeAndPath(entityOnTurn, RangeType.Any);
                    }
                }
            }

            update = false;
        }
    }

    void DisplayEntityInfoWindow(bool display, BattleEntity entity) {

        if(display && entity != null)
            UpdateEntityInfoWindow(entity);

        entityInfoWindow.SetActive(display);
    }

    void UpdateEntityInfoWindow(BattleEntity entity) {

        const float barLength = 80f;

        entityInfoWindow.transform.Find("Upper Half").Find("Name").GetChild(0).GetComponent<Text>().text = entity.name;
        entityInfoWindow.transform.Find("Upper Half").Find("HP MANA").Find("HP MANA VALUE").Find("HP").Find("Text").GetChild(0).GetComponent<Text>().text = entity.entity.stats.health + "/" + entity.entity.stats.maxHealth;
        entityInfoWindow.transform.Find("Upper Half").Find("HP MANA").Find("HP MANA VALUE").Find("MP").Find("Text").GetChild(0).GetComponent<Text>().text = entity.entity.stats.mana + "/" + entity.entity.stats.maxMana;
        float bar = barLength;
        if(entity.entity.stats.maxHealth != 0)
            bar = (entity.entity.stats.health / (float)entity.entity.stats.maxHealth) * barLength;
        entityInfoWindow.transform.Find("Upper Half").Find("HP MANA").Find("HP MANA VALUE").Find("HP").Find("Bar").Find("Green Bar").GetComponent<RectTransform>().offsetMax = new Vector2(bar, 0f);
        if(entity.entity.stats.maxMana != 0)
            bar = (entity.entity.stats.mana / (float)entity.entity.stats.maxMana) * barLength;
        entityInfoWindow.transform.Find("Upper Half").Find("HP MANA").Find("HP MANA VALUE").Find("MP").Find("Bar").Find("Blue Bar").GetComponent<RectTransform>().offsetMax = new Vector2(bar, 0f);

        entityInfoWindow.transform.Find("Bottom Half").Find("Stats Value").GetChild(0).GetChild(0).GetComponent<Text>().text = entity.entity.stats.attack + "\n" + entity.entity.stats.defence + "\n" + entity.entity.stats.agility + "\n" + entity.entity.stats.speed + "\n" + entity.entity.stats.attackPower;
    }

    void GetMouseInput(Tile tileHit) {

        if(mouseClickAllowed) {

            if(Input.GetMouseButtonDown(0)) {

                GridManager.instance.ResetGridColor();

                List<BattleEntity> enemiesOnTargetTile = tileHit.GetEntitiesInRange(entityOnTurn, RangeType.Intimidation, Relationship.Enemy);

                bool flag = false;
                foreach(BattleEntity be in enemiesOnTargetTile) {
                    if(!entityOnTurn.bEntitiesInIntimidationRange.Contains(be))
                        flag = true;
                }
                    
                if(!flag)
                    entityOnTurn.ModifyRange(-cost);
                else
                    entityOnTurn.ModifyRange(-entityOnTurn.movementRange);

                MoveEntity(entityOnTurn, entityOnTurn.path);
            }
        }
    }

    void StartTurnForEntity(BattleEntity entity) {

        ModifyLowerBody(entity, true);
        ModifyUpperBody(entity, true);

        entity.ProcessTick();

        entity.SetRangeToMax();
        entity.RecalculateEnemiesInIntimidationRange();
        entity.CalculateReachableAreas();
        BattleManager.instance.CalculateSharedIntimidationRanges();

        entityOnTurn = entity;
    }

    public void EndTurnForEntity() {
        EndTurnForEntity(entityOnTurn);
    }

    void EndTurnForEntity(BattleEntity entity) {

        entityOnTurn = null;

        entityOnTurnIndex += 1;
        entityOnTurnIndex %= battleEntities.Count;
        StartTurnForEntity(battleEntities[entityOnTurnIndex]);
    }

    bool characterMoving;

    void MoveEntity(BattleEntity bEntity, Tile[] path) {

        GridManager.instance.ResetGridColor();
        characterMoving = true;

        bEntity.MoveCharacter(path, OnEntityMoved);
    }

    void OnEntityMoved(BattleEntity entity) {

        characterMoving = false;

        inputPhase = InputPhase.NoInput;

        entity.CalculateRanges();
        entity.RecalculateEnemiesInIntimidationRange();
        entity.CalculateReachableAreas();

        List<Tile> neighbors = Pathfinding.GetNeighbors(entity.tile);
        bool okay = false;

        foreach(Tile n in neighbors) {
            if(n.baseWeight <= entity.movementRange) {
                okay = true;
                break;
            }
        }

        if(okay == false) {
            ModifyLowerBody(entity, false);
            
        }

        if(entity.lowerBodyAvailable == false && entity.upperBodyAvailable == false) {
            EndTurnForEntity(entity);
        }
    }

    void CalculateSharedIntimidationRanges() {

        Dictionary<BattleEntity, List<BattleEntity>> battleEntitiesSharingIntimidationRange = new Dictionary<BattleEntity, List<BattleEntity>>();

        foreach(BattleEntity be in enemies) {

            List<BattleEntity> list = new List<BattleEntity>();

            foreach(BattleEntity ally in be.GetEntitiesInRange(be, RangeType.Intimidation, Relationship.Ally)) {
                list.Add(ally);
            }

            battleEntitiesSharingIntimidationRange.Add(be, list);
        }

        foreach(KeyValuePair<BattleEntity, List<BattleEntity>> sharer in battleEntitiesSharingIntimidationRange) {
            foreach(BattleEntity be in sharer.Value) {
                if(battleEntitiesSharingIntimidationRange.ContainsKey(be)) {
                    if(!battleEntitiesSharingIntimidationRange[be].Contains(sharer.Key)) {
                        battleEntitiesSharingIntimidationRange[be].Add(sharer.Key);
                    }
                }
            }
        }

        foreach(KeyValuePair<BattleEntity, List<BattleEntity>> sharer in battleEntitiesSharingIntimidationRange) {
            sharer.Key.alliesSharingIntimidationRange = sharer.Value;
        }

        foreach(KeyValuePair<BattleEntity, List<BattleEntity>> sharer in battleEntitiesSharingIntimidationRange) {
            List<BattleEntity> closedSet = new List<BattleEntity>();
            closedSet = GetAllAlliesRecursive(sharer.Value, closedSet);
            sharer.Key.alliesSharingIntimidationRange = closedSet;
        }
    }

    List<BattleEntity> GetAllAlliesRecursive(List<BattleEntity> list, List<BattleEntity> closedSet) {

        foreach(BattleEntity be in list) {

            if(!closedSet.Contains(be)) {
                closedSet.Add(be);
                closedSet = GetAllAlliesRecursive(be.alliesSharingIntimidationRange, closedSet);
            }
        }
        return closedSet;
    }

    public InputPhase inputPhase;
    public enum InputPhase { Move, Looking, Battle, NoInput };

    public void iwantMove() {

        MenuButtonPressed(0);
        update = true;
        inputPhase = InputPhase.Move;
        GridManager.instance.DisplayRelativeRangeAndPath(entityOnTurn, RangeType.Any);
    }

    public void iwantNothing() {

        MenuButtonPressed(0);
        update = true;
        inputPhase = InputPhase.Looking;
        entityOnTurn.path = null;
        GridManager.instance.ResetGridColor();
    }

    public void iwantBattle() {

        MenuButtonPressed(0);
        update = true;
        inputPhase = InputPhase.Battle;
    }

    public void iwantPass() {

        MenuButtonPressed(0);
        update = true;
        EndTurnForEntity();
    }

    void ModifyUpperBody(BattleEntity entity, bool modify) {
        handsButton.interactable = modify;
        lipsButton.interactable = false;
        activeItemButton.interactable = false;
        upperBodyButton.interactable = modify;
        entity.upperBodyAvailable = modify;
    }

    void ModifyLowerBody(BattleEntity entity, bool modify) {
        kickButton.interactable = false;
        moveButton.interactable = modify;
        lowerBodyButton.interactable = modify;
        entity.lowerBodyAvailable = modify;
    }

    public Button upperBodyButton;
    public Button lowerBodyButton;
    public Button handsButton;
    public Button lipsButton;
    public Button activeItemButton;
    public Button moveButton;
    public Button kickButton;

    public GameObject upperBodyMenu;
    public GameObject lowerBodyMenu;
    public GameObject inventoryMenu;

    public int currentMenuSelection;


    public void MenuButtonPressed(int i) {

        upperBodyMenu.SetActive(false);
        lowerBodyMenu.SetActive(false);
        inventoryMenu.SetActive(false);

        switch(i) {
            case 0:
                currentMenuSelection = 0;
                break;
            case 1:
                if(currentMenuSelection != 1) {
                    currentMenuSelection = 1;
                    upperBodyMenu.SetActive(true);
                }
                else
                    currentMenuSelection = 0;
                break;
            case 2:
                if(currentMenuSelection != 2) {
                    currentMenuSelection = 2;
                    lowerBodyMenu.SetActive(true);
                }
                else
                    currentMenuSelection = 0;
                break;
            case 3:
                if(currentMenuSelection != 3) {
                    currentMenuSelection = 3;
                    inventoryMenu.SetActive(true);
                }
                else
                    currentMenuSelection = 0;
                break;
        }
    }
}