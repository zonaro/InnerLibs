using System;
using System.Collections.Generic;
using System.Data.Common;



/// <summary>
/// Constantes utilizadas na funçao <see cref="Util.CreateSQLQuickResponse(DbCommand,
/// string, bool)"/> e <see cref="Util.CreateSQLQuickResponse(DbConnection,
/// FormattableString, string,bool)"/>
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
    /// <remarks>pode tambem ser representado pelas strings "SINGLE", "ID", "KEY"</remarks>
    public const string Value = "VALUE";

    /// <summary>
    /// Coloca todos os valores encontrados na primeira coluna do primeiro dataset no <see cref="SQLResponse{T}.Data"/>
    /// </summary>
    /// <remarks>pode tambem ser representado pelas strings "ARRAY", "LIST"</remarks>
    public const string Values = "VALUES";

    #endregion Public Fields

    #region Public Methods

    public static IEnumerable<string> ToList() => new List<string>() { Many, Pair, Row, Value, Values };

    #endregion Public Methods
}

/// <summary>
/// Enxtensões para <see cref="DbConnection"/> e classes derivadas
/// </summary>


public class SQLResponse<T>
{
    #region Public Properties

    public T Data { get; set; }
    public string Message { get; set; }
    public string SQL { get; set; }
    public string Status { get; set; }

    #endregion Public Properties
}
