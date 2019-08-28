#region Using Directives

using System.Linq;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Utilities {

    class ProjectMapper {

        private readonly ImmutableArray<ProjectInfo> _projectEntries;

        public ProjectMapper(ImmutableArray<ProjectInfo> projectEntries) {
            _projectEntries = projectEntries;

        }

        public static string MiscellaneousFiles = "Miscellaneous Files";

        public static readonly ProjectMapper Empty = new ProjectMapper(ImmutableArray<ProjectInfo>.Empty);

        public ProjectInfo GetProjectInfo(string fileName) {

            var uri = UriBuilder.BuildDirectoryUriFromFile(fileName);
            if (uri == null) {
                return default;
            }

            var projectEntry = _projectEntries.FirstOrDefault(pe => pe.ProjectDirectory.IsBaseOf(uri));

            return projectEntry;

        }

    }

}