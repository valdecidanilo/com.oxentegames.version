namespace HyperVersion.Core
{
    [System.Serializable]
    public class VersionData
    {
        public string release;
        public string build;
        public string date;
        public string environment;
        public bool show_version_web = true;
        public bool show_version_game = false;
    }
}