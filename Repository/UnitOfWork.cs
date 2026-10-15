using System;
using System.Threading.Tasks;
using PROYECTO_REFERENCIAS.Data;
using PROYECTO_REFERENCIAS.Repository;

namespace PROYECTO_REFERENCIAS.Repository
{
    /// <summary>
    /// Unit of Work: patrón que coordina las operaciones de múltiples repositorios
    /// en una única transacción de base de datos.
    /// Permite guardar la cabecera (SolicitudReferencia) y sus detalles (DetalleReferencia)
    /// en una sola operación atómica.
    /// </summary>
    public class UnitOfWork : IDisposable
    {
        private readonly AppDbContext _context;
        private bool _disposed;

        public UnitOfWork(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Repositorios compartidos
        private IRepository<Models.Paciente>? _pacienteRepository;
        private IRepository<Models.EstablecimientoSalud>? _establecimientoRepository;
        private IRepository<Models.Especialidad>? _especialidadRepository;
        private IRepository<Models.PersonalSalud>? _personalRepository;
        private IRepository<Models.SolicitudReferencia>? _referenciaRepository;
        private IRepository<Models.DetalleReferencia>? _detalleRepository;
        private IRepository<Models.Contrareferencia>? _contrareferenciaRepository;
        private IRepository<Models.EstablecimientoEspecialidad>? _establecimientoEspecialidadRepository;

        public IRepository<Models.Paciente> PacienteRepository => _pacienteRepository ??= new EfRepository<Models.Paciente>(_context);
        public IRepository<Models.EstablecimientoSalud> EstablecimientoRepository => _establecimientoRepository ??= new EfRepository<Models.EstablecimientoSalud>(_context);
        public IRepository<Models.Especialidad> EspecialidadRepository => _especialidadRepository ??= new EfRepository<Models.Especialidad>(_context);
        public IRepository<Models.PersonalSalud> PersonalRepository => _personalRepository ??= new EfRepository<Models.PersonalSalud>(_context);
        public IRepository<Models.SolicitudReferencia> ReferenciaRepository => _referenciaRepository ??= new EfRepository<Models.SolicitudReferencia>(_context);
        public IRepository<Models.DetalleReferencia> DetalleRepository => _detalleRepository ??= new EfRepository<Models.DetalleReferencia>(_context);
        public IRepository<Models.Contrareferencia> ContrareferenciaRepository => _contrareferenciaRepository ??= new EfRepository<Models.Contrareferencia>(_context);
        public IRepository<Models.EstablecimientoEspecialidad> EstablecimientoEspecialidadRepository => _establecimientoEspecialidadRepository ??= new EfRepository<Models.EstablecimientoEspecialidad>(_context);

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    _context.Dispose();
                _disposed = true;
            }
        }
    }
}
