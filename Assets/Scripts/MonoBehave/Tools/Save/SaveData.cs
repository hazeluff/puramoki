using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public class SaveData {

    // Saved Data
    private long _lastSave;
    private readonly List<CoreUnitPart> _coreUnitParts;
    private readonly List<BodyPart> _bodyParts;
    private readonly List<ArmsPart> _armsParts;
    private readonly List<LowerPart> _lowerParts;
    private readonly List<WeaponPart> _weaponParts;
    private readonly List<UnitBuild> _builds;

    // Accessors
    public void UpdateSave(long saveTime) {
        this._lastSave = saveTime;
    }
    public long LastSave => _lastSave;
    public DateTime LastSaveDT => DateTimeOffset.FromUnixTimeMilliseconds(_lastSave).DateTime;
    public List<UnitBuild> Builds => _builds;
    public UnitBuild GetBuild(string buildName) {
        return Builds.First(build => build.name.Equals(buildName));
    }

    public SaveData(long lastSave, 
        List<CoreUnitPart> coreUnitParts, List<BodyPart> bodyParts, List<ArmsPart> armsParts, List<LowerPart> lowerParts, List<WeaponPart> weaponParts, 
        List<UnitBuild> builds) {
        this._lastSave = lastSave;
        this._coreUnitParts = coreUnitParts;
        this._bodyParts = bodyParts;
        this._armsParts = armsParts;
        this._lowerParts = lowerParts;
        this._weaponParts = weaponParts;
        this._builds = builds;
    }

    public JObject ToJson(long saveTime) {
        JObject json = new() {
            {   
                "lastSave", saveTime 
            },
            { "builds", JArray.FromObject(_builds.Select(build => {
                return new JObject() {
                    { "name", build.Name },
                    { "core", build.CoreUnitPart.Id },
                    { "arms", build.ArmsPart.Id },
                    { "body", build.BodyPart.Id },
                    { "lower", build.LowerPart.Id },
                    { "weapon", build.WeaponPart.Id }
                };
            }))},
            {
                "progress", new JObject() { {
                    "collection", new JObject() {
                        { "core", JArray.FromObject(_coreUnitParts.Select(core => core.ToSaveJson()).ToList()) },
                        { "body", JArray.FromObject(_bodyParts.Select(body => body.ToSaveJson()).ToList()) },
                        { "arms", JArray.FromObject(_armsParts.Select(arms => arms.ToSaveJson()).ToList()) },
                        { "lower", JArray.FromObject(_lowerParts.Select(lower => lower.ToSaveJson()).ToList()) },
                        { "weapon", JArray.FromObject(_weaponParts.Select(weapon => weapon.ToSaveJson()).ToList()) }
                    }
                }}
            }
        };
        return json;
    }

    public override string ToString() {
        return ToJson(LastSave).ToString(Formatting.None);
    }

    public struct PartMetadata {
        public readonly string id;
        public readonly int lvl;
        public readonly int xp;

        public PartMetadata(string id, int level, int currentExp) {
            this.id = id;
            this.lvl = level;
            this.xp = currentExp;
        }

        public PartMetadata(string id) {
            this.id = id;
            this.lvl = 0;
            this.xp = 0;
        }
    }
}
