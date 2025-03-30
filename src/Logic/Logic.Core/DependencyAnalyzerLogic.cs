namespace Logic.Core
{
    using Interfaces;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.MSBuild;
    using Microsoft.CodeAnalysis;

    using Models;

    public class DependencyAnalyzerLogic : IDependencyAnalyzerLogic
    {
        #region explicit interfaces

        public async Task<List<ProjectDependencyModel>> GetProjectDependenciesAsync(string solutionPath)
        {
            var dependencies = new List<ProjectDependencyModel>();

            using var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionPath);

            foreach (var project in solution.Projects)
            {
                var projectName = project.Name;
                var projectDependency = new ProjectDependencyModel(projectName);

                // Add project references
                foreach (var projectReference in project.ProjectReferences)
                {
                    var referencedProject = solution.GetProject(projectReference.ProjectId);
                    if (referencedProject != null)
                    {
                        projectDependency.Dependencies.Add(referencedProject.Name);
                    }
                }

                dependencies.Add(projectDependency);
            }

            return dependencies;
        }

        #endregion
    }
}