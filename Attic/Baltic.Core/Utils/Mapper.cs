using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Baltic.Core.Utils
{
    public static class Mapper
    {
        public static T Map<T>(object source, object destination)
        {
            foreach (var d in destination.GetType().GetProperties())
            {
                var foundProperty = false;
                foreach (var s in source.GetType().GetProperties())
                {
                    if (s.CustomAttributes.All(t => t.AttributeType != typeof(IgnoreAttribute)))
                        if (s.Name.Equals(d.Name))
                        {
                            if (s.PropertyType == d.PropertyType)
                            {
                                d.SetValue(destination, s.GetValue(source));
                                foundProperty = true;
                            }else
                                throw new MapperException("There is a property of this same name but different type.");
                        }
                }
                if (d.CustomAttributes.Any(t => t.AttributeType == typeof(RequiredAttribute)) && !foundProperty)
                    throw new MapperException("There is no required property in source object");
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
}
