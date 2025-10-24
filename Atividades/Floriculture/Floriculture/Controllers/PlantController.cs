using Floriculture.Models;
using Floriculture.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Floriculture.Controllers
{
    public class PlantController : Controller
    {
        private readonly ILogger<PlantController> _logger;
        private readonly IPlantRepository _plantRepository;

        public PlantController(
            ILogger<PlantController> logger,
            IPlantRepository plantRepository
        )
        {
            _logger = logger;
            _plantRepository = plantRepository;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var plants = await _plantRepository.GetAll();

            return View(plants);
        }
        #endregion

        #region Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Plant plant)
        {
            if (ModelState.IsValid)
            {
                plant.SensorEvent = DateTime.Now; 
                await _plantRepository.Create(plant);

                return RedirectToAction("Index");
            }

            return View(plant);
        }
        #endregion

        #region Update
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var plant = await _plantRepository.GetById(id.Value);
            if (plant == null)
                return NotFound();

            return View(plant);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int routeId, Plant plant)
        {
            if (!ModelState.IsValid)
                return View(plant);

            var existingPlant = await _plantRepository.GetById(routeId);
            if (existingPlant == null)
                return NotFound();

            existingPlant.PlantName = plant.PlantName;
            existingPlant.SensorValue = plant.SensorValue;

            existingPlant.SensorEvent = DateTime.Now;

            await _plantRepository.Update(existingPlant);

            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var plant = await _plantRepository.GetById(id);
            if (plant == null)
                return NotFound();

            return View(plant);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plant = await _plantRepository.GetById(id);
            if (plant == null)
                return NotFound();

            await _plantRepository.Delete(plant);
            return RedirectToAction("Index");
        }
        #endregion

        #region Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorModel = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
            return View(errorModel);
        }
        #endregion
    }
}
