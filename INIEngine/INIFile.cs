using System.IO;

namespace INIEngine
{
    public static class INIFile
    {

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

        public static INI Open(Stream stream)
        {
            INI ret = null;
            if (stream != null)
                ret = new INI(stream);
            return ret;
        }
    }
}
