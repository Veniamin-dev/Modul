using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndModul.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class BalanceController
    {
        private IUserService _service;

        public BalanceController(IUserService service)
        {
            _service = service;
        }

        [HttpPut("ReplenishAccount")]
        public IActionResult TopUpAccount([FromBody] UserCredentials user)
        {
            User accountTo = _service.GetUser(user.ToAccount);

            if (accountTo != null)
            {
                _service.TopUpAccount(accountTo, user.HowMuch);

                var response = new
                {
                    result = "Пополнение произведено успешно"
                };

                return new OkObjectResult(response);
            }
            else
            {
                var errorAccountNumber = new
                {
                    error = "Неправильный номер счета"
                };

                return new BadRequestObjectResult(errorAccountNumber);
            }
        }

        [HttpPut("transfer")]

        public IActionResult Transfer([FromBody] UserCredentials user)
        {
            User accountFrom = _service.GetUser(user.FromAccount);
            User accountTo = _service.GetUser(user.ToAccount);

            if (accountFrom != null && accountTo != null)
            {
                if (accountFrom.AccountNumber != accountTo.AccountNumber)
                {
                    if (_service.Transfer(accountFrom, accountTo, user.HowMuch))
                    {
                        var response = new
                        {
                            result = "Успешно"
                        };

                        return new OkObjectResult(response);
                    }
                    else
                    {
                        var errorTransfer = new
                        {
                            error = "Недостаточно средств"
                        };

                        return new BadRequestObjectResult(errorTransfer);
                    }
                }
                else
                {
                    var errorAccountNumber = new
                    {
                        error = "Номер счета отправителя и получателя совпадает"
                    };

                    return new BadRequestObjectResult(errorAccountNumber);
                }
            }
            else
            {
                var errorAccountNumber = new
                {
                    error = "Неправильный номер счета"
                };

                return new BadRequestObjectResult(errorAccountNumber);
            }
        }
    }
}
