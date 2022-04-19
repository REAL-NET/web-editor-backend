using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Repo;

namespace RepoConstraintsCheck
{
    public class QueryConstraintsCheckStrategy : IConstraintsCheckStrategy
    {
        private IModel model;
        private IEnumerable<INode> operators;
        private static readonly IEnumerable<(int, string)> errors = new ReadOnlyCollection<(int, string)>
        (new List<(int, string)> {
             (1, "Type attribute is missing"),
             (2, "Positional operator does not have any readers"),
             (3, "Tuple operator have a reader")
        });

        public QueryConstraintsCheckStrategy(IModel model)
        {
            this.model = model;
            operators = model.Nodes.Where(x => x.Attributes.Where(x => x.Name == "kind").FirstOrDefault().StringValue == "operator");
        }

        public bool Check(IModel model)
        {
            return CheckPositionalOperatorsHaveReaders().Item1 && CheckTupleOperatorsHaveNoReaders().Item1;
        }

        public (bool, IEnumerable<(int, IEnumerable<int>)>) CheckWithErrorInfo(IModel model)
        {
            var result = (true, Enumerable.Empty<(int, IEnumerable<int>)>());
            result.Item1 = Check(model);
            result.Item2 = CheckPositionalOperatorsHaveReaders().Item2.Concat(CheckTupleOperatorsHaveNoReaders().Item2);
            return result;
        }

        private (bool, IEnumerable<(int, IEnumerable<int>)>) AddErrorInfoToResult((bool, IEnumerable<(int, IEnumerable<int>)>) result, int errorCode, int id)
        {
            result.Item1 = false;
            var item = result.Item2.Where(x => x.Item1 == errorCode).FirstOrDefault();
            if (item == default((int, IEnumerable<int>)))
            {
                var ids = new List<int>();
                ids.Add(id);
                result.Item2 = result.Item2.Append((errorCode, ids)).ToList();
            }
            else
            {
                if (!item.Item2.Contains(id))
                {
                    var errorList = result.Item2.Where(x => x.Item1 == errorCode).FirstOrDefault();
                    errorList.Item2 = errorList.Item2.Append(id).ToList();
                    result.Item2 = result.Item2.Where(x => x.Item1 != errorCode).ToList().Append(errorList);
                }
            }
            return result;
        }

        private (bool, IEnumerable<(int, IEnumerable<int>)>) CheckPositionalOperatorsHaveReaders()
        {
            var result = (true, Enumerable.Empty<(int, IEnumerable<int>)>());
            result.Item2 = new List<(int, IEnumerable<int>)>();
            foreach (var node in operators)
            {
                var type = node.Attributes.Where(x => x.Name == "type").FirstOrDefault();
                if (type == null)
                {
                    result = AddErrorInfoToResult(result, 1, node.Id);
                }
                else if (type.StringValue == "positional")
                {
                    if (model.Edges.Where(x => x.From == node).Count() == 0 || !model.Edges.Where(x => x.From == node).Any(x => x.To.Class.Name == "Read"))
                    {
                        result = AddErrorInfoToResult(result, 2, node.Id);
                    }
                }
            }
            return result;
        }

        private (bool, IEnumerable<(int, IEnumerable<int>)>) CheckTupleOperatorsHaveNoReaders()
        {
            var result = (true, Enumerable.Empty<(int, IEnumerable<int>)>());
            result.Item2 = new List<(int, IEnumerable<int>)>();
            foreach (var node in operators)
            {
                var type = node.Attributes.Where(x => x.Name == "type").FirstOrDefault();
                if (type == null)
                {
                    result = AddErrorInfoToResult(result, 1, node.Id);
                }
                else if (type.StringValue == "tuple")
                {
                    var outgoingEdges = model.Edges.Where(x => x.From == node);
                    if (outgoingEdges.Count() != 0 && outgoingEdges.Any(x => x.To.Class.Name == "Read"))
                    {
                        result = AddErrorInfoToResult(result, 3, node.Id);
                        foreach (var outgoingEdge in outgoingEdges)
                        {
                            if (outgoingEdge.To.Class.Name == "Read")
                            {
                                result = AddErrorInfoToResult(result, 3, outgoingEdge.To.Id);
                            }
                        }
                    }
                }
            }
            return result;
        }

        //private (bool, IEnumerable<(int, IEnumerable<int>)>) CheckTupleOperatorsHaveNoReaders()
        //{
        //    foreach (var node in operators)
        //    {
        //        var type = node.Attributes.Where(x => x.Name == "type").FirstOrDefault();
        //        if (type == null)
        //        {
        //            return false;
        //        }
        //        if (type.StringValue == "tuple")
        //        {
        //            // Check through attribute "children"
        //            var children = node.Attributes.Where(x => x.Name == "children").FirstOrDefault().StringValue.Split(", ");
        //            foreach (var child in children)
        //            {
        //                if (model.Nodes.Where(x => x.Name == child).Any(x => x.Class.Name == "Read"))
        //                {
        //                    return false;
        //                }
        //            }

        //            // Check through edges
        //            if (model.Edges.Where(x => x.From == node).Any(x => x.To.Class.Name == "Read"))
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}
    }
}
