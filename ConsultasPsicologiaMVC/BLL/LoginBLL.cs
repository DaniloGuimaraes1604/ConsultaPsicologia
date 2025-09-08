
using ConsultasPsicologiaMVC.BLL.Interfaces;
using ConsultasPsicologiaMVC.DAO.Interfaces;
using ConsultasPsicologiaMVC.Models;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ConsultasPsicologiaMVC.BLL
{
    public class LoginBLL : ILoginBll
    {
        private readonly ILoginDao _loginDao;

        public LoginBLL(ILoginDao loginDao)
        {
            _loginDao = loginDao;
        }

        public async Task<LoginResult> ValidarLogin(LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Senha))
            {
                return new LoginResult { Success = false, Message = "E-mail e senha são obrigatórios." };
            }

            var user = await _loginDao.BuscarUsuarioPorEmail(model.Email);

            if (user == null || string.IsNullOrEmpty(user.Senha))
            {
                return new LoginResult { Success = false, Message = "E-mail ou senha inválidos." };
            }

            try
            {
                byte[] hashBytes = Convert.FromBase64String(user.Senha);
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                var pbkdf2 = new Rfc2898DeriveBytes(model.Senha, salt, 10000, HashAlgorithmName.SHA256);
                byte[] providedPasswordHash = pbkdf2.GetBytes(20);

                bool passwordMatch = true;
                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != providedPasswordHash[i])
                    {
                        passwordMatch = false;
                        break;
                    }
                }

                if (passwordMatch)
                {
                    return new LoginResult 
                    { 
                        Success = true, 
                        Message = "Login realizado com sucesso!", 
                        Email = user.Email, 
                        UserName = user.Nome 
                    };
                }
                else
                {
                    return new LoginResult { Success = false, Message = "E-mail ou senha inválidos." };
                }
            }
            catch (Exception)
            {
                // Log the exception ex
                return new LoginResult { Success = false, Message = "Ocorreu um erro ao verificar a senha." };
            }
        }
    }
}
