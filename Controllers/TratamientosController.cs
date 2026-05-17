using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGH_PracticaHospital.Data;
using SIGH_PracticaHospital.Models;

namespace SIGH_PracticaHospital.Controllers
{
    public class TratamientosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TratamientosController> _logger;

        public TratamientosController(ApplicationDbContext context, ILogger<TratamientosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Tratamientos
        public async Task<IActionResult> Index()
        {
            var tratamientos = await _context.Tratamientos
                .Include(t => t.Diagnostico)
                .ThenInclude(d => d.Paciente)
                .OrderByDescending(t => t.FechaCreacion)
                .ToListAsync();
            return View(tratamientos);
        }

        // GET: Tratamientos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tratamiento = await _context.Tratamientos
                .Include(t => t.Diagnostico)
                .ThenInclude(d => d.Paciente)
                .FirstOrDefaultAsync(m => m.TratamientoId == id);
            
            if (tratamiento == null)
            {
                return NotFound();
            }

            return View(tratamiento);
        }

        // GET: Tratamientos/Create
        public IActionResult Create()
        {
            ViewData["DiagnosticoId"] = new SelectList(
                _context.Diagnosticos
                    .Include(d => d.Paciente)
                    .Select(d => new { d.DiagnosticoId, Nombre = d.Descripcion + " - " + d.Paciente.Nombre })
                    .AsEnumerable(),
                "DiagnosticoId", "Nombre"
            );
            return View();
        }

        // POST: Tratamientos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TratamientoId,DiagnosticoId,Nombre,Descripcion,FechaInicio,FechaFin,Medicamentos,Recomendaciones,Estado,Observaciones,Activo")] Tratamiento tratamiento)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(tratamiento);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating tratamiento");
                    ModelState.AddModelError("", "Error al crear el tratamiento. Por favor intente de nuevo.");
                }
            }
            ViewData["DiagnosticoId"] = new SelectList(
                _context.Diagnosticos
                    .Include(d => d.Paciente)
                    .Select(d => new { d.DiagnosticoId, Nombre = d.Descripcion + " - " + d.Paciente.Nombre })
                    .AsEnumerable(),
                "DiagnosticoId", "Nombre", tratamiento.DiagnosticoId
            );
            return View(tratamiento);
        }

        // GET: Tratamientos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tratamiento = await _context.Tratamientos.FindAsync(id);
            if (tratamiento == null)
            {
                return NotFound();
            }
            ViewData["DiagnosticoId"] = new SelectList(
                _context.Diagnosticos
                    .Include(d => d.Paciente)
                    .Select(d => new { d.DiagnosticoId, Nombre = d.Descripcion + " - " + d.Paciente.Nombre })
                    .AsEnumerable(),
                "DiagnosticoId", "Nombre", tratamiento.DiagnosticoId
            );
            return View(tratamiento);
        }

        // POST: Tratamientos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TratamientoId,DiagnosticoId,Nombre,Descripcion,FechaInicio,FechaFin,Medicamentos,Recomendaciones,Estado,Observaciones,FechaCreacion,Activo")] Tratamiento tratamiento)
        {
            if (id != tratamiento.TratamientoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tratamiento);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error updating tratamiento");
                    if (!TratamientoExists(tratamiento.TratamientoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["DiagnosticoId"] = new SelectList(
                _context.Diagnosticos
                    .Include(d => d.Paciente)
                    .Select(d => new { d.DiagnosticoId, Nombre = d.Descripcion + " - " + d.Paciente.Nombre })
                    .AsEnumerable(),
                "DiagnosticoId", "Nombre", tratamiento.DiagnosticoId
            );
            return View(tratamiento);
        }

        private bool TratamientoExists(int id)
        {
            return _context.Tratamientos.Any(e => e.TratamientoId == id);
        }
    }
}
