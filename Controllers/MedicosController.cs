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
        public async Task<IActionResult> Index(string searchString, int? especialidadId, string estado)
        {
            // Comienza con todos los médicos incluyendo especialidad
            var medicos = _context.Medicos.Include(m => m.Especialidad).AsQueryable();

            // Filtro de búsqueda por nombre o cédula profesional
            if (!string.IsNullOrEmpty(searchString))
            {
                medicos = medicos.Where(m => 
                    m.Nombre.Contains(searchString) || 
                    m.Apellido.Contains(searchString) || 
                    m.CedulaProfesional.Contains(searchString));
            }

            // Filtro por especialidad
            if (especialidadId.HasValue && especialidadId.Value > 0)
            {
                medicos = medicos.Where(m => m.EspecialidadId == especialidadId.Value);
            }

            // Filtro por estado
            if (!string.IsNullOrEmpty(estado) && estado != "")
            {
                if (estado == "Activo")
                {
                    medicos = medicos.Where(m => m.Activo);
                }
                else if (estado == "Inactivo")
                {
                    medicos = medicos.Where(m => !m.Activo);
                }
            }

            // Ordenar por nombre
            var medicosOrdenados = await medicos.OrderBy(m => m.Nombre).ToListAsync();

            // Obtener lista de especialidades para dropdown
            var especialidades = await _context.Especialidades
                .Where(e => e.Activa)
                .OrderBy(e => e.Nombre)
                .ToListAsync();

            // Pasar valores de filtro a la vista
            ViewBag.SearchString = searchString;
            ViewBag.EspecialidadId = especialidadId;
            ViewBag.Estado = estado;
            ViewBag.Especialidades = especialidades;

            return View(medicosOrdenados);
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
            ViewData["EspecialidadId"] = new SelectList(
                _context.Especialidades.Where(e => e.Activa).OrderBy(e => e.Nombre),
                "EspecialidadId", "Nombre");
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
                    // Validar que la cédula profesional sea única
                    var cedulaExistente = await _context.Medicos
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m => m.CedulaProfesional == medico.CedulaProfesional);

                    if (cedulaExistente != null)
                    {
                        ModelState.AddModelError("CedulaProfesional", "La cédula profesional ya está registrada en el sistema.");
                        ViewData["EspecialidadId"] = new SelectList(
                            _context.Especialidades.Where(e => e.Activa),
                            "EspecialidadId", "Nombre", medico.EspecialidadId);
                        return View(medico);
                    }

                    _context.Add(medico);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Médico creado: {medico.Nombre} {medico.Apellido} (Cédula: {medico.CedulaProfesional})");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error en base de datos al crear médico");
                    ModelState.AddModelError("", "Error al crear el médico. Es posible que la cédula profesional ya esté registrada.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating medico");
                    ModelState.AddModelError("", "Error al crear el médico. Por favor intente de nuevo.");
                }
            }
            ViewData["EspecialidadId"] = new SelectList(
                _context.Especialidades.Where(e => e.Activa),
                "EspecialidadId", "Nombre", medico.EspecialidadId);
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
            ViewData["EspecialidadId"] = new SelectList(
                _context.Especialidades.Where(e => e.Activa),
                "EspecialidadId", "Nombre", medico.EspecialidadId);
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
                    // Validar que la cédula profesional sea única (excepto la del médico actual)
                    var cedulaExistente = await _context.Medicos
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m => m.CedulaProfesional == medico.CedulaProfesional && m.MedicoId != id);

                    if (cedulaExistente != null)
                    {
                        ModelState.AddModelError("CedulaProfesional", "La cédula profesional ya está registrada en el sistema.");
                        ViewData["EspecialidadId"] = new SelectList(
                            _context.Especialidades.Where(e => e.Activa),
                            "EspecialidadId", "Nombre", medico.EspecialidadId);
                        return View(medico);
                    }

                    _context.Update(medico);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Médico actualizado: {medico.Nombre} {medico.Apellido} (ID: {id})");
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
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating medico");
                    ModelState.AddModelError("", "Error al actualizar el médico. Por favor intente de nuevo.");
                }
            }
            ViewData["EspecialidadId"] = new SelectList(
                _context.Especialidades.Where(e => e.Activa),
                "EspecialidadId", "Nombre", medico.EspecialidadId);
            return View(medico);
        }

        private bool MedicoExists(int id)
        {
            return _context.Medicos.Any(e => e.MedicoId == id);
        }
    }
}
