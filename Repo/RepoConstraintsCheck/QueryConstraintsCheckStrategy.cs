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
             (2, "Positional operators must have readers"),
             (3, "Tuple operators must not have readers"),
             (4, "Leaves must be DataSource operators"),
             (5, "Join operators must have at least two readers"),
             (6, "Last operator must be tuple"),
             (7, "Tuple operators must have tuple children"),
             (8, "Positional operators must have positional children"),
             (9, "Materializing operators must have positional children"),
             (10, "Materializing operators must have tuple parent")
        });

        public QueryConstraintsCheckStrategy(IModel model)
        {
            this.model = model;
            operators = model.Nodes.Where(x => x.Attributes.Where(x => x.Name == "kind").FirstOrDefault().StringValue == "operator");
        }

        public (bool, IEnumerable<(int, IEnumerable<int>)>) Check(IModel model)
        {
            var result = (true, Enumerable.Empty<(int, IEnumerable<int>)>());
            result.Item1 = CheckPositionalOperatorsHaveReaders().Item1
                && CheckTupleOperatorsHaveNoReaders().Item1
                && CheckLeavesAreDS().Item1
                && CheckLastOperatorIsTuple().Item1
                && CheckMaterializingOperatorsHaveTupleParent().Item1
                && CheckChildrenTypesAreCorrect().Item1;
            result.Item2 = CheckPositionalOperatorsHaveReaders().Item2
                .Concat(CheckTupleOperatorsHaveNoReaders().Item2)
                .Concat(CheckLeavesAreDS().Item2)
                .Concat(CheckLastOperatorIsTuple().Item2)
                .Concat(CheckMaterializingOperatorsHaveTupleParent().Item2)
                .Concat(CheckChildrenTypesAreCorrect().Item2);
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
                    if (node.Class.Name != "DS" && node.Class.Name != "PosAND" && node.Class.Name != "PosOR" && node.Class.Name != "PosNOT")
                    {
                        var outgoingEdgesToReaders = model.Edges.Where(x => x.From == node && x.To.Class.Name == "Read");
                        if (outgoingEdgesToReaders.Count() == 0)
                        {
                            result = AddErrorInfoToResult(result, 2, node.Id);
                        }
                        else if (node.Class.Name == "Join")
                        {
                            if (outgoingEdgesToReaders.Count() < 2)
                            {
                                result = AddErrorInfoToResult(result, 5, node.Id);
                            }
                        }
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

        private (bool, IEnumerable<(int, IEnumerable<int>)>) CheckLeavesAreDS()
        {
            var result = (true, Enumerable.Empty<(int, IEnumerable<int>)>());
            result.Item2 = new List<(int, IEnumerable<int>)>();
            foreach (var node in operators)
            {
                var incomingEdges = model.Edges.Where(x => x.To == node && x.From.Class.Name != "OperatorInternals");
                var outgoingEdgesToOperators = model.Edges.Where(x => x.From == node && x.To.Class.Name != "Read");
                if (incomingEdges.Count() != 0 && outgoingEdgesToOperators.Count() == 0)
                {
                    if (node.Class.Name != "DS")
                    {
                        result = AddErrorInfoToResult(result, 4, node.Id);
                    }
                }
            }
            return result;
        }

        private (bool, IEnumerable<(int, IEnumerable<int>)>) CheckLastOperatorIsTuple()
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
                else
                {
                    var incomingEdges = model.Edges.Where(x => x.To == node && x.From.Class.Name != "OperatorInternals");
                    var outgoingEdgesToOperators = model.Edges.Where(x => x.From == node && x.To.Class.Name != "Read");
                    if (incomingEdges.Count() == 0 && outgoingEdgesToOperators.Count() != 0)
                    {
                        if (type.StringValue != "tuple")
                        {
                            result = AddErrorInfoToResult(result, 6, node.Id);
                        }
                    }
                }
            }
            return result;
        }

        private (bool, IEnumerable<(int, IEnumerable<int>)>) CheckMaterializingOperatorsHaveTupleParent()
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
                else
                {
                    if (node.Class.Name == "Aggregate" || node.Class.Name == "Materialize")
                    {
                        var incomingEdgesFromOperators = model.Edges.Where(x => x.To == node && x.From.Class.Name != "OperatorInternals");
                        if (incomingEdgesFromOperators.Count() != 0)
                        {
                            foreach (var incomingEdge in incomingEdgesFromOperators)
                            {
                                var parentOperator = incomingEdge.From;
                                var parentOperatorType = parentOperator.Attributes.Where(x => x.Name == "type").FirstOrDefault();
                                if (parentOperatorType.StringValue != "tuple")
                                {
                                    result = AddErrorInfoToResult(result, 10, parentOperator.Id);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        //private (bool, IEnumerable<(int, IEnumerable<int>)>) CheckMaterializationLinePositionIsCorrect()
        //{
        //    var result = (true, Enumerable.Empty<(int, IEnumerable<int>)>());
        //    result.Item2 = new List<(int, IEnumerable<int>)>();
        //    var materializationLine = model.Nodes.Where(x => x.Attributes.Where(x => x.Name == "kind")
        //    .FirstOrDefault().StringValue == "materializationLine");
        //    //foreach (var node in operators)
        //    //{
        //    //    var type = node.Attributes.Where(x => x.Name == "type").FirstOrDefault();
        //    //    if (type == null)
        //    //    {
        //    //        result = AddErrorInfoToResult(result, 1, node.Id);
        //    //    }
        //    //    else
        //    //    {
        //    //        var outgoingEdgesToOperators = model.Edges.Where(x => x.From == node && x.To.Class.Name != "Read");
        //    //        if (outgoingEdgesToOperators.Count() != 0)
        //    //        {
        //    //            foreach (var outgoingEdge in outgoingEdgesToOperators)
        //    //            {
        //    //                var childOperator = outgoingEdge.To;
        //    //                var childOperatorType = childOperator.Attributes.Where(x => x.Name == "type").FirstOrDefault();
        //    //                if (type.StringValue == "tuple")
        //    //                {
        //    //                    if (node.Class.Name != "Aggregate" && node.Class.Name != "Materialize")
        //    //                    {
        //    //                        if (childOperatorType.StringValue != "tuple")
        //    //                        {
        //    //                            result = AddErrorInfoToResult(result, 7, childOperator.Id);
        //    //                        }
        //    //                    }
        //    //                    else
        //    //                    {
        //    //                        if (childOperatorType.StringValue != "positional")
        //    //                        {
        //    //                            result = AddErrorInfoToResult(result, 9, childOperator.Id);
        //    //                        }
        //    //                    }
        //    //                }
        //    //                else if (type.StringValue == "positional")
        //    //                {
        //    //                    if (childOperatorType.StringValue != "positional")
        //    //                    {
        //    //                        result = AddErrorInfoToResult(result, 8, childOperator.Id);
        //    //                    }
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    return result;
        //}

        private (bool, IEnumerable<(int, IEnumerable<int>)>) CheckChildrenTypesAreCorrect()
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
                else
                {
                    var outgoingEdgesToOperators = model.Edges.Where(x => x.From == node && x.To.Class.Name != "Read");
                    if (outgoingEdgesToOperators.Count() != 0)
                    {
                        foreach (var outgoingEdge in outgoingEdgesToOperators)
                        {
                            var childOperator = outgoingEdge.To;
                            var childOperatorType = childOperator.Attributes.Where(x => x.Name == "type").FirstOrDefault();
                            if (type.StringValue == "tuple")
                            {
                                if (node.Class.Name != "Aggregate" && node.Class.Name != "Materialize")
                                {
                                    if (childOperatorType.StringValue != "tuple")
                                    {
                                        result = AddErrorInfoToResult(result, 7, childOperator.Id);
                                    }
                                }
                                else
                                {
                                    if (childOperatorType.StringValue != "positional")
                                    {
                                        result = AddErrorInfoToResult(result, 9, childOperator.Id);
                                    }
                                }
                            }
                            else if (type.StringValue == "positional")
                            {
                                if (childOperatorType.StringValue != "positional")
                                {
                                    result = AddErrorInfoToResult(result, 8, childOperator.Id);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
