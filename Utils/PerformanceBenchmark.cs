using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AsadorMoron.Utils
{
    /// <summary>
    /// Clase para medir y comparar el rendimiento de operaciones.
    /// Usar antes y después de las optimizaciones para ver mejoras.
    /// </summary>
    public static class PerformanceBenchmark
    {
        private static readonly Dictionary<string, List<long>> _measurements = new();
        private static readonly object _lock = new();

        /// <summary>
        /// Mide el tiempo de ejecución de una acción síncrona
        /// </summary>
        public static long Measure(string operationName, Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();

            RecordMeasurement(operationName, sw.ElapsedMilliseconds);
            Debug.WriteLine($"[BENCHMARK] {operationName}: {sw.ElapsedMilliseconds}ms");

            return sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// Mide el tiempo de ejecución de una operación async
        /// </summary>
        public static async Task<long> MeasureAsync(string operationName, Func<Task> asyncAction)
        {
            var sw = Stopwatch.StartNew();
            await asyncAction();
            sw.Stop();

            RecordMeasurement(operationName, sw.ElapsedMilliseconds);
            Debug.WriteLine($"[BENCHMARK] {operationName}: {sw.ElapsedMilliseconds}ms");

            return sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// Mide el tiempo de ejecución de una operación async con resultado
        /// </summary>
        public static async Task<(T Result, long ElapsedMs)> MeasureAsync<T>(string operationName, Func<Task<T>> asyncAction)
        {
            var sw = Stopwatch.StartNew();
            var result = await asyncAction();
            sw.Stop();

            RecordMeasurement(operationName, sw.ElapsedMilliseconds);
            Debug.WriteLine($"[BENCHMARK] {operationName}: {sw.ElapsedMilliseconds}ms");

            return (result, sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Inicia un cronómetro para mediciones manuales
        /// </summary>
        public static Stopwatch StartTimer() => Stopwatch.StartNew();

        /// <summary>
        /// Detiene y registra una medición manual
        /// </summary>
        public static long StopAndRecord(Stopwatch sw, string operationName)
        {
            sw.Stop();
            RecordMeasurement(operationName, sw.ElapsedMilliseconds);
            Debug.WriteLine($"[BENCHMARK] {operationName}: {sw.ElapsedMilliseconds}ms");
            return sw.ElapsedMilliseconds;
        }

        private static void RecordMeasurement(string operationName, long elapsedMs)
        {
            lock (_lock)
            {
                if (!_measurements.ContainsKey(operationName))
                    _measurements[operationName] = new List<long>();

                _measurements[operationName].Add(elapsedMs);
            }
        }

        /// <summary>
        /// Obtiene el promedio de mediciones para una operación
        /// </summary>
        public static double GetAverage(string operationName)
        {
            lock (_lock)
            {
                if (!_measurements.ContainsKey(operationName) || _measurements[operationName].Count == 0)
                    return 0;

                double sum = 0;
                foreach (var m in _measurements[operationName])
                    sum += m;

                return sum / _measurements[operationName].Count;
            }
        }

        /// <summary>
        /// Obtiene el reporte completo de todas las mediciones
        /// </summary>
        public static string GetReport()
        {
            lock (_lock)
            {
                var report = new System.Text.StringBuilder();
                report.AppendLine("╔══════════════════════════════════════════════════════════════╗");
                report.AppendLine("║           REPORTE DE RENDIMIENTO - BENCHMARK                 ║");
                report.AppendLine("╠══════════════════════════════════════════════════════════════╣");

                foreach (var kvp in _measurements)
                {
                    var measurements = kvp.Value;
                    if (measurements.Count == 0) continue;

                    double sum = 0, min = long.MaxValue, max = long.MinValue;
                    foreach (var m in measurements)
                    {
                        sum += m;
                        if (m < min) min = m;
                        if (m > max) max = m;
                    }
                    double avg = sum / measurements.Count;

                    report.AppendLine($"║ {kvp.Key,-40}                      ║");
                    report.AppendLine($"║   Promedio: {avg,8:F2}ms | Min: {min,6}ms | Max: {max,6}ms      ║");
                    report.AppendLine($"║   Mediciones: {measurements.Count,-5}                                     ║");
                    report.AppendLine("╠──────────────────────────────────────────────────────────────╣");
                }

                report.AppendLine("╚══════════════════════════════════════════════════════════════╝");

                var reportStr = report.ToString();
                Debug.WriteLine(reportStr);
                return reportStr;
            }
        }

        /// <summary>
        /// Limpia todas las mediciones
        /// </summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _measurements.Clear();
            }
        }

        /// <summary>
        /// Compara dos operaciones y retorna el porcentaje de mejora
        /// </summary>
        public static string Compare(string beforeOperation, string afterOperation)
        {
            var avgBefore = GetAverage(beforeOperation);
            var avgAfter = GetAverage(afterOperation);

            if (avgBefore == 0) return "No hay datos de la operación 'antes'";
            if (avgAfter == 0) return "No hay datos de la operación 'después'";

            var improvement = ((avgBefore - avgAfter) / avgBefore) * 100;
            var speedup = avgBefore / avgAfter;

            return $"Mejora: {improvement:F1}% más rápido ({speedup:F2}x) - Antes: {avgBefore:F0}ms, Después: {avgAfter:F0}ms";
        }
    }
}
