﻿using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace SportsStore.Controllers
{  
    public class OrderController : Controller
    {
        private IOrderRepository repository;
        private Cart cart;
        public OrderController(IOrderRepository repoService, Cart cartService)
        {
            repository = repoService;
            cart = cartService;
            //this.repository = repoService;
        }

        [Authorize]
        public ViewResult List() => View(repository.Orders.Where(o => !o.Shipped));
        [Authorize]
        public ViewResult ViewOrders() => View(repository.Orders.Where(o => User.Identity.Name == o.Name));

        [HttpPost]
        [Authorize]
        public IActionResult MarkShipped(int orderID)
        {
            Order order = repository.Orders.FirstOrDefault(o => o.OrderID == orderID);
            if (order != null)
            {
                order.Shipped = true;
                repository.SaveOrder(order);
            }
            return RedirectToAction(nameof(List));
        }

        public ViewResult Checkout() => View(new Order());

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }
            if (ModelState.IsValid)
            {
                order.Lines = cart.Lines.ToArray();
                //ViewData["order"] = order;
                ViewBag.tempOrder = order.OrderID;
                repository.SaveOrder(order);
                ViewBag.order = order;
                
                return Completed(order.OrderID);
            }
            else
            {
                return View(order);
            }
        }

        public ViewResult Completed(int orderID)
        {
            cart.Clear();
            ViewBag.ordernum = orderID;
            //ViewBag.rder = order;
            return View("Completed");
        }
    }
}
