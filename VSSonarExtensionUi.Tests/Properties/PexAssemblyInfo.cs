// <copyright file="PexAssemblyInfo.cs">Copyright ©  2014</copyright>
using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Validation;

// Microsoft.Pex.Framework.Settings
[assembly: PexAssemblySettings(TestFramework = "VisualStudioUnitTest")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("VSSonarExtensionUi")]
[assembly: PexInstrumentAssembly("CredentialManagement")]
[assembly: PexInstrumentAssembly("System.Core")]
[assembly: PexInstrumentAssembly("SonarLocalAnalyser")]
[assembly: PexInstrumentAssembly("System.Windows.Forms")]
[assembly: PexInstrumentAssembly("System.IO.Compression.FileSystem")]
[assembly: PexInstrumentAssembly("DifferenceEngine")]
[assembly: PexInstrumentAssembly("SonarRestService")]
[assembly: PexInstrumentAssembly("VSSonarQubeCmdExecutor")]
[assembly: PexInstrumentAssembly("WindowsBase")]
[assembly: PexInstrumentAssembly("PresentationCore")]
[assembly: PexInstrumentAssembly("PresentationFramework")]
[assembly: PexInstrumentAssembly("System.Xaml")]
[assembly: PexInstrumentAssembly("System.IO.Compression")]
[assembly: PexInstrumentAssembly("GalaSoft.MvvmLight")]
[assembly: PexInstrumentAssembly("MahApps.Metro")]
[assembly: PexInstrumentAssembly("VSSonarPlugins")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "CredentialManagement")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Core")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "SonarLocalAnalyser")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Windows.Forms")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.IO.Compression.FileSystem")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "DifferenceEngine")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "SonarRestService")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "VSSonarQubeCmdExecutor")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "WindowsBase")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "PresentationCore")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "PresentationFramework")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Xaml")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.IO.Compression")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "GalaSoft.MvvmLight")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "MahApps.Metro")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "VSSonarPlugins")]

