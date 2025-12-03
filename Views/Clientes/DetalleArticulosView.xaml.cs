using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using AsadorMoron.ViewModels.Clientes;
using AsadorMoron.Models;
using System;
using System.Linq;

namespace AsadorMoron.Views.Clientes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetalleArticulosView : Microsoft.Maui.Controls.ContentPage
    {
        DetalleArticuloViewModel vm;
        public DetalleArticulosView()
        {
            InitializeComponent();
            //this.SizeChanged += MainPage_SizeChanged;
        }
        void CheckBox_CheckedChanged(CheckBox sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                foreach (OpcionesModel o in vm.Articulo.listadoOpciones)
                {
                    try
                    {
                        if (o.id != int.Parse(sender.AutomationId))
                            o.seleccionado = false;
                        else
                            vm.OpcionSeleccionada = o;
                    }
                    catch (Exception)
                    {

                    }
                }
                if (vm.Combo && !vm.esInicio)
                {
                    if (vm.Combo1.Count(p => p.id == int.Parse(sender.AutomationId))>0)
                        foreach (ComboModel o in vm.Combo1.Where(p => p.id != int.Parse(sender.AutomationId) && p.tipo==1))
                            o.Seleccionado = false;

                    if (vm.Combo2.Count(p => p.id == int.Parse(sender.AutomationId)) > 0)
                        foreach (ComboModel o in vm.Combo2.Where(p => p.id != int.Parse(sender.AutomationId) && p.tipo == 2))
                            o.Seleccionado = false;

                    if (vm.Combo3.Count(p => p.id == int.Parse(sender.AutomationId)) > 0)
                        foreach (ComboModel o in vm.Combo3.Where(p => p.id != int.Parse(sender.AutomationId) && p.tipo == 3))
                            o.Seleccionado = false;

                    if (vm.Combo4.Count(p => p.id == int.Parse(sender.AutomationId)) > 0)
                        foreach (ComboModel o in vm.Combo4.Where(p => p.id != int.Parse(sender.AutomationId) && p.tipo == 4))
                            o.Seleccionado = false;

                    if (vm.Combo5.Count(p => p.id == int.Parse(sender.AutomationId)) > 0)
                        foreach (ComboModel o in vm.Combo5.Where(p => p.id != int.Parse(sender.AutomationId) && p.tipo == 5))
                            o.Seleccionado = false;

                    if (vm.Combo6.Count(p => p.id == int.Parse(sender.AutomationId)) > 0)
                        foreach (ComboModel o in vm.Combo6.Where(p => p.id != int.Parse(sender.AutomationId) && p.tipo == 6))
                            o.Seleccionado = false;

                    if (vm.Combo7.Count(p => p.id == int.Parse(sender.AutomationId)) > 0)
                        foreach (ComboModel o in vm.Combo7.Where(p => p.id != int.Parse(sender.AutomationId) && p.tipo == 7))
                            o.Seleccionado = false;

                    if (vm.Combo8.Count(p => p.id == int.Parse(sender.AutomationId)) > 0)
                        foreach (ComboModel o in vm.Combo8.Where(p => p.id != int.Parse(sender.AutomationId) && p.tipo == 8))
                            o.Seleccionado = false;
                }
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm = BindingContext as DetalleArticuloViewModel;
        }
    }
}
