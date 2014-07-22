using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Softland.ERP.FR.Mobile.Cls.Documentos.FRDesBon
{
    public class ConvertidorFiltro
    {
        #region Atributos
        /// <summary>
        /// Palabras reservadas que deben conservarse al momento de hacer búsquedas de datos
        /// al agregar una se debe separar con un  " | " y debe tener un espacio al inicio y
        /// el final de la palabra
        /// </summary>
        private const string palabrasReservadas = " BETWEEN | AND | OR ";

        #endregion

        #region Propiedades

        #endregion

        #region Contructor
        /// <summary>
        /// Provee las funciones necesarias para convertir los filtros.
        /// </summary>
        /// <param name="session"></param>
        public ConvertidorFiltro()
        {
        }
        #endregion

        #region Metodos

        #region Conversión de Filtro de DeveloperExpress a SQL
        /// <summary>
        /// Utilizando los datos de la sesión con la que se crea, determina 
        /// el tipo de base de datos para la cual debe traducir el filtro
        /// </summary>
        /// <param name="filtroDevEx">Filtro en formato de Developer Express a ser convertido a SQL según el tipo de base de datos</param>
        /// <returns>Filtro convertido en SQL para el tipo de base de datos de la sesión</returns>
        public string ConvertirFiltroDevExASQL(string filtroDevEx)
        {
            filtroDevEx = this.AgregarCambiosDeLineaPalabrasReservadas(filtroDevEx);
            filtroDevEx = this.CambiarDevExBETWEENaSQL(filtroDevEx);
            filtroDevEx = this.CambiarDevExFiltroFechasASQL(filtroDevEx);
            filtroDevEx = this.CambiarNumerosConMAlFinal(filtroDevEx);
            //filtroDevEx = this.CambiarParentesisCuadrados(filtroDevEx);
            // TODO: Revisar qué otras conversiones son necesarias para los filtros de Developer Express, principalmente Fechas
            filtroDevEx = this.EliminarCambiosDeLineaPalabrasReservadas(filtroDevEx);
            return filtroDevEx;
        }
        // TODO: Documentar función AgregarCambiosDeLineaPalabrasReservadas
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtroSinCambiosDeLinea"></param>
        /// <returns></returns>
        private string AgregarCambiosDeLineaPalabrasReservadas(string filtroSinCambiosDeLinea)
        {
            return Regex.Replace(filtroSinCambiosDeLinea
                    , string.Format("(?<1>{0})", palabrasReservadas)
                    , "\n$1"
                    , RegexOptions.IgnoreCase);
        }
        // TODO: Documentar función EliminarCambiosDeLineaPalabrasReservadas
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtroConCambiosDeLinea"></param>
        /// <returns></returns>
        private string EliminarCambiosDeLineaPalabrasReservadas(string filtroConCambiosDeLinea)
        {
            return Regex.Replace(filtroConCambiosDeLinea
                    , string.Format("\n(?<1>{0})", palabrasReservadas)
                    , "$1"
                    , RegexOptions.IgnoreCase);
        }
        // TODO: Documentar función CambiarDevExBETWEENaSQL
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtroConBetween"></param>
        /// <returns></returns>
        private string CambiarDevExBETWEENaSQL(string filtroConBetween)
        {
            return Regex.Replace(filtroConBetween, @"BETWEEN\((?<1>.+),(?<2>.+)\)", coinsidencia => string.Format("BETWEEN {0} AND {1}", coinsidencia.Groups[1].Value, coinsidencia.Groups[2].Value), RegexOptions.IgnoreCase);
        }

        #region Conversión de Filtros de Fecha
        private string CambiarDevExFiltroFechasASQL(string filtroConFechas)
        {
            filtroConFechas = this.CambiarIntervaloDespuesDeEsteAnio(filtroConFechas);
            filtroConFechas = this.CambiarIntervaloPorTerminarAnio(filtroConFechas);
            filtroConFechas = this.CambiarIntervaloPorTerminarMes(filtroConFechas);
            filtroConFechas = this.CambiarIntervaloProximaSemana(filtroConFechas);
            filtroConFechas = this.CambiarIntervaloPorTerminarSemana(filtroConFechas);
            filtroConFechas = this.CambiarIntervaloManiana(filtroConFechas);
            filtroConFechas = this.CambiarIntervaloHoy(filtroConFechas);
            filtroConFechas = this.CambiarIntervaloAyer(filtroConFechas);
            filtroConFechas = this.CambiarIntervaloTranscurridoSemana(filtroConFechas);
            filtroConFechas = this.CambiarIntervaloSemanaPasada(filtroConFechas);
            filtroConFechas = this.CambiarIntervaloTranscurridoMes(filtroConFechas);
            filtroConFechas = this.CambiarIntervaloTranscurridoAnio(filtroConFechas);
            filtroConFechas = this.CambiarIntervaloAntesEsteAnio(filtroConFechas);
            filtroConFechas = this.CambiarFormatoDeFechas(filtroConFechas);

            return filtroConFechas;
        }

        private string CambiarFormatoDeFechas(string filtroConFechas)
        {
            MatchEvaluator evaluador = null;

            evaluador = coinsidencia => string.Format("'{0}-{1}-{2}'", coinsidencia.Groups["anio"].Value, coinsidencia.Groups["mes"].Value, coinsidencia.Groups["dia"].Value);

            return Regex.Replace(filtroConFechas
                    , @"#(?<anio>[0-9]+)-(?<mes>[0-9]+)-(?<dia>[0-9]+)#"
                    , evaluador
                    , RegexOptions.IgnoreCase);
        }

        private string CambiarIntervaloAntesEsteAnio(string filtroConFechas)
        {
            MatchEvaluator evaluador = null;

            evaluador = coinsidencia => string.Format("YEAR({0}) <= YEAR(GETDATE())-1", coinsidencia.Groups["campoFecha"].Value);

            return Regex.Replace(filtroConFechas
                    , @"IsOutlookIntervalPriorThisYear\((?<campoFecha>\[.+\])\)"
                    , evaluador
                    , RegexOptions.IgnoreCase);
        }

        private string CambiarIntervaloTranscurridoAnio(string filtroConFechas)
        {
            MatchEvaluator evaluador = null;
            
            evaluador = coinsidencia => string.Format("DATEADD(day, DATEDIFF(day, 0, {0}), 0) BETWEEN DATEADD(month,(YEAR(GETDATE())-1900)* 12 ,0) AND DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)", coinsidencia.Groups["campoFecha"].Value);

            return Regex.Replace(filtroConFechas
                    , @"IsOutlookIntervalEarlierThisYear\((?<campoFecha>\[.+\])\)"
                    , evaluador
                    , RegexOptions.IgnoreCase);
        }

        private string CambiarIntervaloTranscurridoMes(string filtroConFechas)
        {
            MatchEvaluator evaluador = null;
            evaluador = coinsidencia => string.Format("DATEADD(day, DATEDIFF(day, 0, {0}), 0) BETWEEN DATEADD(ms,0,DATEADD(mm, DATEDIFF(m,0,GETDATE()),0)) AND DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)", coinsidencia.Groups["campoFecha"].Value);

            return Regex.Replace(filtroConFechas
                    , @"IsOutlookIntervalEarlierThisMonth\((?<campoFecha>\[.+\])\)"
                    , evaluador
                    , RegexOptions.IgnoreCase);
        }

        private string CambiarIntervaloSemanaPasada(string filtroConFechas)
        {
            MatchEvaluator evaluador = null;
            evaluador = coinsidencia => string.Format("DATEADD(day, DATEDIFF(day, 0, {0}), 0) BETWEEN DATEADD(day,-(6 + DATEPART(weekday,GETDATE())),DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)) AND DATEADD(day,-DATEPART(weekday,GETDATE()),DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0))", coinsidencia.Groups["campoFecha"].Value);
            
            return Regex.Replace(filtroConFechas
                    , @"IsOutlookIntervalLastWeek\((?<campoFecha>\[.+\])\)"
                    , evaluador
                    , RegexOptions.IgnoreCase);
        }

        private string CambiarIntervaloTranscurridoSemana(string filtroConFechas)
        {
            MatchEvaluator evaluador = null;
            evaluador = coinsidencia => string.Format("DATEADD(day, DATEDIFF(day, 0, {0}), 0) BETWEEN DATEADD(day,+1 - DATEPART(weekday,GETDATE()),DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)) AND DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)", coinsidencia.Groups["campoFecha"].Value);

            return Regex.Replace(filtroConFechas
                    , @"IsOutlookIntervalEarlierThisWeek\((?<campoFecha>\[.+\])\)"
                    , evaluador
                    , RegexOptions.IgnoreCase);
        }

        private string CambiarIntervaloAyer(string filtroConFechas)
        {
            MatchEvaluator evaluador = null;
            evaluador = coinsidencia => string.Format("DATEADD(day, DATEDIFF(day, 0, {0}), 0) = DATEADD(day, DATEDIFF(day, 0, GETDATE()-1), 0)", coinsidencia.Groups["campoFecha"].Value);
            
            return Regex.Replace(filtroConFechas
                    , @"IsOutlookIntervalYesterday\((?<campoFecha>\[.+\])\)"
                    , evaluador
                    , RegexOptions.IgnoreCase);
        }

        private string CambiarIntervaloHoy(string filtroConFechas)
        {
            MatchEvaluator evaluador = null;
            evaluador = coinsidencia => string.Format("DATEADD(day, DATEDIFF(day, 0, {0}), 0) = DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)", coinsidencia.Groups["campoFecha"].Value);
            
            return Regex.Replace(filtroConFechas
                    , @"IsOutlookIntervalToday\((?<campoFecha>\[.+\])\)"
                    , evaluador
                    , RegexOptions.IgnoreCase);
        }

        private string CambiarIntervaloManiana(string filtroConFechas)
        {
            MatchEvaluator evaluador = null;
            evaluador = coinsidencia => string.Format("DATEADD(day, DATEDIFF(day, 0, {0}), 0) = DATEADD(day, DATEDIFF(day, 0, GETDATE()+1), 0)", coinsidencia.Groups["campoFecha"].Value);
            
            return Regex.Replace(filtroConFechas
                    , @"IsOutlookIntervalTomorrow\((?<campoFecha>\[.+\])\)"
                    , evaluador
                    , RegexOptions.IgnoreCase);
        }

        private string CambiarIntervaloPorTerminarSemana(string filtroConFechas)
        {
            MatchEvaluator evaluador = null;
            evaluador = coinsidencia => string.Format("DATEADD(day, DATEDIFF(day, 0, {0}), 0) BETWEEN DATEADD(day, 8 - DATEPART(weekday,GETDATE()),DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)) AND DATEADD(day,14 - DATEPART(weekday,GETDATE()),DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0))", coinsidencia.Groups["campoFecha"].Value);
            
            return Regex.Replace(filtroConFechas
                    , @"IsOutlookIntervalLaterThisWeek\((?<campoFecha>\[.+\])\)"
                    , evaluador
                    , RegexOptions.IgnoreCase);
        }

        private string CambiarIntervaloProximaSemana(string filtroConFechas)
        {
            MatchEvaluator evaluador = null;
            evaluador = coinsidencia => string.Format("DATEADD(day, DATEDIFF(day, 0, {0}), 0) BETWEEN DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) AND DATEADD(day,+8 - DATEPART(weekday,GETDATE()),DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0))", coinsidencia.Groups["campoFecha"].Value);
            
            return Regex.Replace(filtroConFechas
                    , @"IsOutlookIntervalNextWeek\((?<campoFecha>\[.+\])\)"
                    , evaluador
                    , RegexOptions.IgnoreCase);
        }

        private string CambiarIntervaloPorTerminarMes(string filtroConFechas)
        {
            MatchEvaluator evaluador = null;
            evaluador = coinsidencia => string.Format("DATEADD(day, DATEDIFF(day, 0, {0}), 0) BETWEEN DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0) AND DATEADD(ms,-3,DATEADD(mm, DATEDIFF(m,0,GETDATE())+1,0))", coinsidencia.Groups["campoFecha"].Value);
            
            return Regex.Replace(filtroConFechas
                    , @"IsOutlookIntervalLaterThisMonth\((?<campoFecha>\[.+\])\)"
                    , evaluador
                    , RegexOptions.IgnoreCase);
        }

        private string CambiarIntervaloPorTerminarAnio(string filtroConFechas)
        {
            MatchEvaluator evaluador = null;
            evaluador = coinsidencia => string.Format("DATEADD(day, DATEDIFF(day, 0, {0}), 0) BETWEEN DATEADD(day, DATEDIFF(day, 0, GETDATE(), 0) AND DATEADD(month,(YEAR(GETDATE())-1900)*12 + 11,0)+30", coinsidencia.Groups["campoFecha"].Value);
            
            return Regex.Replace(filtroConFechas
                    , @"IsOutlookIntervalLaterThisYear\((?<campoFecha>\[.+\])\)"
                    , evaluador
                    , RegexOptions.IgnoreCase);
        }

        private string CambiarIntervaloDespuesDeEsteAnio(string filtroConFechas)
        {
            MatchEvaluator evaluador = null;
            evaluador = coinsidencia => string.Format("YEAR({0}) >= YEAR(GETDATE())+1", coinsidencia.Groups["campoFecha"].Value);
            
            return Regex.Replace(filtroConFechas
                    , @"IsOutlookIntervalBeyondThisYear\((?<campoFecha>\[.+\])\)"
                    , evaluador
                    , RegexOptions.IgnoreCase);
        }
        #endregion Conversión de Filtros de Fecha

        #region Conversión de Filtros de Números
        private string CambiarNumerosConMAlFinal(string filtroConM)
        {
            MatchEvaluator evaluador = coinsidencia => coinsidencia.Groups["numero"].Value;

            return Regex.Replace(filtroConM
                    , @"(?<numero>[0-9\.]+)(?<m>m)"
                    , evaluador
                    , RegexOptions.IgnoreCase);
        }

        #endregion Conversión de Filtros de Números

        #region Parentesis Oracle
        /// <summary>
        /// Cambia los parentesis cuadrados 
        /// </summary>
        /// <param name="filtroConParentesisCuadrados"></param>
        /// <returns></returns>
        //private string CambiarParentesisCuadrados(string filtroConParentesisCuadrados)
        //{
        //    MatchEvaluator evaluador = null;
        //    string resultado = filtroConParentesisCuadrados;
        //    if (this.session.Session.IsOracle)
        //    {
        //        evaluador = coinsidencia => string.Format("\"{0}\"", coinsidencia.Groups["campo"].Value.ToUpper());
        //        resultado = Regex.Replace(filtroConParentesisCuadrados
        //            , @"(\[(?<campo>.*)\])"
        //            , evaluador
        //            , RegexOptions.IgnoreCase);
        //    }

        //    return resultado;
        //}
        #endregion

        #endregion Conversión de Filtro de DeveloperExpress a SQL


        #endregion
    }
}
