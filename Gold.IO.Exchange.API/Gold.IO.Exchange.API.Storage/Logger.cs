using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace Logger
{
    public class Logger
    {
        static List<string> buffer = new List<string>();
        static bool IsError;
        private static string path = Directory.GetCurrentDirectory() + "\\AppData\\";
        private Logger() { }

        public static void WriteLogg(string who, string message, string typeLogging)
        {
            string text = string.Empty;


            if (typeLogging.Equals("error"))
            {
                text = DateTime.Now.ToString("HH:mm:ss") + "  ERROR(" + who + ") -> " + message;
                IsError = true;
            }
            else if (typeLogging.Equals("info"))
            {
                text = DateTime.Now.ToString("HH:mm:ss") + "  INFO(" + who + ") -> " + message;
                IsError = false;
            }
            else return;
            lock (buffer)
            {
                buffer.Add(text);
            }
        }

        public static void NhibernateDelete<T>(T item)
        {
            Type type = typeof(T);

            string text = "DELETE FROM " + "`" + type.Name.ToLower() + "`" + " WHERE id = ";

            var fields = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Name.ToLower() == "id")
                {
                    text = text + fields[i].GetValue(item);
                }
            }
            text = text + ";";

            string message = text;
            WriteLogg("NHibernate", message, "info");

        }

        public static void NhibernateInsert<T>(T item, string ID)
        {
            Type type = typeof(T);
            string text = "INSERT INTO " + "`" + type.Name.ToLower() + "`" + " (";

            var fields = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            for (int i = 0; i < fields.Length; i++)
            {
                text = text + fields[i].Name.ToLower();
                if (i < (fields.Length - 1)) text += ", ";
            }
            text = text + ") VALUES(";

            for (int i = 0; i < fields.Length; i++)
            {
                if (checkClass(fields[i].GetValue(item), type.Namespace) == TypeValue.CustomClass)
                {
                    var property = fields[i].GetValue(item).GetType().GetProperty(ID, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    string value = property.GetValue(fields[i].GetValue(item)).ToString();
                    text = text + string.Format("{1}_{0} = {2}", shortName(fields[i].GetValue(item).ToString()).ToLower(), ID.ToLower(), value);
                }
                else if (checkClass(fields[i].GetValue(item), type.Namespace) == TypeValue.EmptyClass) text = text + "'null'";
                else text = text + "'" + fields[i].GetValue(item) + "'";
                if (i < (fields.Length - 1)) text += ", ";
            }
            text = text + ");";
            string message = text;
            WriteLogg("NHibernate", message, "info");

        }

        public static void NhibernateUpdate<T>(T item, string field)
        {
            Type type = typeof(T);

            string text = "UPDATE " + "`" + type.Name.ToLower() + "`" + " SET ",
                ID = string.Empty;

            var fields = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            for (int i = 0; i < fields.Length; i++)
            {
                if (!fields[i].Name.ToLower().Equals("id"))
                {
                    if (checkClass(fields[i].GetValue(item), type.Namespace) == TypeValue.CustomClass)
                    {
                        var property = fields[i].GetValue(item).GetType().GetProperty(field, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        if (property != null)
                        {
                            string value = property.GetValue(fields[i].GetValue(item)).ToString();
                            text = text + string.Format("{1}_{0} = {2}", shortName(fields[i].GetValue(item).ToString()).ToLower(), field.ToLower(), value);
                        }
                    }
                    else if (checkClass(fields[i].GetValue(item), type.Namespace) == TypeValue.EmptyClass) text = text + "'null' = 'null'";
                    else text = text + fields[i].Name.ToLower() + " = '" + fields[i].GetValue(item) + "'";
                    if (i < (fields.Length - 1)) text = text + ", ";
                }
                else ID = fields[i].GetValue(item).ToString();
            }
            text = text + " WHERE id = " + ID + ";";
            string message = text;
            WriteLogg("NHibernate", message, "info");

        }

        public static void NhibernateSelect<T>(string ID)
        {
            Type type = typeof(T);

            string text = "SELECT ";

            var fields = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            for (int i = 0; i < fields.Length; i++)
            {
                if (!fields[i].Name.ToLower().Equals("id"))
                {
                    text = text + fields[i].Name.ToLower();
                    if (i < (fields.Length - 1)) text += ", ";
                }
            }
            if (!string.IsNullOrEmpty(ID)) ID = " WHERE id = " + ID;
            text = text + " FROM " + "`" + type.Name.ToLower() + "`" + ID + ";";
            string message = text;
            WriteLogg("NHibernate", message, "info");

        }

        private static string shortName(string fullName)
        {
            string result = "";
            int i = fullName.Length - 1;
            while (i >= 0)
            {
                if (fullName[i] == '.') break;
                i--;
            }
            result = fullName.Remove(0, i + 1);
            return result;
        }

        private static void write()
        {
            string filename = DateTime.Now.ToString("dd-MM-yyyy") + ".log";

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (!File.Exists(path + filename)) File.Create(path + filename).Close();
            StreamWriter writer = new StreamWriter(path + filename, true, Encoding.Default);
            lock (buffer)
            {
                for (int i = 0; i < buffer.Count; i++)
                {
                    writer.WriteLine(buffer[i]);
                }

                writer.Close();

                buffer.Clear();
            }

            IsError = false;
        }

        public static void StartHelper()
        {
            buffer = new List<string>();
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            new Thread(work).Start();
        }

        private static void work()
        {
            while (true)
            {
                if (IsError || buffer.Count > 4)
                {
                    write();
                }
                if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 58 && DateTime.Now.Minute <= 59)
                {
                    string[] files = Directory.GetFiles(path);
                    if (files.Length >= 32)
                    {
                        foreach (string file in files)
                        {
                            File.Delete(file);
                        }
                    }
                }
            }
        }

        private static TypeValue checkClass(object obj, string nameSpace)
        {
            if (obj == null) return TypeValue.BaseClass;
            PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (properties.Length == 0) return TypeValue.BaseClass;
            PropertyInfo property = properties.FirstOrDefault(x => x.Name.ToLower() == "id");
            if (property != null)
            {
                var value = property.GetValue(obj).GetType().Name;
                if (!property.GetValue(obj).GetType().Name.Contains("Int64")) return TypeValue.BaseClass;
                long id = (long)property.GetValue(obj);
                if (id == 0) return TypeValue.EmptyClass;
                if (obj.GetType().Name.Contains("Proxy")) return TypeValue.EmptyClass;
            }
            if (!obj.ToString().Contains(nameSpace)) return TypeValue.BaseClass;
            return TypeValue.CustomClass;
        }
    }


    enum TypeValue
    {
        BaseClass = 1,
        CustomClass = 2,
        EmptyClass = 3
    }
}