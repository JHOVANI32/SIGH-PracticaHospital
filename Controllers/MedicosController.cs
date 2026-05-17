using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGH_PracticaHospital.Data;
using SIGH_PracticaHospital.Models;

namespace SIGH_PracticaHospital.Controllers
{
    public class MedicosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MedicosController> _logger;

        public MedicosController(ApplicationDbContext context, ILogger<MedicosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Medicos
        public async Task<IActionResult> Index()
        {
            var medicos = await _context.Medicos
                .Include(m => m.Especialidad)
                .ToListAsync();
            return View(medicos);
        }

        // GET: Medicos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medico = await _context.Medicos
                .Include(m => m.Especialidad)
                .Include(m => m.Citas)
                .Include(m => m.Diagnosticos)
                .FirstOrDefaultAsync(m => m.MedicoId == id);
            
            if (medico == null)
            {
                return NotFound();
            }

            return View(medico);
        }

        // GET: Medicos/Create
        public IActionResult Create()
        {
            ViewData["EspecialidadId"] = new SelectList(_context.Especialidades, "EspecialidadId", "Nombre");
            return View();
        }

        // POST: Medicos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicoId,Nombre,Apellido,CedulaProfesional,EspecialidadId,Telefono,CorreoElectronico,NumeroConsultorio,Activo")] Medico medico)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(medico);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating medico");
                    ModelState.AddModelError("", "Error al crear el médico. Por favor intente de nuevo.");
                }
            }
            ViewData["EspecialidadId"] = new SelectList(_context.Especialidades, "EspecialidadId", "Nombre", medico.EspecialidadId);
            return View(medico);
        }

        // GET: Medicos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medico = await _context.Medicos.FindAsync(id);
            if (medico == null)
            {
                return NotFound();
            }
            ViewData["EspecialidadId"] = new SelectList(_context.Especialidades, "EspecialidadId", "Nombre", medico.EspecialidadId);
            return View(medico);
        }

        // POST: Medicos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MedicoId,Nombre,Apellido,CedulaProfesional,EspecialidadId,Telefono,CorreoElectronico,NumeroConsultorio,FechaRegistro,Activo")] Medico medico)
        {
            if (id != medico.MedicoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medico);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error updating medico");
                    if (!MedicoExists(medico.MedicoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["EspecialidadId"] = new SelectList(_context.Especialidades, "EspecialidadId", "Nombre", medico.EspecialidadId);
            return View(medico);
        }

        private bool MedicoExists(int id)
        {
            return _context.Medicos.Any(e => e.MedicoId == id);
        }
    }
}
