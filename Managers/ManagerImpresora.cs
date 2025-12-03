// TODO: Reimplementar impresión para MAUI
using System;
using System.Diagnostics;

namespace AsadorMoron.Managers
{
    public class ManagerImpresora
    {
        public static void ImprimirTicket(string codigo, string nombreImpresora, int alturaLinea, int veces, TipoImpresion tipo, int numeroImpresora = 1)
        {
            // TODO: Implementar impresión térmica para MAUI
            Debug.WriteLine($"[PRINT STUB] Imprimir ticket: {nombreImpresora}, tipo: {tipo}, veces: {veces}");
        }

        public enum TipoImpresion
        {
            Normal,
            Mesa,
            Bebida,
            Comida,
            Cuenta,
            Comanda
        }
    }
}
