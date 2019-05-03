using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AjuPrince.ORM.Frameworker
{
    public static class DBAttributeExtend
    {
        /// <summary>
        /// 获取映射对象名称
        /// </summary>
        /// <typeparam name="T">类型参数</typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetMappingName<T>(this T t) where T : MemberInfo
        {
            if (t.IsDefined(typeof(AjuPrinceBaseMappingAttribute), true))
            {
                AjuPrinceBaseMappingAttribute attribute = (AjuPrinceBaseMappingAttribute)t.GetCustomAttribute(typeof(AjuPrinceBaseMappingAttribute), true);
                return attribute.GetName();
            }
            return t.Name;
        }

        /// <summary>
        /// 过滤主键
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertiesWithoutKey(this Type type)
        {
            return type.GetProperties().Where(p => !p.IsDefined(typeof(AjuPrincePKAttribute), true));
        }
        public static string GetPropertiesKey(this Type type)
        {
            return type.GetProperties().Where(p => p.IsDefined(typeof(AjuPrincePKAttribute), true)).FirstOrDefault()?.Name;
        }
    }
}
