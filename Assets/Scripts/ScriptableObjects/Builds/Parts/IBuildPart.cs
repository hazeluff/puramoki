using Newtonsoft.Json.Linq;
using UnityEngine;

public interface IBuildPart {
    string Name { get; }

    int Hp { get; }
    int Ep { get; }
    int Atk { get; }
    int Def { get; }
    int Acc { get; }
    int Eva { get; }
    int Spd { get; }
    int Rng { get; }
    int Mv { get; }

    GameObject Model { get; }

    float ElemRes(Element element);

    JObject ToSaveJson();
}
