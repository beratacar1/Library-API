using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager; // Identity özelleiklerini kullanmamızı sağlar. giriş yapma ,kayıt olma, rol ekleme
        private readonly IConfiguration _configuration; // appsettings.json dosyasındaki verilere ulaşmamızı sağlarr

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }



        //[HttpPost("Register")]
        //public async Task<IActionResult> Register([FromBody] RegisterModel model)
        //{
        //    var userExists = await _userManager.FindByNameAsync(model.Username);
        //    if (userExists != null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, "Kullanıcı zaten mevcut!");

        //    ApplicationUser user = new()
        //    {
        //        Email = model.Email,
        //        SecurityStamp = Guid.NewGuid().ToString(),
        //        UserName = model.Username
        //    };

        //    var result = await _userManager.CreateAsync(user, model.Password);
        //    if (!result.Succeeded)
        //        return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

        //    // Varsayılan rol: Kullanıcı
        //    if (!await _userManager.IsInRoleAsync(user, "Kullanıcı"))
        //    {
        //        await _userManager.AddToRoleAsync(user, "Kullanıcı");
        //    }

        //    return Ok("Kayıt başarılı!");
        //}

        //public class RegisterModel
        //{
        //    public string Username { get; set; }
        //    public string Email { get; set; }
        //    public string Password { get; set; }
        //}




        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)// LoginModel Kullanıcının girdiği kullanıcı adı ve şifreyi içerir
        {   // FromBoy post isteğiyle gelen Json içeriğini LoginModel nesnesine dönüştürür ve model parametresine atanır
            var user = await _userManager.FindByNameAsync(model.Username); // veritabanından kullanıcıyı bulur
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password)) // Şifrenin doğru mu değil mi diye kontrolü yapılır
            {
                var roles = await _userManager.GetRolesAsync(user); // giriş yapanın kullanıcının hangi rollere sahip olduğunu gösterir

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName), // Kullanıcı adı
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var role in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role)); // her kullanıcı için bir role claimi ekler
                    authClaims.Add(new Claim("role", role));
                }

                var token = GetToken(authClaims); // token üretme kısmı
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token), // tokenı string şekilde yazar
                    expiration = token.ValidTo // tokenın geçersiz olacağı zamanı belirtir
                });
            }
            return Unauthorized();
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims) // Token üretme
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));// Token imzalama anahtarını appsettings.json dosyasından alır.
            // Bu anahtar ile token şifrelenir ve doğrulanabilir hale gelir.

            return new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], // tokenı oluşturan sunucu
                audience: _configuration["Jwt:Audience"], // tokenı kullanacak olan uygulama
                expires: DateTime.Now.AddHours(3), // tokenın geçerlilik süresi
                claims: authClaims, // Kullanıcı bilgileri
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256) // HmacSha256 algoritması kulllanılarak imzalanır
            );
        }
    }

    public class LoginModel // Login endpoint'ine gelen verilerin temsilidir.
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
