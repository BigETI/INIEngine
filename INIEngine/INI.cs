using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// INI engine namespace
/// </summary>
namespace INIEngine
{
    /// <summary>
    /// INI class
    /// </summary>
    public class INI
    {
        /// <summary>
        /// Entries
        /// </summary>
        private readonly Dictionary<string, string> entries = new Dictionary<string, string>();

        /// <summary>
        /// Sections
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, string>> sections = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public INI()
        {
            //
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ini">INI</param>
        public INI(INI ini)
        {
            Dictionary<string, string> s;
            if (ini != null)
            {
                foreach (KeyValuePair<string, string> entry in ini.entries)
                {
                    entries.Add(entry.Key, entry.Value);
                }
                foreach (KeyValuePair<string, Dictionary<string, string>> section in ini.sections)
                {
                    s = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> entry in section.Value)
                    {
                        s.Add(entry.Key, entry.Value);
                    }
                    sections.Add(section.Key, s);
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream</param>
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
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Length > 0)
                            {
                                if (line[0] != ';')
                                {
                                    if (line.StartsWith("[") && line.EndsWith("]"))
                                    {
                                        section = line.Substring(1, line.Length - 2);
                                    }
                                    else
                                    {
                                        kv = line.Split(new [] { '=' }, 1, StringSplitOptions.None);
                                        if (kv != null)
                                        {
                                            if (kv.Length == 2)
                                            {
                                                if (section == null)
                                                {
                                                    if (entries.ContainsKey(kv[0]))
                                                    {
                                                        entries[kv[0]] = kv[1];
                                                    }
                                                    else
                                                    {
                                                        entries.Add(kv[0], kv[1]);
                                                    }
                                                }
                                                else
                                                {
                                                    if (sections.ContainsKey(section))
                                                    {
                                                        s = sections[section];
                                                    }
                                                    else
                                                    {
                                                        s = new Dictionary<string, string>();
                                                        sections.Add(section, s);
                                                    }
                                                    if (s.ContainsKey(kv[0]))
                                                    {
                                                        s[kv[0]] = kv[1];
                                                    }
                                                    else
                                                    {
                                                        s.Add(kv[0], kv[1]);
                                                    }
                                                }
                                            }
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
                    {
                        t.Clear();
                    }
                    sections.Clear();
                }
            }
        }

        /// <summary>
        /// Get string value
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>String value</returns>
        public string GetString(string key)
        {
            return GetString(key, "", null);
        }

        /// <summary>
        /// Get string value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns></returns>
        public string GetString(string key, string defaultValue)
        {
            return GetString(key, defaultValue, null);
        }

        /// <summary>
        /// Get string value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="section">Section</param>
        /// <returns>String value</returns>
        public string GetString(string key, string defaultValue, string section)
        {
            string ret = defaultValue;
            if (section == null)
            {
                if (entries.ContainsKey(key))
                {
                    ret = entries[key];
                }
            }
            else
            {
                if (sections.ContainsKey(section))
                {
                    Dictionary<string, string> s = sections[section];
                    if (s.ContainsKey(key))
                    {
                        ret = s[key];
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Set string value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void SetString(string key, string value)
        {
            SetString(key, value, null);
        }

        /// <summary>
        /// Set string value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="section">Section</param>
        public void SetString(string key, string value, string section)
        {
            if (section == null)
            {
                if (entries.ContainsKey(key))
                {
                    entries[key] = value;
                }
                else
                {
                    entries.Add(key, value);
                }
            }
            else
            {
                Dictionary<string, string> s = null;
                if (sections.ContainsKey(section))
                {
                    s = sections[section];
                }
                else
                {
                    s = new Dictionary<string, string>();
                    sections.Add(section, s);
                }
                if (s.ContainsKey(key))
                {
                    s[key] = value;
                }
                else
                {
                    s.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Get boolean value
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Boolean value</returns>
        public bool GetBool(string key)
        {
            return GetBool(key, false, null);
        }

        /// <summary>
        /// Get boolean value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns></returns>
        public bool GetBool(string key, bool defaultValue)
        {
            return GetBool(key, defaultValue, null);
        }

        /// <summary>
        /// Get boolean value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="section">Section</param>
        /// <returns>Boolean value</returns>
        public bool GetBool(string key, bool defaultValue, string section)
        {
            bool ret = defaultValue;
            if (!(bool.TryParse(GetString(key, defaultValue.ToString(), section), out ret)))
            {
                ret = (GetInt32(key, defaultValue ? 1 : 0, section) != 0);
            }
            return ret;
        }

        /// <summary>
        /// Get signed byte
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Get signed byte</returns>
        public sbyte GetSByte(string key)
        {
            return GetSByte(key, 0, null);
        }

        /// <summary>
        /// Get signed byte
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Signed byte</returns>
        public sbyte GetSByte(string key, sbyte defaultValue)
        {
            return GetSByte(key, defaultValue, null);
        }

        /// <summary>
        /// Get signed byte
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="section">Section</param>
        /// <returns>Signed byte value</returns>
        public sbyte GetSByte(string key, sbyte defaultValue, string section)
        {
            sbyte ret = defaultValue;
            sbyte.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        /// <summary>
        /// Get byte value
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Byte value</returns>
        public byte GetByte(string key)
        {
            return GetByte(key, 0, null);
        }

        /// <summary>
        /// Get byte value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Byte value</returns>
        public byte GetByte(string key, byte defaultValue)
        {
            return GetByte(key, defaultValue, null);
        }

        /// <summary>
        /// Get byte value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="section">Section</param>
        /// <returns>Byte value</returns>
        public byte GetByte(string key, byte defaultValue, string section)
        {
            byte ret = defaultValue;
            byte.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        /// <summary>
        /// Get short value
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Short value</returns>
        public short GetInt16(string key)
        {
            return GetInt16(key, 0, null);
        }

        /// <summary>
        /// Get short value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Short value</returns>
        public short GetInt16(string key, short defaultValue)
        {
            return GetInt16(key, defaultValue, null);
        }

        /// <summary>
        /// Get short value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="section">Section</param>
        /// <returns>Short value</returns>
        public short GetInt16(string key, short defaultValue, string section)
        {
            short ret = defaultValue;
            short.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        /// <summary>
        /// Get unsigned short value
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Unsigned short value</returns>
        public uint GetUInt16(string key)
        {
            return GetUInt16(key, 0, null);
        }

        /// <summary>
        /// Get unsigned short value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Unsigned short value</returns>
        public uint GetUInt16(string key, ushort defaultValue)
        {
            return GetUInt16(key, defaultValue, null);
        }

        /// <summary>
        /// Get unsigned short value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="section">Section</param>
        /// <returns>Unsigned short value</returns>
        public uint GetUInt16(string key, ushort defaultValue, string section)
        {
            ushort ret = defaultValue;
            ushort.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        /// <summary>
        /// Get integer value
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Integer value</returns>
        public int GetInt32(string key)
        {
            return GetInt32(key, 0, null);
        }

        /// <summary>
        /// Get integer value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Integer value</returns>
        public int GetInt32(string key, int defaultValue)
        {
            return GetInt32(key, defaultValue, null);
        }

        /// <summary>
        /// Get integer value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="section">Section</param>
        /// <returns>Integer value</returns>
        public int GetInt32(string key, int defaultValue, string section)
        {
            int ret = defaultValue;
            int.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        /// <summary>
        /// Get unsigned integer value
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Unsigned integer value</returns>
        public uint GetUInt32(string key)
        {
            return GetUInt32(key, 0U, null);
        }

        /// <summary>
        /// Get unsigned integer value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Unsigned integer value</returns>
        public uint GetUInt32(string key, uint defaultValue)
        {
            return GetUInt32(key, defaultValue, null);
        }

        /// <summary>
        /// Get unsigned integer value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="section">Section</param>
        /// <returns>Unsigned integer value</returns>
        public uint GetUInt32(string key, uint defaultValue, string section)
        {
            uint ret = defaultValue;
            uint.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        /// <summary>
        /// Get long value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="section">Section</param>
        /// <returns>Long value</returns>
        public long GetInt64(string key, long defaultValue = 0L, string section = null)
        {
            long ret = defaultValue;
            long.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        /// <summary>
        /// Get unsigned long value
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Unsigned long value</returns>
        public ulong GetUInt64(string key)
        {
            return GetUInt64(key, 0UL, null);
        }

        /// <summary>
        /// Get unsigned long value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Unsigned long value</returns>
        public ulong GetUInt64(string key, ulong defaultValue)
        {
            return GetUInt64(key, defaultValue, null);
        }

        /// <summary>
        /// Get unsigned long value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="section">Section</param>
        /// <returns>Unsigned long value</returns>
        public ulong GetUInt64(string key, ulong defaultValue, string section)
        {
            ulong ret = defaultValue;
            ulong.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        /// <summary>
        /// Get single precision floating point value
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Single precision floating point value</returns>
        public float GetFloat(string key)
        {
            return GetFloat(key, 0.0f, null);
        }

        /// <summary>
        /// Get single precision floating point value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Single precision floating point value</returns>
        public float GetFloat(string key, float defaultValue)
        {
            return GetFloat(key, defaultValue, null);
        }

        /// <summary>
        /// Get single precision floating point value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="section">Section</param>
        /// <returns>Single precision floating point value</returns>
        public float GetFloat(string key, float defaultValue, string section)
        {
            float ret = defaultValue;
            float.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        /// <summary>
        /// Get double precision floating point value
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Double precision floating point value</returns>
        public double GetDouble(string key)
        {
            return GetDouble(key, 0.0, null);
        }

        /// <summary>
        /// Get double precision floating point value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Double precision floating point value</returns>
        public double GetDouble(string key, double defaultValue)
        {
            return GetDouble(key, defaultValue, null);
        }

        /// <summary>
        /// Get double precision floating point value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="section">Section</param>
        /// <returns>Double precision floating point value</returns>
        public double GetDouble(string key, double defaultValue, string section)
        {
            double ret = defaultValue;
            double.TryParse(GetString(key, defaultValue.ToString(), section), out ret);
            return ret;
        }

        /// <summary>
        /// Set value
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void SetValue<T>(string key, T value)
        {
            SetValue<T>(key, value, null);
        }

        /// <summary>
        /// Set value
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="section">Section</param>
        public void SetValue<T>(string key, T value, string section)
        {
            SetString(key, value.ToString(), section);
        }

        /// <summary>
        /// To stream
        /// </summary>
        /// <returns>Stream</returns>
        public Stream ToStream()
        {
            MemoryStream ret = new MemoryStream();
            try
            {
                StreamWriter writer = new StreamWriter(ret);
                foreach (KeyValuePair<string, string> entry in entries)
                {
                    writer.WriteLine(entry.Key + "=" + entry.Value);
                }
                foreach (KeyValuePair<string, Dictionary<string, string>> section in sections)
                {
                    writer.WriteLine("[" + section.Key + "]");
                    foreach (KeyValuePair<string, string> entry in section.Value)
                    {
                        writer.WriteLine(entry.Key + "=" + entry.Value);
                    }
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

        /// <summary>
        /// To string
        /// </summary>
        /// <returns>String representation</returns>
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
