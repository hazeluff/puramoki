using UnityEngine;
using UnityEditor;

public class Factions {
    private static readonly string FOLDER = "ScriptableObjects/Factions/";

    public static Faction GOOD = LoadFaction("GoodFaction");
    public static Faction BAD = LoadFaction("BadFaction");
    public static Faction NEUTRAL = LoadFaction("NeutralFaction");

    private static Faction LoadFaction(string fileName) {
        return Resources.Load<Faction>(FOLDER + fileName);
    }
}