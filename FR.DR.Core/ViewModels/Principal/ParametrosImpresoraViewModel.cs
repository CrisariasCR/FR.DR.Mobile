using System;
//using System.Net;
using System.Collections.Generic;
using System.Linq;
//using System.Windows;
using System.Windows.Forms;
using EMF.Printing;
using System.Windows.Input;
using Cirrious.MvvmCross.Commands;
using Softland.ERP.FR.Mobile.Cls.Reporte;
using FR.Core.Model;
using Softland.ERP.FR.Mobile;
using Softland.ERP.FR.Mobile.Cls; // FrmConfig
using Softland.ERP.FR.Mobile.Cls.Cobro;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.UI; //GlobalUI
using Softland.ERP.FR.Mobile.ViewModels;
using FR.DR.Core.Helper;
using Softland.ERP.FR.Mobile.Cls.AccesoDatos;
using Android.Bluetooth;


namespace Softland.ERP.FR.Mobile.ViewModels
{
    public class ParametrosImpresoraViewModel : DialogViewModel<bool>
    {
        
        #region Constructor
        public ParametrosImpresoraViewModel() 
            :base(null)
        {
            CargarCombo();
        }
        #endregion

        #region Propiedades

        public IObservableCollection<string> impresoras { get; set; }
        public IObservableCollection<string> Impresoras
        {
            get { return impresoras; }
            set
            {
                impresoras = value;
                RaisePropertyChanged("Impresoras");
            }
        }
        private string impresoraActual;
        public string ImpresoraActual
        {
            get { return impresoraActual; }
            set
            {
                impresoraActual = value;
                RaisePropertyChanged("ImpresoraActual"); 
            }
        }

        public IObservableCollection<double> papeles { get; set; }
        public IObservableCollection<double> Papeles
        {
            get { return papeles; }
            set
            {
                papeles = value;
                RaisePropertyChanged("Papeles");
            }
        }
        private double papelActual;
        public double PapelActual
        {
            get { return papelActual; }
            set
            {
                papelActual = value;
                RaisePropertyChanged("PapelActual");
            }
        }

        private bool encendiendo=false;

        public bool SugerirImpresion 
        {
            get { return Impresora.SugerirImprimir; }
        }

        #endregion

        #region Comandos

        public ICommand ComandoCancelar
        {
            get { return new MvxRelayCommand(Cancelar); }
        }

        public ICommand ComandoRefrescar
        {
            get { return new MvxRelayCommand(Refrescar); }
        }

        public ICommand ComandoAceptar
        {
            get { return new MvxRelayCommand(Aceptar); }
        }

        #endregion

        #region Metodos		

        /// <summary>
        /// Evento que se dispara en el momento de 
        /// hacer click sobre el image button de cancelar
        /// </summary>
        public void Cancelar()
        {
            this.mostrarMensaje(Mensaje.Accion.Cancelar, "la configuración", resp =>
            {
                if (resp.Equals(DialogResult.Yes) || resp.Equals(DialogResult.OK))
                {
                    this.DoClose();
                }
            }
                );
        }

        /// <summary>
        /// Evento que se dispara en el momento de 
        /// hacer click sobre el image button de aceptar
        /// </summary>
        public void Aceptar()
        {

            //Impresora.IndicaImpresora(this.ImpresoraActual);
            //Impresora.IndicaPuerto(this.PuertoActual);
            //Impresora.ActualizarDatos(); 
            FRmConfig.TamañoPapel = PapelActual;
            string sql = "update " + Table.ERPADMIN_GLOBALES_FR + " set TAMANOPAPEL=" + PapelActual;
            if (!ExisteColumna2())
            {
                try
                {
                    GestorDatos.cnx.ExecuteNonQuery("ALTER TABLE ERPADMIN_GLOBALES_FR ADD COLUMN TAMANOPAPEL numeric");
                }
                catch
                {
                    this.mostrarAlerta("Error Creando columna TAMANOPAPEL en base de datos");
                }
                try
                {
                    GestorDatos.cnx.ExecuteNonQuery(sql);
                }
                catch
                {
                    this.mostrarAlerta("Error Actualizando columna TAMANOPAPEL en base de datos");
                }

            }
            else
            {
                try
                {
                    GestorDatos.cnx.ExecuteNonQuery(sql);
                }
                catch
                {
                    this.mostrarAlerta("Error Actualizando columna TAMANOPAPEL en base de datos");
                }
            }
            if (BluetoothAdapter.DefaultAdapter != null)
            {
                if (BluetoothAdapter.DefaultAdapter.BondedDevices.Count > 0)
                {
                    FRmConfig.Impresora = ImpresoraActual;
                    sql = "update " + Table.ERPADMIN_GLOBALES_FR + " set NOMBREIMPRESORA='" + ImpresoraActual + "'";
                    if (!ExisteColumna())
                    {
                        try
                        {
                            GestorDatos.cnx.ExecuteNonQuery("ALTER TABLE ERPADMIN_GLOBALES_FR ADD COLUMN NOMBREIMPRESORA varchar");
                        }
                        catch
                        {
                            this.mostrarAlerta("Error Creando columna NOMBREIMPRESORA en base de datos");
                        }
                        try
                        {
                            GestorDatos.cnx.ExecuteNonQuery(sql);
                        }
                        catch
                        {
                            this.mostrarAlerta("Error Actualizando columna NOMBREIMPRESORA en base de datos");
                        }

                    }
                    else
                    {
                        try
                        {
                            GestorDatos.cnx.ExecuteNonQuery(sql);
                        }
                        catch
                        {
                            this.mostrarAlerta("Error Actualizando columna NOMBREIMPRESORA en base de datos");
                        }
                    }
                }
            }            
            this.DoClose();
        }

