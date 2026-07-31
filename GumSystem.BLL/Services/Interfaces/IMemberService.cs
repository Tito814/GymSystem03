using GymSystem.BLL.ViewModels.MemberViewModel;
using GymSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Interfaces
{
    public  interface IMemberService
    {
        // Get all members
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default);

        // Get Create Member Form
        Task<bool> CreateMemberAsync(CreateMemberVM member, CancellationToken ct = default);

        //Get Member By Id
        Task<MemberViewModel> GetMemberByIdAsync(int id, CancellationToken ct = default);

        // Get Member Health Record By Id
        Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default);

        Task<MemberToUpdateVM?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default);

        Task<bool> UpdateMemberAsync(int memberId, MemberToUpdateVM member, CancellationToken ct = default);
        
        Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct = default);
    }
}
