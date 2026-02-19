using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AsadorMoron.Print;

namespace AsadorMoron.Managers
{
    public enum TipoImpresion
    {
        Normal,
        Mesa,
        Bebida,
        Comida,
        Cuenta,
        Comanda
    }

    public static class ManagerImpresora
    {
        public static async Task ImprimirTicketAsync(string codigo, string nombreImpresora, int alturaLinea, int veces, TipoImpresion tipo, int numeroImpresora = 1)
        {
            try
            {
#if WINDOWS
                if (string.IsNullOrEmpty(nombreImpresora))
                {
                    Debug.WriteLine("[ManagerImpresora] Nombre de impresora vacío");
                    return;
                }

                for (int i = 0; i < veces; i++)
                {
                    var printer = new Printer(nombreImpresora, codigo, "ISO-8859-1");

                    switch (tipo)
                    {
                        case TipoImpresion.Normal:
                        case TipoImpresion.Mesa:
                            await printer.ImprimirTicketPedidoAsync(alturaLinea);
                            break;
                        case TipoImpresion.Comanda:
                            await printer.ImprimirTicketComandaAsync(numeroImpresora);
                            break;
                        default:
                            await printer.ImprimirTicketPedidoAsync(alturaLinea);
                            break;
                    }

                    printer.PrintDocument();
                }

                Debug.WriteLine($"[ManagerImpresora] Ticket impreso: {codigo}, impresora: {nombreImpresora}, tipo: {tipo}, veces: {veces}");
#else
                Debug.WriteLine($"[ManagerImpresora] Impresión solo disponible en Windows. Ticket: {codigo}");
#endif
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ManagerImpresora] Error al imprimir: {ex.Message}");
            }
        }
    }
}
