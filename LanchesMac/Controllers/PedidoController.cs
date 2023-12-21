﻿using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly CarrinhoCompra _carrinhoCompra;

        public PedidoController(IPedidoRepository pedidoRepository, CarrinhoCompra carrinhoCompra)
        {
            _pedidoRepository = pedidoRepository;
            _carrinhoCompra = carrinhoCompra;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(Pedido pedido)
        {
            try
            {
                var totalItensPedido = 0;
                var precoTotalPedido = 0.0m;

                List<CarrinhoCompraItem> itens = _carrinhoCompra.GetCarrinhoCompraItens();

                if (_carrinhoCompra.CarrinhoCompraItems.Count == 0)
                {
                    ModelState.AddModelError("", "Seu carrinho está vazio, vamos incluir um lanche?");
                }

                foreach (var item in itens)
                {
                    totalItensPedido += item.Quantidade;
                    precoTotalPedido += item.Lanche.Preco * item.Quantidade;
                }

                pedido.TotalItensPedido = totalItensPedido;
                pedido.PedidoTotal = precoTotalPedido;

                if (ModelState.IsValid)
                {
                    _pedidoRepository.CriarPedido(pedido);

                    ViewBag.CheckoutCompletoMensagem = "Obrigado pelo seu pedido!";
                    ViewBag.TotalPedido = _carrinhoCompra.GetCarrinhoCompraTotal();

                    _carrinhoCompra.LimparCarrinho();

                    return View("~/Views/Pedido/CheckoutCompleto.cshtml", pedido);
                }

                return View(pedido);
            }
            catch (Exception ex)
            {
                return View(pedido);
            }
        }
    }
}
