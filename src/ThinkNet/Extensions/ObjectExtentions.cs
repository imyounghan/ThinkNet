﻿
namespace ThinkNet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;


    /// <summary>
    /// 基础类型的扩展类
    /// </summary>
    public static class ObjectExtentions
    {
        /// <summary>
        /// 参数名称为 <paramref name="variableName"/> 的值不能是 null。
        /// </summary>
        public static void NotNull(this object obj, string variableName)
        {
            if(obj.IsNull())
                throw new ArgumentNullException(variableName);
        }

        /// <summary>
        /// 参数名称为 <paramref name="variableName"/> 的字符串不能 <see cref="string.Empty"/> 字符串。
        /// </summary>
        public static void NotEmpty(this string @string, string variableName)
        {
            if(@string != null && string.IsNullOrEmpty(@string))
                throw new ArgumentException(variableName);
        }
        /// <summary>
        /// 参数名称为 <paramref name="variableName"/> 的字符串不能是 null 或 <see cref="string.Empty"/> 字符串。
        /// </summary>
        public static void NotNullOrEmpty(this string @string, string variableName)
        {
            if(string.IsNullOrEmpty(@string))
                throw new ArgumentNullException(variableName);
        }
        /// <summary>
        /// 参数名称为 <paramref name="variableName"/> 的字符串不能是空白字符串。
        /// </summary>
        public static void NotWhiteSpace(this string @string, string variableName)
        {
            if(@string != null && string.IsNullOrWhiteSpace(@string))
                throw new ArgumentException(variableName);
        }
        /// <summary>
        /// 参数名称为 <paramref name="variableName"/> 的字符串不能是 null 或 空白字符串。
        /// </summary>
        public static void NotNullOrWhiteSpace(this string @string, string variableName)
        {
            if(string.IsNullOrWhiteSpace(@string))
                throw new ArgumentNullException(variableName);
        }

        /// <summary>
        /// 参数名称为 <paramref name="variableName"/> 的数值必须是正整数。
        /// </summary>
        public static void MustPositive(this int number, string variableName)
        {
            if(number <= 0)
                throw new ArgumentOutOfRangeException(variableName, string.Concat(variableName, " should be positive."));
        }
        /// <summary>
        /// 参数名称为 <paramref name="variableName"/> 的数值必须是正整数。
        /// </summary>
        public static void MustPositive(this long number, string variableName)
        {
            if(number <= 0L)
                throw new ArgumentOutOfRangeException(variableName, string.Concat(variableName, " should be positive."));
        }
        /// <summary>
        /// 参数名称为 <paramref name="variableName"/> 的数值必须是正整数。
        /// </summary>
        public static void MustPositive(this decimal number, string variableName)
        {
            if(number <= 0m)
                throw new ArgumentOutOfRangeException(variableName, string.Concat(variableName, " should be positive."));
        }
        /// <summary>
        /// 参数名称为 <paramref name="variableName"/> 的数值不能是负数。
        /// </summary>
        public static void NonNegative(this long number, string variableName)
        {
            if(number < 0L)
                throw new ArgumentOutOfRangeException(variableName, string.Concat(variableName, " should be non negative."));
        }
        /// <summary>
        /// 参数名称为 <paramref name="variableName"/> 的数值不能是负数。
        /// </summary>
        public static void NonNegative(this int number, string variableName)
        {
            if(number < 0)
                throw new ArgumentOutOfRangeException(variableName, string.Concat(variableName, " should be non negative."));
        }
        /// <summary>
        /// 参数名称为 <paramref name="variableName"/> 的数值不能是负数。
        /// </summary>
        public static void NonNegative(this decimal number, string variableName)
        {
            if(number < 0m)
                throw new ArgumentOutOfRangeException(variableName, string.Concat(variableName, " should be non negative."));
        }


        /// <summary>
        /// 验证模型的正确性
        /// </summary>
        public static bool IsValid<TModel>(this TModel model, out IEnumerable<ValidationResult> errors)
            where TModel : class
        {
            errors = from property in TypeDescriptor.GetProperties(model).Cast<PropertyDescriptor>()
                     from attribute in property.Attributes.OfType<ValidationAttribute>()
                     where !attribute.IsValid(property.GetValue(model))
                     select new ValidationResult(attribute.FormatErrorMessage(property.DisplayName ?? property.Name));

            return !errors.Any();
        }

        /// <summary>
        /// 检查当前对象是否为 null
        /// </summary>
        public static bool IsNull(this object obj)
        {
            return obj == null || obj == DBNull.Value;
        }

        /// <summary>
        /// 判定字符串是不是数值型
        /// </summary>
        public static bool IsNumeric(this string digit)
        {
            return Regex.IsMatch(digit, @"^[-]?[0-9]*$");
        }

        /// <summary>
        /// 判断字符串是不是日期格式
        /// </summary>
        public static bool IsDate(this string date)
        {
            return Regex.IsMatch(date, @"^(\d{4})(-|/|\.)(\d{1,2})(-|/|\.)(\d{1,2})$");
        }

        /// <summary>
        /// 判断字符串是不是时间格式
        /// </summary>
        public static bool IsTime(this string time)
        {
            return Regex.IsMatch(time, @"^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$");
        }

        /// <summary>
        /// 判断字符串是不是日期时间模式
        /// </summary>
        public static bool IsDateTime(this string dateTime)
        {
            return Regex.IsMatch(dateTime, @"(\d{4})(-|/|\.)(\d{1,2})(-|/|\.)(\d{1,2}) ^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$");
        }

        /// <summary>
        /// 判断字符串是不是小数模式
        /// </summary>
        public static bool IsDecimal(this string @decimal)
        {
            return Regex.IsMatch(@decimal, @"^[-]?[0-9]*[.]?[0-9]*$");
        }


        /// <summary>
        /// 如果当前的字符串为空，则返回安全值
        /// </summary>
        public static string IfEmpty(this string str, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(str) ? defaultValue : str;
        }

        /// <summary>
        /// 如果当前的字符串为空，则返回安全值
        /// </summary>
        public static string IfEmpty(this string str, Func<string> valueFactory)
        {
            return string.IsNullOrWhiteSpace(str) ? valueFactory() : str;
        }


        /// <summary>
        /// 将 <param name="str" /> 转换为 <param name="targetType" /> 的值。转换失败会抛异常
        /// </summary>
        public static object Change(this string str, Type targetType)
        {
            str.NotNullOrEmpty("str");

            if(targetType.IsValueType) {
                if(typeof(bool) == targetType) {
                    var lb = str.ToUpper();
                    if(lb == "T" || lb == "F" || lb == "TRUE" || lb == "FALSE" ||
                        lb == "Y" || lb == "N" || lb == "YES" || lb == "NO") {
                        return (lb == "T" || lb == "TRUE" || lb == "Y" || lb == "YES");
                    }
                }

                var method = targetType.GetMethod("Parse", new Type[] { typeof(string) });
                if(method != null) {
                    return method.Invoke(null, new object[] { str });
                }
            }

            if(targetType.IsEnum) {
                return Enum.Parse(targetType, str);
            }


            throw new ArgumentException(string.Format("Unhandled type of '{0}'.", targetType));
        }

        /// <summary>
        /// 将 <param name="str" /> 转换为 <param name="targetType" /> 的值。如果转换失败则使用 <param name="defaultValue" /> 的值。
        /// </summary>
        public static object ChangeIfError(this string str, Type targetType, object defaultValue)
        {
            if(string.IsNullOrEmpty(str))
                return defaultValue;

            try {
                return str.Change(targetType);
            }
            catch {
                return defaultValue;
            }
        }

        /// <summary>
        /// 将 <param name="str" /> 转换为 <param name="targetType" /> 的值。一个指示转换是否成功的返回值 <param name="result" />。
        /// </summary>
        public static bool TryChange(this string str, Type targetType, out object result)
        {
            try {
                result = str.Change(targetType);
                return true;
            }
            catch {
                result = targetType.GetDefaultValue();
                return false;
            }
        }


        /// <summary>
        /// 将 <param name="str" /> 转换为 <typeparam name="T" /> 的值。
        /// </summary>
        public static T Change<T>(this string str)
            where T : struct
        {
            return (T)Change(str, typeof(T));
        }

        /// <summary>
        /// 将 <param name="str" /> 转换为 <typeparam name="T" /> 的值。如果转换失败则使用 <param name="defaultValue" /> 的值。
        /// </summary>
        public static T ChangeIfError<T>(this string str, T defaultValue)
            where T : struct
        {
            T result;
            if(str.TryChange<T>(out result)) {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// 将 <param name="str" /> 转换为 <typeparam name="T" /> 的值。一个指示转换是否成功的返回值 <param name="result" />。
        /// </summary>
        public static bool TryChange<T>(this string str, out T result)
            where T : struct
        {
            if(string.IsNullOrEmpty(str)) {
                result = default(T);
                return false;
            }

            try {
                result = str.Change<T>();
                return true;
            }
            catch {
                result = default(T);
                return false;
            }
        }

        public static TRole ActAs<TRole>(this object obj) where TRole : class
        {
            if(!typeof(TRole).IsInterface) {
                throw new ApplicationException(string.Format("'{0}' is not an interface type.", typeof(TRole).FullName));
            }

            var actor = obj as TRole;

            if(actor == null) {
                throw new ApplicationException(string.Format("'{0}' cannot act as role '{1}'.", obj.GetType().FullName, typeof(TRole).FullName));
            }

            return actor;
        }
    }
}
