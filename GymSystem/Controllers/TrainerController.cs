using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.ViewModels.TrainerViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymSystem.PL.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainerController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        // Get :: Trainer/Index => List of all trainers

        public async Task<IActionResult> Index(CancellationToken ct = default)
            => View(await _trainerService.GetAllTrainersAsync(ct));



        // Get :: Trainer/Details/{id} => Details of a specific trainer
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerService.GetTrainerDetailsAsync(id, ct);

            if (trainer == null)
            {
                TempData["Error"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // Get :: Trainer/Create => Show form to create a new trainer
        [HttpGet]
        public IActionResult Create()
            => View();
        // Post :: Trainer/Create => Handle form submission to create a new trainer
        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerVM model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors in the form.";
                return View(model);

            }

            var result = await _trainerService.CreateTrainerAsync(model, ct);

            if (result)
            {
                TempData["Success"] = "Trainer created successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = "Failed to create trainer. Please try again.";
                return View(model);

            }
        }

        // Get :: Trainer/Edit/{id} => Show form to edit a trainer
        [HttpGet]

        public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerService.GetTrainerToUpdateAsync(id, ct);

            if (trainer == null)
            {
                TempData["Error"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // Post :: Trainer/Edit/{id} => Handle form submission to update an old trainer
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TrainerToUpdateVM model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _trainerService.UpdateTrainerDetailsAsync(id, model, ct);
            if (result)
            {
                TempData["SuccessMessage"] = "Trainer updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Trainer Not Updated";
            return View(model);
        }


        // Get :: Trainer/Delete/{id} => show form to Delete Trainer
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerDetailsAsync(id, ct);
            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // Post :: Member/Delete/{id} => Handle form submission to delete a trainer
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _trainerService.RemoveTrainerAsync(id, ct);

            if (result)
                TempData["Success"] = "Trainer deleted successfully.";
            else
                TempData["Error"] = "Failed to delete trainer.";
            return RedirectToAction(nameof(Index));

        }
    }
}
