using Mopups.Pages;
using Microsoft.Maui.Controls.Xaml;
using AsadorMoron.Models;
using System.Collections.ObjectModel;

namespace AsadorMoron.Views.Establecimientos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupPageInfoPedido : PopupPage
    {
        public PopupPageInfoPedido()
        {
            InitializeComponent();

        }

        public PopupPageInfoPedido(ObservableCollection<LineasPedido> lineas)
        {
            InitializeComponent();
            lista.ItemsSource = lineas;
        }
    }
}