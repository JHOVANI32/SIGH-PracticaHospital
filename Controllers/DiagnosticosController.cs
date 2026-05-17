using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGH_PracticaHospital.Data;
using SIGH_PracticaHospital.Models;

namespace SIGH_PracticaHospital.Controllers
{
    public class DiagnosticosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DiagnosticosController> _logger;

        public DiagnosticosController(ApplicationDbContext context, ILogger<DiagnosticosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Diagnosticos
        public async Task<IActionResult> Index()
        {
            var diagnosticos = await _context.Diagnosticos
                .Include(d => d.Paciente)
                .Include(d => d.Medico)
                .OrderByDescending(d => d.FechaDiagnostico)
                .ToListAsync();
            return View(diagnosticos);
        }

        // GET: Diagnosticos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnostico = await _context.Diagnosticos
                .Include(d => d.Paciente)
                .Include(d => d.Medico)
                .FirstOrDefaultAsync(m => m.DiagnosticoId == id);
            
            if (diagnostico == null)
            {
                return NotFound();
            }

            return View(diagnostico);
        }

        // GET: Diagnosticos/Create
        public IActionResult Create()
        {
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre");
            ViewData["MedicoId"] = new SelectList(_context.Medicos.Include(m => m.Especialidad), "MedicoId", "Nombre");
            return View();
        }

        // POST: Diagnosticos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DiagnosticoId,PacienteId,MedicoId,Descripcion,CodigoCIE10,Observaciones,Severidad,Activo")] Diagnostico diagnostico)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(diagnostico);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating diagnostico");
                    ModelState.AddModelError("", "Error al crear el diagnóstico. Por favor intente de nuevo.");
                }
            }
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre", diagnostico.PacienteId);
            ViewData["MedicoId"] = new SelectList(_context.Medicos.Include(m => m.Especialidad), "MedicoId", "Nombre", diagnostico.MedicoId);
            return View(diagnostico);
        }

        // GET: Diagnosticos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diagnostico = await _context.Diagnosticos.FindAsync(id);
            if (diagnostico == null)
            {
                return NotFound();
            }
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre", diagnostico.PacienteId);
            ViewData["MedicoId"] = new SelectList(_context.Medicos.Include(m => m.Especialidad), "MedicoId", "Nombre", diagnostico.MedicoId);
            return View(diagnostico);
        }

        // POST: Diagnosticos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DiagnosticoId,PacienteId,MedicoId,Descripcion,CodigoCIE10,Observaciones,Severidad,FechaDiagnostico,Activo")] Diagnostico diagnostico)
        {
            if (id != diagnostico.DiagnosticoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diagnostico);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error updating diagnostico");
                    if (!DiagnosticoExists(diagnostico.DiagnosticoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "PacienteId", "Nombre", diagnostico.PacienteId);
            ViewData["MedicoId"] = new SelectList(_context.Medicos.Include(m => m.Especialidad), "MedicoId", "Nombre", diagnostico.MedicoId);
            return View(diagnostico);
        }

        private bool DiagnosticoExists(int id)
        {
            return _context.Diagnosticos.Any(e => e.DiagnosticoId == id);
        }
    }
}
