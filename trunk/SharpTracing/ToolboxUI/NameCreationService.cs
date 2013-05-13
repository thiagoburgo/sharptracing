using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace ToolBoxUI.Components {
    public class NameCreationService : INameCreationService {
        #region INameCreationService Members

        public string CreateName(IContainer container, Type dataType) {
            string name = Char.ToLower(dataType.Name[0]) + dataType.Name.Substring(1);
            int i = 1;
            while (true) {
                if (container.Components[name + i.ToString()] == null) {
                    return name + i.ToString();
                }
                i++;
            }
        }

        public bool IsValidName(string name) {
            if (name == null || name.Length == 0) {
                return false;
            }
            if (!Char.IsLetter(name, 0)) {
                return false;
            }
            if (name.StartsWith("_")) {
                return false;
            }
            for (int i = 0; i < name.Length; i++) {
                if (!Char.IsLetterOrDigit(name, i)) {
                    return false;
                }
            }
            return true;
        }

        public void ValidateName(string name) {
            if (!this.IsValidName(name)) {
                throw new ArgumentException(string.Format("Неверное имя для компонента: {0}", name));
            }
        }

        #endregion
    }
}