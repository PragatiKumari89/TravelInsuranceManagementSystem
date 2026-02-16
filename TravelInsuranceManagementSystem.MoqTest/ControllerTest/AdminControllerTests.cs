using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TravelInsuranceManagementSystem.Application.Controllers;
using TravelInsuranceManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using TravelInsuranceManagementSystem.Repo.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TravelInsuranceManagementSystem.MoqTest.ControllerTest
{
    [TestFixture]
    public class AdminControllerTests
    {
        private Mock<IAdminService> _svc = null!;
        private Mock<SignInManager<User>> _signIn = null!;
        private Mock<UserManager<User>> _userManager = null!;
        private Mock<IAccountService> _acc = null!;

        private AdminController _ctrl = null!;

        [SetUp]
        public void SetUp()
        {
            _svc = new Mock<IAdminService>();
            _acc = new Mock<IAccountService>();
            var store = new Mock<IUserStore<User>>().Object;
            _userManager = new Mock<UserManager<User>>(store, null, null, null, null, null, null, null, null);
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            _signIn = new Mock<SignInManager<User>>(_userManager.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null);

            _ctrl = new AdminController(_svc.Object, _signIn.Object, _userManager.Object, _acc.Object);

            var user = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[] { new System.Security.Claims.Claim("UserId", "1") }, "mock"));
            _ctrl.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        }

        [TearDown]
        public void TearDown()
        {
            _ctrl?.Dispose();
        }

        [Test]
        public async Task Dashboard_Returns_View()
        {
            _svc.Setup(s => s.GetDashboardSummaryAsync()).ReturnsAsync(new AdminDashboardViewModel());
            var res = await _ctrl.Dashboard();
            Assert.That(res, Is.TypeOf<ViewResult>());
        }
    }
}
