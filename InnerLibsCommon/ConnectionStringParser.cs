using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;


namespace Extensions.Databases
{

    public class ConnectionStringParser : Dictionary<string, string>
    {
        #region Public Constructors

        public ConnectionStringParser(string ConnectionString = null) : base() => Parse(ConnectionString);

        #endregion Public Constructors



        #region Public Properties

        public string ConnectionString { get => ToString(); set => Parse(value); }

        #endregion Public Properties



        #region Public Methods

        public static implicit operator ConnectionStringParser(string s) => new ConnectionStringParser(s);

        public static implicit operator string(ConnectionStringParser cs) => cs.ToString();

        public ConnectionStringParser Parse(string ConnectionString)
        {
            try
            {
                Clear();
                Error = null;
                if (ConnectionString.IsValid())
                {
                    Util.WriteDebug(ConnectionString, "Parsing ConnectionString");
                    foreach (var ii in ConnectionString.SplitAny(";").Select(t => t.Split(new char[] { '=' }, 2)).ToDictionary(t => t[0].Trim(), t => t[1].Trim(), StringComparer.InvariantCultureIgnoreCase))
                        this.Set(ii.Key.ToTitle(true), ii.Value);
                }
            }
            catch (Exception ex)
            {
                Error = ex;
            }

            return this;
        }

        Exception Error { get; set; }

        /// <summary>
        /// Retorna a connectionstring deste parser
        /// </summary>
        /// <returns></returns>
        public override string ToString() => this.SelectJoinString(x => $"{x.Key.ToTitle(true)}={x.Value}", ";");

        #endregion Public Methods
    }

    public class SqlServerConnectionStringParser : ConnectionStringParser
    {
        #region Public Constructors

        public SqlServerConnectionStringParser(string ConnectionString = null) : base(ConnectionString)
        { }

        #endregion Public Constructors



        #region Public Properties

        public string InitialCatalog { get => this.GetValueOr("Initial Catalog"); set => this.Set("Initial Catalog", value.NullIf(x => x.IsNotValid())); }
        public bool IntegratedSecurity { get => this.GetValueOr("Integrated Security", "false").ToLowerInvariant().ToBool(); set => this.SetOrRemove("Integrated Security", value.ToString().ToTitle().NullIf("False")); }
        public string Password { get => this.GetValueOr("Password"); set => this.SetOrRemove("Password", value.NullIf(x => x.IsNotValid())); }
        public string Server { get => this.GetValueOr("Server"); set => this.SetOrRemove("Server", value.NullIf(x => x.IsNotValid())); }
        public string UserID { get => this.GetValueOr("User Id"); set => this.SetOrRemove("User Id", value.NullIf(x => x.IsNotValid())); }

        #endregion Public Properties
    }

}