using aspnet02_boardapp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BasicCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email,
                                            PhoneNumber = model.PhoneNumber };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // 파라미터 순서 : 아이디, 패스워드 isPersistent = RememberMe , 실패할때 계정 잠그기
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {   
                    // TODO : 로그인 후 토스트메시지 띄우기
                    TempData["succeed"] = "로그인했습니다"; // 성공 메세지
                    return RedirectToAction("Index", "Home");

                }
                ModelState.AddModelError("", "로그인 실패!");
            }

            return View(model); // 입력검증에 실패하면 화면에 그대로  대기
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            // TODO : 로그아웃 후 토스트메시지 띄우기
            TempData["succeed"] = "로그아웃했습니다"; // 성공 메세지
            return RedirectToAction("Index", "Home");
        }

    }
}