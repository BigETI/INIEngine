using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INIEngine
{
    public class INI
    {

        private Dictionary<string, string> entries = new Dictionary<string, string>();

        private Dictionary<string, Dictionary<string, string>> sections = new Dictionary<string, Dictionary<string, string>>();

        public INI()
        {
            //
        }

        public INI(INI ini)
        {
            Dictionary<string, string> s;
            if (ini != null)
            {
                foreach (KeyValuePair<string, string> entry in ini.entries)
                    entries.Add(entry.Key, entry.Value);
                foreach (KeyValuePair<string, Dictionary<string, string>> section in ini.sections)
                {
                    s = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> entry in section.Value)
                        s.Add(entry.Key, entry.Value);
                    sections.Add(section.Key, s);
                }
            }
        }

        public INI(Stream stream)
        {
            string section = null;
            string line;
            string[] kv;
            Dictionary<string, string> s;
            if (stream != null)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        line = reader.ReadLine();
                        if (line.Length > 0)
                        {
                            if (line.StartsWith("[") && line.EndsWith("]"))
                                section = line.Substring(1, line.Length - 2);
                            else
                            {
                                kv = line.Split(new char[] { '=' }, 1, StringSplitOptions.None);
                                if (kv != null)
                                {
                                    if (kv.Length == 2)
                                    {
                                        if (section == null)
                                        {
                                            if (entries.ContainsKey(kv[0]))
                                                entries[kv[0]] = kv[1];
                                            else
                                                entries.Add(kv[0], kv[1]);
                                        }
                                        else
                                        {
                                            if (sections.ContainsKey(section))
                                                s = sections[section];
                                            else
                                            {
                                                s = new Dictionary<string, string>();
                                                sections.Add(section, s);
                                            }
                                            if (s.ContainsKey(kv[0]))
                                                s[kv[0]] = kv[1];
                                            else
                                                s.Add(kv[0], kv[1]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    entries.Clear();
                    foreach (Dictionary<string, string> t in sections.Values)
                        t.Clear();
                    sections.Clear();
                }
            }
        }

        public string GetString(string key, string defaultValue = "", string section = null)
        {
            string ret = defaultValue;
            if (section == null)
            {
                if (entries.ContainsKey(key))
                    ret = entries[key];
            }
            else
            {
                if (sections.ContainsKey(section))
                {
                    Dictionary<string, string> s = sections[section];
                    if (s.ContainsKey(key))
                        ret = s[key];
                }
            }
            return ret;
        }

        public void SetString(string key, string value, string section = null)
        {
            if (section == null)
            {
                if (entries.ContainsKey(key))
                    entries[key] = value;
                else
                    entries.Add(key, value);
            }
            else
            {
                Dictionary<string, string> s = null;
                if (sections.ContainsKey(section))
                    s = sections[section];
                else
                {
                    s = new Dictionary<string, string>();
                    sections.Add(section, s);
                }
                if (s.ContainsKey(key))
                    s[key] = value;
                else
                    s.Add(key, value);
            }
        }

        public bool GetBool(string key, bool defaultValue = false, string section = null)
        {
            bool ret = defaultValue;
            if (!(bool.TryParse(GetString(key, defaultValue.ToString(), section), out ret)))
                ret = (GetInt32(key, defaultValue ? 1 : 0, section) != 0);
            return ret;
        }

        public sbyte GetSByte(string key, sbyte defaultValue = 0, string section = null)
        {
            sbyte ret = defaultValue;
            sbyte.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        public byte GetByte(string key, byte defaultValue = 0, string section = null)
        {
            byte ret = defaultValue;
            byte.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        public short GetInt16(string key, short defaultValue = 0, string section = null)
        {
            short ret = defaultValue;
            short.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        public uint GetUInt16(string key, ushort defaultValue = 0, string section = null)
        {
            ushort ret = defaultValue;
            ushort.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        public int GetInt32(string key, int defaultValue = 0, string section = null)
        {
            int ret = defaultValue;
            int.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        public uint GetUInt32(string key, uint defaultValue = 0U, string section = null)
        {
            uint ret = defaultValue;
            uint.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        public long GetInt64(string key, long defaultValue = 0L, string section = null)
        {
            long ret = defaultValue;
            long.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        public ulong GetUInt64(string key, ulong defaultValue = 0UL, string section = null)
        {
            ulong ret = defaultValue;
            ulong.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        public float GetFloat(string key, float defaultValue = 0.0f, string section = null)
        {
            float ret = defaultValue;
            float.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        public double GetUInt64(string key, double defaultValue = 0.0, string section = null)
        {
            double ret = defaultValue;
            double.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        public void SetValue<T>(string key, T value, string section = null)
        {
            SetString(key, value.ToString(), section);
        }

        public Stream ToStream()
        {
            MemoryStream ret = new MemoryStream();
            try
            {
                StreamWriter writer = new StreamWriter(ret);
                foreach (KeyValuePair<string, string> entry in entries)
                    writer.WriteLine(entry.Key + "=" + entry.Value);
                foreach (KeyValuePair<string, Dictionary<string, string>> section in sections)
                {
                    writer.WriteLine("[" + section.Key + "]");
                    foreach (KeyValuePair<string, string> entry in section.Value)
                        writer.WriteLine(entry.Key + "=" + entry.Value);
                }
                writer.Flush();
            }
            catch
            {
                //
            }
            ret.Position = 0L;
            return ret;
        }

        public override string ToString()
        {
            string ret = "";
            try
            {
                using (StreamReader reader = new StreamReader(ToStream()))
                {
                    ret = reader.ReadToEnd();
                }
            }
            catch
            {
                //
            }
            return ret;
        }
    }
}
