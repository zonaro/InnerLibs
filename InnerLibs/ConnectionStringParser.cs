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

    public class SqlServerConnectionsStringParser : ConnectionStringParser
    {

        public SqlServerConnectionsStringParser() : base()
        { }

        public SqlServerConnectionsStringParser(string ConnectionString) : base(ConnectionString)
        { }

        public string InitialCatalog { get => this.GetValueOrDefault("Initial Catalog"); set => this.Set("Initial Catalog", value); }
        public string Server { get => this.GetValueOrDefault("Server"); set => this.SetOrRemove("Server", value); }
        public string UserID { get => this.GetValueOrDefault("User ID"); set => this.SetOrRemove("User ID", value); }
        public string Password { get => this.GetValueOrDefault("Password"); set => this.SetOrRemove("Password", value); }
        public bool IntegratedSecurity { get => this.GetValueOrDefault("Integrated Security")?.ToLower().ToBoolean() ?? false; set => this.SetOrRemove("Integrated Security", value.ToString().ToTitle()); }
    }

}