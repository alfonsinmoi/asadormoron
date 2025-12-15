using AsadorMoron.Interfaces;
using AsadorMoron.Models;
using AsadorMoron.ViewModels.Base;
// 
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
// 
using AsadorMoron.Services;
using AsadorMoron.Recursos;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Newtonsoft.Json;
using System.Diagnostics;
using AsadorMoron.Interfaces;
using System.Collections.Generic;
using AsadorMoron.Messages;
using System.Linq;

namespace AsadorMoron.ViewModels.Clientes
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel()
        {
            if (App.userdialog == null)
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
            }
        }

        private static Stopwatch _loginStopwatch;

        public override Task InitializeAsync(object navigationData)
        {
            _loginStopwatch = Stopwatch.StartNew();
            Debug.WriteLine($"[PERF-LOGIN] InitializeAsync INICIO: {_loginStopwatch.ElapsedMilliseconds}ms");
            try
            {
                App.DAUtil.EnTimer = false;
                Debug.WriteLine($"[PERF-LOGIN] getConfiguracionAdmin INICIO: {_loginStopwatch.ElapsedMilliseconds}ms");
                ConfiguracionAdmin cAdmin = ResponseServiceWS.getConfiguracionAdmin();
                Debug.WriteLine($"[PERF-LOGIN] getConfiguracionAdmin FIN: {_loginStopwatch.ElapsedMilliseconds}ms");
                VisibleRS = cAdmin.visibleRS;
                Debug.WriteLine($"[PERF-LOGIN] GetUsuarioSQLite INICIO: {_loginStopwatch.ElapsedMilliseconds}ms");
                UsuarioModel persona = App.DAUtil.GetUsuarioSQLite();
                Debug.WriteLine($"[PERF-LOGIN] GetUsuarioSQLite FIN: {_loginStopwatch.ElapsedMilliseconds}ms");
                if (persona != null)
                {
                    Email = persona.username;
                    Password = persona.password;
                    Debug.WriteLine($"[PERF-LOGIN] Login() INICIO: {_loginStopwatch.ElapsedMilliseconds}ms");
                    Login();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PERF-LOGIN] ERROR: {ex.Message} ({_loginStopwatch.ElapsedMilliseconds}ms)");
                App.userdialog.HideLoading();
            }
            finally
            {
                App.userdialog.HideLoading();
            }
            return base.InitializeAsync(navigationData).ContinueWith((task) => { App.userdialog.HideLoading(); });
        }

        #region Metodos
        private async void Cancelar()
        {
            try
            {
                App.userdialog?.ShowLoading(AppResources.Cargando);
                await App.DAUtil.NavigationService.InitializeAsync();
            }
            finally
            {
                App.userdialog?.HideLoading();
            }
        }

        private async Task Login()
        {
            Debug.WriteLine($"[PERF-LOGIN] Login() ENTRADA: {_loginStopwatch?.ElapsedMilliseconds ?? 0}ms");
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                await Task.Delay(millisecondsDelay: 200);
                Debug.WriteLine($"[PERF-LOGIN] InitLogin INICIO: {_loginStopwatch?.ElapsedMilliseconds ?? 0}ms");
                await InitLogin();
                Debug.WriteLine($"[PERF-LOGIN] InitLogin FIN (result={resultLogin}): {_loginStopwatch?.ElapsedMilliseconds ?? 0}ms");
                if (resultLogin)
                {
                    // OPTIMIZADO: Usar versiones async para no bloquear UI
                    int rol = App.DAUtil.Usuario.rol;
                    Debug.WriteLine($"[PERF-LOGIN] Carga datos rol {rol} INICIO: {_loginStopwatch?.ElapsedMilliseconds ?? 0}ms");
                    if (rol == (int)RolesEnum.Establecimiento || rol == (int)RolesEnum.Administrador)
                    {
                        string ids = App.DAUtil.Usuario.idPueblo.ToString();
                        await App.AsyncService.ListadoRepartidoresMultiAdminAsync(ids);
                    }
                    else if (rol == (int)RolesEnum.Repartidor)
                    {
                        App.DAUtil.Usuario.Repartidor = await App.AsyncService.GetRepartidorByIdUsuarioAsync(App.DAUtil.Usuario.idUsuario);
                    }
                    else if (rol == (int)RolesEnum.Cliente)
                        App.promocionAmigo = await App.AsyncService.GetPromocionAmigoAsync();
                    Debug.WriteLine($"[PERF-LOGIN] Carga datos rol FIN: {_loginStopwatch?.ElapsedMilliseconds ?? 0}ms");

                    Preferences.Set("PIN", "");
                    Preferences.Set("Social", false);
                    Preferences.Set("RedSocial", "");
                    Debug.WriteLine($"[PERF-LOGIN] GetZonasAsync INICIO: {_loginStopwatch?.ElapsedMilliseconds ?? 0}ms");
                    await App.AsyncService.GetZonasAsync();
                    Debug.WriteLine($"[PERF-LOGIN] GetZonasAsync FIN: {_loginStopwatch?.ElapsedMilliseconds ?? 0}ms");
                    Debug.WriteLine($"[PERF-LOGIN] NavigationService.InitializeAsync INICIO: {_loginStopwatch?.ElapsedMilliseconds ?? 0}ms");
                    App.DAUtil.NavigationService.InitializeAsync().ContinueWith(task => Device.BeginInvokeOnMainThread(() => { App.userdialog.HideLoading(); }));
                }
                else
                {
                    App.userdialog.HideLoading();
                    string mensaje = !string.IsNullOrEmpty(mensajeUsuario) ? mensajeUsuario : AppResources.LoginIncorrecto;
                    await App.customDialog.ShowDialogAsync(mensaje, AppResources.SoloError, AppResources.Aceptar);
                }
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                Debug.WriteLine($"[PERF-LOGIN] ERROR Login: {ex.Message} ({_loginStopwatch?.ElapsedMilliseconds ?? 0}ms)");
            }
            finally
            {
                Debug.WriteLine($"[PERF-LOGIN] Login TOTAL: {_loginStopwatch?.ElapsedMilliseconds ?? 0}ms");
                App.userdialog.HideLoading();
            }
        }
        private async Task InitLogin()
        {
            Debug.WriteLine($"[PERF-LOGIN] InitLogin ENTRADA: {_loginStopwatch?.ElapsedMilliseconds ?? 0}ms");
            try
            {
                if (App.tengoConexion)
                {
                    if (!string.IsNullOrEmpty(Email) || !string.IsNullOrEmpty(Password))
                    {
                        try
                        {
                            Debug.WriteLine($"[PERF-LOGIN] ResponseServiceWS.Login INICIO: {_loginStopwatch?.ElapsedMilliseconds ?? 0}ms");
                            if (ResponseServiceWS.Login(Email, Password))
                            {
                                Debug.WriteLine($"[PERF-LOGIN] ResponseServiceWS.Login FIN (OK): {_loginStopwatch?.ElapsedMilliseconds ?? 0}ms");
                                if (Device.RuntimePlatform != Device.UWP)
                                {
                                    App.DAUtil.Usuario.platform = Device.RuntimePlatform.ToLower();
                                    if (App.DAUtil.InstallId != null)
                                        App.DAUtil.Usuario.token = App.DAUtil.InstallId.ToString();
                                    if (Device.RuntimePlatform == Device.iOS)
                                        App.DAUtil.Usuario.version = App.DAUtil.versioniOS;
                                    else if (Device.RuntimePlatform == Device.Android)
                                        App.DAUtil.Usuario.version = App.DAUtil.versionAndroid;
                                    if (App.DAUtil.InstallId != null)
                                    {
                                        Debug.WriteLine($"[PERF-LOGIN] RegistraTokenFCM INICIO: {_loginStopwatch?.ElapsedMilliseconds ?? 0}ms");
                                        App.ResponseWS.RegistraTokenFCM(App.DAUtil.Usuario);
                                        Debug.WriteLine($"[PERF-LOGIN] RegistraTokenFCM FIN: {_loginStopwatch?.ElapsedMilliseconds ?? 0}ms");
                                    }
                                }

                                // KIOSKO: Si el usuario es Kiosko, precargar todos los datos al inicio
                                if (Services.KioskoPreloadService.EsKiosko && App.EstActual != null)
                                {
                                    Debug.WriteLine("[Login] Usuario Kiosko detectado - Iniciando precarga de datos...");
                                    // Crear tablas Kiosko si no existen
                                    App.DAUtil.CreaTablasKiosko();
                                    // Precargar datos en segundo plano (no bloqueamos el login)
                                    _ = Services.KioskoPreloadService.Instance.PrecargarDatosKioskoAsync(App.EstActual.idEstablecimiento);
                                }

                                resultLogin = true;
                            }
                            else if (App.DAUtil.Usuario != null)
                            {
                                if (string.IsNullOrEmpty(App.DAUtil.Usuario.password))
                                {
                                    try { App.userdialog.ShowLoading(AppResources.Cargando, MaskType.Black); } catch (Exception) { }
                                    Device.BeginInvokeOnMainThread(async () =>
                                    {
                                        App.DAUtil.Idioma = "ES";
                                        await App.DAUtil.NavigationService.NavigateToAsync<RecoveryViewModel>(App.DAUtil.Usuario.email);
                                    });
                                }
                            }/*
                            else if (!ResponseServiceWS.usuarioVerificado(Email))
                            {
                                await App.customDialog.ShowDialogAsync("Su usuario no está verificado, recibirá un email con un PIN de 4 cifras para validarlo" + Environment.NewLine + "Por favor, si no lo recibe, mire en su carpeta de Correo no deseado", AppResources.App, AppResources.Cerrar);
                                string nuevoPIN = App.DAUtil.GenerateRandomNo().ToString();
                                Xamarin.Essentials.Preferences.Set("PIN", nuevoPIN);
                                Preferences.Set("email", Email);
                                string error = App.ResponseWS.actualizaPIN(nuevoPIN, Email);
                                if (!error.Contains("ERROR"))
                                {
                                    EnviaEmail(nuevoPIN);
                                }
                                else
                                {
                                    App.userdialog.HideLoading();
                                    await App.customDialog.ShowDialogAsync(error, AppResources.App, AppResources.Cerrar);
                                }
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    mensajeUsuario = "";
                                    resultLogin = false;

                                    await App.DAUtil.NavigationService.NavigateToAsyncWithoutMenu<PinViewModel>();
                                    App.userdialog.HideLoading();
                                });
                            }*/
                            else
                            {
                                mensajeUsuario = AppResources.LoginIncorrecto;
                                Debug.WriteLine($"Login Incorrecto - Usuario={Email}");
                                resultLogin = false;
                                App.userdialog.HideLoading();
                            }
                        }
                        catch (Exception ex)
                        {
                            App.userdialog.HideLoading();
                            Debug.WriteLine("Error Login: " + ex.Message);
                            resultLogin = false;
                        }
                    }
                    else
                    {
                        mensajeUsuario = AppResources.UsuarioIncorrecto;
                        resultLogin = false;
                    }
                }
                else
                {
                    mensajeUsuario = AppResources.SinInternet;
                    resultLogin = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error InitLogin: " + ex.Message);
                App.userdialog.HideLoading();
                resultLogin = false;
            }
        }

        private async void Registro(object obj)
        {
            try
            {
                App.userdialog?.ShowLoading(AppResources.Cargando);
                App.DAUtil.Idioma = "ES";
                await App.DAUtil.NavigationService.NavigateToAsync<RegistroViewModel>();
            }
            finally
            {
                App.userdialog?.HideLoading();
            }
        }

        private async void Olvido(object obj)
        {
            try
            {
                App.userdialog?.ShowLoading(AppResources.Cargando);
                App.DAUtil.Idioma = "ES";
                await App.DAUtil.NavigationService.NavigateToAsync<RecoveryViewModel>();
            }
            finally
            {
                App.userdialog?.HideLoading();
            }
        }
        private void EnviaEmail(string nuevoPIN)
        {
            try
            {
                string cuerpo = "";
                cuerpo += "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
                cuerpo += "<html style=\"width:100%;font-family:roboto, 'helvetica neue', helvetica, arial, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0;\">";
                cuerpo += " <head> ";
                cuerpo += "<meta charset=\"UTF-8\"> ";
                cuerpo += "<meta content=\"width=device-width, initial-scale=1\" name=\"viewport\"> ";
                cuerpo += "<meta name=\"x-apple-disable-message-reformatting\"> ";
                cuerpo += "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"> ";
                cuerpo += "<meta content=\"telephone=no\" name=\"format-detection\"> ";
                cuerpo += "<title>Nueva plantilla de correo electrónico 2020-05-31</title> ";
                cuerpo += "<!--[if (mso 16)]>    <style type=\"text/css\">    a {text-decoration: none;}    </style>    <![endif]--> ";
                cuerpo += "<!--[if gte mso 9]><style>sup { font-size: 100% !important; }</style><![endif]--> ";
                cuerpo += "<!--[if !mso]><!-- --> ";
                cuerpo += "<link href=\"https://fonts.googleapis.com/css?family=Roboto:400,400i,700,700i\" rel=\"stylesheet\"> ";
                cuerpo += "<!--<![endif]--> ";
                cuerpo += "<style type=\"text/css\">";
                cuerpo += "@media only screen and (max-width:600px) {.st-br { padding-left:10px!important; padding-right:10px!important } p, ul li, ol li, a { font-size:16px!important; line-height:150%!important } h1 { font-size:30px!important; text-align:center; line-height:120%!important } h2 { font-size:26px!important; text-align:center; line-height:120%!important } h3 { font-size:20px!important; text-align:center; line-height:120%!important } h1 a { font-size:30px!important; text-align:center } h2 a { font-size:26px!important; text-align:center } h3 a { font-size:20px!important; text-align:center } .es-menu td a { font-size:14px!important } .es-header-body p, .es-header-body ul li, .es-header-body ol li, .es-header-body a { font-size:16px!important } .es-footer-body p, .es-footer-body ul li, .es-footer-body ol li, .es-footer-body a { font-size:14px!important } .es-infoblock p, .es-infoblock ul li, .es-infoblock ol li, .es-infoblock a { font-size:12px!important } *[class=\"gmail-fix\"] { display:none!important } .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3 { text-align:center!important } .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3 { text-align:right!important } .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3 { text-align:left!important } .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img { display:inline!important } .es-button-border { display:block!important } a.es-button { font-size:16px!important; display:block!important; border-left-width:0px!important; border-right-width:0px!important } .es-btn-fw { border-width:10px 0px!important; text-align:center!important } .es-adaptive table, .es-btn-fw, .es-btn-fw-brdr, .es-left, .es-right { width:100%!important } .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header { width:100%!important; max-width:600px!important } .es-adapt-td { display:block!important; width:100%!important } .adapt-img { width:100%!important; height:auto!important } .es-m-p0 { padding:0px!important } .es-m-p0r { padding-right:0px!important } .es-m-p0l { padding-left:0px!important } .es-m-p0t { padding-top:0px!important } .es-m-p0b { padding-bottom:0!important } .es-m-p20b { padding-bottom:20px!important } .es-mobile-hidden, .es-hidden { display:none!important } .es-desk-hidden { display:table-row!important; width:auto!important; overflow:visible!important; float:none!important; max-height:inherit!important; line-height:inherit!important } .es-desk-menu-hidden { display:table-cell!important } table.es-table-not-adapt, .esd-block-html table { width:auto!important } table.es-social { display:inline-block!important } table.es-social td { display:inline-block!important } }";
                cuerpo += ".rollover:hover .rollover-first {";
                cuerpo += "	max-height:0px!important;";
                cuerpo += "	display:none!important;";
                cuerpo += "}";
                cuerpo += ".rollover:hover .rollover-second {";
                cuerpo += "	max-height:none!important;";
                cuerpo += "display:block!important;";
                cuerpo += "}";
                cuerpo += "#outlook a {";
                cuerpo += "padding:0;";
                cuerpo += "}";
                cuerpo += ".ExternalClass {";
                cuerpo += "width:100%;";
                cuerpo += "}";
                cuerpo += ".ExternalClass,";
                cuerpo += ".ExternalClass p,";
                cuerpo += ".ExternalClass span,";
                cuerpo += ".ExternalClass font,";
                cuerpo += ".ExternalClass td,";
                cuerpo += ".ExternalClass div {";
                cuerpo += "	line-height:100%;";
                cuerpo += "}";
                cuerpo += ".es-button {";
                cuerpo += "	mso-style-priority:100!important;";
                cuerpo += "	text-decoration:none!important;";
                cuerpo += "}";
                cuerpo += "a[x-apple-data-detectors] {";
                cuerpo += "	color:inherit!important;";
                cuerpo += "	text-decoration:none!important;";
                cuerpo += "	font-size:inherit!important;";
                cuerpo += "	font-family:inherit!important;";
                cuerpo += "	font-weight:inherit!important;";
                cuerpo += "	line-height:inherit!important;";
                cuerpo += "}";
                cuerpo += ".es-desk-hidden {";
                cuerpo += "	display:none;";
                cuerpo += "	float:left;";
                cuerpo += "	overflow:hidden;";
                cuerpo += "	width:0;";
                cuerpo += "	max-height:0;";
                cuerpo += "	line-height:0;";
                cuerpo += "	mso-hide:all;";
                cuerpo += "}";
                cuerpo += ".es-button-border:hover {";
                cuerpo += "border-style:solid solid solid solid!important;";
                cuerpo += "background:#d6a700!important;";
                cuerpo += "border-color:#42d159 #42d159 #42d159 #42d159!important;";
                cuerpo += "}";
                cuerpo += ".es-button-border:hover a.es-button {";
                cuerpo += "background:#d6a700!important;";
                cuerpo += "border-color:#d6a700!important;";
                cuerpo += "}";
                cuerpo += "</style> ";
                cuerpo += " </head> ";
                cuerpo += " <body style=\"width:100%;font-family:roboto, 'helvetica neue', helvetica, arial, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0;\"> ";
                cuerpo += "  <div class=\"es-wrapper-color\" style=\"background-color:#F6F6F6;\"> ";
                cuerpo += "   <!--[if gte mso 9]>";
                cuerpo += "			<v:background xmlns:v=\"urn:schemas-microsoft-com:vml\" fill=\"t\">";
                cuerpo += "				<v:fill type=\"tile\" color=\"#f6f6f6\"></v:fill>";
                cuerpo += "			</v:background>";
                cuerpo += "		<![endif]--> ";
                cuerpo += "   <table class=\"es-wrapper\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;\"> ";
                cuerpo += "    <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "    <td class=\"st-br\" valign=\"top\" style=\"padding:0;Margin:0;\"> ";
                cuerpo += "     <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-header\" align=\"center\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top;\"> ";
                cuerpo += "       <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "        <td align=\"center\" style=\"padding:0;Margin:0;background-color:transparent;background-position:center bottom;background-repeat:no-repeat no-repeat;\" bgcolor=\"transparent\"> ";
                cuerpo += "        <v:rect xmlns:v=\"urn:schemas-microsoft-com:vml\" fill=\"true\" stroke=\"false\" style=\"mso-width-percent:1000;height:204px;\"><img type=\"tile\" src=\"http://qoorderapp.eu/pa_ws/images/header2.png\" origin=\"0.5, 0\" position=\"0.5,0\" ></img><v:textbox inset=\"0,0,0,0\"> ";
                cuerpo += "        <div> ";
                cuerpo += "      </div> ";
                cuerpo += "      <!--[if gte mso 9]></v:textbox></v:rect><![endif]--></td> ";
                cuerpo += "    </tr> ";
                cuerpo += "  </table> ";
                cuerpo += "     <table cellpadding=\"0\" cellspacing=\"0\" class=\"es-content\" align=\"center\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;\"> ";
                cuerpo += "       <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "        <td align=\"center\" bgcolor=\"transparent\" style=\"padding:0;Margin:0;background-color:transparent;\"> ";
                cuerpo += "         <table bgcolor=\"transparent\" class=\"es-content-body\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;\"> ";
                cuerpo += "           <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "            <td align=\"left\" style=\"Margin:0;padding-top:10px;padding-bottom:10px;padding-left:20px;padding-right:20px;background-position:left bottom;\"> ";
                cuerpo += "             <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;\"> ";
                cuerpo += "               <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "               <td width=\"560\" align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;\"> ";
                cuerpo += "                 <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;\"> ";
                cuerpo += "                   <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "                  <td align=\"center\" style=\"padding:0;Margin:0;display:none;\"></td> ";
                cuerpo += "                  </tr> ";
                cuerpo += "               </table></td> ";
                cuerpo += "              </tr> ";
                cuerpo += "            </table></td> ";
                cuerpo += "          </tr> ";
                cuerpo += "    <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "      <td align=\"left\" style=\"Margin:0;padding-bottom:15px;padding-top:30px;padding-left:30px;padding-right:30px;border-radius:10px 10px 0px 0px;background-color:#FFFFFF;background-position:left bottom;\" bgcolor=\"#ffffff\"> ";
                cuerpo += "        <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;\"> ";
                cuerpo += "          <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "           <td width=\"540\" align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;\"> ";
                cuerpo += "           <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-position:left bottom;\" role=\"presentation\"> ";
                cuerpo += "             <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "              <td align=\"center\" style=\"padding:0;Margin:0;\"><h1 style=\"Margin:0;line-height:36px;mso-line-height-rule:exactly;font-family:tahoma, verdana, segoe, sans-serif;font-size:30px;font-style:normal;font-weight:bold;color:#212121;\">¡Gracias por registrarse en PolloAndaluz!</h1></td> ";
                cuerpo += "             </tr> ";
                cuerpo += "             <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "              <td align=\"center\" style=\"padding:0;Margin:0;padding-top:20px;\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:16px;font-family:roboto, 'helvetica neue', helvetica, arial, sans-serif;line-height:24px;color:#131313;\">Verifique su cuenta, introduza el PIN en la App</p></td> ";
                cuerpo += "             </tr> ";
                cuerpo += "          </table></td> ";
                cuerpo += "         </tr> ";
                cuerpo += "      </table></td> ";
                cuerpo += "    </tr> ";
                cuerpo += "         <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "           <td align=\"left\" style=\"Margin:0;padding-top:5px;padding-bottom:5px;padding-left:30px;padding-right:30px;border-radius:0px 0px 10px 10px;background-position:left top;background-color:#FFFFFF;\"> ";
                cuerpo += "            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;\"> ";
                cuerpo += "              <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "               <td width=\"540\" align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;\"> ";
                cuerpo += "                <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;\"> ";
                cuerpo += "                  <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "                   <td align=\"center\" style=\"padding:0;Margin:0;display:none;\"></td> ";
                cuerpo += "                  </tr> ";
                cuerpo += "                 </table></td> ";
                cuerpo += "               </tr> ";
                cuerpo += "             </table></td> ";
                cuerpo += "           </tr> ";
                cuerpo += "           <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "            <td align=\"left\" style=\"Margin:0;padding-top:30px;padding-bottom:30px;padding-left:30px;padding-right:30px;background-position:left bottom;\"> ";
                cuerpo += "             <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;\"> ";
                cuerpo += "              <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "                <td width=\"540\" align=\"center\" valign=\"top\" style=\"padding:0;Margin:0;\"> ";
                cuerpo += "                 <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" role=\"presentation\" style=\"mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;\"> ";
                cuerpo += "                   <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "                    <td align=\"center\" style=\"padding:0;Margin:0;\"><h3 style=\"Margin:0;line-height:45px;mso-line-height-rule:exactly;font-family:tahoma, verdana, segoe, sans-serif;font-size:30px;font-style:normal;font-weight:normal;color:#212121;\"><strong>";
                cuerpo += nuevoPIN;
                cuerpo += "</strong></h3></td>";
                cuerpo += "                   </tr>";
                cuerpo += "                  </table></td>";
                cuerpo += "                </tr>";
                cuerpo += "            </table></td>";
                cuerpo += "           </tr> ";
                cuerpo += "         </table></td>";
                cuerpo += "       </tr> ";
                cuerpo += "     </table></td> ";
                cuerpo += "   </tr> ";
                cuerpo += " </table> ";
                cuerpo += "  </div>  ";
                cuerpo += " </body>";
                cuerpo += "</html>";

                App.DAUtil.EnviaEmail(Email, "Verifica tu email", cuerpo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error EnviaEmail: " + ex.Message);
            }
        }
        #endregion

        #region Comandos

        public ICommand BtnLogin { get { return new Command(async () => await Login()); } }
        public ICommand BtnRegistro { get { return new Command(Registro); } }
        public ICommand BtnOlvido { get { return new Command(Olvido); } }
        public ICommand BtnCancel { get { return new Command(Cancelar); } }

        #endregion

        #region Propiedades
        private bool resultLogin = false;
        private string mensajeUsuario = string.Empty;
        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }
        private bool visibleRS;
        public bool VisibleRS
        {
            get { return visibleRS; }
            set
            {
                if (visibleRS != value)
                {
                    visibleRS = value;
                    OnPropertyChanged(nameof(VisibleRS));
                }
            }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }
        #endregion

    }
}
