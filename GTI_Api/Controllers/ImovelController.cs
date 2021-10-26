using GTI_Bll.Classes;
using GTI_Models.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GTI_Api.Controllers {
    public class ImovelController : ApiController {
        private readonly string _connection = "GTIconnection";


        // GET api/values
        [HttpGet]
        public HttpResponseMessage GetImovelId(int id) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);

            bool _existe = imovelRepository.Existe_Imovel(id);
            if (!_existe)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Imóvel não cadastrado.");
            else {
                Imovel_Full _imovel = imovelRepository.Dados_Imovel_Full(id);
                string jsonString = JsonConvert.SerializeObject(_imovel);
                return Request.CreateResponse(HttpStatusCode.OK, jsonString);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetImovelInscricao(int distrito) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);

            int _cod = imovelRepository.Existe_Imovel(distrito,1,1,6,0,0);
            if (_cod==0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Imóvel não cadastrado.");
            else {
                ImovelStruct _imovel = imovelRepository.Dados_Imovel(_cod);
                return Request.CreateResponse(HttpStatusCode.OK, _imovel);
            }
        }


    }
}
