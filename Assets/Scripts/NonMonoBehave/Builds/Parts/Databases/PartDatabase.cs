using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class PartDatabase<P> : ScriptableObject where P : ScriptablePart {

    public List<Entry> parts;

    public P Get(int id) {
        Entry entry = parts.Find(e => e.id == id);
        if (entry != null) {
            return entry.part;
        }
        return null;
    }

    [Serializable]
    public class Entry {
        public int id;
        public P part;
    }
}