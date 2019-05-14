using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using ApplicationLib.Models;
using FileLib.Interfaces;

namespace FileLib.FileParsers
{
    public class CSFileParser : IFileParser
    {
        private const int TypePropertiesColCount = 5;

        #region Fields
        private FieldInfo[] fields;
        private PropertyInfo[] properties;
        private MethodInfo[] methods;

        private string lowerCaseEnglishLetters = "qwertyuiopasdfghjklzxcvbnm";
        private int currIndex = 0;
        private List<string> copiedFiles = new List<string>();
        #endregion

        public Table[] GetAssemblyTables(string filePath)
        {
            Assembly assembly = Assembly.LoadFrom(filePath);

            Type[] assemblyTypes = assembly.GetExportedTypes();
            Table[] assemblyTables = new Table[assemblyTypes.Length + 1];

            //classes table
            assemblyTables[0] = GetClassesTable(assemblyTypes, filePath);

            for (int i = 0; i < assemblyTypes.Length; i++)
            {
                assemblyTables[i + 1] = GetTypePropertiesTable(assemblyTypes[i]);
            }

            ClearTables(assemblyTables);

            return assemblyTables;
        }

        private void ClearTables(Table[] assemblyTables)
        {
            foreach (Table table in assemblyTables)
            {
                List<string[]> tableCellsList = table.TableCells.ToList();
                tableCellsList.RemoveAll(arr => arr == null);

                table.TableCells = tableCellsList.ToArray();
            }
        }

        private Table GetTypePropertiesTable(Type type)
        {
            fields = type.GetTypeInfo().DeclaredFields.ToArray();
            properties = type.GetTypeInfo().DeclaredProperties.ToArray();
            methods = type.GetTypeInfo().DeclaredMethods.ToArray();
            currIndex = 0;

            int tableRowCount = fields.Length + methods.Length + properties.Length
                - CountNumberOfGetAndSet(properties, methods) + ((fields.Length > 0) ? 2 : 0) +
                ((properties.Length > 0) ? 2 : 0) + ((methods.Length > 0) ? 2 : 0);

            string[][] tableCells = new string[tableRowCount][];

            FillTheFieldsCells(tableCells);
            FillThePropertiesCells(tableCells);
            FillTheMethodsCells(tableCells);

            return new Table(tableCells)
            {
                Title = type.Name
            };
        }

        private void FillTheFieldsCells(string[][] tableCells)
        {
            if (fields.Length > 0)
            {
                tableCells[0] = new string[TypePropertiesColCount]
                    { "Поля", string.Empty, string.Empty, string.Empty, string.Empty };
                tableCells[1] = new string[TypePropertiesColCount]
                    { "Имя", "Модификатор доступа", "Тип", "Назначение", string.Empty };

                currIndex = 2;

                for (int i = 0; i < fields.Length; i++)
                {
                    tableCells[i + 2] = new string[TypePropertiesColCount]
                        {fields[i].Name, GetFieldModificators(fields[i]), fields[i].FieldType.Name, string.Empty, string.Empty};
                    currIndex++;
                }
            }
        }

        private void FillThePropertiesCells(string[][] tableCells)
        {
            if (properties.Length > 0)
            {
                tableCells[currIndex++] = new string[TypePropertiesColCount]
                    { "Свойства", string.Empty, string.Empty, string.Empty, string.Empty };
                tableCells[currIndex++] = new string[TypePropertiesColCount]
                    { "Имя", "Модификатор досутпа", "Тип", "Назначение", string.Empty };

                for (int i = 0; i < properties.Length; i++)
                {
                    tableCells[currIndex] = new string[TypePropertiesColCount]
                        {properties[i].Name, GetPropertiesModificators(properties[i], methods),
                    properties[i].PropertyType.Name, string.Empty, string.Empty};
                    currIndex++;
                }
            }
        }

