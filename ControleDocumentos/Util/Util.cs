using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControleDocumentos.Util
{
    public static class Generics
    {
        public static T ComparaValores<T>(T old, T to, params string[] alterar) where T : class
        {
            if (old != null && to != null)
            {
                Type type = typeof(T);
                List<string> alterarList = new List<string>(alterar);
                foreach (System.Reflection.PropertyInfo pi in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    if (alterar.Contains(pi.Name))
                    {
                        object selfValue = type.GetProperty(pi.Name).GetValue(old, null);
                        object toValue = type.GetProperty(pi.Name).GetValue(to, null);

                       if(!(selfValue == toValue))
                        {
                            pi.SetValue(old, toValue);
                        }
                    }
                }
            }
            return old;
        }
    }
}