using System;
using System.Collections.Generic;

namespace NicoFramework.Editor.EditorToolEx
{
    public static class ExTool
    {
        /// <summary>
        /// 获取 type 的所有派生类型
        /// </summary>
        /// <param name="type">基类型</param>
        /// <returns>type 的所有派生类型</returns>
        public static List<Type> GetDerivedClasses(this Type type)
        {
            List<Type> derivedClasses = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var t in assembly.GetTypes()) {
                    if (t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t)) {
                        derivedClasses.Add(t);
                    }
                }
            }

            return derivedClasses;
        }
    }
}