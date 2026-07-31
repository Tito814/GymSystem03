using AutoMapper;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.MemberViewModel;
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
    public class MemberServices : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public MemberServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<bool> CreateMemberAsync(CreateMemberVM member, CancellationToken ct = default)
        {
            // Check if the email already exists in the database
            var EmailExist = await _unitOfWork.GetRepo<Member>().AnyAsync(m => m.Email == member.Email, ct);

            // Check if the phone number already exists in the database
            var PhoneExist = await _unitOfWork.GetRepo<Member>().AnyAsync(m => m.Phone == member.Phone, ct);

            if (EmailExist || PhoneExist)
            {
                // If either the email or phone number already exists, return false
                return false;
            }

            // map from createmembervm to member

            var newMember = _mapper.Map<Member>(member);
            
            _unitOfWork.GetRepo<Member>().AddAsync(newMember);
            var result = await _unitOfWork.Completed(ct);
            return result > 0;
        }

        public async Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepo<Member>().GetByIDAsync(memberId, ct);
            if (member is null)
                return false;
            var bookings = await _unitOfWork.GetRepo<Booking>().AnyAsync(b => b.memberId == memberId && b.session.StartDate > DateTime.Now, ct: ct);
            if (bookings)
                return false;

            _unitOfWork.GetRepo<Member>().DeleteAsync(member);
            var result = await _unitOfWork.Completed(ct);
            return result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepo<Member>().GetAllAsync(ct: ct);

            if (!members.Any()) return [];

            List<MemberViewModel> memberViewModels = new List<MemberViewModel>();
            foreach (var item in members)
            {
                var memberViewModel = _mapper.Map<MemberViewModel>(item);

                memberViewModels.Add(memberViewModel);
            }
            return memberViewModels;
        }

        public async Task<MemberViewModel?> GetMemberByIdAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepo<Member>().GetByIDAsync(id, ct);

            if (member == null) return null;

            var memberViewModel = _mapper.Map<MemberViewModel>(member);
            // if there is active membership for the member, get the plan name and the start and end date of the membership
            var membership = await _unitOfWork.GetRepo<MemberShip>().FirstOrDefaultAsync(m => m.memberId == id && m.EndDate > DateTime.Now, ct: ct);

            if (membership != null)
            {
                var plan = await _unitOfWork.GetRepo<Plan>().GetByIDAsync(membership.planId, ct);
                memberViewModel.PlanName = plan?.Name;
                memberViewModel.MemberShipStartDate = membership?.CreatedAt.ToString();
                memberViewModel.MemberShipEndDate = membership?.EndDate.ToString();
            }
            return memberViewModel;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var healthRecord = await _unitOfWork.GetRepo<HealthRecord>().FirstOrDefaultAsync(h => h.memberId == memberId, ct: ct);

            if (healthRecord == null)
                return null;
            else

                return _mapper.Map<HealthRecordViewModel>(healthRecord);


        }

        public async Task<MemberToUpdateVM?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepo<Member>().GetByIDAsync(memberId, ct: ct);

            if (member == null)
                return null;
            else
                return _mapper.Map<MemberToUpdateVM>(member);
        }

        public async Task<bool> UpdateMemberAsync(int memberId, MemberToUpdateVM model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepo<Member>().GetByIDAsync(memberId, ct);

            var EmailExist = await _unitOfWork.GetRepo<Member>().AnyAsync(m => m.Email == model.Email && m.Id != memberId, ct);
            var PhoneExist = await _unitOfWork.GetRepo<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != memberId, ct);

            if (EmailExist || PhoneExist)
                return false;

            _mapper.Map<Member>(model);
            member.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepo<Member>().UpdateAsync(member);
            var result = await _unitOfWork.Completed(ct);
            return result > 0;

        }
    }
}
