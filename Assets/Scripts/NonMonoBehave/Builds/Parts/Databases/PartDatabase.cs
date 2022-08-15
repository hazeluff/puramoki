using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class PartDatabase<P> : ScriptableObject where P : BuildPart {

    public List<P> parts;

    public P Get(string id) {
        return parts.Find(e => id.Equals(e.Id));
    }
}