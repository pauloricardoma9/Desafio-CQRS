﻿using CQRS.Core.DomainObjects;
using System;

namespace CQRS.Cadastro.Domain.Objects
{
    public class Contato : Entidade
    {
        public Guid ClienteId { get; private set; }
        public int Ddd { get; private set; }
        public int Telefone { get; private set; }
        public string Email { get; private set; }
        public Cliente Cliente { get; private set; }

        protected Contato() { }

        public Contato(int ddd, int telefone, string email, Cliente cliente)
        {
            Ddd = ddd;
            Telefone = telefone;
            Email = email;
            Cliente = cliente;
            ClienteId = cliente.Id;

            Validar();
        }

        public void AlterarDdd(int ddd)
        {
            Validacoes.ValidarTamanho(ddd, 2, 2, "O campo DDD deve conter 3 dígitos.");
            Ddd = ddd;
        }

        public void AlterarNumero(int telefone)
        {
            Validacoes.ValidarTamanho(telefone, 9, 9, "O campo Número deve conter 9 dígitos.");
            Telefone = telefone;
        }

        public void AlterarEmail(string email)
        {
            Validacoes.ValidarSeNaoVazio(email, "O campo Email não pode estar vazio.");
            Email = email;
        }

        public void AlterarCliente(Cliente cliente)
        {
            Validacoes.ValidarSeNaoNulo(cliente, "O Cliente não pode ser nulo.");
            Validacoes.ValidarSeNaoNulo(cliente.Id, "O ClienteId não pode ser nulo.");
            Cliente = cliente;
            ClienteId= cliente.Id;
        }

        public void Validar()
        {
            Validacoes.ValidarTamanho(Ddd, 2, 2, "O campo DDD deve conter 3 dígitos.");
            Validacoes.ValidarTamanho(Telefone, 9, 9, "O campo Número deve conter 9 dígitos.");
            Validacoes.ValidarSeNaoVazio(Email, "O campo Email não pode estar vazio.");
            Validacoes.ValidarSeNaoNulo(Cliente, "O Cliente não pode ser nulo.");
            Validacoes.ValidarSeNaoNulo(ClienteId, "O ClienteId não pode ser nulo.");
        }
    }
}
