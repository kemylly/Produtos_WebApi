using System;
using System.Linq;
using estudo_api.Data;
using estudo_api.Models;
using Microsoft.AspNetCore.Mvc;
using estudo_api.HATEOAS;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace estudo_api.Controllers
{
    [Route("api/v1/[controller]")] //mapeado a rota // versao legada sem suporte
    [ApiController] //dizer que é um api
    [Authorize(Roles = "Admin")] //so acessa quem tiver autorizado - um token valido
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;

        public ProdutosController(ApplicationDbContext database){
            this.database = database;
            HATEOAS = new HATEOAS.HATEOAS("localhost:5001/api/v1/Produtos");
            HATEOAS.AddAction("GET_INFO", "GET");
            HATEOAS.AddAction("DELETE_PRODUCT", "DELETE");
            HATEOAS.AddAction("EDIT_PRODUCT", "PATCH");
        }
        
        [HttpGet("teste")]
        public IActionResult TesteClaims()
        {
            return Ok(HttpContext.User.Claims.First(claim => claim.Type.ToString().Equals("id",StringComparison.InvariantCultureIgnoreCase)).Value);
        }

        //[HttpGet]
        // public IActionResult PegarProdutos()
        // {
        //     return Ok("kemylly"); //retorna o codigo 200 e se você quiser retorna dados tambem
        // }

        // [HttpGet] //mapeando a rota pelo metodo get
        // public IActionResult Get()  //pegar - nome padrao
        // {
        //     return Ok("kemylly"); //retorna o codigo 200 e se você quiser retorna dados tambem
        // }

        // [HttpGet] //mapeando a rota pelo metodo get
        // public IActionResult Get()  //pegar - nome padrao
        // {
        //     return Ok(new {nome = "kemylly", empresa = "GFT"}); //retorna o codigo 200 e se você quiser retorna dados tambem
        // }

        // [HttpGet("{id}")]
        // public IActionResult Get(int id)  //trazer a rota com um id
        // {
        //     return Ok("kemylly " + id); //retorna o codigo 200 e se você quiser retorna dados tambem
        // }

        [HttpGet] //mapeando a rota pelo metodo get
        public IActionResult Get()  //listagem de todos os ids
        {
            var produtos = database.Produtos.ToList();
            List<ProdutoContainer> produtosHateoas = new List<ProdutoContainer>();
            foreach(var prod in produtos)
            {
                ProdutoContainer produtoContainer = new ProdutoContainer();
                produtoContainer.produto = prod;
                produtoContainer.links = HATEOAS.GetActions(prod.Id.ToString());
                produtosHateoas.Add(produtoContainer);
            }
            return Ok(produtosHateoas); //retorna o codigo 200 e se você quiser retorna dados tambem
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)  //listagem de um id unico
        {
            try{ //tentar
                Produto produtos = database.Produtos.First(p => p.Id == id);
                ProdutoContainer produtoHateoas = new ProdutoContainer();
                produtoHateoas.produto = produtos;
                produtoHateoas.links = HATEOAS.GetActions(produtos.Id.ToString());
                return Ok(produtoHateoas); //retorna o codigo 200 e se você quiser retorna dados tambem
            }catch(Exception e){
                //Response.StatusCode = 404; //o retorno do status
                //return new ObjectResult("");
                return BadRequest(new {msg = "Id invalido"});
            }
            
        }

        // [HttpPost]
        // public IActionResult Post()
        // {
        //     return Ok("Esta tudo ok");
        // }

        // [HttpPost]
        // public IActionResult Post([FromBody] ProdutoTemp pTemp)
        // {
        //     return Ok(new {info = "Voce criou: ", produto = pTemp});
        // }

        // [HttpPost]
        // public IActionResult Post([FromBody] ProdutoTemp pTemp)
        // {
        //     Produto p = new Produto();

        //     p.Nome = pTemp.Nome;
        //     p.Preco = pTemp.Preco;

        //     database.Produtos.Add(p);
        //     database.SaveChanges();

        //     Response.StatusCode = 201; //o retorno do status
        //     return new ObjectResult("");
        //     //return Ok(new {info = "Voce criou um produto"});
        // }

        [HttpPost]
        public IActionResult Post([FromBody] ProdutoTemp pTemp)
        {
            /*validacao*/
            if(pTemp.Preco <= 0){
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Preco invalido"});
            }
            if(pTemp.Nome.Length <= 1){
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Nome de produto invalido"});
            }

            Produto p = new Produto();

            p.Nome = pTemp.Nome;
            p.Preco = pTemp.Preco;

            database.Produtos.Add(p);
            database.SaveChanges();

            Response.StatusCode = 201; //o retorno do status
            return new ObjectResult("");
            //return Ok(new {info = "Voce criou um produto"});
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id){
            try{ //tentar
                var produtos = database.Produtos.First(p => p.Id == id);
                database.Produtos.Remove(produtos);
                database.SaveChanges();
                return Ok(); //retorna o codigo 200 e se você quiser retorna dados tambem
            }catch(Exception e){
                //sResponse.StatusCode = 404; //o retorno do status
                //return new ObjectResult("");
                return BadRequest(new {msg = "Id invalido"});
            }
        }

        [HttpPatch]
        public IActionResult Patch([FromBody] Produto produto) //permite editar recursos parcialmente
        {
            if(produto.Id > 0){
                try{
                    var p = database.Produtos.First(ptemp => ptemp.Id == produto.Id);
                    
                    if(p != null){
                        //editar
                        //lugar para guardar = condicao ? se for true faz algo : se não faz outra coisa
                        //a = b > c ? b : a
                        p.Nome = produto.Nome != null ? produto.Nome : p.Nome;
                        p.Preco = produto.Preco != 0 ? produto.Preco : p.Preco;

                        database.SaveChanges();
                        return Ok();
                    }
                    else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Produto não encontrado"});
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Produto não encontrado"});
                }
            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "O id do produto é invalido"});
            }

        }

        public class ProdutoTemp{
            public string Nome {get; set;}
            public float Preco {get; set;}
        }

        public class ProdutoContainer
        {
            public Produto produto;
            public Link[] links;
        }
    }
}