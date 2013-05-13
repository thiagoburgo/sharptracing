/**
 * Criado por: Thiago Burgo Belo (thiagoburgo@gmail.com)
 * SharpTracing é um projeto feito inicialmente para disciplina
 * Computação Gráfica da UFPE e depois melhorado nos tempos livres
 * sinta-se a vontade para copiar modificar e mandar correções e 
 * sugestões. Mantenha os créditos!
 * **************************************************************
 * SharpTracing is a project originally created to discipline 
 * Computer Graphics of UFPE and was improved in my free time.
 * Feel free to copy, modify and  give fixes 
 * suggestions. Keep the credits!
 */

using System;
using System.Collections.Generic;

namespace DrawEngine.Renderer.Lights {
    public class LightDictionary : Dictionary<String, Light> {
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

        public new ICollection<string> Keys {
            get { return base.Keys; }
        }

        public new ICollection<Light> Values {
            get { return base.Values; }
        }

        public new Light this[string key] {
            get { return base[key]; }
            set { base[key] = value; }
        }

        public new void Add(string key, Light value) {
            base.Add(key, value);
            this.OnLightAdded(value);
        }

        public new bool ContainsKey(string key) {
            return base.ContainsKey(key);
        }

        public new bool Remove(string key) {
            Light light = base[key];
            if (base.Remove(key)) {
                return true;
            }
            return false;
        }

        public new bool TryGetValue(string key, out Light value) {
            return base.TryGetValue(key, out value);
        }
    }
}