using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    class ConfiguracionEstablecimientoViewModel : ViewModelBase
    {
        private ConfiguracionAdmin configuracionAdmin = new ConfiguracionAdmin();
        public ConfiguracionEstablecimientoViewModel() { }

        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                Visibilidades = new ObservableCollection<string>();
                Visibilidades.Add("Ver Fecha Entrega");
                Visibilidades.Add("Ver Fecha Comienzo");
                Visibilidades.Add("Ver Dos Fechas");

                TiposAutoPedido = new ObservableCollection<TipoAutoPedidoModel>();
                TipoAutoPedidoModel t = new TipoAutoPedidoModel();
                t.id = 1;
                t.nombre = "Completo";
                TiposAutoPedido.Add(t);
                t = new TipoAutoPedidoModel();
                t.id = 2;
                t.nombre = "Sólo precio";
                TiposAutoPedido.Add(t);
                t = new TipoAutoPedidoModel();
                t.id = 3;
                t.nombre = "Simple";
                TiposAutoPedido.Add(t);

                TiemposComercios = new ObservableCollection<TiempoEntregaComercioModel>();
                TiempoEntregaComercioModel tiempo = new TiempoEntregaComercioModel();
                tiempo.horas = 0;
                tiempo.texto = "MISMO DÍA";
                TiemposComercios.Add(tiempo);
                tiempo = new TiempoEntregaComercioModel();
                tiempo.horas = 24;
                tiempo.texto = "DÍA SIGUIENTE";
                TiemposComercios.Add(tiempo);
                tiempo = new TiempoEntregaComercioModel();
                tiempo.horas = 48;
                tiempo.texto = "DOS DÍAS";
                TiemposComercios.Add(tiempo);
                tiempo = new TiempoEntregaComercioModel();
                tiempo.horas = 72;
                tiempo.texto = "TRES DÍAS";
                TiemposComercios.Add(tiempo);

                EstadosPedido = new ObservableCollection<EstadoPedidoModel>();
                EstadoPedidoModel estad = new EstadoPedidoModel();
                estad.nombre = "Nuevo";
                estad.id = 1;
                EstadosPedido.Add(estad);
                estad = new EstadoPedidoModel();
                estad.nombre = "Visto";
                estad.id = 2;
                EstadosPedido.Add(estad);
                estad = new EstadoPedidoModel();
                estad.nombre = "A Recoger";
                estad.id = 3;
                EstadosPedido.Add(estad);
                MiEstablecimiento = App.EstActual;
                CargaConfiguracion();
                CargaConfiguracionAdmin();
                App.DAUtil.EnTimer = false;

                

                EsAdmin = App.DAUtil.Usuario.rol == (int)RolesEnum.Administrador;

                
            }
            catch (Exception ex)
            {
                // 
            }
            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }

        #region Metodos
        private void CargaConfiguracionAdmin()
        {
            configuracionAdmin = ResponseServiceWS.getConfiguracionAdmin(App.DAUtil.Usuario.idPueblo);
            if (configuracionAdmin == null)
            {
                configuracionAdmin.servicioActivo = false;
            }
            else
            {
                TicketSize = configuracionAdmin.ticketSize;
                IBAN = configuracionAdmin.iban;


                GastosEnvio = configuracionAdmin.gastosEnvio.ToString().Replace(",", ".");

                Efectivo = configuracionAdmin.efectivo;
                Tarjeta = configuracionAdmin.tarjeta;
                Datafono = configuracionAdmin.datafono;

                NombreTicket = configuracionAdmin.nombreTicket;
                DireccionTicket = configuracionAdmin.direccionTicket;
                CIFTicket = configuracionAdmin.cifTicket;
                TelefonoTicket = configuracionAdmin.telefonoTicket;
            }
        }
        private void CargaConfiguracion()
        {
            try
            {
                if (MiEstablecimiento.configuracion == null)
                    MiEstablecimiento.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(MiEstablecimiento.idEstablecimiento);
                if (MiEstablecimiento.configuracion != null)
                {
                    ConfiguracionAdmin admin = ResponseServiceWS.getConfiguracionAdmin();
                    RangoBolsa = MiEstablecimiento.configuracion.rangoBolsas.ToString().Replace(",", ".");
                    PrecioBolsa = MiEstablecimiento.configuracion.precioBolsa.ToString().Replace(",", ".");
                    TicketSize = MiEstablecimiento.configuracion.ticketSize;
                    HoraSoloTarjetaDesde = MiEstablecimiento.configuracion.horaSoloTarjetaDesde;
                    HoraSoloTarjetaHasta = MiEstablecimiento.configuracion.horaSoloTarjetaHasta;
                    ActivaSoloTarjeta = MiEstablecimiento.configuracion.activaSoloTarjeta;
                    EsComercio = MiEstablecimiento.esComercio;
                    CualquierHoraOtroPueblo = MiEstablecimiento.configuracion.cualquierHoraOtroPueblo;
                    NombreImpresora = MiEstablecimiento.configuracion.nombreImpresora;
                    NombreImpresora2 = MiEstablecimiento.configuracion.nombreImpresora2;
                    NombreImpresora3 = MiEstablecimiento.configuracion.nombreImpresora3;
                    NombreImpresora4 = MiEstablecimiento.configuracion.nombreImpresora4;
                    NombreImpresora5 = MiEstablecimiento.configuracion.nombreImpresora5;
                    NombreImpresora6 = MiEstablecimiento.configuracion.nombreImpresora6;
                    NombreImpresora7 = MiEstablecimiento.configuracion.nombreImpresora7;
                    NombreImpresora8 = MiEstablecimiento.configuracion.nombreImpresora8;
                    NombreImpresora9 = MiEstablecimiento.configuracion.nombreImpresora9;
                    NombreImpresora10 = MiEstablecimiento.configuracion.nombreImpresora10;
                    TextoCerrado = MiEstablecimiento.configuracion.textoCerrado;
                    PedidoMinimo = MiEstablecimiento.configuracion.pedidoMinimo.ToString().Replace(",", ".");
                    EstadoPedido = EstadosPedido.Where(p=>p.id==MiEstablecimiento.configuracion.estadoAutoPedido).FirstOrDefault();
                    TipoAutoPedido = TiposAutoPedido.Where(p=>p.id== MiEstablecimiento.configuracion.tipoAutoPedido).FirstOrDefault();
                    ServicioActivo = MiEstablecimiento.configuracion.servicioActivo;
                    TiempoEntrega = MiEstablecimiento.configuracion.tiempoEntrega;

                    NumImpresion = MiEstablecimiento.configuracion.numImpresion;
                    Escaparate = MiEstablecimiento.configuracion.modoEscaparate;
                    Detalle = MiEstablecimiento.configuracion.detalles;
                    TextoIngredientes = MiEstablecimiento.configuracion.textoIngredientes;
                    TextoIngredientes_eng = MiEstablecimiento.configuracion.textoIngredientes;
                    TextoIngredientes_ger = MiEstablecimiento.configuracion.textoIngredientes;
                    TextoIngredientes_fr = MiEstablecimiento.configuracion.textoIngredientes;
                    Comision = MiEstablecimiento.configuracion.comision.ToString().Replace(",", ".");
                    ComisionLocal = MiEstablecimiento.configuracion.comisionLocal.ToString().Replace(",", ".");
                    ComisionRecogida = MiEstablecimiento.configuracion.comisionRecogida.ToString().Replace(",", ".");
                    ComisionReparto = MiEstablecimiento.configuracion.comisionReparto.ToString().Replace(",", ".");
                    GastosEnvioPropio = MiEstablecimiento.configuracion.gastosEnvioPropio.ToString().Replace(",", ".");
                    ComisionAutoPedido = MiEstablecimiento.configuracion.comisionAutoPedido.ToString().Replace(",", ".");
                    CuotaFija = MiEstablecimiento.configuracion.cuotaFija.ToString().Replace(",", ".");
                    OtrasCuotas = MiEstablecimiento.configuracion.otrasCuotas.ToString().Replace(",", ".");
                    RepartoPropio = MiEstablecimiento.configuracion.repartoPropio;
                    RepartoPolloAndaluz = MiEstablecimiento.configuracion.repartoPolloAndaluz;
                    AceptaEncargos = MiEstablecimiento.configuracion.aceptaEncargos;
                    EncargosDiasDesde = MiEstablecimiento.configuracion.encargosDiasDesde;
                    EncargosDiasHasta = MiEstablecimiento.configuracion.encargosDiasHasta;
                    EncargosPorHora = MiEstablecimiento.configuracion.encargosPorHora;
                    SistemaPuntos = MiEstablecimiento.configuracion.sistemaPuntos;
                    PuntosPorPedido = MiEstablecimiento.configuracion.puntosPorPedido;
                    PuntosPorEuro = MiEstablecimiento.configuracion.puntosPorEuro;
                    TextoPuntos = MiEstablecimiento.configuracion.textoPuntos;
                    
                    VisibilidadHoras = MiEstablecimiento.configuracion.visibilidadHoras;
                    TiempoComercio = TiemposComercios.Where(p => p.horas == MiEstablecimiento.configuracion.tiempoRepartoComercio).FirstOrDefault();
                    RepartoComercioM = MiEstablecimiento.configuracion.repartoComercioM;
                    RepartoComercioT = MiEstablecimiento.configuracion.repartoComercioT;
                }
            }
            catch (Exception ex)
            {
                App.customDialog.ShowDialogAsync(AppResources.Error + ex.Message, AppResources.App, AppResources.Cerrar);
            }
        }
        private void GuardarConfiguracionAdmin()
        {
            try
            {
                configuracionAdmin.tiempoEntreMenus = 0;
                configuracionAdmin.ticketSize = TicketSize;
                configuracionAdmin.beneficios = 0;
                configuracionAdmin.servicioActivo = ServicioActivo;
                configuracionAdmin.idPueblo = App.DAUtil.Usuario.idPueblo;
                configuracionAdmin.nombreImpresora = NombreImpresora;
                //dias activos
                configuracionAdmin.activoLunes = true;
                configuracionAdmin.activoMartes = true;
                configuracionAdmin.activoMiercoles = true;
                configuracionAdmin.activoJueves = true;
                configuracionAdmin.activoViernes = true;
                configuracionAdmin.activoSabado = true;
                configuracionAdmin.activoDomingo = true;
                configuracionAdmin.activoLunesTarde = true;
                configuracionAdmin.activoMartesTarde = true;
                configuracionAdmin.activoMiercolesTarde = true;
                configuracionAdmin.activoJuevesTarde = true;
                configuracionAdmin.activoViernesTarde = true;
                configuracionAdmin.activoSabadoTarde = true;
                configuracionAdmin.activoDomingoTarde = true;
                //fecha inicio
                configuracionAdmin.inicioLunes = new TimeSpan(0,0,0);
                configuracionAdmin.inicioMartes = new TimeSpan(0, 0, 0);
                configuracionAdmin.inicioMiercoles = new TimeSpan(0, 0, 0);
                configuracionAdmin.inicioJueves = new TimeSpan(0, 0, 0);
                configuracionAdmin.inicioViernes = new TimeSpan(0, 0, 0);
                configuracionAdmin.inicioSabado = new TimeSpan(0, 0, 0);
                configuracionAdmin.inicioDomingo = new TimeSpan(0, 0, 0);
                //fecha fin
                configuracionAdmin.finLunes = new TimeSpan(23, 59, 59);
                configuracionAdmin.finMartes = new TimeSpan(23, 59, 59);
                configuracionAdmin.finMiercoles = new TimeSpan(23, 59, 59);
                configuracionAdmin.finJueves = new TimeSpan(23, 59, 59);
                configuracionAdmin.finViernes = new TimeSpan(23, 59, 59);
                configuracionAdmin.finSabado = new TimeSpan(23, 59, 59);
                configuracionAdmin.finDomingo = new TimeSpan(23, 59, 59);

                //fecha inicio tarde
                configuracionAdmin.inicioLunesTarde = new TimeSpan(0, 0, 0);
                configuracionAdmin.inicioMartesTarde = new TimeSpan(0, 0, 0);
                configuracionAdmin.inicioMiercolesTarde = new TimeSpan(0, 0, 0);
                configuracionAdmin.inicioJuevesTarde = new TimeSpan(0, 0, 0);
                configuracionAdmin.inicioViernesTarde = new TimeSpan(0, 0, 0);
                configuracionAdmin.inicioSabadoTarde = new TimeSpan(0, 0, 0);
                configuracionAdmin.inicioDomingoTarde = new TimeSpan(0, 0, 0);
                //fecha fin tarde
                configuracionAdmin.finLunesTarde = new TimeSpan(23, 59, 59);
                configuracionAdmin.finMartesTarde = new TimeSpan(23, 59, 59);
                configuracionAdmin.finMiercolesTarde = new TimeSpan(23, 59, 59);
                configuracionAdmin.finJuevesTarde = new TimeSpan(23, 59, 59);
                configuracionAdmin.finViernesTarde = new TimeSpan(23, 59, 59);
                configuracionAdmin.finSabadoTarde = new TimeSpan(23, 59, 59);
                configuracionAdmin.finDomingoTarde = new TimeSpan(23, 59, 59);

                configuracionAdmin.latitud = 0;
                configuracionAdmin.longitud = 0;
                configuracionAdmin.gastosEnvio = double.Parse(GastosEnvio.ToString().Replace(".", ","));
                configuracionAdmin.distanciaMapas = 0;
                configuracionAdmin.pedidoMinimo = 0;
                configuracionAdmin.pedidoMinimoComercio = 0;

                configuracionAdmin.inicioComercio = new TimeSpan(0, 0, 0);
                configuracionAdmin.finComercio = new TimeSpan(23, 59, 59);
                configuracionAdmin.telefono = "";
                configuracionAdmin.whatsapp = "";
                configuracionAdmin.email = "";

                configuracionAdmin.inicioComercioTarde = new TimeSpan(0, 0, 0);
                configuracionAdmin.finComercioTarde = new TimeSpan(23, 59, 59);

                configuracionAdmin.tarjeta = Tarjeta;
                configuracionAdmin.efectivo = Efectivo;
                configuracionAdmin.bizum = false;
                configuracionAdmin.datafono = Datafono;

                configuracionAdmin.telefonoTicket = TelefonoTicket;
                configuracionAdmin.nombreTicket = NombreTicket;
                configuracionAdmin.direccionTicket = DireccionTicket;
                configuracionAdmin.cifTicket = CIFTicket;

                configuracionAdmin.extraLunes = 0;
                configuracionAdmin.extraMartes = 0;
                configuracionAdmin.extraMiercoles = 0;
                configuracionAdmin.extraJueves = 0;
                configuracionAdmin.extraViernes = 0;
                configuracionAdmin.extraSabado = 0;
                configuracionAdmin.extraDomingo = 0;

                configuracionAdmin.iban = IBAN;
                bool result = App.ResponseWS.updateConfiguracionAdmin(configuracionAdmin);
                if (result)
                    App.customDialog.ShowDialogAsync(AppResources.DatosModificadosOK, AppResources.App, AppResources.Cerrar);
                else
                    App.customDialog.ShowDialogAsync(AppResources.DatosModificadosKO, AppResources.SoloError, AppResources.Cerrar);
                
            }
            catch (Exception ex)
            {
                // 
                App.customDialog.ShowDialogAsync(AppResources.DatosModificadosKO, AppResources.SoloError, AppResources.Cerrar);
            }
        }
        private void GuardarConfiguracionEstablecimiento(object obj)
        {
            try
            {
                if (EncargosDiasDesde > EncargosDiasHasta && AceptaEncargos)
                    App.customDialog.ShowDialogAsync(AppResources.ErrorEncargosDias, AppResources.SoloError, AppResources.Cerrar);
                else if (EncargosPorHora <= 0 && AceptaEncargos)
                    App.customDialog.ShowDialogAsync(AppResources.ErrorEncargosHora, AppResources.SoloError, AppResources.Cerrar);
                else
                {
                    ConfiguracionAdmin conf = ResponseServiceWS.getConfiguracionAdmin();
                    MiEstablecimiento.configuracion.textoCerrado = TextoCerrado;
                    MiEstablecimiento.configuracion.cualquierHoraOtroPueblo = CualquierHoraOtroPueblo;
                    MiEstablecimiento.configuracion.pedidoMinimo = double.Parse(PedidoMinimo.ToString().Replace(".", ","));
                    MiEstablecimiento.configuracion.rangoBolsas = double.Parse(RangoBolsa.ToString().Replace(".", ","));
                    MiEstablecimiento.configuracion.precioBolsa = double.Parse(PrecioBolsa.ToString().Replace(".", ","));
                    MiEstablecimiento.configuracion.tiempoEntrega = TiempoEntrega;
                    MiEstablecimiento.configuracion.activaSoloTarjeta = ActivaSoloTarjeta;
                    MiEstablecimiento.configuracion.horaSoloTarjetaDesde = HoraSoloTarjetaDesde;
                    MiEstablecimiento.configuracion.horaSoloTarjetaHasta = HoraSoloTarjetaHasta;
                    MiEstablecimiento.configuracion.servicioActivo = ServicioActivo;
                    MiEstablecimiento.configuracion.numeroPedidosSoportado = 1000;
                    MiEstablecimiento.configuracion.nombreImpresora = NombreImpresora;
                    MiEstablecimiento.configuracion.nombreImpresora2 = NombreImpresora2;
                    MiEstablecimiento.configuracion.nombreImpresora3 = NombreImpresora3;
                    MiEstablecimiento.configuracion.nombreImpresora4 = NombreImpresora4;
                    MiEstablecimiento.configuracion.nombreImpresora5 = NombreImpresora5;
                    MiEstablecimiento.configuracion.nombreImpresora6 = NombreImpresora6;
                    MiEstablecimiento.configuracion.nombreImpresora7 = NombreImpresora7;
                    MiEstablecimiento.configuracion.nombreImpresora8 = NombreImpresora8;
                    MiEstablecimiento.configuracion.nombreImpresora9 = NombreImpresora9;
                    MiEstablecimiento.configuracion.nombreImpresora10 = NombreImpresora10;
                    MiEstablecimiento.configuracion.tipoAutoPedido = TipoAutoPedido.id;
                    MiEstablecimiento.configuracion.estadoAutoPedido = EstadoPedido.id;
                    MiEstablecimiento.configuracion.modoEscaparate = Escaparate;
                    MiEstablecimiento.configuracion.detalles = Detalle;
                    MiEstablecimiento.configuracion.ticketSize = TicketSize;
                    MiEstablecimiento.configuracion.textoDetalle_eng = "";
                    MiEstablecimiento.configuracion.textoDetalle_ger = "";
                    MiEstablecimiento.configuracion.textoDetalle_fr = "";
                    MiEstablecimiento.configuracion.textoDetalle = "";
                    MiEstablecimiento.configuracion.textoIngredientes = TextoIngredientes;
                    MiEstablecimiento.configuracion.numImpresion= NumImpresion;
                    if (string.IsNullOrEmpty(TextoIngredientes_eng))
                        TextoIngredientes_eng = TextoIngredientes;
                    if (string.IsNullOrEmpty(TextoIngredientes_ger))
                        TextoIngredientes_ger = TextoIngredientes;
                    if (string.IsNullOrEmpty(TextoIngredientes_fr))
                        TextoIngredientes_fr = TextoIngredientes;
                    MiEstablecimiento.configuracion.textoIngredientes_eng = TextoIngredientes_eng;
                    MiEstablecimiento.configuracion.textoIngredientes_ger = TextoIngredientes_ger;
                    MiEstablecimiento.configuracion.textoIngredientes_fr = TextoIngredientes_fr;
                    MiEstablecimiento.configuracion.comision = double.Parse(Comision.ToString().Replace(".", ","));
                    MiEstablecimiento.configuracion.comisionAutoPedido = double.Parse(ComisionAutoPedido.ToString().Replace(".", ","));
                    MiEstablecimiento.configuracion.comisionLocal = double.Parse(ComisionLocal.ToString().Replace(".", ","));
                    MiEstablecimiento.configuracion.comisionReparto = double.Parse(ComisionReparto.ToString().Replace(".", ","));
                    MiEstablecimiento.configuracion.comisionRecogida = double.Parse(ComisionRecogida.ToString().Replace(".", ","));
                    MiEstablecimiento.configuracion.cuotaFija = double.Parse(CuotaFija.ToString().Replace(".", ","));
                    MiEstablecimiento.configuracion.otrasCuotas = double.Parse(OtrasCuotas.ToString().Replace(".", ","));
                    MiEstablecimiento.configuracion.gastosEnvioPropio = double.Parse(GastosEnvioPropio.ToString().Replace(".", ","));
                    MiEstablecimiento.configuracion.repartoPolloAndaluz = RepartoPolloAndaluz;
                    MiEstablecimiento.configuracion.repartoPropio = RepartoPropio;
                    MiEstablecimiento.configuracion.preferenciaReparto = "Q";
                    MiEstablecimiento.configuracion.aceptaEncargos = AceptaEncargos;
                    MiEstablecimiento.configuracion.encargosDiasDesde = EncargosDiasDesde;
                    MiEstablecimiento.configuracion.encargosDiasHasta = EncargosDiasHasta;
                    MiEstablecimiento.configuracion.encargosPorHora = EncargosPorHora;
                    MiEstablecimiento.configuracion.sistemaPuntos=SistemaPuntos;
                    MiEstablecimiento.configuracion.puntosPorPedido=PuntosPorPedido;
                    MiEstablecimiento.configuracion.puntosPorEuro=PuntosPorEuro;
                    MiEstablecimiento.configuracion.textoPuntos=TextoPuntos;
                    MiEstablecimiento.configuracion.visibilidadHoras = VisibilidadHoras;
                    MiEstablecimiento.configuracion.tiempoRepartoComercio = TiempoComercio.horas;
                    MiEstablecimiento.configuracion.repartoComercioM=RepartoComercioM;
                    MiEstablecimiento.configuracion.repartoComercioT=RepartoComercioT;
                    bool result = App.ResponseWS.updateConfiguracionEstablecimiento(MiEstablecimiento.configuracion);
                    if (result)
                        App.customDialog.ShowDialogAsync(AppResources.DatosModificadosOK, AppResources.App, AppResources.Cerrar);
                    else
                        App.customDialog.ShowDialogAsync(AppResources.DatosModificadosKO, AppResources.SoloError, AppResources.Cerrar);
                }
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
        private int numImpresion = 3;
        public int NumImpresion
        {
            get { return numImpresion; }
            set
            {
                numImpresion = value;
                OnPropertyChanged(nameof(NumImpresion));
            }
        }private Establecimiento miEstablecimiento;
        public Establecimiento MiEstablecimiento
        {
            get { return miEstablecimiento; }
            set
            {
                miEstablecimiento = value;
                OnPropertyChanged(nameof(MiEstablecimiento));
                CargaConfiguracion();
            }
        }
        private string nombreImpresora;
        public string NombreImpresora
        {
            get
            {
                return nombreImpresora;
            }
            set
            {
                if (nombreImpresora != value)
                {
                    nombreImpresora = value;
                    OnPropertyChanged(nameof(NombreImpresora));
                }
            }
        }
        private bool cualquierHoraOtroPueblo;
        public bool CualquierHoraOtroPueblo
        {
            get
            {
                return cualquierHoraOtroPueblo;
            }
            set
            {
                if (cualquierHoraOtroPueblo != value)
                {
                    cualquierHoraOtroPueblo = value;
                    OnPropertyChanged(nameof(CualquierHoraOtroPueblo));
                }
            }
        }
        private string nombreImpresora2;
        public string NombreImpresora2
        {
            get
            {
                return nombreImpresora2;
            }
            set
            {
                if (nombreImpresora2 != value)
                {
                    nombreImpresora2 = value;
                    OnPropertyChanged(nameof(NombreImpresora2));
                }
            }
        }
        private string nombreImpresora3;
        public string NombreImpresora3
        {
            get
            {
                return nombreImpresora3;
            }
            set
            {
                if (nombreImpresora3 != value)
                {
                    nombreImpresora3 = value;
                    OnPropertyChanged(nameof(NombreImpresora3));
                }
            }
        }
        private string nombreImpresora4;
        public string NombreImpresora4
        {
            get
            {
                return nombreImpresora4;
            }
            set
            {
                if (nombreImpresora4 != value)
                {
                    nombreImpresora4 = value;
                    OnPropertyChanged(nameof(NombreImpresora4));
                }
            }
        }
        private string nombreImpresora5;
        public string NombreImpresora5
        {
            get
            {
                return nombreImpresora5;
            }
            set
            {
                if (nombreImpresora5 != value)
                {
                    nombreImpresora5 = value;
                    OnPropertyChanged(nameof(NombreImpresora5));
                }
            }
        }
        private string nombreImpresora6;
        public string NombreImpresora6
        {
            get
            {
                return nombreImpresora6;
            }
            set
            {
                if (nombreImpresora6 != value)
                {
                    nombreImpresora6 = value;
                    OnPropertyChanged(nameof(NombreImpresora6));
                }
            }
        }
        private string nombreImpresora7;
        public string NombreImpresora7
        {
            get
            {
                return nombreImpresora7;
            }
            set
            {
                if (nombreImpresora7 != value)
                {
                    nombreImpresora7 = value;
                    OnPropertyChanged(nameof(NombreImpresora7));
                }
            }
        }
        private string nombreImpresora8;
        public string NombreImpresora8
        {
            get
            {
                return nombreImpresora8;
            }
            set
            {
                if (nombreImpresora8 != value)
                {
                    nombreImpresora8 = value;
                    OnPropertyChanged(nameof(NombreImpresora8));
                }
            }
        }
        private string nombreImpresora9;
        public string NombreImpresora9
        {
            get
            {
                return nombreImpresora9;
            }
            set
            {
                if (nombreImpresora9 != value)
                {
                    nombreImpresora9 = value;
                    OnPropertyChanged(nameof(NombreImpresora9));
                }
            }
        }
        private string nombreImpresora10;
        public string NombreImpresora10
        {
            get
            {
                return nombreImpresora10;
            }
            set
            {
                if (nombreImpresora10 != value)
                {
                    nombreImpresora10 = value;
                    OnPropertyChanged(nameof(NombreImpresora10));
                }
            }
        }
        private ObservableCollection<TiempoEntregaComercioModel> tiemposComercios;
        public ObservableCollection<TiempoEntregaComercioModel> TiemposComercios
        {
            get
            {
                return tiemposComercios;
            }
            set
            {
                if (tiemposComercios != value)
                {
                    tiemposComercios = value;
                    OnPropertyChanged(nameof(TiemposComercios));
                }
            }
        }
        private TiempoEntregaComercioModel tiempoComercio;
        public TiempoEntregaComercioModel TiempoComercio
        {
            get
            {
                return tiempoComercio;
            }
            set
            {
                if (tiempoComercio != value)
                {
                    tiempoComercio = value;
                    OnPropertyChanged(nameof(TiempoComercio));
                }
            }
        }
        private bool repartoComercioM;
        public bool RepartoComercioM
        {
            get
            {
                return repartoComercioM;
            }
            set
            {
                if (repartoComercioM != value)
                {
                    repartoComercioM = value;
                    OnPropertyChanged(nameof(RepartoComercioM));
                }
            }
        }
        private bool repartoComercioT;
        public bool RepartoComercioT
        {
            get
            {
                return repartoComercioT;
            }
            set
            {
                if (repartoComercioT != value)
                {
                    repartoComercioT = value;
                    OnPropertyChanged(nameof(RepartoComercioT));
                }
            }
        }
        private bool aceptaEncargos;
        public bool AceptaEncargos
        {
            get
            {
                return aceptaEncargos;
            }
            set
            {
                if (aceptaEncargos != value)
                {
                    aceptaEncargos = value;
                    OnPropertyChanged(nameof(AceptaEncargos));
                }
            }
        }
        private int encargosPorHora;
        public int EncargosPorHora
        {
            get
            {
                return encargosPorHora;
            }
            set
            {
                if (encargosPorHora != value)
                {
                    encargosPorHora = value;
                    OnPropertyChanged(nameof(EncargosPorHora));
                }
            }
        }
        private int encargosDiasDesde;
        public int EncargosDiasDesde
        {
            get
            {
                return encargosDiasDesde;
            }
            set
            {
                if (encargosDiasDesde != value)
                {
                    encargosDiasDesde = value;
                    OnPropertyChanged(nameof(EncargosDiasDesde));
                }
            }
        }
        private int encargosDiasHasta;
        public int EncargosDiasHasta
        {
            get
            {
                return encargosDiasHasta;
            }
            set
            {
                if (encargosDiasHasta != value)
                {
                    encargosDiasHasta = value;
                    OnPropertyChanged(nameof(EncargosDiasHasta));
                }
            }
        }
        private bool sistemaPuntos;
        public bool SistemaPuntos
        {
            get
            {
                return sistemaPuntos;
            }
            set
            {
                if (sistemaPuntos != value)
                {
                    sistemaPuntos = value;
                    OnPropertyChanged(nameof(SistemaPuntos));
                }
            }
        }
        private int puntosPorPedido;
        public int PuntosPorPedido
        {
            get
            {
                return puntosPorPedido;
            }
            set
            {
                if (puntosPorPedido != value)
                {
                    puntosPorPedido = value;
                    OnPropertyChanged(nameof(PuntosPorPedido));
                }
            }
        }
        private int puntosPorEuro;
        public int PuntosPorEuro
        {
            get
            {
                return puntosPorEuro;
            }
            set
            {
                if (puntosPorEuro != value)
                {
                    puntosPorEuro = value;
                    OnPropertyChanged(nameof(PuntosPorEuro));
                }
            }
        }
        private string textoPuntos;
        public string TextoPuntos
        {
            get
            {
                return textoPuntos;
            }
            set
            {
                if (textoPuntos != value)
                {
                    textoPuntos = value;
                    OnPropertyChanged(nameof(TextoPuntos));
                }
            }
        }
        private bool escaparate;
        public bool Escaparate
        {
            get { return escaparate; }
            set
            {
                escaparate = value;
                OnPropertyChanged(nameof(Escaparate));
            }
        }
        private bool repartoPropio;
        public bool RepartoPropio
        {
            get { return repartoPropio; }
            set
            {
                repartoPropio = value;
                OnPropertyChanged(nameof(RepartoPropio));
            }
        }
        private bool repartoPolloAndaluz;
        public bool RepartoPolloAndaluz
        {
            get { return repartoPolloAndaluz; }
            set
            {
                repartoPolloAndaluz = value;
                OnPropertyChanged(nameof(RepartoPolloAndaluz));
            }
        }
        private bool esAdmin;
        public bool EsAdmin
        {
            get { return esAdmin; }
            set
            {
                esAdmin = value;
                OnPropertyChanged(nameof(EsAdmin));
            }
        }
        private bool esComercio;
        public bool EsComercio
        {
            get { return esComercio; }
            set
            {
                esComercio = value;
                OnPropertyChanged(nameof(EsComercio));
            }
        }
        private bool detalle;
        public bool Detalle
        {
            get { return detalle; }
            set
            {
                detalle = value;
                OnPropertyChanged(nameof(Detalle));
            }
        }
        private bool servicioActivo;
        public bool ServicioActivo
        {
            get { return servicioActivo; }
            set
            {
                servicioActivo = value;
                OnPropertyChanged(nameof(ServicioActivo));
            }
        }

        private int tiempoEntrega;
        public int TiempoEntrega
        {
            get { return tiempoEntrega; }
            set
            {
                tiempoEntrega = value;
                OnPropertyChanged(nameof(TiempoEntrega));
            }
        }

        private string pedidoMinimo;
        public string PedidoMinimo
        {
            get { return pedidoMinimo; }
            set
            {
                pedidoMinimo = value;
                OnPropertyChanged(nameof(PedidoMinimo));
            }
        }

        private string comision;
        public string Comision
        {
            get { return comision; }
            set
            {
                comision = value;
                OnPropertyChanged(nameof(Comision));
            }
        }
        private string comisionAutoPedido;
        public string ComisionAutoPedido
        {
            get { return comisionAutoPedido; }
            set
            {
                comisionAutoPedido = value;
                OnPropertyChanged(nameof(ComisionAutoPedido));
            }
        }
        private string comisionRecogida;
        public string ComisionRecogida
        {
            get { return comisionRecogida; }
            set
            {
                comisionRecogida = value;
                OnPropertyChanged(nameof(ComisionRecogida));
            }
        }
        private string gastosEnvioPropio;
        public string GastosEnvioPropio
        {
            get { return gastosEnvioPropio; }
            set
            {
                gastosEnvioPropio = value;
                OnPropertyChanged(nameof(GastosEnvioPropio));
            }
        }
        private string comisionLocal;
        public string ComisionLocal
        {
            get { return comisionLocal; }
            set
            {
                comisionLocal = value;
                OnPropertyChanged(nameof(ComisionLocal));
            }
        }
        private string comisionReparto;
        public string ComisionReparto
        {
            get { return comisionReparto; }
            set
            {
                comisionReparto = value;
                OnPropertyChanged(nameof(ComisionReparto));
            }
        }
        private string cuotaFija;
        public string CuotaFija
        {
            get { return cuotaFija; }
            set
            {
                cuotaFija = value;
                OnPropertyChanged(nameof(CuotaFija));
            }
        }
        private string otrasCuotas;
        public string OtrasCuotas
        {
            get { return otrasCuotas; }
            set
            {
                otrasCuotas = value;
                OnPropertyChanged(nameof(OtrasCuotas));
            }
        }

        private int ticketSize;
        public int TicketSize
        {
            get { return ticketSize; }
            set
            {
                ticketSize = value;
                OnPropertyChanged(nameof(TicketSize));
            }
        }
        private TimeSpan horaSoloTarjetaDesde;
        public TimeSpan HoraSoloTarjetaDesde
        {
            get { return horaSoloTarjetaDesde; }
            set
            {
                horaSoloTarjetaDesde = value;
                OnPropertyChanged(nameof(HoraSoloTarjetaDesde));
            }
        }
        private TimeSpan horaSoloTarjetaHasta;
        public TimeSpan HoraSoloTarjetaHasta
        {
            get { return horaSoloTarjetaHasta; }
            set
            {
                horaSoloTarjetaHasta = value;
                OnPropertyChanged(nameof(HoraSoloTarjetaHasta));
            }
        }
        private bool activaSoloTarjeta;
        public bool ActivaSoloTarjeta
        {
            get { return activaSoloTarjeta; }
            set
            {
                activaSoloTarjeta = value;
                OnPropertyChanged(nameof(ActivaSoloTarjeta));
            }
        }
        private string textoIngredientes;
        public string TextoIngredientes
        {
            get { return textoIngredientes; }
            set
            {
                textoIngredientes = value;
                OnPropertyChanged(nameof(TextoIngredientes));
            }
        }
        private string textoIngredientes_eng;
        public string TextoIngredientes_eng
        {
            get { return textoIngredientes_eng; }
            set
            {
                textoIngredientes_eng = value;
                OnPropertyChanged(nameof(TextoIngredientes_eng));
            }
        }
        private string textoIngredientes_ger;
        public string TextoIngredientes_ger
        {
            get { return textoIngredientes_ger; }
            set
            {
                textoIngredientes_ger = value;
                OnPropertyChanged(nameof(TextoIngredientes_ger));
            }
        }
        private string textoIngredientes_fr;
        public string TextoIngredientes_fr
        {
            get { return textoIngredientes_fr; }
            set
            {
                textoIngredientes_fr = value;
                OnPropertyChanged(nameof(TextoIngredientes_fr));
            }
        }
        private ObservableCollection<string> visibilidades;
        public ObservableCollection<string> Visibilidades
        {
            get
            {
                return visibilidades;
            }
            set
            {
                if (visibilidades != value)
                {
                    visibilidades = value;
                    OnPropertyChanged(nameof(Visibilidades));
                }
            }
        }
        private ObservableCollection<TipoAutoPedidoModel> tiposAutoPedido;
        public ObservableCollection<TipoAutoPedidoModel> TiposAutoPedido
        {
            get
            {
                return tiposAutoPedido;
            }
            set
            {
                if (tiposAutoPedido != value)
                {
                    tiposAutoPedido = value;
                    OnPropertyChanged(nameof(TiposAutoPedido));
                }
            }
        }
        private TipoAutoPedidoModel tipoAutoPedido;
        public TipoAutoPedidoModel TipoAutoPedido
        {
            get
            {
                return tipoAutoPedido;
            }
            set
            {
                if (tipoAutoPedido != value)
                {
                    tipoAutoPedido = value;
                    OnPropertyChanged(nameof(TipoAutoPedido));
                }
            }
        }
        private ObservableCollection<EstadoPedidoModel> estadosPedido;
        public ObservableCollection<EstadoPedidoModel> EstadosPedido
        {
            get
            {
                return estadosPedido;
            }
            set
            {
                if (estadosPedido != value)
                {
                    estadosPedido = value;
                    OnPropertyChanged(nameof(EstadosPedido));
                }
            }
        }
        private EstadoPedidoModel estadoPedido;
        public EstadoPedidoModel EstadoPedido
        {
            get
            {
                return estadoPedido;
            }
            set
            {
                if (estadoPedido != value)
                {
                    estadoPedido = value;
                    OnPropertyChanged(nameof(EstadoPedido));
                }
            }
        }
        private int visibilidadHoras=-1;
        public int VisibilidadHoras
        {
            get
            {
                return visibilidadHoras;
            }
            set
            {
                if (visibilidadHoras != value)
                {
                    visibilidadHoras = value;
                    OnPropertyChanged(nameof(VisibilidadHoras));
                }
            }
        }

        private string gastosEnvio;
        public string GastosEnvio
        {
            get { return gastosEnvio; }
            set
            {
                gastosEnvio = value;
                OnPropertyChanged(nameof(GastosEnvio));
            }
        }
        private string precioBolsa;
        public string PrecioBolsa
        {
            get { return precioBolsa; }
            set
            {
                precioBolsa = value;
                OnPropertyChanged(nameof(PrecioBolsa));
            }
        }
        private string rangoBolsa;
        public string RangoBolsa
        {
            get { return rangoBolsa; }
            set
            {
                rangoBolsa = value;
                OnPropertyChanged(nameof(RangoBolsa));
            }
        }

        private string textoCerrado;
        public string TextoCerrado
        {
            get { return textoCerrado; }
            set
            {
                textoCerrado = value;
                OnPropertyChanged(nameof(TextoCerrado));
            }
        }
        private bool efectivo;
        public bool Efectivo
        {
            get
            {
                return efectivo;
            }
            set
            {
                if (efectivo != value)
                {
                    efectivo = value;
                    OnPropertyChanged(nameof(Efectivo));
                }
            }
        }
        private bool datafono;
        public bool Datafono
        {
            get
            {
                return datafono;
            }
            set
            {
                if (datafono != value)
                {
                    datafono = value;
                    OnPropertyChanged(nameof(Datafono));
                }
            }
        }
        private bool tarjeta;
        public bool Tarjeta
        {
            get
            {
                return tarjeta;
            }
            set
            {
                if (tarjeta != value)
                {
                    tarjeta = value;
                    OnPropertyChanged(nameof(Tarjeta));
                }
            }
        }
        private string iban;
        public string IBAN
        {
            get
            {
                return iban;
            }
            set
            {
                if (iban != value)
                {
                    iban = value;
                    OnPropertyChanged(nameof(IBAN));
                }
            }
        }
        private string nombreTicker;
        public string NombreTicket
        {
            get
            {
                return nombreTicker;
            }
            set
            {
                if (nombreTicker != value)
                {
                    nombreTicker = value;
                    OnPropertyChanged(nameof(NombreTicket));
                }
            }
        }
        private string cifTicket;
        public string CIFTicket
        {
            get
            {
                return cifTicket;
            }
            set
            {
                if (cifTicket != value)
                {
                    cifTicket = value;
                    OnPropertyChanged(nameof(CIFTicket));
                }
            }
        }
        private string direccionTicket;
        public string DireccionTicket
        {
            get
            {
                return direccionTicket;
            }
            set
            {
                if (direccionTicket != value)
                {
                    direccionTicket = value;
                    OnPropertyChanged(nameof(DireccionTicket));
                }
            }
        }
        private string telefonoTicker;
        public string TelefonoTicket
        {
            get
            {
                return telefonoTicker;
            }
            set
            {
                if (telefonoTicker != value)
                {
                    telefonoTicker = value;
                    OnPropertyChanged(nameof(TelefonoTicket));
                }
            }
        }
        #endregion
    }
}
