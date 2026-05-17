using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGH_PracticaHospital.Data;
using SIGH_PracticaHospital.Models;

namespace SIGH_PracticaHospital.Controllers
{
    public class CitasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CitasController> _logger;

        public CitasController(ApplicationDbContext context, ILogger<CitasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Citas
        public async Task<IActionResult> Index()
        {
            var citas = await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .OrderBy(c => c.FechaCita)
                .ToListAsync();
            return View(citas);
        }

        // GET: Citas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .FirstOrDefaultAsync(m => m.CitaId == id);
            
            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }

        // GET: Citas/Create
        public IActionResult Create()
        {
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre");
            ViewData["MedicoId"] = new SelectList(_context.Medicos.Include(m => m.Especialidad), "MedicoId", "Nombre");
            return View();
        }

        // POST: Citas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CitaId,PacienteId,MedicoId,FechaCita,Hora,Motivo,Estado,Observaciones")] Cita cita)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(cita);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating cita");
                    ModelState.AddModelError("", "Error al crear la cita. Por favor intente de nuevo.");
                }
            }
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre", cita.PacienteId);
            ViewData["MedicoId"] = new SelectList(_context.Medicos.Include(m => m.Especialidad), "MedicoId", "Nombre", cita.MedicoId);
            return View(cita);
        }

        // GET: Citas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
            {
                return NotFound();
            }
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre", cita.PacienteId);
            ViewData["MedicoId"] = new SelectList(_context.Medicos.Include(m => m.Especialidad), "MedicoId", "Nombre", cita.MedicoId);
            return View(cita);
        }

        // POST: Citas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CitaId,PacienteId,MedicoId,FechaCita,Hora,Motivo,Estado,Observaciones,FechaCreacion")] Cita cita)
        {
            if (id != cita.CitaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cita);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error updating cita");
                    if (!CitaExists(cita.CitaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre", cita.PacienteId);
            ViewData["MedicoId"] = new SelectList(_context.Medicos.Include(m => m.Especialidad), "MedicoId", "Nombre", cita.MedicoId);
            return View(cita);
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.CitaId == id);
        }
    }
}
