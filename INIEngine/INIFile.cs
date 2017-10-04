using System.IO;

/// <summary>
/// INI engine namespace
/// </summary>
namespace INIEngine
{
    /// <summary>
    /// INI file class
    /// </summary>
    public static class INIFile
    {
        /// <summary>
        /// Open file
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>INI</returns>
        public static INI Open(string path)
        {
            INI ret = null;
            try
            {
                if (File.Exists(path))
                {
                    using (FileStream stream = File.Open(path, FileMode.Open))
                    {
                        ret = Open(stream);
                    }
                }
            }
            catch
            {
                //
            }
            return ret;
        }

        /// <summary>
        /// Open stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>INI</returns>
        public static INI Open(Stream stream)
        {
            INI ret = null;
            if (stream != null)
            {
                ret = new INI(stream);
            }
            return ret;
        }
    }
}
