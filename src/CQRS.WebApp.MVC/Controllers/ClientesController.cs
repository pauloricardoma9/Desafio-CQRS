﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRS.Cadastro.Application.Commands;
using CQRS.Cadastro.Application.Queries;
using CQRS.Cadastro.Application.ViewModels;
using CQRS.Core.Communication.Mediator;
using CQRS.Core.Messages.CommonMessages.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CQRS.WebApp.MVC.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IMediatorHandler _mediatorHandler;
        private readonly ICadastroQueries _cadastroQueries;

        public ClientesController(IMediatorHandler mediatorHandler,
                                  INotificationHandler<NotificacaoDeDominio> notificacoes,
                                  ICadastroQueries cadastroQueries) : base(notificacoes, mediatorHandler)
        {
            _mediatorHandler = mediatorHandler;
            _cadastroQueries = cadastroQueries;
        }

        [HttpGet]
        [Route("index")]
        public async Task<ActionResult<IList<ClienteViewModel>>> IndexClientes()
        {
            var clientesViewModel = await _cadastroQueries.ObterClientes();

            if (OperacaoValida())
            {
                return Ok(clientesViewModel);
            }

            return NotFound(ObterMensagensErro());
        }

        [HttpGet]
        [Route("contatos/index")]
        public async Task<ActionResult<IList<ContatoViewModel>>> IndexContatos()
        {
            var contatosViewModel = await _cadastroQueries.ObterContatos();

            if (OperacaoValida())
            {
                return Ok(contatosViewModel);
            }

            return NotFound(ObterMensagensErro());
        }

        [HttpGet("{clienteId:guid}")]
        [Route("busca/{clienteId:guid}")]
        public async Task<ActionResult<ClienteViewModel>> ObterClienteComContatoPorId(Guid clienteId)
        {
            var clienteViewModel = await _cadastroQueries.ObterClienteComContatoPorId(clienteId);

            if (OperacaoValida())
            {
                return Ok(clienteViewModel);
            }

            return NotFound(ObterMensagensErro());
        }

        [HttpPost] 
        [Route("adicionar")]
        public async Task<IActionResult> AdicionarCliente(ClienteViewModel cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if(cliente.Id == Guid.Empty)
            {
                cliente.Id = Guid.NewGuid();
            }

            var comandoAddCliente = new ComandoAdicionarCliente(cliente.Id, cliente.Nome, cliente.Sobrenome, cliente.Cpf, cliente.Sexo);
            await _mediatorHandler.EnviarComando(comandoAddCliente);

            if (cliente.Contato != null)
            {
                await AdicionarContato(cliente.Contato, cliente.Id);
                
            }

            if (OperacaoValida())
            {
                return CreatedAtAction("AdicionarCliente", null);
            }

            return BadRequest();
        }

        [HttpPost("{clienteId:guid}")]
        [Route("contato/adicionar/{clienteId:guid}")]
        public async Task<IActionResult> AdicionarContato(ContatoViewModel contato, Guid clienteId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (contato.Id == Guid.Empty)
            {
                contato.Id = Guid.NewGuid();
            }

            var comandoAddContato = new ComandoAdicionarContato(contato.Id, clienteId, contato.Ddd, contato.Telefone, contato.Email);
            await _mediatorHandler.EnviarComando(comandoAddContato);

            if (OperacaoValida())
            {
                return CreatedAtAction("AdicionarContato", null);
            }

            return BadRequest(ObterMensagensErro());
        }

        [HttpDelete("{id:guid}")]
        [Route("remover/{id:guid}")]
        public async Task<IActionResult> RemoverCliente(Guid id)
        {
            var comandoRemCliente = new ComandoRemoverCliente(id);
            await _mediatorHandler.EnviarComando(comandoRemCliente);

            if (OperacaoValida())
            {
                return NoContent();
            }

            return BadRequest(ObterMensagensErro());
        }

        [HttpDelete("{id:guid}")]
        [Route("contato/remover/{id:guid}")]
        public async Task<IActionResult> RemoverContato(Guid id)
        {
            var comandoRemContato = new ComandoRemoverContato(id);
            await _mediatorHandler.EnviarComando(comandoRemContato);

            if (OperacaoValida())
            {
                return NoContent();
            }

            return BadRequest(ObterMensagensErro());
        }
    }
}