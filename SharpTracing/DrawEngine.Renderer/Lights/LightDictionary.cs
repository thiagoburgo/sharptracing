using System;
using System.Collections.Generic;

namespace DrawEngine.Renderer.Lights
{
    public class LightDictionary : Dictionary<String, Light>
    {
        #region EVENTOS

        #region Delegates
        public delegate void LightEventHandler(Light light);
        #endregion

        public event LightEventHandler OnLightAdded;
        public event LightEventHandler OnLightRemoved;
        #endregion

        public LightDictionary() : base() {}
        public LightDictionary(int capacity) : base(capacity) {}
        public LightDictionary(IDictionary<String, Light> dictionary) : base(dictionary) {}
        public new ICollection<string> Keys
        {
            get { return base.Keys; }
        }
        public new ICollection<Light> Values
        {
            get { return base.Values; }
        }
        public new Light this[string key]
        {
            get { return base[key]; }
            set { base[key] = value; }
        }
        public new void Add(string key, Light value)
        {
            base.Add(key, value);
            this.OnLightAdded(value);
        }
        public new bool ContainsKey(string key)
        {
            return base.ContainsKey(key);
        }
        public new bool Remove(string key)
        {
            Light light = base[key];
            if(base.Remove(key)){
                return true;
            }
            return false;
        }
        public new bool TryGetValue(string key, out Light value)
        {
            return base.TryGetValue(key, out value);
        }
    }
}