#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Utilities {

    struct ProjectInfo {

        private readonly string _name;

        public ProjectInfo(Uri directory, string name, Guid projectGuid) {

            _name            = name      ?? throw new ArgumentNullException(nameof(name));
            ProjectDirectory = directory ?? throw new ArgumentNullException(nameof(directory));
            ProjectGuid      = projectGuid;
        }

        public string ProjectName      => _name ?? ProjectMapper.MiscellaneousFiles;
        public Uri    ProjectDirectory { get; }
        public Guid   ProjectGuid      { get; }

    }

}