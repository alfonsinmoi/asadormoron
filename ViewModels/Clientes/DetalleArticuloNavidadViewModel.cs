using AsadorMoron.Interfaces;
using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using AsadorMoron.Recursos;
using System.Linq;
// 
using AsadorMoron.Services;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;

namespace AsadorMoron.ViewModels.Clientes
{

    public class DetalleArticuloNavidadViewModel : ViewModelBase
    {
        public DetalleArticuloNavidadViewModel()
        {
            App.entradoEnCarta = true;
            if (App.DAUtil.NotificacionPantalla.Equals(""))
            {
                if (App.userdialog == null)
                {
                    try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                }
            }
        }
        public bool esInicio = true;
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                Idioma = App.idioma;
                if (navigationData != null)
                {
                    Articulo = (ArticuloModel)navigationData;
                    Combo = Articulo.idArticulo == 11806;
                    if (Combo)
                    {
                        TieneOpciones = true;
                        Combo1 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 1));
                        Combo2 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 2));
                        Combo3 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 3));
                        Combo4 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 4));
                        Combo5 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 5));
                        Combo6 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 6));
                        Combo7 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 7));
                        Combo8 = new ObservableCollection<ComboModel>(App.combos.Where(p => p.tipo == 8));
                        Combo1.First().Seleccionado = true;
                        Combo2.First().Seleccionado = true;
                        Combo3.First().Seleccionado = true;
                        Combo4.First().Seleccionado = true;
                        Combo5.First().Seleccionado = true;
                        Combo6.First().Seleccionado = true;
                        Combo7.First().Seleccionado = true;
                        Combo8.First().Seleccionado = true;
                    }
                    carrito = App.DAUtil.Getcarrito();
                    Cantidad = carrito.Count().ToString();
                    foreach (CarritoModel c in carrito)
                    {
                        Cantidad = (int.Parse(Cantidad) + c.cantidad).ToString();
                    }
                    Articulo.listadoIngredientes = new ObservableCollection<IngredienteProductoModel>(await App.DAUtil.GetIngredienteProducto(Articulo.idArticulo));
                    cantidadIngredientes = Articulo.numeroIngredientes;
                    cantidadActual = 0;
                    if (App.DAUtil.Usuario != null)
                    {
                        if (App.EstActual.configuracion == null)
                            App.EstActual.configuracion = ResponseServiceWS.getConfiguracionEstablecimiento(App.EstActual.idEstablecimiento);
                        ModoTienda = App.EstActual.configuracion.modoEscaparate;
                        SistemaPuntos = App.EstActual.configuracion.sistemaPuntos;
                        if (SistemaPuntos)
                        {
                            Puntos = ResponseServiceWS.getPuntosEstablecimiento();
                            foreach (CarritoModel c in carrito.Where(p => p.porPuntos == 1).ToList())
                            {
                                Puntos -= c.puntos;
                            }
                        }
                        else
                            Puntos = 0;
                        if (Puntos <= 0)
                            VisiblePuntos = false;
                        ModoTienda = !App.EstActual.configuracion.modoEscaparate;
                        TextoIngredientes = App.EstActual.configuracion.textoIngredientes;
                        if (App.EstActual.horario.Equals(AppResources.Cerrado))
                            ModoTienda = false;
                    }
                    else
                    {
                        ModoTienda = false;
                        Articulo.listadoIngredientes = new ObservableCollection<IngredienteProductoModel>();
                    }
                    ListadoIngredientes = new ObservableCollection<IngredienteProductoBindableModel>();

                    Total = Articulo.precio;
                    TotalPuntos = Articulo.puntos;
                    CantidadOpcion = 1;
                    TextoCaja =AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                    TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                    if (Articulo.listadoOpciones.Count > 0)
                    {
                        OpcionSeleccionada = Articulo.listadoOpciones[0];
                        Articulo.listadoOpciones[0].seleccionado = true;
                    }

                    foreach (OpcionesModel op in Articulo.listadoOpciones)
                    {
                        try
                        {
                            op.cantidad = carrito.Count > 0 ? carrito.Find(pr => pr.idArticulo == Articulo.idArticulo && pr.opcion == op.id).cantidad : 0;
                            if (TotalPuntos + op.puntos <= Puntos && SistemaPuntos && TotalPuntos > 0)
                                op.visiblePuntos = true;
                            else
                                op.visiblePuntos = false;
                        }
                        catch (Exception)
                        {
                            op.cantidad = 0;
                            op.visiblePuntos = false;
                        }
                    }
                    foreach (IngredienteProductoModel l in Articulo.listadoIngredientes)
                    {
                        IngredienteProductoBindableModel ll = new IngredienteProductoBindableModel();
                        ll.Cantidad = 0;
                        ll.id = l.id;
                        ll.idIngrediente = l.idIngrediente;
                        ll.idProducto = l.idProducto;
                        ll.nombre = l.nombre;
                        ll.nombre_eng = l.nombre_eng;
                        ll.nombre_ger = l.nombre_ger;
                        ll.nombre_fr = l.nombre_fr;
                        ll.precio = l.precio;
                        ll.puntos = l.puntos;
                        if (TotalPuntos+ll.puntos <= Puntos && SistemaPuntos && ll.puntos>0)
                            ll.visiblePuntos = true;
                        else
                            ll.visiblePuntos = false;
                        ListadoIngredientes.Add(ll);
                    }
                    TieneAlergenos = Articulo.listadoAlergenos.Count > 0;
                    TieneIngredientes = Articulo.listadoIngredientes.Count > 0;
                    TieneOpciones = Articulo.listadoOpciones.Count > 0 || Combo;
                    VisiblePuntos = TotalPuntos <= Puntos && SistemaPuntos && TotalPuntos>0;
                }
                esInicio = false;
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }

        #region Metodos

        private void ActualizarCaja()
        {
            try
            {
                if (OpcionSeleccionada == null && Articulo.listadoOpciones.Count > 0)
                {
                    App.customDialog.ShowDialogAsync(AppResources.SeleccioneOpcion, AppResources.App, AppResources.Cerrar);
                }
                else if (CantidadOpcion == 0)
                {
                    App.customDialog.ShowDialogAsync(AppResources.IndiqueCantidad, AppResources.App, AppResources.Cerrar);
                }else if (cantidadIngredientes != cantidadActual && Articulo.fuerzaIngredientes==1)
                {
                    App.customDialog.ShowDialogAsync(AppResources.IndiqueNumeroIngredientes, AppResources.App, AppResources.Cerrar);
                }
                else
                {
                    CarritoModel c = new CarritoModel();
                    c.id = -1;
                    c.cantidad = CantidadOpcion;
                    if (!Combo)
                    {
                        if (OpcionSeleccionada != null)
                        {
                            c.comida = Articulo.nombre + " (" + OpcionSeleccionada.opcion + ")";
                            c.comida_eng = Articulo.nombre_eng + " (" + OpcionSeleccionada.opcion_eng + ")";
                            c.comida_ger = Articulo.nombre_ger + " (" + OpcionSeleccionada.opcion_ger + ")";
                            c.comida_fr = Articulo.nombre_eng + " (" + OpcionSeleccionada.opcion_fr + ")";
                        }
                        else
                        {
                            c.comida = Articulo.nombre;
                            c.comida_eng = Articulo.nombre_eng;
                            c.comida_ger = Articulo.nombre_ger;
                            c.comida_fr = Articulo.nombre_fr;
                        }
                    }
                    else
                    {
                        c.comida = Combo1.First(p => p.Seleccionado == true).nombre + ", " + Combo2.First(p => p.Seleccionado == true).nombre + ", " + Combo3.First(p => p.Seleccionado == true).nombre + ", ";
                        c.comida += Combo4.First(p => p.Seleccionado == true).nombre + ", " + Combo5.First(p => p.Seleccionado == true).nombre + ", " + Combo6.First(p => p.Seleccionado == true).nombre + ", ";
                        c.comida += Combo7.First(p => p.Seleccionado == true).nombre + " y " + Combo8.First(p => p.Seleccionado == true).nombre + ", ";
                        c.comida = c.comida.ToUpper();
                        c.comida_eng = c.comida;
                        c.comida_ger = c.comida;
                        c.comida_fr = c.comida;
                    }
                    foreach (IngredienteProductoBindableModel p in ListadoIngredientes)
                    {
                        if (p.Cantidad > 0)
                        {
                            c.comida += Environment.NewLine + p.Cantidad + " x " + p.nombre;
                            c.comida_eng += Environment.NewLine + p.Cantidad + " x " + p.nombre_eng;
                            c.comida_ger += Environment.NewLine + p.Cantidad + " x " + p.nombre_ger;
                            c.comida_fr += Environment.NewLine + p.Cantidad + " x " + p.nombre_fr;
                        }
                    }
                    c.porEncargo = Articulo.porEncargo;
                    c.idEstablecimiento = Articulo.idEstablecimiento;
                    c.idArticulo = Articulo.idArticulo;
                    c.imagen = Articulo.imagen;
                    c.observaciones = "";
                    c.comentario = Comentario;
                    c.precio = Total / c.cantidad;
                    c.puntos = 0;
                    c.porPuntos = 0;
                    if (OpcionSeleccionada != null)
                        c.opcion = opcionSeleccionada.id;
                    else
                        c.opcion = 0;

                    carrito.Add(c);
                    App.DAUtil.ActualizaCarrito(carrito);
                    /*MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await App.DAUtil.NavigationService.NavigateBackAsync();
                    });*/
                    App.DAUtil.NavigationService.NavigateBackAsync();
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void ActualizarCajaPuntos()
        {
            try
            {
                if (OpcionSeleccionada == null && Articulo.listadoOpciones.Count > 0)
                {
                    App.customDialog.ShowDialogAsync(AppResources.SeleccioneOpcion, AppResources.App, AppResources.Cerrar);
                }
                else if (CantidadOpcion == 0)
                {
                    App.customDialog.ShowDialogAsync(AppResources.IndiqueCantidad, AppResources.App, AppResources.Cerrar);
                }
                else if (cantidadIngredientes != cantidadActual && Articulo.fuerzaIngredientes == 1)
                {
                    App.customDialog.ShowDialogAsync(AppResources.IndiqueNumeroIngredientes, AppResources.App, AppResources.Cerrar);
                }
                else if (Articulo.puntos>Puntos)
                    App.customDialog.ShowDialogAsync("No tiene suficientes puntos", AppResources.App, AppResources.Cerrar);
                else
                {
                    CarritoModel c = new CarritoModel();
                    c.id = -1;
                    c.cantidad = CantidadOpcion;
                    if (OpcionSeleccionada != null)
                    {
                        c.comida = Articulo.nombre + " (" + OpcionSeleccionada.opcion + ")";
                        c.comida_eng = Articulo.nombre_eng + " (" + OpcionSeleccionada.opcion_eng + ")";
                        c.comida_ger = Articulo.nombre_ger + " (" + OpcionSeleccionada.opcion_ger + ")";
                        c.comida_fr = Articulo.nombre_eng + " (" + OpcionSeleccionada.opcion_fr + ")";
                    }
                    else
                    {
                        c.comida = Articulo.nombre;
                        c.comida_eng = Articulo.nombre_eng;
                        c.comida_ger = Articulo.nombre_ger;
                        c.comida_fr = Articulo.nombre_fr;
                    }
                    foreach (IngredienteProductoBindableModel p in ListadoIngredientes)
                    {
                        if (p.Cantidad > 0)
                        {
                            c.comida += Environment.NewLine + p.Cantidad + " x " + p.nombre;
                            c.comida_eng += Environment.NewLine + p.Cantidad + " x " + p.nombre_eng;
                            c.comida_ger += Environment.NewLine + p.Cantidad + " x " + p.nombre_ger;
                            c.comida_fr += Environment.NewLine + p.Cantidad + " x " + p.nombre_fr;
                        }
                    }
                    c.porEncargo = Articulo.porEncargo;
                    c.idEstablecimiento = Articulo.idEstablecimiento;
                    c.idArticulo = Articulo.idArticulo;
                    c.imagen = Articulo.imagen;
                    c.observaciones = "";
                    c.comentario = Comentario;
                    c.puntos = TotalPuntos;
                    c.porPuntos = 1;
                    c.precio = Total / c.cantidad;
                    if (OpcionSeleccionada != null)
                        c.opcion = opcionSeleccionada.id;
                    else
                        c.opcion = 0;

                    carrito.Add(c);
                    App.DAUtil.ActualizaCarrito(carrito);
                    App.DAUtil.NavigationService.NavigateBackAsync();
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }

        private void IrDetallePedido()
        {
            try
            {
                if (carrito.Count > 0)
                {
                    try { App.userdialog.ShowLoading(AppResources.Cargando); } catch (Exception) { App.userdialog.HideLoading(); }
                    MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            App.DAUtil.Idioma = "ES";
                            await App.DAUtil.NavigationService.NavigateToAsync<DetallePedidoViewModel>(carrito);
                        });
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }

        private void add(object parametro)
        {
            try
            {
                CantidadOpcion++;
                if (OpcionSeleccionada != null)
                {
                    Total = App.ParseaPrecio(OpcionSeleccionada.precio) * cantidadOpcion;
                    TotalPuntos = OpcionSeleccionada.puntos * cantidadOpcion;
                    foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                    {
                        Total += l.precio * l.Cantidad * CantidadOpcion;
                        TotalPuntos += CantidadOpcion * l.puntos;
                    }
                    TextoCaja =AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                    TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                }
                else
                {
                    Total = Articulo.precio * cantidadOpcion;
                    TotalPuntos = Articulo.puntos * cantidadOpcion;
                    foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                    {
                        Total += l.precio * l.Cantidad * CantidadOpcion;
                        TotalPuntos += CantidadOpcion * l.puntos;
                    }
                    TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                    TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                }
                VisiblePuntos = TotalPuntos <= Puntos && SistemaPuntos && TotalPuntos > 0;
            }
            catch (Exception ex)
            {
                // 
            }
        }

        private void addIng(object parametro)
        {
            try
            {
                int idIngrediente = (int)parametro;
                if (cantidadIngredientes == 0 || cantidadIngredientes > cantidadActual)
                {
                    IngredienteProductoBindableModel opt = ListadoIngredientes.Where((obj) => obj.idIngrediente == idIngrediente).FirstOrDefault();
                    opt.Cantidad++;
                    cantidadActual++;
                    if (OpcionSeleccionada != null)
                    {
                        Total = App.ParseaPrecio(OpcionSeleccionada.precio) * CantidadOpcion;
                        TotalPuntos = OpcionSeleccionada.puntos * CantidadOpcion;
                        foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                        {
                            Total += l.precio * l.Cantidad * CantidadOpcion;
                            TotalPuntos += l.puntos * CantidadOpcion;
                        }
                        TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                        TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                    }
                    else
                    {
                        Total = Articulo.precio * cantidadOpcion;
                        TotalPuntos = Articulo.puntos * cantidadOpcion;
                        foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                        {
                            Total += l.precio * l.Cantidad * CantidadOpcion;
                            TotalPuntos += l.puntos * l.Cantidad * CantidadOpcion;
                        }
                        TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                        TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                    }
                    VisiblePuntos = TotalPuntos <= Puntos && SistemaPuntos && TotalPuntos > 0;
                }
                else if (cantidadIngredientes > 0)
                {
                    App.customDialog.ShowDialogAsync(AppResources.NoPuedeSeleccionarMas + TextoIngredientes, AppResources.App, AppResources.Cerrar);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void removeIng(object parametro)
        {
            try
            {
                int idIngrediente = (int)parametro;
                IngredienteProductoBindableModel opt = ListadoIngredientes.Where((obj) => obj.idIngrediente == idIngrediente).FirstOrDefault();
                if (opt.Cantidad > 0)
                    opt.Cantidad--;
                if (cantidadActual > 0)
                    cantidadActual--;
                if (OpcionSeleccionada != null)
                {
                    Total = App.ParseaPrecio(OpcionSeleccionada.precio) * CantidadOpcion;
                    TotalPuntos = OpcionSeleccionada.puntos * CantidadOpcion;
                    foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                    {
                        Total += l.precio * l.Cantidad * CantidadOpcion;
                        TotalPuntos += l.puntos * l.Cantidad * CantidadOpcion;
                    }
                    TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                    TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                }
                else
                {
                    Total = Articulo.precio * cantidadOpcion;
                    TotalPuntos = Articulo.puntos * cantidadOpcion;
                    foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                    {
                        Total += l.precio * l.Cantidad * CantidadOpcion;
                        TotalPuntos += l.puntos * l.Cantidad * CantidadOpcion;
                    }
                    TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                    TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                }
                VisiblePuntos = TotalPuntos <= Puntos && SistemaPuntos && TotalPuntos > 0;
            }
            catch (Exception ex)
            {
                // 
            }
        }

        private void remove(object parametro)
        {
            try
            {
                if (CantidadOpcion > 0)
                    CantidadOpcion--;

                if (OpcionSeleccionada != null)
                {
                    Total = App.ParseaPrecio(OpcionSeleccionada.precio) * CantidadOpcion;
                    TotalPuntos = OpcionSeleccionada.puntos * CantidadOpcion;
                    foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                    {
                        Total += l.precio * l.Cantidad * CantidadOpcion;
                        TotalPuntos += l.puntos * l.Cantidad * CantidadOpcion;
                    }
                    TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                    TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                }
                else
                {
                    Total = Articulo.precio * cantidadOpcion;
                    TotalPuntos = Articulo.puntos * cantidadOpcion;
                    foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                    {
                        Total += l.precio * l.Cantidad * CantidadOpcion;
                        TotalPuntos += l.puntos * l.Cantidad * CantidadOpcion;
                    }
                    TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                    TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                }
                VisiblePuntos = TotalPuntos <= Puntos && SistemaPuntos && TotalPuntos > 0;
            }
            catch (Exception ex)
            {
                // 
            }
        }

        #endregion

        #region Comandos

        public ICommand cmdActualizarCaja { get { return new Command(ActualizarCaja); } }
        public ICommand cmdActualizarCajaPuntos { get { return new Command(ActualizarCajaPuntos); } }
        public ICommand IrDetallePedidoCommand { get { return new Command(IrDetallePedido); } }
        public ICommand ClickMasCommand { get { return new Command((parametro) => add(parametro)); } }
        public ICommand ClickMasIngCommand { get { return new Command((parametro) => addIng(parametro)); } }
        public ICommand ClickMenosIngCommand { get { return new Command((parametro) => removeIng(parametro)); } }
        public ICommand ClickMenosCommand { get { return new Command((parametro) => remove(parametro)); } }

        #endregion

        #region Propiedades


        List<CarritoModel> carrito = new List<CarritoModel>();
        private ArticuloModel articulo;
        public ArticuloModel Articulo
        {
            get
            {
                return articulo;
            }
            set
            {
                if (articulo != value)
                {
                    articulo = value;
                    OnPropertyChanged(nameof(Articulo));
                }
            }
        }
        private bool combo;
        public bool Combo
        {
            get
            {
                return combo;
            }
            set
            {
                if (combo != value)
                {
                    combo = value;
                    OnPropertyChanged(nameof(Combo));
                }
            }
        }
        private int cantidadIngredientes = 0;
        private int cantidadActual = 0;
        private bool tieneOpcionSeleccionada;
        public bool TieneOpcionSeleccionada
        {
            get
            {
                return tieneOpcionSeleccionada;
            }
            set
            {
                if (tieneOpcionSeleccionada != value)
                {
                    tieneOpcionSeleccionada = value;
                    OnPropertyChanged(nameof(TieneOpcionSeleccionada));
                }
            }
        }
        private bool modoTienda;
        public bool ModoTienda
        {
            get { return modoTienda; }
            set
            {
                if (modoTienda != value)
                {
                    modoTienda = value;
                    OnPropertyChanged(nameof(ModoTienda));
                }
            }
        }
        private bool visiblePuntos;
        public bool VisiblePuntos
        {
            get { return visiblePuntos; }
            set
            {
                if (visiblePuntos != value)
                {
                    visiblePuntos = value;
                    OnPropertyChanged(nameof(VisiblePuntos));
                }
            }
        }
        private bool sistemaPuntos;
        public bool SistemaPuntos
        {
            get { return sistemaPuntos; }
            set
            {
                if (sistemaPuntos != value)
                {
                    sistemaPuntos = value;
                    OnPropertyChanged(nameof(SistemaPuntos));
                }
            }
        }
        private int puntos;
        public int Puntos
        {
            get { return puntos; }
            set
            {
                if (puntos != value)
                {
                    puntos = value;
                    OnPropertyChanged(nameof(Puntos));
                }
            }
        }
        private string textoIngredientes;
        public string TextoIngredientes
        {
            get { return textoIngredientes; }
            set
            {
                if (textoIngredientes != value)
                {
                    textoIngredientes = value;
                    OnPropertyChanged(nameof(TextoIngredientes));
                }
            }
        }
        private bool tieneOpciones;
        public bool TieneOpciones
        {
            get { return tieneOpciones; }
            set
            {
                if (tieneOpciones != value)
                {
                    tieneOpciones = value;
                    OnPropertyChanged(nameof(TieneOpciones));
                }
            }
        }
        private bool tieneIngredientees;
        public bool TieneIngredientes
        {
            get
            {
                return tieneIngredientees;
            }
            set
            {
                if (tieneIngredientees != value)
                {
                    tieneIngredientees = value;
                    OnPropertyChanged(nameof(TieneIngredientes));
                }
            }
        }
        private int cantidadOpcion;
        public int CantidadOpcion
        {
            get
            {
                return cantidadOpcion;
            }
            set
            {
                if (cantidadOpcion != value)
                {
                    cantidadOpcion = value;
                    OnPropertyChanged(nameof(CantidadOpcion));
                }
            }
        }

        private OpcionesModel opcionSeleccionada;
        public OpcionesModel OpcionSeleccionada
        {
            get
            {
                return opcionSeleccionada;
            }
            set
            {
                if (opcionSeleccionada != value)
                {
                    opcionSeleccionada = value;
                    OnPropertyChanged(nameof(OpcionSeleccionada));
                    if (CantidadOpcion == 0)
                        CantidadOpcion = 1;
                    try
                    {
                        Total = App.ParseaPrecio(OpcionSeleccionada.precio) * CantidadOpcion;
                        TotalPuntos = OpcionSeleccionada.puntos * CantidadOpcion;
                        foreach (IngredienteProductoBindableModel l in ListadoIngredientes)
                        {
                            Total += l.precio * l.Cantidad * CantidadOpcion;
                            TotalPuntos += l.puntos * l.Cantidad * CantidadOpcion;
                        }
                        TextoCaja = AppResources.AñadirCarrito + " (" + Total.ToString("N2") + "€)";
                        TextoCajaPuntos = AppResources.AñadirCarritoPuntos + " (" + TotalPuntos + " p.)";
                        TieneOpcionSeleccionada = true;
                    }
                    catch (Exception ex)
                    {
                        // 
                    }
                }
            }
        }
        private ObservableCollection<IngredienteProductoBindableModel> listadoIngredientes;
        public ObservableCollection<IngredienteProductoBindableModel> ListadoIngredientes
        {
            get
            {
                return listadoIngredientes;
            }
            set
            {
                if (listadoIngredientes != value)
                {
                    listadoIngredientes = value;
                    OnPropertyChanged(nameof(ListadoIngredientes));
                }
            }
        }
        private string idioma;
        public string Idioma
        {
            get
            {
                return idioma;
            }
            set
            {
                if (idioma != value)
                {
                    idioma = value;
                    OnPropertyChanged(nameof(Idioma));
                }
            }
        }
        private string textoCaja;
        public string TextoCaja
        {
            get
            {
                return textoCaja;
            }
            set
            {
                if (textoCaja != value)
                {
                    textoCaja = value;
                    OnPropertyChanged(nameof(TextoCaja));
                }
            }
        }

        private ObservableCollection<ComboModel> combo1;
        public ObservableCollection<ComboModel> Combo1
        {
            get
            {
                return combo1;
            }
            set
            {
                if (combo1 != value)
                {
                    combo1 = value;
                    OnPropertyChanged(nameof(Combo1));
                }
            }
        }
        private ObservableCollection<ComboModel> combo2;
        public ObservableCollection<ComboModel> Combo2
        {
            get
            {
                return combo2;
            }
            set
            {
                if (combo2 != value)
                {
                    combo2 = value;
                    OnPropertyChanged(nameof(Combo2));
                }
            }
        }
        private ObservableCollection<ComboModel> combo3;
        public ObservableCollection<ComboModel> Combo3
        {
            get
            {
                return combo3;
            }
            set
            {
                if (combo3 != value)
                {
                    combo3 = value;
                    OnPropertyChanged(nameof(Combo3));
                }
            }
        }
        private ObservableCollection<ComboModel> combo4;
        public ObservableCollection<ComboModel> Combo4
        {
            get
            {
                return combo4;
            }
            set
            {
                if (combo4 != value)
                {
                    combo4 = value;
                    OnPropertyChanged(nameof(Combo4));
                }
            }
        }
        private ObservableCollection<ComboModel> combo5;
        public ObservableCollection<ComboModel> Combo5
        {
            get
            {
                return combo5;
            }
            set
            {
                if (combo5 != value)
                {
                    combo5 = value;
                    OnPropertyChanged(nameof(Combo5));
                }
            }
        }
        private ObservableCollection<ComboModel> combo6;
        public ObservableCollection<ComboModel> Combo6
        {
            get
            {
                return combo6;
            }
            set
            {
                if (combo6 != value)
                {
                    combo6 = value;
                    OnPropertyChanged(nameof(Combo6));
                }
            }
        }
        private ObservableCollection<ComboModel> combo7;
        public ObservableCollection<ComboModel> Combo7
        {
            get
            {
                return combo7;
            }
            set
            {
                if (combo7 != value)
                {
                    combo7 = value;
                    OnPropertyChanged(nameof(Combo7));
                }
            }
        }
        private ObservableCollection<ComboModel> combo8;
        public ObservableCollection<ComboModel> Combo8
        {
            get
            {
                return combo8;
            }
            set
            {
                if (combo8 != value)
                {
                    combo8 = value;
                    OnPropertyChanged(nameof(Combo8));
                }
            }
        }
        private string textoCajaPuntos;
        public string TextoCajaPuntos
        {
            get
            {
                return textoCajaPuntos;
            }
            set
            {
                if (textoCajaPuntos != value)
                {
                    textoCajaPuntos = value;
                    OnPropertyChanged(nameof(TextoCajaPuntos));
                }
            }
        }

        private double total;
        public double Total
        {
            get
            {
                return total;
            }
            set
            {
                if (total != value)
                {
                    total = value;
                    OnPropertyChanged(nameof(Total));
                }
            }
        }
        private int totalPuntos;
        public int TotalPuntos
        {
            get
            {
                return totalPuntos;
            }
            set
            {
                if (totalPuntos != value)
                {
                    totalPuntos = value;
                    OnPropertyChanged(nameof(TotalPuntos));
                }
            }
        }

        private string cantidad;
        public string Cantidad
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
        private bool tieneAlergenos;
        public bool TieneAlergenos
        {
            get { return tieneAlergenos; }
            set
            {
                if (tieneAlergenos != value)
                {
                    tieneAlergenos = value;
                    OnPropertyChanged(nameof(TieneAlergenos));
                }
            }
        }
        private string comentario="";
        public string Comentario
        {
            get { return comentario; }
            set
            {
                if (comentario != value)
                {
                    comentario = value;
                    OnPropertyChanged(nameof(Comentario));
                }
            }
        }
        #endregion


    }
}