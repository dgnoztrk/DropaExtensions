﻿using System.Data;
using System.Reflection;

namespace Dropa.Extensions
{
    public static class Converter
    {
        public static int ToInt(this object data)
        {
            if (data == null)
            {
                return 0;
            }
            else
            {
                int result;
                int.TryParse(data.ToString(), out result);
                return result;
            }
        }
        public static DateTime ToDateTime(this object data)
        {
            if (data == null)
            {
                return DateTime.MinValue;
            }
            else
            {
                DateTime result;
                DateTime.TryParse(data.ToString(), out result);
                return result;
            }
        }
        public static double ToDouble(this object data)
        {
            if (data == null)
            {
                return 0;
            }
            else
            {
                double result;
                double.TryParse(data.ToString(), out result);
                return result;
            }
        }
        public static bool ToBool(this object data)
        {
            if (data == null)
            {
                return false;
            }
            else
            {
                if (data is int && (int)data == 1)
                {
                    return true;
                }
                return bool.Parse(data.ToString());
            }
        }
        public static Stream ToStream(this string @this)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(@this);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        public static List<T> ConvertTo<T>(this DataTable datatable) where T : new()
        {
            List<T> Temp = new List<T>();
            try
            {
                List<string> columnsNames = new List<string>();
                foreach (DataColumn DataColumn in datatable.Columns)
                    columnsNames.Add(DataColumn.ColumnName);
                Temp = datatable.AsEnumerable().ToList().ConvertAll(row => row.GetObject<T>(columnsNames));
                return Temp;
            }
            catch
            {
                return Temp;
            }

        }
        public static T GetObject<T>(this DataRow row, List<string> columnsName) where T : new()
        {
            var obj = new T();
            try
            {
                var properties = typeof(T).GetProperties();
                foreach (PropertyInfo objProperty in properties)
                {
                    var columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        var value = row[columnname].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
                            {
                                value = row[columnname].ToString().Replace("$", "").Replace(",", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
                            }
                            else
                            {
                                value = row[columnname].ToString().Replace("%", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
                            }
                        }
                    }
                }
                return obj;
            }
            catch
            {
                return obj;
            }
        }
        public static TResult ConvertObject<TRequest, TResult>(this TRequest objRequest) where TResult : new() where TRequest : new()
        {
            return objRequest.ConvertObject<TRequest, TResult>(false);
        }
        public static TResult ConvertObject<TRequest, TResult>(this TRequest objRequest, bool skipTrows) where TResult : new() where TRequest : new()
        {
            return objRequest.Map<TResult>(default, skipTrows);

        }

        public static T Map<T>(this object data, T target) where T : new()
        {
            return data.Map(target, false);
        }

        public static T Map<T>(this object data, T target, bool skipThrows) where T : new()
        {
            if (data == null) return target;

            if (target == null) target = Activator.CreateInstance<T>();

            var properties = data.GetType().GetProperties();

            var targetType = typeof(T);
            foreach (var item in properties)
            {
                var prop = targetType.GetProperty(item.Name, item.PropertyType);

                if (prop != null)
                {
                    try
                    {
                        prop.SetValue(target, item.GetValue(data, null), null);
                    }
                    catch
                    {
                        if (skipThrows == false) throw;
                    }
                }
            }
            return target;
        }
    }
}
