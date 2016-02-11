using System.IO;
using System.Reflection;
namespace ImapLibrary.Data
{
    /// <summary>
    /// Locates the directory where the application is executing
    /// </summary>
    public static class AssemblyLocator
    {
        /// <summary>
        /// Returns the folder where the application is executing including the following backslash
        /// </summary>
        /// <returns>string - the name of the directory where the application is executing</returns>
        public static string ExecutingDirectory()
        {
            var assem = Assembly.GetExecutingAssembly();
            var info = new FileInfo(assem.Location);
            string directory = info.Directory.FullName;
            if (!directory.EndsWith(@"\"))
            {
                directory += @"\";
            }
            return directory;
        }
    }
}
