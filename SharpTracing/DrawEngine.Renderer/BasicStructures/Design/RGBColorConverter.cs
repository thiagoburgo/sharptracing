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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;

namespace DrawEngine.Renderer.BasicStructures.Design {
    /// <summary>Converts colors from one data type to another. Access this class through the <see cref="T:System.ComponentModel.TypeDescriptor"></see>.</summary>
    /// <filterpriority>1</filterpriority>
    public class RGBColorConverter : TypeConverter {
        private static Hashtable colorConstants;
        private static readonly string ColorConstantsLock;
        private static Hashtable systemColorConstants;
        private static readonly string SystemColorConstantsLock;
        private static StandardValuesCollection values;
        private static readonly string ValuesLock;

        static RGBColorConverter() {
            ColorConstantsLock = "colorConstants";
            SystemColorConstantsLock = "systemColorConstants";
            ValuesLock = "values";
        }

        private static Hashtable Colors {
            get {
                if (colorConstants == null) {
                    lock (ColorConstantsLock) {
                        if (colorConstants == null) {
                            Hashtable hashtable1 = new Hashtable(StringComparer.OrdinalIgnoreCase);
                            FillConstants(hashtable1, typeof (Color));
                            colorConstants = hashtable1;
                        }
                    }
                }
                return colorConstants;
            }
        }

        private static Hashtable SystemColors {
            get {
                if (systemColorConstants == null) {
                    lock (SystemColorConstantsLock) {
                        if (systemColorConstants == null) {
                            Hashtable hashtable1 = new Hashtable(StringComparer.OrdinalIgnoreCase);
                            FillConstants(hashtable1, typeof (SystemColors));
                            systemColorConstants = hashtable1;
                        }
                    }
                }
                return systemColorConstants;
            }
        }

        /// <summary>Determines if this converter can convert an object in the given source type to the native type of the converter.</summary>
        /// <returns>true if this object can perform the conversion; otherwise, false.</returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context. You can use this object to get additional information about the environment from which this converter is being invoked. </param>
        /// <param name="sourceType">The type from which you want to convert. </param>
        /// <filterpriority>1</filterpriority>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof (string)) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>Returns a value indicating whether this converter can convert an object to the given destination type using the context.</summary>
        /// <returns>true if this converter can perform the operation; otherwise, false.</returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context. </param>
        /// <param name="destinationType">A <see cref="T:System.Type"></see> that represents the type to which you want to convert. </param>
        /// <filterpriority>1</filterpriority>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof (InstanceDescriptor)) {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>Converts the given object to the converter's native type.</summary>
        /// <returns>An <see cref="T:System.Object"></see> representing the converted value.</returns>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"></see> that specifies the culture to represent the color. </param>
        /// <param name="context">A <see cref="T:System.ComponentModel.TypeDescriptor"></see> that provides a format context. You can use this object to get additional information about the environment from which this converter is being invoked. </param>
        /// <param name="value">The object to convert. </param>
        /// <exception cref="T:System.ArgumentException">The conversion cannot be performed.</exception>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /></PermissionSet>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            string text1 = value as string;
            if (text1 == null) {
                return base.ConvertFrom(context, culture, value);
            }
            object obj1 = null;
            string text2 = text1.Trim();
            if (text2.Length == 0) {
                return RGBColor.FromColor(Color.Empty);
            }
            obj1 = GetNamedColor(text2);
            if (obj1 == null) {
                if (culture == null) {
                    culture = CultureInfo.CurrentCulture;
                }
                char ch1 = culture.TextInfo.ListSeparator[0];
                bool flag1 = true;
                TypeConverter converter1 = TypeDescriptor.GetConverter(typeof (int));
                if (text2.IndexOf(ch1) == -1) {
                    if (((text2.Length >= 2) && ((text2[0] == '\'') || (text2[0] == '"'))) &&
                        (text2[0] == text2[text2.Length - 1])) {
                        string text3 = text2.Substring(1, text2.Length - 2);
                        obj1 = Color.FromName(text3);
                        flag1 = false;
                    } else if ((((text2.Length == 7) && (text2[0] == '#')) ||
                                ((text2.Length == 8) && (text2.StartsWith("0x") || text2.StartsWith("0X")))) ||
                               ((text2.Length == 8) && (text2.StartsWith("&h") || text2.StartsWith("&H")))) {
                        obj1 =
                            Color.FromArgb(-16777216 | ((int) converter1.ConvertFromString(context, culture, text2)));
                    }
                }
                if (obj1 == null) {
                    string[] textArray1 = text2.Split(new char[] {ch1});
                    int[] numArray1 = new int[textArray1.Length];
                    for (int num1 = 0; num1 < numArray1.Length; num1++) {
                        numArray1[num1] = (int) converter1.ConvertFromString(context, culture, textArray1[num1]);
                    }
                    switch (numArray1.Length) {
                        case 1:
                            obj1 = Color.FromArgb(numArray1[0]);
                            break;
                        case 3:
                            obj1 = Color.FromArgb(numArray1[0], numArray1[1], numArray1[2]);
                            break;
                        case 4:
                            obj1 = Color.FromArgb(numArray1[0], numArray1[1], numArray1[2], numArray1[3]);
                            break;
                    }
                    flag1 = true;
                }
                if ((obj1 != null) && flag1) {
                    int num2 = ((Color) obj1).ToArgb();
                    foreach (Color color1 in Colors.Values) {
                        if (color1.ToArgb() == num2) {
                            obj1 = color1;
                            break;
                        }
                    }
                }
            }
            if (obj1 == null) {
                throw new ArgumentException("InvalidColor");
            }
            return RGBColor.FromColor((Color) obj1);
        }

