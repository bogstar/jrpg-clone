using UnityEngine;
using System.Collections;

[System.Serializable]
public class Entity {

    public string id;
    public string name;
    public float height;
    public GameObject graphics;
    public Stats stats;

    [System.Serializable]
    public struct Stats {
        public int level;
        public int experience;
        public int health;
        public int maxHealth;
        public int mana;
        public int maxMana;

        public int attack;
        public int defence;
        public int agility;
        public int speed;
        public int attackPower;

        public int movementRange;
        public int meleeRange;
        public int intimidationRange;

        public int retaliationCharges;
    }
}