using System;
using System.Collections.Generic;
using System.Linq;
using InnerLibs.LINQ;

namespace InnerLibs
{
    public class ConnectionStringParser : Dictionary<string, string>
    {
        public ConnectionStringParser() : base()
        {
        }

        public ConnectionStringParser(string ConnectionString) : base()
        {
            Parse(ConnectionString);
        }



        public string ConnectionString { get => ToString(); set => Parse(value); }

        public ConnectionStringParser Parse(string ConnectionString)
        {
            try
            {
                Clear();
                foreach (var ii in ConnectionString.IfBlank("").SplitAny(";").Select(t => t.Split(new char[] { '=' }, 2)).ToDictionary(t => t[0].Trim(), t => t[1].Trim(), StringComparer.InvariantCultureIgnoreCase))
                    this.Set(ii.Key.ToTitle(true), ii.Value);
            }
            catch
            {
            }

            return this;
        }

        /// <summary>
        /// Retorna a connectionstring deste parser
        /// </summary>
        /// <returns></returns>
        public override string ToString() => this.SelectJoinString(x => $"{x.Key.ToTitle()}={x.Value}", ";");

        public static implicit operator string(ConnectionStringParser cs) => cs.ToString();

        public static implicit operator ConnectionStringParser(string s) => new ConnectionStringParser(s);
    }

    public class SqlServerConnectionStringParser : ConnectionStringParser
    {

        public SqlServerConnectionStringParser() : base()
        { }

        public SqlServerConnectionStringParser(string ConnectionString) : base(ConnectionString)
        { }

        public string InitialCatalog { get => this.GetValueOr("Initial Catalog"); set => this.Set("Initial Catalog", value.NullIf(x => x.IsBlank())); }
        public string Server { get => this.GetValueOr("Server"); set => this.SetOrRemove("Server", value.NullIf(x => x.IsBlank())); }
        public string UserID { get => this.GetValueOr("User ID"); set => this.SetOrRemove("User ID", value.NullIf(x => x.IsBlank())); }
        public string Password { get => this.GetValueOr("Password"); set => this.SetOrRemove("Password", value.NullIf(x => x.IsBlank())); }
        public bool IntegratedSecurity { get => this.GetValueOr("Integrated Security", "false").ToLower().ToBoolean(); set => this.SetOrRemove("Integrated Security", value.ToString().ToTitle().NullIf("False")); }
    }

}