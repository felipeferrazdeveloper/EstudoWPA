using IniParser;
using IniParser.Model;
using MySql.Data.MySqlClient;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Context;
using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EstudoWPA.Model.Database
{
    public class DbConfig
    {
        private static DbConfig _instance = null;

        private ISessionFactory _sessionFactory;

        public static DbConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DbConfig();
                }

                return _instance;
            }
        }

        //public PessoaRepository PessoaRepository { get; set; }
        //public UsuarioRepository UsuarioRepository { get; set; }
        //public InstituicaoRespository InstituicaoRespository { get; set; }
        //public CursoRepository CursoRepository { get; set; }
        //public DisciplinaRepository DisciplinaRepository { get; set; }
        //public ProfessorRepository ProfessorRepository { get; set; }
        //public TarefaRepository TarefaRepository { get; set; }
        //public DisciplinaAlunoRepository DisciplinaAlunoRepository { get; set; }
        //public TarefaAlunoRepository TarefaAlunoRepository { get; set; }
        //public ConfiguracaoRepository ConfiguracaoRepository { get; set; }

        private DbConfig()
        {
            Conectar();

            //this.PessoaRepository = new PessoaRepository(this.Session);
            //this.UsuarioRepository = new UsuarioRepository(this.Session);
            //this.InstituicaoRespository = new InstituicaoRespository(this.Session);
            //this.CursoRepository = new CursoRepository(this.Session);
            //this.DisciplinaRepository = new DisciplinaRepository(this.Session);
            //this.ProfessorRepository = new ProfessorRepository(this.Session);
            //this.TarefaRepository = new TarefaRepository(this.Session);
            //this.DisciplinaAlunoRepository = new DisciplinaAlunoRepository(this.Session);
            //this.TarefaAlunoRepository = new TarefaAlunoRepository(this.Session);
            //this.ConfiguracaoRepository = new ConfiguracaoRepository(this.Session);
        }

        public void Initialize(object obj)
        {
            NHibernateUtil.Initialize(obj);
        }

        public IniData LerIni()
        {
            try
            {

                var diretorio = System.Environment.CurrentDirectory;
                var arquivo = diretorio + "/Config/GertConfig.ini";
                if (HttpContext.Current != null)
                {
                    arquivo = HttpContext.Current.Server.MapPath("/Config/GertConfig.ini").Replace("\\", "/");
                }

                if (!System.IO.File.Exists(arquivo))
                {
                    throw new Exception("O arquivo de configuração não existe no diretório.");
                }

                var parser = new FileIniDataParser();

                return parser.ReadFile(arquivo);
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível ler o arquivo de configuração.", ex);
            }
        }

        private void Conectar()
        {
            try
            {
                var iniFile = LerIni();

                var server = iniFile["DbConfig"]["server"];
                var port = iniFile["DbConfig"]["port"];
                var dbName = iniFile["DbConfig"]["dbName"];
                var user = iniFile["DbConfig"]["user"];
                var psw = iniFile["DbConfig"]["psw"];

                var stringConexao = "Persist Security Info=False;server=" + server + ";port=" + port + ";database=" +
                                    dbName + ";uid=" + user + ";pwd=" + psw;

                var mySql = new MySqlConnection(stringConexao);
                try
                {
                    mySql.Open();
                }
                catch
                {
                    CriarSchemaBanco(server, port, dbName, psw, user);
                }
                finally
                {
                    if (mySql.State == ConnectionState.Open)
                    {
                        mySql.Close();
                    }
                }

                ConexaoNHibernate(stringConexao);
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível conectar ao banco de dados.", ex);
            }
        }

        private void CriarSchemaBanco(string server, string port, string dbName, string psw, string user)
        {
            try
            {
                var stringConexao = "server=" + server + ";user=" + user + ";port=" + port + ";password=" + psw + ";";
                var mySql = new MySqlConnection(stringConexao);
                var cmd = mySql.CreateCommand();

                mySql.Open();
                cmd.CommandText = "CREATE DATABASE IF NOT EXISTS `" + dbName + "`;";
                cmd.ExecuteNonQuery();
                mySql.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi criar o banco de dados.", ex);
            }
        }

        private void ConexaoNHibernate(string stringConexao)
        {
            //Cria a configuração com o NH
            var config = new Configuration();
            try
            {
                //Integração com o Banco de Dados
                config.DataBaseIntegration(c =>
                {
                    //Dialeto de Banco
                    c.Dialect<NHibernate.Dialect.MySQLDialect>();
                    //Conexao string
                    c.ConnectionString = stringConexao;
                    //Drive de conexão com o banco
                    c.Driver<NHibernate.Driver.MySqlDataDriver>();
                    // Provedor de conexão do MySQL 
                    c.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                    // GERA LOG DOS SQL EXECUTADOS NO CONSOLE
                    c.LogSqlInConsole = true;
                    // DESCOMENTAR CASO QUEIRA VISUALIZAR O LOG DE SQL FORMATADO NO CONSOLE
                    //c.LogFormattedSql = true;
                    // CRIA O SCHEMA DO BANCO DE DADOS SEMPRE QUE A CONFIGURATION FOR UTILIZADA
                    c.SchemaAction = SchemaAutoAction.Update;
                });

                //Realiza o mapeamento das classes
                var maps = this.Mapeamento();
                config.AddMapping(maps);

                //Verifico se a aplicação é Desktop ou Web
                if (HttpContext.Current == null)
                {
                    config.CurrentSessionContext<ThreadStaticSessionContext>();
                }
                else
                {
                    config.CurrentSessionContext<WebSessionContext>();
                }

                this._sessionFactory = config.BuildSessionFactory();
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível conectar o NHibernate.", ex);
            }
        }

        private HbmMapping Mapeamento()
        {
            try
            {
                var mapper = new ModelMapper();

                //mapper.AddMappings(
                //    Assembly.GetAssembly(typeof(UsuarioMap)).GetTypes()
                //);

                return mapper.CompileMappingForAllExplicitlyAddedEntities();
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível realizar o mapeamento do modelo.", ex);
            }
        }

        private ISession Session
        {
            get
            {
                try
                {
                    if (CurrentSessionContext.HasBind(_sessionFactory))
                        return _sessionFactory.GetCurrentSession();

                    var session = _sessionFactory.OpenSession();
                    CurrentSessionContext.Bind(session);

                    return session;
                }
                catch (Exception ex)
                {
                    throw new Exception("Não foi possível criar a Sessão.", ex);
                }
            }
        }
    }
}
