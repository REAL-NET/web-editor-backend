using AutoMapper;
using RepoAPI.Models;
using Repo.DeepMetamodel;

namespace RepoAPI
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<IDeepContext, DeepContext>();
            CreateMap<IDeepElement, ElementInfo>();
            CreateMap<IDeepElement, Element>();
            CreateMap<IDeepAttribute, Attribute>();
            CreateMap<IDeepSlot, Slot>();
            CreateMap<IDeepNode, Node>();
            CreateMap<IDeepRelationship, Relationship>()
                .ForMember(x => x.Type, x => x.MapFrom(y => GetTypeForRelationship(y)));
            CreateMap<IDeepGeneralization, Generalization>();
            CreateMap<IDeepAssociation, Association>();
            CreateMap<IDeepInstanceOf, InstanceOf>();
            CreateMap<IDeepModel, ModelInfo>();
            CreateMap<IDeepModel, Model>();
        }

        private string GetTypeForRelationship(IDeepRelationship relationship)
        {
            return relationship is IDeepAssociation ? "Association" : "Generalization";
        }
    }
}
