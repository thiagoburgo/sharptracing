using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DrawEngine.SharpTracingUI.Util
{
    public static class EventsExtensions
    {
        public static void CopyEvents<T>(this T from, T to) {
            EventInfo[] events = from.GetType().GetEvents();
            foreach (EventInfo eventInfo in events) {
                Delegate[] subscribers = GetEventSubscribers(from, eventInfo.Name);
                foreach (Delegate subscriber in subscribers) {
                    eventInfo.AddEventHandler(to, subscriber);    
                }
                
            }
        }
        private static Delegate[] GetEventSubscribers(object target, string eventName)
        {
            string winFormsEventName = "Event" + eventName;
            Type t = target.GetType();

            do
            {

                FieldInfo[] fia = t.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (FieldInfo fi in fia)
                {
                    if (fi.Name == eventName)
                    {
                        //we've found the compiler generated event
                        Delegate d = fi.GetValue(target) as Delegate;
                        if (d != null)
                            return d.GetInvocationList();
                    }
                    if (fi.Name == winFormsEventName)
                    {
                        //we've found an EventHandlerList key
                        //get the list
                        EventHandlerList ehl = (EventHandlerList)target.GetType().GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).GetValue(target, null);
                        //and dereference the delegate.
                        Delegate d = ehl[fi.GetValue(target)];
                        if (d != null)
                            return d.GetInvocationList();
                    }
                }
                t = t.BaseType;
            } while (t != null);

            return new Delegate[] { };
        }

    }
    

}
