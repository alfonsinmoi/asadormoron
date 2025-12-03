using System;
using System.Collections.Generic;
using AsadorMoron.Models;
using Mopups.Pages;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using AsadorMoron.Services;
using Microsoft.Maui.Controls.Xaml;
using Mopups.Services;
using System.Linq;

namespace AsadorMoron.Views.Repartidores
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupPageInfoRepartidor : PopupPage
    {
        RepartidorModel rep;
        public PopupPageInfoRepartidor()
        {
            InitializeComponent();
        }

        public PopupPageInfoRepartidor(RepartidorModel repartidor)
        {
            InitializeComponent();
            rep=repartidor;
            txtNombre.Text=repartidor.nombre;
            txtTelefono.Text=repartidor.telefono;
            pbFoto.Source=repartidor.foto;
            lista.ItemsSource=ResponseServiceWS.TraeMensajesRepartidor(repartidor.id);
            if (App.DAUtil.Usuario.rol == 3)
            {
                List<PredefinidosModel> mens = new List<PredefinidosModel>();
                PredefinidosModel p = new PredefinidosModel();
                p.texto = "";
                p.textoCorto = "(Manual)";
                mens.Add(p);
                if (App.DAUtil.Usuario.rol==3)
                    mens.AddRange(App.MensajesPredefinidos.Where(p2=>p2.administrador==true));
                else
                    mens.AddRange(App.MensajesPredefinidos.Where(p2 => p2.establecimiento == true));
                cbPredefinidos.ItemsSource = mens;
                cbPredefinidos.SelectedItem = mens[0];
            }
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            await App.ResponseWS.EnviarMensajeRepartidor(txtMensaje.Text,rep.id);
            await MopupService.Instance.PopAsync(true);
        }

        void cbPredefinidos_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            Picker pick = (Picker)sender;
            if (pick.SelectedIndex>0)
                txtMensaje.Text = ((PredefinidosModel)pick.SelectedItem).texto;
        }
    }
}
