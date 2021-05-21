using System;
using System.Collections.Generic;
using System.Linq;
using Generator.Requests;
using Newtonsoft.Json;
using RepoAPI.Models;

namespace Generator.Services
{
    public class RepoLoader
    {
        private Dictionary<int, Element> _elementMap = new Dictionary<int, Element>();

        public RepoLoader()
        {
        }

        public RepoLoader(string token)
        {
            RepoRequest.SetToken(token);
        }

        public Model LoadModel(string modelName)
        {
            var modelJson = RepoRequest.GetModelAsString(modelName);
            var model = JsonConvert.DeserializeObject<Model>(modelJson);

            if (!model.MetamodelName.Equals("InfrastructureMetamodel"))
            {
                LoadModel(model.MetamodelName);
            }

            var edgeIds = model.Edges.Select(node => node.Id).ToList();
            var elementIds = model.Elements.Select(node => node.Id).ToList();
            LoadElements(modelName, elementIds, edgeIds);

            RestoreElements(elementIds, edgeIds);
            model.Elements = model.Elements.Select(element => _elementMap[element.Id]).ToList();
            model.Nodes = model.Nodes.Select(element => (Node) _elementMap[element.Id]).ToList();
            model.Edges = model.Edges.Select(element => (Edge) _elementMap[element.Id]).ToList();
            
            return model;
        }


        private void RestoreElements(ICollection<int> elementIds, ICollection<int> edgeIds)
        {
            foreach (var elementId in elementIds)
            {
                var element = _elementMap[elementId];
                foreach (var attribute in element.Attributes)
                {
                    attribute.Type = attribute.Type == null ? null : _elementMap[attribute.Type.Id];
                    attribute.ReferenceValue = attribute.ReferenceValue == null
                        ? null
                        : _elementMap[attribute.ReferenceValue.Id];
                }

                if (edgeIds.Contains(element.Id))
                {
                    var edge = (Edge) element;
                    Console.Out.WriteLine(edge.Id);
                    edge.From = edge.From == null ? null : _elementMap[edge.From.Id];
                    edge.To = edge.To == null ? null : _elementMap[edge.To.Id];
                }
            }
        }

        private void LoadElements(string modelName, 
            ICollection<int> elementIds, ICollection<int> edgeIds)
        {
            foreach (var elementId in elementIds)
            {
                if (edgeIds.Contains(elementId))
                {
                    var elementJson = RepoRequest.GetEdgeAsString(modelName, elementId);
                    var element = JsonConvert.DeserializeObject<Edge>(elementJson);
                    _elementMap.Add(elementId, element);
                }
                else
                {
                    var elementJson = RepoRequest.GetElementAsString(modelName, elementId);
                    var element = JsonConvert.DeserializeObject<Node>(elementJson);
                    _elementMap.Add(elementId, element);
                }
            }
        }

    }
}