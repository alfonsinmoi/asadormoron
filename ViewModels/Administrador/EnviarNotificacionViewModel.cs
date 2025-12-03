using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class EnviarNotificacionViewModel : ViewModelBase
    {
        public EnviarNotificacionViewModel() { }

        public async override Task InitializeAsync(object navigationData)
        {
            try
            {
                OpcionesTexto = new ObservableCollection<string>();
                OpcionesTexto.Add(AppResources.Igual);
                OpcionesTexto.Add(AppResources.EmpiezaPor);
                OpcionesTexto.Add(AppResources.TerminaEn);
                OpcionesTexto.Add(AppResources.Contiene);
                VersionDesde = 1;
                if (App.DAUtil.versionAppAndroid>App.DAUtil.versionAppiOS)
                    VersionHasta = App.DAUtil.versionAppAndroid;
                else
                    VersionHasta = App.DAUtil.versionAppiOS;
                VerificadoTodos = true;
                Verificado = false;
                NoVerificado = false;

                Roles = new ObservableCollection<RolModel>();
                RolModel es = new RolModel();
                es.id = 0;
                es.nombre = AppResources.Todos;
                Roles.Add(es);

                es = new RolModel();
                es.id = 1;
                es.nombre = AppResources.Usuario;
                Roles.Add(es);

                es = new RolModel();
                es.id = 2;
                es.nombre = AppResources.Establecimiento;
                Roles.Add(es);

                es = new RolModel();
                es.id = 3;
                es.nombre = AppResources.Administrador;
                Roles.Add(es);

                es = new RolModel();
                es.id = 4;
                es.nombre = AppResources.Repartidor;
                Roles.Add(es);

                es = new RolModel();
                es.id = 6;
                es.nombre = AppResources.Camarero;
                Roles.Add(es);

                Rol = Roles[0];
                SOTodos = true;
                Android = false;
                IOS = false;
                OpcionNombre = OpcionesTexto[0];
                OpcionApellidos = OpcionesTexto[0];
                OpcionEmail = OpcionesTexto[0];
                App.DAUtil.EnTimer = false;
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
        }

        #region Metodos

        private async Task EnviarMail()
        {
            try
            {
                if (Cuerpo.Length > 10)
                {
                    usuarios = App.ResponseWS.GetUsuariosFiltro(Filtro());
                    bool result = await App.customDialog.ShowDialogConfirmationAsync(AppResources.App,AppResources.PreguntaEnvioNotificacion.Replace("%1", usuarios.Count.ToString()), AppResources.No, AppResources.Si);
                    Enviar(result);
                }
            }
            catch (Exception ex)
            {
                // 
            }
        }
        private async void Enviar(bool hacer)
        {
            if (hacer)
            {
                try
                {
                    if (await ResponseServiceWS.sendNotificacionVarios(Cuerpo, Filtro()))
                    {
                        await App.customDialog.ShowDialogAsync(AppResources.NotificacionOK, AppResources.App, AppResources.Cerrar);
                    }
                    else
                        await App.customDialog.ShowDialogAsync( AppResources.NotificacionKO, AppResources.App, AppResources.Cerrar);
                }
                catch (Exception)
                {
                    await App.customDialog.ShowDialogAsync(AppResources.NotificacionKO, AppResources.App, AppResources.Cerrar);
                }
            }
        }
        private string Filtro()
        {
            string sSQL = "SELECT token FROM qo_users WHERE token <>''";
            if (!string.IsNullOrEmpty(Nombre))
            {
                if (OpcionNombre.Equals(AppResources.Igual))
                    sSQL += " AND nombre='" + Nombre + "'";
                else if (OpcionNombre.Equals(AppResources.EmpiezaPor))
                    sSQL += " AND nombre like '" + Nombre + "%'";
                else if (OpcionNombre.Equals(AppResources.TerminaEn))
                    sSQL += " AND nombre like '%" + Nombre + "'";
                else if (OpcionNombre.Equals(AppResources.Contiene))
                    sSQL += " AND nombre like '%" + Nombre + "%'";
            }
            if (!string.IsNullOrEmpty(Apellidos))
            {
                if (OpcionApellidos.Equals(AppResources.Igual))
                    sSQL += " AND apellidos='" + Apellidos + "'";
                else if (OpcionNombre.Equals(AppResources.EmpiezaPor))
                    sSQL += " AND apellidos like '" + Apellidos + "%'";
                else if (OpcionNombre.Equals(AppResources.TerminaEn))
                    sSQL += " AND apellidos like '%" + Apellidos + "'";
                else if (OpcionNombre.Equals(AppResources.Contiene))
                    sSQL += " AND apellidos like '%" + Apellidos + "%'";
            }
            if (!string.IsNullOrEmpty(Email))
            {
                if (OpcionEmail.Equals(AppResources.Igual))
                    sSQL += " AND email='" + Email + "'";
                else if (OpcionNombre.Equals(AppResources.EmpiezaPor))
                    sSQL += " AND email like '" + Email + "%'";
                else if (OpcionNombre.Equals(AppResources.TerminaEn))
                    sSQL += " AND email like '%" + Email + "'";
                else if (OpcionNombre.Equals(AppResources.Contiene))
                    sSQL += " AND email like '%" + Email + "%'";
            }

            sSQL += " AND version>=" + VersionDesde + " AND version<=" + VersionHasta;
            if (Verificado)
                sSQL += " AND verificado=1";
            if (NoVerificado)
                sSQL += " AND verificado=0";
            if (Rol.id > 0)
                sSQL += " AND rol=" + Rol.id;
            if (Android)
                sSQL += " AND plataforma='android'";
            if (IOS)
                sSQL += " AND plataforma='ios'";
            return sSQL;
        }

        #endregion

        #region Comandos
        public ICommand cmdEmail { get { return new DelegateCommandAsync(EnviarMail); } }
        #endregion

        #region Propiedades
        List<UsuarioModel> usuarios;
        private ObservableCollection<String> opcionesTexto;
        public ObservableCollection<String> OpcionesTexto
        {
            get
            {
                return opcionesTexto;
            }
            set
            {
                if (opcionesTexto != value)
                {
                    opcionesTexto = value;
                    OnPropertyChanged(nameof(OpcionesTexto));
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
        private int versionDesde;
        public int VersionDesde
        {
            get
            {
                return versionDesde;
            }
            set
            {
                if (versionDesde != value)
                {
                    versionDesde = value;
                    OnPropertyChanged(nameof(VersionDesde));

                }
            }
        }
        private int versionHasta;
        public int VersionHasta
        {
            get
            {
                return versionHasta;
            }
            set
            {
                if (versionHasta != value)
                {
                    versionHasta = value;
                    OnPropertyChanged(nameof(VersionHasta));

                }
            }
        }
        private bool verificado;
        public bool Verificado
        {
            get
            {
                return verificado;
            }
            set
            {
                if (verificado != value)
                {
                    verificado = value;
                    OnPropertyChanged(nameof(Verificado));
                    if (!Verificado)
                    {
                        if (!NoVerificado && !VerificadoTodos)
                            Verificado = true;
                    }
                    else
                    {
                        NoVerificado = false;
                        VerificadoTodos = false;
                    }

                }
            }
        }
        private bool noVerificado;
        public bool NoVerificado
        {
            get
            {
                return noVerificado;
            }
            set
            {
                if (noVerificado != value)
                {
                    noVerificado = value;
                    OnPropertyChanged(nameof(NoVerificado));
                    if (!NoVerificado)
                    {
                        if (!Verificado && !VerificadoTodos)
                            NoVerificado = true;
                    }
                    else
                    {
                        Verificado = false;
                        VerificadoTodos = false;
                    }

                }
            }
        }
        private bool verificadoTodos;
        public bool VerificadoTodos
        {
            get
            {
                return verificadoTodos;
            }
            set
            {
                if (verificadoTodos != value)
                {
                    verificadoTodos = value;
                    OnPropertyChanged(nameof(VerificadoTodos));
                    if (!VerificadoTodos)
                    {
                        if (!Verificado && !NoVerificado)
                            VerificadoTodos = true;
                    }
                    else
                    {
                        Verificado = false;
                        NoVerificado = false;
                    }

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
        private RolModel rol;
        public RolModel Rol
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
        private string cuerpo = "";
        public string Cuerpo
        {
            get
            {
                return cuerpo;
            }
            set
            {
                if (cuerpo != value)
                {
                    cuerpo = value;
                    OnPropertyChanged(nameof(Cuerpo));
                }
            }
        }
        private string opcionEmail;
        public string OpcionEmail
        {
            get
            {
                return opcionEmail;
            }
            set
            {
                if (opcionEmail != value)
                {
                    opcionEmail = value;
                    OnPropertyChanged(nameof(OpcionEmail));

                }
            }
        }
        private string opcionNombre;
        public string OpcionNombre
        {
            get
            {
                return opcionNombre;
            }
            set
            {
                if (opcionNombre != value)
                {
                    opcionNombre = value;
                    OnPropertyChanged(nameof(OpcionNombre));

                }
            }
        }
        private string opcionApellidos;
        public string OpcionApellidos
        {
            get
            {
                return opcionApellidos;
            }
            set
            {
                if (opcionApellidos != value)
                {
                    opcionApellidos = value;
                    OnPropertyChanged(nameof(OpcionApellidos));
                }
            }
        }
        private bool soTodos;
        public bool SOTodos
        {
            get
            {
                return soTodos;
            }
            set
            {
                if (soTodos != value)
                {
                    soTodos = value;
                    OnPropertyChanged(nameof(SOTodos));
                    if (!SOTodos)
                    {
                        if (!Android && !IOS)
                            SOTodos = true;
                    }
                    else
                    {
                        Android = false;
                        IOS = false;
                    }
                }
            }
        }
        private bool android;
        public bool Android
        {
            get
            {
                return android;
            }
            set
            {
                if (android != value)
                {
                    android = value;
                    OnPropertyChanged(nameof(Android));
                    if (!Android)
                    {
                        if (!SOTodos && !IOS)
                            Android = true;
                    }
                    else
                    {
                        SOTodos = false;
                        IOS = false;
                    }
                }
            }
        }
        private bool ios;
        public bool IOS
        {
            get
            {
                return ios;
            }
            set
            {
                if (ios != value)
                {
                    ios = value;
                    OnPropertyChanged(nameof(IOS));
                    if (!IOS)
                    {
                        if (!SOTodos && !Android)
                            IOS = true;
                    }
                    else
                    {
                        SOTodos = false;
                        Android = false;
                    }
                }
            }
        }
        private bool esMultiAdmin;
        public bool EsMultiAdmin
        {
            get
            {
                return esMultiAdmin;
            }
            set
            {
                if (esMultiAdmin != value)
                {
                    esMultiAdmin = value;
                    OnPropertyChanged(nameof(EsMultiAdmin));
                }
            }
        }
        #endregion
    }
}
