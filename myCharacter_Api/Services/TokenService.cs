using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using myCharacter.Models; 

namespace myCharacter.Services 
{
    public class TokenService
    {
        // Injetando IConfiguration para acessar as configurações do Secret Manager
        // Isso permite que o TokenService leia as chaves de configuração necessárias para gerar o token
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> GenerateToken(ApplicationUser user)
        {
            // Cria uma lista de claims que serão incluídas no token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            // Busca Papel ("Admin" ou "Player")
            var roles = await _userManager.GetRolesAsync(user);

            // Adiciona cada papel como uma claim
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
            
            // Cria uma chave simétrica usando a chave secreta definida no Secret Manager
                // A chave é usada para assinar o token, garantindo sua integridade e autenticidade
            var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            
            // Cria as credenciais de assinatura do token usando a chave simétrica
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Monta o token com todas as informações
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7), // Validade do token
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],     // Lê o Issuer do Secret Manager
                Audience = _config["Jwt:Audience"] // Lê a Audience do Secret Manager
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Retorna o token como uma string
            return tokenHandler.WriteToken(token);
        }
    }
}
