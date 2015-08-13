// <copyright file="AssociationModelTest.cs">Copyright ©  2014</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSSonarExtensionUi.ViewModel.Association;
using VSSonarPlugins.Types;

namespace VSSonarExtensionUi.ViewModel.Association.Tests
{
    /// <summary>This class contains parameterized unit tests for AssociationModel</summary>
    [PexClass(typeof(AssociationModel))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class AssociationModelTest
    {
        /// <summary>Test stub for GetResourceForSolution(String, String)</summary>
        [PexMethod]
        public Resource GetResourceForSolutionTest(
            [PexAssumeUnderTest]AssociationModel target,
            string solutionName,
            string solutionPath
        )
        {
            Resource result = target.GetResourceForSolution(solutionName, solutionPath);
            return result;
            // TODO: add assertions to method AssociationModelTest.GetResourceForSolutionTest(AssociationModel, String, String)
        }
    }
}
