using Microsoft.Extensions.Configuration;
using NLog;
using Prism.Ioc;
using Prism.Modularity;
using System.IO;
using System.Windows;
using VerifyAttendanceCSV.Views;
using Wada.AchievementEntry;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.DesignManagementWriter;
using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.AchieveTrackSpreadSheet;
using Wada.Data.OrderManagement;
using Wada.IO;
using Wada.ReadWorkRecordApplication;
using Wada.VerifyAchievementRecordContentApplication;
using Wada.WriteWorkRecordApplication;

namespace VerifyAttendanceCSV
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 環境変数を読み込む
            DotNetEnv.Env.Load(".env");

            // DI 設定
            _ = containerRegistry.Register<IConfiguration>(_ => MyConfigurationBuilder());
            // DI logger
            _ = containerRegistry.RegisterSingleton<ILogger>(_ => LogManager.GetCurrentClassLogger());

            // Wada.IO
            _ = containerRegistry.Register<IFileStreamOpener, FileStreamOpener>();

            // 日報読み込み
            _ = containerRegistry.Register<IWorkRecordReader, WorkRecordReader>();
            _ = containerRegistry.Register<IReadAchieveTrackUseCase, ReadAchieveTrackUseCase>();

            // 日報検証
            _ = containerRegistry.Register<IWorkingLedgerRepository, WorkingLedgerRepository>();
            _ = containerRegistry.Register<IDesignManagementRepository, DesignManagementRepository>();
            _ = containerRegistry.Register<IWorkRecordValidator, WorkRecordValidator>();
            _ = containerRegistry.Register<IVerifyWorkRecordUseCase, VerifyWorkRecordUseCase>();

            // 日報書き込み
            _ = containerRegistry.Register<IEmployeeRepository, EmployeeRepository>();
            _ = containerRegistry.Register<IAchievementLedgerRepository, AchievementLedgerRepository>();
            _ = containerRegistry.Register<IDesignManagementWriter, DesignManagementWriter>();
            _ = containerRegistry.Register<IWriteWorkRecordUseCase, WriteWorkRecordUseCase>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);

            // Moduleを読み込む
            moduleCatalog.AddModule<AchievementEntryModule>(InitializationMode.WhenAvailable);
        }

        // 設定情報ライブラリを作る
        static IConfigurationRoot MyConfigurationBuilder() =>
            // NOTE: https://tech-blog.cloud-config.jp/2019-7-11-how-to-configuration-builder/
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
    }
}
