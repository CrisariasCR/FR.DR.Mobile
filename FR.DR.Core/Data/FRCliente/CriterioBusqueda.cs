using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Softland.ERP.FR.Mobile.Cls.FRCliente
{
    /// <summary>
    /// Campos de busqueda para un cliente
    /// </summary>
    public enum CriterioCliente
    {
        Codigo,
        Nombre,
        Zona,
        Ninguno
    }
    public class CriterioBusquedaCliente
    {
        private CriterioCliente criterio = CriterioCliente.Ninguno;
        /// <summary>
        /// campo criterio para la busqueda
        /// </summary>
        public CriterioCliente Criterio
        {
            get { return criterio; }
            set { criterio = value; }
        }
        private string valor;
        /// <summary>
        /// Cadena de busqueda para el cliente
        /// </summary>
        public string Valor
        {
            get { return valor; }
            set { valor = value; }
        }
        private bool alfabetico;
        /// <summary>
        /// ordenar alfabeticamente
        /// </summary>
        public bool Alfabetico
        {
            get { return alfabetico; }
            set { alfabetico = value; }
        }

        private Dias dias = Dias.T;
        /// <summary>
        /// Si la busqueda es por dias
        /// </summary>
        public Dias Dias
        {
            get { return dias; }
            set { dias = value; }
        }
        private string visita;
        /// <summary>
        /// Visitado o no visitado
        /// </summary>
        public string Visita
        {
            get { return visita; }
            set { visita = value; }
        }
        private bool agil = false;
        /// <summary>
        /// Si es una busqueda exacta
        /// </summary>
        public bool Agil
        {
            get { return agil; }
            set { agil = value; }
        }
        /// <summary>
        /// dar formato a la cadena de busqueda segun sea una busqueda agil o no
        /// </summary>
        private void FormatoValor()
        {
            if (criterio != CriterioCliente.Zona && !agil)
                valor = "%" + valor + "%";
        }
        /// <summary>
        /// Cuando se realiza por codigo o por nombre
        /// </summary>
        /// <param name="c"></param>
        /// <param name="value"></param>
        /// <param name="alfabetico"></param>
        public CriterioBusquedaCliente(CriterioCliente c, string value, bool alfabetico, bool agil)
        {
            this.criterio = c;
            this.valor = value;
            this.alfabetico = alfabetico;
            this.agil = agil;
            FormatoValor();
        }
        /// <summary>
        /// Cuando se realiza por zona
        /// </summary>
        /// <param name="dias"></param>
        /// <param name="visita">Visitados, No visitados, Todos</param>
        /// <param name="alfabetico"></param>
        public CriterioBusquedaCliente(string zona, Dias dias, string visita, bool alfabetico)
        {
            this.valor = zona;
            this.criterio = CriterioCliente.Zona;
            this.Dias = dias;
            this.visita = visita;
            this.alfabetico = alfabetico;
        }
    }
}
