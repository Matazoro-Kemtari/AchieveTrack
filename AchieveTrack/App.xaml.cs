using AchieveTrack.Views;
using Microsoft.Extensions.Configuration;
using NLog;
using System.IO;
using System.Windows;
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

namespace AchieveTrack;

using Prism.Ioc;
using Prism.Modularity;
using System.Diagnostics;
using System.Reflection;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
	private static readonly string _configurationBaseDirectory =
		Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					 "Wadass",
					 Process.GetCurrentProcess().ProcessName);

#if DEBUG
	private const string _dotEnvFileName = "debug.env";
#else
	private const string _dotEnvFileName = ".env";
#endif
	private const string _dotEnvResourceName = $"AchieveTrack.{_dotEnvFileName}";


	protected override Window CreateShell()
	{
		return Container.Resolve<MainWindow>();
	}

	protected override void RegisterTypes(IContainerRegistry containerRegistry)
	{
		InitializeDotEnv();

		// 環境変数を読み込む
		DotNetEnv.Env.Load(Path.Combine(_configurationBaseDirectory, _dotEnvFileName));

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
		_ = containerRegistry.Register<IProcessFlowRepository, ProcessFlowRepository>();
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

	// 設定情報ライブラリを作る Directory.GetCurrentDirectory()
	static IConfigurationRoot MyConfigurationBuilder()
		// NOTE: https://tech-blog.cloud-config.jp/2019-7-11-how-to-configuration-builder/
		=> new ConfigurationBuilder()
			.AddEnvironmentVariables()
			.Build();


	private static void InitializeDotEnv()
	{
		var info = new FileInfo(Path.Combine(_configurationBaseDirectory, _dotEnvFileName));
		if (info.Directory != null && !info.Directory.Exists)
			info.Directory.Create();

		if (info.Exists)
			info.Delete();

		using var templateStream = GetResourceStream(_dotEnvResourceName);
		using var newStream = info.Create();
		templateStream.CopyTo(newStream);
	}

	private static Stream GetResourceStream(string resourceName)
	{
		// リソースからテンプレートを取得する
		var assembly = Assembly.GetExecutingAssembly();
		return assembly.GetManifestResourceStream(resourceName)
			   ?? throw new InvalidOperationException($"Could not find embedded resource: {resourceName}");
	}
}
