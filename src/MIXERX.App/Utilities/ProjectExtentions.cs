using MIXERX.App.Models;
using System.Diagnostics.CodeAnalysis;

namespace MIXERX.App.Utilities
{
    /// <summary>
    /// Project Model Extentions
    /// </summary>
    public static class ProjectExtentions
    {
        /// <summary>
        /// Check if the project is null or new.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static bool IsNullOrNew([NotNullWhen(false)] this Project? project)
        {
            return project is null || project.ID == 0;
        }
    }
}