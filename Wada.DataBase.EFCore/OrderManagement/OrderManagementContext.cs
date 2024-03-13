using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wada.DataBase.EFCore.OrderManagement.Entities;

namespace Wada.DataBase.EFCore.OrderManagement;

public class OrderManagementContext : DbContext
{
    private readonly DbConfig _dbConfig;

    public OrderManagementContext(IConfiguration configuration)
        : this(new DbConfig(configuration))
    { }

    private OrderManagementContext(DbConfig orderDbConfig)
    {
        _dbConfig = orderDbConfig ?? throw new ArgumentNullException(nameof(orderDbConfig));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // 接続文字列を作成する
        // NOTE: コンフィグに持つやり方も参考になる
        // https://csharp.sql55.com/database/how-to-use-transaction-scope.php
        var connectionString = new SqlConnectionStringBuilder
        {
            DataSource = _dbConfig.Server,
            InitialCatalog = _dbConfig.DataBase,
            UserID = _dbConfig.User,
            Password = _dbConfig.Password,

            // NOTE: SQLサーバーへのアクセスで証明書のエラーが出る場合の対処法
            // https://tech.tinybetter.com/Article/7b5d05c8-de00-2985-ebb7-3a00e1e23073/View
            TrustServerCertificate = true,
        }.ToString();
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 複合キーを定義する
        modelBuilder.Entity<AchievementDetail>()
            .HasKey(x => new { x.AchievementLedgerId, x.OwnCompanyNumber, x.ProcessFlowId });
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<WorkingLedger> WorkingLedgers { get; set; }
    public DbSet<DesignManagement> DesignManagements { get; set; }
    public DbSet<AchievementLedger> AchievementLedgers { get; set; }
    public DbSet<AchievementDetail> AchievementDetails { get; set; }
    public DbSet<ProcessFlow> ProcessFlows { get; set; }

    /// <summary>
    /// データベース接続情報
    /// </summary>
    /// <param name="Server"></param>
    /// <param name="DataBase"></param>
    /// <param name="User"></param>
    /// <param name="Password"></param>
    record class DbConfig
    {
        internal DbConfig(IConfiguration configuration)
        {
            Server = configuration.GetValue("DB_SERVER", string.Empty)!;
            DataBase = configuration.GetValue("ORDER_DB_NAME", string.Empty)!;
            User = configuration.GetValue("DB_USER", string.Empty)!;
            Password = configuration.GetValue("DB_PASS", string.Empty)!;
        }

        public string Server { get; set; }
        public string DataBase { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}