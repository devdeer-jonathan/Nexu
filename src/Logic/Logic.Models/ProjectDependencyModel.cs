namespace Logic.Models
{
    public class ProjectDependencyModel
    {
        #region constructors and destructors

        public ProjectDependencyModel(string projectName)
        {
            ProjectName = projectName;
            Dependencies = new List<string>();
        }

        #endregion

        #region properties

        public string ProjectName { get; set; }

        public List<string> Dependencies { get; set; }

        #endregion
    }
}