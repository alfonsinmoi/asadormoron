using System;
using System.Diagnostics;
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
        public static void ImprimirTicket(string codigo, string nombreImpresora, int alturaLinea, int veces, TipoImpresion tipo, int numeroImpresora = 1)
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
                            printer.ImprimirTicketPedido(alturaLinea);
                            break;
                        case TipoImpresion.Comanda:
                            printer.ImprimirTicketComanda(numeroImpresora);
                            break;
                        default:
                            printer.ImprimirTicketPedido(alturaLinea);
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
