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
        public async Task<IActionResult> Index(string searchString, int? medicoId, string estado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            // Comienza con todas las citas
            var citas = _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .AsQueryable();

            // Filtro por búsqueda de paciente
            if (!string.IsNullOrEmpty(searchString))
            {
                citas = citas.Where(c => 
                    c.Paciente.Nombre.Contains(searchString) || 
                    c.Paciente.Apellido.Contains(searchString) ||
                    c.Medico.Nombre.Contains(searchString));
            }

            // Filtro por médico
            if (medicoId.HasValue && medicoId.Value > 0)
            {
                citas = citas.Where(c => c.MedicoId == medicoId.Value);
            }

            // Filtro por estado
            if (!string.IsNullOrEmpty(estado) && estado != "")
            {
                citas = citas.Where(c => c.Estado == estado);
            }

            // Filtro por rango de fechas
            if (fechaInicio.HasValue)
            {
                citas = citas.Where(c => c.FechaCita >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                var fechaFinConHora = fechaFin.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                citas = citas.Where(c => c.FechaCita <= fechaFinConHora);
            }

            var citasOrdenadas = await citas.OrderBy(c => c.FechaCita).ToListAsync();

            // Pasar valores de filtro a la vista
            ViewBag.SearchString = searchString;
            ViewBag.MedicoId = medicoId;
            ViewBag.Estado = estado;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            // Obtener lista de médicos para dropdown
            ViewBag.Medicos = await _context.Medicos
                .Where(m => m.Activo)
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            return View(citasOrdenadas);
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
            ViewData["PacienteId"] = new SelectList(
                _context.Pacientes.Where(p => p.Activo).OrderBy(p => p.Nombre),
                "PacienteId", "Nombre");
            ViewData["MedicoId"] = new SelectList(
                _context.Medicos.Where(m => m.Activo).Include(m => m.Especialidad).OrderBy(m => m.Nombre),
                "MedicoId", "Nombre");
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
                    // Validar que no haya traslapes de horarios para el médico
                    var citalError = await ValidarTraslapeHorarioMedico(cita.MedicoId, cita.FechaCita, cita.Hora, null);
                    
                    if (citalError)
                    {
                        ModelState.AddModelError("", 
                            "El médico ya tiene una cita programada en esta fecha y hora. Por favor seleccione otro horario.");
                        ViewData["PacienteId"] = new SelectList(
                            _context.Pacientes.Where(p => p.Activo),
                            "PacienteId", "Nombre", cita.PacienteId);
                        ViewData["MedicoId"] = new SelectList(
                            _context.Medicos.Where(m => m.Activo).Include(m => m.Especialidad),
                            "MedicoId", "Nombre", cita.MedicoId);
                        return View(cita);
                    }

                    _context.Add(cita);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Cita creada - Médico: {cita.MedicoId}, Paciente: {cita.PacienteId}, Fecha: {cita.FechaCita:dd/MM/yyyy HH:mm}");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating cita");
                    ModelState.AddModelError("", "Error al crear la cita. Por favor intente de nuevo.");
                }
            }
            ViewData["PacienteId"] = new SelectList(
                _context.Pacientes.Where(p => p.Activo),
                "PacienteId", "Nombre", cita.PacienteId);
            ViewData["MedicoId"] = new SelectList(
                _context.Medicos.Where(m => m.Activo).Include(m => m.Especialidad),
                "MedicoId", "Nombre", cita.MedicoId);
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
            ViewData["PacienteId"] = new SelectList(
                _context.Pacientes.Where(p => p.Activo),
                "PacienteId", "Nombre", cita.PacienteId);
            ViewData["MedicoId"] = new SelectList(
                _context.Medicos.Where(m => m.Activo).Include(m => m.Especialidad),
                "MedicoId", "Nombre", cita.MedicoId);
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
                    // Validar traslapes EXCEPTO la cita que se está editando
                    var citalError = await ValidarTraslapeHorarioMedico(cita.MedicoId, cita.FechaCita, cita.Hora, id);
                    
                    if (citalError)
                    {
                        ModelState.AddModelError("", 
                            "El médico ya tiene una cita programada en esta fecha y hora. Por favor seleccione otro horario.");
                        ViewData["PacienteId"] = new SelectList(
                            _context.Pacientes.Where(p => p.Activo),
                            "PacienteId", "Nombre", cita.PacienteId);
                        ViewData["MedicoId"] = new SelectList(
                            _context.Medicos.Where(m => m.Activo).Include(m => m.Especialidad),
                            "MedicoId", "Nombre", cita.MedicoId);
                        return View(cita);
                    }

                    _context.Update(cita);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Cita actualizada - ID: {id}, Fecha: {cita.FechaCita:dd/MM/yyyy HH:mm}");
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
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating cita");
                    ModelState.AddModelError("", "Error al actualizar la cita. Por favor intente de nuevo.");
                }
            }
            ViewData["PacienteId"] = new SelectList(
                _context.Pacientes.Where(p => p.Activo),
                "PacienteId", "Nombre", cita.PacienteId);
            ViewData["MedicoId"] = new SelectList(
                _context.Medicos.Where(m => m.Activo).Include(m => m.Especialidad),
                "MedicoId", "Nombre", cita.MedicoId);
            return View(cita);
        }

        /// <summary>
        /// Valida que no exista un traslape de horarios para un médico en una fecha específica.
        /// Comprueba si el médico ya tiene una cita en la misma fecha y hora.
        /// </summary>
        /// <param name="medicoId">ID del médico a validar</param>
        /// <param name="fechaCita">Fecha de la cita</param>
        /// <param name="horaCita">Hora de la cita</param>
        /// <param name="citaIdActual">ID de la cita que se está editando (null si es nueva)</param>
        /// <returns>true si hay traslape, false si no hay</returns>
        private async Task<bool> ValidarTraslapeHorarioMedico(int medicoId, DateTime fechaCita, TimeSpan horaCita, int? citaIdActual)
        {
            try
            {
                var fechaInicio = fechaCita.Date;
                var fechaFin = fechaCita.Date.AddDays(1).AddSeconds(-1);

                // Buscar citas del mismo médico en la misma fecha
                var citasExistentes = await _context.Citas
                    .AsNoTracking()
                    .Where(c => c.MedicoId == medicoId &&
                                c.FechaCita >= fechaInicio &&
                                c.FechaCita <= fechaFin &&
                                c.Estado != "Cancelada") // No contar citas canceladas
                    .ToListAsync();

                // Si se está editando, excluir la cita actual de la validación
                if (citaIdActual.HasValue)
                {
                    citasExistentes = citasExistentes
                        .Where(c => c.CitaId != citaIdActual.Value)
                        .ToList();
                }

                // Verificar si existe alguna cita exactamente a la misma hora
                var traslape = citasExistentes.Any(c => c.Hora == horaCita);

                if (traslape)
                {
                    _logger.LogWarning(
                        $"Intento de crear cita con traslape - Médico ID: {medicoId}, " +
                        $"Fecha: {fechaCita:dd/MM/yyyy}, Hora: {horaCita:hh\\:mm}");
                }

                return traslape;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating cita schedule");
                return false; // En caso de error, permitir crear la cita (no bloquear)
            }
        }

        /// <summary>
        /// Obtiene los horarios ocupados de un médico en una fecha específica (para validación en frontend).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ObtenerHorariosOcupados(int medicoId, DateTime fechaCita)
        {
            try
            {
                var fechaInicio = fechaCita.Date;
                var fechaFin = fechaCita.Date.AddDays(1).AddSeconds(-1);

                var horariosOcupados = await _context.Citas
                    .AsNoTracking()
                    .Where(c => c.MedicoId == medicoId &&
                                c.FechaCita >= fechaInicio &&
                                c.FechaCita <= fechaFin &&
                                c.Estado != "Cancelada")
                    .Select(c => c.Hora.ToString(@"hh\:mm"))
                    .ToListAsync();

                return Json(new { success = true, horariosOcupados = horariosOcupados });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching occupied schedules");
                return Json(new { success = false, message = "Error al obtener horarios" });
            }
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.CitaId == id);
        }
    }
}
