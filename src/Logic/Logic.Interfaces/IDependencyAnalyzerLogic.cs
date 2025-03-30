namespace Logic.Interfaces
{
    using Models;

    public interface IDependencyAnalyzerLogic
    {
        #region methods

        Task<List<ProjectDependencyModel>> GetProjectDependenciesAsync(string solutionPath);

        #endregion
    }
}