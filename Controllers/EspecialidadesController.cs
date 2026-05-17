using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGH_PracticaHospital.Data;
using SIGH_PracticaHospital.Models;

namespace SIGH_PracticaHospital.Controllers
{
    public class EspecialidadesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EspecialidadesController> _logger;

        public EspecialidadesController(ApplicationDbContext context, ILogger<EspecialidadesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Especialidades
        public async Task<IActionResult> Index()
        {
            return View(await _context.Especialidades.ToListAsync());
        }

        // GET: Especialidades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var especialidad = await _context.Especialidades
                .Include(e => e.Medicos)
                .FirstOrDefaultAsync(m => m.EspecialidadId == id);
            
            if (especialidad == null)
            {
                return NotFound();
            }

            return View(especialidad);
        }

        // GET: Especialidades/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Especialidades/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EspecialidadId,Nombre,Descripcion,Activa")] Especialidad especialidad)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(especialidad);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating especialidad");
                    ModelState.AddModelError("", "Error al crear la especialidad. Por favor intente de nuevo.");
                }
            }
            return View(especialidad);
        }

        // GET: Especialidades/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var especialidad = await _context.Especialidades.FindAsync(id);
            if (especialidad == null)
            {
                return NotFound();
            }
            return View(especialidad);
        }

        // POST: Especialidades/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EspecialidadId,Nombre,Descripcion,FechaCreacion,Activa")] Especialidad especialidad)
        {
            if (id != especialidad.EspecialidadId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(especialidad);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error updating especialidad");
                    if (!EspecialidadExists(especialidad.EspecialidadId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(especialidad);
        }

        private bool EspecialidadExists(int id)
        {
            return _context.Especialidades.Any(e => e.EspecialidadId == id);
        }
    }
}
