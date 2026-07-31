using GymManagementBLL.Common;
using GymManagementBLL.ViewModels.SessionViewModels;
using GymSystem.BLL.ViewModels.SessionViewModel;
using GymSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Interfaces
{
    public interface ISessionService 
    {
        Task<IEnumerable<SessionVM>?> GetAllSessionsAsync(CancellationToken ct = default);
        Task<Result> CreateSessionAsync(CreateSessionVM model, CancellationToken ct = default);
        Task<SessionVM?> GetSessionByIdAsync(int sessionId, CancellationToken ct = default);
        Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default);
        Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default);
        Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct = default);
        Task<IEnumerable<TrainerSelectVM>> GetTrainersForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<CategorySelectVM>> GetCategoriesForDropDownAsync(CancellationToken ct = default);


    }
}
