using System;
using System.Collections.Generic;
using System.Linq;
using Repo;

namespace RepoConstraintsCheck
{
    public class QueryConstraintsCheckStrategy : IConstraintsCheckStrategy
    {
        private IModel model;
        private IEnumerable<INode> operators;
        public QueryConstraintsCheckStrategy(IModel model)
        {
            this.model = model;
            operators = model.Nodes.Where(x => x.Class.Name == "Operator");
        }

        public bool Check(IModel model)
        {
            return CheckPositionalOperatorsHaveReaders() && CheckTupleOperatorsHaveNoReaders();
        }

        private bool CheckPositionalOperatorsHaveReaders()
        {
            foreach (var node in operators)
            {
                var type = node.Attributes.Where(x => x.Name == "type").FirstOrDefault();
                if (type == null)
                {
                    return false;
                }
                if (type.StringValue == "positional")
                {
                    // Check through attribute "children"
                    var children = node.Attributes.Where(x => x.Name == "children").FirstOrDefault().StringValue.Split(", ");
                    if (children.Length == 0)
                    {
                        return false;
                    }
                    foreach (var child in children)
                    {
                        if (!model.Nodes.Where(x => x.Name == child).Any(x => x.Class.Name == "Read"))
                        {
                            return false;
                        }
                    }

                    // Check through edges
                    if (!model.Edges.Where(x => x.From == node).Any(x => x.To.Class.Name == "Read"))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckTupleOperatorsHaveNoReaders()
        {
            foreach (var node in operators)
            {
                var type = node.Attributes.Where(x => x.Name == "type").FirstOrDefault();
                if (type == null)
                {
                    return false;
                }
                if (type.StringValue == "tuple")
                {
                    // Check through attribute "children"
                    var children = node.Attributes.Where(x => x.Name == "children").FirstOrDefault().StringValue.Split(", ");
                    foreach (var child in children)
                    {
                        if (model.Nodes.Where(x => x.Name == child).Any(x => x.Class.Name == "Read"))
                        {
                            return false;
                        }
                    }

                    // Check through edges
                    if (model.Edges.Where(x => x.From == node).Any(x => x.To.Class.Name == "Read"))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
