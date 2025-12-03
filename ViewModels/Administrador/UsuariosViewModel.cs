using System;
using System.Threading.Tasks;
using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
using AsadorMoron.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using AsadorMoron.Recursos;
using AsadorMoron.Interfaces;
// 
using System.Linq;

namespace AsadorMoron.ViewModels.Administrador
{
    public class UsuariosViewModel : ViewModelBase
    {
        public UsuariosViewModel()
        {
        }
        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                Roles = new ObservableCollection<RolModel>();
                RolModel es = new RolModel();
                es.id = 1;
                es.nombre = AppResources.Usuario;
                Roles.Add(es);

                es = new RolModel();
                es.id = 2;
                es.nombre = AppResources.Administrador;
                Roles.Add(es);
                es = new RolModel();
                es.id = 4;
                es.nombre = AppResources.Repartidor;
                Roles.Add(es);

                Repartidores = new ObservableCollection<RepartidorModel>(App.DAUtil.GetRepartidores());
                if (Repartidores.Count() > 0)
                    RepartidorSeleccionado = Repartidores[0];

                VisibleCliente = false;
            }
            catch (Exception ex)
            {
                // 
            }
            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));

        }
        
        #region Comandos
        public ICommand cmdBuscarUsuario { get { return new Command(Buscar); } }
        public ICommand cmdModificar { get { return new Command(Modificar); } }
        #endregion
       
        #region Métodos
        private async void Buscar()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                await Task.Run(async () => { await initBuscar(); }).ContinueWith(res => MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (resultadoBusqueda != null && resultadoBusqueda.nombre != null)
                    {
                        Nombre = resultadoBusqueda.nombre;
                        Apellidos = resultadoBusqueda.apellidos;
                        if (resultadoBusqueda.estado == 1)
                            Estado = true;
                        else
                            Estado = false;
                        Password = resultadoBusqueda.password;
                        Rol = resultadoBusqueda.rol;
                        RolSeleccionado = Roles.Where(p => p.id == Rol).FirstOrDefault();
                        VisibleCliente = true;
                        ID = resultadoBusqueda.idUsuario;
                        
                        if (rol == (int)RolesEnum.Repartidor)
                        {
                            //todos los repartidores tienen el mismo id
                            RepartidorSeleccionado = Repartidores.Where(p => p.idUsuario == ID).FirstOrDefault();
                        }

                        App.userdialog.HideLoading();
                    }
                    else
                    {
                        VisibleCliente = false;
                        App.userdialog.HideLoading();
                        App.customDialog.ShowDialogAsync(AppResources.UsuarioNoEncontrado, AppResources.App, AppResources.Cerrar);
                    }
                }));
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async Task initBuscar()
        {
            try
            {
                //var current = Connectivity.NetworkAccess;
                if (App.tengoConexion)
                {
                    if (!string.IsNullOrEmpty(Email))
                    {
                        try
                        {
                            resultadoBusqueda = await ResponseServiceWS.TraeUsuario(Email,1);
                        }
                        catch (Exception ex)
                        {

                            // 
                            resultadoBusqueda = null;
                        }
                    }
                    else
                    {
                        resultadoBusqueda = null;
                    }
                }
                else
                {
                    resultadoBusqueda = null;
                }
            }
            catch (Exception ex)
            {
                // 
                resultadoBusqueda = null;
            }
        }
        private async void Modificar()
        {
            try
            {
                bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App, AppResources.PreguntaModificarUsuario, AppResources.No, AppResources.Si);
                ModificarUsuario(result);
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private void ModificarUsuario(bool hacer)
        {
            try
            {
                if (hacer)
                {
                    try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                    Task.Run(async () => { await initModificar(); }).ContinueWith(res => MainThread.BeginInvokeOnMainThread(() =>
                    {

                        //if (1==1)
                        if (resultadoModificacion)
                        {

                            App.userdialog.HideLoading();
                            App.customDialog.ShowDialogAsync(AppResources.mUsuarioOK, AppResources.App, AppResources.Cerrar);
                        }
                        else
                        {
                            VisibleCliente = false;
                            App.userdialog.HideLoading();
                            App.customDialog.ShowDialogAsync(AppResources.UsuarioNoEncontrado, AppResources.App, AppResources.Cerrar);
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                // 
                App.customDialog.ShowDialogAsync(AppResources.Error, AppResources.App, AppResources.Cerrar);
            }
        }
        private async Task initModificar()
        {
            try
            {
                //var current = Connectivity.NetworkAccess;
                if (App.tengoConexion)
                {
                    try
                    {
                        if (RepartidorSeleccionado == null)
                            RepartidorSeleccionado = new RepartidorModel();
                        else
                            Repartidores.First(p => p.id == RepartidorSeleccionado.id).idUsuario = RepartidorSeleccionado.idUsuario;
                        resultadoModificacion = await ResponseServiceWS.actualizaUsuario(ID, Estado, RolSeleccionado.id, Password, 67, RepartidorSeleccionado.id);
                    }
                    catch (Exception ex)
                    {

                        // 
                        resultadoModificacion = false;
                    }
                }
                else
                {
                    resultadoModificacion = false;
                }
            }
            catch (Exception ex)
            {
                // 
                resultadoModificacion = false;
            }
        }
        #endregion
        
        #region Propiedades
        private int ID;
        private bool resultadoModificacion = false;
        UsuarioModel resultadoBusqueda;
        private bool visibleRepartidor;
        public bool VisibleRepartidor
        {
            get
            {
                return visibleRepartidor;
            }
            set
            {
                if (visibleRepartidor != value)
                {
                    visibleRepartidor = value;
                    OnPropertyChanged(nameof(VisibleRepartidor));
                }
            }
        }
        private ObservableCollection<RepartidorModel> repartidores;
        public ObservableCollection<RepartidorModel> Repartidores
        {
            get
            {
                return repartidores;
            }
            set
            {
                if (repartidores != value)
                {
                    repartidores = value;
                    OnPropertyChanged(nameof(Repartidores));
                }
            }
        }
        private RepartidorModel repartidorSeleccionado;
        public RepartidorModel RepartidorSeleccionado
        {
            get
            {
                return repartidorSeleccionado;
            }
            set
            {
                if (repartidorSeleccionado != value)
                {
                    repartidorSeleccionado = value;
                    OnPropertyChanged(nameof(RepartidorSeleccionado));
                }
            }
        }
        private bool visibleCliente;
        public bool VisibleCliente
        {
            get
            {
                return visibleCliente;
            }
            set
            {
                if (visibleCliente != value)
                {
                    visibleCliente = value;
                    OnPropertyChanged(nameof(VisibleCliente));
                }
            }
        }
        private string email;
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }
        private string nombre;
        public string Nombre
        {
            get
            {
                return nombre;
            }
            set
            {
                if (nombre != value)
                {
                    nombre = value;
                    OnPropertyChanged(nameof(Nombre));
                }
            }
        }
        private string apellidos;
        public string Apellidos
        {
            get
            {
                return apellidos;
            }
            set
            {
                if (apellidos != value)
                {
                    apellidos = value;
                    OnPropertyChanged(nameof(Apellidos));
                }
            }
        }
        private string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }
        private int rol;
        public int Rol
        {
            get
            {
                return rol;
            }
            set
            {
                if (rol != value)
                {
                    rol = value;
                    OnPropertyChanged(nameof(Rol));
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
        private ObservableCollection<RolModel> roles;
        public ObservableCollection<RolModel> Roles
        {
            get
            {
                return roles;
            }
            set
            {
                if (roles != value)
                {
                    roles = value;
                    OnPropertyChanged(nameof(Roles));
                }
            }
        }
        private RolModel rolSeleccionado;
        public RolModel RolSeleccionado
        {
            get
            {
                return rolSeleccionado;
            }
            set
            {
                if (rolSeleccionado != value)
                {
                    rolSeleccionado = value;
                    OnPropertyChanged(nameof(RolSeleccionado));
                    VisibleRepartidor = RolSeleccionado.id == 4;
                }
            }
        }
        #endregion
    }
}
