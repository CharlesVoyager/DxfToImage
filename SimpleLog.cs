using System.IO;

namespace DxfToImage
{
    class SimpleLog
    {
        #region Singleton pattern
        private static SimpleLog instance = null;
        private static readonly object padlock = new object();

        public static SimpleLog Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SimpleLog("DxfToImage.log", "");
                    }
                    return instance;
                }
            }
        }
        #endregion

        StreamWriter writer = null;
        public SimpleLog(string pathLog, string header)
        {

            if (File.Exists(pathLog))
                File.Delete(pathLog);

            writer = new StreamWriter(pathLog, true); ;
            writer.AutoFlush = true;
            writer.WriteLine(header);
        }

        public void WriteLine(string line)
        {
            if (writer == null) return;
            writer.WriteLine(line);
        }

        public void Close()
        {
            if (writer == null) return;
            writer.Flush();
            writer.Close();
        }
    }
}
