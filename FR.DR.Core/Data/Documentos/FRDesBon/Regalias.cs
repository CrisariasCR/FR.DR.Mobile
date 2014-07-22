using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Softland.ERP.FR.Mobile.Cls.Documentos.FRPedido;
using Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon;
using Softland.ERP.FR.Mobile.Cls;
using Softland.ERP.FR.Mobile.Cls.FRArticulo;
using Softland.ERP.FR.Mobile.Cls.FRCliente;
using Softland.ERP.FR.Mobile.Interfaces;
using Softland.ERP.FR.Mobile.Cls.Corporativo; 

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon
{
    public class Regalias
    {
        #region Constantes

        private const string REGLA_FANTASMA = "<SinCodigo>";

        #endregion Constantes

        #region Attributes
        private static String compania;
        private static String cliente;
        private static Pedido pedido;
        private static ClienteCia clienteCia;
        private static String nivelPrecio;
        private static String ruta;

        private static List<LineaRegalia> regalias;
        private static List<Paquete> paquetes;

        private static bool aplicarDescuentosGenerales;
        private static bool tieneBonificaciones = false;
        #endregion

        #region Properties
        /// <summary>
        /// Indica si luego de aplicar el proceso se aplicaron reglas de bonificaci�n.
        /// </summary>
        public static bool TieneBonificaciones
        {
            get { return Regalias.tieneBonificaciones; }
        }
        #endregion

        #region Constructors
        #endregion

        #region Methods

        /// <summary>
        /// Aplica las regalias a las lineas del pedido.
        /// </summary>
        /// <param name="pedido">Pedido al que se le requiere aplicar las regalias. Sera modificado para agregar las
        /// lineas bonificadas y los descuentos aplicados.</param>
        /// <returns></returns>
        public static bool AplicarRegalias(ref Pedido pedido, ref DetallePedido detalle, String nivelPrecio, String ruta, out String mensaje)
        {
            bool resultado = true;
            mensaje = String.Empty;
            aplicarDescuentosGenerales = detalle == null;
            tieneBonificaciones = false;
            try
            {

                    List<ClienteCia> clientes = ClienteCia.ObtenerClientes(pedido.Cliente, false);
                    resultado = clientes.Count > 0;
                    if (resultado)
                    {
                        // Inicializa los atributos de la clase estatica cada vez que inicia el proceso.
                        clienteCia = clientes[0];
                        regalias = new List<LineaRegalia>();
                        Regalias.pedido = pedido;
                        Regalias.nivelPrecio = nivelPrecio;
                        Regalias.ruta = ruta;

                        cliente = pedido.Cliente;
                        // Si es la primera vez que se ejecuta o cambio de compa�ia se cargan los paquetes con las reglas.
                        if (compania == null || compania != pedido.Compania || paquetes == null)
                        {
                            compania = pedido.Compania;
                            paquetes = Paquete.FindAll(compania);
                        }

                        // Verifica si existe alg�n paquete activo
                        if (paquetes != null && paquetes.Count > 0)
                        {

                            if (detalle == null)
                            {
                                // Aplica las reglas para todos los detalles.

                                // Limpia los detalles (descuentos y bonificaci�n)
                                LimpiarDetalles(pedido);
                                // Aplica las reglas marcar para aplicar por grupo
                                AplicarRegaliasGrupo(paquetes, pedido);
                                foreach (DetallePedido linea in pedido.Detalles.Lista)
                                {
                                    // Aplica la reglas al detalle dado.
                                    AplicarRegaliasDetalle(paquetes, linea);
                                }
                                AplicarRegaliaDescuentoGeneral(paquetes);
                            }
                            else
                            {
                                // Aplica las reglas para el detalle dado.
                                LimpiarDetalles(pedido);
                                AplicarRegaliasGrupo(paquetes, pedido);
                                AplicarRegaliasDetalle(paquetes, detalle);
                                AplicarRegaliaDescuentoGeneral(paquetes);
                            }
                        }
                    }
                    else
                    {
                        mensaje = "Paquetes y Reglas de Descuentos-Bonificaciones: No se ha podido obtener la informaci�n del cliente asociada al pedido.";
                    }
                
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = "Paquetes y Reglas de Descuentos-Bonificaciones:" + ex.Message;
            }
            return resultado;
        }

        /// <summary>
        /// Libera las instancias de los objetos utilizados.
        /// </summary>
        public static void Liberar()
        {
            compania = null;
            cliente = null;
            pedido = null;
            clienteCia = null;
            regalias = null;
            paquetes = null;
        }

        /// <summary>
        /// Limpia los detalles con reglas aplicadas anteriormente.
        /// </summary>
        /// <param name="pedido"></param>
        private static void LimpiarDetalles(Pedido pedido)
        {
            decimal montoDesc = 0;

            // Limpia el descuento general
            pedido.PorcentajeDescuento2 = 0;


            //pedido.MontoDescuentoLineas = 0;  

            pedido.RegaliaDescuentoGeneral = null;

            foreach (DetallePedido detalle in pedido.Detalles.Lista)
            {
                montoDesc = detalle.Descuento == null ? 0 : detalle.Descuento.Monto;
                TipoDescuento tip = detalle.Descuento == null ? TipoDescuento.Porcentual : detalle.Descuento.Tipo;

                CambiarDescuento(detalle, montoDesc, tip, clienteCia, 0);

                detalle.RegaliaBonificacion = null;

                detalle.RegaliaDescuentoLinea = null;
            }
        }

        /// <summary>
        /// Verifica que solo se aplique una regla por Bonificaci�n, una al descuento general
        /// y una al descuento de la linea.
        /// </summary>
        /// <param name="paquetes"></param>
        /// <param name="detalle"></param>
        private static void AplicarRegaliasGrupo(List<Paquete> paquetes, Pedido pedido)
        {
            List<Regla> reglasAplicables = new List<Regla>();
            bool aplicaReglaMontoMin = false;
            List<DetallePedido> LineasDocGrupo = new List<DetallePedido>();
            Dictionary<string, List<DetallePedido>> detallesPorRegla = new Dictionary<string, List<DetallePedido>>();
            LineaRegalia regaliaAplicada = null;
            List<Regla> reglaFiltradas = new List<Regla>();
            List<string> articuloAplicables = null;
            DetallePedido lineaDocumento = null;
            decimal cantidadVerificar = 0;
            decimal subTotalDocumento = 0;

            foreach (Paquete paquete in paquetes)
            {
                // Verifica la fecha, los d�as y las horas del paquete.
                if (paquete.Verificar(DateTime.Now) && paquete.VerificaRuta(Regalias.ruta))
                {
                    reglasAplicables.Clear();
                    detallesPorRegla.Clear();

                    //Se buscan las reglas asociadas al paquete que sean de descuento de l�nea
                    var reglasDescLinea = (from reglasLinea in paquete.Reglas
                                           where reglasLinea.Tipo == Constantes.TIPO_REGLA_DESCUENTO &&
                                           reglasLinea.TipoDescuento == Constantes.TIPO_DESCUENTO_LINEA &&
                                               //reglasLinea.Tipo == Constantes.TIPO_VALOR_PORCENTAGE &&
                                           reglasLinea.AplicacionGrupos
                                           select reglasLinea);


                    #region Descuentos de L�nea En Grupo

                    foreach (Regla regla in reglasDescLinea)
                    {
                        aplicaReglaMontoMin = (regla.MontoMinimo == 0 || regla.MontoMinimo <= pedido.MontoNeto);

                        if (aplicaReglaMontoMin && VerificarFiltroCliente(regla.FiltroCliente))
                        {
                            articuloAplicables = ObtenerArticulosAplicables(regla.Compania, regla.FiltroArticulo);
                            var lineaAplicarGrupo = from lin in pedido.Detalles.Lista
                                                    where lin.RegaliaDescuentoLinea == null &&
                                                    !lin.EsBonificada &&
                                                    VerificarFiltroArticulo(lin.Articulo.Codigo, articuloAplicables)
                                                    select lin;
                            if (lineaAplicarGrupo != null)
                            {
                                LineasDocGrupo = lineaAplicarGrupo.ToList();                               

                                //Se obtiene la suma de las cantidades de las l�neas
                                cantidadVerificar = LineasDocGrupo.Sum(lin => lin.UnidadesAlmacen + (lin.UnidadesDetalle / lin.Articulo.UnidadEmpaque));

                                //Se verifica el rango de cantidades pero con la suma de todas las l�neas que aplican al filtro de art�culo para la regla
                                //Como es la suma de todas las l�neas no se valida unidades de detalle
                                if (VerificarDescuento(cantidadVerificar, 1, regla))
                                {
                                    if (!reglasAplicables.Contains(regla))
                                    {
                                        reglasAplicables.Add(regla);
                                        //Se asocia la regla aplicable con las l�neas por las cu�les le hacen aplicar
                                        detallesPorRegla.Add(regla.Codigo, LineasDocGrupo);
                                    }
                                }
                            }
                        }
                    }

                    if (reglasDescLinea.Count() > 0 && reglasAplicables.Count > 0)
                    {
                        regaliaAplicada = ObtenerRegaliaAplicada(reglasAplicables, Constantes.TIPO_DESCUENTO_LINEA, ref detallesPorRegla);

                        if (regaliaAplicada != null)
                        {
                            //Verificar si el descuento aplica a la l�nea cuando es por monto, pues podr�a ser que el monto de descuento sea mayor al subtotal de la l�nea
                            if (regaliaAplicada.Regla.TipoValor == Constantes.TIPO_VALOR_MONTO)
                            {
                                subTotalDocumento = detallesPorRegla[regaliaAplicada.Regla.Codigo].Sum(lin => lin.MontoTotal);

                                if (regaliaAplicada.MontoDescuento > subTotalDocumento)
                                {
                                    //Si el monto del descuento es mayor a la suma de los subtotales de las l�neas utilizadas en la aplicaci�n de la regla no aplica el descuento
                                    regaliaAplicada = null;
                                }

                                //Las reglas de descuento en Grupo s�lo aplican en porcentaje, por lo que se debe convertir el monto a un porcentaje que se pueda aplicar
                                regaliaAplicada.PorcentajeDescuento = (regaliaAplicada.MontoDescuento / subTotalDocumento) * 100;
                                regaliaAplicada.MontoDescuento = 0;
                            }

                            if (regaliaAplicada != null)
                            {
                                AplicarLineaRegaliaDescuento(regaliaAplicada, detallesPorRegla[regaliaAplicada.Regla.Codigo]);
                            }
                        }
                    }
                    #endregion Descuentos de L�nea En Grupo


                    #region Bonificaciones En Grupo

                    reglasAplicables.Clear();
                    detallesPorRegla.Clear();

                    regaliaAplicada = null;

                    //Se buscan las reglas asociadas al paquete que sean de descuento de l�nea
                    var reglasDescBonificacion = (from reglasLinea in paquete.Reglas
                                                  where reglasLinea.Tipo == Constantes.TIPO_REGLA_BONIFICACION &&
                                                  reglasLinea.AplicacionGrupos //!reglasLinea.AplicacionGrupos
                                                  select reglasLinea);

                    if (Regalias.pedido.Configuracion.Compania.ModoDescuentosMultiples == ModoDescuentoMultipleEnum.PrimeroSegunPrioridad)
                    {
                        //Si aplica la regla seg�n la prioridad, se ordenan las reglas seg�n su prioridad para que verifique primero las de prioridad m�s alta (ascendente)
                        reglaFiltradas = reglasDescBonificacion.OrderBy(reg => reg.Prioridad).ToList();
                    }
                    else
                    {
                        reglaFiltradas = reglasDescBonificacion.ToList();
                    }

                    //Obtener la lista de reglas que aplican seg�n los datos de la l�nea del documento
                    foreach (Regla regla in reglaFiltradas)
                    {
                        aplicaReglaMontoMin = (regla.MontoMinimo == 0 || regla.MontoMinimo <= Regalias.pedido.MontoNeto);

                        //Se hacen primero verificaciones sin acceso a base de datos para mejor rendimiento
                        if (aplicaReglaMontoMin)
                        {
                            if (VerificarFiltroCliente(regla.FiltroCliente))
                            {
                                if (!reglasAplicables.Contains(regla))
                                {
                                    reglasAplicables.Add(regla);
                                }
                            }
                        }
                    }
                    //Si hay reglas por aplicar se van a aplicar las mismas
                    if (reglasDescBonificacion.Count() > 0 && reglasAplicables.Count > 0)
                    {
                        lineaDocumento = null;

                        LineasDocGrupo = pedido.Detalles.Lista;

                        regaliaAplicada = ObtenerRegaliaBonificacion(reglasAplicables, ref LineasDocGrupo, out lineaDocumento);

                        if (lineaDocumento != null && regaliaAplicada != null)
                        {
                            //Buscar la l�nea de detalle que recibe la bonificacion

                            int lineaConBonificacion = LineasDocGrupo.FindIndex(delegate(DetallePedido seekDetalle)
                            {
                                return seekDetalle.Equals(lineaDocumento);
                            });

                            if (lineaConBonificacion >= 0)
                            {
                                // Obtiene la linea de bonificaci�n con las cantidades ya asignadas seg�n la escala.          
                                pedido.Detalles.Lista[lineaConBonificacion].LineaBonificada = regaliaAplicada.ObtenerLineaBonificacion(clienteCia);
                            }

                        }
                    }

                    #endregion Bonificaciones En Grupo
                }
            }
        }


        /// <summary>
        /// Verifica que solo se aplique una regla por Bonificaci�n, una al descuento general
        /// y una al descuento de la linea.
        /// </summary>
        /// <param name="paquetes"></param>
        /// <param name="detalle"></param>
        private static void AplicarRegaliasDetalle(List<Paquete> paquetes, DetallePedido detalle)
        {
            bool aplicaReglaMontoMin = false;
            List<Regla> reglasAplicables = new List<Regla>();
            List<Regla> reglaFiltradas = new List<Regla>();
            List<DetallePedido> LineasPorAplicar = new List<DetallePedido>();
            Dictionary<string, List<DetallePedido>> detallesPorRegla = new Dictionary<string, List<DetallePedido>>();
            LineaRegalia regaliaAplicada = null;
            DetallePedido lineaDocumento = null;
            List<DetallePedido> LineasDoc = null;


            decimal cantidadPedida = 0;

            foreach (Paquete paquete in paquetes)
            {
                reglasAplicables.Clear();
                detallesPorRegla.Clear();

                // Verifica la fecha, los d�as y las horas del paquete.
                if (paquete.Verificar(DateTime.Now) && paquete.VerificaRuta(Regalias.ruta))
                {
                    #region Aplicaci�n de Descuento de L�nea

                    //Si la l�nea no tiene ya aplicado un descuento de l�nea se verifica si le aplica
                    if (detalle.RegaliaDescuentoLinea == null)
                    {
                        regaliaAplicada = null;
                        var reglasDescLinea = (from reglasLinea in paquete.Reglas
                                               where reglasLinea.Tipo == Constantes.TIPO_REGLA_DESCUENTO &&
                                               reglasLinea.TipoDescuento == Constantes.TIPO_DESCUENTO_LINEA &&
                                               !reglasLinea.AplicacionGrupos
                                               select reglasLinea);

                        //Obtener la lista de reglas que aplican seg�n los datos de la l�nea del documento
                        foreach (Regla regla in reglasDescLinea)
                        {
                            LineasPorAplicar.Clear();
                            aplicaReglaMontoMin = (regla.MontoMinimo == 0 || regla.MontoMinimo <= Regalias.pedido.MontoNeto);
                            cantidadPedida = detalle.UnidadesAlmacen + (detalle.UnidadesDetalle / detalle.Articulo.UnidadEmpaque);
                            if (aplicaReglaMontoMin && VerificarDescuento(cantidadPedida, detalle.Articulo.UnidadEmpaque, regla))
                            {
                                // Verifica que el cliente cumpla el filtro de la regla y que el articulo del detalle se encuentre dentro del filtro de articulos de la regla.
                                if (VerificarFiltroCliente(regla.FiltroCliente) && VerificarFiltroArticulo(detalle.Articulo.Compania, detalle.Articulo.Codigo, regla.FiltroArticulo))
                                {
                                    LineasPorAplicar.Add(detalle);
                                    if (!reglasAplicables.Contains(regla))
                                    {
                                        reglasAplicables.Add(regla);
                                        //Se asocia la regla aplicable con las l�neas por las cu�les le hacen aplicar
                                        detallesPorRegla.Add(regla.Codigo, LineasPorAplicar);
                                    }
                                }
                            }
                        }
                        if (reglasDescLinea.Count() > 0 && reglasAplicables.Count > 0)
                        {
                            regaliaAplicada = ObtenerRegaliaAplicada(reglasAplicables, Constantes.TIPO_DESCUENTO_LINEA, ref detallesPorRegla);
                            if (regaliaAplicada != null)
                            {
                                //Verificar si el descuento aplica a la l�nea cuando es por monto, pues podr�a ser que el monto de descuento sea mayor al subtotal de la l�nea
                                if (regaliaAplicada.Regla.TipoValor == Constantes.TIPO_VALOR_MONTO)
                                {
                                    if (regaliaAplicada.MontoDescuento > detallesPorRegla[regaliaAplicada.Regla.Codigo].Sum(lin => lin.MontoTotal))
                                    {
                                        //Si el monto del descuento es mayor al subtotal de la l�nea no aplica el descuento
                                        regaliaAplicada = null;
                                    }
                                }
                                if (regaliaAplicada != null)
                                {
                                    // Aplica la regla al detalle
                                    regaliaAplicada = AplicarLineaRegalia(regaliaAplicada.Regla, detalle);

                                    regalias.Add(regaliaAplicada);
                                    tieneBonificaciones = tieneBonificaciones || regaliaAplicada.Tipo == TipoRegalia.Bonificacion;
                                }
                            }
                        }
                    }
                    #endregion Aplicaci�n de Descuento de L�nea

                    #region Aplicacion de Bonificaciones por L�nea

                    //Si la l�nea no tiene ya aplicado una bonificaci�n se verifica si le aplica
                    if (detalle.RegaliaBonificacion == null)
                    {
                        reglasAplicables.Clear();
                        detallesPorRegla.Clear();
                        regaliaAplicada = null;
                        //Se buscan las reglas asociadas al paquete que sean de descuento de l�nea
                        var reglasDescBonificacion = (from reglasLinea in paquete.Reglas
                                                      where reglasLinea.Tipo == Constantes.TIPO_REGLA_BONIFICACION &&
                                                      !reglasLinea.AplicacionGrupos
                                                      select reglasLinea);
                        if (Regalias.pedido.Configuracion.Compania.ModoDescuentosMultiples == ModoDescuentoMultipleEnum.PrimeroSegunPrioridad)
                        {
                            //Si aplica la regla seg�n la prioridad, se ordenan las reglas seg�n su prioridad para que verifique primero las de prioridad m�s alta (ascendente)
                            reglaFiltradas = reglasDescBonificacion.OrderBy(reg => reg.Prioridad).ToList();
                        }
                        else
                        {
                            reglaFiltradas = reglasDescBonificacion.ToList();
                        }
                        //Obtener la lista de reglas que aplican seg�n los datos de la l�nea del documento
                        foreach (Regla regla in reglaFiltradas)
                        {
                            aplicaReglaMontoMin = (regla.MontoMinimo == 0 || regla.MontoMinimo <= Regalias.pedido.MontoNeto);
                            //Se hacen primero verificaciones sin acceso a base de datos para mejor rendimiento
                            if (aplicaReglaMontoMin)
                            {
                                if (VerificarFiltroCliente(regla.FiltroCliente))
                                {
                                    if (!reglasAplicables.Contains(regla))
                                    {
                                        reglasAplicables.Add(regla);
                                    }
                                }
                            }
                        }
                        //Si hay reglas por aplicar se van a aplicar las mismas
                        if (reglasDescBonificacion.Count() > 0 && reglasAplicables.Count > 0)
                        {
                            lineaDocumento = null;
                            LineasDoc = new List<DetallePedido>();
                            //S�lo se asigna, como l�nea a revisar, el detalle que se pas� por par�metro a este m�todo
                            LineasDoc.Add(detalle);
                            regaliaAplicada = ObtenerRegaliaBonificacion(reglasAplicables, ref LineasDoc, out lineaDocumento);
                            if (lineaDocumento != null && regaliaAplicada != null)
                            {
                                // Obtiene la linea de bonificaci�n con las cantidades ya asignadas seg�n la escala.          
                                detalle.LineaBonificada = regaliaAplicada.ObtenerLineaBonificacion(clienteCia);
                                detalle.RegaliaBonificacion = regaliaAplicada;
                            }
                            LineasDoc.Clear();
                            LineasDoc = null;
                        }
                    }

                    #endregion Aplicacion de Bonificaciones por L�nea
                }
            }
        }
        

        /// <summary>
        /// Aplica la reglia seg�n el tipo de regla.
        /// </summary>
        /// <param name="regla"></param>
        /// <param name="detalle"></param>
        /// <returns></returns>
        private static LineaRegalia AplicarLineaRegalia(Regla regla, DetallePedido detalle)
        {
            LineaRegalia resultado = null;
            if (regla.Tipo == Constantes.TIPO_REGLA_DESCUENTO)
            {
                // Aplica la regla como un descuento.
                resultado = AplicarLineaRegaliaDescuento(regla, detalle);
            }
            return resultado;
        }


        /// <summary>
        /// Aplica la regalia de descuento seg�n el tipo de descuento (General | Linea)
        /// El descuento general se carga sobre monto de descuento1.
        /// </summary>
        /// <param name="regla"></param>
        /// <param name="detalle"></param>
        /// <returns></returns>
        private static LineaRegalia AplicarLineaRegaliaDescuento(Regla regla, DetallePedido detalle)
        {
            LineaRegalia resultado = null;
            decimal monto = 0;
            decimal porcentaje = 0;
            TipoRegalia tipo = TipoRegalia.DescuentoLinea;


            //decimal cantidadPedida = detalle.UnidadesAlmacen + (detalle.UnidadesDetalle / detalle.Articulo.UnidadEmpaque);


            if (regla.TipoDescuento == Constantes.TIPO_DESCUENTO_LINEA)
            {
                //A PRUEBA
                decimal cantidadPedida = detalle.UnidadesAlmacen + (detalle.UnidadesDetalle / detalle.Articulo.UnidadEmpaque);

                TipoDescuento tipoDescuento = regla.TipoValor == Constantes.TIPO_VALOR_MONTO ? TipoDescuento.Fijo : TipoDescuento.Porcentual;
                tipo = TipoRegalia.DescuentoLinea;
                // Si el descuento es por monto se calcula el porcentage y se aplica por porcentage
                porcentaje = regla.TipoValor == Constantes.TIPO_VALOR_MONTO ? Decimal.Round((regla.Valor * 100 / detalle.MontoTotal), 2) : regla.Valor;
                CambiarDescuento(detalle, regla, tipoDescuento);
                resultado = new LineaRegalia(regla, monto, porcentaje, tipo);
                detalle.RegaliaDescuentoLinea = resultado;
            }
            else if (regla.TipoDescuento == Constantes.TIPO_DESCUENTO_GENERAL)
            {
                // Calcula el monto del descuento.
                monto = regla.TipoValor == Constantes.TIPO_VALOR_MONTO ? regla.Valor : Regalias.pedido.MontoBruto * regla.Valor / 100;
                // Calcula el porcentaje de descuento cuando se da el monto.
                porcentaje = regla.TipoValor == Constantes.TIPO_VALOR_MONTO ? (regla.Valor * 100 / pedido.MontoSubTotal) : regla.Valor;
                porcentaje = Decimal.Truncate(porcentaje * 100) / 100;
                tipo = TipoRegalia.DescuentoGeneral;
                pedido.PorcentajeDescuento2 += porcentaje;
                resultado = new LineaRegalia(regla, monto, porcentaje, tipo);
                pedido.RegaliaDescuentoGeneral = resultado;
            }


            return resultado;
        }

        /// <summary>
        /// Aplica la regla al conjunto de detalles dado tomando en cuanta la sumatoria de la cantidad pedida de todas las reglas.
        /// </summary>
        /// <param name="regla"></param>
        /// <param name="detalles"></param>
        /// <param name="cantidadPedida"></param>
        /// <returns></returns>
        private static void AplicarLineaRegaliaDescuento(LineaRegalia RegaliaAplica, List<DetallePedido> detalles)
        {
            if (RegaliaAplica != null)
            {
                // Aplica los descuentos a todas las lineas del grupo.
                foreach (DetallePedido detalle in detalles)
                {
                    CambiarDescuento(detalle, RegaliaAplica.Regla, TipoDescuento.Porcentual);
                    detalle.RegaliaDescuentoLinea = RegaliaAplica;
                }
            }
        }       

        /// <summary>
        /// Cambia el descuento del detalle del pedido.
        /// </summary>
        /// <param name="detalle"></param>
        /// <param name="regla"></param>
        /// <param name="tipoDescuento"></param>
        private static void CambiarDescuento(DetallePedido detalle, Regla regla, TipoDescuento tipoDescuento)
        {
            decimal descuentogeneral = pedido.PorcDescGeneral;
            pedido.RestarMontosLinea(detalle);
            CambiarDescuento(detalle, regla.Valor, tipoDescuento, clienteCia, descuentogeneral);
            pedido.AgregarMontosLinea(detalle);
            pedido.ReCalcularDescuentosGenerales();
        }

        /// <summary>
        /// Cambia el descunto del detalle del pedido
        /// </summary>
        /// <param name="detalle"></param>
        /// <param name="descuento"></param>
        /// <param name="tipo"></param>
        /// <param name="cliente"></param>
        /// <param name="descuentoGeneral"></param>
        private static void CambiarDescuento(DetallePedido detalle, decimal descuento, TipoDescuento tipo, ClienteCia cliente, decimal descuentoGeneral)
        {
            detalle.Descuento = new Descuento();
            detalle.Descuento.Monto = descuento;
            detalle.Descuento.Tipo = tipo;
            detalle.CalcularImpuestos(cliente.ExoneracionImp1, cliente.ExoneracionImp2, descuentoGeneral);
            detalle.CalcularMontoDescuento();
            detalle.CalcularMontos(cliente, false);
        }

        

        /// <summary>
        /// Verifica que el cliente se encuentre dentro del filtro.
        /// </summary>
        /// <param name="filtroCliente"></param>
        /// <returns></returns>
        private static bool VerificarFiltroCliente(String filtroCliente)
        {
            bool resultado = false;
            if (String.IsNullOrEmpty(filtroCliente))
            {
                resultado = true;
            }
            else
            {
                ConvertidorFiltro convertidor = new ConvertidorFiltro();
                String filtro = new ConvertidorFiltro().ConvertirFiltroDevExASQL(filtroCliente);
                resultado = ProbadorFiltros.ProbarCliente(cliente, filtro);
            }

            return resultado;
        }
        /// <summary>
        /// Verifica que el articulo cumpla con el filtro de articulos de la regla.
        /// </summary>
        /// <param name="articulo"></param>
        /// <param name="filtroArticulos"></param>
        /// <returns></returns>
        private static bool VerificarFiltroArticulo(String compania, String articulo, String filtroArticulos)
        {
            bool resultado = false;
            if (String.IsNullOrEmpty(filtroArticulos))
            {
                resultado = true;
            }
            else
            {
                ConvertidorFiltro convertidor = new ConvertidorFiltro();
                String filtro = new ConvertidorFiltro().ConvertirFiltroDevExASQL(filtroArticulos);
                resultado = ProbadorFiltros.ProbarArticulo(compania, articulo, filtro);
            }

            return resultado;
        }
        /// <summary>
        /// Verifica que el articulo se encuentre dentro de la lista de art�culos especificada
        /// </summary>
        /// <param name="articulo">C�digo de art�culo del cual se verificar� si existe en la lista de art�culos suministrada</param>
        /// <param name="ArticulosFiltrados">Lista de Art�culos en las cuales se verificar� si el art�culo indicado forma parta</param>
        /// <returns>True si el art�culo est� dentro de la lista; False si no se encuentra o bien si la lista de art�culos se encuentra vac�a</returns>
        private static bool VerificarFiltroArticulo(String articulo, List<string> ArticulosFiltrados)
        {
            bool resultado = false;
            if (ArticulosFiltrados.Count == 0)
            {
                resultado = true;
            }
            else
            {
                resultado = ArticulosFiltrados.Contains(articulo);
            }

            return resultado;
        }

        /// <summary>
        /// Busca las reglas de descuento de tipo Descuento General y las aplica
        /// </summary>
        /// <param name="paquetes">Lista de Paquetes de Descuento que se deben verificar</param>
        private static void AplicarRegaliaDescuentoGeneral(List<Paquete> paquetes)
        {
            List<Regla> reglasAplicables = new List<Regla>();
            Dictionary<string, List<DetallePedido>> detallesPorRegla = new Dictionary<string, List<DetallePedido>>();
            List<string> articuloAplicables = null;
            LineaRegalia regaliaAplicada = null;
            bool aplicaReglaMontoMin = false;
            bool aplicaReglaEnArticulo = false;
            bool aplicaReglaEnDesc = false;

            //Solo se verifica si no se ha asignado con anterioridad un descuento General
            if (pedido.RegaliaDescuentoGeneral == null && aplicarDescuentosGenerales)
            {
                foreach (Paquete paquete in paquetes)
                {
                    reglasAplicables.Clear();
                    detallesPorRegla.Clear();

                    // Verifica la fecha, los d�as y las horas del paquete.
                    if (paquete.Verificar(DateTime.Now) && paquete.VerificaRuta(Regalias.ruta))
                    {
                        //Se buscan las reglas asociadas al paquete que sean de descuento General
                        var reglasDescLinea = (from reglasLinea in paquete.Reglas
                                               where reglasLinea.Tipo == Constantes.TIPO_REGLA_DESCUENTO &&
                                               reglasLinea.TipoDescuento == Constantes.TIPO_DESCUENTO_GENERAL &&
                                               !reglasLinea.AplicacionGrupos
                                               select reglasLinea);

                        //Obtener la lista de reglas que aplican seg�n los datos de la l�nea del documento
                        foreach (Regla regla in reglasDescLinea)
                        {
                            //Verifica para la regla en proceso, que el documento cumpla con el monto m�nimo
                            aplicaReglaMontoMin = (regla.MontoMinimo == 0 || regla.MontoMinimo <= Regalias.pedido.MontoNeto);

                            //Se hacen primero verificaciones que no tienen interacci�n con la Base de Datos para que sea m�s r�pido
                            if (aplicaReglaMontoMin)
                            {
                                //Verifica para la regla en proceso, que exista al menos una l�nea dentro del documento que cumpla con el filtro cantidades de la regla
                                aplicaReglaEnDesc = (pedido.Detalles.Lista.Where(lin => VerificarDescuento(lin.UnidadesAlmacen + (lin.UnidadesDetalle / lin.Articulo.UnidadEmpaque), lin.Articulo.UnidadEmpaque, regla)).Count() > 0);

                                if (aplicaReglaEnDesc && VerificarFiltroCliente(regla.FiltroCliente))
                                {
                                    //Se obtiene la lista de art�culos que cumplen con la regla
                                    articuloAplicables = ObtenerArticulosAplicables(regla.Compania, regla.FiltroArticulo);

                                    //Verifica para la regla en proceso, que exista al menos un art�culo dentro del documento que cumpla con el filtro de la regla
                                    aplicaReglaEnArticulo = (pedido.Detalles.Lista.Where(lin => VerificarFiltroArticulo(lin.Articulo.Codigo, articuloAplicables)).Count() > 0);

                                    if (aplicaReglaEnArticulo)
                                    {
                                        if (!reglasAplicables.Contains(regla))
                                        {
                                            reglasAplicables.Add(regla);

                                            //Se asocia la regla aplicable con las l�neas por las cu�les le hacen aplicar
                                            detallesPorRegla.Add(regla.Codigo, pedido.Detalles.Lista);
                                        }
                                    }
                                }
                            }
                        }

                        if (reglasDescLinea.Count() > 0 && reglasAplicables.Count() > 0)
                        {
                            regaliaAplicada = ObtenerRegaliaAplicada(reglasAplicables, Constantes.TIPO_DESCUENTO_GENERAL, ref detallesPorRegla);

                            if (regaliaAplicada != null)
                            {
                                //Verificar si el descuento aplica a la l�nea cuando es por monto, pues podr�a ser que el monto de descuento sea mayor al subtotal de la l�nea
                                if (regaliaAplicada.Regla.TipoValor == Constantes.TIPO_VALOR_MONTO)
                                {
                                    if (regaliaAplicada.MontoDescuento > pedido.MontoBruto)
                                    {
                                        //Si el monto del descuento es mayor a la suma del subtotal no aplica el descuento
                                        regaliaAplicada = null;
                                    }
                                }
                            }

                            pedido.RegaliaDescuentoGeneral = regaliaAplicada;
                        }

                        if (pedido.RegaliaDescuentoGeneral != null)
                        {
                            AplicarLineaRegalia(pedido.RegaliaDescuentoGeneral.Regla, null);

                            //Si ya se aplic� una regla, no se procesan m�s para este documento
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///Verifica que se cumpla la cantidad m�nima y m�xima para el descuento. Por medio del factor de empaque definido tambi�n se toma en cuenta las unidades de detalle
        /// </summary>
        /// <param name="CantidadPedida">Cantida expresada en unidades de alamac�n a verificar</param>
        /// <param name="FactorEmpaque">Factor de empaque a utilizar para convertir unidades de detalle de la regla en unidades de almac�n</param>
        /// <param name="regla">Regla de descuento de la cual se debe verificar la opci�n de validar Cantidad</param>
        /// <returns>True si la cantidad indicada se encuentra dentro del rango establecido por la regla</returns>
        private static bool VerificarDescuento(decimal CantidadPedida, decimal FactorEmpaque, Regla regla)
        {
            return !regla.ValidarCantidad || (regla.ValidarCantidad && CantidadPedida >= regla.CantidadMinima + (regla.CantidadMinimaDetalle / FactorEmpaque) && CantidadPedida <= regla.CantidadMaxima + (regla.CantidadMaximaDetalle / FactorEmpaque));
        }

        /// <summary>
        /// Este m�todo se verifica la parametrizaci�n global para la aplicaci�n de las reglas de descuento que aplican para una l�nea de un documento. 
        /// S�lo se toman reglas de tipo descuento
        /// Opciones:
        ///     Aplica primera regla seg�n la prioridad
        ///     Aplica mayor descuento: todo se convierte a monto para otener el mayor valor
        ///     Aplica acumulando todos: todo se convierte a monto para obtener una suma de todas la reglas. Si el valor es mayor al subtotal no aplica ninguna
        ///     Aplica acumulando cascada: Se obtiene el porcentaje aplicado en cascada, luego ese procentaje se combierte a montos seg�n el subotal de la l�nea y finalmente, se le suma
        ///     a ese resultado la suma de todas las reglas que son de tipo monto. Si el valor es mayor al subtotal no aplica ninguna
        /// </summary>
        /// <param name="ReglasAplicables">Lista de reglas que aplican seg�n filtro de cliente, filtro de art�culo y rango cantidad</param>
        /// <param name="TipoDescuentoAplicar">Tipo de Descuento para el cual se va a obtener la Regal�a por aplicar (Descuento L�nea, Descuento General o Forma de Pago)</param>
        /// <param name="LineasAplicables">Conjunto de detalles que hicieron posible la aplicaci�n de las reglas pasadas por par�metro. Este diccionario asocia una regla con los detalles aplicables. 
        /// Esto es importante al tener que aplicar o buscar reglas con combinaci�n de tipos (Monto y/o Porcentaje) y luego saber cu�les son los detalles que se deben aplicar</param>
        /// <returns>Regal�a que aplica seg�n la configuraci�n del m�dulo</returns>
        private static LineaRegalia ObtenerRegaliaAplicada(List<Regla> ReglasAplicables, string TipoDescuentoAplicar, ref Dictionary<string, List<DetallePedido>> LineasAplicables)
        {
            LineaRegalia regaliaAplicada = null;
            TipoRegalia tipoRegaliaAplicada = TipoRegalia.DescuentoLinea;
            Dictionary<string, List<DetallePedido>> detallesAplicables = LineasAplicables;
            List<DetallePedido> lineasDocAplicables = new List<DetallePedido>();
            Regla reglaAplicar = null;
            decimal porcentajeDescuento = 0;
            decimal montoDescuento = 0;
            bool requiereAutorizacion = false;
            decimal montoDescontar = 0;

            switch (Regalias.pedido.Configuracion.Compania.ModoDescuentosMultiples)
            {
                #region Primero Seg�n Prioridad

                case ModoDescuentoMultipleEnum.PrimeroSegunPrioridad:
                    reglaAplicar = ReglasAplicables.OrderBy(regAp => regAp.Prioridad).First();
                    break;

                #endregion Primero Seg�n Prioridad

                #region Primero Seg�n Porcentaje Mayor

                case ModoDescuentoMultipleEnum.PorcentajeMayor:
                    //Convertir los porcentajes a montos, tomando como base Detalle.Cantidad * Detalle.PrecioUnitario, luego

                    //Se obtienen las reglas de tipo descuento, pero aplicando el descuento para saber el monto que corresponde a ese descuento
                    var reglaDesc = (from regDesc in ReglasAplicables
                                     let Descuento = ((regDesc.Valor / 100) * detallesAplicables[regDesc.Codigo].Sum(lin => lin.MontoTotal))
                                     where regDesc.Tipo == Constantes.TIPO_REGLA_DESCUENTO
                                     && regDesc.TipoValor == Constantes.TIPO_VALOR_PORCENTAGE
                                     select new { Codigo = regDesc.Codigo, Descuento });

                    //Se obtienen las reglas de tipo monto
                    var reglaMont = (from regMont in ReglasAplicables
                                     let Descuento = regMont.Valor
                                     where regMont.Tipo == Constantes.TIPO_REGLA_DESCUENTO
                                     && regMont.TipoValor == Constantes.TIPO_VALOR_MONTO
                                     select new { Codigo = regMont.Codigo, Descuento });

                    //Se combinan ambos resultados y se ordena seg�n el mayor monto por aplicar, obteniendo el primer registro
                    var reglaApp = reglaDesc.Concat(reglaMont).OrderByDescending(reg => reg.Descuento).FirstOrDefault();

                    if (reglaApp != null)
                    {
                        reglaAplicar = ReglasAplicables.Where(regAp => regAp.Codigo == reglaApp.Codigo).FirstOrDefault();
                    }
                    break;

                #endregion Primero Seg�n Porcentaje Mayor

                #region Acumular Todos

                case ModoDescuentoMultipleEnum.AcumularTodos:
                    //Convertir los porcentajes a montos, tomando como base Detalle.Cantidad * Detalle.PrecioUnitario, al final, se obtendr� un monto, ese monto se restar� al total de la l�nea,
                    //Si el monto es menor o igual a Detalle.Cantidad * Detalle.PrecioUnitario, el descuento aplica, sino no hay regla aplicable para esa l�nea
                    montoDescontar = 0;

                    //Se obtienen las reglas de tipo descuento, pero aplicando el descuento para saber el monto que corresponde a ese descuento
                    var reglaDesct = (from regDesc in ReglasAplicables
                                      let Descuento = ((regDesc.Valor / 100) * detallesAplicables[regDesc.Codigo].Sum(lin => lin.MontoTotal))
                                      where regDesc.Tipo == Constantes.TIPO_REGLA_DESCUENTO
                                      && regDesc.TipoValor == Constantes.TIPO_VALOR_PORCENTAGE
                                      select new { regDesc.Codigo, regDesc.Descripcion, regDesc.RequiereAutorizacion, Descuento });

                    //Se obtienen las reglas de tipo monto
                    var reglaMonto = (from regMont in ReglasAplicables
                                      let Descuento = regMont.Valor
                                      where regMont.Tipo == Constantes.TIPO_REGLA_DESCUENTO
                                      && regMont.TipoValor == Constantes.TIPO_VALOR_MONTO
                                      select new { regMont.Codigo, regMont.Descripcion, regMont.RequiereAutorizacion, Descuento });

                    var reglasApp = reglaDesct.Concat(reglaMonto);

                    //Se combinan ambos resultados y se suman todos los montos
                    montoDescontar = reglasApp.Sum(reg => reg.Descuento);

                    //Si aplica la regla se arma una regla "fantasma" indicando el porcentaje a aplicar
                    reglaAplicar = new Regla();

                    reglaAplicar.Codigo = REGLA_FANTASMA;
                    reglaAplicar.Activa = true;
                    reglaAplicar.AplicacionGrupos = false;

                    reglasApp.ToList().ForEach(reg => reglaAplicar.Descripcion += reg.Descripcion + ",");

                    //quita �ltima coma
                    reglaAplicar.Descripcion.Remove(reglaAplicar.Descripcion.Length - 1, 1);

                    reglaAplicar.Prioridad = 0;

                    reglasApp.ToList().ForEach(reg =>
                    {
                        if (reg.RequiereAutorizacion)
                        {
                            requiereAutorizacion = true;
                        }
                    });

                    reglaAplicar.RequiereAutorizacion = requiereAutorizacion;

                    reglaAplicar.Tipo = Constantes.TIPO_REGLA_DESCUENTO;
                    reglaAplicar.TipoDescuento = TipoDescuentoAplicar;
                    reglaAplicar.TipoValor = Constantes.TIPO_VALOR_MONTO;
                    reglaAplicar.ValidarCantidad = false;
                    reglaAplicar.Valor = montoDescontar;

                    //Se hace un resumen de los detalles que se utilizaron en la aplicaci�n de las reglas acumuladas          
                    reglasApp.ToList().ForEach(regla =>
                    {
                        foreach (DetallePedido linDoc in detallesAplicables[regla.Codigo])
                        {
                            //Si una linea est� en la regla A y B s�lo se debe existir una vez
                            if (!lineasDocAplicables.Contains(linDoc))
                            {
                                lineasDocAplicables.Add(linDoc);
                            }
                        }
                    });

                    //Se agrega la regla fantasma, con el acumulado de detalles aplicables, a los cuales se les deber� aplicar el descuento
                    LineasAplicables.Add(reglaAplicar.Codigo, lineasDocAplicables);

                    break;

                #endregion Acumular Todos

                #region Acumular Todos Cascada

                case ModoDescuentoMultipleEnum.AcumularCascada:

                    decimal porcentajeAcumulado = 0;
                    decimal porcentajeAumento = 0;
                    decimal descuentoBase = 0;
                    bool PrimerDescuentoObtenido = false;


                    //Se obtienen las reglas de tipo descuento, pero aplicando el descuento para saber el monto que corresponde a ese descuento
                    var reglasDesc = (from regDesc in ReglasAplicables
                                      where regDesc.Tipo == Constantes.TIPO_REGLA_DESCUENTO
                                      && regDesc.TipoValor == Constantes.TIPO_VALOR_PORCENTAGE
                                      select new { regDesc.Codigo, regDesc.Descripcion, regDesc.RequiereAutorizacion, regDesc.Valor });

                    reglasDesc.ToList().ForEach(regAp =>
                    {
                        if (!PrimerDescuentoObtenido)
                        {
                            porcentajeAcumulado = regAp.Valor;
                            porcentajeAumento = regAp.Valor;
                            PrimerDescuentoObtenido = true;
                        }
                        else
                        {
                            descuentoBase = 1 - porcentajeAumento;
                            porcentajeAumento = (descuentoBase * regAp.Valor) + porcentajeAcumulado;
                            porcentajeAcumulado = porcentajeAumento;
                        }
                    });

                    var reglasMonto = (from regMont in ReglasAplicables
                                       where regMont.Tipo == Constantes.TIPO_REGLA_DESCUENTO
                                       && regMont.TipoValor == Constantes.TIPO_VALOR_MONTO
                                       select new { regMont.Codigo, regMont.Descripcion, regMont.RequiereAutorizacion, regMont.Valor });

                    if (reglasMonto != null)
                    {
                        montoDescontar = reglasMonto.Sum(p => p.Valor);
                    }

                    var reglasAppCasc = reglasDesc.Concat(reglasMonto);

                    //Se hace un resumen de los detalles que se utilizaron en la aplicaci�n de las reglas acumuladas          
                    reglasAppCasc.ToList().ForEach(regla =>
                    {
                        foreach (DetallePedido linDoc in detallesAplicables[regla.Codigo])
                        {
                            //Si una linea est� en la regla A y B s�lo se debe existir una vez
                            if (!lineasDocAplicables.Contains(linDoc))
                            {
                                lineasDocAplicables.Add(linDoc);
                            }
                        }
                    });

                    //Obtener el monto final. 
                    //Para esto se combierte el porcentajeDescontar a monto por medio del subtotal de la l�nea y a eso se le suma el montoDescontar
                    montoDescontar = montoDescontar + (lineasDocAplicables.Sum(lin => lin.MontoTotal) * (porcentajeAcumulado / 100));

                    //Si aplica la regla se arma una regla "fantasma" indicando el porcentaje a aplicar
                    reglaAplicar = new Regla();

                    reglaAplicar.Codigo = REGLA_FANTASMA;
                    reglaAplicar.Activa = true;
                    reglaAplicar.AplicacionGrupos = false;

                    reglasAppCasc.ToList().ForEach(reg => reglaAplicar.Descripcion += reg.Descripcion + ",");

                    //quita �ltima coma
                    reglaAplicar.Descripcion.Remove(reglaAplicar.Descripcion.Length - 1, 1);

                    reglaAplicar.Prioridad = 0;

                    reglasAppCasc.ToList().ForEach(reg =>
                    {
                        if (reg.RequiereAutorizacion)
                        {
                            requiereAutorizacion = true;
                        }
                    });

                    reglaAplicar.RequiereAutorizacion = requiereAutorizacion;

                    reglaAplicar.Tipo = Constantes.TIPO_REGLA_DESCUENTO;
                    reglaAplicar.TipoDescuento = TipoDescuentoAplicar;
                    reglaAplicar.TipoValor = Constantes.TIPO_VALOR_MONTO;
                    reglaAplicar.ValidarCantidad = false;
                    reglaAplicar.Valor = montoDescontar;

                    //Se agrega la regla fantasma, con el acumulado de detalles aplicables, a los cuales se les deber� aplicar el descuento
                    LineasAplicables.Add(reglaAplicar.Codigo, lineasDocAplicables);

                    break;

                #endregion Acumular Todos Cascada
            }

            #region Genera la Regal�a que Aplica

            //Aplica una regla, se crea la regal�a
            if (reglaAplicar != null)
            {
                if (reglaAplicar.TipoDescuento == Constantes.TIPO_DESCUENTO_LINEA)
                {
                    tipoRegaliaAplicada = TipoRegalia.DescuentoLinea;
                }
                else if (reglaAplicar.TipoDescuento == Constantes.TIPO_DESCUENTO_GENERAL)
                {
                    tipoRegaliaAplicada = TipoRegalia.DescuentoGeneral;
                }

                montoDescuento = 0;
                porcentajeDescuento = 0;

                if (reglaAplicar.TipoValor == Constantes.TIPO_VALOR_MONTO)
                {
                    montoDescuento = reglaAplicar.Valor;
                }
                else
                {
                    porcentajeDescuento = reglaAplicar.Valor;
                }

                regaliaAplicada = new LineaRegalia(reglaAplicar, montoDescuento, porcentajeDescuento, tipoRegaliaAplicada);
            }

            #endregion Genera la Regal�a que Aplica

            return regaliaAplicada;
        }

        /// <summary>
        /// Este m�todo busca la primera regla de bonificaci�n que aplica y de ella genera la regal�a correspondiente, de igual forma, retorna un objeto LineaDocumento con la informaci�n
        /// de la bonificaci�n
        /// </summary>
        /// <param name="ReglasPorAplicar">Conjunto de Reglas que aplican seg�n filtro de Cliente y Art�culo y de las cu�les se debe verificar las escalas de bonificaci�n</param>
        /// <param name="CodigoArticulo">C�digo del art�culo para la l�nea que se debe verificar si aplica bonificaci�n</param>
        /// <param name="Cantidad">Cantidad del art�culo del cual se debe verificar si aplica bonificaci�n</param>
        /// <param name="LineaRecibeBonifcacion">L�nea del Documento correspondiente a la bonificaci�n generada. Si no aplica bonificaci�n esta linea estar� nula</param>
        /// <returns>Objeto Regal�a con la informaci�n del la bonificaci�n aplicada</returns>
        private static LineaRegalia ObtenerRegaliaBonificacion(List<Regla> ReglasPorAplicar, ref List<DetallePedido> LineasPorRevisar, out DetallePedido LineaRecibeBonifcacion)
        {
            LineaRegalia resultado = null;
            List<string> articuloAplicables = null;
            List<DetallePedido> lineasAplicables = null;
            bool reglaAplicada = false;
            String articuloBonificado = string.Empty;
            decimal cantidad = 0;
            LineaRecibeBonifcacion = null;

            foreach (Regla regla in ReglasPorAplicar)
            {
                //Se obtiene la lista de art�culos que cumplen con la regla
                articuloAplicables = ObtenerArticulosAplicables(regla.Compania, regla.FiltroArticulo);

                var lineasQueAplican = (from lin in LineasPorRevisar
                                        where VerificarFiltroArticulo(lin.Articulo.Codigo, articuloAplicables) &&
                                        !lin.EsBonificada && lin.RegaliaBonificacion == null //lin.RegaliaBonificacion != null
                                        select lin);

                //Si aplica al menos uno el filtro, la regla aplica
                if (lineasQueAplican != null && lineasQueAplican.Count() > 0)
                {
                    lineasAplicables = lineasQueAplican.ToList();

                    cantidad = lineasAplicables.Sum(lin => lin.UnidadesAlmacen + (lin.UnidadesDetalle / lin.Articulo.UnidadEmpaque));

                    //Se cargan las escalas asociada a la regla
                    regla.CargarEscalas();

                    foreach (Escala escala in regla.Escalas)
                    {
                        if (lineasQueAplican.Count() == 1)
                        {
                            //Se le asigna a la escala el factor de empaque del art�culo facturado
                            escala.FactorEmpaqueArticulo = lineasAplicables[0].Articulo.UnidadEmpaque;

                            // Verifica el rango de las cantidades de la escala.
                            if (cantidad >= escala.ObtenerCantMinimaAlmacen(true) && cantidad <= escala.ObtenerCantMaximaAlmacen(true))
                            {  

                                articuloBonificado = regla.UtilizarArticuloLinea ? lineasAplicables[0].Articulo.Codigo : escala.Articulo;
                                

                                reglaAplicada = true;
                            }
                        }
                        else
                        {
                            // Verifica el rango de las cantidades de la escala.
                            if (cantidad >= escala.CantidadMinima && cantidad <= escala.CantidadMaxima)
                            {
                                articuloBonificado = escala.Articulo;

                                reglaAplicada = true;
                            }
                        }


                        if (reglaAplicada)
                        {
                            //La bonificaci�n se asigna a la primera l�nea de aquellas que aplicaron para la regla de bonificaci�n
                            LineaRecibeBonifcacion = lineasAplicables[0];

                            resultado = new LineaRegalia(regla, escala, articuloBonificado, cantidad);

                            //Se asigna la linea de regalia a cada una de las l�neas que aplicaron para que la regla fuese elegida
                            for (int cont = 0; cont < lineasAplicables.Count(); cont++)
                            {
                                lineasAplicables[cont].RegaliaBonificacion = resultado;
                            }

                            break;
                        }
                    }
                }
                if (reglaAplicada)
                {
                    break;
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene una lista de art�culos que aplican para el filtro especificado por par�metro
        /// </summary>
        /// <param name="filtroArticulos">Filtro que se utilizar� para obtener los art�culos. Si no hay un filtro no se devuelve ning�n valor en la lista</param>
        /// <returns>Lista de Art�culos que cumplen con el filtro especificado</returns>
        private static List<string> ObtenerArticulosAplicables(string Compania, string filtroArticulos)
        {
            List<string> listaArticulos = new List<string>();
            List<Articulo> ArticulosSeleccionados = new List<Articulo>();

            if (!String.IsNullOrEmpty(filtroArticulos))
            {
                ConvertidorFiltro convertidor = new ConvertidorFiltro();
                String filtro = new ConvertidorFiltro().ConvertirFiltroDevExASQL(filtroArticulos);
                ArticulosSeleccionados = ProbadorFiltros.ObtenerArticulos(Compania, filtro, nivelPrecio, ruta);

                listaArticulos = ArticulosSeleccionados.Select(art => art.Codigo).ToList();
            }

            return listaArticulos;
        }      

        #endregion

        public class Constantes
        {
            public const String TIPO_REGLA_DESCUENTO = "D";
            public const String TIPO_REGLA_BONIFICACION = "B";

            public const String TIPO_DESCUENTO_LINEA = "L";
            public const String TIPO_DESCUENTO_GENERAL = "G";

            public const String TIPO_VALOR_MONTO = "M";
            public const String TIPO_VALOR_PORCENTAGE = "P";
        }

        #region C�digo por Eliminar!!!!!!

        ///// <summary>
        ///// Resta el monto de descuento general asignado anteriormente.
        ///// </summary>
        ///// <param name="detalle"></param>
        //private static void RestarDescuentosGenerales(DetallePedido detalle)
        //{
        //  if (detalle.RegaliaDescuentoGeneral != null)
        //  {
        //    pedido.PorcentajeDescuento1 -= detalle.RegaliaDescuentoGeneral.PorcentajeDescuento;
        //  }
        //}

        ///// <summary>
        ///// Obtiene la cantidad pedida que aplique para los articulos dados.
        ///// </summary>
        ///// <param name="articulos">Lista de articulos por comparar.</param>
        ///// <param name="detalles">Lista de detalles por analizar</param>
        ///// <param name="detallesAplicar">Lista de detalles que se deben aplicar, de esta lista se obtiene la sumatoria.</param>
        ///// <returns></returns>
        //private static decimal ObtenerCantidadGrupo(List<Articulo> articulos, List<DetallePedido> detalles, out List<DetallePedido> detallesAplicar, string tipoRegla)
        //{
        //  decimal resultado = 0;
        //  // Obtiene la lista de los c�digos de los art�culos
        //  var codigoArticulos = (from articulo in articulos
        //                         select articulo.Codigo);
        //  // Obtiene la lista de los detalles por aplicar
        //  var aplicar = (from detalle in detalles
        //                 where codigoArticulos.Contains(detalle.Articulo.Codigo) == true
        //                  && ((tipoRegla == Constantes.TIPO_REGLA_BONIFICACION && detalle.RegaliaBonificacion == null)
        //                     || (tipoRegla == Constantes.TIPO_REGLA_DESCUENTO && detalle.RegaliaDescuentoLinea == null))
        //                 select detalle);
        //  // Ejecuta el proceso de LinQ y obtiene la suma de las cantidades.
        //  resultado = aplicar.Sum(detalle => detalle.UnidadesAlmacen + (detalle.UnidadesDetalle / detalle.Articulo.UnidadEmpaque));
        //  // Obtiene la lista de los detalles por aplicar.
        //  detallesAplicar = aplicar.ToList();
        //  return resultado;
        //}

        ///// <summary>
        ///// Aplica las reglas al conjunto de detalles dados toman en cuenta la cantidad pedida para ese grupo de detalles.
        ///// </summary>
        ///// <param name="regla"></param>
        ///// <param name="detallesAplicar"></param>
        ///// <param name="cantidadPedida"></param>
        //private static void AplicarRegaliaDetalles(Regla regla, List<DetallePedido> detallesAplicar, decimal cantidadPedida)
        //{
        //  LineaRegalia regalia = null;
        //  if (regla.Tipo == Constantes.TIPO_REGLA_DESCUENTO)
        //  {
        //    // Aplica la regla como un descuento.
        //    regalia = AplicarLineaRegaliaDescuento(regla, detallesAplicar, cantidadPedida);

        //  }
        //  if (regla.Tipo == Constantes.TIPO_REGLA_BONIFICACION)
        //  {
        //    // Aplica la regla como una bonificaci�n
        //    regalia = AplicarLineaRegaliaBonificacion(regla, detallesAplicar, cantidadPedida);

        //    if (regalia != null)
        //    {
        //      regalias.Add(regalia);
        //      tieneBonificaciones = tieneBonificaciones || regalia.Tipo == TipoRegalia.Bonificacion;
        //      // Asocia la regalia creada a todas las lineas donde se aplico.
        //      foreach (DetallePedido detalleAplicar in detallesAplicar)
        //      {
        //        detalleAplicar.RegaliaBonificacion = regalia;
        //      }
        //    }
        //  }
        //}

        ///// <summary>
        ///// Verifica que se cumpla la cantidad m�nima y m�xima para el descuento.
        ///// </summary>
        ///// <param name="detalle"></param>
        ///// <param name="regla"></param>
        ///// <returns></returns>
        //private static bool VerificarDescuento(decimal cantidadPedida, Regla regla)
        //{
        //    return !regla.ValidarCantidad || (regla.ValidarCantidad && cantidadPedida >= regla.CantidadMinima && cantidadPedida <= regla.CantidadMaxima);
        //}


        ///// <summary>
        ///// Aplica la regalia de la bonificaci�n al detalle del pedido,
        ///// la nueva linea se carga sobre la linea de bonificaci�n adicional
        ///// </summary>
        ///// <param name="regla"></param>
        ///// <param name="detalle"></param>
        ///// <returns></returns>
        //private static LineaRegalia AplicarLineaRegaliaBonificacion(Regla regla, List<DetallePedido> detalles, decimal cantidadPedida)
        //{
        //    LineaRegalia resultado = null;
        //    DetallePedido detalle = detalles.Count > 0 ? detalles[0] : null;
        //    if (detalle != null)
        //    {
        //        foreach (Escala escala in regla.Escalas)
        //        {
        //            // Verifica el rango de las cantidades de la escala.
        //            if (cantidadPedida >= escala.CantidadMinima && cantidadPedida <= escala.CantidadMaxima)
        //            {
        //                resultado = new LineaRegalia(regla, escala, escala.Articulo, cantidadPedida);
        //                // Obtiene la linea de bonificaci�n con las cantidades ya asignadas seg�n la escala.
        //                DetallePedido lineaBonificada = resultado.ObtenerLineaBonificacion(clienteCia);
        //                detalle.LineaBonificada = lineaBonificada;
        //                lineaBonificada.RegaliaBonificacion = resultado;
        //                break;
        //            }
        //        }
        //    }
        //    return resultado;
        //}

        ///// <summary>
        ///// Aplica la regalia de la bonificaci�n al detalle del pedido,
        ///// la nueva linea se carga sobre la linea de bonificaci�n adicional
        ///// </summary>
        ///// <param name="regla"></param>
        ///// <param name="detalle"></param>
        ///// <returns></returns>
        //private static LineaRegalia AplicarLineaRegaliaBonificacion(Regla regla, DetallePedido detalle)
        //{
        //    LineaRegalia resultado = null;
        //    foreach (Escala escala in regla.Escalas)
        //    {
        //        decimal cantidadPedida = detalle.UnidadesAlmacen + (detalle.UnidadesDetalle / detalle.Articulo.UnidadEmpaque);
        //        // Verifica el rango de las cantidades de la escala.
        //        if (cantidadPedida >= escala.CantidadMinima && cantidadPedida <= escala.CantidadMaxima)
        //        {
        //            String articulo = regla.UtilizarArticuloLinea ? detalle.Articulo.Codigo : escala.Articulo;
        //            resultado = new LineaRegalia(regla, escala, articulo, cantidadPedida);
        //            // Obtiene la linea de bonificaci�n con las cantidades ya asignadas seg�n la escala.
        //            DetallePedido lineaBonificada = resultado.ObtenerLineaBonificacion(clienteCia);
        //            detalle.LineaBonificada = lineaBonificada;
        //            detalle.RegaliaBonificacion = resultado;
        //            break;
        //        }
        //    }
        //    return resultado;
        //}

        #endregion  C�digo por Eliminar!!!!!!
    }
}
