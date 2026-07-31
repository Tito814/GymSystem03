using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.MemberViewModel;
using Microsoft.AspNetCore.Mvc;

namespace GymSystem.PL.Controllers
{
    public class MemberController : Controller
    {

        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        // Get :: Member/Index => List of all members
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var members = await _memberService.GetAllMembersAsync(ct);
            return View(members);
        }

        // Get :: Member/Details/{id} => Details of a specific member
        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var member = await _memberService.GetMemberByIdAsync(id, ct);
            if (member == null)
            {
                TempData["Error"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        // Get :: Member/HealthRecord/{id} => Details of a specific member's health record
        public async Task<IActionResult> HealthRecord(int id, CancellationToken ct)
        {
            var healthRecord = await _memberService.GetMemberHealthRecordAsync(id, ct);

            if (healthRecord is null)
            {
                TempData["Error"] = "Health record not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(healthRecord);
        }
        // Add

        // Get :: Member/Create => Form to create a new member
        [HttpGet]
        public IActionResult Create()
            => View();


        // Post :: Member/Create/{member} => Handle form submission to create a new member
        // CreateMember
        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberVM member, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return View(nameof(Create), member);
            var result = await _memberService.CreateMemberAsync(member, ct);
            if (result)
                TempData["Success"] = "Member created successfully.";
            else
                TempData["Error"] = "Failed to create member.";

            return RedirectToAction(nameof(Index));
        }

        // Update

        // Get :: Member/Edit/{id} => Form to edit an existing member
        [HttpGet]
        public async Task<IActionResult> EditMember([FromRoute] int id, CancellationToken ct = default)
        {
            var memberToUpdate = await _memberService.GetMemberToUpdateAsync(id, ct);

            if (memberToUpdate == null)
            {
                TempData["Error"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(memberToUpdate);
        }


        // Post :: Member/Edit/{member} => Handle form submission to update an existing member
        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute] int id, MemberToUpdateVM member, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return View(nameof(EditMember), member);
            var result = await _memberService.UpdateMemberAsync(id, member, ct);
            if (result)
                TempData["Success"] = "Member updated successfully.";
            else
                TempData["Error"] = "Failed to update member.";
            return RedirectToAction(nameof(Index));
        }

        // Delete
        // Get :: Member/Delete/{id} => Confirmation page to delete a member
        [HttpGet]
        public async Task<IActionResult> DeleteMember([FromRoute] int id, CancellationToken ct = default)
        {
            var memberToDelete = await _memberService.GetMemberByIdAsync(id, ct);

            if (memberToDelete == null)
            {
                TempData["Error"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }


        // Post :: Member/Delete/{id} => Handle form submission to delete a member
        [HttpPost]
        public async Task<IActionResult> DeleteMemberConfirmed([FromRoute] int id, CancellationToken ct = default)
        {
            var result = await _memberService.DeleteMemberAsync(id, ct);
            if (result)
                TempData["Success"] = "Member deleted successfully.";
            else
                TempData["Error"] = "Failed to delete member. Ensure there are no future bookings for this member.";
            return RedirectToAction(nameof(Index));
        }
    }
}
