using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BackEndModul.Controllers
{
     
    
    [Route("api/[controller]")]
    [ApiController]
        public class TokenController
        {
            private IUserService _service;
            public AuthOptions _authOptions;

            //private object _authOptions;

            public TokenController(IUserService service, IOptions<AuthOptions> authOptionsAccessor)
            {
                _service = service;
                _authOptions = authOptionsAccessor.Value;
            }

            [HttpPost("Authorization")]
            public IActionResult Get([FromBody] UserCredentials user)
            {
            if (_service.IsValidUser(user.Email, user.Password))
            {
                var token = GetToken(user.Email);
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

                var response = new
                {
                    access_token = encodedJwt,
                    expiration = token.ValidTo
                };

                return new OkObjectResult(response);
            }

            return new UnauthorizedResult();
            }

        [HttpPost("Registration")]
        public IActionResult Registration([FromBody] UserCredentials user)
        {
            if (_service.isValidEmail(user.Email))
            {
                if (user.UserName.Length >= 3)
                {
                    if (_service.CheckUniqueNameAndEmail(user.UserName, user.Email))
                    {
                        if (user.Password == user.PasswordConfirm)
                        {
                            _service.CreateUser(user);

                            var token = GetToken(user.Email);
                            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

                            var response = new
                            {
                                access_token = encodedJwt,
                                expiration = token.ValidTo
                            };

                            return new OkObjectResult(response);
                        }
                        else
                        {
                            var errorConfirm = new
                            {
                                error = "Внимательно проверьте введенные пароли"
                            };

                            return new BadRequestObjectResult(errorConfirm);
                        }
                    }
                    else
                    {
                        var errorUser = new
                        {
                            error = "Пользователь с таким именем/почтой уже существует"
                        };

                        return new BadRequestObjectResult(errorUser);
                    }
                }
                else
                {
                    var errorUsername = new
                    {
                        error = "Имя должно содержать не менее 3 символов"
                    };

                    return new BadRequestObjectResult(errorUsername);
                }

            }
            else
            {
                var errorEmail = new
                {
                    error = "Вы ввели неправильный email"
                };

                return new BadRequestObjectResult(errorEmail);
            }
        }

        private JwtSecurityToken GetToken(String email)
        {
            var authClaims = new[]
               {
                        new Claim(JwtRegisteredClaimNames.Sub, email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            var token = new JwtSecurityToken(
                _authOptions.Issuer,
                _authOptions.Audience,
                authClaims,
                DateTime.Now, DateTime.Now.AddMinutes(_authOptions.ExpiresInMinute),
                new SigningCredentials
                    (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.SecureKey)),
                    SecurityAlgorithms.HmacSha256Signature)
                );
            return token;
        }
    }
    
}
