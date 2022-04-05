using AutoMapper;
using Baltic.DataModel.Execution;
using Baltic.TaskManager.Models;

namespace Baltic.TaskManager
{
    public class TaskManagerProfile : Profile 
    {
        public TaskManagerProfile()
        {
            CreateMap<XTaskParameters, TaskParameters>()
                .ForMember(dest =>
                        dest.ReservationRange,
                    opt => opt.Ignore())
                .ForMember(dest =>
                        dest.CustomParameters,
                    opt => opt.Ignore());
            /* .ForMember(dest =>
                    dest.ReservationRange.MinReservation.Gpus,
                opt => opt.MapFrom(src => src.MinGPUs))
            .ForMember(dest =>
                    dest.ReservationRange.MinReservation.Memory,
                opt => opt.MapFrom(src => src.MinMemory))
            .ForMember(dest =>
                    dest.ReservationRange.MinReservation.Storage,
                opt => opt.MapFrom(src => src.MinStorage))
            .ForMember(dest =>
                    dest.ReservationRange.MaxReservation.Cpus,
                opt => opt.MapFrom(src => src.MaxCPUs))
            .ForMember(dest =>
                    dest.ReservationRange.MaxReservation.Gpus,
                opt => opt.MapFrom(src => src.MaxGPUs))
            .ForMember(dest =>
                    dest.ReservationRange.MaxReservation.Memory,
                opt => opt.MapFrom(src => src.MaxMemory))
            .ForMember(dest =>
                    dest.ReservationRange.MaxReservation.Storage,
                opt => opt.MapFrom(src => src.MaxStorage)); */
        }
    }
}