using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsadorMoron.Utils
{
    /// <summary>
    /// Utilidad para implementar debounce en operaciones.
    /// Evita ejecutar operaciones repetitivas demasiado rápido (ej: búsquedas mientras el usuario escribe).
    /// </summary>
    public sealed class Debouncer : IDisposable
    {
        private CancellationTokenSource _cts;
        private readonly object _lock = new();
        private bool _disposed;

        /// <summary>
        /// Ejecuta una acción después de un delay, cancelando cualquier ejecución pendiente anterior.
        /// </summary>
        /// <param name="action">Acción a ejecutar</param>
        /// <param name="delayMs">Delay en milisegundos (default: 300ms)</param>
        public async Task DebounceAsync(Func<Task> action, int delayMs = 300)
        {
            CancellationToken token;

            lock (_lock)
            {
                // Cancelar cualquier operación pendiente
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = new CancellationTokenSource();
                token = _cts.Token;
            }

            try
            {
                await Task.Delay(delayMs, token);

                if (!token.IsCancellationRequested)
                {
                    await action();
                }
            }
            catch (TaskCanceledException)
            {
                // Esperado cuando se cancela por una nueva llamada
            }
        }

        /// <summary>
        /// Ejecuta una acción síncrona después de un delay, cancelando cualquier ejecución pendiente anterior.
        /// </summary>
        /// <param name="action">Acción a ejecutar</param>
        /// <param name="delayMs">Delay en milisegundos (default: 300ms)</param>
        public async Task DebounceAsync(Action action, int delayMs = 300)
        {
            await DebounceAsync(() =>
            {
                action();
                return Task.CompletedTask;
            }, delayMs);
        }

        /// <summary>
        /// Cancela cualquier operación pendiente
        /// </summary>
        public void Cancel()
        {
            lock (_lock)
            {
                _cts?.Cancel();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            lock (_lock)
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Utilidad para limitar la frecuencia de ejecución (throttle).
    /// A diferencia del debounce, ejecuta inmediatamente la primera vez y luego ignora llamadas durante el período.
    /// </summary>
    public sealed class Throttler
    {
        private DateTime _lastExecution = DateTime.MinValue;
        private readonly object _lock = new();

        /// <summary>
        /// Ejecuta la acción solo si ha pasado suficiente tiempo desde la última ejecución.
        /// </summary>
        /// <param name="action">Acción a ejecutar</param>
        /// <param name="intervalMs">Intervalo mínimo entre ejecuciones en milisegundos</param>
        /// <returns>True si se ejecutó, False si fue ignorado por throttle</returns>
        public bool TryExecute(Action action, int intervalMs = 300)
        {
            lock (_lock)
            {
                var now = DateTime.UtcNow;
                if ((now - _lastExecution).TotalMilliseconds >= intervalMs)
                {
                    _lastExecution = now;
                    action();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Ejecuta la acción async solo si ha pasado suficiente tiempo desde la última ejecución.
        /// </summary>
        public async Task<bool> TryExecuteAsync(Func<Task> action, int intervalMs = 300)
        {
            bool shouldExecute;

            lock (_lock)
            {
                var now = DateTime.UtcNow;
                shouldExecute = (now - _lastExecution).TotalMilliseconds >= intervalMs;
                if (shouldExecute)
                    _lastExecution = now;
            }

            if (shouldExecute)
            {
                await action();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Resetea el throttle permitiendo la siguiente ejecución inmediatamente
        /// </summary>
        public void Reset()
        {
            lock (_lock)
            {
                _lastExecution = DateTime.MinValue;
            }
        }
    }
}
