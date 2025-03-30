namespace Logic.Visualizers
{
    using Interfaces;

    using Models;

    using QuickGraph;

    public class GraphVisualizationLogic : IGraphVisualizationLogic
    {
        #region explicit interfaces

        public void GenerateGraphImage(List<ProjectDependencyModel> projectDependencies, string outputDotFilePath)
        {
            // Step 1: Build the graph in QuickGraph
            var graph = new AdjacencyGraph<string, Edge<string>>();

            // Add vertices and edges
            foreach (var dependency in projectDependencies)
            {
                if (!graph.ContainsVertex(dependency.ProjectName))
                {
                    graph.AddVertex(dependency.ProjectName);
                }
                foreach (var dependentProject in dependency.Dependencies)
                {
                    if (!graph.ContainsVertex(dependentProject))
                    {
                        graph.AddVertex(dependentProject);
                    }
                    graph.AddEdge(new Edge<string>(dependency.ProjectName, dependentProject));
                }
            }

            // Step 2: Export the graph to DOT format
            var dotContent = ExportToDotFormat(graph);
            File.WriteAllText(outputDotFilePath, dotContent);
        }

        #endregion

        #region methods

        private static string ExportToDotFormat(IEdgeSet<string, Edge<string>> graph)
        {
            var dotBuilder = new StringWriter();
            dotBuilder.WriteLine("digraph G {");
            foreach (var edge in graph.Edges)
            {
                dotBuilder.WriteLine($"  \"{edge.Source}\" -> \"{edge.Target}\";");
            }
            dotBuilder.WriteLine("}");
            return dotBuilder.ToString();
        }


        #endregion
    }
}