using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Establecimientos
{
    class HorariosEstablecimientoViewModel : ViewModelBase
    {
        public HorariosEstablecimientoViewModel() { }

        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;

                try
                {
                    MisEstablecimientos = new ObservableCollection<Establecimiento>(App.DAUtil.Usuario.establecimientos);
                }
                catch (Exception ex)
                {
                    MisEstablecimientos = new ObservableCollection<Establecimiento>();
                    MisEstablecimientos.Add(App.EstActual);
                }
                MiEstablecimiento = App.EstActual;
                VisibleEstablecimiento = MisEstablecimientos.Count > 1;
            }
            catch (Exception ex)
            {
                // 
            }
            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }

        #region Metodos
        private void CargaHorarios()
        {
            Local = MiEstablecimiento.local==1;
            if (MiEstablecimiento.configuracion == null)
                MiEstablecimiento.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(MiEstablecimiento.idEstablecimiento);
            if (Local)
                NumTabs = 2;
            else
                NumTabs = 1;
            ActivoLunes = MiEstablecimiento.configuracion.activoLunes;
            ActivoMartes = MiEstablecimiento.configuracion.activoMartes;
            ActivoMiercoles = MiEstablecimiento.configuracion.activoMiercoles;
            ActivoJueves = MiEstablecimiento.configuracion.activoJueves;
            ActivoViernes = MiEstablecimiento.configuracion.activoViernes;
            ActivoSabado = MiEstablecimiento.configuracion.activoSabado;
            ActivoDomingo = MiEstablecimiento.configuracion.activoDomingo;
            ActivoLunesTarde = MiEstablecimiento.configuracion.activoLunesTarde;
            ActivoMartesTarde = MiEstablecimiento.configuracion.activoMartesTarde;
            ActivoMiercolesTarde = MiEstablecimiento.configuracion.activoMiercolesTarde;
            ActivoJuevesTarde = MiEstablecimiento.configuracion.activoJuevesTarde;
            ActivoViernesTarde = MiEstablecimiento.configuracion.activoViernesTarde;
            ActivoSabadoTarde = MiEstablecimiento.configuracion.activoSabadoTarde;
            ActivoDomingoTarde = MiEstablecimiento.configuracion.activoDomingoTarde;
            ActivoLunesLocal = MiEstablecimiento.configuracion.activoLunesLocal;
            ActivoMartesLocal = MiEstablecimiento.configuracion.activoMartesLocal;
            ActivoMiercolesLocal = MiEstablecimiento.configuracion.activoMiercolesLocal;
            ActivoJuevesLocal = MiEstablecimiento.configuracion.activoJuevesLocal;
            ActivoViernesLocal = MiEstablecimiento.configuracion.activoViernesLocal;
            ActivoSabadoLocal = MiEstablecimiento.configuracion.activoSabadoLocal;
            ActivoDomingoLocal = MiEstablecimiento.configuracion.activoDomingoLocal;
            ActivoLunesTardeLocal = MiEstablecimiento.configuracion.activoLunesTardeLocal;
            ActivoMartesTardeLocal = MiEstablecimiento.configuracion.activoMartesTardeLocal;
            ActivoMiercolesTardeLocal = MiEstablecimiento.configuracion.activoMiercolesTardeLocal;
            ActivoJuevesTardeLocal = MiEstablecimiento.configuracion.activoJuevesTardeLocal;
            ActivoViernesTardeLocal = MiEstablecimiento.configuracion.activoViernesTardeLocal;
            ActivoSabadoTardeLocal = MiEstablecimiento.configuracion.activoSabadoTardeLocal;
            ActivoDomingoTardeLocal = MiEstablecimiento.configuracion.activoDomingoTardeLocal;

            FinLunes = MiEstablecimiento.configuracion.finLunes;
            FinMartes = MiEstablecimiento.configuracion.finMartes;
            FinMiercoles = MiEstablecimiento.configuracion.finMiercoles;
            FinJueves = MiEstablecimiento.configuracion.finJueves;
            FinViernes = MiEstablecimiento.configuracion.finViernes;
            FinSabado = MiEstablecimiento.configuracion.finSabado;
            FinDomingo = MiEstablecimiento.configuracion.finDomingo;

            InicioLunes = MiEstablecimiento.configuracion.inicioLunes;
            InicioMartes = MiEstablecimiento.configuracion.inicioMartes;
            InicioMiercoles = MiEstablecimiento.configuracion.inicioMiercoles;
            InicioJueves = MiEstablecimiento.configuracion.inicioJueves;
            InicioViernes = MiEstablecimiento.configuracion.inicioViernes;
            InicioSabado = MiEstablecimiento.configuracion.inicioSabado;
            InicioDomingo = MiEstablecimiento.configuracion.inicioDomingo;

            FinLunesTarde = MiEstablecimiento.configuracion.finLunesTarde;
            FinMartesTarde = MiEstablecimiento.configuracion.finMartesTarde;
            FinMiercolesTarde = MiEstablecimiento.configuracion.finMiercolesTarde;
            FinJuevesTarde = MiEstablecimiento.configuracion.finJuevesTarde;
            FinViernesTarde = MiEstablecimiento.configuracion.finViernesTarde;
            FinSabadoTarde = MiEstablecimiento.configuracion.finSabadoTarde;
            FinDomingoTarde = MiEstablecimiento.configuracion.finDomingoTarde;

            InicioLunesTarde = MiEstablecimiento.configuracion.inicioLunesTarde;
            InicioMartesTarde = MiEstablecimiento.configuracion.inicioMartesTarde;
            InicioMiercolesTarde = MiEstablecimiento.configuracion.inicioMiercolesTarde;
            InicioJuevesTarde = MiEstablecimiento.configuracion.inicioJuevesTarde;
            InicioViernesTarde = MiEstablecimiento.configuracion.inicioViernesTarde;
            InicioSabadoTarde = MiEstablecimiento.configuracion.inicioSabadoTarde;
            InicioDomingoTarde = MiEstablecimiento.configuracion.inicioDomingoTarde;

            FinLunesLocal = MiEstablecimiento.configuracion.finLunesLocal;
            FinMartesLocal = MiEstablecimiento.configuracion.finMartesLocal;
            FinMiercolesLocal = MiEstablecimiento.configuracion.finMiercolesLocal;
            FinJuevesLocal = MiEstablecimiento.configuracion.finJuevesLocal;
            FinViernesLocal = MiEstablecimiento.configuracion.finViernesLocal;
            FinSabadoLocal = MiEstablecimiento.configuracion.finSabadoLocal;
            FinDomingoLocal = MiEstablecimiento.configuracion.finDomingoLocal;

            InicioLunesLocal = MiEstablecimiento.configuracion.inicioLunesLocal;
            InicioMartesLocal = MiEstablecimiento.configuracion.inicioMartesLocal;
            InicioMiercolesLocal = MiEstablecimiento.configuracion.inicioMiercolesLocal;
            InicioJuevesLocal = MiEstablecimiento.configuracion.inicioJuevesLocal;
            InicioViernesLocal = MiEstablecimiento.configuracion.inicioViernesLocal;
            InicioSabadoLocal = MiEstablecimiento.configuracion.inicioSabadoLocal;
            InicioDomingoLocal = MiEstablecimiento.configuracion.inicioDomingoLocal;

            FinLunesTardeLocal = MiEstablecimiento.configuracion.finLunesTardeLocal;
            FinMartesTardeLocal = MiEstablecimiento.configuracion.finMartesTardeLocal;
            FinMiercolesTardeLocal = MiEstablecimiento.configuracion.finMiercolesTardeLocal;
            FinJuevesTardeLocal = MiEstablecimiento.configuracion.finJuevesTardeLocal;
            FinViernesTardeLocal = MiEstablecimiento.configuracion.finViernesTardeLocal;
            FinSabadoTardeLocal = MiEstablecimiento.configuracion.finSabadoTardeLocal;
            FinDomingoTardeLocal = MiEstablecimiento.configuracion.finDomingoTardeLocal;

            InicioLunesTardeLocal = MiEstablecimiento.configuracion.inicioLunesTardeLocal;
            InicioMartesTardeLocal = MiEstablecimiento.configuracion.inicioMartesTardeLocal;
            InicioMiercolesTardeLocal = MiEstablecimiento.configuracion.inicioMiercolesTardeLocal;
            InicioJuevesTardeLocal = MiEstablecimiento.configuracion.inicioJuevesTardeLocal;
            InicioViernesTardeLocal = MiEstablecimiento.configuracion.inicioViernesTardeLocal;
            InicioSabadoTardeLocal = MiEstablecimiento.configuracion.inicioSabadoTardeLocal;
            InicioDomingoTardeLocal = MiEstablecimiento.configuracion.inicioDomingoTardeLocal;
        }
        private void GuardarConfiguracionEstablecimiento(object obj)
        {
            try
            {
                    //dias activos
                    MiEstablecimiento.configuracion.activoLunes = ActivoLunes;
                    MiEstablecimiento.configuracion.activoMartes = ActivoMartes;
                    MiEstablecimiento.configuracion.activoMiercoles = ActivoMiercoles;
                    MiEstablecimiento.configuracion.activoJueves = ActivoJueves;
                    MiEstablecimiento.configuracion.activoViernes = ActivoViernes;
                    MiEstablecimiento.configuracion.activoSabado = ActivoSabado;
                    MiEstablecimiento.configuracion.activoDomingo = ActivoDomingo;
                    MiEstablecimiento.configuracion.activoLunesTarde = ActivoLunesTarde;
                    MiEstablecimiento.configuracion.activoMartesTarde = ActivoMartesTarde;
                    MiEstablecimiento.configuracion.activoMiercolesTarde = ActivoMiercolesTarde;
                    MiEstablecimiento.configuracion.activoJuevesTarde = ActivoJuevesTarde;
                    MiEstablecimiento.configuracion.activoViernesTarde = ActivoViernesTarde;
                    MiEstablecimiento.configuracion.activoSabadoTarde = ActivoSabadoTarde;
                    MiEstablecimiento.configuracion.activoDomingoTarde = ActivoDomingoTarde;

                    MiEstablecimiento.configuracion.activoLunesLocal = ActivoLunesLocal;
                    MiEstablecimiento.configuracion.activoMartesLocal = ActivoMartesLocal;
                    MiEstablecimiento.configuracion.activoMiercolesLocal = ActivoMiercolesLocal;
                    MiEstablecimiento.configuracion.activoJuevesLocal = ActivoJuevesLocal;
                    MiEstablecimiento.configuracion.activoViernesLocal = ActivoViernesLocal;
                    MiEstablecimiento.configuracion.activoSabadoLocal = ActivoSabadoLocal;
                    MiEstablecimiento.configuracion.activoDomingoLocal = ActivoDomingoLocal;
                    MiEstablecimiento.configuracion.activoLunesTardeLocal = ActivoLunesTardeLocal;
                    MiEstablecimiento.configuracion.activoMartesTardeLocal = ActivoMartesTardeLocal;
                    MiEstablecimiento.configuracion.activoMiercolesTardeLocal = ActivoMiercolesTardeLocal;
                    MiEstablecimiento.configuracion.activoJuevesTardeLocal = ActivoJuevesTardeLocal;
                    MiEstablecimiento.configuracion.activoViernesTardeLocal = ActivoViernesTardeLocal;
                    MiEstablecimiento.configuracion.activoSabadoTardeLocal = ActivoSabadoTardeLocal;
                    MiEstablecimiento.configuracion.activoDomingoTardeLocal = ActivoDomingoTardeLocal;

                    //fecha inicio
                    MiEstablecimiento.configuracion.inicioLunes = InicioLunes;
                    MiEstablecimiento.configuracion.inicioMartes = InicioMartes;
                    MiEstablecimiento.configuracion.inicioMiercoles = InicioMiercoles;
                    MiEstablecimiento.configuracion.inicioJueves = InicioJueves;
                    MiEstablecimiento.configuracion.inicioViernes = InicioViernes;
                    MiEstablecimiento.configuracion.inicioSabado = InicioSabado;
                    MiEstablecimiento.configuracion.inicioDomingo = InicioDomingo;

                    MiEstablecimiento.configuracion.inicioLunesLocal = InicioLunesLocal;
                    MiEstablecimiento.configuracion.inicioMartesLocal = InicioMartesLocal;
                    MiEstablecimiento.configuracion.inicioMiercolesLocal = InicioMiercolesLocal;
                    MiEstablecimiento.configuracion.inicioJuevesLocal = InicioJuevesLocal;
                    MiEstablecimiento.configuracion.inicioViernesLocal = InicioViernesLocal;
                    MiEstablecimiento.configuracion.inicioSabadoLocal = InicioSabadoLocal;
                    MiEstablecimiento.configuracion.inicioDomingoLocal = InicioDomingoLocal;
                    //fecha fin
                    MiEstablecimiento.configuracion.finLunes = FinLunes;
                    MiEstablecimiento.configuracion.finMartes = FinMartes;
                    MiEstablecimiento.configuracion.finMiercoles = FinMiercoles;
                    MiEstablecimiento.configuracion.finJueves = FinJueves;
                    MiEstablecimiento.configuracion.finViernes = FinViernes;
                    MiEstablecimiento.configuracion.finSabado = FinSabado;
                    MiEstablecimiento.configuracion.finDomingo = FinDomingo;

                    MiEstablecimiento.configuracion.finLunesLocal = FinLunesLocal;
                    MiEstablecimiento.configuracion.finMartesLocal = FinMartesLocal;
                    MiEstablecimiento.configuracion.finMiercolesLocal = FinMiercolesLocal;
                    MiEstablecimiento.configuracion.finJuevesLocal = FinJuevesLocal;
                    MiEstablecimiento.configuracion.finViernesLocal = FinViernesLocal;
                    MiEstablecimiento.configuracion.finSabadoLocal = FinSabadoLocal;
                    MiEstablecimiento.configuracion.finDomingoLocal = FinDomingoLocal;

                    //fecha inicio tarde
                    MiEstablecimiento.configuracion.inicioLunesTarde = InicioLunesTarde;
                    MiEstablecimiento.configuracion.inicioMartesTarde = InicioMartesTarde;
                    MiEstablecimiento.configuracion.inicioMiercolesTarde = InicioMiercolesTarde;
                    MiEstablecimiento.configuracion.inicioJuevesTarde = InicioJuevesTarde;
                    MiEstablecimiento.configuracion.inicioViernesTarde = InicioViernesTarde;
                    MiEstablecimiento.configuracion.inicioSabadoTarde = InicioSabadoTarde;
                    MiEstablecimiento.configuracion.inicioDomingoTarde = InicioDomingoTarde;

                    MiEstablecimiento.configuracion.inicioLunesTardeLocal = InicioLunesTardeLocal;
                    MiEstablecimiento.configuracion.inicioMartesTardeLocal = InicioMartesTardeLocal;
                    MiEstablecimiento.configuracion.inicioMiercolesTardeLocal = InicioMiercolesTardeLocal;
                    MiEstablecimiento.configuracion.inicioJuevesTardeLocal = InicioJuevesTardeLocal;
                    MiEstablecimiento.configuracion.inicioViernesTardeLocal = InicioViernesTardeLocal;
                    MiEstablecimiento.configuracion.inicioSabadoTardeLocal = InicioSabadoTardeLocal;
                    MiEstablecimiento.configuracion.inicioDomingoTardeLocal = InicioDomingoTardeLocal;
                    //fecha fin tarde
                    MiEstablecimiento.configuracion.finLunesTarde = FinLunesTarde;
                    MiEstablecimiento.configuracion.finMartesTarde = FinMartesTarde;
                    MiEstablecimiento.configuracion.finMiercolesTarde = FinMiercolesTarde;
                    MiEstablecimiento.configuracion.finJuevesTarde = FinJuevesTarde;
                    MiEstablecimiento.configuracion.finViernesTarde = FinViernesTarde;
                    MiEstablecimiento.configuracion.finSabadoTarde = FinSabadoTarde;
                    MiEstablecimiento.configuracion.finDomingoTarde = FinDomingoTarde;

                    MiEstablecimiento.configuracion.finLunesTardeLocal = FinLunesTardeLocal;
                    MiEstablecimiento.configuracion.finMartesTardeLocal = FinMartesTardeLocal;
                    MiEstablecimiento.configuracion.finMiercolesTardeLocal = FinMiercolesTardeLocal;
                    MiEstablecimiento.configuracion.finJuevesTardeLocal = FinJuevesTardeLocal;
                    MiEstablecimiento.configuracion.finViernesTardeLocal = FinViernesTardeLocal;
                    MiEstablecimiento.configuracion.finSabadoTardeLocal = FinSabadoTardeLocal;
                    MiEstablecimiento.configuracion.finDomingoTardeLocal = FinDomingoTardeLocal;
                bool result = App.ResponseWS.updateConfiguracionEstablecimiento(MiEstablecimiento.configuracion);
                if (result)
                    App.customDialog.ShowDialogAsync(AppResources.DatosModificadosOK,AppResources.App, AppResources.Cerrar);
                else
                    App.customDialog.ShowDialogAsync(AppResources.DatosModificadosKO,AppResources.SoloError, AppResources.Cerrar);
            }
            catch (Exception ex)
            {
                // 
                App.customDialog.ShowDialogAsync(AppResources.DatosModificadosKO, AppResources.SoloError, AppResources.Cerrar);
            }
        }
        #endregion

        #region Comandos
        public ICommand GuardarConfiguracionEstablecimientoCommand { get { return new Command(GuardarConfiguracionEstablecimiento); } }
        #endregion

        #region Propiedades
        private int numTabs = 1;
        public int NumTabs
        {
            get { return numTabs; }
            set
            {
                numTabs = value;
                OnPropertyChanged(nameof(NumTabs));
            }
        }
        private bool activoLunes;
        public bool ActivoLunes
        {
            get { return activoLunes; }
            set
            {
                activoLunes = value;
                OnPropertyChanged(nameof(ActivoLunes));
            }
        }
        private bool activoLunesLocal;
        public bool ActivoLunesLocal
        {
            get { return activoLunesLocal; }
            set
            {
                activoLunesLocal = value;
                OnPropertyChanged(nameof(ActivoLunesLocal));
            }
        }
        private bool activoMartes;
        public bool ActivoMartes
        {
            get { return activoMartes; }
            set
            {
                activoMartes = value;
                OnPropertyChanged(nameof(ActivoMartes));
            }
        }

        private bool activoMiercoles;
        public bool ActivoMiercoles
        {
            get { return activoMiercoles; }
            set
            {
                activoMiercoles = value;
                OnPropertyChanged(nameof(ActivoMiercoles));
            }
        }

        private bool activoJueves;
        public bool ActivoJueves
        {
            get { return activoJueves; }
            set
            {
                activoJueves = value;
                OnPropertyChanged(nameof(ActivoJueves));
            }
        }

        private bool activoViernes;
        public bool ActivoViernes
        {
            get { return activoViernes; }
            set
            {
                activoViernes = value;
                OnPropertyChanged(nameof(ActivoViernes));
            }
        }

        private bool activoSabado;
        public bool ActivoSabado
        {
            get { return activoSabado; }
            set
            {
                activoSabado = value;
                OnPropertyChanged(nameof(ActivoSabado));
            }
        }

        private bool activoDomingo;
        public bool ActivoDomingo
        {
            get { return activoDomingo; }
            set
            {
                activoDomingo = value;
                OnPropertyChanged(nameof(ActivoDomingo));
            }
        }
        private bool activoMartesLocal;
        public bool ActivoMartesLocal
        {
            get { return activoMartesLocal; }
            set
            {
                activoMartesLocal = value;
                OnPropertyChanged(nameof(ActivoMartesLocal));
            }
        }

        private bool activoMiercolesLocal;
        public bool ActivoMiercolesLocal
        {
            get { return activoMiercolesLocal; }
            set
            {
                activoMiercolesLocal = value;
                OnPropertyChanged(nameof(ActivoMiercolesLocal));
            }
        }

        private bool activoJuevesLocal;
        public bool ActivoJuevesLocal
        {
            get { return activoJuevesLocal; }
            set
            {
                activoJuevesLocal = value;
                OnPropertyChanged(nameof(ActivoJuevesLocal));
            }
        }

        private bool activoViernesLocal;
        public bool ActivoViernesLocal
        {
            get { return activoViernesLocal; }
            set
            {
                activoViernesLocal = value;
                OnPropertyChanged(nameof(ActivoViernesLocal));
            }
        }

        private bool activoSabadoLocal;
        public bool ActivoSabadoLocal
        {
            get { return activoSabadoLocal; }
            set
            {
                activoSabadoLocal = value;
                OnPropertyChanged(nameof(ActivoSabadoLocal));
            }
        }

        private bool activoDomingoLocal;
        public bool ActivoDomingoLocal
        {
            get { return activoDomingoLocal; }
            set
            {
                activoDomingoLocal = value;
                OnPropertyChanged(nameof(ActivoDomingoLocal));
            }
        }
        private ObservableCollection<Establecimiento> misEstablecimientos;
        public ObservableCollection<Establecimiento> MisEstablecimientos
        {
            get { return misEstablecimientos; }
            set
            {
                misEstablecimientos = value;
                OnPropertyChanged(nameof(MisEstablecimientos));
            }
        }
        private Establecimiento miEstablecimiento;
        public Establecimiento MiEstablecimiento
        {
            get { return miEstablecimiento; }
            set
            {
                miEstablecimiento = value;
                OnPropertyChanged(nameof(MiEstablecimiento));
                CargaHorarios();
            }
        }
        private bool visibleEstablecimiento = false;
        public bool VisibleEstablecimiento
        {
            get { return visibleEstablecimiento; }
            set
            {
                visibleEstablecimiento = value;
                OnPropertyChanged(nameof(VisibleEstablecimiento));
            }
        }
        private TimeSpan inicioLunes;
        public TimeSpan InicioLunes
        {
            get { return inicioLunes; }
            set
            {
                inicioLunes = value;
                OnPropertyChanged(nameof(InicioLunes));
            }
        }

        private TimeSpan inicioMartes;
        public TimeSpan InicioMartes
        {
            get { return inicioMartes; }
            set
            {
                inicioMartes = value;
                OnPropertyChanged(nameof(InicioMartes));
            }
        }

        private TimeSpan inicioMiercoles;
        public TimeSpan InicioMiercoles
        {
            get { return inicioMiercoles; }
            set
            {
                inicioMiercoles = value;
                OnPropertyChanged(nameof(InicioMiercoles));
            }
        }

        private TimeSpan inicioJueves;
        public TimeSpan InicioJueves
        {
            get { return inicioJueves; }
            set
            {
                inicioJueves = value;
                OnPropertyChanged(nameof(InicioJueves));
            }
        }

        private TimeSpan inicioViernes;
        public TimeSpan InicioViernes
        {
            get { return inicioViernes; }
            set
            {
                inicioViernes = value;
                OnPropertyChanged(nameof(InicioViernes));
            }
        }

        private TimeSpan inicioSabado;
        public TimeSpan InicioSabado
        {
            get { return inicioSabado; }
            set
            {
                inicioSabado = value;
                OnPropertyChanged(nameof(InicioSabado));
            }
        }

        private TimeSpan inicioDomingo;
        public TimeSpan InicioDomingo
        {
            get { return inicioDomingo; }
            set
            {
                inicioDomingo = value;
                OnPropertyChanged(nameof(InicioDomingo));
            }
        }

        private TimeSpan finLunes;
        public TimeSpan FinLunes
        {
            get { return finLunes; }
            set
            {
                finLunes = value;
                OnPropertyChanged(nameof(FinLunes));
            }
        }
        private TimeSpan finMartes;

        public TimeSpan FinMartes
        {
            get { return finMartes; }
            set
            {
                finMartes = value;
                OnPropertyChanged(nameof(FinMartes));
            }
        }
        private TimeSpan finMiercoles;

        public TimeSpan FinMiercoles
        {
            get { return finMiercoles; }
            set
            {
                finMiercoles = value;
                OnPropertyChanged(nameof(FinMiercoles));
            }
        }
        private TimeSpan finJueves;

        public TimeSpan FinJueves
        {
            get { return finJueves; }
            set
            {
                finJueves = value;
                OnPropertyChanged(nameof(FinJueves));
            }
        }
        private TimeSpan finViernes;

        public TimeSpan FinViernes
        {
            get { return finViernes; }
            set
            {
                finViernes = value;
                OnPropertyChanged(nameof(FinViernes));
            }
        }
        private TimeSpan finSabado;

        public TimeSpan FinSabado
        {
            get { return finSabado; }
            set
            {
                finSabado = value;
                OnPropertyChanged(nameof(FinSabado));
            }
        }
        private TimeSpan finDomingo;

        public TimeSpan FinDomingo
        {
            get { return finDomingo; }
            set
            {
                finDomingo = value;
                OnPropertyChanged(nameof(FinDomingo));
            }
        }

        private TimeSpan inicioLunesTarde;

        public TimeSpan InicioLunesTarde
        {
            get { return inicioLunesTarde; }
            set
            {
                inicioLunesTarde = value;
                OnPropertyChanged(nameof(InicioLunesTarde));
            }
        }
        private TimeSpan inicioMartesTarde;

        public TimeSpan InicioMartesTarde
        {
            get { return inicioMartesTarde; }
            set
            {
                inicioMartesTarde = value;
                OnPropertyChanged(nameof(InicioMartesTarde));
            }
        }
        private TimeSpan inicioMiercolesTarde;

        public TimeSpan InicioMiercolesTarde
        {
            get { return inicioMiercolesTarde; }
            set
            {
                inicioMiercolesTarde = value;
                OnPropertyChanged(nameof(InicioMiercolesTarde));
            }
        }
        private TimeSpan inicioJuevesTarde;

        public TimeSpan InicioJuevesTarde
        {
            get { return inicioJuevesTarde; }
            set
            {
                inicioJuevesTarde = value;
                OnPropertyChanged(nameof(InicioJuevesTarde));
            }
        }
        private TimeSpan inicioViernesTarde;

        public TimeSpan InicioViernesTarde
        {
            get { return inicioViernesTarde; }
            set
            {
                inicioViernesTarde = value;
                OnPropertyChanged(nameof(InicioViernesTarde));
            }
        }
        private TimeSpan inicioSabadoTarde;

        public TimeSpan InicioSabadoTarde
        {
            get { return inicioSabadoTarde; }
            set
            {
                inicioSabadoTarde = value;
                OnPropertyChanged(nameof(InicioSabadoTarde));
            }
        }
        private TimeSpan inicioDomingoTarde;

        public TimeSpan InicioDomingoTarde
        {
            get { return inicioDomingoTarde; }
            set
            {
                inicioDomingoTarde = value;
                OnPropertyChanged(nameof(InicioDomingoTarde));
            }
        }

        private TimeSpan finLunesTarde;
        public TimeSpan FinLunesTarde
        {
            get { return finLunesTarde; }
            set
            {
                finLunesTarde = value;
                OnPropertyChanged(nameof(FinLunesTarde));
            }
        }
        private TimeSpan finMartesTarde;

        public TimeSpan FinMartesTarde
        {
            get { return finMartesTarde; }
            set
            {
                finMartesTarde = value;
                OnPropertyChanged(nameof(FinMartesTarde));
            }
        }
        private TimeSpan finMiercolesTarde;

        public TimeSpan FinMiercolesTarde
        {
            get { return finMiercolesTarde; }
            set
            {
                finMiercolesTarde = value;
                OnPropertyChanged(nameof(FinMiercolesTarde));
            }
        }
        private TimeSpan finJuevesTarde;

        public TimeSpan FinJuevesTarde
        {
            get { return finJuevesTarde; }
            set
            {
                finJuevesTarde = value;
                OnPropertyChanged(nameof(FinJuevesTarde));
            }
        }
        private TimeSpan finViernesTarde;

        public TimeSpan FinViernesTarde
        {
            get { return finViernesTarde; }
            set
            {
                finViernesTarde = value;
                OnPropertyChanged(nameof(FinViernesTarde));
            }
        }
        private TimeSpan finSabadoTarde;

        public TimeSpan FinSabadoTarde
        {
            get { return finSabadoTarde; }
            set
            {
                finSabadoTarde = value;
                OnPropertyChanged(nameof(FinSabadoTarde));
            }
        }
        private TimeSpan finDomingoTarde;

        public TimeSpan FinDomingoTarde
        {
            get { return finDomingoTarde; }
            set
            {
                finDomingoTarde = value;
                OnPropertyChanged(nameof(FinDomingoTarde));
            }
        }
        private TimeSpan inicioLunesLocal;
        public TimeSpan InicioLunesLocal
        {
            get { return inicioLunesLocal; }
            set
            {
                inicioLunesLocal = value;
                OnPropertyChanged(nameof(InicioLunesLocal));
            }
        }

        private TimeSpan inicioMartesLocal;
        public TimeSpan InicioMartesLocal
        {
            get { return inicioMartesLocal; }
            set
            {
                inicioMartesLocal = value;
                OnPropertyChanged(nameof(InicioMartesLocal));
            }
        }

        private TimeSpan inicioMiercolesLocal;
        public TimeSpan InicioMiercolesLocal
        {
            get { return inicioMiercolesLocal; }
            set
            {
                inicioMiercolesLocal = value;
                OnPropertyChanged(nameof(InicioMiercolesLocal));
            }
        }

        private TimeSpan inicioJuevesLocal;
        public TimeSpan InicioJuevesLocal
        {
            get { return inicioJuevesLocal; }
            set
            {
                inicioJuevesLocal = value;
                OnPropertyChanged(nameof(InicioJuevesLocal));
            }
        }

        private TimeSpan inicioViernesLocal;
        public TimeSpan InicioViernesLocal
        {
            get { return inicioViernesLocal; }
            set
            {
                inicioViernesLocal = value;
                OnPropertyChanged(nameof(InicioViernesLocal));
            }
        }

        private TimeSpan inicioSabadoLocal;
        public TimeSpan InicioSabadoLocal
        {
            get { return inicioSabadoLocal; }
            set
            {
                inicioSabadoLocal = value;
                OnPropertyChanged(nameof(InicioSabadoLocal));
            }
        }

        private TimeSpan inicioDomingoLocal;
        public TimeSpan InicioDomingoLocal
        {
            get { return inicioDomingoLocal; }
            set
            {
                inicioDomingoLocal = value;
                OnPropertyChanged(nameof(InicioDomingoLocal));
            }
        }

        private TimeSpan finLunesLocal;
        public TimeSpan FinLunesLocal
        {
            get { return finLunesLocal; }
            set
            {
                finLunesLocal = value;
                OnPropertyChanged(nameof(FinLunesLocal));
            }
        }
        private TimeSpan finMartesLocal;

        public TimeSpan FinMartesLocal
        {
            get { return finMartesLocal; }
            set
            {
                finMartesLocal = value;
                OnPropertyChanged(nameof(FinMartesLocal));
            }
        }
        private TimeSpan finMiercolesLocal;

        public TimeSpan FinMiercolesLocal
        {
            get { return finMiercolesLocal; }
            set
            {
                finMiercolesLocal = value;
                OnPropertyChanged(nameof(FinMiercolesLocal));
            }
        }
        private TimeSpan finJuevesLocal;

        public TimeSpan FinJuevesLocal
        {
            get { return finJuevesLocal; }
            set
            {
                finJuevesLocal = value;
                OnPropertyChanged(nameof(FinJuevesLocal));
            }
        }
        private TimeSpan finViernesLocal;

        public TimeSpan FinViernesLocal
        {
            get { return finViernesLocal; }
            set
            {
                finViernesLocal = value;
                OnPropertyChanged(nameof(FinViernesLocal));
            }
        }
        private TimeSpan finSabadoLocal;

        public TimeSpan FinSabadoLocal
        {
            get { return finSabadoLocal; }
            set
            {
                finSabadoLocal = value;
                OnPropertyChanged(nameof(FinSabadoLocal));
            }
        }
        private TimeSpan finDomingoLocal;

        public TimeSpan FinDomingoLocal
        {
            get { return finDomingoLocal; }
            set
            {
                finDomingoLocal = value;
                OnPropertyChanged(nameof(FinDomingoLocal));
            }
        }

        private TimeSpan inicioLunesTardeLocal;

        public TimeSpan InicioLunesTardeLocal
        {
            get { return inicioLunesTardeLocal; }
            set
            {
                inicioLunesTardeLocal = value;
                OnPropertyChanged(nameof(InicioLunesTardeLocal));
            }
        }
        private TimeSpan inicioMartesTardeLocal;

        public TimeSpan InicioMartesTardeLocal
        {
            get { return inicioMartesTardeLocal; }
            set
            {
                inicioMartesTardeLocal = value;
                OnPropertyChanged(nameof(InicioMartesTardeLocal));
            }
        }
        private TimeSpan inicioMiercolesTardeLocal;

        public TimeSpan InicioMiercolesTardeLocal
        {
            get { return inicioMiercolesTardeLocal; }
            set
            {
                inicioMiercolesTardeLocal = value;
                OnPropertyChanged(nameof(InicioMiercolesTardeLocal));
            }
        }
        private TimeSpan inicioJuevesTardeLocal;

        public TimeSpan InicioJuevesTardeLocal
        {
            get { return inicioJuevesTardeLocal; }
            set
            {
                inicioJuevesTardeLocal = value;
                OnPropertyChanged(nameof(InicioJuevesTardeLocal));
            }
        }
        private TimeSpan inicioViernesTardeLocal;

        public TimeSpan InicioViernesTardeLocal
        {
            get { return inicioViernesTardeLocal; }
            set
            {
                inicioViernesTardeLocal = value;
                OnPropertyChanged(nameof(InicioViernesTardeLocal));
            }
        }
        private TimeSpan inicioSabadoTardeLocal;

        public TimeSpan InicioSabadoTardeLocal
        {
            get { return inicioSabadoTardeLocal; }
            set
            {
                inicioSabadoTardeLocal = value;
                OnPropertyChanged(nameof(InicioSabadoTardeLocal));
            }
        }
        private TimeSpan inicioDomingoTardeLocal;

        public TimeSpan InicioDomingoTardeLocal
        {
            get { return inicioDomingoTardeLocal; }
            set
            {
                inicioDomingoTardeLocal = value;
                OnPropertyChanged(nameof(InicioDomingoTardeLocal));
            }
        }

        private TimeSpan finLunesTardeLocal;
        public TimeSpan FinLunesTardeLocal
        {
            get { return finLunesTardeLocal; }
            set
            {
                finLunesTardeLocal = value;
                OnPropertyChanged(nameof(FinLunesTardeLocal));
            }
        }
        private TimeSpan finMartesTardeLocal;

        public TimeSpan FinMartesTardeLocal
        {
            get { return finMartesTardeLocal; }
            set
            {
                finMartesTardeLocal = value;
                OnPropertyChanged(nameof(FinMartesTardeLocal));
            }
        }
        private TimeSpan finMiercolesTardeLocal;

        public TimeSpan FinMiercolesTardeLocal
        {
            get { return finMiercolesTardeLocal; }
            set
            {
                finMiercolesTardeLocal = value;
                OnPropertyChanged(nameof(FinMiercolesTardeLocal));
            }
        }
        private TimeSpan finJuevesTardeLocal;

        public TimeSpan FinJuevesTardeLocal
        {
            get { return finJuevesTardeLocal; }
            set
            {
                finJuevesTardeLocal = value;
                OnPropertyChanged(nameof(FinJuevesTardeLocal));
            }
        }
        private TimeSpan finViernesTardeLocal;

        public TimeSpan FinViernesTardeLocal
        {
            get { return finViernesTardeLocal; }
            set
            {
                finViernesTardeLocal = value;
                OnPropertyChanged(nameof(FinViernesTardeLocal));
            }
        }
        private TimeSpan finSabadoTardeLocal;

        public TimeSpan FinSabadoTardeLocal
        {
            get { return finSabadoTardeLocal; }
            set
            {
                finSabadoTardeLocal = value;
                OnPropertyChanged(nameof(FinSabadoTardeLocal));
            }
        }
        private TimeSpan finDomingoTardeLocal;

        public TimeSpan FinDomingoTardeLocal
        {
            get { return finDomingoTardeLocal; }
            set
            {
                finDomingoTardeLocal = value;
                OnPropertyChanged(nameof(FinDomingoTardeLocal));
            }
        }
        private bool activoLunesTarde;
        public bool ActivoLunesTarde
        {
            get
            {
                return activoLunesTarde;
            }
            set
            {
                if (activoLunesTarde != value)
                {
                    activoLunesTarde = value;
                    OnPropertyChanged(nameof(ActivoLunesTarde));
                }
            }
        }
        private bool activoMartesTarde;
        public bool ActivoMartesTarde
        {
            get
            {
                return activoMartesTarde;
            }
            set
            {
                if (activoMartesTarde != value)
                {
                    activoMartesTarde = value;
                    OnPropertyChanged(nameof(ActivoMartesTarde));
                }
            }
        }
        private bool activoMiercolesTarde;
        public bool ActivoMiercolesTarde
        {
            get
            {
                return activoMiercolesTarde;
            }
            set
            {
                if (activoMiercolesTarde != value)
                {
                    activoMiercolesTarde = value;
                    OnPropertyChanged(nameof(ActivoMiercolesTarde));
                }
            }
        }
        private bool activoJuevesTarde;
        public bool ActivoJuevesTarde
        {
            get
            {
                return activoJuevesTarde;
            }
            set
            {
                if (activoJuevesTarde != value)
                {
                    activoJuevesTarde = value;
                    OnPropertyChanged(nameof(ActivoJuevesTarde));
                }
            }
        }
        private bool activoViernesTarde;
        public bool ActivoViernesTarde
        {
            get
            {
                return activoViernesTarde;
            }
            set
            {
                if (activoViernesTarde != value)
                {
                    activoViernesTarde = value;
                    OnPropertyChanged(nameof(ActivoViernesTarde));
                }
            }
        }
        private bool activoSabadoTarde;
        public bool ActivoSabadoTarde
        {
            get
            {
                return activoSabadoTarde;
            }
            set
            {
                if (activoSabadoTarde != value)
                {
                    activoSabadoTarde = value;
                    OnPropertyChanged(nameof(ActivoSabadoTarde));
                }
            }
        }
        private bool activoDomingoTarde;
        public bool ActivoDomingoTarde
        {
            get
            {
                return activoDomingoTarde;
            }
            set
            {
                if (activoDomingoTarde != value)
                {
                    activoDomingoTarde = value;
                    OnPropertyChanged(nameof(ActivoDomingoTarde));
                }
            }
        }
        private bool activoLunesTardeLocal;
        public bool ActivoLunesTardeLocal
        {
            get
            {
                return activoLunesTardeLocal;
            }
            set
            {
                if (activoLunesTardeLocal != value)
                {
                    activoLunesTardeLocal = value;
                    OnPropertyChanged(nameof(ActivoLunesTardeLocal));
                }
            }
        }
        private bool activoMartesTardeLocal;
        public bool ActivoMartesTardeLocal
        {
            get
            {
                return activoMartesTardeLocal;
            }
            set
            {
                if (activoMartesTardeLocal != value)
                {
                    activoMartesTardeLocal = value;
                    OnPropertyChanged(nameof(ActivoMartesTardeLocal));
                }
            }
        }
        private bool activoMiercolesTardeLocal;
        public bool ActivoMiercolesTardeLocal
        {
            get
            {
                return activoMiercolesTardeLocal;
            }
            set
            {
                if (activoMiercolesTardeLocal != value)
                {
                    activoMiercolesTardeLocal = value;
                    OnPropertyChanged(nameof(ActivoMiercolesTardeLocal));
                }
            }
        }
        private bool activoJuevesTardeLocal;
        public bool ActivoJuevesTardeLocal
        {
            get
            {
                return activoJuevesTardeLocal;
            }
            set
            {
                if (activoJuevesTardeLocal != value)
                {
                    activoJuevesTardeLocal = value;
                    OnPropertyChanged(nameof(ActivoJuevesTardeLocal));
                }
            }
        }
        private bool local;
        public bool Local
        {
            get
            {
                return local;
            }
            set
            {
                if (local != value)
                {
                    local = value;
                    OnPropertyChanged(nameof(Local));
                }
            }
        }
        private bool activoViernesTardeLocal;
        public bool ActivoViernesTardeLocal
        {
            get
            {
                return activoViernesTardeLocal;
            }
            set
            {
                if (activoViernesTardeLocal != value)
                {
                    activoViernesTardeLocal = value;
                    OnPropertyChanged(nameof(ActivoViernesTardeLocal));
                }
            }
        }
        private bool activoSabadoTardeLocal;
        public bool ActivoSabadoTardeLocal
        {
            get
            {
                return activoSabadoTardeLocal;
            }
            set
            {
                if (activoSabadoTardeLocal != value)
                {
                    activoSabadoTardeLocal = value;
                    OnPropertyChanged(nameof(ActivoSabadoTardeLocal));
                }
            }
        }
        private bool activoDomingoTardeLocal;
        public bool ActivoDomingoTardeLocal
        {
            get
            {
                return activoDomingoTardeLocal;
            }
            set
            {
                if (activoDomingoTardeLocal != value)
                {
                    activoDomingoTardeLocal = value;
                    OnPropertyChanged(nameof(ActivoDomingoTardeLocal));
                }
            }
        }
        #endregion
    }
}
