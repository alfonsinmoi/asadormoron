using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
    public class DatosFiscalesEstablecimientoViewModel:ViewModelBase
    {
        #region Propiedades
        private EstablecimientoFiscalModel fiscal;
        private List<PueblosModel> poblaciones = App.DAUtil.GetPueblosSQLite().Where(p => p.visibleListado == true).ToList();
        private string razonSocial="";
        public string RazonSocial
        {
            get { return razonSocial; }
            set
            {
                if (razonSocial != value)
                {
                    razonSocial = value;
                    OnPropertyChanged(nameof(RazonSocial));
                }
            }
        }
        private string direccion="";
        public string Direccion
        {
            get { return direccion; }
            set
            {
                if (direccion != value)
                {
                    direccion = value;
                    OnPropertyChanged(nameof(Direccion));
                }
            }
        }
        private string cif="";
        public string CIF
        {
            get { return cif; }
            set
            {
                if (cif != value)
                {
                    cif = value;
                    OnPropertyChanged(nameof(CIF));
                }
            }
        }
        private CodigosPostales cp;
        public CodigosPostales CP
        {
            get { return cp; }
            set
            {
                if (cp != value)
                {
                    cp = value;
                    OnPropertyChanged(nameof(CP));
                }
            }
        }
        private string poblacion="";
        public string Poblacion
        {
            get { return poblacion; }
            set
            {
                if (poblacion != value)
                {
                    poblacion = value;
                    OnPropertyChanged(nameof(Poblacion));
                    if (!string.IsNullOrEmpty(Poblacion))
                        CargaCodPostales();
                }
            }
        }
        private ObservableCollection<string> provincias;
        public ObservableCollection<string> Provincias
        {
            get
            {
                return provincias;
            }
            set
            {
                if (provincias != value)
                {
                    provincias = value;
                    OnPropertyChanged(nameof(Provincias));

                }
            }
        }
        private ObservableCollection<string> pueblos;
        public ObservableCollection<string> Pueblos
        {
            get
            {
                return pueblos;
            }
            set
            {
                if (pueblos != value)
                {
                    pueblos = value;
                    OnPropertyChanged(nameof(Pueblos));
                }
            }
        }
        private ObservableCollection<CodigosPostales> codPostales;
        public ObservableCollection<CodigosPostales> CodPostales
        {
            get
            {
                return codPostales;
            }
            set
            {
                if (codPostales != value)
                {
                    codPostales = value;
                    OnPropertyChanged(nameof(CodPostales));
                }
            }
        }
        private string provincia="";
        public string Provincia
        {
            get { return provincia; }
            set
            {
                if (provincia != value)
                {
                    provincia = value;
                    OnPropertyChanged(nameof(Provincia));
                    CargaPueblos();
                }
            }
        }
        private string iban="";
        public string IBAN
        {
            get { return iban; }
            set
            {
                if (iban != value)
                {
                    iban = value;
                    OnPropertyChanged(nameof(IBAN));
                }
            }
        }
        private string telefono="";
        public string Telefono
        {
            get { return telefono; }
            set
            {
                if (telefono != value)
                {
                    telefono = value;
                    OnPropertyChanged(nameof(Telefono));
                }
            }
        }
        #endregion
        #region Comandos
        public ICommand GuardarConfiguracionEstablecimientoCommand { get { return new Command(GuardarConfiguracionEstablecimiento); } }
        #endregion
        #region Funciones
        public static bool ValidateBankAccount(string bankAccount)
        {
            try
            {
                bankAccount = bankAccount.ToUpper().Replace("-",""); //IN ORDER TO COPE WITH THE REGEX BELOW
                if (String.IsNullOrEmpty(bankAccount))
                    return false;
                else if (System.Text.RegularExpressions.Regex.IsMatch(bankAccount, "^[A-Z0-9]"))
                {
                    bankAccount = bankAccount.Replace(" ", String.Empty);
                    string bank =
                    bankAccount.Substring(4, bankAccount.Length - 4) + bankAccount.Substring(0, 4);
                    int asciiShift = 55;
                    StringBuilder sb = new StringBuilder();
                    foreach (char c in bank)
                    {
                        int v;
                        if (Char.IsLetter(c)) v = c - asciiShift;
                        else v = int.Parse(c.ToString());
                        sb.Append(v);
                    }
                    string checkSumString = sb.ToString();
                    int checksum = int.Parse(checkSumString.Substring(0, 1));
                    for (int i = 1; i < checkSumString.Length; i++)
                    {
                        int v = int.Parse(checkSumString.Substring(i, 1));
                        checksum *= 10;
                        checksum += v;
                        checksum %= 97;
                    }
                    return checksum == 1;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void GuardarConfiguracionEstablecimiento(object obj)
        {
            try
            {
                if (string.IsNullOrEmpty(RazonSocial) || string.IsNullOrEmpty(Direccion) || string.IsNullOrEmpty(CIF)
                    || CP==null || string.IsNullOrEmpty(IBAN) || string.IsNullOrEmpty(Poblacion)
                    || string.IsNullOrEmpty(Provincia) || string.IsNullOrEmpty(Telefono))
                {
                    App.customDialog.ShowDialogAsync(AppResources.TodosCamposObligatorios, AppResources.App, AppResources.Cerrar);
                }else if (ValidateBankAccount(IBAN))
                {
                    fiscal.razonSocial = RazonSocial;
                    fiscal.direccion = Direccion;
                    fiscal.cif = CIF;
                    fiscal.cp = CP.codPostal;
                    fiscal.iban = IBAN.Replace("-","");
                    fiscal.poblacion = Poblacion;
                    fiscal.provincia = Provincia;
                    fiscal.telefono = Telefono;
                    if (fiscal.idEstablecimiento == 0)
                        fiscal.idEstablecimiento = App.EstActual.idEstablecimiento;

                    EstablecimientoFiscalModel result = App.ResponseWS.actualizaDatosFiscalesEstablecimiento(fiscal);
                    if (result != null)
                    {
                        fiscal = result;
                        App.customDialog.ShowDialogAsync(AppResources.DatosModificadosOK, AppResources.App, AppResources.Cerrar);
                    }
                    else
                        App.customDialog.ShowDialogAsync(AppResources.DatosModificadosKO, AppResources.SoloError, AppResources.Cerrar);
                }else
                    App.customDialog.ShowDialogAsync(AppResources.IBANIncorrecto, AppResources.App, AppResources.Cerrar);
            }
            catch (Exception ex)
            {
                // 
                App.customDialog.ShowDialogAsync(AppResources.DatosModificadosKO,AppResources.SoloError, AppResources.Cerrar);
            }
        }
        private void CargaCodPostales()
        {
            CodPostales = new ObservableCollection<CodigosPostales>();
            foreach (PueblosModel p in poblaciones.Where(n => n.Provincia.ToUpper().Equals(Provincia.ToUpper()) && n.nombre.ToUpper().Equals(Poblacion.ToUpper())).OrderBy(pr => pr.codPostal))
            {
                foreach (string a in p.codPostal.Split(','))
                {
                    CodigosPostales c = new CodigosPostales();
                    c.codPostal = a;
                    c.idPueblo = p.id;
                    CodPostales.Add(c);
                }
            }
            CP = CodPostales[0];
        }

        private void CargaPueblos()
        {
            Pueblos = new ObservableCollection<string>();
            foreach (PueblosModel p in poblaciones.Where(n => n.Provincia.ToUpper().Equals(Provincia.ToUpper())).OrderBy(pr => pr.nombre))
            {
                if (!Pueblos.Contains(p.nombre))
                    Pueblos.Add(p.nombre);
            }
            Poblacion = Pueblos[0];
        }
        private async Task recargarDatosAsync()
        {
            try
            {
                EstablecimientoFiscalModel f = ResponseServiceWS.getDatosFiscalesEstablecimiento(App.EstActual.idEstablecimiento);
                if (!string.IsNullOrEmpty(f.razonSocial))
                {
                    if (!RazonSocial.Equals(f.razonSocial))
                        RazonSocial = f.razonSocial;
                }
                if (!string.IsNullOrEmpty(f.direccion))
                {
                    if (!Direccion.Equals(f.direccion))
                        Direccion = f.direccion;
                }
                if (!string.IsNullOrEmpty(f.cp))
                {
                    if (!CP.Equals(f.cp))
                        CP = CodPostales.Where(p=>p.codPostal.Equals(f.cp)).FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(f.poblacion))
                {
                    if (!Poblacion.Equals(f.poblacion))
                        Poblacion = f.poblacion;
                }
                if (!string.IsNullOrEmpty(f.provincia))
                {
                    if (!Provincia.Equals(f.provincia))
                        Provincia = f.provincia;
                }
                if (!string.IsNullOrEmpty(f.telefono))
                {
                    if (!Telefono.Equals(f.telefono))
                        Telefono = f.telefono;
                }
                if (!string.IsNullOrEmpty(f.cif))
                {
                    if (!CIF.Equals(f.cif))
                        CIF = f.cif;
                }
                if (!string.IsNullOrEmpty(f.iban))
                {
                    if (!IBAN.Equals(f.iban))
                        IBAN = f.iban;
                }
            }

            catch (Exception ex)
            {
                // 
            }
        }
        #endregion
        public DatosFiscalesEstablecimientoViewModel()
        {
        }
        public override async Task InitializeAsync(object navigationData)
        {
            App.DAUtil.EnTimer = false;
            Provincias = new ObservableCollection<string>();
            foreach (PueblosModel p in poblaciones.OrderBy(pr => pr.Provincia))
            {
                if (!Provincias.Contains(p.Provincia))
                    Provincias.Add(p.Provincia);
            }
            fiscal = App.DAUtil.GetDatosFiscalesEstablecimiento(App.EstActual.idEstablecimiento);
            try
            {
                if (!string.IsNullOrEmpty(fiscal.razonSocial))
                    RazonSocial = fiscal.razonSocial;
                if (!string.IsNullOrEmpty(fiscal.direccion))
                    Direccion = fiscal.direccion;
                if (!string.IsNullOrEmpty(fiscal.telefono))
                    Telefono = fiscal.telefono;
                
                if (!string.IsNullOrEmpty(fiscal.provincia))
                {
                    Provincia = fiscal.provincia;
                    Poblacion = fiscal.poblacion;
                }
                else
                    Provincia = Provincias[0];
                if (!string.IsNullOrEmpty(fiscal.cp))
                    CP = CodPostales.Where(p => p.codPostal.Equals(fiscal.cp)).FirstOrDefault();
                if (!string.IsNullOrEmpty(fiscal.cif))
                    CIF =fiscal.cif;
                if (!string.IsNullOrEmpty(fiscal.iban))
                    IBAN =fiscal.iban;
            }catch (Exception)
            {

            }
            finally
            {
                App.userdialog.HideLoading();
            }

            await base.InitializeAsync(navigationData).ContinueWith(task => MainThread.BeginInvokeOnMainThread(() => {
                recargarDatosAsync();
                App.userdialog.HideLoading(); }));
        }
    }
}
