using AutoMapper;
using GymManagementBLL.ViewModels.SessionViewModels;
using GymSystem.BLL.ViewModels.MemberViewModel;
using GymSystem.BLL.ViewModels.SessionViewModel;
using GymSystem.DAL.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            MemberMaps();

            SessionMaps();
        }
        private void MemberMaps()
        {
            CreateMap<Member, MemberViewModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.address.BuildingNumber} - {src.address.Street} - {src.address.City}"))
                .ForMember(dest => dest.DOB, opt => opt.MapFrom(src => src.DOB.ToShortDateString()));

            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();

            CreateMap<Member, MemberToUpdateVM>()
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.address.BuildingNumber))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.address.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.address.City));

            CreateMap<MemberToUpdateVM, Member>()
                .ForMember(dest => dest.photo, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.address.BuildingNumber = src.BuildingNumber;
                    dest.address.Street = src.Street;
                    dest.address.City = src.City;
                });

            CreateMap<CreateMemberVM, Member>()
                .ForMember(dest => dest.address, opt => opt.MapFrom(src => new Address()
                {
                    BuildingNumber = src.BuildingNumber,
                    Street = src.Street,
                    City = src.City

                }))
                .ForMember(dest => dest.healthrecord, opt => opt.MapFrom(src => src.HealthRecordViewModel));

        }
        private void SessionMaps()
        {
            CreateMap<CreateSessionVM, Session>();
            CreateMap<Session, SessionVM>()
                        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.category.CategoryName))
                        .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.trainer.Name))
                        .ForMember(dest => dest.AvailableSlots, opt => opt.Ignore()); 
            CreateMap<UpdateSessionViewModel, Session>().ReverseMap();


            CreateMap<Trainer, TrainerSelectVM>();
            CreateMap<Category, CategorySelectVM>();

        }

    }
}