        public void Refrescar() 
        {
            if(encendiendo)
            {
                encendiendo = false;
                return;
            }
            if (BluetoothAdapter.DefaultAdapter == null)
            {
                this.mostrarAlerta("El dispositivo no soporta Bluetooth");
                List<string> Impresoras = new List<string>();
                Impresoras.Add("Bluetooth no soportado");
                this.Impresoras = new SimpleObservableCollection<string>(Impresoras);
                return;
            }
            if (!BluetoothAdapter.DefaultAdapter.IsEnabled)
            {
                this.mostrarMensaje(Mensaje.Accion.Decision, " activar el bluetooth para seleccionar la impresora", res =>
                {
                    if (res.Equals(DialogResult.Yes))
                    {
                        BluetoothAdapter.DefaultAdapter.Enable();
                        encendiendo = true;
                        this.Refrescar();
                    }
                    else
                    {
                        List<string> Impresoras = new List<string>();
                        Impresoras.Add("Bluetooth Apagado");
                        this.Impresoras = new SimpleObservableCollection<string>(Impresoras);
                        return;                        
                    }

                });

            }
            else
            {
                List<string> ImpresorasList = new List<string>();
                if (BluetoothAdapter.DefaultAdapter.BondedDevices.Count > 0)
                {
                    ICollection<BluetoothDevice> btd = BluetoothAdapter.DefaultAdapter.BondedDevices;
                    foreach (BluetoothDevice d in btd)
                    {
                        ImpresorasList.Add(d.Name);
                    }
                }
                else
                {
                    ImpresorasList.Add("Ningun Vinculado");
                    this.mostrarAlerta("No hay ningún dispositivo vinculado");
                }
                this.Impresoras = new SimpleObservableCollection<string>(ImpresorasList);
                ImpresoraActual = Impresoras[0];
                if (!string.IsNullOrEmpty(FRmConfig.Impresora) && (BluetoothAdapter.DefaultAdapter.BondedDevices.Count > 0))
                {
                    SeleccionarPreferencia();
                }
            }
        }

        public void SeleccionarPreferencia() 
        {            
            if (BluetoothAdapter.DefaultAdapter != null && BluetoothAdapter.DefaultAdapter.IsEnabled)
            {
                if (BluetoothAdapter.DefaultAdapter.BondedDevices.Count > 0)
                {
                    foreach (string str in this.Impresoras)
                    {
                        if (str.Equals(FRmConfig.Impresora))
                        {
                            ImpresoraActual = FRmConfig.Impresora;
                        }
                    }
                }
            }

        }

        public void CargarCombo() 
        {
            List<string> ImpresorasList = new List<string>();
            List<double> papers = new List<double> { 2,2.5,3};
            Papeles = new SimpleObservableCollection<double>(papers);
            PapelActual = FRmConfig.TamañoPapel;
            if (BluetoothAdapter.DefaultAdapter != null && BluetoothAdapter.DefaultAdapter.IsEnabled)
            {
                if (BluetoothAdapter.DefaultAdapter.BondedDevices.Count > 0)
                {
                    ICollection<BluetoothDevice> btd = BluetoothAdapter.DefaultAdapter.BondedDevices;
                    foreach (BluetoothDevice d in btd)
                    {
                        ImpresorasList.Add(d.Name);
                    }
                }
                else
                {
                    ImpresorasList.Add("Ningun Vinculado");
                }
                this.Impresoras = new SimpleObservableCollection<string>(ImpresorasList);
                ImpresoraActual = Impresoras[0];
                if (!string.IsNullOrEmpty(FRmConfig.Impresora) && (BluetoothAdapter.DefaultAdapter.BondedDevices.Count > 0))
                {
                    SeleccionarPreferencia();
                }

            }
            else 
            {
                if (BluetoothAdapter.DefaultAdapter == null)
                {
                    ImpresorasList.Add("Bluetooth no soportado");
                    this.Impresoras = new SimpleObservableCollection<string>(ImpresorasList);
                }
                else
                {
                    ImpresorasList.Add("Bluetooth Apagado");
                    this.Impresoras = new SimpleObservableCollection<string>(ImpresorasList);
                }
            }
            
            
        }

        public void Sugerir() 
        {
            Impresora.SugerirImprimir = true;
        }

        public void NoSugerir() 
        {
            Impresora.SugerirImprimir = false;
        }

        public bool ExisteColumna() 
        {
            string sentencia = "select count(*) from sqlite_master where name='" + Table.ERPADMIN_GLOBALES_FR + "' AND sql like '%NOMBREIMPRESORA%'";
            int res = Convert.ToInt16(GestorDatos.cnx.ExecuteScalar(sentencia));
            if (res > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public bool ExisteColumna2()
        {
            string sentencia = "select count(*) from sqlite_master where name='" + Table.ERPADMIN_GLOBALES_FR + "' AND sql like '%TAMANOPAPEL%'";
            int res = Convert.ToInt16(GestorDatos.cnx.ExecuteScalar(sentencia));
            if (res > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        #endregion

    }
}
