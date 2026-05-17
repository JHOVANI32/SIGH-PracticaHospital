using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGH_PracticaHospital.Data;
using SIGH_PracticaHospital.Models;

namespace SIGH_PracticaHospital.Controllers
{
    public class PacientesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PacientesController> _logger;

        public PacientesController(ApplicationDbContext context, ILogger<PacientesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Pacientes
        public async Task<IActionResult> Index(string searchString, string genero, string estado)
        {
            // Comienza con todos los pacientes
            var pacientes = from p in _context.Pacientes select p;

            // Filtro de búsqueda por nombre o cédula
            if (!string.IsNullOrEmpty(searchString))
            {
                pacientes = pacientes.Where(p => 
                    p.Nombre.Contains(searchString) || 
                    p.Apellido.Contains(searchString) || 
                    p.Cedula.Contains(searchString));
            }

            // Filtro por género
            if (!string.IsNullOrEmpty(genero) && genero != "")
            {
                pacientes = pacientes.Where(p => p.Genero == genero);
            }

            // Filtro por estado
            if (!string.IsNullOrEmpty(estado) && estado != "")
            {
                if (estado == "Activo")
                {
                    pacientes = pacientes.Where(p => p.Activo);
                }
                else if (estado == "Inactivo")
                {
                    pacientes = pacientes.Where(p => !p.Activo);
                }
            }

            // Ordenar por nombre
            var pacientesOrdenados = await pacientes.OrderBy(p => p.Nombre).ToListAsync();

            // Pasar valores de filtro a la vista
            ViewBag.SearchString = searchString;
            ViewBag.Genero = genero;
            ViewBag.Estado = estado;

            return View(pacientesOrdenados);
        }

        // GET: Pacientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes
                .Include(p => p.Citas)
                .Include(p => p.Diagnosticos)
                .FirstOrDefaultAsync(m => m.PacienteId == id);
            
            if (paciente == null)
            {
                return NotFound();
            }

            return View(paciente);
        }

        // GET: Pacientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pacientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PacienteId,Nombre,Apellido,Cedula,FechaNacimiento,Genero,GrupoSanguineo,Direccion,Telefono,CorreoElectronico,Activo")] Paciente paciente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validar que la cédula sea única
                    var cedulaExistente = await _context.Pacientes
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Cedula == paciente.Cedula);

                    if (cedulaExistente != null)
                    {
                        ModelState.AddModelError("Cedula", "La cédula ya está registrada en el sistema.");
                        return View(paciente);
                    }

                    _context.Add(paciente);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Paciente creado: {paciente.Nombre} {paciente.Apellido} (Cédula: {paciente.Cedula})");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error en base de datos al crear paciente");
                    ModelState.AddModelError("", "Error al crear el paciente. Es posible que la cédula ya esté registrada.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating paciente");
                    ModelState.AddModelError("", "Error al crear el paciente. Por favor intente de nuevo.");
                }
            }
            return View(paciente);
        }

        // GET: Pacientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }
            return View(paciente);
        }

        // POST: Pacientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PacienteId,Nombre,Apellido,Cedula,FechaNacimiento,Genero,GrupoSanguineo,Direccion,Telefono,CorreoElectronico,FechaRegistro,Activo")] Paciente paciente)
        {
            if (id != paciente.PacienteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Validar que la cédula sea única (excepto la del paciente actual)
                    var cedulaExistente = await _context.Pacientes
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Cedula == paciente.Cedula && p.PacienteId != id);

                    if (cedulaExistente != null)
                    {
                        ModelState.AddModelError("Cedula", "La cédula ya está registrada en el sistema.");
                        return View(paciente);
                    }

                    _context.Update(paciente);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Paciente actualizado: {paciente.Nombre} {paciente.Apellido} (ID: {id})");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error updating paciente");
                    if (!PacienteExists(paciente.PacienteId))
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
                    _logger.LogError(ex, "Error updating paciente");
                    ModelState.AddModelError("", "Error al actualizar el paciente. Por favor intente de nuevo.");
                }
            }
            return View(paciente);
        }

        private bool PacienteExists(int id)
        {
            return _context.Pacientes.Any(e => e.PacienteId == id);
        }
    }
}
