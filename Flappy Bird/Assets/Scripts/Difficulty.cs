using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Difficulty {

    static float timeToHardest = 60f;
    public static float gameStartTime;
    public static float offset;

    public static float GetDifficulty() {
        return Mathf.Clamp01((Time.time - gameStartTime + offset) / timeToHardest);
    }

}
