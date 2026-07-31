using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels;
using GymSystem.BLL.ViewModels.PlanViewModel;
using GymSystem.DAL.Models;
using GymSystem.DAL.Repo.Interfaces;
using GymSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepo<Plan>().GetAllAsync(ct: ct);
            if (!plans.Any()) return Array.Empty<PlanViewModel>();


            List<PlanViewModel> planViewModels = new List<PlanViewModel>();
            foreach (var item in plans)
            {
                var planViewModel = new PlanViewModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    DurationDays = item.DurationInDays,
                    Price = item.Price,
                    IsActive = item.IsActive
                };
                planViewModels.Add(planViewModel);
            }
            return planViewModels;

        }
        public async Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepo<Plan>().GetByIDAsync(planId, ct);
            if (plan is null) return null;

            return new PlanViewModel
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationInDays,
                Price = plan.Price,
                IsActive = plan.IsActive
            };
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepo<Plan>().GetByIDAsync(planId, ct);
            if (plan is null || !plan.IsActive) return null;

            if (await HasActiveMembershipsAsync(planId, ct)) return null;

            return new UpdatePlanViewModel
            {
                PlanName = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationInDays,
                Price = plan.Price
            };

        }


        public async Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepo<Plan>().GetByIDAsync(id, ct);
            if (plan is null) return false;
            if (await HasActiveMembershipsAsync(id, ct))
                return false;

            var updatedPlan = new Plan
            {
                Id = plan.Id,
                Name = model.PlanName,
                Description = model.Description,
                DurationInDays = model.DurationDays,
                Price = model.Price,
                IsActive = plan.IsActive
            };
            _unitOfWork.GetRepo<Plan>().UpdateAsync(updatedPlan);
            var result = await _unitOfWork.Completed(ct);
            return result > 0;
        }

        private async Task<bool> HasActiveMembershipsAsync(int planId, CancellationToken ct)
        {
            return await _unitOfWork.GetRepo<MemberShip>().AnyAsync(m => m.planId == planId && m.EndDate > DateTime.Now, ct);
        }

    }
}