        /// <summary>Converts the specified object to another type. </summary>
        /// <returns>An <see cref="T:System.Object"></see> representing the converted value.</returns>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"></see> that specifies the culture to represent the color. </param>
        /// <param name="context">A formatter context. Use this object to extract additional information about the environment from which this converter is being invoked. Always check whether this value is null. Also, properties on the context object may return null. </param>
        /// <param name="destinationType">The type to convert the object to. </param>
        /// <param name="value">The object to convert. </param>
        /// <exception cref="T:System.ArgumentNullException">destinationtype is null.</exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed.</exception>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /></PermissionSet>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType) {
            if (destinationType == null) {
                throw new ArgumentNullException("destinationType");
            }
            if (value is RGBColor) {
                value = ((RGBColor) value).ToColor();
            }
            if (value is Color) {
                if (destinationType == typeof (string)) {
                    string[] textArray1;
                    Color color1 = (Color) value;
                    if (color1 == Color.Empty) {
                        return string.Empty;
                    }
                    if (color1.IsKnownColor) {
                        return color1.Name;
                    }
                    if (color1.IsNamedColor) {
                        return ("'" + color1.Name + "'");
                    }
                    if (culture == null) {
                        culture = CultureInfo.CurrentCulture;
                    }
                    string text1 = culture.TextInfo.ListSeparator + " ";
                    TypeConverter converter1 = TypeDescriptor.GetConverter(typeof (int));
                    int num1 = 0;
                    if (color1.A < 0xff) {
                        textArray1 = new string[4];
                        textArray1[num1++] = converter1.ConvertToString(context, culture, color1.A);
                    } else {
                        textArray1 = new string[3];
                    }
                    textArray1[num1++] = converter1.ConvertToString(context, culture, color1.R);
                    textArray1[num1++] = converter1.ConvertToString(context, culture, color1.G);
                    textArray1[num1++] = converter1.ConvertToString(context, culture, color1.B);
                    return string.Join(text1, textArray1);
                }
                if (destinationType == typeof (InstanceDescriptor)) {
                    MemberInfo info1 = null;
                    object[] objArray1 = null;
                    Color color2 = (Color) value;
                    if (color2.IsEmpty) {
                        info1 = typeof (Color).GetField("Empty");
                    } else if (color2.IsSystemColor) {
                        info1 = typeof (SystemColors).GetProperty(color2.Name);
                    } else if (color2.IsKnownColor) {
                        info1 = typeof (Color).GetProperty(color2.Name);
                    } else if (color2.A != 0xff) {
                        info1 = typeof (Color).GetMethod("FromArgb",
                                                         new Type[]
                                                         {typeof (int), typeof (int), typeof (int), typeof (int)});
                        objArray1 = new object[] {color2.A, color2.R, color2.G, color2.B};
                    } else if (color2.IsNamedColor) {
                        info1 = typeof (Color).GetMethod("FromName", new Type[] {typeof (string)});
                        objArray1 = new object[] {color2.Name};
                    } else {
                        info1 = typeof (Color).GetMethod("FromArgb",
                                                         new Type[] {typeof (int), typeof (int), typeof (int)});
                        objArray1 = new object[] {color2.R, color2.G, color2.B};
                    }
                    if (info1 != null) {
                        return new InstanceDescriptor(info1, objArray1);
                    }
                    return null;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static void FillConstants(Hashtable hash, Type enumType) {
            MethodAttributes attributes1 = MethodAttributes.Static | MethodAttributes.Public;
            foreach (PropertyInfo info1 in enumType.GetProperties()) {
                if (info1.PropertyType == typeof (Color)) {
                    MethodInfo info2 = info1.GetGetMethod();
                    if ((info2 != null) && ((info2.Attributes & attributes1) == attributes1)) {
                        object[] objArray1 = null;
                        hash[info1.Name] = info1.GetValue(null, objArray1);
                    }
                }
            }
        }

        internal static object GetNamedColor(string name) {
            object obj1 = null;
            obj1 = Colors[name];
            if (obj1 != null) {
                return obj1;
            }
            return SystemColors[name];
        }

        /// <summary>Retrieves a collection containing a set of standard values for the data type for which this validator is designed. This will return null if the data type does not support a standard set of values.</summary>
        /// <returns>A collection containing null or a standard set of valid values. The default implementation always returns null.</returns>
        /// <param name="context">A formatter context. Use this object to extract additional information about the environment from which this converter is being invoked. Always check whether this value is null. Also, properties on the context object may return null. </param>
        /// <filterpriority>1</filterpriority>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            if (values == null) {
                lock (ValuesLock) {
                    if (values == null) {
                        ArrayList list1 = new ArrayList();
                        list1.AddRange(Colors.Values);
                        list1.AddRange(SystemColors.Values);
                        int num1 = list1.Count;
                        for (int num2 = 0; num2 < (num1 - 1); num2++) {
                            for (int num3 = num2 + 1; num3 < num1; num3++) {
                                if (list1[num2].Equals(list1[num3])) {
                                    list1.RemoveAt(num3);
                                    num1--;
                                    num3--;
                                }
                            }
                        }
                        list1.Sort(0, list1.Count, new ColorComparer());
                        values = new StandardValuesCollection(list1.ToArray());
                    }
                }
            }
            return values;
        }

        /// <summary>Determines if this object supports a standard set of values that can be chosen from a list.</summary>
        /// <returns>true if <see cref="Overload:System.Drawing.RGBColorConverter.GetStandardValues"></see> must be called to find a common set of values the object supports; otherwise, false.</returns>
        /// <param name="context">A <see cref="T:System.ComponentModel.TypeDescriptor"></see> through which additional context can be provided. </param>
        /// <filterpriority>1</filterpriority>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }

        #region Nested type: ColorComparer

        private class ColorComparer : IComparer {
            #region IComparer Members

            public int Compare(object left, object right) {
                Color color1 = (Color) left;
                Color color2 = (Color) right;
                return string.Compare(color1.Name, color2.Name, false, CultureInfo.InvariantCulture);
            }

            #endregion
        }

        #endregion
    }
}