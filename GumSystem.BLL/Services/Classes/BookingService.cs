
using AutoMapper;
using GymManagementBLL.Common;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.BookingViewModels;
using GymManagementBLL.ViewModels.MembershipViewModels;
using GymManagementDAL.Repositories.Interfaces;
using GymSystem.BLL.ViewModels.SessionViewModel;
using GymSystem.DAL.Models;
using GymSystem.DAL.Repo.Interfaces;

namespace GymManagementBLL.Services.Classes
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIDAsync(sessionId, ct);
            if (session is null) return Result.NotFound("Session not found.");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot cancel a booking for a session that has already started.");

            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.sessionId == sessionId && b.memberId == memberId, tracking: true, ct: ct);
            if (booking is null) return Result.NotFound("Booking not found.");

            _unitOfWork.BookingRepository.DeleteAsync(booking);
            var result = await _unitOfWork.Completed(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Booking Cancel Failed");
        }
        public async Task<Result> MarkAttendedAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.memberId == memberId && b.sessionId == sessionId, tracking: true, ct: ct);
            if (booking is null) return Result.NotFound("Booking not found.");

            booking.IsAttended = true;
            booking.UpdatedAt = DateTime.Now;
            _unitOfWork.BookingRepository.UpdateAsync(booking);

            var result = await _unitOfWork.Completed(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Mark As Attended");
        }
        public async Task<Result> CreateNewBookingAsync(CreateBookingViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIDAsync(model.SessionId, ct);
            if (session is null) return Result.NotFound("Session not found.");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot book a session that has already started.");

            var hasActiveMembership = await _unitOfWork.MembershipRepository
                .AnyAsync(m => m.memberId == model.MemberId && m.EndDate > DateTime.Now, ct);
            if (!hasActiveMembership)
                return Result.Fail("Member does not have an active membership.");

            // Prevent double-booking the same member into the same session.
            var alreadyBooked = await _unitOfWork.BookingRepository
                .AnyAsync(b => b.sessionId == model.SessionId && b.memberId == model.MemberId, ct);
            if (alreadyBooked)
                return Result.Fail("Member is already booked for this session.");

            var booked = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(model.SessionId, ct);
            if (booked >= session.Capacity)
                return Result.Fail("Session is full.");

            _unitOfWork.BookingRepository.AddAsync(new Booking
            {
                memberId = model.MemberId,
                sessionId = model.SessionId,
                IsAttended = false,
                CreatedAt = DateTime.Now,
            });

            var result = await _unitOfWork.Completed(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Book Session");
        }
        public async Task<IEnumerable<SessionVM>> GetAllSessionsAsync(CancellationToken ct = default)
        {

            var bookings = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(x => x.EndDate >= DateTime.Now);
            if (!bookings.Any()) return null!;
            var MappedSession = _mapper.Map<IEnumerable<SessionVM>>(bookings);
            foreach (var item in MappedSession)
            {
                item.AvailableSlots = item.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(item.Id);
            }
            return MappedSession;
        }
        public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForUpcomingBySessionIdAsync(
         int sessionId, CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBySessionIdAsync(sessionId, ct);
            return bookings.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.memberId,
                SessionId = sessionId,
                MemberName = b.member.Name,
                BookingDate = b.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
            }).ToList();
        }
        public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForOngoingBySessionIdAsync(
         int sessionId, CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBySessionIdAsync(sessionId, ct);
            return bookings.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.memberId,
                SessionId = sessionId,
                MemberName = b.member.Name,
                BookingDate = b.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                IsAttended = b.IsAttended,
            }).ToList();
        }

        public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForDropDownAsync(int sessionId, CancellationToken ct)
        {
            var booking = await _unitOfWork.BookingRepository.GetAllAsync(x => x.sessionId == sessionId);

            var bookedMemberIds = booking.Select(x => x.memberId);

            var availableMembers = await _unitOfWork.GetRepo<Member>()
                                              .GetAllAsync(x => !bookedMemberIds.Contains(x.Id));

            return (IEnumerable<MemberForSessionViewModel>)_mapper.Map<IEnumerable<MemberSelectListViewModel>>(availableMembers);
        }
    }
}
