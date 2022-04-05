using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Baltic.Core.Utils
{
    public static class DBMapper
    {
        public static T Map<T>(object source, object destination)
        {
            foreach (var d in destination.GetType().GetProperties())
            {
                foreach (var s in source.GetType().GetProperties())
                {
                    if (s.CustomAttributes.All(t => t.AttributeType != typeof(IgnoreAttribute)))
                        if (s.Name.Equals(d.Name))
                        {
                            if (s.PropertyType == d.PropertyType && null != d.GetSetMethod())
                            {
                                d.SetValue(destination, s.GetValue(source));
                                break;
                            } /* else
                                throw new MapperException("There is a property of this same name but different type."); */
                        }
                }
                /* if (d.CustomAttributes.Any(t => t.AttributeType == typeof(RequiredAttribute)) && !foundProperty)
                    throw new MapperException("There is no required property in source object"); */
            }
            return (T)destination;
        }

        public static dynamic MapToExpando(object source)
        {
            var destination = (IDictionary<string, object>)new ExpandoObject();

            foreach (var s in source.GetType().GetProperties())
            {
                if (s.CustomAttributes.All(t => t.AttributeType != typeof(IgnoreAttribute)))
                {
                    destination.Add(s.Name, s.GetValue(source));
                }
            }

            return destination;
        }

        public static dynamic MapToExpando(object source, ExpandoObject expando)
        {
            var destination = (IDictionary<string, object>)expando;

            foreach (var dp in expando.GetType().GetProperties())
                foreach (var dpa in dp.CustomAttributes)
                    if (dpa.AttributeType == typeof(RequiredAttribute)
                        && source.GetType().GetProperties().All(sp =>
                            !(sp.Name.Equals(dp.Name) || (sp.PropertyType != dp.PropertyType) || sp.CustomAttributes.Any(t =>
                                                      t.AttributeType == typeof(IgnoreAttribute)))))
                    {
                        throw new MapperException("There is no required property in source object");
                    }

            foreach (var s in source.GetType().GetProperties())
            {
                if (s.CustomAttributes.All(t => t.AttributeType != typeof(IgnoreAttribute)))
                {
                    if ( !destination.ContainsKey(s.Name))
                        destination.Add(s.Name, s.GetValue(source));
                    else if(s.PropertyType == destination[s.Name].GetType())
                        destination[s.Name] = s.GetValue(source);
                    else
                        throw new MapperException("There is a property of this same name but different type.");
                }
            }

            return destination;
        }

        public static List<T> MapList<T>(IEnumerable<object> sourceList)
        {
            var destinationList = new List<T>();

            foreach (var o in sourceList)
            {
                var destinationObject = (T)Activator.CreateInstance(typeof(T));
                destinationList.Add(Map<T>(o, destinationObject));
            }

            return destinationList;
        }

        public static T MapWithDelta<T>(object source, object destination, string delta)
        {
            if (source == null)
            {
                return (T)destination;
            }

            if (destination == null)
            {
                return (T)source;
            }

            var propertiesDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(delta);


            foreach (var d in destination.GetType().GetProperties())
            {
                var foundProperty = false;

                foreach (var s in source.GetType().GetProperties())
                {
                    if (s.CustomAttributes.All(t => t.AttributeType != typeof(IgnoreAttribute)))
                    {
                        if (s.Name.Equals(d.Name))
                        {
                            if (s.PropertyType == d.PropertyType)
                            {
                                if (propertiesDictionary.Any(x => x.Key.ToLower().Equals(s.Name.ToLower())))
                                {
                                    d.SetValue(destination, s.GetValue(source));
                                    foundProperty = true;
                                }
                            }
                            else
                            {
                                throw new MapperException($"There is a property of this same name but different type. {s.Name} {s.PropertyType} {s.GetValue(source)} {d.Name} {d.PropertyType} {d.GetValue(destination)}");
                            }
                        }
                    }
                }

                if (d.CustomAttributes.Any(t => t.AttributeType == typeof(RequiredAttribute)) && !foundProperty)
                {
                    throw new MapperException("There is no required property in source object");
                }
            }

            return (T)destination;
        }
    }
}


class RequiredAttribute : Attribute
    { }

    class IgnoreAttribute : Attribute
    { }

    class MapperException : Exception
    {
        public MapperException() : base() { }
        public MapperException(string message) : base(message) { }
    }
