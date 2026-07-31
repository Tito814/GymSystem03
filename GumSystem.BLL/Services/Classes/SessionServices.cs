using AutoMapper;
using GymManagementBLL.Common;
using GymManagementBLL.ViewModels.SessionViewModels;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.SessionViewModel;
using GymSystem.DAL.Models;
using GymSystem.DAL.Models.Enums;
using GymSystem.DAL.Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class SessionServices : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        // Get all Sessions
        public async Task<Result> CreateSessionAsync(CreateSessionVM model, CancellationToken ct = default)
        {
            if (model.StartDate <= DateTime.Now)
                return Result.Validation("Start date must be in the future.");

            if (model.EndDate <= model.StartDate)
                return Result.Validation("End date must be after start date.");

            if (model.Capacity < 1 || model.Capacity > 25)
                return Result.Validation("Capacity Must be between 1 and 25 ");

            // Get Trainer
            var trainerRepo = _unitOfWork.GetRepo<Trainer>();

            var trainer = await trainerRepo.GetByIDAsync(model.TrainerId, ct);

            if (trainer is null)
                return Result.NotFound("Trainer not found.");


            // Get Category
            var categoryRepo = _unitOfWork.GetRepo<Category>();
            var category = await categoryRepo.GetByIDAsync(model.CategoryId, ct);

            if (category is null)
                return Result.NotFound("Category not found.");

            // Check if trainer specialty == category specialty
            var IsValid = Enum.TryParse<Specialty>(category.CategoryName, true, out var CategorySpecialty);
            if (!IsValid || trainer.Specialty != CategorySpecialty)
                return Result.Validation("Trainer Specialty not equal Category Specialty");

            var session = _mapper.Map<Session>(model);

            _unitOfWork.GetRepo<Session>().AddAsync(session);

            var AffectedRows = await _unitOfWork.Completed(ct);

            return AffectedRows > 0 ? Result.Ok() : Result.Fail("Failed to create session.");

        }

        public async Task<IEnumerable<SessionVM>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(ct: ct);


            if (sessions?.Any() != true) return null;

            sessions = sessions.OrderByDescending(X => X.StartDate);
            var MappedSessions = _mapper.Map<IEnumerable<SessionVM>>(sessions);

            foreach (var session in MappedSessions)
            {
                session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            }
            return MappedSessions;
        }

        public async Task<IEnumerable<CategorySelectVM>> GetCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            var categories = await _unitOfWork.GetRepo<Category>().GetAllAsync(ct: ct);
            return _mapper.Map<List<CategorySelectVM>>(categories); ;
        }

        public async Task<SessionVM?> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategoryAsync(sessionId, ct);

            if (session == null)
                return null;

            var MappedSession = _mapper.Map<Session, SessionVM>(session);
            MappedSession.AvailableSlots = MappedSession.Capacity - (await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct));
            return MappedSession;
        }

        public async Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.GetRepo<Session>().GetByIDAsync(sessionId, ct);
            if (session is null) return null;
            if (!await IsSessionValidForUpdatingAsync(session, ct)) return null;
            return _mapper.Map<UpdateSessionViewModel>(session);
        }

        public async Task<IEnumerable<TrainerSelectVM>> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepo<Trainer>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<TrainerSelectVM>>(trainers);
        }

        public async Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepo<Session>();
            var session = await repo.GetByIDAsync(sessionId, ct);
            if (session is null) return Result.NotFound("Session not found.");

            if (session.EndDate >= DateTime.Now)
                return Result.Fail("Cannot delete a session that has not yet ended.");

            var bookedCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            if (bookedCount > 0)
                return Result.Fail("Cannot delete a session that has bookings.");

            repo.DeleteAsync(session);
            var affectedRows = await _unitOfWork.Completed(ct);

            return affectedRows > 0 ? Result.Ok() : Result.Fail("Failed to Delete session.");
        }

        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var sessionRepo = _unitOfWork.GetRepo<Session>();
            var session = await sessionRepo.GetByIDAsync(id, ct);

            if (session is null)
                return Result.NotFound("Session not found.");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot edit a session that has already started.");

            var bookedCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);

            if (bookedCount > 0)
                return Result.Fail("Cannot edit a session that already has bookings.");

            if (model.EndDate <= model.StartDate)
                return Result.Validation("End date must be after start date.");

            if (model.StartDate <= DateTime.Now)
                return Result.Validation(
                    "Start date must be in the future.");

            var trainerRepo = _unitOfWork.GetRepo<Trainer>();

            var trainer = await trainerRepo.GetByIDAsync(model.TrainerId, ct);

            if (trainer is null)
                return Result.NotFound("Trainer not found.");

            var categoryRepo = _unitOfWork.GetRepo<Category>();
            var category = await categoryRepo.GetByIDAsync(session.categoryId, ct);

            if (category is null)
                return Result.NotFound("Category not found.");

            var isValidSpecialty = Enum.TryParse<Specialty>(category.CategoryName, true, out var categorySpecialty);

            if (!isValidSpecialty || trainer.Specialty != categorySpecialty)
            {
                return Result.Validation("This trainer does not match the session category.");
            }

            _mapper.Map(model, session);

            session.UpdatedAt = DateTime.Now;

            sessionRepo.UpdateAsync(session);

            var affectedRows = await _unitOfWork.Completed(ct);

            return affectedRows > 0 ? Result.Ok() : Result.Fail("Failed to update session.");
        }

        private async Task<bool> IsSessionValidForUpdatingAsync(Session session, CancellationToken ct = default)
        {
            if (session.StartDate <= DateTime.Now) return false;
            var booked = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            return booked == 0;
        }
    }
}
