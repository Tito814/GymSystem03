using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.MemberViewModel;
using GymSystem.BLL.ViewModels.TrainerViewModel;
using GymSystem.DAL.Models;
using GymSystem.DAL.Models.Enums;
using GymSystem.DAL.Repo.Classes;
using GymSystem.DAL.Repo.Interfaces;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class TrainerServices : ITrainerService
    {
        private readonly IUnitOfWork _UnitOfWork;
        public TrainerServices(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }


        public Task<bool> CreateTrainerAsync(CreateTrainerVM model, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TrainerVM>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _UnitOfWork.GetRepo<Trainer>().GetAllAsync(ct: ct);

            if (!trainers.Any()) return [];

            List<TrainerVM> trainerViewModels = new List<TrainerVM>();
            foreach (var item in trainers)
            {
                var TrainerViewModel = new TrainerVM()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email,
                    Phone = item.Phone,
                    DateOfBirth = item.DOB.ToString(),
                    Address = $"{item.address.BuildingNumber} - {item.address.Street} - {item.address.City}",
                    Specialties = item.Specialty.ToString(),
                    Gender = item.Gender.ToString(),

                };
                trainerViewModels.Add(TrainerViewModel);
            }
            return trainerViewModels;
        }

        public async Task<TrainerVM?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _UnitOfWork.GetRepo<Trainer>().GetByIDAsync(trainerId, ct);

            if (trainer is null)
                return null;
            else
            {
                var TrainerViewModel = new TrainerVM()
                {
                    Id = trainer.Id,
                    Name = trainer.Name,
                    Email = trainer.Email,
                    Phone = trainer.Phone,
                    DateOfBirth = trainer.DOB.ToString(),
                    Address = $"{trainer.address.BuildingNumber} - {trainer.address.Street} - {trainer.address.City}",
                    Specialties = trainer.Specialty.ToString(),
                    Gender = trainer.Gender.ToString()

                };
                return TrainerViewModel;
            }

        }

        public async Task<TrainerToUpdateVM?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _UnitOfWork.GetRepo<Trainer>().GetByIDAsync(trainerId, ct);

            if (trainer is null) return null;
            else
            {
                var TrainerToUpdateViewModel = new TrainerToUpdateVM()
                {
                    Name = trainer.Name,
                    Email = trainer.Email,
                    Phone = trainer.Phone,
                    BuildingNumber = trainer.address.BuildingNumber,
                    Street = trainer.address.Street,
                    City = trainer.address.City,
                    Specialties = trainer.Specialty
                };
                return TrainerToUpdateViewModel;
            }
        }

        public async Task<bool> RemoveTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _UnitOfWork.GetRepo<Trainer>().GetByIDAsync(trainerId, ct);

            if (trainer is null) return false;

            var hasActiveSessions = await _UnitOfWork.GetRepo<Session>().AnyAsync(s => s.trainerId == trainerId && s.StartDate > DateTime.Now, ct);
            if (hasActiveSessions) return false;

            _UnitOfWork.GetRepo<Trainer>().DeleteAsync(trainer);
            var result = await _UnitOfWork.Completed(ct);
            return result > 0;

        }

        public async Task<bool> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateVM model, CancellationToken ct = default)
        {
            var trainer = await _UnitOfWork.GetRepo<Trainer>().GetByIDAsync(trainerId, ct);
            if (trainer is null) return false;

            var EmailExist = await _UnitOfWork.GetRepo<Trainer>().AnyAsync(m => m.Email == model.Email && m.Id != trainerId, ct);
            var PhoneExist = await _UnitOfWork.GetRepo<Trainer>().AnyAsync(m => m.Phone == model.Phone && m.Id != trainerId, ct);

            if (EmailExist || PhoneExist) return false;

            trainer.Name = model.Name;
            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.address.BuildingNumber = model.BuildingNumber;
            trainer.address.Street = model.Street;
            trainer.address.City = model.City;
            trainer.Specialty = model.Specialties;
            trainer.UpdatedAt = DateTime.Now;

            _UnitOfWork.GetRepo<Trainer>().UpdateAsync(trainer);

            var result = await _UnitOfWork.Completed(ct);
            return result > 0;

        }
    }
}
