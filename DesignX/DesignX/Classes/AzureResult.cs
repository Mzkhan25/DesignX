#region
using System.Collections.Generic;
#endregion

namespace DesignX.Classes
{
    public static class AzureResult
    {
        public static List<Project> AzureResults { get; set; }
        public static string UserName { get; set; }
        public static int Count { get; set; }
    }

    public static class InformationClass
    {
        public static Dictionary<int, string> Dict = new Dictionary<int, string>();
    }

}