using System;
using System.Collections.Generic;
using System.Text;

namespace ExtPlugins
{
    class Utils
    {
        public static object has_plugin(object pluginname)
        {
            return ExtPlugin.exists(pluginname);
        }

        public static object GetPrivate(object self, object name)
        {
            var type = this.GetType();
            var typefield = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            return typefield.GetValue(this);
        }

        public static object CallPrivate(object self, object name, object p)
        {
            var method = this.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            var ar = tuple(p);
            return method.Invoke(this, ar);
        }

        public static object GetPrivateStatic(object type, object name)
        {
            //return type.__dict__(name)
            //type = self.GetType()
            //typeof()
            //clr.GetClrType(str)
            var typefield = clr.GetClrType(type).GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Static);
            return typefield.GetValue(null);
        }
    }
}
