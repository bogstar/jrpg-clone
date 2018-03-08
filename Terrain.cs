using UnityEngine;
using System.Collections;

namespace JRPG {

    [System.Serializable]
    public class Terrain {

        public string id;
        public GameObject tileModel;
        [Range(float.Epsilon, float.MaxValue)]
        [Tooltip("As in percentage multiplier (ex. 0.5f)")]
        public float movementPenaltyMultiplier;
    }
}