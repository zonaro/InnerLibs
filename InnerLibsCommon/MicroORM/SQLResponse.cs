using System;
using System.Collections.Generic;

namespace Extensions.DataBases
{


    /// <summary>
    /// Constantes utilizadas na funçao <see cref="Util.CreateSQLQuickResponse(DbConnection, System.FormattableString, string)"/>
    /// e <see  cref="Util.CreateSQLQuickResponse(DbCommand, string)"/>
    /// </summary>
    public static class DataSetType
    {
        #region Public Fields

        /// <summary>
        /// Coloca todos os datasets no <see cref="SQLResponse{T}.Data"/>.
        /// </summary>
        /// <remarks>pode tambem ser representado pelas strings "DEFAULT", "SETS"</remarks>
        public const string Many = "MANY";

        /// <summary>
        /// Coloca primeira e ultima coluna do primeiro dataset no <see cref="SQLResponse{T}.Data"/> como um <see cref="Dictionary{string, object}"/>
        /// </summary>
        ///<remarks>
        /// pode tambem ser representada pelas strings "PAIRS", "DICTIONARY", "ASSOCIATIVE",
        ///</remarks>
        public const string Pair = "PAIR";



        /// <summary>
        /// Coloca a primeira coluna do primeiro dataset no <see cref="SQLResponse{T}.Data"/>
        /// </summary>
        /// <remarks>pode tambem ser representado pelas strings "ONE", "FIRST"</remarks>
        public const string Row = "ROW";

        /// <summary>
        /// Coloca o primeiro valor da primeira linha do primeiro dataset no <see cref="SQLResponse{T}.Data"/>
        /// </summary>
        /// <remarks>pode tambem ser representado pelas strings "SINGLE", "Id", "KEY"</remarks>
        public const string Value = "VALUE";

        /// <summary>
        /// Coloca todos os valores encontrados na primeira coluna do primeiro dataset no <see cref="SQLResponse{T}.Data"/>
        /// </summary>
        /// <remarks>pode tambem ser representado pelas strings "ARRAY", "LIST"</remarks>
        public const string Values = "VALUES";

        /// <summary>
        /// Coloca todos os datasets no <see cref="SQLResponse{T}.Data"/> como um <see cref="Dictionary{string, object}"/>
        /// </summary>
        public const string NamedSets = "NAMEDSETS";

        #endregion Public Fields

        #region Public Methods

        public static List<string> ToList() => new List<string>() { Many, Pair, Row, Value, Values };

        #endregion Public Methods
    }







    public class SQLResponse<T> : ApiResponse<T>
    {



        public string DataSetType { get => this.GetValueOr("dataSetType").ChangeType<string>(); set => this["dataSetType"] = value; }
        public string SQL { get => this.GetValueOr("sql").ChangeType<string>(); set => this["sql"] = value; }




    }

    public class ApiResponse : Dictionary<string, object>
    {
        public string Message { get => this.GetValueOr("message").ChangeType<string>(); set => this["message"] = value; }
        public bool HasError => this.ContainsKey("error") || Status.FlatEqual("error");
        public string Status { get => this.GetValueOr("status").ChangeType<string>(); set => this["status"] = value; }

        public void SetError(string message)
        {
            this["message"] = message;
            this["status"] = "error";
        }

        public void SetError(Exception ex) => SetError(ex.ToFullExceptionString());

    }

    public class ApiResponse<T> : ApiResponse
    {
        #region Public Properties

        public T Data
        {
            get
            {
                var item = this.GetValueOr("data");
                if (item is T v)
                {
                    return v;
                }

                return item.ChangeType<T>();
            }

            set
            {
                this.SetOrRemove("data", value);
            }
        }

        public bool HasData => Data != null && Data.IsValid();




        #endregion Public Properties
    }
}
