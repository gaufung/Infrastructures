﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Aiursoft.Pylon.Services
{
    public class InstranceMaker
    {
        private IList GetArrayWithInstanceInherts(Type itemType)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(itemType);
            var instance = (IList)Activator.CreateInstance(constructedListType);
            if (!itemType.IsAbstract)
            {
                instance.Add(Make(itemType));
            }
            foreach (var item in Assembly.GetEntryAssembly().GetTypes().Where(t => !t.IsAbstract).Where(t => t.IsSubclassOf(itemType)))
            {
                instance.Add(Make(item));
            }
            return instance;
        }

        public T Make<T>() where T : class, new()
        {
            return Make(typeof(T)) as T;
        }

        public object Make(Type type)
        {
            if (type == typeof(string))
            {
                return "an example string.";
            }
            else if (type == typeof(int))
            {
                return 0;
            }
            else if (type == typeof(bool))
            {
                return true;
            }
            // List
            else if (type.IsGenericType && type.GetGenericTypeDefinition().GetInterfaces().Any(t => t.IsAssignableFrom(typeof(IEnumerable))))
            {
                Type itemType = type.GetGenericArguments()[0];
                return GetArrayWithInstanceInherts(itemType);
            }
            // Array
            else if (type.GetInterface(typeof(IEnumerable<>).FullName) != null)
            {
                Type itemType = type.GetElementType();
                var list = GetArrayWithInstanceInherts(itemType);
                Array array = Array.CreateInstance(itemType, list.Count);
                list.CopyTo(array, 0);
                return array;
            }
            else
            {
                var instance = Assembly.GetAssembly(type).CreateInstance(type.FullName);
                foreach (var property in instance.GetType().GetProperties())
                {
                    property.SetValue(instance, Make(property.PropertyType));
                }
                return instance;
            }
        }
    }
}
