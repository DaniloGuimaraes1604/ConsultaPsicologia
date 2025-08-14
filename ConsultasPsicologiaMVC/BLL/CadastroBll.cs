using ConsultasPsicologiaMVC.BLL.Interfaces;
using ConsultasPsicologiaMVC.DAO.Interfaces;
using ConsultasPsicologiaMVC.Models;
using System;
using System.Security.Cryptography;

namespace ConsultasPsicologiaMVC.BLL
{
    public class CadastroBll : ICadastroBll
    {
        private readonly ICadastroDao _cadastroDao;

        public CadastroBll(ICadastroDao cadastroDao)
        {
            _cadastroDao = cadastroDao;
        }

        public string Salvar(Cadastrar cadastrar)
        {
            if (_cadastroDao.EmailExiste(cadastrar.Email).Result)
            {
                return "Este e-mail já está cadastrado.";
            }

            // Gerar um salt aleatório
            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);

            // Criar o hash da senha usando PBKDF2
            var pbkdf2 = new Rfc2898DeriveBytes(cadastrar.Senha, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20);

            // Combinar o salt e o hash para armazenamento
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string senhaArmazenada = Convert.ToBase64String(hashBytes);

            cadastrar.Senha = senhaArmazenada;
            cadastrar.Nome = cadastrar.Nome.ToUpper();
            cadastrar.Email = cadastrar.Email.ToUpper();

            var sucesso = _cadastroDao.SalvarCadastro(cadastrar).Result;

            return sucesso ? "Cadastro realizado com sucesso!" : "Não foi possível realizar o cadastro. Nenhum dado foi salvo.";
        }
    }
}