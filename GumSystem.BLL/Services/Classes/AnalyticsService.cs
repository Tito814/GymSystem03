
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.AnalyticsViewModels;
using GymSystem.DAL.Models;
using GymSystem.DAL.Repo.Interfaces;

namespace GymManagementBLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AnalyticsViewModel> GetAnalyticsDataAsync(CancellationToken ct = default)
        {
            var now = DateTime.Now;
            var upcomingSessions = await _unitOfWork.GetRepo<Session>().CountAsync(s => s.StartDate > now);
            var ongoingSessions = await _unitOfWork.GetRepo<Session>().CountAsync(X => X.StartDate <= now && X.EndDate >= now);
            var completedSessions = await _unitOfWork.GetRepo<Session>().CountAsync(X => X.EndDate < now);
            var totalMembers = await _unitOfWork.GetRepo<Member>().CountAsync(ct: ct);
            var totalTrainers = await _unitOfWork.GetRepo<Trainer>().CountAsync(ct: ct);
            var activeMembers = await _unitOfWork.GetRepo<MemberShip>().CountAsync(m => m.EndDate > now, ct);
            return new AnalyticsViewModel()
            {
                TotalMembers = totalMembers,
                TotalTrainers = totalTrainers,
                ActiveMembers = activeMembers,
                UpcomingSessions = upcomingSessions,
                OngoingSessions = ongoingSessions,
                CompletedSessions = completedSessions
            };
        }
    }
}
