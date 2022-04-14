using System;
using System.Collections.Generic;
using System.Linq;
using Repo;

namespace RepoConstraintsCheck
{
    public class QueryConstraintsCheckStrategy : IConstraintsCheckStrategy
    {
        private IModel model;
        private INode[] nodes;
        public QueryConstraintsCheckStrategy(IModel model)
        {
            this.model = model;
            nodes = model.Nodes.ToArray();
        }

        public bool Check(IModel model)
        {
            return CheckDataTypes() && isAcyclic();
        }

        private bool CheckDataTypes()
        {
            foreach (var edge in model.Edges)
            {
                var output = edge.From.Attributes.Where(x => x.Name == "outputType").First().StringValue;
                var inputs = edge.To.Attributes.Where(x => x.Name == "inputType").Select(x => x.StringValue);
                var result = inputs.Contains(output);
                if (!result) return false;
            }
            return true;
        }

        private bool isAcyclic()
        {
            var roots = new List<INode>();
            foreach (var node in model.Nodes)
            {
                var inEdges = model.Edges.Where(x => x.To == node).ToList();
                if (inEdges.Count() == 0)
                {
                    roots.Add(node);
                }
            }
            if (roots.Count == 0) return model.Edges.Count() == 0;
            var result = true;
            foreach (var edge in roots)
            {
                result = result && isAcyclicPart(edge, new List<int>(), new List<int>());
            }
            return result;
        }

        private bool isAcyclicPart(INode node, List<int> visited, List<int> stack)
        {
            if (stack.Contains(node.Id)) return false;
            if (visited.Contains(node.Id)) return true;
            stack.Add(node.Id);
            visited.Add(node.Id);
            var children = model.Edges.Where(x => x.From == node).Select(x => (INode)x.To).ToList();
            foreach (var c in children)
            {
                if (isAcyclicPart(c, visited, stack))
                {
                    return true;
                }
            }
            stack.Remove(node.Id);
            return true;
        }

        private bool isCyclicUtil(int i, bool[] visited, bool[] stack)
        {
            if (stack[i])
                return true;

            if (visited[i])
                return false;

            visited[i] = true;

            stack[i] = true;
            var children = model.Edges.Where(x => x.From == nodes[i]).Select(x => Array.IndexOf(nodes, x)).ToList();

            foreach (int c in children)
                if (isCyclicUtil(c, visited, stack))
                    return true;

            stack[i] = false;

            return false;
        }
    }
}
