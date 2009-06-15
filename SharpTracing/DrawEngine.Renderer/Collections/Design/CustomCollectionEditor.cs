using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.Collections.Design
{
    public class CustomCollectionEditor : CollectionEditor
    {
        public CustomCollectionEditor(Type type) : base(type) {}
        protected override Type[] CreateNewItemTypes()
        {
            Type type = base.CreateNewItemTypes()[0];
            //Assembly[] assembliesLoaded = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> listType = new List<Type>();
            if(!type.IsAbstract){
                listType.Add(type);
            }
            //foreach(Assembly loaded in assembliesLoaded) {
            Assembly loaded = Assembly.GetAssembly(type);
            foreach(Type typeTemp in loaded.GetExportedTypes().OrderBy(t => t.Name)){
                ConstructorInfo[] infos = typeTemp.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                ConstructorInfo info = infos.FirstOrDefault(x => x.GetParameters().Length == 0);
                if(!typeTemp.IsAbstract && info != null){
                    if(typeTemp.IsSubclassOf(type)){
                        listType.Add(typeTemp);
                    }
                }
            }
            //}
            return listType.ToArray();
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            INotify notify = value as INotify;
            if(notify != null && notify.NotificationsEnabled){
                notify.NotificationsEnabled = false;
            }
            object val = base.EditValue(context, provider, value);
            if(context.Instance is IPreprocess){
                ((IPreprocess)context.Instance).Preprocess();
            }
            if(notify != null){
                notify.NotificationsEnabled = true;
            }
            return val;
        }
    }
}