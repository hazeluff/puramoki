using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class PartDatabase<P> : ScriptableObject where P : IDatabasePart {

    [SerializeField]
    private List<P> _parts;
    public List<P> Parts => _parts;

    public P Get(string id) {
        return _parts.Find(e => id.Equals(e.Id));
    }
}