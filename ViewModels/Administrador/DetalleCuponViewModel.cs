using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsadorMoron.Interfaces;
// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.Services;
using AsadorMoron.ViewModels.Base;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Administrador
{
    public class DetalleCuponViewModel:ViewModelBase
    {
        public DetalleCuponViewModel()
        {
        }
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                try
                {
                    if (!haEntrado)
                    {
                        
                        Cupon = (CuponesModel)navigationData;
                        TiposOferta = new ObservableCollection<string>();
                        TiposOferta.Add("Descuento Fijo");
                        TiposOferta.Add("Porcentaje");
                        if (Cupon != null)
                        {
                            CodigoCupon = Cupon.codigoCupon;
                            Lunes = Cupon.lunes;
                            Martes = Cupon.martes;
                            Miercoles = Cupon.miercoles;
                            Jueves = Cupon.jueves;
                            Viernes = Cupon.viernes;
                            Sabado = Cupon.sabado;
                            Domingo = Cupon.domingo;
                            Establecimiento = Cupon.establecimiento;
                            Pueblo = Cupon.pueblo;
                            Estado = Cupon.estado;
                            FechaDesde = Cupon.fechaDesde;
                            FechaHasta = Cupon.fechaHasta;
                            Gastos = Cupon.gastos;
                            Limitado = Cupon.limitado;
                            Valor = Cupon.valor;
                            Cantidad = Cupon.cantidad;
                            TipoSeleccionado = Cupon.tipoOferta;
                        }
                        else
                        {
                            Cupon = new CuponesModel();
                            TipoSeleccionado = 1;
                            Estado = true;
                            FechaDesde = DateTime.Now;
                            FechaHasta = DateTime.Now;
                            Cupon.idPueblo = "";
                            Cupon.idEstablecimiento = "";
                        }
                        establecimientos = new List<Establecimiento>(ResponseServiceWS.getListadoEstablecimientos().OrderBy(p => p.nombre));
                    }
                    if (App.okSeleccion || !haEntrado)
                    {
                        if (Cupon != null)
                        {
                            if (App.establecimientosSeleccionados.Count == 0)
                            {
                                string[] ps = Cupon.idEstablecimiento.Split(',');
                                if (establecimientos.Count != ps.Length)
                                {
                                    foreach (Establecimiento p in establecimientos)
                                    {
                                        foreach (string p2 in ps)
                                        {
                                            if (int.Parse(p2) == p.idEstablecimiento)
                                            {
                                                if (!string.IsNullOrEmpty(EstablecimientosCupon))
                                                    EstablecimientosCupon += ", ";
                                                EstablecimientosCupon += p.nombre;
                                                App.establecimientosSeleccionados.Add(p);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    EstablecimientosCupon = AppResources.Todos;
                                    App.establecimientosSeleccionados = new ObservableCollection<Object>(establecimientos);
                                }
                            }
                            else
                            {
                                if (establecimientos.Count != App.establecimientosSeleccionados.Count)
                                {
                                    EstablecimientosCupon = "";
                                    foreach (Establecimiento p in establecimientos)
                                    {
                                        foreach (Establecimiento p2 in App.establecimientosSeleccionados)
                                        {
                                            if (p2.idEstablecimiento == p.idEstablecimiento)
                                            {
                                                if (!string.IsNullOrEmpty(EstablecimientosCupon))
                                                    EstablecimientosCupon += ", ";
                                                EstablecimientosCupon += p.nombre;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    EstablecimientosCupon = AppResources.Todos;
                                }
                            }
                        }
                    }
                    haEntrado = true;
                }
                catch (Exception)
                {
                }
                App.DAUtil.EnTimer = false;
                if (navigationData != null)
                {
                    try
                    {
                        Cupon = (CuponesModel)navigationData;
                    }
                    catch (Exception)
                    {
                    }

                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                
                // 
            }
            finally
            {
                App.userdialog.HideLoading();
            }

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }
        #region Propiedades
        private bool haEntrado = false;
        private string codigoCupon;
        public string CodigoCupon
        {
            get
            {
                return codigoCupon;
            }
            set
            {
                if (codigoCupon != value)
                {
                    codigoCupon = value;
                    OnPropertyChanged(nameof(CodigoCupon));
                }
            }
        }
        private int tipoSeleccionado=-1;
        public int TipoSeleccionado
        {
            get
            {
                return tipoSeleccionado;
            }
            set
            {
                if (tipoSeleccionado != value)
                {
                    tipoSeleccionado = value;
                    OnPropertyChanged(nameof(TipoSeleccionado));
                }
            }
        }
        private ObservableCollection<string> tiposOferta;
        public ObservableCollection<string> TiposOferta
        {
            get
            {
                return tiposOferta;
            }
            set
            {
                if (tiposOferta != value)
                {
                    tiposOferta = value;
                    OnPropertyChanged(nameof(TiposOferta));
                }
            }
        }
        private bool limitado;
        public bool Limitado
        {
            get
            {
                return limitado;
            }
            set
            {
                if (limitado != value)
                {
                    limitado = value;
                    OnPropertyChanged(nameof(Limitado));
                }
            }
        }
        private bool pueblo;
        public bool Pueblo
        {
            get
            {
                return pueblo;
            }
            set
            {
                if (pueblo != value)
                {
                    pueblo = value;
                    OnPropertyChanged(nameof(Pueblo));
                }
            }
        }
        private bool establecimiento;
        public bool Establecimiento
        {
            get
            {
                return establecimiento;
            }
            set
            {
                if (establecimiento != value)
                {
                    establecimiento = value;
                    OnPropertyChanged(nameof(Establecimiento));
                }
            }
        }
        private bool gastos;
        public bool Gastos
        {
            get
            {
                return gastos;
            }
            set
            {
                if (gastos != value)
                {
                    gastos = value;
                    OnPropertyChanged(nameof(Gastos));
                    if (Gastos == Precio)
                        Precio = !Gastos;
                }
            }
        }
        private bool precio;
        public bool Precio
        {
            get
            {
                return precio;
            }
            set
            {
                if (precio != value)
                {
                    precio = value;
                    OnPropertyChanged(nameof(Precio));
                    if (Gastos==Precio)
                        Gastos = !Precio;
                }
            }
        }
        private DateTime fechaDesde;
        public DateTime FechaDesde
        {
            get
            {
                return fechaDesde;
            }
            set
            {
                if (fechaDesde != value)
                {
                    fechaDesde = value;
                    OnPropertyChanged(nameof(FechaDesde));
                }
            }
        }
        private DateTime fechaHasta;
        public DateTime FechaHasta
        {
            get
            {
                return fechaHasta;
            }
            set
            {
                if (fechaHasta != value)
                {
                    fechaHasta = value;
                    OnPropertyChanged(nameof(FechaHasta));
                }
            }
        }
        private bool lunes;
        public bool Lunes
        {
            get
            {
                return lunes;
            }
            set
            {
                if (lunes != value)
                {
                    lunes = value;
                    OnPropertyChanged(nameof(Lunes));
                }
            }
        }
        private bool martes;
        public bool Martes
        {
            get
            {
                return martes;
            }
            set
            {
                if (martes != value)
                {
                    martes = value;
                    OnPropertyChanged(nameof(Martes));
                }
            }
        }
        private bool miercoles;
        public bool Miercoles
        {
            get
            {
                return miercoles;
            }
            set
            {
                if (miercoles != value)
                {
                    miercoles = value;
                    OnPropertyChanged(nameof(Miercoles));
                }
            }
        }
        private bool jueves;
        public bool Jueves
        {
            get
            {
                return jueves;
            }
            set
            {
                if (jueves != value)
                {
                    jueves = value;
                    OnPropertyChanged(nameof(Jueves));
                }
            }
        }
        private bool viernes;
        public bool Viernes
        {
            get
            {
                return viernes;
            }
            set
            {
                if (viernes != value)
                {
                    viernes = value;
                    OnPropertyChanged(nameof(Viernes));
                }
            }
        }
        private bool sabado;
        public bool Sabado
        {
            get
            {
                return sabado;
            }
            set
            {
                if (sabado != value)
                {
                    sabado = value;
                    OnPropertyChanged(nameof(Sabado));
                }
            }
        }
        private bool domingo;
        public bool Domingo
        {
            get
            {
                return domingo;
            }
            set
            {
                if (domingo != value)
                {
                    domingo = value;
                    OnPropertyChanged(nameof(Domingo));
                }
            }
        }
        private bool estado;
        public bool Estado
        {
            get
            {
                return estado;
            }
            set
            {
                if (estado != value)
                {
                    estado = value;
                    OnPropertyChanged(nameof(Estado));
                }
            }
        }
        private double valor;
        public double Valor
        {
            get
            {
                return valor;
            }
            set
            {
                if (valor != value)
                {
                    valor = value;
                    OnPropertyChanged(nameof(Valor));
                }
            }
        }
        private int cantidad;
        public int Cantidad
        {
            get
            {
                return cantidad;
            }
            set
            {
                if (cantidad != value)
                {
                    cantidad = value;
                    OnPropertyChanged(nameof(Cantidad));
                }
            }
        }
        private CuponesModel cupon;
        public CuponesModel Cupon
        {
            get
            {
                return cupon;
            }
            set
            {
                if (cupon != value)
                {
                    cupon = value;
                    OnPropertyChanged(nameof(Cupon));
                }
            }
        }
        private string pueblosCupon="";
        public string PueblosCupon
        {
            get
            {
                return pueblosCupon;
            }
            set
            {
                if (pueblosCupon != value)
                {
                    pueblosCupon = value;
                    OnPropertyChanged(nameof(PueblosCupon));
                }
            }
        }
        private string establecimientosCupon="";
        public string EstablecimientosCupon
        {
            get
            {
                return establecimientosCupon;
            }
            set
            {
                if (establecimientosCupon != value)
                {
                    establecimientosCupon = value;
                    OnPropertyChanged(nameof(EstablecimientosCupon));
                }
            }
        }
        private List<Establecimiento> establecimientos;
        private bool ok = false;
        #endregion
        #region Comandos
        public ICommand CommandGuardar { get { return new Command(Guardar); } }
        #endregion
        #region Métodos
        private async void Guardar()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                await Task.Delay(200);
                await Task.Run(async () => { await initGuardar(); }).ContinueWith(res => MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (ok)
                        App.DAUtil.NavigationService.NavigateBackAsync().ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
                }));
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async Task initGuardar()
        {
            try
            {
                ok = false;
                if (string.IsNullOrEmpty(CodigoCupon))
                    await App.userdialog.AlertAsync("El código del cupón es obligatorio");
                else if (FechaHasta < FechaDesde)
                    await App.userdialog.AlertAsync("La fecha desde no puede ser mayor que la fecha hasta");
                else if (Valor <= 0)
                    await App.userdialog.AlertAsync("El valor tiene que ser mayor que 0");
                else if (Limitado && Cantidad <= 0)
                    await App.userdialog.AlertAsync("La cantidad tiene que ser mayor que 0");
                else if (Pueblo & string.IsNullOrEmpty(PueblosCupon))
                    await App.userdialog.AlertAsync("Debe seleccionar los pueblos afectados");
                else if (Establecimiento & string.IsNullOrEmpty(EstablecimientosCupon))
                    await App.userdialog.AlertAsync("Debe seleccionar los establecimientos afectados");
                else
                {

                    if (Cupon == null)
                        Cupon = new CuponesModel();

                    Cupon.codigoCupon = CodigoCupon;
                    Cupon.cantidad = Cantidad;
                    Cupon.creador = 3;
                    Cupon.domingo = Domingo;
                    Cupon.establecimiento = Establecimiento;
                    Cupon.estado = Estado;
                    Cupon.fechaDesde = FechaDesde;
                    Cupon.fechaHasta = FechaHasta;
                    Cupon.gastos = Gastos;
                    Cupon.idEstablecimiento = "";
                    foreach (Establecimiento e in App.establecimientosSeleccionados)
                    {
                        if (!Cupon.idEstablecimiento.Equals(""))
                            Cupon.idEstablecimiento += ",";
                        Cupon.idEstablecimiento += e.idEstablecimiento;
                    }

                    Cupon.idGrupo = Preferences.Get("idGrupo", 0);
                    Cupon.idPueblo = App.DAUtil.Usuario.idPueblo.ToString();
                    Cupon.jueves = Jueves;
                    Cupon.limitado = Limitado;
                    Cupon.lunes = Lunes;
                    Cupon.martes = Martes;
                    Cupon.miercoles = Miercoles;
                    Cupon.pueblo = Pueblo;
                    Cupon.sabado = Sabado;
                    Cupon.tipoOferta = TipoSeleccionado;
                    Cupon.valor = Valor;
                    Cupon.viernes = Viernes;
                    if (Cupon.idProducto == null)
                        Cupon.idProducto = "";
                    if (Cupon.idCategoria == null)
                        Cupon.idCategoria = "";
                    if (Cupon.idProducto == null)
                        Cupon.idProducto = "";

                    if (App.ResponseWS.editaCupones(Cupon))
                    {
                        ok = true;
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync("El proceso ha terminado correctamente", AppResources.App, AppResources.Aceptar);
                    }
                    else
                    {
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
                    }
                }
            }
            catch (Exception ex)
            {
                // 
                await App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.SoloError, AppResources.Aceptar);
            }
            finally
            {
                App.userdialog.HideLoading();
            }
        }
        #endregion
    }
}

