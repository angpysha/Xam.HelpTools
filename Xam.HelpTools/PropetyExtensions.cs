using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Xam.HelpTools
{
    public static class PropetyExtensions
    {
        public static PropertyDescriptor ToPropertyDescriptor(this PropertyInfo propertyInfo)
        {
            return TypeDescriptor.GetProperties(propertyInfo.DeclaringType).Find(propertyInfo.Name,false);
        }
    }
}
