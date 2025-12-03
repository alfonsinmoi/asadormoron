using AsadorMoron.Interfaces;
// 
using AsadorMoron.Models;
using AsadorMoron.Recursos;
using AsadorMoron.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.ViewModels.Clientes
{

    public class RecoveryViewModel : ViewModelBase
    {
        public RecoveryViewModel() { }

        public override Task InitializeAsync(object navigationData)
        {
            try
            {
                App.DAUtil.EnTimer = false;
                try
                {
                    if (string.IsNullOrEmpty(navigationData.ToString()))
                        Email = "";
                    else
                        Email = navigationData.ToString();
                    Pass = "";
                    RePass = "";
                    return base.InitializeAsync(navigationData);

                }
                catch (Exception ex)
                {
                    // 
                }
                finally
                {
                    App.userdialog.HideLoading();
                }
            }
            catch (Exception ex)
            {
                App.userdialog.HideLoading();
                // 
            }
            return base.InitializeAsync(navigationData);
        }

        #region Metodos

        private async void Guardar()
        {
            try
            {
                try { App.userdialog.ShowLoading(AppResources.Guardando, MaskType.Black); } catch (Exception) { }
                await Task.Delay(200);
                await Task.Run(async () => { await initGuardar(); }).ContinueWith(res => MainThread.BeginInvokeOnMainThread(() =>
                {
                    App.userdialog.HideLoading();
                    if (ok)
                        App.DAUtil.NavigationService.NavigateToAsync<PinViewModel>();

                }));
            }
            catch (Exception ex)
            {
                // 
            }
            finally
            {
                App.userdialog.HideLoading();
            }
        }

        private void Cancelar()
        {
            App.DAUtil.NavigationService.LogOutApp(typeof(LoginViewModel), null);
        }

        bool ValidatePassword(string password)
        {
            try
            {
                const int MIN_LENGTH = 6;
                const int MAX_LENGTH = 15;

                if (password == null) throw new ArgumentNullException();

                bool meetsLengthRequirements = password.Length >= MIN_LENGTH && password.Length <= MAX_LENGTH;

                bool isValid = meetsLengthRequirements;
                return isValid;
            }
            catch (Exception ex)
            {
                // 
                return false;
            }
        }

        private async Task initGuardar()
        {
            try
            {
                ok = false;
                if (string.IsNullOrEmpty(Email))
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync(AppResources.EmailObligatorio, AppResources.App, AppResources.Cerrar);
                }
                else if (Pass.Equals(RePass))
                {
                    if (!ValidatePassword(Pass))
                    {
                        App.userdialog.HideLoading();
                        await App.customDialog.ShowDialogAsync(AppResources.PassNoSeguro, AppResources.App, AppResources.Cerrar);
                    }
                    else
                    {
                        PIN = App.DAUtil.GenerateRandomNo().ToString();
                        String error = App.ResponseWS.CambioPass(Email, Pass, PIN);
                        Preferences.Set("email", email);
                        Preferences.Set("PIN", PIN);
                        if (!error.Contains("ERROR"))
                        {
                            ok = true;
                            if (App.DAUtil.UsuarioNoVerificado == null)
                                App.DAUtil.UsuarioNoVerificado = new UsuarioModel();
                            App.DAUtil.UsuarioNoVerificado.email = Email;
                            EnviaEmail();
                            await App.customDialog.ShowDialogAsync(AppResources.SolicitudOK, AppResources.App, AppResources.Cerrar);
                        }
                        else
                        {
                            Preferences.Set("PIN", string.Empty);
                            App.userdialog.HideLoading();
                            await App.customDialog.ShowDialogAsync(error, AppResources.App, AppResources.Cerrar);
                        }
                    }
                }
                else
                {
                    App.userdialog.HideLoading();
                    await App.customDialog.ShowDialogAsync(AppResources.PassNoCoincide, AppResources.App, AppResources.Cerrar);
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

        private void EnviaEmail()
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
                cuerpo += "        <td align=\"center\" style=\"padding:0;Margin:0;background-color:transparent;background-position:center bottom;background-repeat:no-repeat no-repeat;\" bgcolor=\"transparent\" > ";
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
                cuerpo += "              <td align=\"center\" style=\"padding:0;Margin:0;\"><h1 style=\"Margin:0;line-height:36px;mso-line-height-rule:exactly;font-family:tahoma, verdana, segoe, sans-serif;font-size:30px;font-style:normal;font-weight:bold;color:#212121;\">¡Gracias por Utilizar PolloAndaluz!</h1></td> ";
                cuerpo += "             </tr> ";
                cuerpo += "             <tr style=\"border-collapse:collapse;\"> ";
                cuerpo += "              <td align=\"center\" style=\"padding:0;Margin:0;padding-top:20px;\"><p style=\"Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:16px;font-family:roboto, 'helvetica neue', helvetica, arial, sans-serif;line-height:24px;color:#131313;\">Verifique su cuenta, introduza el PIN en la App y la nueva contraseña estará activa</p></td> ";
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
                cuerpo += PIN;
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

                App.DAUtil.EnviaEmail(Email, AppResources.VerificaEmail, cuerpo);
            }
            catch (Exception ex)
            {
                // 
            }
        }

        #endregion

        #region Comandos

        public ICommand CommandGuardar { get { return new Command(Guardar); } }
        public ICommand CommandCancelar { get { return new Command(Cancelar); } }

        #endregion

        #region Propiedades

        private bool ok = false;
        private string PIN;
        private string pass;
        public string Pass
        {
            get { return pass; }
            set
            {
                if (pass != value)
                {
                    pass = value;
                    OnPropertyChanged(nameof(Pass));
                }
            }
        }

        private string rePass;
        public string RePass
        {
            get { return rePass; }
            set
            {
                if (rePass != value)
                {
                    rePass = value;
                    OnPropertyChanged(nameof(RePass));
                }
            }
        }

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

        #endregion

    }
}
