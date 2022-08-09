using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class PartDatabase<P> : ScriptableObject where P : ScriptablePart {

    public List<Entry> parts;

    public P Get(string id) {
        Entry entry = parts.Find(e => id.Equals(e.id));
        if (entry != null) {
            return entry.part;
        }
        return null;
    }

    [Serializable]
    public class Entry {
        public string id;
        public P part;
    }
}