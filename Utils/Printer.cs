using System;
using System.Diagnostics;
using AsadorMoron.Managers;

namespace AsadorMoron.Utils
{
    /// <summary>
    /// Stub de impresora térmica - TODO: Implementar para MAUI
    /// </summary>
    public class Printer
    {
        private string _printerName;
        private string _data;
        private string _encoding;

        public Printer(string printerName, string data, string encoding = "UTF-8")
        {
            _printerName = printerName;
            _data = data;
            _encoding = encoding;
        }

        public void PrintDocument()
        {
            Debug.WriteLine($"[PRINTER STUB] PrintDocument called for printer: {_printerName}");
            // TODO: Implementar impresión real con librería compatible con MAUI
        }

        public void Append(string value)
        {
            _data += value;
        }

        public void NewLine()
        {
            _data += Environment.NewLine;
        }

        public void PartialPaperCut()
        {
            Debug.WriteLine("[PRINTER STUB] PartialPaperCut");
        }

        public void FullPaperCut()
        {
            Debug.WriteLine("[PRINTER STUB] FullPaperCut");
        }

        public void OpenDrawer()
        {
            Debug.WriteLine("[PRINTER STUB] OpenDrawer");
        }

        public void ImprimirTicketPedido(int alturaLinea)
        {
            Debug.WriteLine($"[PRINTER STUB] ImprimirTicketPedido with altura: {alturaLinea}");
            // TODO: Implementar impresión de ticket real
        }
    }
}