        private void FillTheMethodsCells(string[][] tableCells)
        {
            if (methods.Length > 0)
            {
                tableCells[currIndex++] = new string[TypePropertiesColCount]
                    { "Методы", string.Empty, string.Empty, string.Empty, string.Empty };
                tableCells[currIndex++] = new string[TypePropertiesColCount]
                    { "Имя", "Модификатор досутпа", "Тип", "Аргументы", "Назначение" };

                for (int i = 0; i < methods.Length; i++)
                {
                    if (!CheckIfMethodIsProperty(methods[i], properties) &&
                        lowerCaseEnglishLetters.IndexOf(methods[i].Name[0]) == -1)
                    {
                        tableCells[currIndex] = new string[TypePropertiesColCount]
                            { methods[i].Name, GetMethodModificators(methods[i]), methods[i].ReturnType.Name,
                            GetMethodArguments(methods[i]), "" };
                        currIndex++;
                    }
                }
            }
        }

        private string GetMethodArguments(MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();

            string paramsString = string.Empty;
            foreach (ParameterInfo p in parameters)
            {
                paramsString += p.ParameterType.Name + " " + p.Name + ", ";
            }

            if (parameters.Length > 0)
                paramsString = paramsString.Remove(paramsString.Length - 2, 2);

            return paramsString;
        }

        private int CountNumberOfGetAndSet(PropertyInfo[] properties, MethodInfo[] methods)
        {
            int res = 0;
            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].Name.IndexOf("set_") > -1)
                    for (int j = 0; j < properties.Length; j++)
                    {
                        if ("set_" + properties[j].Name == methods[i].Name)
                        {
                            res++;
                            break;
                        }
                    }

                if (methods[i].Name.IndexOf("get_") > -1)
                    for (int j = 0; j < properties.Length; j++)
                    {
                        if ("get_" + properties[j].Name == methods[i].Name)
                        {
                            res++;
                            break;
                        }
                    }
            }

            return res;
        }

        private bool CheckIfMethodIsProperty(MethodInfo method, PropertyInfo[] properties)
        {
            foreach (PropertyInfo property in properties)
                if (method.Name == "get_" + property.Name || method.Name == "set_" + property.Name)
                    return true;

            return false;
        }

        private string GetFieldModificators(FieldInfo field)
        {
            string mods = string.Empty;

            if (field.IsStatic)
                mods += "static ";
            if (field.IsPrivate)
                mods += "private ";
            if (field.IsPublic)
                mods += "public ";

            return mods;
        }

        private string GetPropertiesModificators(PropertyInfo property, MethodInfo[] methods)
        {
            string mods = string.Empty;

            string getMods = string.Empty;
            string setMods = string.Empty;

            for (int i = 0; i < methods.Length; i++)
            {
                if ("get_" + property.Name == methods[i].Name)
                {
                    getMods = GetMethodModificators(methods[i]);
                }

                if ("set_" + property.Name == methods[i].Name)
                {
                    setMods = GetMethodModificators(methods[i]);
                }
            }

            mods = "GET: (" + getMods + ") SET: (" + setMods + ")";
            return mods;
        }

        private string GetMethodModificators(MethodInfo method)
        {
            string mods = string.Empty;

            if (method.IsAbstract)
                mods += "abstract ";
            if (method.IsPrivate)
                mods += "private ";
            if (method.IsPublic)
                mods += "public ";
            if (method.IsStatic)
                mods += "static ";

            return mods;
        }

        private Table GetClassesTable(Type[] types, string filePath)
        {
            string[][] classesTableCells = GetClassesTableCells(types);

            return new Table(classesTableCells)
            {
                Title = $"Таблица классов {filePath.Substring(filePath.LastIndexOf("\\") + 1)}"
            };
        }

        private Type[] GetAssemblyTypes(string filePath)
        {
            Assembly assembly = Assembly.LoadFrom(filePath);
            Type[] types = assembly.GetExportedTypes();

            return types;
        }

        private string[][] GetClassesTableCells(Type[] types)
        {
            string[][] classesTableCells = new string[types.Length + 1][];
            classesTableCells[0] = new string[2] { "Класс", "Назначение" };

            for (int i = 1; i < classesTableCells.Length; i++)
            {
                classesTableCells[i] = new string[2];

                classesTableCells[i][0] = types[i - 1].Name;
                classesTableCells[i][1] = string.Empty;
            }

            return classesTableCells;
        }
    }
}
