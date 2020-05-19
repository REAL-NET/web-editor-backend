using AutoMapper;
using Repo;
using RepoAPI.Models;
using System.Linq;

namespace RepoAPI
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Repo.Metatype, Models.Metatype>().ReverseMap();
            CreateMap<Repo.AttributeKind, Models.AttributeKind>().ReverseMap();
            CreateMap<IAttribute, Attribute>().ReverseMap();
            CreateMap<IElement, ElementInfo>();
            CreateMap<IElement, Element>()
                .ForMember(x => x.InstanceMetatype, x => x.MapFrom(y => y.IsAbstract ? 0 : y.InstanceMetatype))
                .ForMember(x => x.Shape, x => x.MapFrom(y => y.IsAbstract ? null : y.Shape));
            CreateMap<INode, Node>()
                .ForMember(x => x.InstanceMetatype, x => x.MapFrom(y => y.IsAbstract ? 0 : y.InstanceMetatype))
                .ForMember(x => x.Shape, x => x.MapFrom(y => y.IsAbstract ? null : y.Shape));
            CreateMap<IEdge, Edge>()
                .ForMember(x => x.InstanceMetatype, x => x.MapFrom(y => y.IsAbstract ? 0 : y.InstanceMetatype))
                .ForMember(x => x.Shape, x => x.MapFrom(y => y.IsAbstract ? null : y.Shape));
            CreateMap<IModel, Model>()
                .ForMember(x => x.MetamodelName, x => x.MapFrom(y => y.Metamodel.Name));
        }
    }
}
